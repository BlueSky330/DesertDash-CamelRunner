using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

public static class BuildScript
{
    public static void PerformAndroidBuild()
    {
        // ── Player Settings ────────────────────────────────────────────────
        PlayerSettings.companyName = "AI Game Factory";
        PlayerSettings.productName = "Desert Dash Camel Runner";
        PlayerSettings.applicationIdentifier = "com.aigamefactory.desertdash";
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.Android.bundleVersionCode = 1;

        // ── Android Architecture — ARMv7 + ARM64 ──────────────────────────
        PlayerSettings.Android.targetArchitectures =
            AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;

        // ── Scripting backend ─────────────────────────────────────────────
        PlayerSettings.SetScriptingBackend(
            BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

        // ── Android SDK versions ───────────────────────────────────────────
        PlayerSettings.Android.minSdkVersion  = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;

        // ── Scenes ────────────────────────────────────────────────────────
        var scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            // Fallback: include all scenes found in project
            scenes = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();
            Debug.LogWarning($"[BuildScript] No scenes in BuildSettings — using all {scenes.Length} scenes found.");
        }

        // ── Build options ─────────────────────────────────────────────────
        var options = new BuildPlayerOptions
        {
            scenes            = scenes,
            locationPathName  = "build/Android/DesertDash-CamelRunner.apk",
            target            = BuildTarget.Android,
            options           = BuildOptions.None,
        };

        Debug.Log($"[BuildScript] Building APK — scenes: {scenes.Length}, arch: ARMv7+ARM64");

        var report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            throw new Exception($"[BuildScript] Build FAILED: {report.summary.result} — {report.summary.totalErrors} errors");
        }

        Debug.Log($"[BuildScript] Build SUCCESS — {report.summary.totalSize} bytes");
    }

    public static void PerformIOSBuild()
    {
        // ── Player Settings ────────────────────────────────────────────────
        PlayerSettings.companyName = "AI Game Factory";
        PlayerSettings.productName = "Desert Dash Camel Runner";
        PlayerSettings.applicationIdentifier = "com.aigamefactory.desertdash";
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.iOS.buildNumber = "1";

        // ── Scripting backend ─────────────────────────────────────────────
        PlayerSettings.SetScriptingBackend(
            BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);

        // ── iOS version ───────────────────────────────────────────────────
        PlayerSettings.iOS.targetOSVersionString = "13.0";

        // ── Signing — manual, handled by Codemagic ────────────────────────
        PlayerSettings.iOS.appleEnableAutomaticSigning = false;
        PlayerSettings.iOS.iOSManualProvisioningProfileType =
            ProvisioningProfileType.Distribution;

        // ── Scenes ────────────────────────────────────────────────────────
        var scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            scenes = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();
            Debug.LogWarning($"[BuildScript] No scenes in BuildSettings — using all {scenes.Length} scenes found.");
        }

        // ── Build output path (Codemagic injects CM_BUILD_DIR) ────────────
        var buildPath = Environment.GetEnvironmentVariable("CM_BUILD_DIR") + "/ios"
                        ?? "build/ios/xcode";

        // ── Build options ─────────────────────────────────────────────────
        var options = new BuildPlayerOptions
        {
            scenes           = scenes,
            locationPathName = buildPath,
            target           = BuildTarget.iOS,
            options          = BuildOptions.None,
        };

        Debug.Log($"[BuildScript] Building iOS Xcode project — scenes: {scenes.Length}, output: {buildPath}");

        var report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            throw new Exception($"[BuildScript] iOS Build FAILED: {report.summary.result} — {report.summary.totalErrors} errors");
        }

        Debug.Log($"[BuildScript] iOS Build SUCCESS — {report.summary.totalSize} bytes");
    }
}
