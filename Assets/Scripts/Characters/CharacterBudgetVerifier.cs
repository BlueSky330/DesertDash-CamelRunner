using UnityEngine;

/// <summary>
/// Verifies that a character's mesh stays within the 2 000-triangle performance budget
/// required for 60 fps on mid-range Android devices.
///
/// Usage (runtime):
///   Attach to any character root.  Verification runs automatically on Start().
///   Call CharacterBudgetVerifier.Verify(go, label) from any code (e.g., ThiefSpawner)
///   to check a freshly-built runtime object.
///
/// Usage (static):
///   int tris = CharacterBudgetVerifier.CountTriangles(gameObject);
///   bool ok  = CharacterBudgetVerifier.IsWithinBudget(gameObject);
/// </summary>
[DisallowMultipleComponent]
public sealed class CharacterBudgetVerifier : MonoBehaviour
{
    // ── Budget constant ────────────────────────────────────────────────────────

    public const int MAX_TRIS = 2_000;

    // ── Reported values (visible in Inspector) ─────────────────────────────────

    [Header("Budget Report (auto-filled at runtime)")]
    [SerializeField] private int  _triangleCount;
    [SerializeField] private bool _withinBudget;

    // ── Unity lifecycle ────────────────────────────────────────────────────────

    private void Start() => RunVerification();

    // ── Instance API ───────────────────────────────────────────────────────────

    /// <summary>Run budget check on this GameObject and log the result.</summary>
    public void RunVerification()
    {
        _triangleCount = CountTriangles(gameObject);
        _withinBudget  = _triangleCount <= MAX_TRIS;
        LogResult(name, _triangleCount, _withinBudget);
    }

    // ── Static API ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Verify a character's triangle budget and log the result.
    /// Returns true if within budget.
    /// </summary>
    public static bool Verify(GameObject target, string label = null)
    {
        int  tris  = CountTriangles(target);
        bool pass  = tris <= MAX_TRIS;
        LogResult(label ?? target.name, tris, pass);
        return pass;
    }

    /// <summary>
    /// Count total triangles across all MeshFilter and SkinnedMeshRenderer
    /// components in the target's hierarchy.
    /// </summary>
    public static int CountTriangles(GameObject target)
    {
        int total = 0;

        foreach (var mf in target.GetComponentsInChildren<MeshFilter>())
        {
            // sharedMesh avoids material instance alloc; safe for read-only count
            Mesh mesh = Application.isPlaying ? mf.mesh : mf.sharedMesh;
            if (mesh != null)
                total += mesh.triangles.Length / 3;
        }

        foreach (var smr in target.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            Mesh mesh = Application.isPlaying ? smr.sharedMesh : smr.sharedMesh;
            if (mesh != null)
                total += mesh.triangles.Length / 3;
        }

        return total;
    }

    /// <summary>Returns true if the target's total triangle count is within budget.</summary>
    public static bool IsWithinBudget(GameObject target)
        => CountTriangles(target) <= MAX_TRIS;

    // ── Private helpers ────────────────────────────────────────────────────────

    private static void LogResult(string label, int tris, bool pass)
    {
        if (pass)
            Debug.Log($"[BudgetVerifier] PASS  '{label}': {tris}/{MAX_TRIS} tris");
        else
            Debug.LogError($"[BudgetVerifier] FAIL  '{label}': {tris} tris — EXCEEDS {MAX_TRIS} tri budget!");
    }
}
