using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generic object pool. Eliminates runtime Instantiate/Destroy calls in the gameplay loop.
/// Usage: call SpawnFromPool() instead of Instantiate(), ReturnToPool() instead of Destroy().
/// </summary>
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int initialSize = 10;
    }

    public List<Pool> pools;

    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, GameObject> prefabLookup;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        prefabLookup = new Dictionary<string, GameObject>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();
            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = CreateNewObject(pool.prefab, pool.tag);
                objectQueue.Enqueue(obj);
            }
            poolDictionary[pool.tag] = objectQueue;
            prefabLookup[pool.tag] = pool.prefab;
        }
    }

    private GameObject CreateNewObject(GameObject prefab, string tag)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.name = tag;
        obj.SetActive(false);
        return obj;
    }

    /// <summary>Retrieves an object from the pool. Grows the pool if empty.</summary>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"[ObjectPool] Pool '{tag}' not found.");
            return null;
        }

        Queue<GameObject> queue = poolDictionary[tag];

        // Grow pool on demand — cheaper than failing
        if (queue.Count == 0)
        {
            if (prefabLookup.TryGetValue(tag, out GameObject prefab))
            {
                GameObject newObj = CreateNewObject(prefab, tag);
                queue.Enqueue(newObj);
            }
            else
            {
                Debug.LogError($"[ObjectPool] No prefab registered for tag '{tag}'.");
                return null;
            }
        }

        GameObject obj2 = queue.Dequeue();
        obj2.transform.SetPositionAndRotation(position, rotation);
        obj2.SetActive(true);
        return obj2;
    }

    /// <summary>Returns an object to its pool by tag.</summary>
    public void ReturnToPool(string tag, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"[ObjectPool] Pool '{tag}' not found. Destroying object instead.");
            Destroy(obj);
            return;
        }
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        poolDictionary[tag].Enqueue(obj);
    }
}
