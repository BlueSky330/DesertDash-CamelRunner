using UnityEngine;
using System.Collections;
using System.Text;

public class AdTester : MonoBehaviour
{
    [Header("Ad Test Settings")]
    public float testDelayBetweenAds = 2f; // Delay between simulating ad shows

    private StringBuilder report = new StringBuilder();

    void Start()
    {
        // StartAdTests(); // Call this from a UI button or dedicated test scene
    }

    public void StartAdTests()
    {
        Debug.Log("Starting Ad Test Suite...");
        report.Clear();
        report.AppendLine("--- Ad Test Report ---");

        // Subscribe to AdManager events for reward verification
        AdManager.onAdRewardGranted += OnAdRewardGranted;
        AdManager.onAdFailed += OnAdFailed;

        StartCoroutine(RunAdTests());
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        AdManager.onAdRewardGranted -= OnAdRewardGranted;
        AdManager.onAdFailed -= OnAdFailed;
    }

    private IEnumerator RunAdTests()
    {
        // Test 1: Simulate Quick Watch ad completion
        report.AppendLine("\n--- Test 1: Quick Watch Ad Completion ---");
        SimulateAdShow(AdManager.AdPurpose.QuickCoinReward, true);
        yield return new WaitForSeconds(testDelayBetweenAds);

        // Test 2: Simulate Standard Watch ad completion
        report.AppendLine("\n--- Test 2: Standard Watch Ad Completion ---");
        SimulateAdShow(AdManager.AdPurpose.StandardCoinReward, true);
        yield return new WaitForSeconds(testDelayBetweenAds);

        // Test 3: Simulate Premium Watch ad completion
        report.AppendLine("\n--- Test 3: Premium Watch Ad Completion ---");
        SimulateAdShow(AdManager.AdPurpose.PremiumCoinReward, true);
        yield return new WaitForSeconds(testDelayBetweenAds);

        // Test 4: Simulate Quick Watch ad skip (no reward)
        report.AppendLine("\n--- Test 4: Quick Watch Ad Skip ---");
        SimulateAdShow(AdManager.AdPurpose.QuickCoinReward, false); // Simulate skip/fail
        yield return new WaitForSeconds(testDelayBetweenAds);

        // Test 5: Simulate Health Restoration ad completion
        report.AppendLine("\n--- Test 5: Health Restoration Ad Completion ---");
        SimulateAdShow(AdManager.AdPurpose.HealthPremium, true);
        yield return new WaitForSeconds(testDelayBetweenAds);

        // Test 6: Simulate Game Over Continue ad completion
        report.AppendLine("\n--- Test 6: Game Over Continue Ad Completion ---");
        SimulateAdShow(AdManager.AdPurpose.GameOverContinue, true);
        yield return new WaitForSeconds(testDelayBetweenAds);

        GenerateReport();
        Debug.Log("Ad Test Suite Finished.");
    }

    private void SimulateAdShow(AdManager.AdPurpose purpose, bool shouldSucceed)
    {
        Debug.Log($"Simulating ad show for purpose: {purpose}, expecting success: {shouldSucceed}");
        // In a real scenario, this would call AdManager.Instance.ShowRewardedAd(purpose)
        // and the AdManager would then interact with AdSDKIntegration.
        // For testing, we directly simulate the outcome for verification.

        if (shouldSucceed)
        {
            // Directly call AdManager's internal reward granting logic for testing
            // This bypasses the actual ad SDK, assuming the SDK would call this on success.
            // AdManager.Instance.GrantReward(purpose); // This method is private, so we'll simulate via event
            AdManager.Instance.ShowRewardedAd(purpose); // This will trigger the simulated ad watch
        }
        else
        {
            // Simulate ad failure or skip
            AdManager.onAdFailed?.Invoke("Simulated ad skip/failure.");
        }
    }

    private void OnAdRewardGranted(AdManager.AdRewardType rewardType, float amount)
    {
        report.AppendLine($"  [SUCCESS] Ad Reward Granted: Type={rewardType}, Amount={amount}");
        Debug.Log($"AdTester: Reward Granted - Type: {rewardType}, Amount: {amount}");
    }

    private void OnAdFailed(string message)
    {
        report.AppendLine($"  [FAILURE] Ad Failed: {message}");
        Debug.LogWarning($"AdTester: Ad Failed - {message}");
    }

    private void GenerateReport()
    {
        report.AppendLine("\n--- Content Filtering Verification (Manual) ---");
        report.AppendLine("  Verify in AdMob dashboard (Blocking controls -> Content) that adult categories, gambling, alcohol, dating, and inappropriate ads are blocked.");
        report.AppendLine("  Verify 'Max Ad Content Rating' is set to G or PG in AdMob dashboard.");
        report.AppendLine("  Verify Unity Ads content rating is set to 'Everyone' or 'Family' in Unity Dashboard.");
        Debug.Log(report.ToString());
    }
}
