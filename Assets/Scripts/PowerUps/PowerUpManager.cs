using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Manages the four gameplay power-ups:
///   Magic Carpet  — speed boost + invincibility for duration
///   Scarab Shell  — absorbs exactly one obstacle collision then expires
///   Oasis Breeze  — magnet: pulls nearby collectibles toward player
///   Golden Scarab — 2× coin value for duration
/// </summary>
public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance { get; private set; }

    public enum PowerUpType
    {
        MagicCarpet,
        ScarabShell,
        OasisBreeze,
        GoldenScarab
    }

    // ── Durations (seconds) ───────────────────────────────────────────────
    [Header("Power-up Durations")]
    public float magicCarpetDuration  = 5f;
    public float oasisBreezeDuration  = 7f;
    public float goldenScarabDuration = 10f;

    // ── Magic Carpet speed multiplier ─────────────────────────────────────
    [Header("Magic Carpet")]
    public float magicCarpetSpeedMultiplier = 1.5f;

    // ── Audit counter (read by DifficultyAuditScript) ─────────────────────
    /// <summary>Total power-ups activated since game start.</summary>
    public int TotalSpawned { get; private set; }

    // ── States ────────────────────────────────────────────────────────────
    public bool isMagicCarpetActive  { get; private set; }
    public bool isScarabShieldActive { get; private set; }
    public bool isOasisBreezeActive  { get; private set; }
    public bool isGoldenScarabActive { get; private set; }

    // ── Events ────────────────────────────────────────────────────────────
    public static event Action<PowerUpType, float> OnPowerUpActivated;
    public static event Action<PowerUpType>        OnPowerUpDeactivated;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── Public API ─────────────────────────────────────────────────────────

    public void ActivatePowerUp(PowerUpType type)
    {
        TotalSpawned++;
        switch (type)
        {
            case PowerUpType.MagicCarpet:
                StartCoroutine(TimedPowerUp(type, magicCarpetDuration,
                    () => isMagicCarpetActive = true,
                    () => isMagicCarpetActive = false));
                break;

            case PowerUpType.ScarabShell:
                // One-shot shield; no timer — consumed on first collision
                isScarabShieldActive = true;
                OnPowerUpActivated?.Invoke(type, 0f);
                Debug.Log("[PowerUp] Scarab Shell: one-collision shield active.");
                break;

            case PowerUpType.OasisBreeze:
                StartCoroutine(TimedPowerUp(type, oasisBreezeDuration,
                    () => isOasisBreezeActive = true,
                    () => isOasisBreezeActive = false));
                break;

            case PowerUpType.GoldenScarab:
                StartCoroutine(TimedPowerUp(type, goldenScarabDuration,
                    () => isGoldenScarabActive = true,
                    () => isGoldenScarabActive = false));
                break;
        }
    }

    /// <summary>Returns true if Scarab Shield can absorb a hit.</summary>
    public bool HasScarabShield() => isScarabShieldActive;

    /// <summary>Consumes the Scarab Shield (called by PlayerController on collision).</summary>
    public void ConsumeScarabShield()
    {
        if (!isScarabShieldActive) return;
        isScarabShieldActive = false;
        OnPowerUpDeactivated?.Invoke(PowerUpType.ScarabShell);
        Debug.Log("[PowerUp] Scarab Shell consumed.");
    }

    /// <summary>
    /// Coin multiplier — CollectibleSystem uses this to double coin rewards.
    /// Returns 2 when Golden Scarab is active, 1 otherwise.
    /// </summary>
    public int CoinMultiplier() => isGoldenScarabActive ? 2 : 1;

    /// <summary>
    /// Speed multiplier — DifficultyManager/PlayerController apply this on top of base speed.
    /// </summary>
    public float SpeedMultiplier() => isMagicCarpetActive ? magicCarpetSpeedMultiplier : 1f;

    // ── Internals ─────────────────────────────────────────────────────────

    private IEnumerator TimedPowerUp(PowerUpType type, float duration,
        System.Action onActivate, System.Action onDeactivate)
    {
        // If already active, restart the timer (stacking not allowed)
        onDeactivate();

        onActivate();
        OnPowerUpActivated?.Invoke(type, duration);
        Debug.Log($"[PowerUp] {type} activated for {duration}s.");

        yield return new WaitForSeconds(duration);

        onDeactivate();
        OnPowerUpDeactivated?.Invoke(type);
        Debug.Log($"[PowerUp] {type} expired.");
    }
}
