using UnityEngine;
using UnityEditor;

/// <summary>
/// M4 Egypt Level Integration — one-click wiring tool.
///
/// Run via: Tools > Camel Runner > Setup Egypt Level Scene
///
/// Workflow:
///   1. Run "Build Placeholder Assets" first (creates prefabs in Assets/Prefabs/Placeholder/).
///   2. Open your Gameplay scene.
///   3. Run this tool. It:
///        a. Calls PlaceholderSceneSetup to wire base gameplay objects.
///        b. Applies Egypt-specific lighting (warm golden-hour).
///        c. Sets ambient trilight for desert sky / horizon / sand.
///        d. Configures LevelGenerator to use Egypt chunk weights.
///        e. Expands ObstacleSpawner egypt tags to all five Egypt variants.
///        f. Removes generic background props and places parallax landmark set
///           (far pyramid, mid pyramid, sphinx stand-in, obelisk accents).
///   4. Press Play — verify 60 FPS, correct obstacle spawning, parallax depth.
/// </summary>
public static class EgyptSceneSetup
{
    private const string PrefabDir = "Assets/Prefabs/Placeholder";

    // All five Egypt obstacle variants (Ruins added in M4)
    private static readonly string[] EgyptObstacleTags =
    {
        "Obstacle_Rock",
        "Obstacle_Cactus",
        "Obstacle_Pillar",
        "Obstacle_Pyramid",
        "Obstacle_Ruins",
    };

    // ── Entry point ───────────────────────────────────────────────────────

    [MenuItem("Tools/Camel Runner/Setup Egypt Level Scene")]
    public static void SetupEgyptScene()
    {
        // 1. Base gameplay wiring (pools, player, singletons, camera, ground)
        PlaceholderSceneSetup.SetupScene();

        // 2. Egypt-specific overrides (lighting, level config, background)
        ApplyEgyptLighting();
        ConfigureEgyptLevelGenerator();
        ConfigureEgyptObstacleSpawners();
        PlaceEgyptBackground();

        Debug.Log("[EgyptSceneSetup] M4 Egypt integration complete. Press Play to test.");

        EditorUtility.DisplayDialog(
            "Egypt Level Ready",
            "M4 Egypt integration complete.\n\n" +
            "Configured:\n" +
            "  \u2713 Golden-hour directional light + ambient trilight\n" +
            "  \u2713 LevelGenerator: 3:1 straight-to-curved chunk ratio\n" +
            "  \u2713 ObstacleSpawner: Rock, Cactus, Pillar, Pyramid, Ruins\n" +
            "  \u2713 Parallax background: 2x pyramids + sphinx + obelisk\n\n" +
            "Press Play. Verify:\n" +
            "  - 60 FPS in Stats panel\n" +
            "  - Obstacles spawn in correct lanes\n" +
            "  - Background scrolls at depth (parallax visible)\n" +
            "  - Collectibles pickupable across all lanes",
            "Got it");
    }

    // ── Egypt lighting ────────────────────────────────────────────────────

    /// <summary>
    /// Sets directional light to warm desert golden-hour (~3 PM Egyptian sun).
    /// Sets ambient trilight: warm-blue sky / hot horizon glow / cool sand shadow.
    /// </summary>
    private static void ApplyEgyptLighting()
    {
        Light dirLight = Object.FindObjectOfType<Light>();
        if (dirLight != null && dirLight.type == LightType.Directional)
        {
            dirLight.color     = new Color(1.00f, 0.78f, 0.42f); // warm gold
            dirLight.intensity = 1.05f;
            // Low sun angle from south-east — casts long desert shadows
            dirLight.transform.eulerAngles = new Vector3(28f, -45f, 0f);
            EditorUtility.SetDirty(dirLight.gameObject);
        }
        else
        {
            Debug.LogWarning("[EgyptSceneSetup] No Directional Light found — skipping light config.");
        }

        // Trilight ambient: sky (blue-warm) / equator (horizon glow) / ground (dark sand)
        RenderSettings.ambientMode         = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor     = new Color(0.55f, 0.72f, 1.00f); // warm azure sky
        RenderSettings.ambientEquatorColor = new Color(0.90f, 0.68f, 0.38f); // horizon heat shimmer
        RenderSettings.ambientGroundColor  = new Color(0.35f, 0.28f, 0.18f); // shadowed desert floor
        RenderSettings.ambientIntensity    = 0.80f;
    }

    // ── LevelGenerator ────────────────────────────────────────────────────

    /// <summary>
    /// Configures the LevelGenerator with Egypt-biased chunk weights:
    /// 3 straight : 1 curved — keeps the endless runner feeling fast and open.
    /// </summary>
    private static void ConfigureEgyptLevelGenerator()
    {
        LevelGenerator levelGen = Object.FindObjectOfType<LevelGenerator>();
        if (levelGen == null)
        {
            Debug.LogWarning("[EgyptSceneSetup] LevelGenerator not found — skipping chunk tag config.");
            return;
        }

        levelGen.chunkPoolTags = new[]
        {
            "RoadChunk_Straight",
            "RoadChunk_Straight",
            "RoadChunk_Straight",
            "RoadChunk_Curved",
        };

        EditorUtility.SetDirty(levelGen.gameObject);
    }

    // ── ObstacleSpawner ───────────────────────────────────────────────────

    /// <summary>
    /// Sets egypt obstacle tags on every ObstacleSpawner in the scene.
    /// Chunks have a SpawnerAnchor child that hosts ObstacleSpawner — we update
    /// all instances so newly spawned chunks also pick up the full Egypt set.
    /// </summary>
    private static void ConfigureEgyptObstacleSpawners()
    {
        ObstacleSpawner[] spawners = Object.FindObjectsOfType<ObstacleSpawner>();
        if (spawners.Length == 0)
        {
            Debug.LogWarning("[EgyptSceneSetup] No ObstacleSpawner found in scene. " +
                             "They will still be configured via prefab inspector.");
            return;
        }

        foreach (ObstacleSpawner spawner in spawners)
        {
            spawner.egyptObstacleTags = EgyptObstacleTags;
            EditorUtility.SetDirty(spawner.gameObject);
        }

        Debug.Log($"[EgyptSceneSetup] Updated {spawners.Length} ObstacleSpawner(s) with " +
                  $"{EgyptObstacleTags.Length} Egypt obstacle tags.");
    }

    // ── Background parallax landmarks ─────────────────────────────────────

    /// <summary>
    /// Removes the generic props placed by PlaceholderSceneSetup and replaces them
    /// with Egypt-specific parallax landmarks at staggered depths and parallax factors.
    ///
    /// Parallax factors:
    ///   0.10  far pyramid  (appears nearly stationary — huge, distant)
    ///   0.12  mid pyramid
    ///   0.18  sphinx placeholder (obelisk until art lands)
    ///   0.22  obelisk accent
    /// </summary>
    private static void PlaceEgyptBackground()
    {
        RemovePlaceholderSetupProps();

        // Far pyramid — left, large, very slow scroll
        PlaceParallaxProp("Prop_Pyramid",
            "[EgyptBG] Pyramid_Far_Left",
            position:      new Vector3(-22f, 0f, 130f),
            scale:         Vector3.one * 4.5f,
            parallaxFactor: 0.10f,
            resetZ:        320f);

        // Mid pyramid — right, smaller
        PlaceParallaxProp("Prop_Pyramid",
            "[EgyptBG] Pyramid_Mid_Right",
            position:      new Vector3(26f, 0f, 90f),
            scale:         Vector3.one * 2.8f,
            parallaxFactor: 0.12f,
            resetZ:        270f);

        // Sphinx stand-in: a wide-scaled Obelisk until the Sphinx art asset lands.
        // When the real Sphinx prefab is imported, swap "Prop_Obelisk" → "Prop_Sphinx".
        PlaceParallaxProp("Prop_Obelisk",
            "[EgyptBG] Sphinx_Placeholder_Left",
            position:      new Vector3(-15f, 0f, 60f),
            scale:         new Vector3(2.2f, 3.5f, 2.2f),
            parallaxFactor: 0.18f,
            resetZ:        230f);

        // Obelisk accent — right, near mid-ground
        PlaceParallaxProp("Prop_Obelisk",
            "[EgyptBG] Obelisk_Right",
            position:      new Vector3(14f, 0f, 48f),
            scale:         Vector3.one * 1.3f,
            parallaxFactor: 0.22f,
            resetZ:        215f);
    }

    /// <summary>Destroys props placed by PlaceholderSceneSetup so we don't double them.</summary>
    private static void RemovePlaceholderSetupProps()
    {
        GameObject[] all = Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in all)
        {
            // PlaceholderSceneSetup names them "[PlaceholderSetup] Prop_*"
            if (go != null && go.name.StartsWith("[PlaceholderSetup] Prop_"))
                Object.DestroyImmediate(go);
        }
    }

    private static void PlaceParallaxProp(string prefabName, string goName,
        Vector3 position, Vector3 scale, float parallaxFactor, float resetZ)
    {
        string path   = PrefabDir + "/" + prefabName + ".prefab";
        var    prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogWarning($"[EgyptSceneSetup] Prefab not found: {path}");
            return;
        }

        var go               = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        go.name              = goName;
        go.transform.position   = position;
        go.transform.localScale = scale;

        // Attach parallax scrolling component
        var parallax             = go.AddComponent<ParallaxBackground>();
        parallax.parallaxFactor  = parallaxFactor;
        parallax.resetZ          = resetZ;
        parallax.cullZ           = -30f;
    }
}
