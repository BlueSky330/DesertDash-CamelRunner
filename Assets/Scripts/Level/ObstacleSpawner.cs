using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawns obstacles into lane positions using the object pool.
/// No Instantiate calls — obstacles are recycled via ObjectPool.ReturnToPool().
///
/// Country flag drives variant selection so Egypt gets desert rocks,
/// China gets terracotta warriors, etc.
/// Spawn interval is driven by DifficultyManager for density ramp.
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
    public static ObstacleSpawner Instance { get; private set; }

    // ── Audit counters (read by DifficultyAuditScript) ────────────────────
    /// <summary>Total obstacles spawned since game start.</summary>
    public int SpawnCount { get; private set; }

    /// <summary>Current spawn rate in obstacles per minute, derived from DifficultyManager interval.</summary>
    public float CurrentSpawnRate => 60f / Mathf.Max(0.01f, GetSpawnInterval());

    // ── Lane constants ─────────────────────────────────────────────────────
    // Matches PlayerController.laneWidth default of 2f.
    // Set via inspector if laneWidth differs.
    [Header("Lanes")]
    public float laneWidth    = 2f;
    public float spawnZOffset = 50f; // how far ahead of player to spawn

    [Header("Obstacle Pool Tags by Country")]
    [Tooltip("Obstacle pool tags for Egypt / default")]
    public string[] egyptObstacleTags   = { "Obstacle_Rock", "Obstacle_Pyramid" };
    [Tooltip("Obstacle pool tags for Morocco")]
    public string[] moroccoObstacleTags = { "Obstacle_Cart", "Obstacle_Barrel" };
    [Tooltip("Obstacle pool tags for UAE")]
    public string[] uaeObstacleTags     = { "Obstacle_Crate", "Obstacle_Camel" };

    [Header("Fallback")]
    public string fallbackObstacleTag   = "Obstacle_Rock";

    [Header("Spawn Timing")]
    public float fallbackSpawnInterval  = 2f; // used if DifficultyManager absent

    private Transform playerTransform;
    private float     spawnTimer;
    private string    currentCountry = "Egypt";

    // Pool of spawned obstacles still alive (needed to return them to pool)
    private List<PooledObstacle> liveObstacles = new List<PooledObstacle>();

    // Small struct so we can return obstacle to the right pool tag
    private struct PooledObstacle
    {
        public GameObject obj;
        public string     tag;
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        spawnTimer      = GetSpawnInterval();
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameManager.GameState.Running)
            return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnObstacle();
            spawnTimer = GetSpawnInterval();
        }

        RecycleOffscreenObstacles();
    }

    // ── Public API ─────────────────────────────────────────────────────────

    /// <summary>Called by WorldMapManager when the player enters a new country.</summary>
    public void SetCountry(string country)
    {
        currentCountry = country;
    }

    // ── Spawn ──────────────────────────────────────────────────────────────

    private void SpawnObstacle()
    {
        if (ObjectPool.Instance == null || playerTransform == null) return;

        string tag = PickObstacleTag();
        int lane   = Random.Range(0, 3); // 0, 1, 2

        Vector3 spawnPos = new Vector3(
            LaneX(lane),
            0f,
            playerTransform.position.z + spawnZOffset);

        GameObject obj = ObjectPool.Instance.SpawnFromPool(tag, spawnPos, Quaternion.identity);
        if (obj != null)
        {
            liveObstacles.Add(new PooledObstacle { obj = obj, tag = tag });
            SpawnCount++;
        }
    }

    private void RecycleOffscreenObstacles()
    {
        if (playerTransform == null) return;

        for (int i = liveObstacles.Count - 1; i >= 0; i--)
        {
            PooledObstacle po = liveObstacles[i];
            if (po.obj == null || !po.obj.activeInHierarchy)
            {
                liveObstacles.RemoveAt(i);
                continue;
            }
            // Recycle once obstacle is behind the player
            if (po.obj.transform.position.z < playerTransform.position.z - 10f)
            {
                ObjectPool.Instance.ReturnToPool(po.tag, po.obj);
                liveObstacles.RemoveAt(i);
            }
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private string PickObstacleTag()
    {
        string[] tags = GetTagsForCountry(currentCountry);
        if (tags == null || tags.Length == 0) return fallbackObstacleTag;
        return tags[Random.Range(0, tags.Length)];
    }

    private string[] GetTagsForCountry(string country)
    {
        switch (country)
        {
            case "Egypt":
            case "Jordan":
                return egyptObstacleTags;
            case "Morocco":
                return moroccoObstacleTags;
            case "UAE":
                return uaeObstacleTags;
            default:
                return egyptObstacleTags; // default to Egypt theme
        }
    }

    private float LaneX(int lane) => (lane - 1) * laneWidth;

    private float GetSpawnInterval()
    {
        return (DifficultyManager.Instance != null)
            ? DifficultyManager.Instance.GetCurrentObstacleSpawnInterval()
            : fallbackSpawnInterval;
    }
}
