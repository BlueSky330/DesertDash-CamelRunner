using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// Editor utility to create Assets/Models/Egypt/Characters/ShadowThief/Prefabs/ShadowThief.prefab
/// using ProceduralShadowThiefMesh (no FBX required). Implements AIG-198.
///
/// Run: Tools → Camel Runner → Generate ShadowThief Prefab
///
/// Prefab components:
///   ProceduralShadowThiefMesh — generates skinned mesh + 10-bone skeleton at runtime
///   SkinnedMeshRenderer       — set by ProceduralShadowThiefMesh.Build()
///   CapsuleCollider           — trigger, sized to character (~1.83 units tall from smoke tip)
///   Rigidbody                 — kinematic, freeze-rotation
///   Animator                  — empty controller ready for animation clips
///
/// Tag:   "Enemy"
/// Layer: Default (0)
///
/// Material slots (ordered by SubMesh index):
///   [0] Body  — very dark navy-gray #1A1A2E (opaque)
///   [1] Eyes  — glowing cyan #00FFFF (emissive)
///   [2] Cape  — dark navy #16213E at 70% alpha (transparent)
///
/// Child slots created by ProceduralShadowThiefMesh.Build():
///   glowSlot — near eyes, for attaching particle/glow effects
///   capeSlot — at cape origin, for runtime cape swaps
/// </summary>
public static class ShadowThiefPrefabGenerator
{
    private const string PrefabFolder     = "Assets/Models/Egypt/Characters/ShadowThief/Prefabs";
    private const string PrefabPath       = PrefabFolder + "/ShadowThief.prefab";
    private const string ControllerFolder = "Assets/Models/Egypt/Characters/ShadowThief";
    private const string ControllerPath   = ControllerFolder + "/ShadowThief_AnimatorController.controller";

    private const string MatFolder  = "Assets/Models/Egypt/Characters/ShadowThief/Materials";
    private const string MatBody    = MatFolder + "/ShadowThief_Body.mat";
    private const string MatEyes    = MatFolder + "/ShadowThief_Eyes.mat";
    private const string MatCape    = MatFolder + "/ShadowThief_Cape.mat";

    [MenuItem("Tools/Camel Runner/Generate ShadowThief Prefab")]
    public static void GenerateShadowThiefPrefab()
    {
        Debug.Log("[ShadowThiefPrefabGenerator] Building ShadowThief prefab...");

        EnsureFolders();

        var root   = new GameObject("ShadowThief");
        root.tag   = "Enemy";
        root.layer = 0; // Default

        // ── ProceduralShadowThiefMesh — generates skinned geometry + skeleton ──
        var shadowThief = root.AddComponent<ProceduralShadowThiefMesh>();

        // Inject saved material assets so the prefab references persistent assets,
        // not transient in-memory materials.
        shadowThief.matBody = LoadOrCreateMat(MatBody, ProceduralShadowThiefMesh.ColorBody,
                                              createFn: () => ProceduralShadowThiefMesh.CreateMat(
                                                  ProceduralShadowThiefMesh.ColorBody, 0f, 0f));
        shadowThief.matEyes = LoadOrCreateMat(MatEyes, ProceduralShadowThiefMesh.ColorEyes,
                                              createFn: () => ProceduralShadowThiefMesh.CreateMatEmissive(
                                                  ProceduralShadowThiefMesh.ColorEyes));
        shadowThief.matCape = LoadOrCreateMat(MatCape, ProceduralShadowThiefMesh.ColorCape,
                                              createFn: () => ProceduralShadowThiefMesh.CreateMatTransparent(
                                                  ProceduralShadowThiefMesh.ColorCape));

        shadowThief.Build();

        // ── CapsuleCollider — trigger for enemy detection ─────────────────────
        // Character extends from smoke tip (Y=0) to head top (~Y=1.83).
        // Centre at Y≈0.915; height ~1.83 units.
        var col      = root.AddComponent<CapsuleCollider>();
        col.radius   = 0.22f;
        col.height   = 1.83f;
        col.center   = new Vector3(0f, 0.915f, 0f);
        col.isTrigger = true;

        // ── Rigidbody — kinematic, matches ThiefSpawner expectations ─────────
        var rb             = root.AddComponent<Rigidbody>();
        rb.isKinematic     = true;
        rb.useGravity      = false;
        rb.constraints     = RigidbodyConstraints.FreezeRotation;

        // ── Animator — empty controller, ready for animation clips ────────────
        var animator = root.AddComponent<Animator>();
        animator.runtimeAnimatorController = LoadOrCreateAnimatorController();

        // ── Save prefab ───────────────────────────────────────────────────────
        PrefabUtility.SaveAsPrefabAsset(root, PrefabPath, out bool success);

        if (success)
            Debug.Log($"[ShadowThiefPrefabGenerator] ✓ Prefab saved: {PrefabPath}");
        else
            Debug.LogError($"[ShadowThiefPrefabGenerator] ✗ Failed to save prefab at {PrefabPath}");

        Object.DestroyImmediate(root);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Camel Runner/Validate ShadowThief Prefab")]
    public static void ValidateShadowThiefPrefab()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"[ShadowThiefPrefabGenerator] ✗ Not found: {PrefabPath}\n" +
                           "  Run Tools → Camel Runner → Generate ShadowThief Prefab first.");
            return;
        }

        bool hasMesh     = prefab.GetComponent<ProceduralShadowThiefMesh>() != null;
        bool hasSmr      = prefab.GetComponent<SkinnedMeshRenderer>() != null;
        bool hasCollider = prefab.GetComponent<CapsuleCollider>() != null;
        bool hasRb       = prefab.GetComponent<Rigidbody>() != null;
        bool hasAnimator = prefab.GetComponent<Animator>() != null;
        bool tagEnemy    = prefab.CompareTag("Enemy");

        // Check named child slots
        bool hasGlowSlot = prefab.transform.Find("glowSlot") != null;
        bool hasCapeSlot = prefab.transform.Find("capeSlot") != null;

        // Check bone count
        int boneCount = 0;
        foreach (Transform child in prefab.GetComponentsInChildren<Transform>())
            if (child.name.StartsWith("[Bone]")) boneCount++;

        // Check material count and transparency on cape material
        var smr = prefab.GetComponent<SkinnedMeshRenderer>();
        int matCount = smr?.sharedMaterials?.Length ?? 0;
        bool mats3   = matCount == 3;

        // Check triangle count
        int triCount = smr?.sharedMesh != null ? smr.sharedMesh.triangles.Length / 3 : -1;
        bool triOk   = triCount >= 0 && triCount < 1200;

        // Verify cape material is transparent (has _Surface = 1 in URP or renderQueue >= 3000)
        bool capeTransparent = false;
        if (smr != null && smr.sharedMaterials.Length > 2 && smr.sharedMaterials[2] != null)
        {
            var capeMat = smr.sharedMaterials[2];
            capeTransparent = capeMat.renderQueue >= 3000 ||
                              (capeMat.HasProperty("_Surface") && capeMat.GetFloat("_Surface") > 0.5f);
        }

        string report =
            $"[ShadowThiefPrefabGenerator] Validation — {PrefabPath}\n" +
            $"  ProceduralShadowThiefMesh: {Mark(hasMesh)}\n" +
            $"  SkinnedMeshRenderer:       {Mark(hasSmr)}\n" +
            $"  CapsuleCollider:           {Mark(hasCollider)}\n" +
            $"  Rigidbody:                 {Mark(hasRb)}\n" +
            $"  Animator:                  {Mark(hasAnimator)}\n" +
            $"  Tag = \"Enemy\":             {Mark(tagEnemy)}\n" +
            $"  glowSlot child:            {Mark(hasGlowSlot)}\n" +
            $"  capeSlot child:            {Mark(hasCapeSlot)}\n" +
            $"  Bones found:               {boneCount} (expect 10)\n" +
            $"  Material count:            {matCount} {(mats3 ? "(3 ✓)" : "(expect 3!)")}\n" +
            $"  Cape transparent:          {Mark(capeTransparent)}\n" +
            $"  Triangle count:            {(triCount < 0 ? "n/a" : triCount.ToString())} {(triOk ? "(< 1 200 ✓)" : "(OVER BUDGET!)")}";

        bool allOk = hasMesh && hasSmr && hasCollider && hasRb && hasAnimator &&
                     tagEnemy && hasGlowSlot && hasCapeSlot && mats3 && triOk && capeTransparent;

        if (allOk) Debug.Log(report);
        else       Debug.LogError(report);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────────

    private static void EnsureFolders()
    {
        EnsureFolder("Assets",                                   "Models");
        EnsureFolder("Assets/Models",                            "Egypt");
        EnsureFolder("Assets/Models/Egypt",                      "Characters");
        EnsureFolder("Assets/Models/Egypt/Characters",           "ShadowThief");
        EnsureFolder(ControllerFolder,                           "Prefabs");
        EnsureFolder(ControllerFolder,                           "Materials");
    }

    private static void EnsureFolder(string parent, string child)
    {
        string full = $"{parent}/{child}";
        if (!AssetDatabase.IsValidFolder(full))
            AssetDatabase.CreateFolder(parent, child);
    }

    private static RuntimeAnimatorController LoadOrCreateAnimatorController()
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
    /// Load an existing material asset or create and save a new one via the provided factory.
    /// Saving the material as an asset means the prefab holds a persistent reference
    /// (survives domain reload / editor restart).
    /// </summary>
    private static Material LoadOrCreateMat(string assetPath, Color _, System.Func<Material> createFn)
    {
        var existing = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
        if (existing != null) return existing;

        var mat = createFn();
        AssetDatabase.CreateAsset(mat, assetPath);
        return mat;
    }

    private static string Mark(bool ok) => ok ? "✓" : "✗ MISSING";
}
