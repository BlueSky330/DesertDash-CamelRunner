using UnityEngine;

/// <summary>
/// Attach to each collectible prefab.
/// Registers the pickup with CollectibleSystem on trigger and returns itself to the pool.
/// </summary>
[RequireComponent(typeof(Collider))]
public class CollectiblePickup : MonoBehaviour
{
    public CollectibleSystem.CollectibleType collectibleType;

    // Pool tag is stored so we can return to the correct pool
    // (set by CollectibleSpawner when spawned, falls back to name)
    [HideInInspector]
    public string poolTag;

    void Awake()
    {
        // Ensure collider is a trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Register with scoring system
        if (CollectibleSystem.Instance != null)
            CollectibleSystem.Instance.AddCollectible(collectibleType);

        // Return to pool instead of Destroy
        string tag = string.IsNullOrEmpty(poolTag) ? gameObject.name : poolTag;
        if (ObjectPool.Instance != null)
            ObjectPool.Instance.ReturnToPool(tag, gameObject);
        else
            gameObject.SetActive(false);
    }
}
