using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

/// <summary>
/// Editor utility that creates all placeholder prefabs required by M4 Integration.
///
/// Run via: Tools > Camel Runner > Build Placeholder Assets
///
/// Creates in Assets/Prefabs/Placeholder/:
///   - Camel (player) with CharacterController + Animator stub
///   - RoadChunk_Straight, RoadChunk_Curved (level chunks)
///   - Obstacle_Rock, Obstacle_Cactus, Obstacle_Pillar
///   - Collectible_Date, Collectible_SilverCoin, Collectible_GoldenDate, Collectible_Gem, Collectible_MysteryBox
///   - Prop_Pyramid, Prop_Obelisk, Prop_Sky
/// </summary>
public static class PlaceholderAssetBuilder
{
    private const string PrefabDir  = "Assets/Prefabs/Placeholder";
    private const string MatDir     = "Assets/Prefabs/Placeholder/Materials";

    // ── Entry point ───────────────────────────────────────────────────────

    [MenuItem("Tools/Camel Runner/Build Placeholder Assets")]
    public static void BuildAll()
    {
        EnsureFolders();

        BuildCamel();
        BuildRoadChunks();
        BuildObstacles();
        BuildCollectibles();
        BuildBackgroundProps();
        BuildPlaceholderAnimatorController();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[PlaceholderAssetBuilder] All placeholder prefabs created in " + PrefabDir);
        EditorUtility.DisplayDialog(
            "Placeholder Assets Built",
            "All placeholder prefabs created in:\n" + PrefabDir +
            "\n\nNext step: Open Gameplay scene and run 'Tools > Camel Runner > Setup Placeholder Scene'.",
            "OK");
    }

    // ── Folder setup ──────────────────────────────────────────────────────

    private static void EnsureFolders()
    {
        EnsureFolder("Assets/Prefabs");
        EnsureFolder(PrefabDir);
        EnsureFolder(MatDir);
    }

    private static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            string folderName = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }

    // ── Camel placeholder ─────────────────────────────────────────────────

    private static void BuildCamel()
    {
        // Root: empty transform that PlayerController/CharacterController attach to
        var root = new GameObject("Camel_Placeholder");

        // CharacterController
        var cc = root.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.radius = 0.4f;
        cc.center = new Vector3(0f, 0.9f, 0f);

        // Animator (controller assigned after we build the AnimatorController asset)
        root.AddComponent<Animator>();

        // PlayerController
        root.AddComponent<PlayerController>();

        // Visual body: capsule (body)
        var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Body";
        SetMat(body, MakeMat("CamelBody", new Color(0.76f, 0.60f, 0.42f)));
        DestroyCollider(body);
        body.transform.SetParent(root.transform);
        body.transform.localPosition = new Vector3(0f, 0.9f, 0f);
        body.transform.localScale    = new Vector3(0.6f, 0.55f, 0.9f);

        // Head: sphere
        var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        SetMat(head, GetMat("CamelBody"));
        DestroyCollider(head);
        head.transform.SetParent(root.transform);
        head.transform.localPosition = new Vector3(0f, 2.0f, 0.4f);
        head.transform.localScale    = new Vector3(0.45f, 0.45f, 0.45f);

        // Neck: cylinder
        var neck = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        neck.name = "Neck";
        SetMat(neck, GetMat("CamelBody"));
        DestroyCollider(neck);
        neck.transform.SetParent(root.transform);
        neck.transform.localPosition = new Vector3(0f, 1.65f, 0.25f);
        neck.transform.localEulerAngles = new Vector3(30f, 0f, 0f);
        neck.transform.localScale    = new Vector3(0.18f, 0.3f, 0.18f);

        // Hump: sphere (scaled)
        var hump = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hump.name = "Hump";
        SetMat(hump, GetMat("CamelBody"));
        DestroyCollider(hump);
        hump.transform.SetParent(root.transform);
        hump.transform.localPosition = new Vector3(0f, 1.6f, -0.1f);
        hump.transform.localScale    = new Vector3(0.35f, 0.5f, 0.35f);

        // Legs: 4 cylinders
        CreateLeg(root.transform, "LegFL", new Vector3( 0.25f, 0.45f,  0.35f));
        CreateLeg(root.transform, "LegFR", new Vector3(-0.25f, 0.45f,  0.35f));
        CreateLeg(root.transform, "LegBL", new Vector3( 0.25f, 0.45f, -0.35f));
        CreateLeg(root.transform, "LegBR", new Vector3(-0.25f, 0.45f, -0.35f));

        // Tag & layer
        root.tag = "Player";

        SavePrefab(root, "Camel_Placeholder");
        Object.DestroyImmediate(root);
    }

    private static void CreateLeg(Transform parent, string legName, Vector3 localPos)
    {
        var leg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        leg.name = legName;
        SetMat(leg, GetMat("CamelBody"));
        DestroyCollider(leg);
        leg.transform.SetParent(parent);
        leg.transform.localPosition = localPos;
        leg.transform.localScale    = new Vector3(0.12f, 0.45f, 0.12f);
    }

    // ── Road chunks ───────────────────────────────────────────────────────

    private static void BuildRoadChunks()
    {
        BuildRoadChunk("RoadChunk_Straight", curved: false);
        BuildRoadChunk("RoadChunk_Curved",   curved: true);
    }

    private static void BuildRoadChunk(string chunkName, bool curved)
    {
        var root = new GameObject(chunkName);

        // Road surface
        var surface = GameObject.CreatePrimitive(PrimitiveType.Cube);
        surface.name = "Surface";
        SetMat(surface, MakeMat("Sandstone", new Color(0.87f, 0.72f, 0.53f)));
        DestroyCollider(surface);
        surface.transform.SetParent(root.transform);
        surface.transform.localPosition = new Vector3(0f, -0.05f, 25f);
        // 3 lanes = 6 units wide; chunk length = 50 units
        surface.transform.localScale    = new Vector3(6f, 0.1f, 50f);

        // Lane dividers (2 thin strips)
        CreateLaneDivider(root.transform, -2f);
        CreateLaneDivider(root.transform,  2f);

        if (curved)
        {
            // Simple visual cue: slightly rotate the road surface for "curved" variant
            surface.transform.localEulerAngles = new Vector3(0f, 8f, 0f);
        }

        // Spawn points marker (empty child for ObstacleSpawner / CollectibleSpawner to find)
        var spawnerAnchor = new GameObject("SpawnerAnchor");
        spawnerAnchor.transform.SetParent(root.transform);
        spawnerAnchor.transform.localPosition = Vector3.zero;
        spawnerAnchor.AddComponent<ObstacleSpawner>();
        spawnerAnchor.AddComponent<CollectibleSpawner>();

        SavePrefab(root, chunkName);
        Object.DestroyImmediate(root);
    }

    private static void CreateLaneDivider(Transform parent, float xOffset)
    {
        var div = GameObject.CreatePrimitive(PrimitiveType.Cube);
        div.name = "LaneDivider";
        SetMat(div, MakeMat("LaneMarking", new Color(0.95f, 0.88f, 0.65f)));
        DestroyCollider(div);
        div.transform.SetParent(parent);
        div.transform.localPosition = new Vector3(xOffset, 0f, 25f);
        div.transform.localScale    = new Vector3(0.08f, 0.01f, 50f);
    }

    // ── Obstacles ─────────────────────────────────────────────────────────

    private static void BuildObstacles()
    {
        // Rock: sphere placeholder
        BuildRock();

        // Cactus: green cylinder
        BuildObstacle("Obstacle_Cactus", PrimitiveType.Cylinder, new Color(0.13f, 0.55f, 0.13f),
            new Vector3(0.3f, 1.0f, 0.3f), new Vector3(0f, 1.0f, 0f));

        // Pillar: tall box
        BuildObstacle("Obstacle_Pillar", PrimitiveType.Cube,     new Color(0.80f, 0.70f, 0.55f),
            new Vector3(0.5f, 2.2f, 0.5f), new Vector3(0f, 1.1f, 0f));

        // Pyramid variant
        BuildObstacle("Obstacle_Pyramid", PrimitiveType.Cube,    new Color(0.87f, 0.78f, 0.55f),
            new Vector3(1.2f, 1.0f, 1.2f), new Vector3(0f, 0.5f, 0f));

        // Ruins: crumbled wall — two offset sandstone slabs stacked unevenly
        BuildRuins();
    }

    private static void BuildRock()
    {
        var root = new GameObject("Obstacle_Rock");

        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "RockMesh";
        sphere.transform.SetParent(root.transform);
        sphere.transform.localScale = new Vector3(2.0f, 1.8f, 1.8f);
        sphere.transform.localPosition = new Vector3(0f, 1.0f, 0f);
        Object.DestroyImmediate(sphere.GetComponent<SphereCollider>());
        SetMat(sphere, MakeMat("RockSurface", new Color(0.65f, 0.55f, 0.40f)));

        var box = root.AddComponent<BoxCollider>();
        box.center = new Vector3(0f, 1.0f, 0f);
        box.size   = new Vector3(2.0f, 1.8f, 1.8f);

        root.tag = "Obstacle";

        SavePrefab(root, "Obstacle_Rock");
        Object.DestroyImmediate(root);
    }

    private static void BuildRuins()
    {
        var root = new GameObject("Obstacle_Ruins");

        // Use ProceduralRuinsObstacle for proper Egyptian ruins geometry
        var ruinsMesh = root.AddComponent<ProceduralRuinsObstacle>();
        ruinsMesh.Build();

        // Box collider sized to the ruins bounding volume
        var box = root.AddComponent<BoxCollider>();
        box.center = new Vector3(0f, 1.2f, 0f);
        box.size   = new Vector3(1.2f, 2.4f, 1.2f);

        root.tag = "Obstacle";

        SavePrefab(root, "Obstacle_Ruins");
        Object.DestroyImmediate(root);
    }

    private static void BuildObstacle(string obstacleName, PrimitiveType prim, Color color,
        Vector3 scale, Vector3 localPos)
    {
        var root = new GameObject(obstacleName);

        var vis = GameObject.CreatePrimitive(prim);
        vis.name = "Visual";
        SetMat(vis, MakeMat(obstacleName + "_Mat", color));
        // Keep the built-in collider (box/capsule) on the visual — it IS the hit shape
        vis.transform.SetParent(root.transform);
        vis.transform.localPosition = localPos;
        vis.transform.localScale    = scale;

        // Tag the root as obstacle so PlayerController.OnControllerColliderHit fires
        root.tag = "Obstacle";

        SavePrefab(root, obstacleName);
        Object.DestroyImmediate(root);
    }

    // ── Collectibles ──────────────────────────────────────────────────────

    private static void BuildCollectibles()
    {
        BuildCollectible("Collectible_Date",       new Color(0.45f, 0.22f, 0.05f), 0.35f,
            CollectibleSystem.CollectibleType.Date);
        BuildCollectible("Collectible_SilverCoin", new Color(0.75f, 0.75f, 0.80f), 0.30f,
            CollectibleSystem.CollectibleType.SilverCoin);
        BuildCollectible("Collectible_GoldenDate", new Color(1.00f, 0.80f, 0.00f), 0.35f,
            CollectibleSystem.CollectibleType.GoldenDate);
        BuildCollectible("Collectible_Gem",        new Color(0.00f, 0.90f, 0.95f), 0.28f,
            CollectibleSystem.CollectibleType.Gem);
        BuildCollectible("Collectible_MysteryBox", new Color(0.60f, 0.10f, 0.80f), 0.40f,
            CollectibleSystem.CollectibleType.MysteryBox);
    }

    private static void BuildCollectible(string collectibleName, Color color, float radius,
        CollectibleSystem.CollectibleType type)
    {
        var root = new GameObject(collectibleName);

        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "Visual";
        SetMat(sphere, MakeMat(collectibleName + "_Mat", color));
        sphere.transform.SetParent(root.transform);
        sphere.transform.localPosition = Vector3.zero;
        sphere.transform.localScale    = Vector3.one * (radius * 2f);
        // Remove mesh collider — we add a trigger sphere collider on root
        DestroyCollider(sphere);

        // Trigger collider on root
        var col = root.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius    = radius * 1.2f; // slightly generous pickup radius

        // Pickup component
        var pickup = root.AddComponent<CollectiblePickup>();
        pickup.collectibleType = type;

        // Gentle bob rotation — keeps the editor lively (optional)
        root.AddComponent<CollectibleBob>();

        SavePrefab(root, collectibleName);
        Object.DestroyImmediate(root);
    }

    // ── Background props ──────────────────────────────────────────────────

    private static void BuildBackgroundProps()
    {
        BuildPyramid();
        BuildObelisk();
        BuildSkyPlane();
    }

    private static void BuildPyramid()
    {
        var root = new GameObject("Prop_Pyramid");

        // 3 stacked scaled cubes — each layer narrower and higher
        CreatePyramidLayer(root.transform, 0, new Vector3(4f, 1f, 4f), new Vector3(0f, 0.5f,  0f));
        CreatePyramidLayer(root.transform, 1, new Vector3(2.5f, 1f, 2.5f), new Vector3(0f, 1.5f, 0f));
        CreatePyramidLayer(root.transform, 2, new Vector3(1f, 1f, 1f), new Vector3(0f, 2.5f, 0f));

        SavePrefab(root, "Prop_Pyramid");
        Object.DestroyImmediate(root);
    }

    private static void CreatePyramidLayer(Transform parent, int index, Vector3 scale, Vector3 pos)
    {
        var layer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        layer.name = "Layer" + index;
        SetMat(layer, MakeMat("PyramidStone", new Color(0.87f, 0.78f, 0.55f)));
        DestroyCollider(layer);
        layer.transform.SetParent(parent);
        layer.transform.localPosition = pos;
        layer.transform.localScale    = scale;
    }

    private static void BuildObelisk()
    {
        var root = new GameObject("Prop_Obelisk");

        var shaft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shaft.name = "Shaft";
        SetMat(shaft, MakeMat("ObeliskStone", new Color(0.80f, 0.72f, 0.52f)));
        DestroyCollider(shaft);
        shaft.transform.SetParent(root.transform);
        shaft.transform.localPosition = new Vector3(0f, 3f, 0f);
        shaft.transform.localScale    = new Vector3(0.4f, 6f, 0.4f);

        // Pyramid cap
        var cap = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cap.name = "Cap";
        SetMat(cap, GetMat("ObeliskStone"));
        DestroyCollider(cap);
        cap.transform.SetParent(root.transform);
        cap.transform.localPosition = new Vector3(0f, 6.3f, 0f);
        cap.transform.localScale    = new Vector3(0.5f, 0.5f, 0.5f);

        SavePrefab(root, "Prop_Obelisk");
        Object.DestroyImmediate(root);
    }

    private static void BuildSkyPlane()
    {
        var root = new GameObject("Prop_SkyPlane");

        var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "Sky";
        SetMat(plane, MakeMat("SkyBlue", new Color(0.40f, 0.70f, 1.00f)));
        DestroyCollider(plane);
        plane.transform.SetParent(root.transform);
        plane.transform.localPosition = Vector3.zero;
        // Large backdrop — scaled and rotated to act as sky quad
        plane.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
        plane.transform.localScale       = new Vector3(20f, 1f, 20f);

        SavePrefab(root, "Prop_SkyPlane");
        Object.DestroyImmediate(root);
    }

    // ── Animator Controller stub ──────────────────────────────────────────

    /// <summary>
    /// Creates a minimal AnimatorController with the 4 states PlayerController expects.
    /// Saves to Assets/Prefabs/Placeholder/CamelPlaceholder.controller.
    /// The Camel_Placeholder prefab is then updated to reference this controller.
    /// </summary>
    private static void BuildPlaceholderAnimatorController()
    {
        string controllerPath = PrefabDir + "/CamelPlaceholder.controller";
        var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        // Parameters
        controller.AddParameter("IsRunning", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Jump",      AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Slide",     AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Hit",       AnimatorControllerParameterType.Trigger);

        // States: Idle, Running, Jump, Slide, Hit
        var rootSM = controller.layers[0].stateMachine;
        rootSM.AddState("Idle");
        rootSM.AddState("Running");
        rootSM.AddState("Jump");
        rootSM.AddState("Slide");
        rootSM.AddState("Hit");

        AssetDatabase.SaveAssets();

        // Wire into Camel prefab
        string camelPath = PrefabDir + "/Camel_Placeholder.prefab";
        var camelPrefab  = AssetDatabase.LoadAssetAtPath<GameObject>(camelPath);
        if (camelPrefab != null)
        {
            var anim = camelPrefab.GetComponent<Animator>();
            if (anim != null)
            {
                anim.runtimeAnimatorController = controller;
                EditorUtility.SetDirty(camelPrefab);
                PrefabUtility.SavePrefabAsset(camelPrefab);
            }
        }
    }

    // ── Material helpers ──────────────────────────────────────────────────

    /// <summary>Creates or retrieves a material from the material cache folder.</summary>
    private static Material MakeMat(string matName, Color color)
    {
        string path = MatDir + "/" + matName + ".mat";
        var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null)
        {
            existing.color = color;
            return existing;
        }

        // Try URP first, fall back to Standard
        var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        var mat    = new Material(shader) { name = matName, color = color };
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    /// <summary>Retrieves an already-created material by name.</summary>
    private static Material GetMat(string matName)
    {
        string path = MatDir + "/" + matName + ".mat";
        return AssetDatabase.LoadAssetAtPath<Material>(path);
    }

    private static void SetMat(GameObject obj, Material mat)
    {
        if (mat == null) return;
        var rend = obj.GetComponent<Renderer>();
        if (rend != null) rend.sharedMaterial = mat;
    }

    // ── Collider helper ───────────────────────────────────────────────────

    private static void DestroyCollider(GameObject obj)
    {
        var col = obj.GetComponent<Collider>();
        if (col != null) Object.DestroyImmediate(col);
    }

    // ── Prefab save ───────────────────────────────────────────────────────

    private static void SavePrefab(GameObject go, string prefabName)
    {
        string path = PrefabDir + "/" + prefabName + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(go, path);
    }
}
