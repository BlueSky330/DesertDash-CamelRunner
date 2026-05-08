using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class TextureUpdater
{
    public static void ApplyAll()
    {
        UnityEngine.Debug.Log("[TextureUpdater] Starting texture application...");

        int materialsUpdated = 0;
        int errorsFound = 0;

        // Define texture-to-material mappings
        var textureMap = new Dictionary<string, List<string>>
        {
            { "Assets/Textures/background_desert.png", new List<string> { "Materials/BackgroundDesert" } },
            { "Assets/Textures/sky_desert.png", new List<string> { "Materials/SkyDesert" } },
            { "Assets/Textures/ground_sand.png", new List<string> { "Materials/GroundSand" } },
            { "Assets/Textures/camel_idle.png", new List<string> { "Materials/CamelIdle" } },
            { "Assets/Textures/camel_jump.png", new List<string> { "Materials/CamelJump" } },
            { "Assets/Textures/camel_run_0.png", new List<string> { "Materials/CamelRun0" } },
            { "Assets/Textures/camel_run_1.png", new List<string> { "Materials/CamelRun1" } },
            { "Assets/Textures/camel_run_2.png", new List<string> { "Materials/CamelRun2" } },
            { "Assets/Textures/camel_run_3.png", new List<string> { "Materials/CamelRun3" } },
            { "Assets/Textures/collectible_coin.png", new List<string> { "Materials/Coin" } },
            { "Assets/Textures/cactus_obstacle.png", new List<string> { "Materials/CactusObstacle" } },
            { "Assets/Textures/rock_obstacle.png", new List<string> { "Materials/RockObstacle" } },
            { "Assets/Textures/ruins_obstacle.png", new List<string> { "Materials/RuinsObstacle" } },
            { "Assets/Textures/particle_dust.png", new List<string> { "Materials/DustParticle" } },
            { "Assets/Textures/particle_impact.png", new List<string> { "Materials/ImpactParticle" } }
        };

        // Apply each texture to its corresponding materials
        foreach (var texturePath in textureMap.Keys)
        {
            if (!File.Exists(texturePath))
            {
                UnityEngine.Debug.LogWarning($"[TextureUpdater] Texture not found: {texturePath}");
                continue;
            }

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if (texture == null)
            {
                UnityEngine.Debug.LogWarning($"[TextureUpdater] Failed to load texture: {texturePath}");
                errorsFound++;
                continue;
            }

            foreach (string materialPath in textureMap[texturePath])
            {
                string fullMaterialPath = materialPath + ".mat";
                Material material = AssetDatabase.LoadAssetAtPath<Material>(fullMaterialPath);

                if (material == null)
                {
                    UnityEngine.Debug.LogWarning($"[TextureUpdater] Material not found: {fullMaterialPath}");
                    continue;
                }

                // Apply texture to the material (standard property is "_MainTex")
                material.SetTexture("_MainTex", texture);
                EditorUtility.SetDirty(material);
                UnityEngine.Debug.Log($"[TextureUpdater] Applied {Path.GetFileName(texturePath)} to {Path.GetFileName(materialPath)}");
                materialsUpdated++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        UnityEngine.Debug.Log($"[TextureUpdater] === Done — {materialsUpdated} materials updated ===");
        if (errorsFound > 0)
        {
            UnityEngine.Debug.LogError($"[TextureUpdater] {errorsFound} errors found during texture application");
        }
    }
}
