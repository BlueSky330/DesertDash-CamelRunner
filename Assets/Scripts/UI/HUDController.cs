using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// HUD overlay controller — displays live score, coins, and health during gameplay.
/// Subscribes to GameManager and HealthSystem events; zero per-frame allocations.
/// </summary>
public class HUDController : MonoBehaviour
{
    [Header("Score / Coins")]
    public TextMeshProUGUI scoreLabel;
    public TextMeshProUGUI coinLabel;

    [Header("Health")]
    public Slider healthSlider;

    void OnEnable()
    {
        GameManager.OnScoreChanged += OnScoreChanged;
        HealthSystem.onHealthChanged += OnHealthChanged;
    }

    void OnDisable()
    {
        GameManager.OnScoreChanged -= OnScoreChanged;
        HealthSystem.onHealthChanged -= OnHealthChanged;
    }

    void Start()
    {
        // Sync initial values
        if (GameManager.Instance != null)
            UpdateScore(GameManager.Instance.score);

        if (HealthSystem.Instance != null)
            UpdateHealth(HealthSystem.Instance.currentHealth);
    }

    private void OnScoreChanged(int score) => UpdateScore(score);
    private void OnHealthChanged(float health) => UpdateHealth(health);

    private void UpdateScore(int score)
    {
        if (scoreLabel != null) scoreLabel.text = score.ToString("N0");
    }

    private void UpdateHealth(float health)
    {
        if (healthSlider != null) healthSlider.value = health / 100f;
    }
}
