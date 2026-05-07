using UnityEngine;

/// <summary>
/// Scrolls a background prop at a fraction of the road speed to create a parallax depth illusion.
///
/// Attach to each background landmark root (pyramids, sphinx, obelisks).
/// When the prop scrolls behind the cull threshold it wraps back to resetZ so the
/// horizon never runs dry during an endless run.
///
/// parallaxFactor guide:
///   0.08–0.12  far background (large pyramids on the horizon)
///   0.15–0.20  mid background (obelisks, sphinx)
///   0.25–0.35  near background (palm trees, dunes)
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    [Range(0f, 1f)]
    [Tooltip("Fraction of game scroll speed applied to this object. Lower = more distant.")]
    public float parallaxFactor = 0.15f;

    [Tooltip("Z position behind the camera at which the object wraps forward.")]
    public float cullZ = -30f;

    [Tooltip("Z position to reset to when wrapping. Should be well ahead of the player spawn point.")]
    public float resetZ = 200f;

    // Fallback speed when DifficultyManager is absent (matches LevelGenerator default)
    private const float FallbackSpeed = 10f;

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameManager.GameState.Running)
            return;

        float speed = GetScrollSpeed() * parallaxFactor;
        transform.Translate(0f, 0f, -speed * Time.deltaTime, Space.World);

        // Wrap: keep the landmark visible indefinitely
        if (transform.position.z < cullZ)
        {
            Vector3 p = transform.position;
            p.z = resetZ;
            transform.position = p;
        }
    }

    private float GetScrollSpeed()
    {
        if (DifficultyManager.Instance != null)
            return DifficultyManager.Instance.GetCurrentSpeed();

        // Honor Magic Carpet speed boost even without DifficultyManager
        float mult = (PowerUpManager.Instance != null)
            ? PowerUpManager.Instance.SpeedMultiplier()
            : 1f;

        return FallbackSpeed * mult;
    }
}
