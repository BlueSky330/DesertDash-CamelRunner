using UnityEngine;
using System.Collections.Generic;

public class ThiefSystem : MonoBehaviour
{
    public static ThiefSystem Instance { get; private set; }

    [Header("Thief Spawn Settings")]
    public float minThiefSpawnInterval = 15f;
    public float maxThiefSpawnInterval = 30f;
    private float nextThiefSpawnTime;

    [Header("Thief Types and Coin Steal Percentage")]
    private Dictionary<ThiefType, float> thiefStealPercentages = new Dictionary<ThiefType, float>()
    {
        { ThiefType.DesertBandit, 0.10f }, // 10%
        { ThiefType.NinjaThief, 0.15f },    // 15%
        { ThiefType.Pirate, 0.20f },       // 20%
        { ThiefType.ShadowThief, 0.30f }   // 30%
    };

    public enum ThiefType
    {
        DesertBandit,
        NinjaThief,
        Pirate,
        ShadowThief
    }

    public enum ThiefSpawnPosition
    {
        Ahead,
        Behind,
        Side
    }

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
        SetNextThiefSpawnTime();
    }

    void Update()
    {
        if (Time.time >= nextThiefSpawnTime)
        {
            SpawnRandomThief();
            SetNextThiefSpawnTime();
        }
    }

    private void SetNextThiefSpawnTime()
    {
        nextThiefSpawnTime = Time.time + Random.Range(minThiefSpawnInterval, maxThiefSpawnInterval);
    }

    private void SpawnRandomThief()
    {
        // Determine current country (placeholder for WorldMapManager integration)
        // For now, let's assume Egypt for initial testing
        string currentCountry = "Egypt"; 

        List<ThiefType> availableThieves = GetThievesForCountry(currentCountry);
        if (availableThieves.Count == 0)
        {
            Debug.LogWarning($"No thieves defined for country: {currentCountry}");
            return;
        }

        ThiefType spawnedThiefType = availableThieves[Random.Range(0, availableThieves.Count)];
        ThiefSpawnPosition spawnPosition = (ThiefSpawnPosition)Random.Range(0, System.Enum.GetValues(typeof(ThiefSpawnPosition)).Length));

        Debug.Log($"Spawning {spawnedThiefType} at {spawnPosition} in {currentCountry}");

        // In a real game, this would instantiate a thief prefab and set its behavior
        // For now, we simulate the encounter logic directly.
        SimulateThiefEncounter(spawnedThiefType, spawnPosition);
    }

    private List<ThiefType> GetThievesForCountry(string country)
    {
        List<ThiefType> thieves = new List<ThiefType>();
        switch (country)
        {
            case "Egypt":
            case "Jordan":
            case "UAE":
                thieves.Add(ThiefType.DesertBandit);
                break;
            case "China":
                thieves.Add(ThiefType.NinjaThief);
                break;
            case "Brazil":
            case "Italy":
            case "France":
                thieves.Add(ThiefType.Pirate);
                break;
            default:
                thieves.Add(ThiefType.DesertBandit); // Default for undefined countries
                break;
        }
        // Shadow Thief is rare and can appear anywhere
        if (Random.value < 0.1f) // 10% chance for Shadow Thief
        {
            thieves.Add(ThiefType.ShadowThief);
        }
        return thieves;
    }

    private void SimulateThiefEncounter(ThiefType thiefType, ThiefSpawnPosition spawnPosition)
    {
        // This is a simplified simulation. In a real game, player interaction would determine if caught.
        bool caught = false; // Placeholder for actual game logic

        if (spawnPosition == ThiefSpawnPosition.Ahead)
        {
            // Simulate player failing to react correctly (e.g., not slowing down then speeding up)
            if (Random.value < 0.5f) caught = true; // 50% chance to be caught if ahead
        }
        else // Behind or Side
        {
            // Simulate player failing to react correctly (e.g., not speeding up)
            if (Random.value < 0.3f) caught = true; // 30% chance to be caught if behind/side
        }

        if (caught)
        {
            HandleCaughtByThief(thiefType);
        }
        else
        {
            Debug.Log($"Player successfully evaded the {thiefType}!");
        }
    }

    public void HandleCaughtByThief(ThiefType thiefType)
    {
        if (PowerUpManager.Instance.UseAntiThiefShield())
        {
            Debug.Log("Anti-Thief Shield protected the player!");
            return;
        }

        float stealPercentage = thiefStealPercentages[thiefType];
        int coinsToSteal = Mathf.FloorToInt(CollectibleSystem.Instance.currentCoins * stealPercentage);

        if (CollectibleSystem.Instance.SpendCoins(coinsToSteal))
        {
            Debug.Log($"Caught by {thiefType}! Lost {coinsToSteal} coins.");
        }
        else
        {
            Debug.Log($"Caught by {thiefType}! Player had no coins to steal.");
        }
    }
}
