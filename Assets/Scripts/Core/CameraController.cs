using UnityEngine;

/// <summary>
/// Fixed 3rd-person rear-follow camera rig for Camel Runner.
/// Sits slightly above and behind the camel with a fixed offset — no orbit.
/// Position is SmoothDamped to avoid snappy jumps during lane changes.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // Player Transform; auto-assigned if left null

    [Header("Offset (relative to target)")]
    public Vector3 offset = new Vector3(0f, 3.5f, -6f); // behind + above

    [Header("Smoothing")]
    [Tooltip("Lower = snappier. 0.1–0.3 works well for a runner.")]
    public float smoothTime = 0.15f;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
            else Debug.LogWarning("[CameraController] No target set and no 'Player' tag found.");
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + target.rotation * offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothTime);

        // Always look at the player (slightly above feet)
        transform.LookAt(target.position + Vector3.up * 1.2f);
    }
}
