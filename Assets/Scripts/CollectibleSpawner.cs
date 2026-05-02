using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    public GameObject[] collectiblePrefabs;
    public float spawnInterval = 1f;
    public float spawnRangeX = 2f;
    public float spawnZOffset = 30f;

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
            SpawnCollectible();
            timer = spawnInterval;
        }
    }

    void SpawnCollectible()
    {
        int randomCollectibleIndex = Random.Range(0, collectiblePrefabs.Length);
        GameObject collectibleToSpawn = collectiblePrefabs[randomCollectibleIndex];

        // Randomly choose one of three lanes
        float randomX = Random.Range(-spawnRangeX, spawnRangeX); // Adjust based on lane positions
        // For a 3-lane system, you might want to snap to specific X positions:
        // float[] laneXPositions = { -2f, 0f, 2f }; // Example lane X positions
        // float randomX = laneXPositions[Random.Range(0, laneXPositions.Length)];

        Vector3 spawnPosition = new Vector3(randomX, 0.5f, transform.position.z + spawnZOffset);
        Instantiate(collectibleToSpawn, spawnPosition, Quaternion.identity);
    }
}
