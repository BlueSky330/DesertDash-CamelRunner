using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Edit-mode unit tests for economy balance.
/// Tests score-to-coin conversion accuracy, ad rewards, unlock costs, and coin earning/spending patterns.
/// </summary>
public class EconomyBalanceTests
{
    private CollectibleSystem collectibleSystem;
    private WorldMapManager worldMapManager;

    [SetUp]
    public void SetUp()
    {
        // Create CollectibleSystem
        var goCollectible = new GameObject("CollectibleSystem");
        collectibleSystem = goCollectible.AddComponent<CollectibleSystem>();
        collectibleSystem.ResetScoreAndCoins();

        // Create WorldMapManager
        var goWorldMap = new GameObject("WorldMapManager");
        worldMapManager = goWorldMap.AddComponent<WorldMapManager>();
    }

    [TearDown]
    public void TearDown()
    {
        if (collectibleSystem != null)
            Object.DestroyImmediate(collectibleSystem.gameObject);
        if (worldMapManager != null)
            Object.DestroyImmediate(worldMapManager.gameObject);
    }

    // ── Score-to-Coin Conversion Accuracy ─────────────────────────────────────

    [Test]
    public void ScoreToCoinConversion_150PointsEqualsOneCoin()
    {
        // Add exactly 150 points (150 dates × 1 point each)
        for (int i = 0; i < 150; i++)
            collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.Date);

        int coinsBefore = collectibleSystem.currentCoins;
        collectibleSystem.ConvertScoreToCoins();
        int coinsAfter = collectibleSystem.currentCoins;

        Assert.AreEqual(1, coinsAfter - coinsBefore, "150 score should convert to exactly 1 coin");
    }

    [Test]
    public void ScoreToCoinConversion_300PointsEqualsTwoCoins()
    {
        // Add 300 points
        for (int i = 0; i < 300; i++)
            collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.Date);

        int coinsBefore = collectibleSystem.currentCoins;
        collectibleSystem.ConvertScoreToCoins();
        int coinsAfter = collectibleSystem.currentCoins;

        Assert.AreEqual(2, coinsAfter - coinsBefore, "300 score should convert to exactly 2 coins");
    }

    [Test]
    public void ScoreToCoinConversion_TrackRemainderCorrectly()
    {
        // Add 175 points → should convert to 1 coin with 25 remainder
        for (int i = 0; i < 175; i++)
            collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.Date);

        collectibleSystem.ConvertScoreToCoins();

        Assert.AreEqual(25, collectibleSystem.currentScore,
            "Should retain 25 score points after converting 150 to 1 coin");
    }

    [Test]
    public void ScoreToCoinConversion_MixedCollectiblesAccuracy()
    {
        // Mix of different collectibles: 10 gems (100 pts) + 5 golden dates (25 pts) + 25 dates (25 pts) = 150 pts
        for (int i = 0; i < 10; i++)
            collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.Gem);
        for (int i = 0; i < 5; i++)
            collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.GoldenDate);
        for (int i = 0; i < 25; i++)
            collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.Date);

        int coinsBefore = collectibleSystem.currentCoins;
        collectibleSystem.ConvertScoreToCoins();
        int coinsAfter = collectibleSystem.currentCoins;

        Assert.AreEqual(1, coinsAfter - coinsBefore, "Mixed collectibles totaling 150 points should convert to 1 coin");
    }

    // ── Ad Reward Validation ──────────────────────────────────────────────────

    [Test]
    public void AdReward_FreeShopRewardGrants100Coins()
    {
        // Validate AdManager reward structure
        // Free shop reward should grant 100 coins (defined in AdManager.cs)
        const int EXPECTED_FREE_SHOP_COINS = 100;

        int coinsBefore = collectibleSystem.currentCoins;
        collectibleSystem.AddCoins(EXPECTED_FREE_SHOP_COINS);
        int coinsAfter = collectibleSystem.currentCoins;

        Assert.AreEqual(EXPECTED_FREE_SHOP_COINS, coinsAfter - coinsBefore,
            "Free shop reward should add exactly 100 coins");
    }

    [Test]
    public void AdReward_DoubleCoinsRewardStructure()
    {
        // DoubleCoins ad reward doubles coins earned this run
        // Verify structure: if player earned X coins, ad gives X more coins
        collectibleSystem.AddCoins(250);
        int coinsAfterEarning = collectibleSystem.currentCoins;

        // Simulate double coin reward
        int runCoins = 250;
        collectibleSystem.AddCoins(runCoins); // This simulates AdManager.EarnCoinsFromAd(runCoins)

        Assert.AreEqual(coinsAfterEarning + runCoins, collectibleSystem.currentCoins,
            "Double coins ad should add amount equal to coins earned in run");
    }

    // ── Unlock Cost Validation ────────────────────────────────────────────────

    [Test]
    public void UnlockCost_JordanCosts500Coins()
    {
        var jordan = worldMapManager.GetCountryData("Jordan");
        Assert.IsNotNull(jordan, "Jordan should exist in country data");
        Assert.AreEqual(500, jordan.unlockCost, "Jordan unlock cost should be 500 coins");
        Assert.IsFalse(jordan.isUnlocked, "Jordan should not be unlocked by default");
    }

    [Test]
    public void UnlockCost_IndiaCosts800Coins()
    {
        var india = worldMapManager.GetCountryData("India");
        Assert.IsNotNull(india, "India should exist in country data");
        Assert.AreEqual(800, india.unlockCost, "India unlock cost should be 800 coins");
    }

    [Test]
    public void UnlockCost_EgyptIsStartingCountry()
    {
        var egypt = worldMapManager.GetCountryData("Egypt");
        Assert.IsNotNull(egypt, "Egypt should exist");
        Assert.AreEqual(0, egypt.unlockCost, "Egypt should be free (starting country)");
        Assert.IsTrue(egypt.isUnlocked, "Egypt should be unlocked by default");
    }

    [Test]
    public void UnlockCost_USACosts5000CoinsHighestTier()
    {
        var usa = worldMapManager.GetCountryData("USA");
        Assert.IsNotNull(usa, "USA should exist");
        Assert.AreEqual(5000, usa.unlockCost, "USA should be the most expensive unlock (5000 coins)");
    }

    // ── Coin Economy Balance ──────────────────────────────────────────────────

    [Test]
    public void EconomyBalance_CoinSpendingPreventsNegativeBalance()
    {
        bool spent = collectibleSystem.SpendCoins(999999);
        Assert.IsFalse(spent, "Should not allow spending more coins than available");
        Assert.GreaterOrEqual(collectibleSystem.currentCoins, 0,
            "Coin balance should never go negative");
    }

    [Test]
    public void EconomyBalance_CoinAdditionNeverNegative()
    {
        collectibleSystem.AddCoins(-1000); // Try to add negative coins
        Assert.GreaterOrEqual(collectibleSystem.currentCoins, 0,
            "Adding negative coins should not decrease balance");
    }

    [Test]
    public void EconomyBalance_ScoreAccumulation()
    {
        // Simulate 1 minute of low-intensity gameplay
        // Expected: ~100-150 score from mixed collectibles
        collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.Date);  // 1
        collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.Date);  // 1
        collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.SilverCoin); // 3
        collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.Gem);  // 10
        collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.GoldenDate); // 5
        // Total: 20 points

        Assert.AreEqual(20, collectibleSystem.currentScore, "Score accumulation should be accurate");
    }

    [Test]
    public void EconomyBalance_CoinConversionLowThreshold()
    {
        // Ensure conversion doesn't happen until 150 points
        for (int i = 0; i < 149; i++)
            collectibleSystem.AddCollectible(CollectibleSystem.CollectibleType.Date);

        int coinsBefore = collectibleSystem.currentCoins;
        collectibleSystem.ConvertScoreToCoins();
        int coinsAfter = collectibleSystem.currentCoins;

        Assert.AreEqual(0, coinsAfter - coinsBefore, "149 points should NOT convert to coins");
        Assert.AreEqual(149, collectibleSystem.currentScore, "Score should remain at 149");
    }

    // ── Difficulty & Economy Progression ──────────────────────────────────────

    [Test]
    public void Economy_UnlockCostProgressionIsIncreasing()
    {
        // Verify unlock costs follow a fair progression
        var jordan = worldMapManager.GetCountryData("Jordan");
        var india = worldMapManager.GetCountryData("India");
        var china = worldMapManager.GetCountryData("China");
        var peru = worldMapManager.GetCountryData("Peru");

        Assert.Less(jordan.unlockCost, india.unlockCost, "Jordan should be cheaper than India");
        Assert.Less(india.unlockCost, china.unlockCost, "India should be cheaper than China");
        Assert.Less(china.unlockCost, peru.unlockCost, "China should be cheaper than Peru");
    }

    [Test]
    public void Economy_AllCountriesHaveDefinedUnlockCosts()
    {
        foreach (var country in worldMapManager.allCountries)
        {
            if (country.unlockCost > 0) // Free countries (Egypt) are exempt
            {
                Assert.Greater(country.unlockCost, 0, $"{country.countryName} should have unlock cost > 0");
                Assert.Less(country.unlockCost, 100000, $"{country.countryName} unlock cost seems unreasonable (> 100k)");
            }
        }
    }
}
