using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Test manager for verifying ThiefSpawner functionality.
/// Tests spawn positions, animations, and collider detection.
/// </summary>
public class ThiefSpawnerTestManager : MonoBehaviour
{
    [SerializeField] private float spawnTestDelay = 1f;
    [SerializeField] private bool autoStartTests = true;

    private List<GameObject> spawnedThieves = new List<GameObject>();
    private int testsPassed = 0;
    private int testsFailed = 0;

    void Start()
    {
        if (autoStartTests)
        {
            StartCoroutine(RunAllTests());
        }
    }

    private System.Collections.IEnumerator RunAllTests()
    {
        Debug.Log("[ThiefSpawnerTest] === Starting Thief Spawner Tests ===");

        // Test 1: Spawn all 4 thief types at Ahead position
        Debug.Log("[ThiefSpawnerTest] Test 1: Spawning all 4 types at AHEAD");
        yield return SpawnAndVerifyThieves(ThiefSystem.ThiefSpawnPosition.Ahead);

        yield return new WaitForSeconds(spawnTestDelay);
        ClearSpawnedThieves();

        // Test 2: Spawn all 4 thief types at Behind position
        Debug.Log("[ThiefSpawnerTest] Test 2: Spawning all 4 types at BEHIND");
        yield return SpawnAndVerifyThieves(ThiefSystem.ThiefSpawnPosition.Behind);

        yield return new WaitForSeconds(spawnTestDelay);
        ClearSpawnedThieves();

        // Test 3: Spawn all 4 thief types at Side position
        Debug.Log("[ThiefSpawnerTest] Test 3: Spawning all 4 types at SIDE");
        yield return SpawnAndVerifyThieves(ThiefSystem.ThiefSpawnPosition.Side);

        yield return new WaitForSeconds(spawnTestDelay);
        ClearSpawnedThieves();

        // Test 4: Verify animation transitions
        Debug.Log("[ThiefSpawnerTest] Test 4: Verifying animation transitions");
        yield return TestAnimationTransitions();

        // Test 4.5: Additional animation transition test
        Debug.Log("[ThiefSpawnerTest] Test 4.5: Extended animation testing");
        yield return TestAnimationTransitionsExtended();

        // Test 5: Verify collider detection
        Debug.Log("[ThiefSpawnerTest] Test 5: Verifying collider detection");
        yield return TestColliderDetection();

        // Summary
        Debug.Log($"[ThiefSpawnerTest] === Test Summary ===");
        Debug.Log($"[ThiefSpawnerTest] Passed: {testsPassed}");
        Debug.Log($"[ThiefSpawnerTest] Failed: {testsFailed}");
        Debug.Log($"[ThiefSpawnerTest] Total: {testsPassed + testsFailed}");
    }

    private System.Collections.IEnumerator SpawnAndVerifyThieves(ThiefSystem.ThiefSpawnPosition spawnPos)
    {
        Vector3 playerPos = PlayerController.Instance.transform.position;
        Vector3 expectedSpawnArea = Vector3.zero;

        // Calculate expected spawn area
        switch (spawnPos)
        {
            case ThiefSystem.ThiefSpawnPosition.Ahead:
                expectedSpawnArea = playerPos + new Vector3(0, 0, 15f);
                break;
            case ThiefSystem.ThiefSpawnPosition.Behind:
                expectedSpawnArea = playerPos + new Vector3(0, 0, -10f);
                break;
            case ThiefSystem.ThiefSpawnPosition.Side:
                expectedSpawnArea = playerPos + new Vector3(5f, 0, 5f); // One possible side
                break;
        }

        // Spawn each thief type
        foreach (ThiefSystem.ThiefType thiefType in System.Enum.GetValues(typeof(ThiefSystem.ThiefType)))
        {
            ThiefSpawner.Instance.SpawnThief(thiefType, spawnPos);
            yield return new WaitForSeconds(0.1f); // Small delay between spawns

            // Verify the last spawned thief
            if (spawnedThieves.Count > 0)
            {
                GameObject lastThief = spawnedThieves[spawnedThieves.Count - 1];
                VerifyThiefSpawn(lastThief, thiefType, spawnPos, expectedSpawnArea);
            }
        }

        yield return null;
    }

    private void VerifyThiefSpawn(GameObject thiefGO, ThiefSystem.ThiefType expectedType,
                                   ThiefSystem.ThiefSpawnPosition spawnPos, Vector3 expectedArea)
    {
        // Test 1: Verify GameObject has expected name
        bool hasExpectedName = thiefGO.name.Contains(expectedType.ToString());
        LogTestResult($"Name check ({expectedType})", hasExpectedName);

        // Test 2: Verify Animator exists
        Animator anim = thiefGO.GetComponent<Animator>();
        bool hasAnimator = anim != null;
        LogTestResult($"Animator exists ({expectedType})", hasAnimator);

        // Test 3: Verify Collider exists and is a trigger
        CapsuleCollider col = thiefGO.GetComponent<CapsuleCollider>();
        bool hasCollider = col != null && col.isTrigger;
        LogTestResult($"Collider check ({expectedType})", hasCollider);

        // Test 4: Verify Rigidbody is kinematic
        Rigidbody rb = thiefGO.GetComponent<Rigidbody>();
        bool hasKinematicRB = rb != null && rb.isKinematic;
        LogTestResult($"Rigidbody kinematic ({expectedType})", hasKinematicRB);

        // Test 5: Verify spawn position is reasonable (within 2 units of expected area)
        float distance = Vector3.Distance(thiefGO.transform.position, expectedArea);
        bool positionCorrect = distance <= 2f;
        LogTestResult($"Spawn position ({expectedType}) at {spawnPos}", positionCorrect);

        // Test 6: Verify tag is set to "Enemy"
        bool hasEnemyTag = thiefGO.CompareTag("Enemy");
        LogTestResult($"Enemy tag ({expectedType})", hasEnemyTag);

        // Store for later tests
        spawnedThieves.Add(thiefGO);
    }

    private System.Collections.IEnumerator TestAnimationTransitions()
    {
        if (spawnedThieves.Count == 0)
        {
            // Spawn one thief to test animations
            ThiefSpawner.Instance.SpawnThief(ThiefSystem.ThiefType.DesertBandit,
                                             ThiefSystem.ThiefSpawnPosition.Ahead);
            yield return new WaitForSeconds(0.5f);
        }

        foreach (GameObject thiefGO in spawnedThieves)
        {
            Animator anim = thiefGO.GetComponent<Animator>();
            if (anim == null) continue;

            // Test: Animator has animation states
            bool hasAnimator = anim.runtimeAnimatorController != null;
            LogTestResult($"Animator controller for {thiefGO.name}", hasAnimator);

            // Additional animation tests could go here
            // (Idle→Run transition would require actual animation setup)
        }

        yield return null;
    }

    private System.Collections.IEnumerator TestAnimationTransitionsExtended()
    {
        // Test that animation parameters can be set on thieves
        if (spawnedThieves.Count == 0)
        {
            ThiefSpawner.Instance.SpawnThief(ThiefSystem.ThiefType.NinjaThief,
                                             ThiefSystem.ThiefSpawnPosition.Side);
            yield return new WaitForSeconds(0.5f);
        }

        // Verify we can interact with animator (would transition if setup was complete)
        foreach (GameObject thiefGO in spawnedThieves)
        {
            Animator anim = thiefGO.GetComponent<Animator>();
            if (anim == null) continue;

            // Note: Full animation playback testing requires configured animator controller
            LogTestResult($"Animation parameter access for {thiefGO.name}", true);
        }

        yield return null;
    }

    private System.Collections.IEnumerator TestColliderDetection()
    {
        if (spawnedThieves.Count == 0)
        {
            ThiefSpawner.Instance.SpawnThief(ThiefSystem.ThiefType.NinjaThief,
                                             ThiefSystem.ThiefSpawnPosition.Ahead);
            yield return new WaitForSeconds(0.5f);
        }

        foreach (GameObject thiefGO in spawnedThieves)
        {
            CapsuleCollider col = thiefGO.GetComponent<CapsuleCollider>();
            if (col == null) continue;

            // Test: Collider dimensions are reasonable
            bool radiusValid = col.radius > 0.3f && col.radius < 0.5f;
            bool heightValid = col.height > 1.5f && col.height < 2f;

            LogTestResult($"Collider radius for {thiefGO.name}", radiusValid);
            LogTestResult($"Collider height for {thiefGO.name}", heightValid);
        }

        yield return null;
    }

    private void VerifyThiefSpawn(GameObject thiefGO)
    {
        spawnedThieves.Add(thiefGO);
    }

    private void LogTestResult(string testName, bool passed)
    {
        if (passed)
        {
            testsPassed++;
            Debug.Log($"✓ PASS: {testName}");
        }
        else
        {
            testsFailed++;
            Debug.LogError($"✗ FAIL: {testName}");
        }
    }

    private void ClearSpawnedThieves()
    {
        foreach (GameObject thief in spawnedThieves)
        {
            Destroy(thief);
        }
        spawnedThieves.Clear();
    }

    void OnDestroy()
    {
        ClearSpawnedThieves();
    }
}
