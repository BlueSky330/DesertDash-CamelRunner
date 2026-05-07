using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// Editor utility to create Assets/Models/Egypt/Characters/Pirate/Prefabs/Pirate.prefab
/// using ProceduralPirateMesh (no FBX required). Implements AIG-199.
///
/// Run: Tools → Camel Runner → Generate Pirate Prefab
///
/// Prefab components:
///   ProceduralPirateMesh — generates skinned mesh + 10-bone skeleton at runtime
///   SkinnedMeshRenderer  — set by ProceduralPirateMesh.Build()
///   CapsuleCollider      — trigger, sized to character (~1.65 unit tall)
///   Rigidbody            — kinematic, freeze-rotation (matches other thieves)
///   Animator             — empty controller ready for animation clips
///
/// Child GameObjects (created by ProceduralPirateMesh.Build()):
///   weaponSlot — attach point at right hand for sword/pistol prop
///   hatSlot    — attach point at hat crown top for plume/particles
///
/// Tag:   "Enemy"
/// Layer: Default (0)
///
/// Material slots (ordered by SubMesh index):
///   [0] Navy  — #1A237E dark navy coat and hat
///   [1] Tan   — #C8A474 warm tan shirt and skin
///   [2] Black — #111111 near-black boots and eye patch
///   [3] Red   — #CC2200 crimson red sash
/// </summary>
public static class PiratePrefabGenerator
{
    private const string PrefabFolder     = "Assets/Models/Egypt/Characters/Pirate/Prefabs";
    private const string PrefabPath       = PrefabFolder + "/Pirate.prefab";
    private const string ControllerFolder = "Assets/Models/Egypt/Characters/Pirate";
    private const string ControllerPath   = ControllerFolder + "/Pirate_AnimatorController.controller";

    // Material asset paths — saved alongside the prefab so they are reusable across instances.
    private const string MatFolder = "Assets/Models/Egypt/Characters/Pirate/Materials";
    private const string MatNavy   = MatFolder + "/Pirate_Navy.mat";
    private const string MatTan    = MatFolder + "/Pirate_Tan.mat";
    private const string MatBlack  = MatFolder + "/Pirate_Black.mat";
    private const string MatRed    = MatFolder + "/Pirate_Red.mat";

    [MenuItem("Tools/Camel Runner/Generate Pirate Prefab")]
    public static void GeneratePiratePrefab()
    {
        Debug.Log("[PiratePrefabGenerator] Building Pirate prefab...");

        EnsureFolders();

        // ── Build root GameObject ─────────────────────────────────────────────
        var root   = new GameObject("Pirate");
        root.tag   = "Enemy";
        root.layer = 0; // Default

        // ── ProceduralPirateMesh — generates skinned geometry + skeleton ──────
        var pirate = root.AddComponent<ProceduralPirateMesh>();

        // Inject saved material assets so the prefab references persistent assets,
        // not transient in-memory materials.
        pirate.matNavy  = LoadOrCreateMat(MatNavy,  ProceduralPirateMesh.ColorNavy,  0f,   0f);
        pirate.matTan   = LoadOrCreateMat(MatTan,   ProceduralPirateMesh.ColorTan,   0f,   0f);
        pirate.matBlack = LoadOrCreateMat(MatBlack, ProceduralPirateMesh.ColorBlack, 0.1f, 0.1f);
        pirate.matRed   = LoadOrCreateMat(MatRed,   ProceduralPirateMesh.ColorRed,   0f,   0f);

        pirate.Build();

        // ── CapsuleCollider — trigger for enemy detection ─────────────────────
        // Character is ~1.65 units tall (feet at Y=0, hat peak at Y≈1.83).
        // Collider covers body only (not full hat), centred at Y=0.825.
        var col       = root.AddComponent<CapsuleCollider>();
        col.radius    = 0.35f;
        col.height    = 1.65f;
        col.center    = new Vector3(0f, 0.825f, 0f);
        col.isTrigger = true;

        // ── Rigidbody — kinematic, matches ThiefSpawner expectations ─────────
        var rb         = root.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity  = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // ── Animator — empty controller, ready for animation clips ────────────
        var animator = root.AddComponent<Animator>();
        animator.runtimeAnimatorController = LoadOrCreateAnimatorController();

        // ── Save prefab ───────────────────────────────────────────────────────
        PrefabUtility.SaveAsPrefabAsset(root, PrefabPath, out bool success);

        if (success)
            Debug.Log($"[PiratePrefabGenerator] ✓ Prefab saved: {PrefabPath}");
        else
            Debug.LogError($"[PiratePrefabGenerator] ✗ Failed to save prefab at {PrefabPath}");

        Object.DestroyImmediate(root);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Camel Runner/Validate Pirate Prefab")]
    public static void ValidatePiratePrefab()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"[PiratePrefabGenerator] ✗ Not found: {PrefabPath}\n" +
                           "  Run Tools → Camel Runner → Generate Pirate Prefab first.");
            return;
        }

        bool hasMesh        = prefab.GetComponent<ProceduralPirateMesh>() != null;
        bool hasSmr         = prefab.GetComponent<SkinnedMeshRenderer>() != null;
        bool hasCollider    = prefab.GetComponent<CapsuleCollider>() != null;
        bool hasRb          = prefab.GetComponent<Rigidbody>() != null;
        bool hasAnimator    = prefab.GetComponent<Animator>() != null;
        bool tagEnemy       = prefab.CompareTag("Enemy");

        // Check bone children.
        int boneCount = 0;
        foreach (Transform child in prefab.GetComponentsInChildren<Transform>())
            if (child.name.StartsWith("[Bone]")) boneCount++;

        // Check child slots (parented to bones so use deep search).
        bool hasWeaponSlot = prefab.transform.Find("weaponSlot") != null
                          || FindDeep(prefab.transform, "weaponSlot");
        bool hasHatSlot    = prefab.transform.Find("hatSlot") != null
                          || FindDeep(prefab.transform, "hatSlot");

        // Check triangle count via SkinnedMeshRenderer.
        var smr      = prefab.GetComponent<SkinnedMeshRenderer>();
        int triCount = smr?.sharedMesh != null ? smr.sharedMesh.triangles.Length / 3 : -1;
        bool triOk   = triCount >= 0 && triCount < 1500;

        string report =
            $"[PiratePrefabGenerator] Validation — {PrefabPath}\n" +
            $"  ProceduralPirateMesh: {Mark(hasMesh)}\n" +
            $"  SkinnedMeshRenderer:  {Mark(hasSmr)}\n" +
            $"  CapsuleCollider:      {Mark(hasCollider)}\n" +
            $"  Rigidbody:            {Mark(hasRb)}\n" +
            $"  Animator:             {Mark(hasAnimator)}\n" +
            $"  Tag = \"Enemy\":        {Mark(tagEnemy)}\n" +
            $"  Bones found:          {boneCount} (expect 10)\n" +
            $"  weaponSlot:           {Mark(hasWeaponSlot)}\n" +
            $"  hatSlot:              {Mark(hasHatSlot)}\n" +
            $"  Triangle count:       {(triCount < 0 ? "n/a" : triCount.ToString())} " +
            $"{(triOk ? "(< 1 500 \u2713)" : "(OVER BUDGET!)")}";

        bool allOk = hasMesh && hasSmr && hasCollider && hasRb && hasAnimator &&
                     tagEnemy && hasWeaponSlot && hasHatSlot && triOk;

        if (allOk) Debug.Log(report);
        else       Debug.LogError(report);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────────

    private static void EnsureFolders()
    {
        EnsureFolder("Assets",                                  "Models");
        EnsureFolder("Assets/Models",                           "Egypt");
        EnsureFolder("Assets/Models/Egypt",                     "Characters");
        EnsureFolder("Assets/Models/Egypt/Characters",          "Pirate");
        EnsureFolder("Assets/Models/Egypt/Characters/Pirate",   "Prefabs");
        EnsureFolder("Assets/Models/Egypt/Characters/Pirate",   "Materials");
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

        var mat = ProceduralPirateMesh.CreateMat(albedo, metallic, smoothness);
        AssetDatabase.CreateAsset(mat, assetPath);
        return mat;
    }

    /// <summary>Recursive deep search for a named transform in the hierarchy.</summary>
    private static bool FindDeep(Transform root, string name)
    {
        foreach (Transform child in root)
        {
            if (child.name == name) return true;
            if (FindDeep(child, name)) return true;
        }
        return false;
    }

    private static string Mark(bool ok) => ok ? "\u2713" : "\u2717 MISSING";
}
