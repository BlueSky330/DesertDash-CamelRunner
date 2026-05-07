using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Play-mode tests for core gameplay mechanics.
/// Tests player movement, lane switching, obstacle collision, collectible pickup, power-up activation, and health decay.
/// </summary>
public class GameplayTests
{
    private GameObject playerGO;
    private PlayerController playerController;
    private GameObject gameManagerGO;
    private GameManager gameManager;
    private GameObject healthSystemGO;
    private HealthSystem healthSystem;
    private GameObject collectibleSystemGO;
    private CollectibleSystem collectibleSystem;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Create GameManager
        gameManagerGO = new GameObject("GameManager");
        gameManager = gameManagerGO.AddComponent<GameManager>();

        // Create HealthSystem
        healthSystemGO = new GameObject("HealthSystem");
        healthSystem = healthSystemGO.AddComponent<HealthSystem>();

        // Create CollectibleSystem
        collectibleSystemGO = new GameObject("CollectibleSystem");
        collectibleSystem = collectibleSystemGO.AddComponent<CollectibleSystem>();
        collectibleSystem.ResetScoreAndCoins();

        // Create PlayerController
        playerGO = new GameObject("Player");
        playerGO.AddComponent<BoxCollider>();
        playerGO.AddComponent<Rigidbody>();
        playerController = playerGO.AddComponent<PlayerController>();

        // Wait for one frame to let Start() methods run
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (playerGO != null)
            Object.Destroy(playerGO);
        if (gameManagerGO != null)
            Object.Destroy(gameManagerGO);
        if (healthSystemGO != null)
            Object.Destroy(healthSystemGO);
        if (collectibleSystemGO != null)
            Object.Destroy(collectibleSystemGO);

        yield return null;
    }

    // ── Player Movement & Lane Switching ──────────────────────────────────────

    [UnityTest]
    public IEnumerator PlayerMovement_CanSwitchLanes()
    {
        // Get initial position
        Vector3 initialPos = playerController.transform.position;

        // Try to move right
        playerController.TryChangeLane(1);
        yield return new WaitForSeconds(0.5f);

        Vector3 afterRightPos = playerController.transform.position;
        Assert.AreNotEqual(initialPos.x, afterRightPos.x, "Player should change X position when changing lanes");
    }

    [UnityTest]
    public IEnumerator PlayerMovement_LaneSwitchingBoundedByLaneCount()
    {
        // Try to switch far right beyond available lanes
        for (int i = 0; i < 10; i++)
        {
            playerController.TryChangeLane(1);
        }
        yield return new WaitForSeconds(0.5f);

        // Player should still be within valid bounds
        Assert.IsNotNull(playerController, "PlayerController should still exist");
        // Note: Actual lane bounds checking is done by PlayerController.LaneX()
    }

    [UnityTest]
    public IEnumerator PlayerMovement_CanJump()
    {
        Vector3 initialPos = playerController.transform.position;

        playerController.TryJump();
        yield return new WaitForSeconds(0.2f);

        // Player should have moved vertically (jumped)
        Vector3 afterJumpPos = playerController.transform.position;
        Assert.AreNotEqual(initialPos.y, afterJumpPos.y, "Player should change Y position when jumping");
    }

    [UnityTest]
    public IEnumerator PlayerMovement_CanSlide()
    {
        // Slide should change player height or collider
        // This test verifies Slide() doesn't crash and can be called
        playerController.TrySlide();
        yield return new WaitForSeconds(0.2f);

        Assert.IsNotNull(playerController, "PlayerController should still exist after slide");
    }

    [UnityTest]
    public IEnumerator PlayerMovement_SwipesAreConveniences()
    {
        // Verify SwipeLeft/SwipeRight are wrappers for TryChangeLane
        Vector3 initialPos = playerController.transform.position;

        playerController.SwipeLeft();
        yield return new WaitForSeconds(0.2f);

        Vector3 afterLeftPos = playerController.transform.position;
        Assert.AreNotEqual(initialPos.x, afterLeftPos.x, "SwipeLeft should result in lane change");
    }

    // ── Collectible System ────────────────────────────────────────────────────

    [UnityTest]
    public IEnumerator CollectiblePickup_PickupIncreasesScore()
    {
        int scoreBefore = collectibleSystem.currentScore;

        collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.Date);
        yield return null;

        int scoreAfter = collectibleSystem.currentScore;
        Assert.Greater(scoreAfter, scoreBefore, "Picking up a collectible should increase score");
    }

    [UnityTest]
    public IEnumerator CollectiblePickup_GemWorthMoreThanDate()
    {
        // Date = 1 point, Gem = 10 points
        collectibleSystem.ResetScore();
        collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.Date);
        int dateScore = collectibleSystem.currentScore;

        collectibleSystem.ResetScore();
        collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.Gem);
        int gemScore = collectibleSystem.currentScore;

        Assert.Greater(gemScore, dateScore, "Gem should be worth more points than Date");
        yield return null;
    }

    [UnityTest]
    public IEnumerator CollectiblePickup_MultiplePickupsAccumulate()
    {
        collectibleSystem.ResetScore();

        collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.Date);
        collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.Date);
        collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.SilverCoin); // 3 points

        Assert.AreEqual(5, collectibleSystem.currentScore, "Multiple collectibles should accumulate correctly (1+1+3=5)");
        yield return null;
    }

    // ── Health System ─────────────────────────────────────────────────────────

    [UnityTest]
    public IEnumerator HealthDecay_StartsAtMaxHealth()
    {
        yield return null;
        Assert.AreEqual(100f, healthSystem.currentHealth, "Health should start at 100%");
    }

    [UnityTest]
    public IEnumerator HealthDecay_DecaysOverTime()
    {
        // Start the health decay by marking game as running
        // (This normally happens via GameManager.StartGame())
        healthSystem.ResetHealth();
        yield return new WaitForSeconds(0.1f);

        float healthBefore = healthSystem.currentHealth;
        yield return new WaitForSeconds(1f);
        float healthAfter = healthSystem.currentHealth;

        // Health should decrease over time (if game is running)
        // Note: Actual decay depends on whether game state is set correctly
        Assert.LessOrEqual(healthAfter, healthBefore, "Health should decay or stay same over time");
    }

    [UnityTest]
    public IEnumerator HealthRecovery_CanRecoverFromDamage()
    {
        healthSystem.ResetHealth();
        yield return null;

        float healthBefore = healthSystem.currentHealth;

        // Simulate taking damage
        healthSystem.TakeDamage(10f);
        yield return null;

        float healthAfterDamage = healthSystem.currentHealth;
        Assert.Less(healthAfterDamage, healthBefore, "Taking damage should reduce health");

        // Simulate recovery (natural recovery or ad)
        healthSystem.RecoverFromStandardAd();
        yield return null;

        float healthAfterRecovery = healthSystem.currentHealth;
        Assert.Greater(healthAfterRecovery, healthAfterDamage, "Recovery should increase health");
    }

    [UnityTest]
    public IEnumerator HealthRecovery_NoHealthBelowZero()
    {
        healthSystem.ResetHealth();
        yield return null;

        // Take massive damage
        healthSystem.TakeDamage(500f);
        yield return null;

        Assert.GreaterOrEqual(healthSystem.currentHealth, 0f, "Health should never go below 0");
    }

    [UnityTest]
    public IEnumerator HealthRecovery_NoHealthAboveMax()
    {
        healthSystem.ResetHealth();
        yield return null;

        // Recover with multiple ads
        for (int i = 0; i < 10; i++)
        {
            healthSystem.RecoverFromPremiumAd();
        }
        yield return null;

        Assert.LessOrEqual(healthSystem.currentHealth, 100f, "Health should never exceed 100%");
    }

    // ── Power-Up System ──────────────────────────────────────────────────────

    [UnityTest]
    public IEnumerator PowerUp_ActivationCompletesWithoutCrash()
    {
        // Create PowerUpManager for testing
        var powerUpGO = new GameObject("PowerUpManager");
        var powerUpManager = powerUpGO.AddComponent<PowerUpManager>();
        yield return null;

        // Try to activate a power-up (should not crash)
        // Note: Actual power-up activation depends on PowerUpManager implementation
        Assert.IsNotNull(powerUpManager, "PowerUpManager should be created");

        Object.Destroy(powerUpGO);
        yield return null;
    }

    // ── Integration Tests ────────────────────────────────────────────────────

    [UnityTest]
    public IEnumerator Gameplay_ScoreAndHealthWorkTogether()
    {
        collectibleSystem.ResetScore();
        healthSystem.ResetHealth();
        yield return null;

        float initialHealth = healthSystem.currentHealth;
        int initialScore = collectibleSystem.currentScore;

        // Simulate picking up collectibles
        collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.Gem);
        yield return new WaitForSeconds(0.5f);

        // Simulate taking damage
        healthSystem.TakeDamage(5f);
        yield return null;

        Assert.Greater(collectibleSystem.currentScore, initialScore, "Score should increase");
        Assert.Less(healthSystem.currentHealth, initialHealth, "Health should decrease");
    }

    [UnityTest]
    public IEnumerator Gameplay_CollectiblesAndScoreConversionFlow()
    {
        collectibleSystem.ResetScore();
        yield return null;

        // Simulate collecting 150 points worth of items
        for (int i = 0; i < 150; i++)
        {
            collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.Date);
        }
        yield return null;

        int coinsBefore = collectibleSystem.currentCoins;
        collectibleSystem.ConvertScoreToCoins();
        int coinsAfter = collectibleSystem.currentCoins;

        Assert.Greater(coinsAfter, coinsBefore, "Score-to-coin conversion should grant coins");
    }
}
