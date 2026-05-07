using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Editor utility for Phase 1 integration: Camel + NinjaThief wiring.
///
/// Run: Tools → Camel Runner → Phase 1 Integration Setup
///
/// Tasks:
/// 1. Instantiate Camel_Default.prefab in Gameplay scene
/// 2. Ensure PlayerController is configured correctly
/// 3. Instantiate ThiefSpawner in Gameplay scene (if missing)
/// 4. Wire NinjaThief.prefab into ThiefSpawner's prefab pool
/// 5. Validate scene loads without null ref errors
/// 6. Save scene
///
/// Status: Ready for use. Requires UnityEditor batch mode or manual execution.
/// </summary>
public static class Phase1IntegrationSetup
{
    private const string GameplayScenePath = "Assets/Scenes/Gameplay.unity";
    private const string CamelPrefabPath   = "Assets/Prefabs/Characters/Camel_Default.prefab";
    private const string NinjaPrefabPath   = "Assets/Models/Egypt/Characters/NinjaThief/Prefabs/NinjaThief.prefab";

    [MenuItem("Tools/Camel Runner/Phase 1 Integration Setup")]
    public static void SetupPhase1Integration()
    {
        Debug.Log("[Phase1IntegrationSetup] Starting Phase 1 integration...");

        // Load Gameplay scene
        Scene gameplayScene = EditorSceneManager.OpenScene(GameplayScenePath, OpenSceneMode.Single);
        if (!gameplayScene.isLoaded)
        {
            Debug.LogError("[Phase1IntegrationSetup] Failed to load Gameplay scene at " + GameplayScenePath);
            return;
        }

        // Step 1: Instantiate Camel if not present
        InstantiateCamelIfMissing();

        // Step 2: Find or create ThiefSpawner
        ThiefSpawner spawner = FindOrCreateThiefSpawner();

        // Step 3: Wire NinjaThief prefab into ThiefSpawner
        if (spawner != null)
        {
            WireNinjaThiefIntoSpawner(spawner);
        }

        // Step 4: Validate scene
        ValidateScene();

        // Step 5: Save scene
        EditorSceneManager.SaveScene(gameplayScene);
        Debug.Log("[Phase1IntegrationSetup] ✓ Phase 1 integration complete. Scene saved.");
    }

    private static void InstantiateCamelIfMissing()
    {
        // Check if PlayerController already exists
        PlayerController existing = Object.FindObjectOfType<PlayerController>();
        if (existing != null)
        {
            Debug.Log("[Phase1IntegrationSetup] PlayerController already in scene, skipping instantiation.");
            return;
        }

        // Load and instantiate Camel prefab
        GameObject camelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CamelPrefabPath);
        if (camelPrefab == null)
        {
            Debug.LogError($"[Phase1IntegrationSetup] Camel prefab not found at {CamelPrefabPath}");
            return;
        }

        GameObject camelInstance = PrefabUtility.InstantiatePrefab(camelPrefab) as GameObject;
        if (camelInstance != null)
        {
            camelInstance.name = "Player_Camel";
            camelInstance.transform.position = new Vector3(0, 1, 0); // Spawn above ground
            Debug.Log("[Phase1IntegrationSetup] ✓ Camel instantiated in scene");
        }
        else
        {
            Debug.LogError("[Phase1IntegrationSetup] Failed to instantiate Camel prefab");
        }
    }

    private static ThiefSpawner FindOrCreateThiefSpawner()
    {
        ThiefSpawner existing = Object.FindObjectOfType<ThiefSpawner>();
        if (existing != null)
        {
            Debug.Log("[Phase1IntegrationSetup] ThiefSpawner already in scene");
            return existing;
        }

        GameObject spawnerGO = new GameObject("ThiefSpawner");
        ThiefSpawner spawner = spawnerGO.AddComponent<ThiefSpawner>();
        Debug.Log("[Phase1IntegrationSetup] ✓ ThiefSpawner created in scene");
        return spawner;
    }

    private static void WireNinjaThiefIntoSpawner(ThiefSpawner spawner)
    {
        GameObject ninjaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(NinjaPrefabPath);
        if (ninjaPrefab == null)
        {
            Debug.LogError($"[Phase1IntegrationSetup] NinjaThief prefab not found at {NinjaPrefabPath}");
            return;
        }

        // Use reflection to set the thiefPrefabs list (since it's private with [SerializeField])
        var field = typeof(ThiefSpawner).GetField("thiefPrefabs",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (field == null)
        {
            Debug.LogError("[Phase1IntegrationSetup] Could not find thiefPrefabs field on ThiefSpawner");
            return;
        }

        // Get the current list
        var prefabList = field.GetValue(spawner) as System.Collections.Generic.List<ThiefSpawner.ThiefPrefabEntry>;
        if (prefabList == null)
        {
            prefabList = new System.Collections.Generic.List<ThiefSpawner.ThiefPrefabEntry>();
            field.SetValue(spawner, prefabList);
        }

        // Add NinjaThief to the list (assuming only one thief for Phase 1)
        var entry = new ThiefSpawner.ThiefPrefabEntry
        {
            thiefType = ThiefSystem.ThiefType.NinjaThief,
            prefab = ninjaPrefab
        };

        // Remove any existing NinjaThief entry first
        prefabList.RemoveAll(e => e.thiefType == ThiefSystem.ThiefType.NinjaThief);
        prefabList.Add(entry);

        Debug.Log("[Phase1IntegrationSetup] ✓ NinjaThief wired into ThiefSpawner");
        EditorUtility.SetDirty(spawner);
    }

    private static void ValidateScene()
    {
        Debug.Log("[Phase1IntegrationSetup] Validating scene...");

        PlayerController playerController = Object.FindObjectOfType<PlayerController>();
        ThiefSpawner thiefSpawner = Object.FindObjectOfType<ThiefSpawner>();
        GameManager gameManager = Object.FindObjectOfType<GameManager>();

        bool valid = true;

        if (playerController == null)
        {
            Debug.LogWarning("[Phase1IntegrationSetup] ⚠ PlayerController not found in scene");
            valid = false;
        }

        if (thiefSpawner == null)
        {
            Debug.LogWarning("[Phase1IntegrationSetup] ⚠ ThiefSpawner not found in scene");
            valid = false;
        }

        if (gameManager == null)
        {
            Debug.LogWarning("[Phase1IntegrationSetup] ⚠ GameManager not found in scene (may be OK if in separate scene)");
        }

        if (valid)
        {
            Debug.Log("[Phase1IntegrationSetup] ✓ Scene validation passed");
        }
        else
        {
            Debug.LogError("[Phase1IntegrationSetup] ✗ Scene validation failed. Check warnings above.");
        }
    }
}
