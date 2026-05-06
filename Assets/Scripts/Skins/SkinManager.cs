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
        public float weatherProtectionMultiplier = 1f; // e.g., 0.8 means 20% less health decay from weather
        [Range(0f, 1f)]
        public float thiefResistanceMultiplier = 1f; // e.g., 0.7 means thieves steal 30% less
        // Reference to the actual 3D model/sprite for the skin
        public GameObject skinPrefab; 
    }

    public List<CamelSkin> availableSkins = new List<CamelSkin>();
    public CamelSkin equippedSkin { get; private set; }

    public delegate void OnSkinEquipped(string skinName);
    public static event OnSkinEquipped onSkinEquipped;

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
        InitializeSkins();
        // Equip default skin if none is equipped
        if (equippedSkin == null && availableSkins.Count > 0)
        {
            EquipSkin(availableSkins[0].skinName); // Assuming the first skin is the default
        }
    }

    private void InitializeSkins()
    {
        // Base Kamil (default, free)
        availableSkins.Add(new CamelSkin { skinName = "Kamil (Base)", cost = 0, isUnlocked = true, isEquipped = false, weatherProtectionMultiplier = 1f, thiefResistanceMultiplier = 1f });
        // Pharaoh Kamil
        availableSkins.Add(new CamelSkin { skinName = "Pharaoh Kamil", cost = 1000, isUnlocked = false, isEquipped = false, weatherProtectionMultiplier = 0.9f, thiefResistanceMultiplier = 0.9f });
        // Racing Camel
        availableSkins.Add(new CamelSkin { skinName = "Racing Camel", cost = 1500, isUnlocked = false, isEquipped = false, weatherProtectionMultiplier = 1f, thiefResistanceMultiplier = 0.85f });
        // Mummy Camel
        availableSkins.Add(new CamelSkin { skinName = "Mummy Camel", cost = 2000, isUnlocked = false, isEquipped = false, weatherProtectionMultiplier = 0.8f, thiefResistanceMultiplier = 1f });
        // Golden Camel (premium)
        availableSkins.Add(new CamelSkin { skinName = "Golden Camel", cost = 5000, isUnlocked = false, isEquipped = false, weatherProtectionMultiplier = 0.7f, thiefResistanceMultiplier = 0.7f });
    }

    public CamelSkin GetSkin(string skinName)
    {
        return availableSkins.Find(skin => skin.skinName == skinName);
    }

    public bool TryUnlockSkin(string skinName)
    {
        CamelSkin skin = GetSkin(skinName);
        if (skin != null && !skin.isUnlocked)
        {
            if (CollectibleSystem.Instance.SpendCoins(skin.cost))
            {
                skin.isUnlocked = true;
                Debug.Log($"Unlocked {skinName} for {skin.cost} coins.");
                return true;
            }
            else
            {
                Debug.Log($"Not enough coins to unlock {skinName}.");
            }
        }
        return false;
    }

    public void EquipSkin(string skinName)
    {
        CamelSkin newSkin = GetSkin(skinName);
        if (newSkin != null && newSkin.isUnlocked)
        {
            if (equippedSkin != null) equippedSkin.isEquipped = false;
            newSkin.isEquipped = true;
            equippedSkin = newSkin;
            onSkinEquipped?.Invoke(skinName);
            Debug.Log($"Equipped skin: {skinName}");
            // Logic to change player model/appearance would go here
        }
        else
        {
            Debug.Log($"Cannot equip {skinName}. It is either null or not unlocked.");
        }
    }
}
