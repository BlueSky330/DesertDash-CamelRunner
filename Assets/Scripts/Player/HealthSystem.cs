using UnityEngine;
using System.Collections;

/// <summary>
/// Tracks camel health.
/// - Starts at 100%.
/// - Time decay: 1%/min for first 30 min, then 2.5%/min.
/// - TakeDamage for collision hits.
/// - Natural recovery via environmental pickups or ads.
/// - Fires onGameOver when health hits 0.
/// - IsLowHealth() → true below 25% (PlayerController uses this for slow-down).
/// </summary>
public class HealthSystem : MonoBehaviour
{
    public static HealthSystem Instance { get; private set; }

    public float currentHealth { get; private set; }
    private const float MAX_HEALTH = 100f;

    [Header("Health Decay Settings")]
    public float initialDecayRatePerMinute     = 1f;   // 1%/min for first 30 min
    public float acceleratedDecayRatePerMinute = 2.5f; // 2.5%/min after
    public float initialDecayDurationMinutes   = 30f;

    [Header("Natural Recovery")]
    public float grassRecovery  = 10f;
    public float waterRecovery  = 15f;
    public float oasisRecovery  = 40f;

    [Header("Ad Recovery")]
    public float quickAdRecovery    = 25f;
    public float standardAdRecovery = 50f;
    public float premiumAdRecovery  = 100f;

    public delegate void OnHealthChanged(float newHealth);
    public static event OnHealthChanged onHealthChanged;

    public delegate void OnGameOver();
    public static event OnGameOver onGameOver;

    private float gameTimeElapsed;
    private bool  isGameOver;
    private bool  isRunning;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameReset   += ResetHealth;
        InitHealth();
    }

    void OnDestroy()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameReset   -= ResetHealth;
    }

    void Update()
    {
        if (!isRunning || isGameOver) return;

        gameTimeElapsed += Time.deltaTime;

        float decayRate = (gameTimeElapsed / 60f < initialDecayDurationMinutes)
            ? initialDecayRatePerMinute
            : acceleratedDecayRatePerMinute;

        // Convert %/min → %/s
        currentHealth -= (decayRate / 60f) * Time.deltaTime;
        currentHealth  = Mathf.Max(0f, currentHealth);

        onHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0f && !isGameOver)
        {
            isGameOver = true;
            isRunning  = false;
            onGameOver?.Invoke();
        }
    }

    // ── Public API ─────────────────────────────────────────────────────────

    /// <summary>Deals direct damage (e.g. obstacle collision).</summary>
    public void TakeDamage(float amount)
    {
        if (isGameOver) return;
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        onHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0f && !isGameOver)
        {
            isGameOver = true;
            isRunning  = false;
            onGameOver?.Invoke();
        }
    }

    public void RecoverHealth(float amount)
    {
        currentHealth = Mathf.Min(MAX_HEALTH, currentHealth + amount);
        onHealthChanged?.Invoke(currentHealth);
    }

    public void ResetHealth()
    {
        InitHealth();
    }

    public bool IsLowHealth() => currentHealth < 25f;

    // ── Environmental recovery shortcuts ──────────────────────────────────
    public void RecoverFromGrass()   => RecoverHealth(grassRecovery);
    public void RecoverFromWater()   => RecoverHealth(waterRecovery);
    public void RecoverFromOasis()   => RecoverHealth(oasisRecovery);

    // ── Ad recovery shortcuts ─────────────────────────────────────────────
    public void RecoverFromQuickAd()    => RecoverHealth(quickAdRecovery);
    public void RecoverFromStandardAd() => RecoverHealth(standardAdRecovery);
    public void RecoverFromPremiumAd()  => RecoverHealth(premiumAdRecovery);

    // ── Internals ─────────────────────────────────────────────────────────

    private void InitHealth()
    {
        currentHealth   = MAX_HEALTH;
        gameTimeElapsed = 0f;
        isGameOver      = false;
        isRunning       = false;
        onHealthChanged?.Invoke(currentHealth);
    }

    private void OnGameStarted()
    {
        isRunning  = true;
        isGameOver = false;
    }
}
