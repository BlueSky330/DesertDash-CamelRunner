using UnityEngine;
using System.Collections.Generic;

public class CollectibleSystem : MonoBehaviour
{
    public static CollectibleSystem Instance { get; private set; }

    public int currentScore { get; private set; }
    public int CurrentCoins { get; private set; }

    // Keep legacy accessor for any existing callers
    public int currentCoins => CurrentCoins;

    // Collectible point values
    private const int DATE_POINTS = 1;
    private const int SILVER_COIN_POINTS = 3;
    private const int GEM_POINTS = 10;
    private const int GOLDEN_DATE_POINTS = 5;
    private const int MYSTERY_BOX_MIN_POINTS = 15;
    private const int MYSTERY_BOX_MAX_POINTS = 50;

    // Score to coin conversion rate (150 pts = 1 coin)
    private const int SCORE_TO_COIN_RATE = 150;

    public delegate void OnScoreChanged(int newScore);
    public static event OnScoreChanged onScoreChanged;

    public delegate void OnCoinsChanged(int newCoins);
    public static event OnCoinsChanged onCoinsChanged;

    // Starting coin bonus for new players
    private const int STARTING_COINS = 500;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        currentScore = 0;
        CurrentCoins = STARTING_COINS;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Start()
    {
        onScoreChanged?.Invoke(currentScore);
        onCoinsChanged?.Invoke(CurrentCoins);
    }

    // ── Coin management ───────────────────────────────────────────────────────

    /// <summary>Directly set the coin balance (called by CoinEconomy on load).</summary>
    public void SetCoins(int amount)
    {
        CurrentCoins = Mathf.Max(0, amount);
        onCoinsChanged?.Invoke(CurrentCoins);
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        CurrentCoins += amount;
        onCoinsChanged?.Invoke(CurrentCoins);
        // Notify CoinEconomy so it can persist
        CoinEconomy.Instance?.SyncFromCollectibleSystem(CurrentCoins);
        Debug.Log($"[CollectibleSystem] Added {amount} coins. Total: {CurrentCoins}");
    }

    public bool SpendCoins(int amount)
    {
        if (CurrentCoins < amount)
        {
            Debug.Log($"[CollectibleSystem] Not enough coins. Have {CurrentCoins}, need {amount}.");
            return false;
        }
        CurrentCoins -= amount;
        onCoinsChanged?.Invoke(CurrentCoins);
        CoinEconomy.Instance?.SyncFromCollectibleSystem(CurrentCoins);
        Debug.Log($"[CollectibleSystem] Spent {amount}. Remaining: {CurrentCoins}");
        return true;
    }

    // ── Score management ──────────────────────────────────────────────────────

    public void AddCollectible(CollectibleType type)
    {
        int pointsToAdd = 0;
        switch (type)
        {
            case CollectibleType.Date:
                pointsToAdd = DATE_POINTS;
                GameAudioEvents.OnPlayCollectDates?.Invoke();
                break;
            case CollectibleType.SilverCoin:
                pointsToAdd = SILVER_COIN_POINTS;
                GameAudioEvents.OnPlayCollectCoins?.Invoke();
                break;
            case CollectibleType.Gem:
                pointsToAdd = GEM_POINTS;
                GameAudioEvents.OnPlayCollectGems?.Invoke();
                break;
            case CollectibleType.GoldenDate:
                pointsToAdd = GOLDEN_DATE_POINTS;
                GameAudioEvents.OnPlayCollectDates?.Invoke();
                break;
            case CollectibleType.MysteryBox:
                pointsToAdd = Random.Range(MYSTERY_BOX_MIN_POINTS, MYSTERY_BOX_MAX_POINTS + 1);
                GameAudioEvents.OnPlayCollectGems?.Invoke();
                break;
        }
        currentScore += pointsToAdd;
        onScoreChanged?.Invoke(currentScore);
    }

    /// <summary>Convert accumulated score to coins at rate 150 pts = 1 coin.</summary>
    public void ConvertScoreToCoins()
    {
        int coinsEarned = currentScore / SCORE_TO_COIN_RATE;
        if (coinsEarned > 0)
        {
            currentScore %= SCORE_TO_COIN_RATE; // retain remainder
            AddCoins(coinsEarned);
            onScoreChanged?.Invoke(currentScore);
            Debug.Log($"[CollectibleSystem] Converted score → {coinsEarned} coins.");
        }
    }

    /// <summary>Reset score only; coins persist across sessions via CoinEconomy.</summary>
    public void ResetScore()
    {
        currentScore = 0;
        onScoreChanged?.Invoke(currentScore);
    }

    /// <summary>Legacy reset — restores score to 0 and coins to starting bonus.</summary>
    public void ResetScoreAndCoins()
    {
        ResetScore();
        CurrentCoins = STARTING_COINS;
        onCoinsChanged?.Invoke(CurrentCoins);
        Debug.Log($"[CollectibleSystem] Score and coins reset to defaults (coins: {STARTING_COINS}).");
    }

    public enum CollectibleType
    {
        Date,
        SilverCoin,
        Gem,
        GoldenDate,
        MysteryBox
    }
}
