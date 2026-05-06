using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Edit-mode unit tests for HealthSystem.
/// Validates decay thresholds and recovery methods.
/// </summary>
public class HealthSystemTests
{
    private HealthSystem health;

    [SetUp]
    public void SetUp()
    {
        var go = new GameObject("HealthSystem");
        health = go.AddComponent<HealthSystem>();
        health.ResetHealth();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(health.gameObject);
    }

    [Test]
    public void InitialHealth_Is100()
    {
        Assert.AreEqual(100f, health.currentHealth);
    }

    [Test]
    public void TakeDamage_ReducesHealth()
    {
        health.TakeDamage(25f);
        Assert.AreEqual(75f, health.currentHealth);
    }

    [Test]
    public void TakeDamage_CannotGoBelowZero()
    {
        health.TakeDamage(200f);
        Assert.AreEqual(0f, health.currentHealth);
    }

    [Test]
    public void RecoverHealth_IncreasesHealth()
    {
        health.TakeDamage(50f);
        health.RecoverHealth(20f);
        Assert.AreEqual(70f, health.currentHealth);
    }

    [Test]
    public void RecoverHealth_CannotExceedMax()
    {
        health.RecoverHealth(9999f);
        Assert.AreEqual(100f, health.currentHealth);
    }

    [Test]
    public void IsLowHealth_TrueBelow25()
    {
        health.TakeDamage(76f); // health = 24
        Assert.IsTrue(health.IsLowHealth());
    }

    [Test]
    public void IsLowHealth_FalseAtOrAbove25()
    {
        health.TakeDamage(75f); // health = 25
        Assert.IsFalse(health.IsLowHealth());
    }

    [Test]
    public void ResetHealth_RestoresTo100()
    {
        health.TakeDamage(80f);
        health.ResetHealth();
        Assert.AreEqual(100f, health.currentHealth);
    }

    [Test]
    public void DecayRate_InitialPeriod_IsCorrectPerSecond()
    {
        // 1%/min = 0.01667%/s
        float expectedPerSecond = health.initialDecayRatePerMinute / 60f;
        Assert.That(expectedPerSecond, Is.EqualTo(1f / 60f).Within(0.0001f));
    }

    [Test]
    public void DecayRate_AcceleratedPeriod_IsCorrectPerSecond()
    {
        // 2.5%/min = 0.04167%/s
        float expectedPerSecond = health.acceleratedDecayRatePerMinute / 60f;
        Assert.That(expectedPerSecond, Is.EqualTo(2.5f / 60f).Within(0.0001f));
    }

    [Test]
    public void EnvironmentalRecovery_GrassRestores10()
    {
        health.TakeDamage(50f);
        health.RecoverFromGrass();
        Assert.AreEqual(60f, health.currentHealth);
    }

    [Test]
    public void EnvironmentalRecovery_WaterRestores15()
    {
        health.TakeDamage(50f);
        health.RecoverFromWater();
        Assert.AreEqual(65f, health.currentHealth);
    }

    [Test]
    public void EnvironmentalRecovery_OasisRestores40()
    {
        health.TakeDamage(50f);
        health.RecoverFromOasis();
        Assert.AreEqual(90f, health.currentHealth);
    }
}
