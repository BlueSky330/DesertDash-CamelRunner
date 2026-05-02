using UnityEngine;
using System.Collections;

public class VisualEffects : MonoBehaviour
{
    public static VisualEffects Instance { get; private set; }

    [Header("Particle Systems")]
    public ParticleSystem sandParticles;
    public ParticleSystem collectibleSparkleParticles;
    public ParticleSystem powerUpGlowParticles;
    public ParticleSystem milestoneCelebrationParticles;

    [Header("Screen Effects")]
    public GameObject lowHealthOverlay; // UI Image or Panel
    public float lowHealthPulseSpeed = 1f;

    [Header("Camera Effects")]
    public Camera mainCamera;
    public float thiefZoomDuration = 0.5f;
    public float thiefZoomAmount = 0.8f; // How much to zoom in (e.g., 0.8 means 80% of original FOV)

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        // Initialize particle systems (e.g., stop them if they are set to play on awake)
        if (sandParticles != null) sandParticles.Stop();
        if (collectibleSparkleParticles != null) collectibleSparkleParticles.Stop();
        if (powerUpGlowParticles != null) powerUpGlowParticles.Stop();
        if (milestoneCelebrationParticles != null) milestoneCelebrationParticles.Stop();

        if (lowHealthOverlay != null) lowHealthOverlay.SetActive(false);
    }

    void Update()
    {
        // Low health pulse effect
        if (lowHealthOverlay != null && lowHealthOverlay.activeSelf)
        {
            // Example: pulse alpha of an image
            Image overlayImage = lowHealthOverlay.GetComponent<Image>();
            if (overlayImage != null)
            {
                Color currentColor = overlayImage.color;
                currentColor.a = Mathf.Lerp(0.2f, 0.5f, Mathf.PingPong(Time.time * lowHealthPulseSpeed, 1f));
                overlayImage.color = currentColor;
            }
        }
    }

    // Particle Effects
    public void PlaySandParticles(Vector3 position) { if (sandParticles != null) { sandParticles.transform.position = position; sandParticles.Play(); } }
    public void PlayCollectibleSparkle(Vector3 position) { if (collectibleSparkleParticles != null) { collectibleSparkleParticles.transform.position = position; collectibleSparkleParticles.Play(); } }
    public void PlayPowerUpGlow(Vector3 position) { if (powerUpGlowParticles != null) { powerUpGlowParticles.transform.position = position; powerUpGlowParticles.Play(); } }
    public void PlayMilestoneCelebration(Vector3 position) { if (milestoneCelebrationParticles != null) { milestoneCelebrationParticles.transform.position = position; milestoneCelebrationParticles.Play(); } }

    // Screen Effects
    public void ShowLowHealthWarning(bool show)
    {
        if (lowHealthOverlay != null) lowHealthOverlay.SetActive(show);
    }

    // Camera Effects
    public void TriggerThiefEncounterZoom()
    {
        if (mainCamera != null) StartCoroutine(ThiefZoomRoutine());
    }

    private IEnumerator ThiefZoomRoutine()
    {
        float originalFOV = mainCamera.fieldOfView;
        float targetFOV = originalFOV * thiefZoomAmount;
        float timer = 0f;

        while (timer < thiefZoomDuration)
        {
            mainCamera.fieldOfView = Mathf.Lerp(originalFOV, targetFOV, timer / thiefZoomDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        mainCamera.fieldOfView = targetFOV; // Ensure it reaches target

        // Optional: hold zoom for a moment, then zoom out
        yield return new WaitForSeconds(0.5f);

        timer = 0f;
        while (timer < thiefZoomDuration)
        {
            mainCamera.fieldOfView = Mathf.Lerp(targetFOV, originalFOV, timer / thiefZoomDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        mainCamera.fieldOfView = originalFOV; // Ensure it returns to original
    }

    // Country Transition Animation (to be integrated with UIManager/WorldMapManager)
    public void PlayCountryTransitionEffect()
    {
        Debug.Log("Playing country transition animation.");
        // Example: Fade to black, then fade in new environment, or a map animation
    }
}
