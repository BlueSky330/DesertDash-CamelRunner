using UnityEngine;
using System;
// Uncomment when SDKs are installed:
// using GoogleMobileAds.Api;
// using Unity.Services.Core;
// using Unity.Services.Mediation;

/// <summary>
/// Direct SDK integration for AdMob (primary) and Unity Ads (fallback).
/// Each Show* method returns true if an ad was presented, false if not ready.
///
/// CRITICAL: Rewards are fired ONLY from the SDK's OnUserEarnedReward / OnAdRewarded
/// callback — never from the OnAdClosed callback. This enforces the
/// "reward on 100% completion only" contract.
///
/// Setup:
///   AdMob  — add "com.google.mobile-ads-unity" package; replace test ad unit IDs.
///   UnityAds — add "com.unity.services.mediation" package; set Game ID in dashboard.
/// </summary>
public class AdSDKIntegration : MonoBehaviour
{
    public static AdSDKIntegration Instance { get; private set; }

    // ── Ad Unit IDs (test IDs — replace before production) ───────────────────

    [Header("AdMob Test IDs (Android)")]
    public string adMobAppId              = "ca-app-pub-3940256099942544~3347511713";
    public string adMobRewardedAdUnitId   = "ca-app-pub-3940256099942544/5224354917";

    [Header("AdMob Test IDs (iOS)")]
    public string adMobAppIdIOS           = "ca-app-pub-3940256099942544~1458002511";
    public string adMobRewardedAdUnitIdIOS = "ca-app-pub-3940256099942544/1712485313";

    [Header("Unity Ads")]
    public string unityAdsGameIdAndroid   = "1234567";
    public string unityAdsGameIdIOS       = "7654321";
    public string unityAdsRewardedUnitId  = "Rewarded_Android";

    // ── Internal state ────────────────────────────────────────────────────────

    private bool _adMobInitialized;
    private bool _unityAdsInitialized;
    private AdManager.AdPurpose _pendingPurpose;

    // Callback registered by AdManager
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
        InitializeAdMob();
        InitializeUnityAds();
    }

    // ── Callback registration (called by AdManager) ───────────────────────────

    public void SubscribeCompletionCallback(Action<bool, AdManager.AdPurpose> cb)
        => _completionCallback += cb;

    public void UnsubscribeCompletionCallback(Action<bool, AdManager.AdPurpose> cb)
        => _completionCallback -= cb;

    // ── AdMob ─────────────────────────────────────────────────────────────────

    private void InitializeAdMob()
    {
        Debug.Log("[AdSDK] Initializing Google Mobile Ads…");

        // MobileAds.Initialize(status =>
        // {
        //     // Content rating: max G — blocks adult/gambling/dating/alcohol
        //     var config = new RequestConfiguration.Builder()
        //         .SetMaxAdContentRating(MaxAdContentRating.G)
        //         .build();
        //     MobileAds.SetRequestConfiguration(config);
        //     _adMobInitialized = true;
        //     Debug.Log("[AdSDK] AdMob initialized.");
        //     LoadAdMobRewardedAd();
        // });

        _adMobInitialized = true; // editor simulation
        Debug.Log("[AdSDK] AdMob (simulated) initialized.");
        LoadAdMobRewardedAd();
    }

    private void LoadAdMobRewardedAd()
    {
        if (!_adMobInitialized) return;
        Debug.Log("[AdSDK] Loading AdMob rewarded ad…");

        // string unitId = Application.platform == RuntimePlatform.IPhonePlayer
        //     ? adMobRewardedAdUnitIdIOS : adMobRewardedAdUnitId;
        // RewardedAd.Load(unitId, new AdRequest(), (ad, error) =>
        // {
        //     if (error != null) { Debug.LogError("[AdSDK] AdMob load error: " + error); return; }
        //     _adMobRewardedAd = ad;
        //     // Wire reward callback — fires ONLY on 100% completion
        //     _adMobRewardedAd.OnAdFullScreenContentClosed += () => LoadAdMobRewardedAd();
        // });

        Debug.Log("[AdSDK] AdMob rewarded ad (simulated) ready.");
    }

    /// <returns>True if ad was shown; false if not loaded.</returns>
    public bool ShowAdMobRewardedAd(AdManager.AdPurpose purpose)
    {
        if (!_adMobInitialized)
        {
            Debug.LogWarning("[AdSDK] AdMob not initialized.");
            return false;
        }

        _pendingPurpose = purpose;
        Debug.Log($"[AdSDK] Showing AdMob rewarded ad for: {purpose}");

        // if (_adMobRewardedAd != null && _adMobRewardedAd.CanShowAd())
        // {
        //     _adMobRewardedAd.Show(reward =>
        //     {
        //         // This closure fires ONLY when the user earns the reward (100% watched)
        //         Debug.Log("[AdSDK] AdMob reward earned.");
        //         _completionCallback?.Invoke(true, _pendingPurpose);
        //         LoadAdMobRewardedAd(); // pre-load next
        //     });
        //     return true;
        // }

        // Editor/test simulation — invoke callback as if user watched 100%
#if UNITY_EDITOR
        Debug.Log("[AdSDK] EDITOR: Simulating AdMob 100% completion.");
        _completionCallback?.Invoke(true, purpose);
        return true;
#else
        Debug.LogWarning("[AdSDK] AdMob rewarded ad not ready.");
        return false;
#endif
    }

    // ── Unity Ads ─────────────────────────────────────────────────────────────

    private void InitializeUnityAds()
    {
        Debug.Log("[AdSDK] Initializing Unity Ads…");
        // string gameId = Application.platform == RuntimePlatform.IPhonePlayer
        //     ? unityAdsGameIdIOS : unityAdsGameIdAndroid;
        // Advertisement.Initialize(gameId, testMode: false, this);
        _unityAdsInitialized = true;
        Debug.Log("[AdSDK] Unity Ads (simulated) initialized.");
    }

    /// <returns>True if ad was shown; false if not ready.</returns>
    public bool ShowUnityAdsRewardedAd(AdManager.AdPurpose purpose)
    {
        if (!_unityAdsInitialized)
        {
            Debug.LogWarning("[AdSDK] Unity Ads not initialized.");
            return false;
        }

        _pendingPurpose = purpose;

        // if (Advertisement.IsReady(unityAdsRewardedUnitId))
        // {
        //     Advertisement.Show(unityAdsRewardedUnitId, new ShowOptions
        //     {
        //         resultCallback = result =>
        //         {
        //             // ShowResult.Finished = user watched 100% (SDK guarantee)
        //             bool completed = result == ShowResult.Finished;
        //             _completionCallback?.Invoke(completed, _pendingPurpose);
        //         }
        //     });
        //     return true;
        // }

#if UNITY_EDITOR
        Debug.Log("[AdSDK] EDITOR: Simulating Unity Ads 100% completion.");
        _completionCallback?.Invoke(true, purpose);
        return true;
#else
        Debug.LogWarning("[AdSDK] Unity Ads not ready.");
        return false;
#endif
    }
}
