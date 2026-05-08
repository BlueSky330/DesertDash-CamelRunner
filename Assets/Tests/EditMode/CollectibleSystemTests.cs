using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Edit-mode unit tests for CollectibleSystem.
/// Uses a fresh GameObject per test to isolate state.
/// </summary>
public class CollectibleSystemTests
{
    private CollectibleSystem system;

    [SetUp]
    public void SetUp()
    {
        var go = new GameObject("CollectibleSystem");
        system = go.AddComponent<CollectibleSystem>();
        // Manually call Start logic by invoking the public reset method
        system.ResetScoreAndCoins();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(system.gameObject);
        // Clear static instance to avoid cross-test contamination
        CollectibleSystem.Instance?.gameObject.SetActive(false);
    }

    // ── Point values ──────────────────────────────────────────────────────

    [Test]
    public void AddDate_AddsCorrectPoints()
    {
        system.AddCollectible(CollectibleSystem.CollectibleType.Date);
        Assert.AreEqual(1, system.currentScore);
    }

    [Test]
    public void AddSilverCoin_AddsCorrectPoints()
    {
        system.AddCollectible(CollectibleSystem.CollectibleType.SilverCoin);
        Assert.AreEqual(3, system.currentScore);
    }

    [Test]
    public void AddGem_AddsCorrectPoints()
    {
        system.AddCollectible(CollectibleSystem.CollectibleType.Gem);
        Assert.AreEqual(10, system.currentScore);
    }

    [Test]
    public void AddGoldenDate_AddsCorrectPoints()
    {
        system.AddCollectible(CollectibleSystem.CollectibleType.GoldenDate);
        Assert.AreEqual(5, system.currentScore);
    }

    [Test]
    public void AddMysteryBox_AddsPointsInRange()
    {
        system.AddCollectible(CollectibleSystem.CollectibleType.MysteryBox);
        Assert.That(system.currentScore, Is.InRange(15, 50));
    }

    // ── Score → coin conversion ───────────────────────────────────────────

    [Test]
    public void ConvertScoreToCoins_Converts150PointsToOneCoin()
    {
        // Add 150 points manually via multiple Date pickups
        for (int i = 0; i < 150; i++)
            system.AddCollectible(CollectibleSystem.CollectibleType.Date);

        int coinsBefore = system.currentCoins;
        system.ConvertScoreToCoins();

        Assert.AreEqual(1, system.currentCoins - coinsBefore,
            "150 score should convert to exactly 1 coin");
        Assert.AreEqual(0, system.currentScore,
            "Score remainder after 150-point conversion should be 0");
    }

    [Test]
    public void ConvertScoreToCoins_KeepsRemainder()
    {
        // 160 points → 1 coin, 10 points remain
        for (int i = 0; i < 160; i++)
            system.AddCollectible(CollectibleSystem.CollectibleType.Date);

        system.ConvertScoreToCoins();

        Assert.AreEqual(10, system.currentScore, "10 leftover score should remain after conversion");
    }

    [Test]
    public void ConvertScoreToCoins_BelowThreshold_NoConversion()
    {
        for (int i = 0; i < 100; i++)
            system.AddCollectible(CollectibleSystem.CollectibleType.Date);

        int coinsBefore = system.currentCoins;
        system.ConvertScoreToCoins();

        Assert.AreEqual(coinsBefore, system.currentCoins, "No coins should be added if score < 150");
    }

    // ── Coin operations ───────────────────────────────────────────────────

    [Test]
    public void SpendCoins_SucceedsWhenSufficient()
    {
        system.AddCoins(500);
        system.AddCoins(100);
        bool result = system.SpendCoins(50);
        Assert.IsTrue(result);
        Assert.AreEqual(system.currentCoins, 500 + 100 - 50); // 500 starter + 100 added - 50 spent
    }

    [Test]
    public void SpendCoins_FailsWhenInsufficient()
    {
        bool result = system.SpendCoins(10000);
        Assert.IsFalse(result);
    }

    [Test]
    public void ResetScoreAndCoins_RestoresToDefaults()
    {
        system.AddCollectible(CollectibleSystem.CollectibleType.Gem);
        system.SpendCoins(100);
        system.ResetScoreAndCoins();

        Assert.AreEqual(0, system.currentScore);
        Assert.AreEqual(500, system.currentCoins); // starter bonus
    }
}
