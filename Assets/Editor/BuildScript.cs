using UnityEditor;
using UnityEditor.Build.Reporting;
using System.Collections.Generic;
using System.IO;

public class BuildScript
{
    private static readonly string[] Scenes = {
        "Assets/Scenes/MainMenu.unity",
        "Assets/Scenes/Gameplay.unity",
        "Assets/Scenes/GameOver.unity"
    };

    private static List<string> GetValidScenes()
    {
        List<string> validScenes = new List<string>();
        foreach (string scene in Scenes)
        {
            if (File.Exists(scene))
                validScenes.Add(scene);
        }
        return validScenes;
    }

    private static void ConfigureAndroidSettings()
    {
        PlayerSettings.companyName = "SkyVision";
        PlayerSettings.productName = "Desert Dash Camel Runner";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.skyvision.desertdash");
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel34;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
    }

    private static void ConfigureiOSSettings()
    {
        PlayerSettings.companyName = "SkyVision";
        PlayerSettings.productName = "Desert Dash Camel Runner";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.skyvision.desertdash");
        PlayerSettings.iOS.targetOSVersionString = "12.0";
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
    }

    public static void BuildAPK()
    {
        string outputDir = "Builds";
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        string outputPath = Path.Combine(outputDir, "DesertDash.apk");
        List<string> validScenes = GetValidScenes();

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = validScenes.ToArray();
        options.locationPathName = outputPath;
        options.target = BuildTarget.Android;
        options.options = BuildOptions.None;

        ConfigureAndroidSettings();

        UnityEngine.Debug.Log("========== Starting Android APK build with test ad IDs ==========");
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

    public static void BuildAAB()
    {
        string outputDir = "Builds";
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        string outputPath = Path.Combine(outputDir, "DesertDash.aab");
        List<string> validScenes = GetValidScenes();

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = validScenes.ToArray();
        options.locationPathName = outputPath;
        options.target = BuildTarget.Android;
        options.options = BuildOptions.None;

        ConfigureAndroidSettings();
        EditorUserBuildSettings.buildAppBundle = true;

        UnityEngine.Debug.Log("========== Starting Android AAB (App Bundle) build with test ad IDs ==========");
        BuildReport report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result == BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log("========== AAB BUILD SUCCEEDED ==========");
            UnityEngine.Debug.Log("Output: " + Path.GetFullPath(outputPath));
        }
        else
        {
            UnityEngine.Debug.LogError("========== AAB BUILD FAILED ==========");
        }
    }

    public static void BuildiOS()
    {
        string outputDir = "Builds";
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        string outputPath = Path.Combine(outputDir, "DesertDash_iOS");
        List<string> validScenes = GetValidScenes();

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = validScenes.ToArray();
        options.locationPathName = outputPath;
        options.target = BuildTarget.iOS;
        options.options = BuildOptions.None;

        ConfigureiOSSettings();

        UnityEngine.Debug.Log("========== Starting iOS build with test ad IDs (generates Xcode project) ==========");
        BuildReport report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result == BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log("========== iOS BUILD SUCCEEDED ==========");
            UnityEngine.Debug.Log("Xcode project generated at: " + Path.GetFullPath(outputPath));
            UnityEngine.Debug.Log("Next: Open in Xcode, configure signing, and archive for App Store");
        }
        else
        {
            UnityEngine.Debug.LogError("========== iOS BUILD FAILED ==========");
        }
    }

    public static void ExportAndroidStudio()
    {
        string outputDir = "Builds";
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        string studioPath = Path.Combine(outputDir, "DesertDashStudio");
        List<string> validScenes = GetValidScenes();

        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = validScenes.ToArray();
        options.locationPathName = studioPath;
        options.target = BuildTarget.Android;
        options.options = BuildOptions.None;

        ConfigureAndroidSettings();

        UnityEngine.Debug.Log("Exporting to Android Studio with test ad IDs...");
        BuildReport report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result == BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log("========== ANDROID STUDIO EXPORT SUCCEEDED ==========");
            UnityEngine.Debug.Log("Output: " + Path.GetFullPath(studioPath));
        }
        else
        {
            UnityEngine.Debug.LogError("========== ANDROID STUDIO EXPORT FAILED ==========");
        }
    }
}
