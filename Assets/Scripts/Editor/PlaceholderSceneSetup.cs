using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Wires the Gameplay scene for end-to-end testing with placeholder assets.
///
/// Run AFTER "Build Placeholder Assets":
///   Tools > Camel Runner > Setup Placeholder Scene
///
/// What it sets up:
///   - GameManager, ObjectPool (all placeholder pools wired)
///   - CameraController, AudioManager, DifficultyManager, HealthSystem, CollectibleSystem
///   - LevelGenerator (chunk tags configured)
///   - Player (Camel_Placeholder prefab instance)
///   - Background props (Pyramid, Obelisks)
///   - Directional Light + Camera
///
/// Existing objects are cleared before setup to allow re-runs.
/// </summary>
public static class PlaceholderSceneSetup
{
    private const string PrefabDir = "Assets/Prefabs/Placeholder";

    // ── Pool configuration ────────────────────────────────────────────────

    // (tag, prefabName, initialSize)
    private static readonly (string tag, string prefabName, int size)[] PoolConfig =
    {
        // Road chunks
        ("RoadChunk_Straight", "RoadChunk_Straight", 4),
        ("RoadChunk_Curved",   "RoadChunk_Curved",   4),

        // Obstacles
        ("Obstacle_Rock",    "Obstacle_Rock",    6),
        ("Obstacle_Cactus",  "Obstacle_Cactus",  6),
        ("Obstacle_Pillar",  "Obstacle_Pillar",  4),
        ("Obstacle_Pyramid", "Obstacle_Pyramid", 4),

        // Collectibles
        ("Collectible_Date",       "Collectible_Date",       10),
        ("Collectible_SilverCoin", "Collectible_SilverCoin",  6),
        ("Collectible_GoldenDate", "Collectible_GoldenDate",  4),
        ("Collectible_Gem",        "Collectible_Gem",          3),
        ("Collectible_MysteryBox", "Collectible_MysteryBox",   2),
    };

    // ── Entry point ───────────────────────────────────────────────────────

    [MenuItem("Tools/Camel Runner/Setup Placeholder Scene")]
    public static void SetupScene()
    {
        if (!ValidatePrefabsExist())
        {
            EditorUtility.DisplayDialog("Missing Prefabs",
                "Placeholder prefabs not found.\n\nRun 'Tools > Camel Runner > Build Placeholder Assets' first.",
                "OK");
            return;
        }

        // Clear previous placeholder setup objects so this is re-runnable
        ClearExistingSetup();

        // ── Core singletons ──────────────────────────────────────────────
        var gameManagerGO = CreateSingleton<GameManager>("GameManager");
        var objectPoolGO  = CreateSingleton<ObjectPool>("ObjectPool");
        CreateSingleton<DifficultyManager>("DifficultyManager");
        CreateSingleton<HealthSystem>("HealthSystem");
        CreateSingleton<CollectibleSystem>("CollectibleSystem");
        CreateSingleton<PowerUpManager>("PowerUpManager");

        // AudioManager (optional, may not be present in all scenes)
        if (FindAnyComponentInScene<AudioManager>() == null)
        {
            var audioGO = new GameObject("[PlaceholderSetup] AudioManager");
            audioGO.AddComponent<AudioManager>();
        }

        // ── ObjectPool pools ─────────────────────────────────────────────
        ConfigureObjectPool(objectPoolGO.GetComponent<ObjectPool>());

        // ── Camera ──────────────────────────────────────────────────────
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            var camGO = new GameObject("[PlaceholderSetup] Main Camera");
            camGO.tag = "MainCamera";
            mainCam   = camGO.AddComponent<Camera>();
            camGO.AddComponent<AudioListener>();
        }
        mainCam.transform.position = new Vector3(0f, 4f, -8f);
        mainCam.transform.LookAt(new Vector3(0f, 1f, 0f));

        // Attach CameraController if not already present
        if (mainCam.GetComponent<CameraController>() == null)
            mainCam.gameObject.AddComponent<CameraController>();

        // ── Directional light ────────────────────────────────────────────
        if (Object.FindObjectOfType<Light>() == null)
        {
            var lightGO = new GameObject("[PlaceholderSetup] Directional Light");
            var l       = lightGO.AddComponent<Light>();
            l.type      = LightType.Directional;
            l.intensity = 1.2f;
            lightGO.transform.eulerAngles = new Vector3(50f, -30f, 0f);
        }

        // ── Player (Camel) ───────────────────────────────────────────────
        var camelPrefab = LoadPrefab("Camel_Placeholder");
        GameObject player = null;
        if (camelPrefab != null)
        {
            player = (GameObject)PrefabUtility.InstantiatePrefab(camelPrefab);
            player.name = "[PlaceholderSetup] Player";
            player.transform.position = new Vector3(0f, 0f, 0f);
            player.tag = "Player";

            // Point camera at camel
            if (mainCam != null)
            {
                var cc = mainCam.GetComponent<CameraController>();
                if (cc != null)
                    cc.target = player.transform;
            }
        }
        else
        {
            Debug.LogWarning("[PlaceholderSceneSetup] Camel_Placeholder prefab not found. Skipping player setup.");
        }

        // ── Level Generator ──────────────────────────────────────────────
        var levelGenGO = new GameObject("[PlaceholderSetup] LevelGenerator");
        var levelGen   = levelGenGO.AddComponent<LevelGenerator>();
        levelGen.chunkPoolTags    = new[] { "RoadChunk_Straight", "RoadChunk_Straight", "RoadChunk_Curved" };
        levelGen.chunkLength      = 50f;
        levelGen.chunksAhead      = 3;
        levelGen.initialScrollSpeed = 10f;

        // ── Background props ─────────────────────────────────────────────
        PlaceBackgroundProps();

        // ── Ground plane (so camel has something to stand on) ─────────────
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "[PlaceholderSetup] Ground";
        ground.transform.position   = new Vector3(0f, -0.05f, 50f);
        ground.transform.localScale = new Vector3(10f, 1f, 200f);

        // ── Finish ───────────────────────────────────────────────────────
        Debug.Log("[PlaceholderSceneSetup] Scene wired. Press Play — " +
                  "tap the game view then press Space/Start to begin.");

        EditorUtility.DisplayDialog("Scene Ready",
            "Placeholder Gameplay scene is configured.\n\n" +
            "Press Play. In the Game view:\n" +
            "  - Arrow keys: move lanes / jump / slide\n" +
            "  - Click the GameManager.StartGame() button (or hook up a UI button)\n\n" +
            "All pools, spawners, and the player camel are live.",
            "Got it");
    }

    // ── ObjectPool wiring ─────────────────────────────────────────────────

    private static void ConfigureObjectPool(ObjectPool pool)
    {
        if (pool == null) return;

        var poolList = new List<ObjectPool.Pool>();
        foreach (var (tag, prefabName, size) in PoolConfig)
        {
            var prefab = LoadPrefab(prefabName);
            if (prefab == null)
            {
                Debug.LogWarning($"[PlaceholderSceneSetup] Missing prefab '{prefabName}' for pool tag '{tag}'.");
                continue;
            }
            poolList.Add(new ObjectPool.Pool
            {
                tag         = tag,
                prefab      = prefab,
                initialSize = size,
            });
        }
        pool.pools = poolList;
        EditorUtility.SetDirty(pool.gameObject);
    }

    // ── Background props ──────────────────────────────────────────────────

    private static void PlaceBackgroundProps()
    {
        PlaceProp("Prop_Pyramid", new Vector3(-18f, 0f,  60f), Vector3.one * 3f);
        PlaceProp("Prop_Pyramid", new Vector3( 20f, 0f, 120f), Vector3.one * 2f);
        PlaceProp("Prop_Obelisk", new Vector3(-10f, 0f,  40f), Vector3.one);
        PlaceProp("Prop_Obelisk", new Vector3( 10f, 0f,  40f), Vector3.one);
        PlaceProp("Prop_SkyPlane",new Vector3(0f, 25f, 200f),  Vector3.one);
    }

    private static void PlaceProp(string prefabName, Vector3 pos, Vector3 scale)
    {
        var prefab = LoadPrefab(prefabName);
        if (prefab == null)
        {
            Debug.LogWarning($"[PlaceholderSceneSetup] Prop prefab '{prefabName}' not found.");
            return;
        }
        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        go.name = "[PlaceholderSetup] " + prefabName;
        go.transform.position   = pos;
        go.transform.localScale = scale;
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private static GameObject CreateSingleton<T>(string name) where T : Component
    {
        var existing = Object.FindObjectOfType<T>();
        if (existing != null) return existing.gameObject;

        var go = new GameObject("[PlaceholderSetup] " + name);
        go.AddComponent<T>();
        return go;
    }

    private static T FindAnyComponentInScene<T>() where T : Component
        => Object.FindObjectOfType<T>();

    private static GameObject LoadPrefab(string prefabName)
        => AssetDatabase.LoadAssetAtPath<GameObject>(PrefabDir + "/" + prefabName + ".prefab");

    private static bool ValidatePrefabsExist()
    {
        // Quick check: just verify the camel placeholder exists
        return LoadPrefab("Camel_Placeholder") != null;
    }

    /// <summary>Removes GameObjects created by a previous run of this tool.</summary>
    private static void ClearExistingSetup()
    {
        var all = Object.FindObjectsOfType<GameObject>();
        foreach (var go in all)
        {
            if (go != null && go.name.StartsWith("[PlaceholderSetup]"))
                Object.DestroyImmediate(go);
        }
    }
}
