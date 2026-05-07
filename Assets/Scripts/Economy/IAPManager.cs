using UnityEngine;
using System;
using System.Collections.Generic;
// SDK import — uncomment when Unity Purchasing package is installed:
// using UnityEngine.Purchasing;
// using UnityEngine.Purchasing.Extension;

/// <summary>
/// Unity IAP integration for Camel Runner.
/// Defines all 9 SKUs at EGP pricing and handles purchase/restore flows.
///
/// SDK setup:
///   1. Add "com.unity.purchasing" to Packages/manifest.json
///   2. Enable In-App Purchasing in Unity Services dashboard
///   3. Uncomment the IStoreListener interface and SDK calls below
///   4. Replace sandbox product IDs in the sku table with your real store IDs
/// </summary>
public class IAPManager : MonoBehaviour  // , IStoreListener
{
    public static IAPManager Instance { get; private set; }

    // ── SKU definitions ───────────────────────────────────────────────────────

    public enum ProductID
    {
        SmallCoinPouch,     // EGP  20  — 1 000  coins
        LargeCoinSack,      // EGP  50  — 3 000  coins
        TreasureChest,      // EGP 150  — 10 000 coins
        HandfulOfGems,      // EGP  30  — 50     gems
        BoxOfGems,          // EGP  80  — 150    gems
        VaultOfGems,        // EGP 250  — 500    gems
        NoAdsPremiumPass,   // EGP 100  — removes all banner/interstitial ads (permanent)
        ExclusiveSkin,      // EGP  50–150 (platform stores list individual SKUs per skin)
        DesertExplorerPass  // EGP  80  — monthly subscription
    }

    [Serializable]
    public class SKU
    {
        public ProductID productId;
        public string storeProductId;   // e.g. "com.aigamefactory.camelrunner.coinpouch_small"
        public string displayName;
        public int priceEGP;
        public string productType;      // "Consumable", "NonConsumable", "Subscription"
        public int coinReward;          // coins granted on purchase (0 if not applicable)
        public int gemReward;           // gems granted on purchase (0 if not applicable)
    }

    public List<SKU> catalog = new List<SKU>
    {
        new SKU { productId = ProductID.SmallCoinPouch,    storeProductId = "com.aigamefactory.camelrunner.coins_small",    displayName = "Small Coin Pouch",           priceEGP = 20,  productType = "Consumable",    coinReward = 1000,  gemReward = 0   },
        new SKU { productId = ProductID.LargeCoinSack,     storeProductId = "com.aigamefactory.camelrunner.coins_large",    displayName = "Large Coin Sack",            priceEGP = 50,  productType = "Consumable",    coinReward = 3000,  gemReward = 0   },
        new SKU { productId = ProductID.TreasureChest,     storeProductId = "com.aigamefactory.camelrunner.coins_chest",    displayName = "Treasure Chest",             priceEGP = 150, productType = "Consumable",    coinReward = 10000, gemReward = 0   },
        new SKU { productId = ProductID.HandfulOfGems,     storeProductId = "com.aigamefactory.camelrunner.gems_handful",   displayName = "Handful of Gems",            priceEGP = 30,  productType = "Consumable",    coinReward = 0,     gemReward = 50  },
        new SKU { productId = ProductID.BoxOfGems,         storeProductId = "com.aigamefactory.camelrunner.gems_box",       displayName = "Box of Gems",                priceEGP = 80,  productType = "Consumable",    coinReward = 0,     gemReward = 150 },
        new SKU { productId = ProductID.VaultOfGems,       storeProductId = "com.aigamefactory.camelrunner.gems_vault",     displayName = "Vault of Gems",              priceEGP = 250, productType = "Consumable",    coinReward = 0,     gemReward = 500 },
        new SKU { productId = ProductID.NoAdsPremiumPass,  storeProductId = "com.aigamefactory.camelrunner.noads",          displayName = "No-Ads Premium Pass",        priceEGP = 100, productType = "NonConsumable", coinReward = 0,     gemReward = 0   },
        new SKU { productId = ProductID.ExclusiveSkin,     storeProductId = "com.aigamefactory.camelrunner.skin_pharaoh",   displayName = "Exclusive Skin – Pharaoh",   priceEGP = 100, productType = "NonConsumable", coinReward = 0,     gemReward = 0   },
        new SKU { productId = ProductID.DesertExplorerPass,storeProductId = "com.aigamefactory.camelrunner.pass_monthly",   displayName = "Desert Explorer Pass",       priceEGP = 80,  productType = "Subscription",  coinReward = 5000,  gemReward = 0   },
    };

    private const string NO_ADS_PREF_KEY = "NoAdsPurchased";
    private const string PASS_ACTIVE_PREF_KEY = "DesertPassActive";

    // Flags checked by AdManager to suppress non-rewarded ads
    public bool IsNoAdsActive => PlayerPrefs.GetInt(NO_ADS_PREF_KEY, 0) == 1;
    public bool IsDesertPassActive => PlayerPrefs.GetInt(PASS_ACTIVE_PREF_KEY, 0) == 1;

    // ── Events ────────────────────────────────────────────────────────────────

    public static event Action<ProductID> OnPurchaseSuccess;
    public static event Action<ProductID, string> OnPurchaseFailed;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitializeIAP();
    }

    private void InitializeIAP()
    {
        Debug.Log("[IAPManager] Initializing Unity Purchasing…");

        // ── SDK init (uncomment when package is installed) ─────────────────
        // var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        // foreach (var sku in catalog)
        // {
        //     ProductType type = sku.productType switch
        //     {
        //         "NonConsumable" => ProductType.NonConsumable,
        //         "Subscription"  => ProductType.Subscription,
        //         _               => ProductType.Consumable,
        //     };
        //     builder.AddProduct(sku.storeProductId, type);
        // }
        // UnityPurchasing.Initialize(this, builder);
        // ──────────────────────────────────────────────────────────────────

        Debug.Log("[IAPManager] (Simulated) IAP initialized with 9 SKUs.");
    }

    // ── IStoreListener callbacks (uncomment with SDK) ─────────────────────────
    // private IStoreController _storeController;
    // private IExtensionProvider _extensionProvider;
    //
    // public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    // {
    //     _storeController = controller;
    //     _extensionProvider = extensions;
    //     Debug.Log("[IAPManager] Store initialized successfully.");
    // }
    //
    // public void OnInitializeFailed(InitializationFailureReason error)
    //     => Debug.LogError($"[IAPManager] Init failed: {error}");
    //
    // public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    // {
    //     var sku = catalog.Find(s => s.storeProductId == args.purchasedProduct.definition.id);
    //     if (sku != null)
    //         GrantReward(sku);
    //     return PurchaseProcessingResult.Complete;
    // }
    //
    // public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    // {
    //     var sku = catalog.Find(s => s.storeProductId == product.definition.id);
    //     OnPurchaseFailed?.Invoke(sku?.productId ?? ProductID.SmallCoinPouch, failureReason.ToString());
    //     Debug.LogError($"[IAPManager] Purchase failed: {product.definition.id} — {failureReason}");
    // }
    // ─────────────────────────────────────────────────────────────────────────

    // ── Public purchase API ───────────────────────────────────────────────────

    public void BuyProduct(ProductID id)
    {
        var sku = catalog.Find(s => s.productId == id);
        if (sku == null) { Debug.LogError($"[IAPManager] Unknown product {id}"); return; }

        Debug.Log($"[IAPManager] Initiating purchase: {sku.displayName} (EGP {sku.priceEGP})");

        // ── SDK call (uncomment with SDK) ──────────────────────────────────
        // if (_storeController != null)
        //     _storeController.InitiatePurchase(sku.storeProductId);
        // ──────────────────────────────────────────────────────────────────

        // Sandbox/editor simulation — grants reward immediately in editor/test builds
#if UNITY_EDITOR
        Debug.Log("[IAPManager] EDITOR: Simulating successful purchase.");
        GrantReward(sku);
#endif
    }

    public void RestorePurchases()
    {
        Debug.Log("[IAPManager] Restoring purchases…");
        // _extensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(result =>
        //     Debug.Log(result ? "Restore OK" : "Restore failed or nothing to restore"));
    }

    // ── Private reward dispatch ───────────────────────────────────────────────

    private void GrantReward(SKU sku)
    {
        switch (sku.productId)
        {
            case ProductID.SmallCoinPouch:
            case ProductID.LargeCoinSack:
            case ProductID.TreasureChest:
                CoinEconomy.Instance.EarnCoins(sku.coinReward);
                break;

            case ProductID.HandfulOfGems:
            case ProductID.BoxOfGems:
            case ProductID.VaultOfGems:
                // Gem wallet (extend CoinEconomy or add GemEconomy as needed)
                Debug.Log($"[IAPManager] Granted {sku.gemReward} gems.");
                break;

            case ProductID.NoAdsPremiumPass:
                PlayerPrefs.SetInt(NO_ADS_PREF_KEY, 1);
                PlayerPrefs.Save();
                Debug.Log("[IAPManager] No-Ads pass activated permanently.");
                break;

            case ProductID.ExclusiveSkin:
                // Map store product ID to skin name for SkinManager
                SkinManager.Instance.UnlockSkinFromIAP(sku.storeProductId);
                break;

            case ProductID.DesertExplorerPass:
                PlayerPrefs.SetInt(PASS_ACTIVE_PREF_KEY, 1);
                PlayerPrefs.Save();
                CoinEconomy.Instance.EarnCoins(sku.coinReward);
                Debug.Log("[IAPManager] Desert Explorer Pass activated for this month.");
                break;
        }

        OnPurchaseSuccess?.Invoke(sku.productId);
        Debug.Log($"[IAPManager] Reward granted for: {sku.displayName}");
    }
}
