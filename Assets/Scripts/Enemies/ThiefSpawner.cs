using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawns thief characters at runtime via procedural mesh components.
/// No prefab assets are required — each thief type maps to its ProceduralXxxMesh script
/// which builds the mesh in Awake(), keeping the pipeline fully headless-safe.
/// </summary>
public class ThiefSpawner : MonoBehaviour
{
    // Maps each ThiefType to its procedural mesh component. AddComponent builds the mesh on Awake.
    private static readonly Dictionary<ThiefSystem.ThiefType, System.Type> thiefMeshTypes = new()
    {
        { ThiefSystem.ThiefType.DesertBandit, typeof(ProceduralDesertBanditMesh) },
        { ThiefSystem.ThiefType.Ninja,        typeof(ProceduralNinjaThiefMesh)   },
        { ThiefSystem.ThiefType.Pirate,       typeof(ProceduralPirateMesh)       },
        { ThiefSystem.ThiefType.ShadowThief,  typeof(ProceduralShadowThiefMesh)  },
    };

    [Header("Spawn Settings")]
    [SerializeField] private float aheadDistance = 15f;
    [SerializeField] private float behindDistance = -10f;
    [SerializeField] private float sideOffsetX = 5f;

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
        Vector3 spawnPos = CalculateSpawnPosition(spawnPosition);
        GameObject thiefGO = BuildThief(thiefType, spawnPos, Quaternion.identity);
        thiefGO.name = $"Thief_{thiefType}_{Time.frameCount}";
        Debug.Log($"[ThiefSpawner] Spawned {thiefType} at {spawnPosition} ({spawnPos})");
    }

    /// <summary>
    /// Instantiate a thief GameObject entirely at runtime — no prefab required.
    /// </summary>
    private GameObject BuildThief(ThiefSystem.ThiefType type, Vector3 position, Quaternion rotation)
    {
        var go = new GameObject(type.ToString());
        go.transform.SetPositionAndRotation(position, rotation);

        // Procedural mesh component builds the visual mesh on its own Awake()
        go.AddComponent(thiefMeshTypes[type]);

        var col = go.AddComponent<CapsuleCollider>();
        col.isTrigger = true;
        col.radius = 0.35f;
        col.height = 1.8f;

        var rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        go.AddComponent<Animator>();
        go.tag = "Enemy";

        // Tri-budget guard: logs PASS/FAIL at spawn time (mesh built in AddComponent above)
        CharacterBudgetVerifier.Verify(go, type.ToString());

        return go;
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
}
