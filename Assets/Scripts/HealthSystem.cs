using UnityEngine;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    public static HealthSystem Instance { get; private set; }

    public float currentHealth { get; private set; }
    private const float MAX_HEALTH = 100f;

    [Header("Health Decay Settings")]
    public float initialDecayRatePerMinute = 1f; // 1% per minute
    public float acceleratedDecayRatePerMinute = 2.5f; // 2.5% per minute
    public float initialDecayDurationMinutes = 30f; // After 30 minutes, decay accelerates

    [Header("Natural Recovery Settings")]
    public float grassRecovery = 10f; // 10% health
    public float waterRecovery = 15f; // 15% health
    public float oasisRecovery = 40f; // 40% health

    [Header("Ad Recovery Settings")]
    public float quickAdRecovery = 25f; // 25% health
    public float standardAdRecovery = 50f; // 50% health
    public float premiumAdRecovery = 100f; // 100% health

    public delegate void OnHealthChanged(float newHealth);
    public static event OnHealthChanged onHealthChanged;

    public delegate void OnGameOver();
    public static event OnGameOver onGameOver;

    private float gameTimeElapsed = 0f;
    private bool isGameOver = false;

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

    void Start()
    {
        currentHealth = MAX_HEALTH;
        onHealthChanged?.Invoke(currentHealth);
        isGameOver = false;
        gameTimeElapsed = 0f;
    }

    void Update()
    {
        if (isGameOver) return;

        gameTimeElapsed += Time.deltaTime;

        float decayRate = (gameTimeElapsed / 60f < initialDecayDurationMinutes)
            ? initialDecayRatePerMinute
            : acceleratedDecayRatePerMinute;

        currentHealth -= (decayRate / 60f) * Time.deltaTime; // Convert per minute to per second
        currentHealth = Mathf.Max(0, currentHealth); // Health cannot go below 0

        onHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0 && !isGameOver)
        {
            isGameOver = true;
            onGameOver?.Invoke();
            Debug.Log("Game Over: Health reached 0!");
        }

        // Visual warning for low health (to be handled by UIManager/PlayerController based on currentHealth value)
        if (currentHealth <= 25f)
        {
            // Trigger visual/audio warning (e.g., UIManager.Instance.ShowLowHealthWarning(true))
        }
    }

    public void RecoverHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(MAX_HEALTH, currentHealth); // Health cannot exceed MAX_HEALTH
        onHealthChanged?.Invoke(currentHealth);
        Debug.Log($"Health recovered by {amount}. Current Health: {currentHealth}");
    }

    public void ResetHealth()
    {
        currentHealth = MAX_HEALTH;
        onHealthChanged?.Invoke(currentHealth);
        isGameOver = false;
        gameTimeElapsed = 0f;
    }

    // Methods for natural recovery (called by collision with environmental objects)
    public void RecoverFromGrass()
    {
        RecoverHealth(grassRecovery);
    }

    public void RecoverFromWater()
    {
        RecoverHealth(waterRecovery);
    }

    public void RecoverFromOasis()
    {
        RecoverHealth(oasisRecovery);
    }

    // Methods for ad recovery (called by AdManager)
    public void RecoverFromQuickAd()
    {
        RecoverHealth(quickAdRecovery);
    }

    public void RecoverFromStandardAd()
    {
        RecoverHealth(standardAdRecovery);
    }

    public void RecoverFromPremiumAd()
    {
        RecoverHealth(premiumAdRecovery);
    }

    public bool IsLowHealth() => currentHealth <= 25f;
}
