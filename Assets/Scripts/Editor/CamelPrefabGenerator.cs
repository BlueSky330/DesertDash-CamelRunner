using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// Editor utility to create Assets/Prefabs/Characters/Camel_Default.prefab
/// using ProceduralCamelMesh (no FBX required).
///
/// Run: Tools → Camel Runner → Generate Camel Prefab
///
/// Prefab components:
///   ProceduralCamelMesh  — generates low-poly mesh at runtime
///   MeshFilter           — set by ProceduralCamelMesh.Build()
///   MeshRenderer         — set by ProceduralCamelMesh.Build()
///   CharacterController  — sized for ~1.5-unit-tall camel
///   Animator             — empty controller (clips assigned by Artists/Lead)
///
/// Tag:   "Player"
/// Layer: Default (0)
///
/// Accessory slots created inside Build():
///   [Slot] SaddleBlanket   — child transform above hump
///   [Slot] AviatorGoggles  — child transform at forehead
/// </summary>
public static class CamelPrefabGenerator
{
    private const string PrefabPath      = "Assets/Prefabs/Characters/Camel_Default.prefab";
    private const string PrefabFolder    = "Assets/Prefabs/Characters";
    private const string ControllerPath  = "Assets/Prefabs/Characters/Camel_AnimatorController.controller";

    [MenuItem("Tools/Camel Runner/Generate Camel Prefab")]
    public static void GenerateCamelPrefab()
    {
        Debug.Log("[CamelPrefabGenerator] Building Camel_Default prefab...");

        // Ensure destination folder exists
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder(PrefabFolder))
            AssetDatabase.CreateFolder("Assets/Prefabs", "Characters");

        // ── Build root GameObject ─────────────────────────────────────────────
        var root = new GameObject("Camel_Default");
        root.tag = "Player";
        // Layer "Default" is 0 — explicit assignment for clarity
        root.layer = 0;

        // ── ProceduralCamelMesh — generates geometry ──────────────────────────
        var camelMesh = root.AddComponent<ProceduralCamelMesh>();
        camelMesh.Build();

        // ── CharacterController — sized for the procedural camel ──────────────
        // Feet at Y=0, crown ~1.5 units, so height=1.5, centre at Y=0.75
        var cc        = root.AddComponent<CharacterController>();
        cc.height     = 1.5f;
        cc.radius     = 0.3f;
        cc.center     = new Vector3(0f, 0.75f, 0f);

        // ── Animator — empty controller, ready for animation clips ────────────
        var animator   = root.AddComponent<Animator>();
        var controller = LoadOrCreateAnimatorController();
        animator.runtimeAnimatorController = controller;

        // ── Save prefab ───────────────────────────────────────────────────────
        PrefabUtility.SaveAsPrefabAsset(root, PrefabPath, out bool success);

        if (success)
            Debug.Log($"[CamelPrefabGenerator] ✓ Prefab saved: {PrefabPath}");
        else
            Debug.LogError($"[CamelPrefabGenerator] ✗ Failed to save prefab at {PrefabPath}");

        Object.DestroyImmediate(root);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Camel Runner/Validate Camel Prefab")]
    public static void ValidateCamelPrefab()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"[CamelPrefabGenerator] ✗ Not found: {PrefabPath}\n" +
                           "  Run Tools → Camel Runner → Generate Camel Prefab first.");
            return;
        }

        bool hasProceduralMesh = prefab.GetComponent<ProceduralCamelMesh>() != null;
        bool hasMeshFilter     = prefab.GetComponent<MeshFilter>() != null;
        bool hasMeshRenderer   = prefab.GetComponent<MeshRenderer>() != null;
        bool hasCC             = prefab.GetComponent<CharacterController>() != null;
        bool hasAnimator       = prefab.GetComponent<Animator>() != null;
        bool tagIsPlayer       = prefab.CompareTag("Player");

        // Check accessory slots exist as children
        bool hasSaddleSlot  = prefab.transform.Find("[Slot] SaddleBlanket")  != null;
        bool hasGoggleSlot  = prefab.transform.Find("[Slot] AviatorGoggles") != null;

        string report =
            $"[CamelPrefabGenerator] Validation — {PrefabPath}\n" +
            $"  ProceduralCamelMesh:  {Mark(hasProceduralMesh)}\n" +
            $"  MeshFilter:           {Mark(hasMeshFilter)}\n" +
            $"  MeshRenderer:         {Mark(hasMeshRenderer)}\n" +
            $"  CharacterController:  {Mark(hasCC)}\n" +
            $"  Animator:             {Mark(hasAnimator)}\n" +
            $"  Tag = \"Player\":       {Mark(tagIsPlayer)}\n" +
            $"  [Slot] SaddleBlanket: {Mark(hasSaddleSlot)}\n" +
            $"  [Slot] AviatorGoggles:{Mark(hasGoggleSlot)}";

        bool allOk = hasProceduralMesh && hasMeshFilter && hasMeshRenderer
                  && hasCC && hasAnimator && tagIsPlayer
                  && hasSaddleSlot && hasGoggleSlot;

        if (allOk) Debug.Log(report);
        else       Debug.LogError(report);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────────

    private static AnimatorController LoadOrCreateAnimatorController()
    {
        var existing = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        if (existing != null) return existing;

        var ctrl = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
        // Standard camel animator parameters (mirroring PlayerController hashes)
        ctrl.AddParameter("IsRunning", AnimatorControllerParameterType.Bool);
        ctrl.AddParameter("Jump",      AnimatorControllerParameterType.Trigger);
        ctrl.AddParameter("Slide",     AnimatorControllerParameterType.Trigger);
        ctrl.AddParameter("Hit",       AnimatorControllerParameterType.Trigger);
        return ctrl;
    }

    private static string Mark(bool ok) => ok ? "✓" : "✗ MISSING";
}
