using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Creates all four camel skin material variants with correct properties.
/// Usage: Unity Editor > Assets > Create Camel Materials > Setup All Skins
/// </summary>
public class CreateCamelMaterials
{
    private const string MATERIALS_PATH = "Assets/Materials/Camel/Skins";
    private const string SHADER_NAME = "Standard";

    [MenuItem("Assets/Create Camel Materials/Setup All Skins")]
    public static void SetupAllMaterials()
    {
        // Ensure directory exists
        if (!AssetDatabase.IsValidFolder(MATERIALS_PATH))
        {
            string materialsParent = "Assets/Materials/Camel";
            if (!AssetDatabase.IsValidFolder(materialsParent))
            {
                string materialsRoot = "Assets/Materials";
                if (!AssetDatabase.IsValidFolder(materialsRoot))
                {
                    AssetDatabase.CreateFolder("Assets", "Materials");
                }
                AssetDatabase.CreateFolder(materialsRoot, "Camel");
            }
            AssetDatabase.CreateFolder(materialsParent, "Skins");
        }

        CreatePharaohMaterial();
        CreateRacingMaterial();
        CreateMummyMaterial();
        CreateGoldenMaterial();

        AssetDatabase.Refresh();
        Debug.Log("✓ All camel skin materials created successfully!");
    }

    private static void CreatePharaohMaterial()
    {
        Material mat = new Material(Shader.Find(SHADER_NAME));
        mat.name = "Camel_Pharaoh";

        // Main body - tan/sand
        mat.SetColor("_Color", HexToColor("#C4A574"));
        mat.SetFloat("_Metallic", 0.0f);
        mat.SetFloat("_Glossiness", 0.5f);

        SaveMaterial(mat, "Camel_Pharaoh");

        // Notes: Additional materials for sub-meshes:
        // - Camel_Pharaoh_Gold (for collar, bands, headdress stripes gold parts)
        // - Camel_Pharaoh_GoldGem (for sapphire collar center)
        // - Camel_Pharaoh_Blue (for headdress blue stripes)

        CreateSubMaterial("Camel_Pharaoh_Gold", HexToColor("#FFD700"), 0.8f, 0.7f);
        CreateSubMaterial("Camel_Pharaoh_GoldGem", HexToColor("#0F52BA"), 1.0f, 0.9f);
        CreateSubMaterial("Camel_Pharaoh_Blue", HexToColor("#4169E1"), 0.8f, 0.7f);
    }

    private static void CreateRacingMaterial()
    {
        Material mat = new Material(Shader.Find(SHADER_NAME));
        mat.name = "Camel_Racing";

        // Main body - tan/sand
        mat.SetColor("_Color", HexToColor("#C4A574"));
        mat.SetFloat("_Metallic", 0.0f);
        mat.SetFloat("_Glossiness", 0.5f);

        SaveMaterial(mat, "Camel_Racing");

        // Notes: Additional materials for sub-meshes:
        // - Camel_Racing_Black (for goggles, harness)
        // - Camel_Racing_Yellow (for visor, stripes)

        CreateSubMaterial("Camel_Racing_Black", HexToColor("#1a1a1a"), 0.2f, 0.4f);
        CreateSubMaterial("Camel_Racing_Yellow", HexToColor("#FFD700"), 0.6f, 0.7f);
    }

    private static void CreateMummyMaterial()
    {
        Material mat = new Material(Shader.Find(SHADER_NAME));
        mat.name = "Camel_Mummy";

        // Main wrapped body - white linen
        mat.SetColor("_Color", HexToColor("#F5F5F5"));
        mat.SetFloat("_Metallic", 0.0f);
        mat.SetFloat("_Glossiness", 0.3f);

        SaveMaterial(mat, "Camel_Mummy");

        // Notes: Additional materials for sub-meshes:
        // - Camel_Mummy_Shadow (for wrapping shadows)
        // - Camel_Mummy_Eyes (for glowing yellow eyes with emission)
        // - Camel_Mummy_Tattered (for tattered wrap edges)

        CreateSubMaterial("Camel_Mummy_Shadow", HexToColor("#8B7355"), 0.0f, 0.2f);

        Material eyeMat = CreateSubMaterial("Camel_Mummy_Eyes", HexToColor("#FFFF00"), 0.0f, 0.8f);
        eyeMat.SetColor("_EmissionColor", HexToColor("#FFFF00"));
        eyeMat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;

        CreateSubMaterial("Camel_Mummy_Tattered", HexToColor("#654321"), 0.0f, 0.2f);
    }

    private static void CreateGoldenMaterial()
    {
        Material mat = new Material(Shader.Find(SHADER_NAME));
        mat.name = "Camel_Golden";

        // Main polished gold body - metallic with high specularity
        mat.SetColor("_Color", HexToColor("#FFD700"));
        mat.SetFloat("_Metallic", 1.0f);
        mat.SetFloat("_Glossiness", 0.9f);  // High specularity for premium look

        SaveMaterial(mat, "Camel_Golden");

        // Gem variants with precise spec colors
        CreateSubMaterial("Camel_Golden_ShadowGold", HexToColor("#B8860B"), 1.0f, 0.7f);
        CreateSubMaterial("Camel_Golden_Ruby", HexToColor("#E50000"), 1.0f, 0.95f);      // Spec: ruby red
        CreateSubMaterial("Camel_Golden_Emerald", HexToColor("#00B050"), 1.0f, 0.95f);   // Spec: emerald green
        CreateSubMaterial("Camel_Golden_Sapphire", HexToColor("#0047AB"), 1.0f, 0.95f);  // Spec: sapphire blue

        // Geometric relief pattern (normal-mapped tassels via pattern material)
        CreateSubMaterial("Camel_Golden_Pattern", HexToColor("#FFD700"), 0.8f, 0.75f);
    }

    private static Material CreateSubMaterial(string name, Color color, float metallic, float smoothness)
    {
        Material mat = new Material(Shader.Find(SHADER_NAME));
        mat.name = name;
        mat.SetColor("_Color", color);
        mat.SetFloat("_Metallic", metallic);
        mat.SetFloat("_Glossiness", smoothness);

        SaveMaterial(mat, name);
        return mat;
    }

    private static void SaveMaterial(Material mat, string filename)
    {
        string path = $"{MATERIALS_PATH}/{filename}.mat";
        AssetDatabase.CreateAsset(mat, path);
        Debug.Log($"Created material: {path}");
    }

    private static Color HexToColor(string hex)
    {
        hex = hex.Replace("#", "");
        float r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        float g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        float b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        return new Color(r, g, b, 1f);
    }
}
