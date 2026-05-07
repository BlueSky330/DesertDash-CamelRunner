using UnityEngine;
using UnityEditor;

/// <summary>
/// Helper to generate all Egypt environment assets in one step.
///
/// Run via: Tools > Camel Runner > Generate All Egypt Assets (One-Click)
///
/// This wrapper ensures all 16 procedural Egypt prefabs are created:
/// - 4 road/dune modules (Phase 1)
/// - 6 landmark props (Phase 2)
/// - 6 vegetation assets (Phase 3)
///
/// After generation, run: Tools > Camel Runner > Setup Placeholder Scene
/// Then: Tools > Camel Runner > Setup Egypt Level Scene
/// </summary>
public static class EgyptAssetGenerationHelper
{
    [MenuItem("Tools/Camel Runner/Generate All Egypt Assets (One-Click)", priority = 1)]
    public static void GenerateAllEgyptAssets()
    {
        Debug.Log("[EgyptAssetGenerationHelper] Starting Egypt asset generation...");

        try
        {
            // Call the procedural environment builder
            ProceduralEgyptEnvironmentBuilder.BuildAll();

            Debug.Log("[EgyptAssetGenerationHelper] Egypt asset generation complete!");
            Debug.Log("[EgyptAssetGenerationHelper] Next steps:");
            Debug.Log("  1. Tools > Camel Runner > Setup Placeholder Scene");
            Debug.Log("  2. Tools > Camel Runner > Setup Egypt Level Scene");
            Debug.Log("  3. Press Play and verify 60 FPS");

            EditorUtility.DisplayDialog(
                "Egypt Assets Generated",
                "All 16 Egypt environment prefabs have been generated.\n\n" +
                "Next Steps:\n" +
                "1. Run: Tools > Camel Runner > Setup Placeholder Scene\n" +
                "2. Run: Tools > Camel Runner > Setup Egypt Level Scene\n" +
                "3. Press Play to test\n\n" +
                "Check console for details.",
                "OK");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[EgyptAssetGenerationHelper] Error during asset generation: " + e.Message);
            EditorUtility.DisplayDialog(
                "Asset Generation Failed",
                "Error: " + e.Message + "\n\nCheck console for details.",
                "OK");
        }
    }

    [MenuItem("Tools/Camel Runner/Complete Egypt Setup Workflow (All-In-One)", priority = 2)]
    public static void CompleteEgyptSetup()
    {
        Debug.Log("[EgyptAssetGenerationHelper] Running complete Egypt setup workflow...");

        try
        {
            // Step 1: Generate all assets
            Debug.Log("[EgyptAssetGenerationHelper] Step 1/3: Generating assets...");
            ProceduralEgyptEnvironmentBuilder.BuildAll();

            // Step 2: Setup placeholder scene (pools, objects, etc)
            Debug.Log("[EgyptAssetGenerationHelper] Step 2/3: Setting up placeholder scene...");
            PlaceholderSceneSetup.SetupScene();

            // Step 3: Setup Egypt-specific scene (lighting, landmarks, parallax)
            Debug.Log("[EgyptAssetGenerationHelper] Step 3/3: Setting up Egypt scene...");
            EgyptSceneSetup.SetupEgyptScene();

            Debug.Log("[EgyptAssetGenerationHelper] Complete Egypt setup workflow finished!");
            Debug.Log("[EgyptAssetGenerationHelper] Ready to test! Press Play.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[EgyptAssetGenerationHelper] Error during complete setup: " + e.Message);
            EditorUtility.DisplayDialog(
                "Setup Failed",
                "Error: " + e.Message + "\n\nCheck console for details.",
                "OK");
        }
    }
}
