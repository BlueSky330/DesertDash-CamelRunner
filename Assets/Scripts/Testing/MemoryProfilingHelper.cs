using UnityEngine;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Utility for monitoring memory usage, GC allocations, and tracking potential memory leaks.
/// Logs heap snapshots at intervals to detect allocation patterns and spikes.
///
/// Usage: Attach to a test scene GameObject and call StartMemoryProfiling()
/// </summary>
public class MemoryProfilingHelper : MonoBehaviour
{
    [Header("Profiling Settings")]
    [SerializeField] private float sampleIntervalSeconds = 5f;
    [SerializeField] private int maxHistorySize = 120; // 10 minutes at 5s intervals
    [SerializeField] private long maxMemoryThreshold = 350 * 1024 * 1024; // 350 MB warning threshold

    private List<MemorySample> samples = new List<MemorySample>();
    private float timeSinceLastSample = 0f;
    private bool isProfilingActive = false;
    private StringBuilder report = new StringBuilder();

    private struct MemorySample
    {
        public float timeStamp;
        public long heapSize;
        public long totalAllocated;
    }

    void Update()
    {
        if (!isProfilingActive) return;

        timeSinceLastSample += Time.deltaTime;
        if (timeSinceLastSample >= sampleIntervalSeconds)
        {
            RecordSample();
            timeSinceLastSample = 0f;
        }
    }

    public void StartMemoryProfiling()
    {
        Debug.Log("[MemoryProfilingHelper] Memory profiling started.");
        isProfilingActive = true;
        samples.Clear();
        report.Clear();
        RecordSample(); // Record initial state
    }

    public void StopMemoryProfiling()
    {
        isProfilingActive = false;
        Debug.Log("[MemoryProfilingHelper] Memory profiling stopped.");
        GenerateReport();
    }

    private void RecordSample()
    {
        long heapSize = System.GC.GetTotalMemory(false);

        var sample = new MemorySample
        {
            timeStamp = Time.realtimeSinceStartup,
            heapSize = heapSize,
            totalAllocated = heapSize
        };

        samples.Add(sample);

        // Trim old samples if list exceeds max history
        if (samples.Count > maxHistorySize)
        {
            samples.RemoveAt(0);
        }

        // Check if memory usage exceeds threshold
        if (heapSize > maxMemoryThreshold)
        {
            Debug.LogWarning($"[MemoryProfilingHelper] Memory usage ({heapSize / (1024 * 1024)} MB) exceeds threshold ({maxMemoryThreshold / (1024 * 1024)} MB)!");
        }
    }

    private void GenerateReport()
    {
        if (samples.Count == 0)
        {
            Debug.LogWarning("[MemoryProfilingHelper] No samples recorded.");
            return;
        }

        report.AppendLine("\n=== Memory Profiling Report ===\n");
        report.AppendLine($"Total Samples: {samples.Count}");
        report.AppendLine($"Profiling Duration: {samples[samples.Count - 1].timeStamp - samples[0].timeStamp:F1} seconds\n");

        // Calculate statistics
        long minMemory = samples[0].heapSize;
        long maxMemory = samples[0].heapSize;
        long totalMemory = 0;

        foreach (var sample in samples)
        {
            if (sample.heapSize < minMemory) minMemory = sample.heapSize;
            if (sample.heapSize > maxMemory) maxMemory = sample.heapSize;
            totalMemory += sample.heapSize;
        }

        long avgMemory = totalMemory / samples.Count;
        long memoryDelta = samples[samples.Count - 1].heapSize - samples[0].heapSize;

        report.AppendLine("=== Memory Statistics ===");
        report.AppendLine($"Initial Memory: {FormatBytes(samples[0].heapSize)}");
        report.AppendLine($"Final Memory: {FormatBytes(samples[samples.Count - 1].heapSize)}");
        report.AppendLine($"Min Memory: {FormatBytes(minMemory)}");
        report.AppendLine($"Max Memory: {FormatBytes(maxMemory)}");
        report.AppendLine($"Average Memory: {FormatBytes(avgMemory)}");
        report.AppendLine($"Memory Delta: {FormatBytes(memoryDelta)} ({(memoryDelta > 0 ? "+" : "")}{(float)memoryDelta / (1024 * 1024):F1} MB)\n");

        // Detect potential memory leaks
        if (memoryDelta > 50 * 1024 * 1024) // > 50 MB increase
        {
            report.AppendLine("⚠️  WARNING: Significant memory increase detected (potential memory leak)");
        }

        // Check against target threshold
        if (maxMemory > maxMemoryThreshold)
        {
            report.AppendLine($"⚠️  WARNING: Peak memory ({FormatBytes(maxMemory)}) exceeds target threshold ({FormatBytes(maxMemoryThreshold)})");
        }
        else
        {
            report.AppendLine($"✓ Memory usage within target threshold ({FormatBytes(maxMemoryThreshold)})");
        }

        report.AppendLine("\n=== Sample Timeline ===");
        for (int i = 0; i < samples.Count; i++)
        {
            float elapsed = samples[i].timeStamp - samples[0].timeStamp;
            report.AppendLine($"  {elapsed:F1}s : {FormatBytes(samples[i].heapSize)}");
        }

        report.AppendLine("\n=== Recommendations ===");
        if (memoryDelta > 0)
            report.AppendLine("- Memory is increasing: check for unrelased objects or coroutines");
        if (maxMemory > 300 * 1024 * 1024)
            report.AppendLine("- Peak memory > 300 MB: review texture compression, audio streaming");
        report.AppendLine("- Enable object pooling for frequent spawns (obstacles, collectibles)");
        report.AppendLine("- Monitor GC allocation spikes in Unity Profiler");
        report.AppendLine("- Check for static references keeping objects alive");

        Debug.Log(report.ToString());
    }

    private string FormatBytes(long bytes)
    {
        return $"{bytes / (1024f * 1024f):F1} MB";
    }

    public long GetCurrentMemoryUsage()
    {
        return System.GC.GetTotalMemory(false);
    }

    public long GetPeakMemoryUsage()
    {
        if (samples.Count == 0) return 0;
        long peak = samples[0].heapSize;
        foreach (var sample in samples)
        {
            if (sample.heapSize > peak) peak = sample.heapSize;
        }
        return peak;
    }

    public void ForceSampleNow()
    {
        RecordSample();
    }
}
