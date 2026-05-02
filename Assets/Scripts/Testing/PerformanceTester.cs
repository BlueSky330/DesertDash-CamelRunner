using UnityEngine;
using System.Collections;
using System.Text;

public class PerformanceTester : MonoBehaviour
{
    [Header("Performance Test Settings")]
    public float testDuration = 60f; // Duration of the performance test
    public float fpsUpdateInterval = 0.5f; // How often to update FPS display
    public int targetFPS = 60;

    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval
    private float fps = 0;

    private long initialMemoryUsage;
    private long peakMemoryUsage;

    private StringBuilder report = new StringBuilder();

    void Start()
    {
        // StartPerformanceTest(); // Call this from a UI button or dedicated test scene
    }

    public void StartPerformanceTest()
    {
        Debug.Log("Starting Performance Test...");
        report.Clear();
        report.AppendLine("--- Performance Test Report ---");

        timeleft = fpsUpdateInterval;
        initialMemoryUsage = System.GC.GetTotalMemory(false);
        peakMemoryUsage = initialMemoryUsage;

        StartCoroutine(RunPerformanceTest());
    }

    private IEnumerator RunPerformanceTest()
    {
        float timer = 0f;
        while (timer < testDuration)
        {
            timer += Time.deltaTime;
            UpdateFPS();
            UpdateMemoryUsage();
            yield return null;
        }
        GenerateReport();
        Debug.Log("Performance Test Finished.");
    }

    void UpdateFPS()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if (timeleft <= 0.0f)
        {
            fps = accum / frames;
            // Debug.Log($"FPS: {fps:F2}");
            timeleft = fpsUpdateInterval;
            accum = 0;
            frames = 0;
        }
    }

    void UpdateMemoryUsage()
    {
        long currentMemory = System.GC.GetTotalMemory(false);
        if (currentMemory > peakMemoryUsage)
        {
            peakMemoryUsage = currentMemory;
        }
        // Debug.Log($"Current Memory: {currentMemory / (1024 * 1024)} MB");
    }

    private void GenerateReport()
    {
        report.AppendLine($"\n--- Summary ---");
        report.AppendLine($"Test Duration: {testDuration} seconds");
        report.AppendLine($"Target FPS: {targetFPS}");
        report.AppendLine($"Average FPS: {fps:F2}");
        report.AppendLine($"Initial Memory Usage: {initialMemoryUsage / (1024 * 1024)} MB");
        report.AppendLine($"Peak Memory Usage: {peakMemoryUsage / (1024 * 1024)} MB");

        report.AppendLine($"\n--- Optimization Checks (Manual Verification Required) ---");
        report.AppendLine($"Draw Call Optimization: Verify batching and instancing.");
        report.AppendLine($"Texture Memory Usage: Check texture import settings (compression, max size).");
        report.AppendLine($"Object Pooling: Ensure obstacles, collectibles, effects are pooled.");
        report.AppendLine($"Battery Usage: Monitor device battery during extended play.");
        report.AppendLine($"LOD System: Verify distant objects use lower detail models.");
        report.AppendLine($"Shader Optimization: Ensure shaders are mobile-friendly.");
        report.AppendLine($"Garbage Collection: Monitor GC spikes in Unity Profiler.");

        Debug.Log(report.ToString());
    }
}
