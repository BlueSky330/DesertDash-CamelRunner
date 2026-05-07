#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor utility to create skin material templates and test materials.
/// Run from Tools > Camel Runner > Setup Skin Materials
/// </summary>
public class SkinMaterialSetup
{
    [MenuItem("Tools/Camel Runner/Setup Skin Materials", priority = 100)]
    public static void SetupSkinMaterials()
    {
        string basePath = "Assets/Materials/Skins";
        string resourcesPath = "Assets/Resources/Skins";

        // Create directories if they don't exist
        CreateDirectoryIfNeeded(basePath);
        CreateDirectoryIfNeeded(resourcesPath);

        // Define skin color palette
        var skinDefinitions = new[]
        {
            new { name = "Camel (Base)", color = new Color(0.82f, 0.70f, 0.55f) }, // Sandy tan
            new { name = "Pharaoh Camel", color = new Color(1f, 0.84f, 0f) }, // Gold
            new { name = "Racing Camel", color = new Color(0.2f, 0.2f, 0.2f) }, // Black
            new { name = "Mummy Camel", color = new Color(0.9f, 0.85f, 0.7f) }, // Off-white/wrapped
            new { name = "Golden Camel", color = new Color(1f, 0.95f, 0.4f) } // Bright gold
        };

        // Create materials and templates for each skin
        foreach (var skin in skinDefinitions)
        {
            CreateSkinSetup(basePath, resourcesPath, skin.name, skin.color);
        }

        AssetDatabase.Refresh();
        Debug.Log("[SkinMaterialSetup] Skin materials and templates created successfully!");
    }

    private static void CreateSkinSetup(string basePath, string resourcesPath, string skinName, Color baseColor)
    {
        string skinFolder = Path.Combine(basePath, SanitizeFolderName(skinName));
        CreateDirectoryIfNeeded(skinFolder);

        // Create base material
        Material baseMat = new Material(Shader.Find("Standard"));
        baseMat.name = $"{skinName}_Default";
        baseMat.color = baseColor;
        AssetDatabase.CreateAsset(baseMat, Path.Combine(skinFolder, $"{baseMat.name}.mat"));

        // Create variant material (darker shade)
        Material darkMat = new Material(Shader.Find("Standard"));
        darkMat.name = $"{skinName}_Dark";
        darkMat.color = baseColor * 0.7f; // darker
        AssetDatabase.CreateAsset(darkMat, Path.Combine(skinFolder, $"{darkMat.name}.mat"));

        // Create variant material (lighter shade)
        Material lightMat = new Material(Shader.Find("Standard"));
        lightMat.name = $"{skinName}_Light";
        lightMat.color = Color.Lerp(baseColor, Color.white, 0.3f); // lighter
        AssetDatabase.CreateAsset(lightMat, Path.Combine(skinFolder, $"{lightMat.name}.mat"));

        // Create template scriptable object in Resources folder
        var template = ScriptableObject.CreateInstance<SkinMaterialTemplate>();
        template.skinName = skinName;
        template.variants = new SkinMaterialTemplate.MaterialVariant[3]
        {
            new SkinMaterialTemplate.MaterialVariant { variantName = "Default", material = baseMat },
            new SkinMaterialTemplate.MaterialVariant { variantName = "Dark", material = darkMat },
            new SkinMaterialTemplate.MaterialVariant { variantName = "Light", material = lightMat }
        };
        template.defaultVariantIndex = 0;

        string templatePath = Path.Combine(resourcesPath, $"SkinTemplate_{SanitizeFileName(skinName)}.asset");
        AssetDatabase.CreateAsset(template, templatePath);

        Debug.Log($"[SkinMaterialSetup] Created skin setup for '{skinName}' at {skinFolder}");
    }

    private static void CreateDirectoryIfNeeded(string path)
    {
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }
    }

    private static string SanitizeFolderName(string name)
    {
        return name.Replace(" ", "_").Replace("(", "").Replace(")", "");
    }

    private static string SanitizeFileName(string name)
    {
        return name.Replace(" ", "_").Replace("(", "").Replace(")", "");
    }
}
#endif
