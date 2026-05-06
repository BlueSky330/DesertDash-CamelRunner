using UnityEngine;

public class CoinEconomy : MonoBehaviour
{
    public static CoinEconomy Instance { get; private set; }

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

    // Generic purchase method for any item
    public bool TryPurchase(int cost, string itemName)
    {
        if (CollectibleSystem.Instance.SpendCoins(cost))
        {
            Debug.Log($"Successfully purchased {itemName} for {cost} coins.");
            return true;
        }
        else
        {
            Debug.Log($"Not enough coins to purchase {itemName}. Cost: {cost}, Current: {CollectibleSystem.Instance.currentCoins}");
            // UIManager.Instance.ShowNotEnoughCoinsPrompt(); // Placeholder for UI interaction
            return false;
        }
    }

    // Method to handle coin earning from ads (called by AdManager)
    public void EarnCoinsFromAd(int amount)
    {
        CollectibleSystem.Instance.AddCoins(amount);
        Debug.Log($"Earned {amount} coins from ad. Total: {CollectibleSystem.Instance.currentCoins}");
    }

    // Method to handle score to coin conversion (called by UIManager/CollectibleSystem)
    public void ConvertScoreToCoins()
    {
        CollectibleSystem.Instance.ConvertScoreToCoins();
    }

    // Placeholder for saving/loading coin data (e.g., using PlayerPrefs or a more robust save system)
    public void SaveCoinData()
    {
        PlayerPrefs.SetInt("PlayerCoins", CollectibleSystem.Instance.currentCoins);
        PlayerPrefs.Save();
        Debug.Log("Coin data saved.");
    }

    public void LoadCoinData()
    {
        if (PlayerPrefs.HasKey("PlayerCoins"))
        {
            // This would ideally set the CollectibleSystem.Instance.currentCoins directly
            // For now, we just log it.
            int loadedCoins = PlayerPrefs.GetInt("PlayerCoins");
            Debug.Log($"Loaded {loadedCoins} coins.");
            // CollectibleSystem.Instance.currentCoins = loadedCoins; // Needs a public setter or a method in CollectibleSystem
        }
        else
        {
            Debug.Log("No saved coin data found.");
        }
    }
}
