using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// Editor utility to create Assets/Models/Egypt/Characters/DesertBandit/Prefabs/DesertBandit.prefab
/// using ProceduralDesertBanditMesh (no FBX required). Implements AIG-197.
///
/// Run: Tools → Camel Runner → Generate DesertBandit Prefab
///
/// Prefab components:
///   ProceduralDesertBanditMesh — generates skinned mesh + 9-bone skeleton at runtime
///   SkinnedMeshRenderer        — set by ProceduralDesertBanditMesh.Build()
///   CapsuleCollider            — trigger, sized to character (~1.35 unit tall)
///   Rigidbody                  — kinematic, freeze-rotation
///   Animator                   — empty controller ready for animation clips
///
/// Child GameObjects:
///   weaponSlot — attach point on back for curved dagger
///   bagSlot    — attach point on belt for coin bag
///
/// Tag:   "Enemy"
/// Layer: Default (0)
///
/// Material slots (ordered by SubMesh index):
///   [0] Robe  — sandy tan  #C8A474
///   [1] Wraps — dark brown #3D2010  (face wraps + scabbard)
///   [2] Sash  — cream      #E8D5A0
/// </summary>
public static class DesertBanditPrefabGenerator
{
    private const string PrefabFolder     = "Assets/Models/Egypt/Characters/DesertBandit/Prefabs";
    private const string PrefabPath       = PrefabFolder + "/DesertBandit.prefab";
    private const string ControllerFolder = "Assets/Models/Egypt/Characters/DesertBandit";
    private const string ControllerPath   = ControllerFolder + "/DesertBandit_AnimatorController.controller";

    private const string MatFolder = "Assets/Models/Egypt/Characters/DesertBandit/Materials";
    private const string MatRobe   = MatFolder + "/DesertBandit_Robe.mat";
    private const string MatWraps  = MatFolder + "/DesertBandit_Wraps.mat";
    private const string MatSash   = MatFolder + "/DesertBandit_Sash.mat";

    [MenuItem("Tools/Camel Runner/Generate DesertBandit Prefab")]
    public static void GenerateDesertBanditPrefab()
    {
        Debug.Log("[DesertBanditPrefabGenerator] Building DesertBandit prefab...");

        EnsureFolders();

        // ── Build root GameObject ─────────────────────────────────────────────
        var root   = new GameObject("DesertBandit");
        root.tag   = "Enemy";
        root.layer = 0; // Default

        // ── ProceduralDesertBanditMesh — generates skinned geometry + skeleton ──
        var bandit = root.AddComponent<ProceduralDesertBanditMesh>();

        // Inject persistent material assets so the prefab holds stable references.
        bandit.matRobe  = LoadOrCreateMat(MatRobe,  ProceduralDesertBanditMesh.ColorRobe,  0f, 0.10f);
        bandit.matWraps = LoadOrCreateMat(MatWraps, ProceduralDesertBanditMesh.ColorWraps, 0f, 0.05f);
        bandit.matSash  = LoadOrCreateMat(MatSash,  ProceduralDesertBanditMesh.ColorSash,  0f, 0.15f);

        bandit.Build();

        // ── CapsuleCollider — trigger for enemy detection ─────────────────────
        // Character is ~1.35 units tall; feet at Y=0 → centre at Y=0.675.
        var col       = root.AddComponent<CapsuleCollider>();
        col.radius    = 0.30f;
        col.height    = 1.35f;
        col.center    = new Vector3(0f, 0.675f, 0f);
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
            Debug.Log($"[DesertBanditPrefabGenerator] ✓ Prefab saved: {PrefabPath}");
        else
            Debug.LogError($"[DesertBanditPrefabGenerator] ✗ Failed to save prefab at {PrefabPath}");

        Object.DestroyImmediate(root);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Camel Runner/Validate DesertBandit Prefab")]
    public static void ValidateDesertBanditPrefab()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"[DesertBanditPrefabGenerator] ✗ Not found: {PrefabPath}\n" +
                           "  Run Tools → Camel Runner → Generate DesertBandit Prefab first.");
            return;
        }

        bool hasMesh       = prefab.GetComponent<ProceduralDesertBanditMesh>() != null;
        bool hasSmr        = prefab.GetComponent<SkinnedMeshRenderer>() != null;
        bool hasCollider   = prefab.GetComponent<CapsuleCollider>() != null;
        bool hasRb         = prefab.GetComponent<Rigidbody>() != null;
        bool hasAnimator   = prefab.GetComponent<Animator>() != null;
        bool tagEnemy      = prefab.CompareTag("Enemy");
        bool hasWeaponSlot = prefab.transform.Find("weaponSlot") != null;
        bool hasBagSlot    = prefab.transform.Find("bagSlot") != null;

        // Check triangle count via SkinnedMeshRenderer.
        var smr      = prefab.GetComponent<SkinnedMeshRenderer>();
        int triCount = smr?.sharedMesh != null ? smr.sharedMesh.triangles.Length / 3 : -1;
        bool triOk   = triCount >= 0 && triCount < 1500;

        string report =
            $"[DesertBanditPrefabGenerator] Validation — {PrefabPath}\n" +
            $"  ProceduralDesertBanditMesh: {Mark(hasMesh)}\n" +
            $"  SkinnedMeshRenderer:        {Mark(hasSmr)}\n" +
            $"  CapsuleCollider:            {Mark(hasCollider)}\n" +
            $"  Rigidbody:                  {Mark(hasRb)}\n" +
            $"  Animator:                   {Mark(hasAnimator)}\n" +
            $"  Tag = \"Enemy\":              {Mark(tagEnemy)}\n" +
            $"  weaponSlot child:           {Mark(hasWeaponSlot)}\n" +
            $"  bagSlot child:              {Mark(hasBagSlot)}\n" +
            $"  Triangle count:             {(triCount < 0 ? "n/a" : triCount.ToString())} " +
            $"{(triOk ? "(< 1 500 \u2713)" : "(OVER BUDGET!)")}";

        bool allOk = hasMesh && hasSmr && hasCollider && hasRb && hasAnimator &&
                     tagEnemy && hasWeaponSlot && hasBagSlot && triOk;

        if (allOk) Debug.Log(report);
        else       Debug.LogError(report);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────────

    private static void EnsureFolders()
    {
        EnsureFolder("Assets",                                      "Models");
        EnsureFolder("Assets/Models",                               "Egypt");
        EnsureFolder("Assets/Models/Egypt",                         "Characters");
        EnsureFolder("Assets/Models/Egypt/Characters",              "DesertBandit");
        EnsureFolder("Assets/Models/Egypt/Characters/DesertBandit", "Prefabs");
        EnsureFolder("Assets/Models/Egypt/Characters/DesertBandit", "Materials");
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
        ctrl.AddParameter("IsRunning", AnimatorControllerParameterType.Bool);
        ctrl.AddParameter("Jump",      AnimatorControllerParameterType.Trigger);
        ctrl.AddParameter("Slide",     AnimatorControllerParameterType.Trigger);
        ctrl.AddParameter("Hit",       AnimatorControllerParameterType.Trigger);
        return ctrl;
    }

    /// <summary>
    /// Load an existing material asset or create and save a new one.
    /// Saving the material as an asset gives the prefab a persistent reference.
    /// </summary>
    private static Material LoadOrCreateMat(string assetPath, Color albedo, float metallic, float smoothness)
    {
        var existing = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
        if (existing != null) return existing;

        var mat = ProceduralDesertBanditMesh.CreateMat(albedo, metallic, smoothness);
        AssetDatabase.CreateAsset(mat, assetPath);
        return mat;
    }

    private static string Mark(bool ok) => ok ? "\u2713" : "\u2717 MISSING";
}
