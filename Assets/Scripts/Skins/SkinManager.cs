using UnityEngine;
using System.Collections.Generic;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance { get; private set; }

    [System.Serializable]
    public class CamelSkin
    {
        public string skinName;
        public int cost;
        public bool isUnlocked;
        public bool isEquipped;
        [Range(0f, 1f)]
        public float weatherProtectionMultiplier = 1f;
        [Range(0f, 1f)]
        public float thiefResistanceMultiplier = 1f;
        public GameObject skinPrefab;

        // Optional store product ID for IAP-unlocked skins (empty = coin-purchase only)
        public string iapStoreProductId;
    }

    public List<CamelSkin> availableSkins = new List<CamelSkin>();
    public CamelSkin equippedSkin { get; private set; }

    public delegate void OnSkinEquipped(string skinName);
    public static event OnSkinEquipped onSkinEquipped;

    // PlayerPrefs key prefixes
    private const string PREF_SKIN_UNLOCKED = "Skin_Unlocked_";
    private const string PREF_SKIN_EQUIPPED  = "Skin_Equipped";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitializeSkins();
        LoadSkinData();

        // Equip persisted selection, or default to first unlocked skin
        string savedEquipped = PlayerPrefs.GetString(PREF_SKIN_EQUIPPED, "");
        CamelSkin toEquip = string.IsNullOrEmpty(savedEquipped)
            ? availableSkins.Find(s => s.isUnlocked)
            : GetSkin(savedEquipped);

        if (toEquip != null && toEquip.isUnlocked)
            EquipSkin(toEquip.skinName);
    }

    private void InitializeSkins()
    {
        availableSkins.Add(new CamelSkin
        {
            skinName = "Camel (Base)", cost = 0, isUnlocked = true,
            weatherProtectionMultiplier = 1f, thiefResistanceMultiplier = 1f
        });
        availableSkins.Add(new CamelSkin
        {
            skinName = "Pharaoh Camel", cost = 1000, isUnlocked = false,
            weatherProtectionMultiplier = 0.9f, thiefResistanceMultiplier = 0.9f,
            iapStoreProductId = "com.aigamefactory.camelrunner.skin_pharaoh"
        });
        availableSkins.Add(new CamelSkin
        {
            skinName = "Racing Camel", cost = 1500, isUnlocked = false,
            weatherProtectionMultiplier = 1f, thiefResistanceMultiplier = 0.85f
        });
        availableSkins.Add(new CamelSkin
        {
            skinName = "Mummy Camel", cost = 2000, isUnlocked = false,
            weatherProtectionMultiplier = 0.8f, thiefResistanceMultiplier = 1f
        });
        availableSkins.Add(new CamelSkin
        {
            skinName = "Golden Camel", cost = 5000, isUnlocked = false,
            weatherProtectionMultiplier = 0.7f, thiefResistanceMultiplier = 0.7f
        });
    }

    // ── Persistence ───────────────────────────────────────────────────────────

    private void LoadSkinData()
    {
        foreach (var skin in availableSkins)
        {
            // Base skin is always unlocked; everything else reads from prefs
            if (skin.cost == 0) { skin.isUnlocked = true; continue; }
            skin.isUnlocked = PlayerPrefs.GetInt(PREF_SKIN_UNLOCKED + skin.skinName, 0) == 1;
        }
    }

    private void SaveSkinUnlocked(string skinName)
    {
        PlayerPrefs.SetInt(PREF_SKIN_UNLOCKED + skinName, 1);
        PlayerPrefs.Save();
    }

    private void SaveEquippedSkin(string skinName)
    {
        PlayerPrefs.SetString(PREF_SKIN_EQUIPPED, skinName);
        PlayerPrefs.Save();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public CamelSkin GetSkin(string skinName)
        => availableSkins.Find(skin => skin.skinName == skinName);

    /// <summary>Purchase a skin with coins via CoinEconomy.</summary>
    public bool TryUnlockSkin(string skinName)
    {
        CamelSkin skin = GetSkin(skinName);
        if (skin == null || skin.isUnlocked) return false;

        if (!CoinEconomy.Instance.TryPurchase(skin.cost, skinName)) return false;

        skin.isUnlocked = true;
        SaveSkinUnlocked(skinName);
        Debug.Log($"[SkinManager] Unlocked '{skinName}' for {skin.cost} coins.");
        return true;
    }

    /// <summary>Called by IAPManager when a skin IAP completes.</summary>
    public void UnlockSkinFromIAP(string storeProductId)
    {
        CamelSkin skin = availableSkins.Find(s => s.iapStoreProductId == storeProductId);
        if (skin == null)
        {
            Debug.LogWarning($"[SkinManager] No skin mapped to IAP product '{storeProductId}'.");
            return;
        }
        skin.isUnlocked = true;
        SaveSkinUnlocked(skin.skinName);
        Debug.Log($"[SkinManager] Unlocked '{skin.skinName}' via IAP.");
    }

    public void EquipSkin(string skinName)
    {
        CamelSkin newSkin = GetSkin(skinName);
        if (newSkin == null || !newSkin.isUnlocked)
        {
            Debug.LogWarning($"[SkinManager] Cannot equip '{skinName}': null or locked.");
            return;
        }
        if (equippedSkin != null) equippedSkin.isEquipped = false;
        newSkin.isEquipped = true;
        equippedSkin = newSkin;
        SaveEquippedSkin(skinName);
        onSkinEquipped?.Invoke(skinName);
        Debug.Log($"[SkinManager] Equipped: {skinName}");
        // Player model swap hook — connect to PlayerController.ApplySkin(skin.skinPrefab)
    }
}
