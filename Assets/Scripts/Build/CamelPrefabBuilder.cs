using UnityEngine;

/// <summary>
/// Runtime/Build-time utility to create Camel_Default prefab.
/// Can be called from a build script, initialization, or Editor menu.
///
/// Creates a GameObject with:
///   - ProceduralCamelMesh (generates geometry at Awake)
///   - CharacterController (for player movement)
///   - Animator (empty, ready for animation clips)
///   - Accessory slots (SaddleBlanket, AviatorGoggles)
///   - Tag: "Player"
///
/// This is a simpler alternative to CamelPrefabGenerator for environments
/// where the Editor is not available or full asset database access is needed.
/// </summary>
public static class CamelPrefabBuilder
{
    /// <summary>
    /// Build Camel_Default GameObject hierarchy (non-persistent).
    /// Returns the root GameObject without saving it as a prefab.
    /// Call from an Editor script with PrefabUtility.SaveAsPrefabAsset() to persist.
    /// </summary>
    public static GameObject BuildCamelGameObject()
    {
        // ── Root GameObject ───────────────────────────────────────────────────
        var root = new GameObject("Camel_Default");
        root.tag = "Player";
        root.layer = LayerMask.NameToLayer("Default");

        // ── ProceduralCamelMesh ──────────────────────────────────────────────
        // This generates the mesh and sets up MeshFilter + MeshRenderer
        var procedural = root.AddComponent<ProceduralCamelMesh>();
        // NOTE: Build() will be called automatically at Awake() at runtime.
        // If building in Editor or headless, call Build() explicitly below.
        procedural.Build();

        // ── CharacterController ──────────────────────────────────────────────
        // Sized to match procedural camel (foot at Y=0, crown ~1.5 units)
        var cc = root.AddComponent<CharacterController>();
        cc.height = 1.5f;
        cc.radius = 0.3f;
        cc.center = new Vector3(0f, 0.75f, 0f);

        // ── Animator ─────────────────────────────────────────────────────────
        // Empty controller; clips will be added by animators.
        var animator = root.AddComponent<Animator>();
        // NOTE: controller should be assigned by build/initialization code

        return root;
    }

    /// <summary>
    /// Get a summary of the built camel for validation.
    /// </summary>
    public static string ValidateCamelStructure(GameObject camel)
    {
        var checks = new System.Collections.Generic.List<string>();

        checks.Add($"GameObject: {(camel != null ? "✓" : "✗")}");
        checks.Add($"Tag 'Player': {(camel.CompareTag("Player") ? "✓" : "✗")}");

        var procedural = camel.GetComponent<ProceduralCamelMesh>();
        checks.Add($"ProceduralCamelMesh: {(procedural != null ? "✓" : "✗")}");

        var mf = camel.GetComponent<MeshFilter>();
        checks.Add($"MeshFilter: {(mf != null ? "✓" : "✗")}");
        if (mf != null)
        {
            int triCount = mf.sharedMesh != null ? mf.sharedMesh.triangles.Length / 3 : 0;
            checks.Add($"  Triangle count: {triCount} {(triCount < 1200 ? "✓" : "✗ OVER BUDGET")}");
        }

        var mr = camel.GetComponent<MeshRenderer>();
        checks.Add($"MeshRenderer: {(mr != null ? "✓" : "✗")}");

        var cc = camel.GetComponent<CharacterController>();
        checks.Add($"CharacterController: {(cc != null ? "✓" : "✗")}");

        var anim = camel.GetComponent<Animator>();
        checks.Add($"Animator: {(anim != null ? "✓" : "✗")}");

        Transform saddleSlot = camel.transform.Find("[Slot] SaddleBlanket");
        checks.Add($"[Slot] SaddleBlanket: {(saddleSlot != null ? "✓" : "✗")}");

        Transform goggleSlot = camel.transform.Find("[Slot] AviatorGoggles");
        checks.Add($"[Slot] AviatorGoggles: {(goggleSlot != null ? "✓" : "✗")}");

        return string.Join("\n", checks);
    }
}
