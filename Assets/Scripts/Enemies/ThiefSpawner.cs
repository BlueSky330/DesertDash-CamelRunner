using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawns thief character prefabs based on the ThiefType and spawn position.
/// Works in tandem with ThiefSystem to instantiate visual representations of thieves.
///
/// Prefab references are set via inspector, allowing artists to swap models without code changes.
/// </summary>
public class ThiefSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ThiefPrefabEntry
    {
        public ThiefSystem.ThiefType thiefType;
        public GameObject prefab;
    }

    [SerializeField] private List<ThiefPrefabEntry> thiefPrefabs = new List<ThiefPrefabEntry>();

    [Header("Spawn Settings")]
    [SerializeField] private float aheadDistance = 15f;     // Z distance ahead of player
    [SerializeField] private float behindDistance = -10f;   // Z distance behind player
    [SerializeField] private float sideOffsetX = 5f;        // X offset for side spawn

    public static ThiefSpawner Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Spawn a thief at the specified position relative to the player.
    /// Called by ThiefSystem.SpawnRandomThief().
    /// </summary>
    public void SpawnThief(ThiefSystem.ThiefType thiefType, ThiefSystem.ThiefSpawnPosition spawnPosition)
    {
        GameObject prefab = GetThiefPrefab(thiefType);
        if (prefab == null)
        {
            Debug.LogError($"[ThiefSpawner] No prefab found for {thiefType}");
            return;
        }

        Vector3 spawnPos = CalculateSpawnPosition(spawnPosition);
        Quaternion spawnRot = Quaternion.identity;

        // Instantiate thief at calculated position
        GameObject thiefGO = Instantiate(prefab, spawnPos, spawnRot);
        thiefGO.name = $"Thief_{thiefType}_{Time.frameCount}";

        Debug.Log($"[ThiefSpawner] Spawned {thiefType} at {spawnPosition} ({spawnPos})");
    }

    /// <summary>
    /// Get the prefab for a specific thief type.
    /// </summary>
    private GameObject GetThiefPrefab(ThiefSystem.ThiefType thiefType)
    {
        foreach (var entry in thiefPrefabs)
        {
            if (entry.thiefType == thiefType)
                return entry.prefab;
        }
        return null;
    }

    /// <summary>
    /// Calculate world spawn position based on spawn position enum and player location.
    /// </summary>
    private Vector3 CalculateSpawnPosition(ThiefSystem.ThiefSpawnPosition spawnPosition)
    {
        Vector3 playerPos = PlayerController.Instance != null
            ? PlayerController.Instance.transform.position
            : Vector3.zero;

        switch (spawnPosition)
        {
            case ThiefSystem.ThiefSpawnPosition.Ahead:
                return playerPos + new Vector3(0, 0, aheadDistance);

            case ThiefSystem.ThiefSpawnPosition.Behind:
                return playerPos + new Vector3(0, 0, behindDistance);

            case ThiefSystem.ThiefSpawnPosition.Side:
                float sideX = Random.value > 0.5f ? sideOffsetX : -sideOffsetX;
                return playerPos + new Vector3(sideX, 0, 5f);

            default:
                return playerPos;
        }
    }

    /// <summary>
    /// Editor helper: validate that all thief types have prefab assignments.
    /// </summary>
    public void ValidatePrefabAssignments()
    {
        var allTypes = System.Enum.GetValues(typeof(ThiefSystem.ThiefType));
        foreach (ThiefSystem.ThiefType type in allTypes)
        {
            bool found = false;
            foreach (var entry in thiefPrefabs)
            {
                if (entry.thiefType == type && entry.prefab != null)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                Debug.LogWarning($"[ThiefSpawner] No prefab assigned for {type}");
        }
    }
}
