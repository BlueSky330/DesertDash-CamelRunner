using UnityEditor;
using UnityEditor.Build.Reporting;
using System.Collections.Generic;
using System.IO;

public class BuildScript
{
    public static void BuildAndroid()
    {
        string outputDir = "Builds";
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);
        
        string outputPath = Path.Combine(outputDir, "DesertDash.apk");
        
        string[] scenes = {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/Gameplay.unity",
            "Assets/Scenes/GameOver.unity"
        };

        List<string> validScenes = new List<string>();
        foreach (string scene in scenes)
        {
            if (File.Exists(scene))
                validScenes.Add(scene);
        }

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = validScenes.ToArray();
        options.locationPathName = outputPath;
        options.target = BuildTarget.Android;
        options.options = BuildOptions.None;

        PlayerSettings.companyName = "SkyVision";
        PlayerSettings.productName = "Desert Dash Camel Runner";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.skyvision.desertdash");
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

        UnityEngine.Debug.Log("Starting Android APK build...");
        BuildReport report = BuildPipeline.BuildPlayer(options);
        
        if (report.summary.result == BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log("========== APK BUILD SUCCEEDED ==========");
            UnityEngine.Debug.Log("Output: " + Path.GetFullPath(outputPath));
        }
        else
        {
            UnityEngine.Debug.LogError("========== APK BUILD FAILED ==========");
        }
    }
}
