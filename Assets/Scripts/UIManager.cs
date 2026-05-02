using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Main Menu UI")]
    public GameObject mainMenuPanel;
    public Button playButton;
    public Button shopButton;
    public Button charactersButton;
    public Button settingsButton;

    [Header("Gameplay HUD")]
    public GameObject gameplayHUDPanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinsText;
    public Slider healthBar;
    public Image miniMapImage; // Placeholder for mini-map display
    public GameObject activePowerUpContainer; // To display active power-up icons

    [Header("Watch & Earn UI")]
    public GameObject watchAndEarnPanel;
    public Button quickAdButton;
    public Button standardAdButton;
    public Button premiumAdButton;

    [Header("Shop UI")]
    public GameObject shopPanel;
    public Button shopSkinsButton;
    public Button shopPowerUpsButton;
    public Button shopCoinPacksButton;

    [Header("World Map UI")]
    public GameObject worldMapPanel;
    public GameObject countrySelectionContainer; // Container for country buttons/visuals

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalCoinsText;
    public Button restartMilestoneButton;
    public Button continueAdButton;

    [Header("Offline Mode UI")]
    public GameObject offlineWarningPanel;
    public TextMeshProUGUI offlineWarningText;

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
        // Subscribe to events
        CollectibleSystem.onScoreChanged += UpdateScoreDisplay;
        CollectibleSystem.onCoinsChanged += UpdateCoinsDisplay;
        HealthSystem.onHealthChanged += UpdateHealthBar;
        HealthSystem.onGameOver += ShowGameOverScreen;
        // AdManager.onAdRewardGranted += OnAdRewardGranted; // Example
        // AdManager.onAdFailed += OnAdFailed; // Example
        // WorldMapManager.onCurrentCountryChanged += UpdateMiniMapDisplay; // Example

        // Initial UI state
        ShowMainMenu();
        UpdateScoreDisplay(CollectibleSystem.Instance.currentScore);
        UpdateCoinsDisplay(CollectibleSystem.Instance.currentCoins);
        UpdateHealthBar(HealthSystem.Instance.currentHealth);
    }

    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        CollectibleSystem.onScoreChanged -= UpdateScoreDisplay;
        CollectibleSystem.onCoinsChanged -= UpdateCoinsDisplay;
        HealthSystem.onHealthChanged -= UpdateHealthBar;
        HealthSystem.onGameOver -= ShowGameOverScreen;
    }

    public void ShowMainMenu()
    {
        HideAllPanels();
        mainMenuPanel.SetActive(true);
    }

    public void ShowGameplayHUD()
    {
        HideAllPanels();
        gameplayHUDPanel.SetActive(true);
    }

    public void ShowWatchAndEarnMenu()
    {
        HideAllPanels();
        watchAndEarnPanel.SetActive(true);
    }

    public void ShowShop()
    {
        HideAllPanels();
        shopPanel.SetActive(true);
    }

    public void ShowWorldMap()
    {
        HideAllPanels();
        worldMapPanel.SetActive(true);
        PopulateWorldMap();
    }

    public void ShowGameOverScreen()
    {
        HideAllPanels();
        gameOverPanel.SetActive(true);
        finalScoreText.text = "Score: " + CollectibleSystem.Instance.currentScore.ToString();
        finalCoinsText.text = "Coins: " + CollectibleSystem.Instance.currentCoins.ToString();
    }

    public void ShowOfflineWarning(bool show, string message = "Go online to earn more coins!")
    {
        offlineWarningPanel.SetActive(show);
        offlineWarningText.text = message;
    }

    private void HideAllPanels()
    {
        mainMenuPanel.SetActive(false);
        gameplayHUDPanel.SetActive(false);
        watchAndEarnPanel.SetActive(false);
        shopPanel.SetActive(false);
        worldMapPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        offlineWarningPanel.SetActive(false);
    }

    private void UpdateScoreDisplay(int score)
    {
        if (scoreText != null) scoreText.text = score.ToString();
    }

    private void UpdateCoinsDisplay(int coins)
    {
        if (coinsText != null) coinsText.text = coins.ToString();
    }

    private void UpdateHealthBar(float health)
    {
        if (healthBar != null) healthBar.value = health / 100f; // Assuming health is 0-100
        // Also handle visual warning for low health here
        if (health <= 25f)
        {
            // Trigger visual effect on screen edges or health bar itself
        }
    }

    private void PopulateWorldMap()
    {
        // Clear existing country buttons/visuals
        foreach (Transform child in countrySelectionContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Create buttons/visuals for each country
        foreach (var country in WorldMapManager.Instance.allCountries)
        {
            GameObject countryButtonGO = new GameObject(country.countryName + "Button");
            countryButtonGO.transform.SetParent(countrySelectionContainer.transform);
            Button countryButton = countryButtonGO.AddComponent<Button>();
            TextMeshProUGUI buttonText = countryButtonGO.AddComponent<TextMeshProUGUI>();
            buttonText.text = country.countryName + (country.isUnlocked ? " (Unlocked)" : $" ({country.unlockCost} Coins)");
            buttonText.color = country.isUnlocked ? Color.green : Color.red;
            buttonText.fontSize = 24;
            buttonText.alignment = TextAlignmentOptions.Center;

            countryButton.onClick.AddListener(() => {
                if (country.isUnlocked)
                {
                    WorldMapManager.Instance.SetCurrentCountry(country.countryName);
                    ShowGameplayHUD(); // Or go to a level start screen
                }
                else
                {
                    if (WorldMapManager.Instance.TryUnlockCountry(country.countryName))
                    {
                        // Successfully unlocked, update UI
                        PopulateWorldMap();
                    }
                    else
                    {
                        // Not enough coins, show prompt
                        ShowOfflineWarning(true, $"Not enough coins to unlock {country.countryName}. Watch ads to earn more!");
                    }
                }
            });
        }
    }

    // Placeholder for mini-map update logic
    public void UpdateMiniMapDisplay(string countryName, float progress)
    {
        // Update miniMapImage based on countryName and progress
        Debug.Log($"Updating mini-map for {countryName} with progress {progress}");
    }
}
