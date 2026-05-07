using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// Editor utility to create Assets/Models/Egypt/Characters/NinjaThief/Prefabs/NinjaThief.prefab
/// using ProceduralNinjaThiefMesh (no FBX required). Implements AIG-187.
///
/// Run: Tools → Camel Runner → Generate NinjaThief Prefab
///
/// Prefab components:
///   ProceduralNinjaThiefMesh — generates skinned mesh + 10-bone skeleton at runtime
///   SkinnedMeshRenderer      — set by ProceduralNinjaThiefMesh.Build()
///   CapsuleCollider          — trigger, sized to character (~1.6 unit tall)
///   Rigidbody                — kinematic, freeze-rotation (matches other thieves)
///   Animator                 — empty controller ready for animation clips
///
/// Tag:   "Enemy"
/// Layer: Default (0)
///
/// Material slots (ThiefNinja_Master set, ordered by SubMesh index):
///   [0] Black   — body / outfit
///   [1] White   — eyes
///   [2] Silver  — belt + buckle
///   [3] Gold    — shuriken on chest
/// </summary>
public static class NinjaThiefPrefabGenerator
{
    private const string PrefabFolder     = "Assets/Models/Egypt/Characters/NinjaThief/Prefabs";
    private const string PrefabPath       = PrefabFolder + "/NinjaThief.prefab";
    private const string ControllerFolder = "Assets/Models/Egypt/Characters/NinjaThief";
    private const string ControllerPath   = ControllerFolder + "/NinjaThief_AnimatorController.controller";

    // Material asset paths — saved alongside the prefab so they are reusable across instances.
    private const string MatFolder  = "Assets/Models/Egypt/Characters/NinjaThief/Materials";
    private const string MatBlack   = MatFolder + "/ThiefNinja_Black.mat";
    private const string MatWhite   = MatFolder + "/ThiefNinja_White.mat";
    private const string MatSilver  = MatFolder + "/ThiefNinja_Silver.mat";
    private const string MatGold    = MatFolder + "/ThiefNinja_Gold.mat";

    [MenuItem("Tools/Camel Runner/Generate NinjaThief Prefab")]
    public static void GenerateNinjaThiefPrefab()
    {
        Debug.Log("[NinjaThiefPrefabGenerator] Building NinjaThief prefab...");

        EnsureFolders();

        // ── Build root GameObject ─────────────────────────────────────────────
        var root   = new GameObject("NinjaThief");
        root.tag   = "Enemy";
        root.layer = 0; // Default

        // ── ProceduralNinjaThiefMesh — generates skinned geometry + skeleton ──
        var ninjaThief = root.AddComponent<ProceduralNinjaThiefMesh>();

        // Inject saved material assets so the prefab references persistent assets,
        // not transient in-memory materials.
        ninjaThief.matBlack  = LoadOrCreateMat(MatBlack,  ProceduralNinjaThiefMesh.ColorBlack,  0f,   0f);
        ninjaThief.matWhite  = LoadOrCreateMat(MatWhite,  ProceduralNinjaThiefMesh.ColorWhite,  0f,   0f);
        ninjaThief.matSilver = LoadOrCreateMat(MatSilver, ProceduralNinjaThiefMesh.ColorSilver, 0.5f, 0.4f);
        ninjaThief.matGold   = LoadOrCreateMat(MatGold,   ProceduralNinjaThiefMesh.ColorGold,   0.6f, 0.5f);

        ninjaThief.Build();

        // ── CapsuleCollider — trigger for enemy detection ─────────────────────
        // Character is ~1.60 units tall; feet at Y=0 → centre at Y=0.80.
        var col    = root.AddComponent<CapsuleCollider>();
        col.radius  = 0.35f;
        col.height  = 1.60f;
        col.center  = new Vector3(0f, 0.80f, 0f);
        col.isTrigger = true;

        // ── Rigidbody — kinematic, matches ThiefSpawner expectations ─────────
        var rb              = root.AddComponent<Rigidbody>();
        rb.isKinematic      = true;
        rb.useGravity       = false;
        rb.constraints      = RigidbodyConstraints.FreezeRotation;

        // ── Animator — empty controller, ready for animation clips ────────────
        var animator          = root.AddComponent<Animator>();
        animator.runtimeAnimatorController = LoadOrCreateAnimatorController();

        // ── Save prefab ───────────────────────────────────────────────────────
        PrefabUtility.SaveAsPrefabAsset(root, PrefabPath, out bool success);

        if (success)
            Debug.Log($"[NinjaThiefPrefabGenerator] ✓ Prefab saved: {PrefabPath}");
        else
            Debug.LogError($"[NinjaThiefPrefabGenerator] ✗ Failed to save prefab at {PrefabPath}");

        Object.DestroyImmediate(root);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Camel Runner/Validate NinjaThief Prefab")]
    public static void ValidateNinjaThiefPrefab()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"[NinjaThiefPrefabGenerator] ✗ Not found: {PrefabPath}\n" +
                           "  Run Tools → Camel Runner → Generate NinjaThief Prefab first.");
            return;
        }

        bool hasMesh     = prefab.GetComponent<ProceduralNinjaThiefMesh>() != null;
        bool hasSmr      = prefab.GetComponent<SkinnedMeshRenderer>() != null;
        bool hasCollider = prefab.GetComponent<CapsuleCollider>() != null;
        bool hasRb       = prefab.GetComponent<Rigidbody>() != null;
        bool hasAnimator = prefab.GetComponent<Animator>() != null;
        bool tagEnemy    = prefab.CompareTag("Enemy");

        // Check bone children
        int boneCount = 0;
        foreach (Transform child in prefab.GetComponentsInChildren<Transform>())
            if (child.name.StartsWith("[Bone]")) boneCount++;

        // Check triangle count via SkinnedMeshRenderer
        var smr = prefab.GetComponent<SkinnedMeshRenderer>();
        int triCount = smr?.sharedMesh != null ? smr.sharedMesh.triangles.Length / 3 : -1;
        bool triOk   = triCount >= 0 && triCount < 1200;

        string report =
            $"[NinjaThiefPrefabGenerator] Validation — {PrefabPath}\n" +
            $"  ProceduralNinjaThiefMesh: {Mark(hasMesh)}\n" +
            $"  SkinnedMeshRenderer:      {Mark(hasSmr)}\n" +
            $"  CapsuleCollider:          {Mark(hasCollider)}\n" +
            $"  Rigidbody:                {Mark(hasRb)}\n" +
            $"  Animator:                 {Mark(hasAnimator)}\n" +
            $"  Tag = \"Enemy\":            {Mark(tagEnemy)}\n" +
            $"  Bones found:              {boneCount} (expect 10)\n" +
            $"  Triangle count:           {(triCount < 0 ? "n/a" : triCount.ToString())} {(triOk ? "(< 1 200 ✓)" : "(OVER BUDGET!)")}";

        bool allOk = hasMesh && hasSmr && hasCollider && hasRb && hasAnimator && tagEnemy && triOk;

        if (allOk) Debug.Log(report);
        else       Debug.LogError(report);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────────

    private static void EnsureFolders()
    {
        EnsureFolder("Assets", "Models");
        EnsureFolder("Assets/Models", "Egypt");
        EnsureFolder("Assets/Models/Egypt", "Characters");
        EnsureFolder("Assets/Models/Egypt/Characters", "NinjaThief");
        EnsureFolder(ControllerFolder, "Prefabs");
        EnsureFolder(ControllerFolder, "Materials");
    }

    private static void EnsureFolder(string parent, string child)
    {
        string full = $"{parent}/{child}";
        if (!AssetDatabase.IsValidFolder(full))
            AssetDatabase.CreateFolder(parent, child);
    }

    private static AnimatorController LoadOrCreateAnimatorController()
    {
        var existing = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        if (existing != null) return existing;

        var ctrl = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
        // Standard thief animator parameters (mirroring ThiefSpawner / other thief prefabs).
        ctrl.AddParameter("IsRunning", AnimatorControllerParameterType.Bool);
        ctrl.AddParameter("Jump",      AnimatorControllerParameterType.Trigger);
        ctrl.AddParameter("Slide",     AnimatorControllerParameterType.Trigger);
        ctrl.AddParameter("Hit",       AnimatorControllerParameterType.Trigger);
        return ctrl;
    }

    /// <summary>
    /// Load an existing material asset or create and save a new one.
    /// Saving the material as an asset means the prefab holds a persistent reference
    /// (survives domain reload / editor restart).
    /// </summary>
    private static Material LoadOrCreateMat(string assetPath, Color albedo, float metallic, float smoothness)
    {
        var existing = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
        if (existing != null) return existing;

        var mat = ProceduralNinjaThiefMesh.CreateMat(albedo, metallic, smoothness);
        AssetDatabase.CreateAsset(mat, assetPath);
        return mat;
    }

    private static string Mark(bool ok) => ok ? "✓" : "✗ MISSING";
}
