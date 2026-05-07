using UnityEngine;
using System.Text;

/// <summary>
/// Logs device information for bug reports and compatibility testing.
/// Include this info in bug reports to help reproduce issues on specific devices.
///
/// Usage: Call LogDeviceInfo() at game start or on demand.
/// Output: Console log with device details + can be written to file for reports.
/// </summary>
public class DeviceCompatibilityLogger : MonoBehaviour
{
    public static void LogDeviceInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine("\n========== DEVICE COMPATIBILITY INFO ==========\n");

        // Device Hardware
        sb.AppendLine("--- DEVICE HARDWARE ---");
        sb.AppendLine($"Device Name: {SystemInfo.deviceName}");
        sb.AppendLine($"Device Model: {SystemInfo.deviceModel}");
        sb.AppendLine($"Device Type: {SystemInfo.deviceType}");
        sb.AppendLine($"Device ID: {SystemInfo.deviceUniqueIdentifier}");

        // OS Information
        sb.AppendLine("\n--- OPERATING SYSTEM ---");
        sb.AppendLine($"OS: {SystemInfo.operatingSystem}");
        sb.AppendLine($"OS Version: {SystemInfo.operatingSystemFamily}");

        // Processor
        sb.AppendLine("\n--- PROCESSOR ---");
        sb.AppendLine($"Processor Count: {SystemInfo.processorCount}");
        sb.AppendLine($"Processor Type: {SystemInfo.processorType}");
        sb.AppendLine($"Processor Frequency: {SystemInfo.processorFrequency} MHz");

        // Memory
        sb.AppendLine("\n--- MEMORY ---");
        sb.AppendLine($"RAM: {SystemInfo.systemMemorySize} MB");
        sb.AppendLine($"Current Heap Size: {(System.GC.GetTotalMemory(false) / (1024 * 1024))} MB");

        // Graphics
        sb.AppendLine("\n--- GRAPHICS ---");
        sb.AppendLine($"GPU Name: {SystemInfo.graphicsDeviceName}");
        sb.AppendLine($"GPU Type: {SystemInfo.graphicsDeviceType}");
        sb.AppendLine($"GPU Memory: {SystemInfo.graphicsMemorySize} MB");
        sb.AppendLine($"GPU Vendor: {SystemInfo.graphicsDeviceVendor}");
        sb.AppendLine($"GPU Driver Version: {SystemInfo.graphicsDeviceVersion}");
        sb.AppendLine($"Max Texture Size: {SystemInfo.maxTextureSize}");
        sb.AppendLine($"Supported Render Texture Formats: {SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Default)}");

        // Screen
        sb.AppendLine("\n--- SCREEN ---");
        sb.AppendLine($"Resolution: {Screen.width} x {Screen.height}");
        sb.AppendLine($"Aspect Ratio: {(float)Screen.width / Screen.height:F2}");
        sb.AppendLine($"DPI: {Screen.dpi}");
        sb.AppendLine($"Refresh Rate: {Screen.currentResolution.refreshRate} Hz");

        // Audio
        sb.AppendLine("\n--- AUDIO ---");
        sb.AppendLine($"Audio DSP Buffer Size: {AudioSettings.GetConfiguration().dspBufferSize}");
        sb.AppendLine($"Audio Sample Rate: {AudioSettings.outputSampleRate} Hz");

        // Battery & Power
        sb.AppendLine("\n--- POWER & BATTERY ---");
        sb.AppendLine($"Battery Level: {SystemInfo.batteryLevel * 100:F1}%");
        sb.AppendLine($"Battery Status: {SystemInfo.batteryStatus}");

        // Network
        sb.AppendLine("\n--- NETWORK ---");
        sb.AppendLine($"Internet Reachability: {Application.internetReachability}");

        // Supports
        sb.AppendLine("\n--- FEATURE SUPPORT ---");
        sb.AppendLine($"Supports Instancing: {SystemInfo.supportsInstancing}");
        sb.AppendLine($"Supports Compute Shaders: {SystemInfo.supportsComputeShaders}");
        sb.AppendLine($"Supports Async GPU Readback: {SystemInfo.supportsAsyncGPUReadback}");

        // Unity & Build Info
        sb.AppendLine("\n--- BUILD INFO ---");
        sb.AppendLine($"Unity Version: {Application.unityVersion}");
        sb.AppendLine($"Build GUID: {Application.cloudProjectId}");
        sb.AppendLine($"App Version: {Application.version}");
        sb.AppendLine($"Bundle Version: {Application.installerName}");
        sb.AppendLine($"Is Editor: {Application.isEditor}");
        sb.AppendLine($"Platform: {Application.platform}");
        sb.AppendLine($"Build Architecture: {RuntimeInformation.OSArchitecture}");

        // Performance
        sb.AppendLine("\n--- PERFORMANCE ---");
        sb.AppendLine($"Frame Rate Target: {Application.targetFrameRate}");
        sb.AppendLine($"Current FPS: {(1f / Time.deltaTime):F1}");
        sb.AppendLine($"Time Scale: {Time.timeScale}");

        sb.AppendLine("\n==============================================\n");

        Debug.Log(sb.ToString());
    }

    public static string GetBugReportDeviceInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"**Device:** {SystemInfo.deviceModel}");
        sb.AppendLine($"**OS:** {SystemInfo.operatingSystemFamily} {SystemInfo.operatingSystem}");
        sb.AppendLine($"**RAM:** {SystemInfo.systemMemorySize} MB");
        sb.AppendLine($"**GPU:** {SystemInfo.graphicsDeviceName}");
        sb.AppendLine($"**Screen:** {Screen.width}×{Screen.height} @ {Screen.dpi} DPI");
        sb.AppendLine($"**Unity:** {Application.unityVersion}");
        sb.AppendLine($"**Battery:** {SystemInfo.batteryLevel * 100:F0}%");

        return sb.ToString();
    }

    public static void LogScreenResolutionCompatibility()
    {
        Debug.Log($"Screen Resolution: {Screen.width}x{Screen.height} ({(float)Screen.width/Screen.height:F2} aspect)");

        // Common mobile resolutions
        if (Screen.width == 720 && Screen.height == 1600) Debug.Log("Device: Galaxy A series (standard)");
        else if (Screen.width == 1080 && Screen.height == 2340) Debug.Log("Device: Galaxy A51/S10/flagship");
        else if (Screen.width == 1080 && Screen.height == 2400) Debug.Log("Device: Pixel 4/5 series");
        else if (Screen.width == 828 && Screen.height == 1792) Debug.Log("Device: iPhone 11/12 mini");
        else if (Screen.width == 1170 && Screen.height == 2532) Debug.Log("Device: iPhone 12/13 Pro");
        else Debug.Log($"Device: Custom resolution {Screen.width}x{Screen.height}");
    }
}

// Polyfill for RuntimeInformation (if not available in older .NET)
public static class RuntimeInformation
{
    public static string OSArchitecture => SystemInfo.processorType.Contains("ARM") ? "ARM" : "x86";
}
