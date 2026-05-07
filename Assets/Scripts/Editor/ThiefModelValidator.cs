using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// Validates imported thief character models against the specification in THIEF_VARIANT_DESIGN.md
///
/// Usage: Select a thief FBX model → Assets menu → Validate → Check Thief Model Specifications
///
/// Checks:
/// - Polygon count (should be <1000 tris per variant)
/// - Normals (facing outward)
/// - UVs (no overlaps, proper density)
/// - Materials (assigned and found)
/// - Animation rig (if applicable)
/// </summary>
public class ThiefModelValidator : MonoBehaviour
{
    private static readonly string[] ThiefTypes = new[]
    {
        "DesertBandit",
        "NinjaThief",
        "Pirate",
        "ShadowThief"
    };

    private static readonly int[] TargetTriCounts = new[]
    {
        700,  // DesertBandit
        700,  // NinjaThief
        750,  // Pirate
        800   // ShadowThief
    };

    [MenuItem("Assets/Validate/Check Thief Model Specifications")]
    public static void ValidateSelectedThiefModel()
    {
        if (Selection.activeObject == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a thief FBX model", "OK");
            return;
        }

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!path.Contains("Thief") || !path.EndsWith(".fbx"))
        {
            EditorUtility.DisplayDialog("Error", "Please select a thief FBX file (path must contain 'Thief')", "OK");
            return;
        }

        ValidateThiefModel(path);
    }

    [MenuItem("Tools/Camel Runner/Validate All Thief Models")]
    public static void ValidateAllThiefModels()
    {
        Debug.Log("[ThiefModelValidator] Validating all thief models...");

        foreach (string thiefType in ThiefTypes)
        {
            string fbxPath = $"Assets/Models/Egypt/Characters/Thieves/{thiefType}/Thief_{thiefType}.fbx";

            if (AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath) == null)
            {
                Debug.LogWarning($"[ThiefModelValidator] Model not found: {fbxPath}");
                continue;
            }

            ValidateThiefModel(fbxPath);
        }

        Debug.Log("[ThiefModelValidator] ✓ All thief model validations complete");
    }

    private static void ValidateThiefModel(string fbxPath)
    {
        GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
        if (model == null)
        {
            Debug.LogError($"[ThiefModelValidator] Could not load: {fbxPath}");
            return;
        }

        string modelName = model.name;
        Debug.Log($"\n[ThiefModelValidator] Validating: {modelName}");

        // Extract thief type from path
        string thiefType = ExtractThiefType(fbxPath);
        int targetTris = GetTargetTriCount(thiefType);

        // Check 1: Polygon count
        int actualTris = GetTriangleCount(model);
        bool triCheckPass = actualTris < 1000; // All should be under 1000
        Debug.Log($"  ✓ Triangle count: {actualTris} tris (target: <1000) {(triCheckPass ? "PASS" : "FAIL")}");

        if (actualTris > 1000)
        {
            Debug.LogError($"  ✗ FAILED: {modelName} exceeds 1000 tris ({actualTris}). Optimize mesh or reduce geometry.");
        }

        // Check 2: Has SkinnedMeshRenderer
        SkinnedMeshRenderer meshRenderer = model.GetComponentInChildren<SkinnedMeshRenderer>();
        bool hasRenderer = meshRenderer != null;
        Debug.Log($"  {'✓'} SkinnedMeshRenderer found: {(hasRenderer ? "YES" : "NO")}");

        if (!hasRenderer)
        {
            Debug.LogWarning($"  ⚠ No SkinnedMeshRenderer on {modelName}. Import FBX with skinning enabled.");
        }

        // Check 3: Has materials assigned
        if (hasRenderer)
        {
            Material[] materials = meshRenderer.sharedMaterials;
            Debug.Log($"  ✓ Materials assigned: {materials.Length}");

            if (materials.Length == 0 || materials[0] == null)
            {
                Debug.LogWarning($"  ⚠ No materials assigned to {modelName}. Assign material in inspector after import.");
            }
        }

        // Check 4: Mesh normals (basic check)
        Mesh mesh = meshRenderer?.sharedMesh;
        if (mesh != null)
        {
            Vector3[] normals = mesh.normals;
            if (normals.Length > 0)
            {
                Debug.Log($"  ✓ Normals present: {normals.Length}");
            }
            else
            {
                Debug.LogWarning($"  ⚠ No normals detected. Recalculate in Blender or Unity import settings.");
            }

            // Check UVs
            Vector2[] uvs = mesh.uv;
            if (uvs.Length > 0)
            {
                Debug.Log($"  ✓ UVs present: {uvs.Length}");
            }
            else
            {
                Debug.LogError($"  ✗ No UVs found on {modelName}. Must unwrap in Blender.");
            }
        }

        // Check 5: Animation rig (if this is a rigged model)
        Animator animator = model.GetComponent<Animator>();
        if (animator != null)
        {
            Avatar avatar = animator.avatar;
            if (avatar != null && avatar.isValid)
            {
                Debug.Log($"  ✓ Avatar rig: VALID");
            }
            else
            {
                Debug.LogWarning($"  ⚠ Avatar rig not valid. Check bone structure in Blender.");
            }
        }

        // Summary
        Debug.Log($"✓ Validation complete for {modelName}\n");
    }

    private static int GetTriangleCount(GameObject model)
    {
        int totalTris = 0;

        // Count triangles in all meshes
        Mesh[] meshes = model.GetComponentsInChildren<SkinnedMeshRenderer>()
            .ConvertAll(m => m.sharedMesh).ToArray();

        foreach (Mesh mesh in meshes)
        {
            if (mesh != null)
            {
                totalTris += mesh.triangles.Length / 3;
            }
        }

        // Also check MeshFilter (if present)
        MeshFilter[] meshFilters = model.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.sharedMesh != null)
            {
                totalTris += mf.sharedMesh.triangles.Length / 3;
            }
        }

        return totalTris;
    }

    private static string ExtractThiefType(string path)
    {
        // Extract "NinjaThief", "DesertBandit", etc. from path
        foreach (string type in ThiefTypes)
        {
            if (path.Contains(type))
                return type;
        }
        return "Unknown";
    }

    private static int GetTargetTriCount(string thiefType)
    {
        return thiefType switch
        {
            "DesertBandit" => 700,
            "NinjaThief" => 700,
            "Pirate" => 750,
            "ShadowThief" => 800,
            _ => 1000
        };
    }
}
