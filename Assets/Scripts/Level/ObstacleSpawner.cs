using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    public float spawnInterval = 2f;
    public float spawnRangeX = 2f;
    public float spawnZOffset = 50f;

    private float timer;

    void Start()
    {
        timer = spawnInterval;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            return; // Stop spawning if game is over
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SpawnObstacle();
            timer = spawnInterval;
        }
    }

    void SpawnObstacle()
    {
        int randomObstacleIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject obstacleToSpawn = obstaclePrefabs[randomObstacleIndex];

        // Randomly choose one of three lanes
        float randomX = Random.Range(-spawnRangeX, spawnRangeX); // Adjust based on lane positions
        // For a 3-lane system, you might want to snap to specific X positions:
        // float[] laneXPositions = { -2f, 0f, 2f }; // Example lane X positions
        // float randomX = laneXPositions[Random.Range(0, laneXPositions.Length)];

        Vector3 spawnPosition = new Vector3(randomX, 0.5f, transform.position.z + spawnZOffset);
        Instantiate(obstacleToSpawn, spawnPosition, Quaternion.identity);
    }
}
