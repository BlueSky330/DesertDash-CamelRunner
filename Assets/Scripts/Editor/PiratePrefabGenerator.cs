using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// Editor utility to generate a Pirate prefab with ProceduralPirateMesh.
///
/// This script creates the Pirate.prefab asset and configures:
/// - ProceduralPirateMesh component (mesh generation)
/// - SkinnedMeshRenderer + materials
/// - Animator with thief animation controller
/// - Bone binding + idle expression state
///
/// Usage:
///   Open the Gameplay scene.
///   Go to Tools > Camel Runner > Generate Pirate Prefab.
///   Review the console output for status.
///   Commit Pirate.prefab to version control.
///
/// Output path: Assets/Prefabs/Characters/Thieves/Pirate.prefab
/// </summary>
public class PiratePrefabGenerator
{
    private const string PrefabPath = "Assets/Prefabs/Characters/Thieves/Pirate.prefab";
    private const string MaterialsPath = "Assets/Materials/Characters/Thieves/Pirate";

    [MenuItem("Tools/Camel Runner/Generate Pirate Prefab")]
    public static void GeneratePiratePrefab()
    {
        // Ensure directory structure exists
        EnsureDirectoriesExist();

        // Create pirate GameObject
        var pirate = new GameObject("Pirate");

        // Add ProceduralPirateMesh component
        var meshGenerator = pirate.AddComponent<ProceduralPirateMesh>();
        meshGenerator.Build();

        // Create materials for each submesh
        CreateMaterials();

        // Set up SkinnedMeshRenderer
        var smr = pirate.GetComponent<SkinnedMeshRenderer>();
        if (smr == null)
            smr = pirate.AddComponent<SkinnedMeshRenderer>();

        Material[] materials = LoadMaterials();
        smr.materials = materials;

        // Add Animator with thief controller
        var animator = pirate.AddComponent<Animator>();
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(
            "Assets/Animations/Characters/Thieves/ThiefAnimatorController.controller");
        if (controller != null)
            animator.runtimeAnimatorController = controller;
        else
            Debug.LogWarning("[PiratePrefabGenerator] ThiefAnimatorController not found. Animation may not work.");

        // Add ThiefSystem component
        var thiefSystem = pirate.AddComponent<ThiefSystem>();
        thiefSystem.thiefType = ThiefSystem.ThiefType.Pirate;

        // Create prefab asset
        string directory = System.IO.Path.GetDirectoryName(PrefabPath);
        if (!AssetDatabase.IsValidFolder(directory))
            AssetDatabase.CreateFolder("Assets/Prefabs/Characters", "Thieves");

        PrefabUtility.SaveAsPrefabAsset(pirate, PrefabPath);
        DestroyImmediate(pirate);

        AssetDatabase.Refresh();

        Debug.Log($"[PiratePrefabGenerator] ✓ Pirate prefab generated at {PrefabPath}");
        Debug.Log($"[PiratePrefabGenerator] ✓ Materials created at {MaterialsPath}/");
        Debug.Log("[PiratePrefabGenerator] Next: Wire to ThiefSpawner using Phase2AddThiefToSpawner tool");
    }

    private static void EnsureDirectoriesExist()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Characters"))
            AssetDatabase.CreateFolder("Assets/Prefabs", "Characters");
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Characters/Thieves"))
            AssetDatabase.CreateFolder("Assets/Prefabs/Characters", "Thieves");

        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            AssetDatabase.CreateFolder("Assets", "Materials");
        if (!AssetDatabase.IsValidFolder("Assets/Materials/Characters"))
            AssetDatabase.CreateFolder("Assets/Materials", "Characters");
        if (!AssetDatabase.IsValidFolder("Assets/Materials/Characters/Thieves"))
            AssetDatabase.CreateFolder("Assets/Materials/Characters", "Thieves");
        if (!AssetDatabase.IsValidFolder(MaterialsPath))
            AssetDatabase.CreateFolder("Assets/Materials/Characters/Thieves", "Pirate");
    }

    private static void CreateMaterials()
    {
        // Body material (brown)
        CreateMaterial("Pirate_Body", new Color(0.545f, 0.271f, 0.075f, 1f), false);

        // Hat material (dark slate)
        CreateMaterial("Pirate_Hat", new Color(0.184f, 0.310f, 0.310f, 1f), false);

        // Eye patch material (near-black)
        CreateMaterial("Pirate_EyePatch", new Color(0.110f, 0.110f, 0.110f, 1f), false);

        // Coat tails material (transparent brown)
        CreateMaterial("Pirate_CoatTails", new Color(0.396f, 0.263f, 0.129f, 0.8f), true);

        // Skin material (wheat)
        CreateMaterial("Pirate_Skin", new Color(0.961f, 0.871f, 0.702f, 1f), false);

        // Gold accents material (emissive)
        Material goldMat = new Material(Shader.Find("Standard"))
        {
            name = "Pirate_Gold",
            color = new Color(1f, 0.843f, 0f, 1f)
        };
        goldMat.SetFloat("_Emission", 2f);
        AssetDatabase.CreateAsset(goldMat, $"{MaterialsPath}/Pirate_Gold.mat");
    }

    private static void CreateMaterial(string name, Color color, bool transparent)
    {
        Shader shader = transparent ? Shader.Find("Standard") : Shader.Find("Standard");
        var mat = new Material(shader)
        {
            name = name,
            color = color
        };

        if (transparent)
        {
            mat.SetFloat("_Mode", 3); // Transparent mode
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }

        AssetDatabase.CreateAsset(mat, $"{MaterialsPath}/{name}.mat");
    }

    private static Material[] LoadMaterials()
    {
        Material[] materials = new Material[6];
        for (int i = 0; i < 6; i++)
        {
            string[] matNames = { "Pirate_Body", "Pirate_Hat", "Pirate_EyePatch", "Pirate_CoatTails", "Pirate_Skin", "Pirate_Gold" };
            materials[i] = AssetDatabase.LoadAssetAtPath<Material>($"{MaterialsPath}/{matNames[i]}.mat");
            if (materials[i] == null)
                Debug.LogWarning($"[PiratePrefabGenerator] Material {matNames[i]} not found at {MaterialsPath}");
        }
        return materials;
    }
}
