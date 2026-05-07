using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

/// <summary>
/// Unity Editor utility to auto-generate placeholder thief prefabs.
/// Runs from Unity Editor menu: Tools → Camel Runner → Generate Thief Placeholder Prefabs
///
/// Creates 4 thief prefabs (DesertBandit, NinjaThief, Pirate, ShadowThief) with:
/// - Placeholder capsule geometry (stand-in for FBX models)
/// - CapsuleCollider (trigger, for enemy detection)
/// - Rigidbody (kinematic)
/// - Animator with empty controller (ready for animation clips)
/// - Proper naming & folder structure per CAMEL_ASSET_NAMING_CONVENTIONS.md
///
/// Usage:
/// 1. Ensure Assets/Models/Egypt/Characters/Thieves/{Type}/ folders exist
/// 2. Tools → Camel Runner → Generate Thief Placeholder Prefabs
/// 3. Prefabs are created in each thief folder
/// 4. Inspector: Assign animator controller clips when real animations are ready
/// </summary>
public class ThiefPrefabGenerator : MonoBehaviour
{
    private static readonly List<string> ThiefTypes = new()
    {
        "DesertBandit",
        "NinjaThief",
        "Pirate",
        "ShadowThief"
    };

    [MenuItem("Tools/Camel Runner/Generate Thief Placeholder Prefabs")]
    public static void GenerateAllThiefPrefabs()
    {
        Debug.Log("[ThiefPrefabGenerator] Starting placeholder prefab generation...");

        foreach (string thiefType in ThiefTypes)
        {
            GenerateThiefPrefab(thiefType);
        }

        Debug.Log("[ThiefPrefabGenerator] ✓ All 4 thief placeholder prefabs generated!");
    }

    private static void GenerateThiefPrefab(string thiefType)
    {
        // Paths
        string prefabFolder = $"Assets/Models/Egypt/Characters/Thieves/{thiefType}";
        string controllerPath = $"{prefabFolder}/Thief_{thiefType}_AnimatorController.controller";
        string prefabPath = $"{prefabFolder}/Thief_{thiefType}.prefab";

        Debug.Log($"[ThiefPrefabGenerator] Creating {thiefType} prefab...");

        // Create root GameObject
        GameObject thiefGO = new GameObject($"Thief_{thiefType}");

        // Add Animator
        Animator animator = thiefGO.AddComponent<Animator>();

        // Load or create animator controller
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        if (controller == null)
        {
            Debug.LogWarning($"[ThiefPrefabGenerator] Animator controller not found at {controllerPath}. Creating empty controller...");
            controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

            // Add parameters
            controller.AddParameter("IsRunning", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Jump", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Slide", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Hit", AnimatorControllerParameterType.Trigger);
        }
        animator.runtimeAnimatorController = controller;

        // Add CapsuleCollider (trigger)
        CapsuleCollider collider = thiefGO.AddComponent<CapsuleCollider>();
        collider.radius = 0.4f;
        collider.height = 1.8f;
        collider.center = new Vector3(0, 0.9f, 0);
        collider.isTrigger = true;

        // Add Rigidbody (kinematic)
        Rigidbody rb = thiefGO.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;

        // Create placeholder visual (capsule)
        GameObject modelGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        modelGO.name = $"[Model]_{thiefType}";
        modelGO.transform.SetParent(thiefGO.transform);
        modelGO.transform.localPosition = Vector3.zero;
        modelGO.transform.localRotation = Quaternion.identity;
        modelGO.transform.localScale = Vector3.one;

        // Remove the collider from the model (we have one on parent)
        DestroyImmediate(modelGO.GetComponent<Collider>());

        // Optionally add a material to differentiate thieves by color
        AssignPlaceholderMaterial(modelGO, thiefType);

        // Set tag
        thiefGO.tag = "Enemy";

        // Create prefab
        string prefabFolder_Full = Application.dataPath + prefabFolder.Substring("Assets".Length);
        if (!System.IO.Directory.Exists(prefabFolder_Full))
        {
            System.IO.Directory.CreateDirectory(prefabFolder_Full);
        }

        PrefabUtility.SaveAsPrefabAsset(thiefGO, prefabPath);
        Debug.Log($"✓ Created: {prefabPath}");

        // Clean up scene
        DestroyImmediate(thiefGO);
    }

    private static void AssignPlaceholderMaterial(GameObject modelGO, string thiefType)
    {
        // Create a placeholder material with a unique color per thief type
        Material mat = new Material(Shader.Find("Standard"));

        // Different colors per thief to make them visually distinct
        mat.color = thiefType switch
        {
            "DesertBandit" => new Color(0.9f, 0.7f, 0.4f, 1f),  // Tan
            "NinjaThief" => new Color(0.2f, 0.2f, 0.2f, 1f),     // Dark gray
            "Pirate" => new Color(0.4f, 0.2f, 0.1f, 1f),         // Brown
            "ShadowThief" => new Color(0.1f, 0.1f, 0.2f, 1f),    // Dark blue
            _ => Color.white
        };

        Renderer renderer = modelGO.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = mat;
        }
    }

    [MenuItem("Tools/Camel Runner/Validate Thief Prefabs")]
    public static void ValidateThiefPrefabs()
    {
        Debug.Log("[ThiefPrefabGenerator] Validating thief prefabs...");

        int validCount = 0;
        foreach (string thiefType in ThiefTypes)
        {
            string prefabPath = $"Assets/Models/Egypt/Characters/Thieves/{thiefType}/Thief_{thiefType}.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogError($"✗ Missing: {prefabPath}");
                continue;
            }

            // Check components
            bool hasAnimator = prefab.GetComponent<Animator>() != null;
            bool hasCollider = prefab.GetComponent<CapsuleCollider>() != null;
            bool hasRigidbody = prefab.GetComponent<Rigidbody>() != null;

            if (hasAnimator && hasCollider && hasRigidbody)
            {
                Debug.Log($"✓ Valid: {thiefType}");
                validCount++;
            }
            else
            {
                Debug.LogError($"✗ Invalid {thiefType}: Animator={hasAnimator}, Collider={hasCollider}, Rigidbody={hasRigidbody}");
            }
        }

        Debug.Log($"[ThiefPrefabGenerator] Validation complete: {validCount}/4 prefabs valid");
    }
}
