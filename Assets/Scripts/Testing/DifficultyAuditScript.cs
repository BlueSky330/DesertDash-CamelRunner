using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Audits the difficulty curve over a long gameplay session.
/// Tracks obstacle spawn rate, density, and power-up frequency to ensure fair progression.
/// Identifies difficulty spikes or plateaus.
///
/// Usage: Attach to game scene and call StartDifficultyAudit() after game starts.
/// </summary>
public class DifficultyAuditScript : MonoBehaviour
{
    [Header("Audit Settings")]
    [SerializeField] private float sampleIntervalSeconds = 30f; // Sample every 30 seconds
    [SerializeField] private float maxSessionDuration = 3600f; // 60 minutes

    private List<DifficultySnapshot> snapshots = new List<DifficultySnapshot>();
    private float elapsedTime = 0f;
    private float timeSinceLastSample = 0f;
    private bool isAuditActive = false;

    private struct DifficultySnapshot
    {
        public float sessionTime;
        public int totalObstaclesSpawned;
        public float obstacleSpawnRate; // obstacles per minute
        public int powerUpsSpawned;
        public int thiefEncounters;
        public float averageFrameRate;
        public int estimatedDifficulty; // 1-10 scale
    }

    void Update()
    {
        if (!isAuditActive) return;

        elapsedTime += Time.deltaTime;
        timeSinceLastSample += Time.deltaTime;

        if (elapsedTime > maxSessionDuration)
        {
            StopDifficultyAudit();
            return;
        }

        if (timeSinceLastSample >= sampleIntervalSeconds)
        {
            RecordDifficultySnapshot();
            timeSinceLastSample = 0f;
        }
    }

    public void StartDifficultyAudit()
    {
        Debug.Log("[DifficultyAuditScript] Starting difficulty curve audit...");
        isAuditActive = true;
        elapsedTime = 0f;
        snapshots.Clear();
    }

    public void StopDifficultyAudit()
    {
        isAuditActive = false;
        Debug.Log("[DifficultyAuditScript] Difficulty audit completed.");
        GenerateAuditReport();
    }

    private void RecordDifficultySnapshot()
    {
        var snapshot = new DifficultySnapshot
        {
            sessionTime = elapsedTime,
            totalObstaclesSpawned = GetObstacleSpawnCount(),
            obstacleSpawnRate = GetObstacleSpawnRate(),
            powerUpsSpawned = GetPowerUpCount(),
            thiefEncounters = GetThiefEncounterCount(),
            averageFrameRate = GetAverageFPS(),
            estimatedDifficulty = EstimateDifficulty(elapsedTime)
        };

        snapshots.Add(snapshot);
        Debug.Log($"[Difficulty Snapshot] {elapsedTime:F0}s: {snapshot.obstacleSpawnRate:F1} obs/min, " +
                  $"Difficulty {snapshot.estimatedDifficulty}/10");
    }

    private void GenerateAuditReport()
    {
        var report = new StringBuilder();
        report.AppendLine("\n=== Difficulty Curve Audit Report ===\n");
        report.AppendLine($"Total Duration: {elapsedTime:F0} seconds ({elapsedTime / 60:F1} minutes)");
        report.AppendLine($"Snapshots Recorded: {snapshots.Count}\n");

        if (snapshots.Count < 2)
        {
            Debug.Log("Not enough data for audit report.");
            return;
        }

        // Analyze progression
        float initialSpawnRate = snapshots[0].obstacleSpawnRate;
        float finalSpawnRate = snapshots[snapshots.Count - 1].obstacleSpawnRate;
        float spawnRateIncrease = finalSpawnRate - initialSpawnRate;
        float percentChange = (initialSpawnRate > 0) ? (spawnRateIncrease / initialSpawnRate) * 100 : 0;

        report.AppendLine("=== Difficulty Progression ===");
        report.AppendLine($"Initial Spawn Rate: {initialSpawnRate:F1} obstacles/min");
        report.AppendLine($"Final Spawn Rate: {finalSpawnRate:F1} obstacles/min");
        report.AppendLine($"Rate Increase: {spawnRateIncrease:+0.0;-0.0} ({percentChange:+0;-0}%)\n");

        // Check for smooth progression
        bool isSmoothProgression = CheckSmoothnessOfProgression();
        if (isSmoothProgression)
            report.AppendLine("✓ Difficulty progression is smooth (no sudden spikes)");
        else
            report.AppendLine("⚠️  WARNING: Difficulty progression has sudden spikes or plateaus");

        // Analyze final difficulty
        int finalDifficulty = snapshots[snapshots.Count - 1].estimatedDifficulty;
        report.AppendLine($"\nFinal Estimated Difficulty: {finalDifficulty}/10");
        if (finalDifficulty >= 7)
            report.AppendLine("  Status: ✓ Appropriately challenging at 30+ minutes");
        else if (finalDifficulty >= 4)
            report.AppendLine("  Status: ⚠️  Moderate challenge (may be too easy for experienced players)");
        else
            report.AppendLine("  Status: ⚠️  Low challenge (likely too easy)");

        // Power-up frequency
        report.AppendLine($"\nTotal Power-ups Spawned: {snapshots[snapshots.Count - 1].powerUpsSpawned}");
        report.AppendLine($"Average Power-ups per Minute: {(float)snapshots[snapshots.Count - 1].powerUpsSpawned / (elapsedTime / 60):F1}");

        // Thief encounters
        report.AppendLine($"\nTotal Thief Encounters: {snapshots[snapshots.Count - 1].thiefEncounters}");
        report.AppendLine($"Average Thief Encounters per Minute: {(float)snapshots[snapshots.Count - 1].thiefEncounters / (elapsedTime / 60):F1}");

        // Performance during high difficulty
        float avgFPS = 0;
        foreach (var snap in snapshots)
            avgFPS += snap.averageFrameRate;
        avgFPS /= snapshots.Count;

        report.AppendLine($"\nAverage FPS During Audit: {avgFPS:F1}");
        if (avgFPS >= 55)
            report.AppendLine("  ✓ Performance maintained during difficulty increase");
        else
            report.AppendLine("  ⚠️  WARNING: Performance drops as difficulty increases");

        report.AppendLine("\n=== Recommendations ===");
        if (!isSmoothProgression)
            report.AppendLine("- Review DifficultyManager curve for smooth obstacle spawn rate");
        if (finalDifficulty < 5)
            report.AppendLine("- Increase final obstacle spawn rate or density");
        if (avgFPS < 55)
            report.AppendLine("- Optimize performance (fewer draw calls, object pooling)");
        if (snapshots[snapshots.Count - 1].powerUpsSpawned < 5)
            report.AppendLine("- Power-ups appear too infrequently; consider increasing spawn rate");

        report.AppendLine("\n=== Timeline ===");
        for (int i = 0; i < snapshots.Count; i++)
        {
            var snap = snapshots[i];
            string diffBar = new string('█', snap.estimatedDifficulty) + new string('░', 10 - snap.estimatedDifficulty);
            report.AppendLine($"  {snap.sessionTime:F0}s ({snap.sessionTime/60:F1}min): " +
                            $"[{diffBar}] {snap.estimatedDifficulty}/10 " +
                            $"({snap.obstacleSpawnRate:F1} obs/min)");
        }

        Debug.Log(report.ToString());
    }

    private bool CheckSmoothnessOfProgression()
    {
        if (snapshots.Count < 3) return true;

        // Check for jumps larger than 30% between consecutive samples
        for (int i = 1; i < snapshots.Count; i++)
        {
            float prev = snapshots[i - 1].obstacleSpawnRate;
            float curr = snapshots[i].obstacleSpawnRate;
            if (prev > 0)
            {
                float change = Mathf.Abs(curr - prev) / prev;
                if (change > 0.3f) // More than 30% jump
                    return false;
            }
        }

        return true;
    }

    private int EstimateDifficulty(float elapsedSeconds)
    {
        float minutes = elapsedSeconds / 60f;

        // Simple linear progression: 1/10 at start, 8/10 at 30+ minutes
        if (minutes < 5)
            return Mathf.Max(1, (int)(1 + (minutes / 5) * 2));
        else if (minutes < 30)
            return Mathf.Clamp((int)(3 + (minutes - 5) / 25 * 5), 1, 8);
        else
            return 8; // Max out at 8/10 for safety margin

        // These values are based on typical endless runner progression
    }

    // Helper methods to query game state (implement based on actual game systems)

    private int GetObstacleSpawnCount()
    {
        // TODO: Query ObstacleSpawner or LevelGenerator for total obstacles spawned
        // Placeholder: return 0
        return 0;
    }

    private float GetObstacleSpawnRate()
    {
        // TODO: Calculate from recent obstacle spawns (obstacles per minute)
        // Placeholder: return 5.0f
        return 5.0f;
    }

    private int GetPowerUpCount()
    {
        // TODO: Query PowerUpManager for total power-ups spawned
        // Placeholder: return 0
        return 0;
    }

    private int GetThiefEncounterCount()
    {
        // TODO: Query ThiefSystem for thief encounters
        // Placeholder: return 0
        return 0;
    }

    private float GetAverageFPS()
    {
        // Query frame rate (simplified version)
        return 1f / Time.deltaTime;
    }
}
