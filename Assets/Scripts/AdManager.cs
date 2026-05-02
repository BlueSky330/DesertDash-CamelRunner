using UnityEngine;
using System.Collections;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    public delegate void OnAdRewardGranted(AdRewardType rewardType, float amount);
    public static event OnAdRewardGranted onAdRewardGranted;

    public delegate void OnAdFailed(string message);
    public static event OnAdFailed onAdFailed;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Placeholder for actual Ad SDK integration (e.g., Google AdMob, Unity Ads)
    // In a real scenario, these would call the respective SDK methods.

    public void ShowRewardedAd(AdPurpose purpose)
    {
        Debug.Log($"Attempting to show rewarded ad for purpose: {purpose}");
        // Simulate ad watching
        StartCoroutine(SimulateAdWatch(purpose));
    }

    private IEnumerator SimulateAdWatch(AdPurpose purpose)
    {
        // Simulate ad duration
        float adDuration = 0f;
        switch (purpose)
        {
            case AdPurpose.QuickCoinReward:
            case AdPurpose.HealthQuick:
                adDuration = 15f; // ~15 seconds
                break;
            case AdPurpose.StandardCoinReward:
            case AdPurpose.HealthStandard:
                adDuration = 30f; // ~30 seconds
                break;
            case AdPurpose.PremiumCoinReward:
            case AdPurpose.HealthPremium:
            case AdPurpose.GameOverContinue:
                adDuration = 60f; // ~60 seconds
                break;
        }

        Debug.Log($"Simulating ad watch for {adDuration} seconds...");
        yield return new WaitForSeconds(adDuration); // Wait for simulated ad to finish

        // Simulate ad completion success/failure
        if (Random.value > 0.1f) // 90% chance of success
        {
            GrantReward(purpose);
        }
        else
        {
            onAdFailed?.Invoke("Ad failed to load or was skipped.");
            Debug.LogWarning("Ad failed or was skipped, no reward granted.");
        }
    }

    private void GrantReward(AdPurpose purpose)
    {
        switch (purpose)
        {
            case AdPurpose.QuickCoinReward:
                // Coins x3 multiplier (enough for ~3 minutes of play)
                // Assuming 30-50 coins/min spend, 3 mins = 90-150 coins. Let's use 120 as average.
                CollectibleSystem.Instance.AddCoins(120);
                onAdRewardGranted?.Invoke(AdRewardType.Coins, 120);
                Debug.Log("Granted Quick Coin Reward: 120 coins.");
                break;
            case AdPurpose.StandardCoinReward:
                // Coins x4 multiplier (enough for ~5-7 minutes of play)
                // 5-7 mins = 150-350 coins. Let's use 250 as average.
                CollectibleSystem.Instance.AddCoins(250);
                onAdRewardGranted?.Invoke(AdRewardType.Coins, 250);
                Debug.Log("Granted Standard Coin Reward: 250 coins.");
                break;
            case AdPurpose.PremiumCoinReward:
                // Coins x4 multiplier + bonus power-up (enough for ~7-10 minutes of play)
                // 7-10 mins = 210-500 coins. Let's use 400 as average.
                CollectibleSystem.Instance.AddCoins(400);
                PowerUpManager.Instance.ActivatePowerUp(PowerUpManager.PowerUpType.MagicCarpet); // Example bonus power-up
                onAdRewardGranted?.Invoke(AdRewardType.CoinsAndPowerUp, 400);
                Debug.Log("Granted Premium Coin Reward: 400 coins + Power-up.");
                break;
            case AdPurpose.HealthQuick:
                HealthSystem.Instance.RecoverFromQuickAd();
                onAdRewardGranted?.Invoke(AdRewardType.Health, HealthSystem.Instance.quickAdRecovery);
                Debug.Log("Granted Health Quick Ad Reward.");
                break;
            case AdPurpose.HealthStandard:
                HealthSystem.Instance.RecoverFromStandardAd();
                onAdRewardGranted?.Invoke(AdRewardType.Health, HealthSystem.Instance.standardAdRecovery);
                Debug.Log("Granted Health Standard Ad Reward.");
                break;
            case AdPurpose.HealthPremium:
                HealthSystem.Instance.RecoverFromPremiumAd();
                onAdRewardGranted?.Invoke(AdRewardType.Health, HealthSystem.Instance.premiumAdRecovery);
                Debug.Log("Granted Health Premium Ad Reward.");
                break;
            case AdPurpose.GameOverContinue:
                HealthSystem.Instance.RecoverFromPremiumAd(); // Full health
                GameManager.Instance.ContinueGame(); // Resume game from where it stopped
                onAdRewardGranted?.Invoke(AdRewardType.ContinueGame, 0);
                Debug.Log("Granted Game Over Continue Reward.");
                break;
        }
    }

    public enum AdPurpose
    {
        QuickCoinReward,
        StandardCoinReward,
        PremiumCoinReward,
        HealthQuick,
        HealthStandard,
        HealthPremium,
        GameOverContinue,
        UnlockCountry // For future use if country unlock requires ad
    }

    public enum AdRewardType
    {
        Coins,
        CoinsAndPowerUp,
        Health,
        ContinueGame
    }
}
