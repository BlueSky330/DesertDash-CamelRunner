using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawns collectibles using the object pool based on a rarity table.
/// No Instantiate in the gameplay loop.
///
/// Rarity table (weights):
///   Date        60
///   SilverCoin  20
///   Gem          8
///   GoldenDate  10
///   MysteryBox   2
///
/// When Oasis Breeze (magnet) power-up is active, nearby collectibles
/// are attracted toward the player each frame.
/// </summary>
public class CollectibleSpawner : MonoBehaviour
{
    // ── Rarity table ──────────────────────────────────────────────────────
    [System.Serializable]
    public class CollectibleEntry
    {
        public CollectibleSystem.CollectibleType type;
        public string   poolTag;
        [Range(0, 100)]
        public int      weight = 10;
    }

    [Header("Rarity Table")]
    public CollectibleEntry[] collectibleTable = new CollectibleEntry[]
    {
        new CollectibleEntry { type = CollectibleSystem.CollectibleType.Date,        poolTag = "Collectible_Date",       weight = 60 },
        new CollectibleEntry { type = CollectibleSystem.CollectibleType.SilverCoin,  poolTag = "Collectible_SilverCoin", weight = 20 },
        new CollectibleEntry { type = CollectibleSystem.CollectibleType.GoldenDate,  poolTag = "Collectible_GoldenDate", weight = 10 },
        new CollectibleEntry { type = CollectibleSystem.CollectibleType.Gem,         poolTag = "Collectible_Gem",        weight = 8  },
        new CollectibleEntry { type = CollectibleSystem.CollectibleType.MysteryBox,  poolTag = "Collectible_MysteryBox", weight = 2  },
    };

    [Header("Lanes")]
    public float laneWidth    = 2f;
    public float spawnZOffset = 30f;

    [Header("Spawn Timing")]
    public float spawnInterval = 1f;

    [Header("Magnet (Oasis Breeze)")]
    public float magnetRadius = 6f;
    public float magnetSpeed  = 8f;

    private Transform playerTransform;
    private float     spawnTimer;
    private int       totalWeight;

    // Track live collectibles for recycle + magnet
    private struct PooledCollectible
    {
        public GameObject                       obj;
        public string                           tag;
        public CollectibleSystem.CollectibleType type;
    }
    private List<PooledCollectible> liveCollectibles = new List<PooledCollectible>();

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        spawnTimer      = spawnInterval;

        // Pre-compute total weight
        totalWeight = 0;
        foreach (var entry in collectibleTable)
            totalWeight += entry.weight;
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameManager.GameState.Running)
            return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnCollectible();
            spawnTimer = spawnInterval;
        }

        if (PowerUpManager.Instance != null && PowerUpManager.Instance.isOasisBreezeActive)
            ApplyMagnet();

        RecycleOffscreenCollectibles();
    }

    // ── Spawn ──────────────────────────────────────────────────────────────

    private void SpawnCollectible()
    {
        if (ObjectPool.Instance == null || playerTransform == null) return;

        CollectibleEntry entry = PickEntry();
        if (entry == null) return;

        int lane = Random.Range(0, 3);
        Vector3 spawnPos = new Vector3(
            LaneX(lane),
            0.5f,
            playerTransform.position.z + spawnZOffset);

        GameObject obj = ObjectPool.Instance.SpawnFromPool(entry.poolTag, spawnPos, Quaternion.identity);
        if (obj != null)
        {
            // Store collectible type on a component so OnTriggerEnter can report it
            var pickup = obj.GetComponent<CollectiblePickup>();
            if (pickup != null) pickup.collectibleType = entry.type;

            liveCollectibles.Add(new PooledCollectible { obj = obj, tag = entry.poolTag, type = entry.type });
        }
    }

    // ── Magnet ────────────────────────────────────────────────────────────

    private void ApplyMagnet()
    {
        if (playerTransform == null) return;

        foreach (PooledCollectible pc in liveCollectibles)
        {
            if (pc.obj == null || !pc.obj.activeInHierarchy) continue;

            float dist = Vector3.Distance(pc.obj.transform.position, playerTransform.position);
            if (dist < magnetRadius)
            {
                pc.obj.transform.position = Vector3.MoveTowards(
                    pc.obj.transform.position,
                    playerTransform.position,
                    magnetSpeed * Time.deltaTime);
            }
        }
    }

    // ── Recycle ───────────────────────────────────────────────────────────

    private void RecycleOffscreenCollectibles()
    {
        if (playerTransform == null) return;

        for (int i = liveCollectibles.Count - 1; i >= 0; i--)
        {
            PooledCollectible pc = liveCollectibles[i];
            if (pc.obj == null || !pc.obj.activeInHierarchy)
            {
                liveCollectibles.RemoveAt(i);
                continue;
            }
            if (pc.obj.transform.position.z < playerTransform.position.z - 10f)
            {
                ObjectPool.Instance.ReturnToPool(pc.tag, pc.obj);
                liveCollectibles.RemoveAt(i);
            }
        }
    }

    // ── Rarity selection ──────────────────────────────────────────────────

    private CollectibleEntry PickEntry()
    {
        if (totalWeight <= 0) return null;
        int roll = Random.Range(0, totalWeight);
        foreach (var entry in collectibleTable)
        {
            if (roll < entry.weight) return entry;
            roll -= entry.weight;
        }
        return collectibleTable[collectibleTable.Length - 1];
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private float LaneX(int lane) => (lane - 1) * laneWidth;
}
