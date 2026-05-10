using UnityEngine;

/// <summary>
/// Persistent coin wallet. Owns the canonical coin balance stored in PlayerPrefs.
/// CollectibleSystem tracks in-session score; CoinEconomy owns the durable wallet.
/// </summary>
public class CoinEconomy : MonoBehaviour
{
    public static CoinEconomy Instance { get; private set; }

    private const string COINS_PREF_KEY = "PlayerCoins";
    private const int STARTING_COINS = 500;

    public int Coins { get; private set; }

    public delegate void OnCoinsChanged(int newAmount);
    public static event OnCoinsChanged onCoinsChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadCoins();
    }

    // ── Persistence ──────────────────────────────────────────────────────────

    private void LoadCoins()
    {
        Coins = PlayerPrefs.GetInt(COINS_PREF_KEY, STARTING_COINS);
        // Sync to CollectibleSystem after it has initialized (called from Start)
    }

    void Start()
    {
        // Push persisted wallet into CollectibleSystem so both stay in sync
        if (CollectibleSystem.Instance != null)
            CollectibleSystem.Instance.SetCoins(Coins);
        onCoinsChanged?.Invoke(Coins);
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt(COINS_PREF_KEY, Coins);
        PlayerPrefs.Save();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Earn coins (from ad reward, IAP, etc.).</summary>
    public void EarnCoins(int amount)
    {
        if (amount <= 0) return;
        Coins += amount;
        CollectibleSystem.Instance?.SetCoins(Coins);
        SaveCoins();
        onCoinsChanged?.Invoke(Coins);
        Debug.Log($"[CoinEconomy] Earned {amount}. Wallet: {Coins}");
    }

    /// <summary>
    /// Spend coins. Returns true and persists on success; returns false if insufficient.
    /// </summary>
    public bool TrySpend(int amount, string reason = "")
    {
        if (amount <= 0) return true;
        if (Coins < amount)
        {
            Debug.Log($"[CoinEconomy] Insufficient coins for '{reason}'. Have {Coins}, need {amount}.");
            return false;
        }
        Coins -= amount;
        CollectibleSystem.Instance?.SetCoins(Coins);
        SaveCoins();
        onCoinsChanged?.Invoke(Coins);
        Debug.Log($"[CoinEconomy] Spent {amount} on '{reason}'. Wallet: {Coins}");
        return true;
    }

    /// <summary>Generic purchase helper — logs item name on success.</summary>
    public bool TryPurchase(int cost, string itemName)
    {
        bool ok = TrySpend(cost, itemName);
        if (ok) Debug.Log($"[CoinEconomy] Purchased '{itemName}' for {cost} coins.");
        return ok;
    }

    /// <summary>Called by AdManager after a rewarded ad completes 100%.</summary>
    public void EarnCoinsFromAd(int amount) => EarnCoins(amount);

    /// <summary>Flush end-of-run score conversion into wallet.</summary>
    public void ConvertScoreToCoins()
    {
        if (CollectibleSystem.Instance == null) return;
        int before = Coins;
        CollectibleSystem.Instance.ConvertScoreToCoins();
        // CollectibleSystem.AddCoins calls SetCoins which updates Coins via sync
        // Sync back from CollectibleSystem after conversion
        Coins = CollectibleSystem.Instance.CurrentCoins;
        SaveCoins();
        onCoinsChanged?.Invoke(Coins);
        Debug.Log($"[CoinEconomy] Score conversion: {before} → {Coins} coins.");
    }

    /// <summary>Called by CollectibleSystem when its coin value changes externally.</summary>
    public void SyncFromCollectibleSystem(int newCoins)
    {
        if (Coins == newCoins) return;
        Coins = newCoins;
        SaveCoins();
        onCoinsChanged?.Invoke(Coins);
    }
}
