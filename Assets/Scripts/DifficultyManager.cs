using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [Header("Speed Curve")]
    public float initialSpeed = 10f;
    public float maxSpeed = 30f;
    public float speedIncreaseRate = 0.1f; // Speed increases by this amount per second

    [Header("Obstacle Density")]
    public float initialObstacleSpawnInterval = 2f;
    public float minObstacleSpawnInterval = 0.5f;
    public float obstacleDensityIncreaseRate = 0.05f; // Interval decreases by this amount per minute

    [Header("Thief Frequency")]
    public float initialThiefSpawnInterval = 30f;
    public float minThiefSpawnInterval = 10f;
    public float thiefFrequencyIncreaseRate = 0.1f; // Interval decreases by this amount per minute

    private float currentSpeed;
    private float currentObstacleSpawnInterval;
    private float currentThiefSpawnInterval;
    private float gameTime = 0f;

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
        ResetDifficulty();
    }

    void Update()
    {
        gameTime += Time.deltaTime;
        UpdateSpeed();
        UpdateObstacleDensity();
        UpdateThiefFrequency();
    }

    public void ResetDifficulty()
    {
        currentSpeed = initialSpeed;
        currentObstacleSpawnInterval = initialObstacleSpawnInterval;
        currentThiefSpawnInterval = initialThiefSpawnInterval;
        gameTime = 0f;
        Debug.Log("Difficulty reset.");
    }

    private void UpdateSpeed()
    {
        currentSpeed = Mathf.Min(maxSpeed, initialSpeed + (speedIncreaseRate * gameTime));
        // Apply currentSpeed to PlayerController or LevelGenerator
    }

    private void UpdateObstacleDensity()
    {
        float intervalDecrease = (gameTime / 60f) * obstacleDensityIncreaseRate; // Decrease per minute
        currentObstacleSpawnInterval = Mathf.Max(minObstacleSpawnInterval, initialObstacleSpawnInterval - intervalDecrease);
        // Apply currentObstacleSpawnInterval to ObstacleSpawner
    }

    private void UpdateThiefFrequency()
    {
        float intervalDecrease = (gameTime / 60f) * thiefFrequencyIncreaseRate; // Decrease per minute
        currentThiefSpawnInterval = Mathf.Max(minThiefSpawnInterval, initialThiefSpawnInterval - intervalDecrease);
        // Apply currentThiefSpawnInterval to ThiefSystem
    }

    public float GetCurrentSpeed() => currentSpeed;
    public float GetCurrentObstacleSpawnInterval() => currentObstacleSpawnInterval;
    public float GetCurrentThiefSpawnInterval() => currentThiefSpawnInterval;

    // Method to adjust difficulty based on current country (e.g., make thieves more frequent in certain countries)
    public void AdjustDifficultyForCountry(string countryName)
    {
        // Example: Increase thief frequency in China
        if (countryName == "China")
        {
            currentThiefSpawnInterval = Mathf.Max(minThiefSpawnInterval, currentThiefSpawnInterval * 0.8f); // 20% faster thieves
        }
        // Add more country-specific adjustments here
    }
}
