using UnityEngine;
using System;
// Uncomment when ironSource LevelPlay SDK is installed:
// using IronSource;
// using IronSourceRewardedVideoEvents;

/// <summary>
/// ironSource / LevelPlay mediation waterfall for Camel Runner.
///
/// Waterfall priority (configured in LevelPlay dashboard):
///   1. ironSource Rewarded Video (primary demand)
///   2. AdMob (demand partner via LevelPlay)
///   3. Unity Ads (demand partner via LevelPlay)
///
/// This class is the FIRST hop in AdManager's waterfall. If ironSource has no fill,
/// AdManager falls back to AdSDKIntegration (direct AdMob, then direct Unity Ads).
///
/// "Reward on 100% completion" is enforced by using ONLY the onRewardedVideoAdRewardedEvent
/// callback for reward dispatch — never onRewardedVideoAdClosedEvent.
///
/// SDK setup:
///   1. Import IronSource Unity SDK (LevelPlay) from ironSource dashboard.
///   2. Replace APP_KEY below with your LevelPlay app key.
///   3. Configure AdMob and Unity Ads as demand partners in LevelPlay dashboard.
///   4. Uncomment SDK calls below.
/// </summary>
public class AdMediation : MonoBehaviour
{
    public static AdMediation Instance { get; private set; }

    [Header("LevelPlay / ironSource")]
    public string ironSourceAppKey = "YOUR_LEVPLAY_APP_KEY"; // Replace before production

    [Header("Content Filter")]
    [Tooltip("Match ad_config.json — block adult/gambling/alcohol/dating.")]
    public string[] blockedCategories = { "Gambling", "Alcohol", "Dating", "Adult" };

    private bool _initialized;
    private AdManager.AdPurpose _pendingPurpose;
    private Action<bool, AdManager.AdPurpose> _completionCallback;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitializeIronSource();
    }

    // ── Callback registration (called by AdManager) ───────────────────────────

    public void SubscribeCompletionCallback(Action<bool, AdManager.AdPurpose> cb)
        => _completionCallback += cb;

    public void UnsubscribeCompletionCallback(Action<bool, AdManager.AdPurpose> cb)
        => _completionCallback -= cb;

    // ── Init ──────────────────────────────────────────────────────────────────

    private void InitializeIronSource()
    {
        Debug.Log("[AdMediation] Initializing ironSource LevelPlay…");

        // IronSource.Agent.setAdaptersDebug(Debug.isDebugBuild);
        // IronSource.Agent.shouldTrackNetworkState(true);
        // IronSource.Agent.init(ironSourceAppKey, IronSourceAdUnits.REWARDED_VIDEO);
        // IronSource.Agent.validateIntegration();

        // Wire reward events (ONLY rewardedEvent grants reward — closed/failed do not)
        // IronSourceRewardedVideoEvents.onAdOpenedEvent       += OnISAdOpened;
        // IronSourceRewardedVideoEvents.onAdClosedEvent       += OnISAdClosed;
        // IronSourceRewardedVideoEvents.onAdRewardedEvent     += OnISAdRewarded;   // 100% watched
        // IronSourceRewardedVideoEvents.onAdShowFailedEvent   += OnISAdShowFailed;
        // IronSourceRewardedVideoEvents.onAdAvailableEvent    += OnISAdAvailable;
        // IronSourceRewardedVideoEvents.onAdUnavailableEvent  += OnISAdUnavailable;

        _initialized = true;
        Debug.Log("[AdMediation] ironSource (simulated) initialized.");
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <returns>True if ironSource had fill and showed the ad; false → caller tries next.</returns>
    public bool ShowIronSourceRewardedAd(AdManager.AdPurpose purpose)
    {
        if (!_initialized)
        {
            Debug.LogWarning("[AdMediation] ironSource not initialized.");
            return false;
        }

        _pendingPurpose = purpose;
        Debug.Log($"[AdMediation] Requesting ironSource rewarded ad for: {purpose}");

        // if (IronSource.Agent.isRewardedVideoAvailable())
        // {
        //     IronSource.Agent.showRewardedVideo();
        //     return true;
        // }

        // Editor simulation
#if UNITY_EDITOR
        Debug.Log("[AdMediation] EDITOR: Simulating ironSource 100% completion.");
        _completionCallback?.Invoke(true, purpose);
        return true;
#else
        Debug.LogWarning("[AdMediation] ironSource: no fill.");
        return false;
#endif
    }

    // ── ironSource event handlers ─────────────────────────────────────────────

    // private void OnISAdOpened(IronSourceAdInfo info)
    //     => Debug.Log("[AdMediation] ironSource ad opened.");

    // private void OnISAdClosed(IronSourceAdInfo info)
    // {
    //     // Do NOT grant reward here — only OnISAdRewarded does that.
    //     Debug.Log("[AdMediation] ironSource ad closed (no reward on close).");
    // }

    // private void OnISAdRewarded(IronSourcePlacement placement, IronSourceAdInfo info)
    // {
    //     // SDK fires this ONLY after the user watches 100% of the ad.
    //     Debug.Log($"[AdMediation] ironSource reward earned: {placement.getRewardName()} x{placement.getRewardAmount()}");
    //     _completionCallback?.Invoke(true, _pendingPurpose);
    // }

    // private void OnISAdShowFailed(IronSourceError error, IronSourceAdInfo info)
    // {
    //     Debug.LogError($"[AdMediation] ironSource show failed: {error.getDescription()}");
    //     _completionCallback?.Invoke(false, _pendingPurpose);
    // }

    // private void OnISAdAvailable(IronSourceAdInfo info)
    //     => Debug.Log("[AdMediation] ironSource ad available.");

    // private void OnISAdUnavailable()
    //     => Debug.LogWarning("[AdMediation] ironSource ad unavailable (no fill).");

    // ── Application lifecycle (required by ironSource) ────────────────────────

    void OnApplicationPause(bool isPaused)
    {
        // IronSource.Agent.onApplicationPause(isPaused);
    }
}
