using UnityEngine;
using System.Collections.Generic;

public class CollectibleSystem : MonoBehaviour
{
    public static CollectibleSystem Instance { get; private set; }

    public int currentScore { get; private set; }
    public int currentCoins { get; private set; }

    // Collectible point values
    private const int DATE_POINTS = 1;
    private const int SILVER_COIN_POINTS = 3;
    private const int GEM_POINTS = 10;
    private const int GOLDEN_DATE_POINTS = 5;
    private const int MYSTERY_BOX_MIN_POINTS = 15;
    private const int MYSTERY_BOX_MAX_POINTS = 50;

    // Score to coin conversion rate
    private const int SCORE_TO_COIN_RATE = 150;

    public delegate void OnScoreChanged(int newScore);
    public static event OnScoreChanged onScoreChanged;

    public delegate void OnCoinsChanged(int newCoins);
    public static event OnCoinsChanged onCoinsChanged;

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
        currentScore = 0;
        currentCoins = 500; // Starter bonus
        onScoreChanged?.Invoke(currentScore);
        onCoinsChanged?.Invoke(currentCoins);
    }

    public void AddCollectible(CollectibleType type)
    {
        int pointsToAdd = 0;
        switch (type)
        {
            case CollectibleType.Date:
                pointsToAdd = DATE_POINTS;
                break;
            case CollectibleType.SilverCoin:
                pointsToAdd = SILVER_COIN_POINTS;
                break;
            case CollectibleType.Gem:
                pointsToAdd = GEM_POINTS;
                break;
            case CollectibleType.GoldenDate:
                pointsToAdd = GOLDEN_DATE_POINTS;
                break;
            case CollectibleType.MysteryBox:
                pointsToAdd = Random.Range(MYSTERY_BOX_MIN_POINTS, MYSTERY_BOX_MAX_POINTS + 1);
                break;
        }
        currentScore += pointsToAdd;
        onScoreChanged?.Invoke(currentScore);
    }

    public void ConvertScoreToCoins()
    {
        int coinsEarned = currentScore / SCORE_TO_COIN_RATE;
        if (coinsEarned > 0)
        {
            AddCoins(coinsEarned);
            currentScore %= SCORE_TO_COIN_RATE; // Keep remainder
            onScoreChanged?.Invoke(currentScore);
            Debug.Log($"Converted {coinsEarned * SCORE_TO_COIN_RATE} score to {coinsEarned} coins.");
        }
        else
        {
            Debug.Log("Not enough score to convert to coins.");
        }
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        onCoinsChanged?.Invoke(currentCoins);
        Debug.Log($"Added {amount} coins. Total: {currentCoins}");
    }

    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            onCoinsChanged?.Invoke(currentCoins);
            Debug.Log($"Spent {amount} coins. Remaining: {currentCoins}");
            return true;
        }
        Debug.Log($"Not enough coins to spend {amount}. Current: {currentCoins}");
        return false;
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
