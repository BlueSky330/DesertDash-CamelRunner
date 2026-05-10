using UnityEngine;
using System;

/// <summary>
/// Orchestrates rewarded ad flow for Camel Runner.
///
/// Flow:
///   1. Caller invokes ShowRewardedAd(purpose).
///   2. AdManager requests an ad via the mediation waterfall:
///      ironSource (LevelPlay) → AdMob → Unity Ads.
///   3. SDK fires a completion callback ONLY when the user watches 100% of the ad.
///      Skipped or failed ads do NOT grant a reward.
///   4. On 100% completion, GrantReward() dispatches the appropriate reward.
///
/// "100% completion" is enforced by the SDK: AdSDKIntegration and AdMediation
/// only raise OnRewardedAdFinished(completed: true) from the SDK's reward callback
/// — never from a close or skip callback.
/// </summary>
public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    // ── Events ────────────────────────────────────────────────────────────────

    public static event Action<AdPurpose, AdRewardType, float> OnAdRewardGranted;
    public static event Action<AdPurpose, string> OnAdFailed;

    // ── Ad purpose / reward types ─────────────────────────────────────────────

    public enum AdPurpose
    {
        /// <summary>Continue after game-over (revive).</summary>
        Revive,
        /// <summary>Double coins earned at end of run.</summary>
        DoubleCoins,
        /// <summary>Restore player health mid-run.</summary>
        HealthRestore,
        /// <summary>Unlock a free reward in the shop.</summary>
        FreeShopReward
    }

    public enum AdRewardType
    {
        ContinueGame,
        Coins,
        Health,
        ShopItem
    }

    // ── Ad frequency throttle ─────────────────────────────────────────────────

    [Header("Ad Frequency")]
    [Tooltip("Minimum seconds between rewarded ads.")]
    public float minSecondsBetweenAds = 30f;
    private float _lastAdShownAt = -999f;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Subscribe after all Awake() calls — AdSDKIntegration.Instance is guaranteed
        // to exist by this point, so the subscription is never silently skipped.
        AdSDKIntegration.Instance?.SubscribeCompletionCallback(HandleAdCompleted);
        AdMediation.Instance?.SubscribeCompletionCallback(HandleAdCompleted);
    }

    void OnDestroy()
    {
        AdSDKIntegration.Instance?.UnsubscribeCompletionCallback(HandleAdCompleted);
        AdMediation.Instance?.UnsubscribeCompletionCallback(HandleAdCompleted);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Request a rewarded ad for the given purpose.
    /// Returns false if no ad is available or throttled.
    /// </summary>
    public bool ShowRewardedAd(AdPurpose purpose)
    {
        if (!CanShowAd(purpose)) return false;

        // If the player owns No-Ads pass, skip non-critical ads but still allow
        // opt-in rewarded ads that the player explicitly requests.
        // (All purposes here are opt-in, so they pass regardless of no-ads pass.)

        Debug.Log($"[AdManager] Requesting rewarded ad for: {purpose}");

        // Waterfall: ironSource (LevelPlay) → AdMob → Unity Ads
        if (AdMediation.Instance != null && AdMediation.Instance.ShowIronSourceRewardedAd(purpose))
            return true;

        if (AdSDKIntegration.Instance != null && AdSDKIntegration.Instance.ShowAdMobRewardedAd(purpose))
            return true;

        if (AdSDKIntegration.Instance != null && AdSDKIntegration.Instance.ShowUnityAdsRewardedAd(purpose))
            return true;

        Debug.LogWarning($"[AdManager] No ad available for {purpose}.");
        OnAdFailed?.Invoke(purpose, "No ad available in waterfall.");
        return false;
    }

    // ── Completion callback (called by SDK layers) ────────────────────────────

    /// <summary>
    /// Called by AdSDKIntegration / AdMediation when an ad concludes.
    /// completed == true ONLY when the user watched 100% (SDK reward callback).
    /// completed == false means skipped or failed — no reward is granted.
    /// </summary>
    public void HandleAdCompleted(bool completed, AdPurpose purpose)
    {
        if (!completed)
        {
            Debug.LogWarning($"[AdManager] Ad for '{purpose}' was skipped or failed — no reward.");
            OnAdFailed?.Invoke(purpose, "Ad not completed.");
            return;
        }

        _lastAdShownAt = Time.realtimeSinceStartup;
        GrantReward(purpose);
    }

    // ── Reward dispatch ───────────────────────────────────────────────────────

    private void GrantReward(AdPurpose purpose)
    {
        switch (purpose)
        {
            case AdPurpose.Revive:
                // Restore full health and resume the run
                HealthSystem.Instance.RecoverFromPremiumAd();
                GameManager.Instance.ContinueGame();
                OnAdRewardGranted?.Invoke(purpose, AdRewardType.ContinueGame, 0);
                Debug.Log("[AdManager] Reward: Revive — full health + continue.");
                break;

            case AdPurpose.DoubleCoins:
                // Double the coins earned this run (called at run-end screen)
                int runCoins = CollectibleSystem.Instance.currentCoins;
                CoinEconomy.Instance.EarnCoinsFromAd(runCoins); // adds equal amount → 2×
                OnAdRewardGranted?.Invoke(purpose, AdRewardType.Coins, runCoins);
                Debug.Log($"[AdManager] Reward: Double coins — +{runCoins} coins.");
                break;

            case AdPurpose.HealthRestore:
                // Restore partial health mid-run
                HealthSystem.Instance.RecoverFromStandardAd();
                float restored = HealthSystem.Instance.standardAdRecovery;
                OnAdRewardGranted?.Invoke(purpose, AdRewardType.Health, restored);
                Debug.Log($"[AdManager] Reward: Health restore — +{restored} health.");
                break;

            case AdPurpose.FreeShopReward:
                // Grant a fixed coin bonus redeemable in the shop
                const int FREE_SHOP_COINS = 100;
                CoinEconomy.Instance.EarnCoinsFromAd(FREE_SHOP_COINS);
                OnAdRewardGranted?.Invoke(purpose, AdRewardType.ShopItem, FREE_SHOP_COINS);
                Debug.Log($"[AdManager] Reward: Free shop reward — +{FREE_SHOP_COINS} coins.");
                break;
        }
    }

    // ── Throttle / availability helpers ──────────────────────────────────────

    private bool CanShowAd(AdPurpose purpose)
    {
        if (OfflineManager.Instance == null || !OfflineManager.Instance.CanAccessOnlineFeatures())
        {
            Debug.LogWarning("[AdManager] Cannot show ad: offline or OfflineManager not ready.");
            return false;
        }

        float elapsed = Time.realtimeSinceStartup - _lastAdShownAt;
        if (elapsed < minSecondsBetweenAds)
        {
            Debug.LogWarning($"[AdManager] Ad throttled — {minSecondsBetweenAds - elapsed:F0}s remaining.");
            return false;
        }

        return true;
    }
}
