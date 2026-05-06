using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Edit-mode unit tests for PowerUpManager.
/// Validates state tracking for all four power-ups.
/// </summary>
public class PowerUpManagerTests
{
    private PowerUpManager pum;

    [SetUp]
    public void SetUp()
    {
        var go = new GameObject("PowerUpManager");
        pum = go.AddComponent<PowerUpManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(pum.gameObject);
    }

    // ── Scarab Shell ──────────────────────────────────────────────────────

    [Test]
    public void ScarabShell_ActiveAfterActivation()
    {
        pum.ActivatePowerUp(PowerUpManager.PowerUpType.ScarabShell);
        Assert.IsTrue(pum.HasScarabShield());
    }

    [Test]
    public void ScarabShell_InactiveAfterConsumption()
    {
        pum.ActivatePowerUp(PowerUpManager.PowerUpType.ScarabShell);
        pum.ConsumeScarabShield();
        Assert.IsFalse(pum.HasScarabShield());
    }

    [Test]
    public void ScarabShell_ConsumingWhenInactive_DoesNothing()
    {
        // Should not throw; just silently does nothing
        Assert.DoesNotThrow(() => pum.ConsumeScarabShield());
        Assert.IsFalse(pum.HasScarabShield());
    }

    // ── Golden Scarab coin multiplier ─────────────────────────────────────

    [Test]
    public void CoinMultiplier_DefaultIsOne()
    {
        Assert.AreEqual(1, pum.CoinMultiplier());
    }

    // ── Speed multiplier ─────────────────────────────────────────────────

    [Test]
    public void SpeedMultiplier_DefaultIsOne()
    {
        Assert.AreEqual(1f, pum.SpeedMultiplier());
    }

    // ── Initial states ───────────────────────────────────────────────────

    [Test]
    public void AllPowerUps_InactiveOnStart()
    {
        Assert.IsFalse(pum.isMagicCarpetActive);
        Assert.IsFalse(pum.isScarabShieldActive);
        Assert.IsFalse(pum.isOasisBreezeActive);
        Assert.IsFalse(pum.isGoldenScarabActive);
    }
}
