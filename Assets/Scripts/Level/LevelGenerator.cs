using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Procedurally spawns level chunks from the object pool.
/// No Instantiate/Destroy calls during gameplay — chunks are recycled.
///
/// Difficulty ramp: chunk scroll speed is driven by DifficultyManager.
/// Each chunk has an ObstacleSpawner and CollectibleSpawner child that activates
/// when the chunk is placed and deactivates when returned to the pool.
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    [Header("Chunk Pool Tags")]
    [Tooltip("ObjectPool tags that correspond to level chunk prefabs")]
    public string[] chunkPoolTags;

    [Header("Layout")]
    public float chunkLength      = 50f;
    public int   chunksAhead      = 3;   // how many chunks to keep in front of player

    [Header("Scroll")]
    public float initialScrollSpeed = 10f; // fallback if DifficultyManager absent

    private Transform   playerTransform;
    private List<GameObject> activeChunks = new List<GameObject>();
    private float spawnZ;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
        {
            Debug.LogError("[LevelGenerator] No GameObject tagged 'Player' found.");
            return;
        }

        spawnZ = playerTransform.position.z;

        for (int i = 0; i < chunksAhead; i++)
            SpawnChunk();

        GameManager.OnGameReset += OnGameReset;
    }

    void OnDestroy()
    {
        GameManager.OnGameReset -= OnGameReset;
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameManager.GameState.Running)
            return;

        float speed = CurrentSpeed();

        // Scroll all active chunks toward the player (player moves forward in world space,
        // so we move chunks in -Z so the world flows past the camera).
        foreach (GameObject chunk in activeChunks)
        {
            if (chunk == null || !chunk.activeInHierarchy) continue;
            chunk.transform.Translate(0f, 0f, -speed * Time.deltaTime, Space.World);
        }

        // When the rearmost chunk has scrolled past the player, recycle it and spawn a new one ahead
        if (activeChunks.Count > 0)
        {
            GameObject rear = activeChunks[0];
            if (rear != null && rear.transform.position.z < playerTransform.position.z - chunkLength)
            {
                RecycleChunk(rear);
                activeChunks.RemoveAt(0);
                SpawnChunk();
            }
        }
    }

    // ── Chunk management ──────────────────────────────────────────────────

    private void SpawnChunk()
    {
        if (ObjectPool.Instance == null || chunkPoolTags == null || chunkPoolTags.Length == 0)
        {
            Debug.LogWarning("[LevelGenerator] ObjectPool not ready or no chunk tags configured.");
            return;
        }

        string tag = chunkPoolTags[Random.Range(0, chunkPoolTags.Length)];
        GameObject chunk = ObjectPool.Instance.SpawnFromPool(tag,
            new Vector3(0f, 0f, spawnZ), Quaternion.identity);

        if (chunk != null)
        {
            activeChunks.Add(chunk);
            spawnZ += chunkLength;
        }
    }

    private void RecycleChunk(GameObject chunk)
    {
        // Determine pool tag from chunk name (ObjectPool sets obj.name = tag)
        ObjectPool.Instance.ReturnToPool(chunk.name, chunk);
    }

    private void OnGameReset()
    {
        // Return all active chunks to pool
        foreach (GameObject chunk in activeChunks)
        {
            if (chunk != null)
                ObjectPool.Instance.ReturnToPool(chunk.name, chunk);
        }
        activeChunks.Clear();

        if (playerTransform != null)
            spawnZ = playerTransform.position.z;

        for (int i = 0; i < chunksAhead; i++)
            SpawnChunk();
    }

    private float CurrentSpeed()
    {
        float base_ = (DifficultyManager.Instance != null)
            ? DifficultyManager.Instance.GetCurrentSpeed()
            : initialScrollSpeed;

        // Apply Magic Carpet speed boost if active
        float mult = (PowerUpManager.Instance != null)
            ? PowerUpManager.Instance.SpeedMultiplier()
            : 1f;

        return base_ * mult;
    }
}
