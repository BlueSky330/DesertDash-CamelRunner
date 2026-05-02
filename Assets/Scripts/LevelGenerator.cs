using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] levelChunks;
    public float spawnZ = 50f;
    public float chunkLength = 50f;
    public int chunksOnScreen = 3;

    private List<GameObject> activeChunks = new List<GameObject>();
    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        for (int i = 0; i < chunksOnScreen; i++)
        {
            SpawnChunk();
        }
    }

    void Update()
    {
        if (playerTransform.position.z - chunkLength > spawnZ - (chunksOnScreen * chunkLength))
        {
            SpawnChunk();
            DeleteChunk();
        }
    }

    void SpawnChunk()
    {
        GameObject chunk = Instantiate(levelChunks[Random.Range(0, levelChunks.Length)], transform.forward * spawnZ, transform.rotation);
        activeChunks.Add(chunk);
        spawnZ += chunkLength;
    }

    void DeleteChunk()
    {
        Destroy(activeChunks[0]);
        activeChunks.RemoveAt(0);
    }
}
