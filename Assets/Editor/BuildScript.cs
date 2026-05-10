using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// Headless build script for CI/CD pipelines (Codemagic, GitHub Actions, Unity Build Agent).
/// Invoked via Unity -executeMethod BuildScript.BuildIOS or BuildScript.PerformAndroidBuild.
/// </summary>
public static class BuildScript
{
    private const string BundleId      = "com.skyvision.desertdash";
    private const string CompanyName   = "SkyVision";
    private const string ProductName   = "Desert Dash: Camel Runner";
    private const string BundleVersion = "1.0.0";

    // iOS Build (invoked by Codemagic: -executeMethod BuildScript.BuildIOS)
    public static void BuildIOS()
    {
        Debug.Log("[BuildScript] Starting iOS build...");

        PlayerSettings.companyName   = CompanyName;
        PlayerSettings.productName   = ProductName;
        PlayerSettings.bundleVersion = BundleVersion;
        PlayerSettings.iOS.buildNumber = "1";

        PlayerSettings.applicationIdentifier = BundleId;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, BundleId);

        // iOS 13.0 minimum
        PlayerSettings.iOS.targetOSVersionString = "13.0";

        // IL2CPP scripting backend
        PlayerSettings.SetScriptingBackend(
            BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);

        PlayerSettings.SetApiCompatibilityLevel(
            BuildTargetGroup.iOS, ApiCompatibilityLevel.NET_Standard_2_1);

        PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;

        // Codemagic manages code signing via xcode-project use-profiles
        PlayerSettings.iOS.appleEnableAutomaticSigning = false;

        var scenes = GetScenes();

        string outputPath = "ios_build";
        System.IO.Directory.CreateDirectory(outputPath);

        var options = new BuildPlayerOptions
        {
            scenes           = scenes,
            locationPathName = outputPath,
            target           = BuildTarget.iOS,
            options          = BuildOptions.None,
        };

        Debug.Log($"[BuildScript] iOS build: {scenes.Length} scenes -> {outputPath}");

        var report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            throw new Exception(
                $"[BuildScript] iOS build FAILED: {report.summary.result} - {report.summary.totalErrors} errors");
        }

        Debug.Log($"[BuildScript] iOS build SUCCESS - {report.summary.totalSize} bytes");
    }

    // Android Build (invoked by Unity Build Agent batch mode)
    public static void PerformAndroidBuild()
    {
        Debug.Log("[BuildScript] Starting Android build...");

        PlayerSettings.companyName   = CompanyName;
        PlayerSettings.productName   = ProductName;
        PlayerSettings.bundleVersion = BundleVersion;
        PlayerSettings.Android.bundleVersionCode = 1;

        PlayerSettings.applicationIdentifier = BundleId;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, BundleId);

        PlayerSettings.Android.targetArchitectures =
            AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;

        PlayerSettings.SetScriptingBackend(
            BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

        PlayerSettings.Android.minSdkVersion    = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;

        var scenes = GetScenes();

        System.IO.Directory.CreateDirectory("build/Android");
        var options = new BuildPlayerOptions
        {
            scenes           = scenes,
            locationPathName = "build/Android/DesertDash-CamelRunner.apk",
            target           = BuildTarget.Android,
            options          = BuildOptions.None,
        };

        Debug.Log($"[BuildScript] Android build: {scenes.Length} scenes -> APK");

        var report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            throw new Exception(
                $"[BuildScript] Android build FAILED: {report.summary.result} - {report.summary.totalErrors} errors");
        }

        Debug.Log($"[BuildScript] Android build SUCCESS - {report.summary.totalSize} bytes");
    }

    private static string[] GetScenes()
    {
        var scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            scenes = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();
            Debug.LogWarning(
                $"[BuildScript] No enabled scenes in BuildSettings - falling back to all {scenes.Length} scenes.");
        }

        return scenes;
    }
}
