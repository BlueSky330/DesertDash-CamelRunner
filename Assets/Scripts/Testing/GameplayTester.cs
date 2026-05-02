using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameplayTester : MonoBehaviour
{
    [Header("Test Settings")]
    public float testDuration = 60f; // Duration of each simulated gameplay test run
    public int numberOfRuns = 5; // Number of times to run the full test suite

    private int currentRun = 0;
    private float timer = 0f;
    private bool isTesting = false;

    void Start()
    {
        // StartTests(); // Call this from a UI button or a dedicated test scene
    }

    void Update()
    {
        if (isTesting)
        {
            timer += Time.deltaTime;
            if (timer >= testDuration)
            {
                EndCurrentRun();
            }
            SimulatePlayerInput();
        }
    }

    public void StartTests()
    {
        Debug.Log("Starting Gameplay Test Suite...");
        currentRun = 0;
        StartNextRun();
    }

    private void StartNextRun()
    {
        if (currentRun < numberOfRuns)
        {
            currentRun++;
            Debug.Log($"--- Starting Test Run {currentRun}/{numberOfRuns} ---");
            ResetGameForTest();
            timer = 0f;
            isTesting = true;
        }
        else
        {
            Debug.Log("Gameplay Test Suite Finished.");
            isTesting = false;
        }
    }

    private void EndCurrentRun()
    {
        Debug.Log($"--- Ending Test Run {currentRun} ---");
        LogTestResults();
        StartNextRun();
    }

    private void ResetGameForTest()
    {
        // Reset all game systems for a fresh test run
        GameManager.Instance.ResetGame();
        HealthSystem.Instance.ResetHealth();
                CollectibleSystem.Instance.ResetScoreAndCoins();
                MilestoneSystem.Instance.ResetMilestones();
                PlayerController.Instance.ResetPosition();
        Debug.Log("Game systems reset for new test run.");
    }

    private void SimulatePlayerInput()
    {
        // Simulate random player actions: swipe left/right, jump, slide
        if (Random.value < 0.01f) // Small chance each frame to perform an action
        {
            float action = Random.value;
            if (action < 0.3f) // Swipe Left
            {
                // PlayerController.Instance.SwipeLeft();
                Debug.Log("Simulating Swipe Left");
            }
            else if (action < 0.6f) // Swipe Right
            {
                // PlayerController.Instance.SwipeRight();
                Debug.Log("Simulating Swipe Right");
            }
            else if (action < 0.8f) // Jump
            {
                // PlayerController.Instance.Jump();
                Debug.Log("Simulating Jump");
            }
            else // Slide
            {
                // PlayerController.Instance.Slide();
                Debug.Log("Simulating Slide");
            }
        }

        // Simulate power-up activation (if available)
        if (Random.value < 0.001f) // Very small chance
        {
            PowerUpManager.PowerUpType randomPowerUp = (PowerUpManager.PowerUpType)Random.Range(0, System.Enum.GetValues(typeof(PowerUpManager.PowerUpType)).Length);
            // PowerUpManager.Instance.ActivatePowerUp(randomPowerUp);
            Debug.Log($"Simulating Power-up Activation: {randomPowerUp}");
        }

        // Simulate thief interaction (if a thief is present)
        // This would require more complex interaction with ThiefSystem
    }

    private void LogTestResults()
    {
        Debug.Log($"--- Test Run {currentRun} Results ---");
        Debug.Log($"Final Score: {CollectibleSystem.Instance.currentScore}");
        Debug.Log($"Final Coins: {CollectibleSystem.Instance.currentCoins}");
        Debug.Log($"Final Health: {HealthSystem.Instance.currentHealth:F2}%");
        Debug.Log($"Milestones Reached: {MilestoneSystem.Instance.lastMilestoneIndex + 1}");
        // Add more detailed logging for specific events (collisions, power-ups, thief encounters)
        // Example: Check if health decay matches expected values
        // Example: Check if score-to-coin conversion happened correctly
    }

    // Helper to trigger game over scenarios
    public void TriggerGameOver(bool watchAdToContinue)
    {
        if (watchAdToContinue)
        {
            // AdManager.Instance.ShowRewardedAd(AdManager.AdPurpose.GameOverContinue);
            Debug.Log("Simulating Game Over: Watch Ad to Continue");
        }
        else
        {
            // GameManager.Instance.RestartFromMilestone();
            Debug.Log("Simulating Game Over: Restart from Milestone");
        }
    }
}
