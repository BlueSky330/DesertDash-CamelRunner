using UnityEditor;
using UnityEngine;
using System;

public static class BuildScript
{
    public static void PerformAndroidBuild()
    {
        // Force ARM64 + ARMv7 for broad device compatibility
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;
        PlayerSettings.companyName = "AI Game Factory";
        PlayerSettings.productName = "Desert Dash Camel Runner";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.aigamefactory.desertdash");

        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = "build/Android/DesertDash-CamelRunner.apk",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.LogError("Build failed: " + report.summary.result);
            EditorApplication.Exit(1);
        }
        else
        {
            Debug.Log("Build succeeded: " + report.summary.outputPath);
            EditorApplication.Exit(0);
        }
    }

    private static string[] GetScenes()
    {
        var scenes = new System.Collections.Generic.List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled) scenes.Add(scene.path);
        }
        return scenes.ToArray();
    }
}
