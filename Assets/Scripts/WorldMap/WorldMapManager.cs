using UnityEngine;
using System.Collections.Generic;

public class WorldMapManager : MonoBehaviour
{
    public static WorldMapManager Instance { get; private set; }

    [System.Serializable]
    public class CountryData
    {
        public string countryName;
        public string landmarkTheme;
        public string environment;
        public int unlockCost;
        public bool isUnlocked;
    }

    public List<CountryData> allCountries = new List<CountryData>();
    public CountryData currentCountry { get; private set; }

    public delegate void OnCountryUnlocked(string countryName);
    public static event OnCountryUnlocked onCountryUnlocked;

    public delegate void OnCurrentCountryChanged(string countryName);
    public static event OnCurrentCountryChanged onCurrentCountryChanged;

    private const string PREF_COUNTRY_UNLOCKED = "Country_Unlocked_";
    private const string PREF_CURRENT_COUNTRY   = "Country_Current";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitializeCountries();
        LoadUnlockState();

        string saved = PlayerPrefs.GetString(PREF_CURRENT_COUNTRY, "Egypt");
        CountryData startCountry = GetCountryData(saved);
        if (startCountry == null || !startCountry.isUnlocked)
            startCountry = GetCountryData("Egypt");

        SetCurrentCountry(startCountry.countryName);
    }

    private void InitializeCountries()
    {
        allCountries.Add(new CountryData { countryName = "Egypt",      landmarkTheme = "Pyramids of Giza & Sphinx",        environment = "Desert, sand dunes, Nile",                unlockCost = 0,    isUnlocked = true  });
        allCountries.Add(new CountryData { countryName = "Jordan",     landmarkTheme = "Petra (Treasury)",                 environment = "Rocky desert, canyons",                   unlockCost = 500,  isUnlocked = false });
        allCountries.Add(new CountryData { countryName = "India",      landmarkTheme = "Taj Mahal",                        environment = "Colorful streets, temples, jungle",        unlockCost = 800,  isUnlocked = false });
        allCountries.Add(new CountryData { countryName = "China",      landmarkTheme = "Great Wall of China",              environment = "Mountains, bamboo forests, pagodas",       unlockCost = 1200, isUnlocked = false });
        allCountries.Add(new CountryData { countryName = "Italy",      landmarkTheme = "Colosseum (Rome)",                 environment = "Cobblestone streets, vineyards, ruins",    unlockCost = 1500, isUnlocked = false });
        allCountries.Add(new CountryData { countryName = "Peru",       landmarkTheme = "Machu Picchu",                     environment = "Mountains, jungle, Inca ruins",            unlockCost = 2000, isUnlocked = false });
        allCountries.Add(new CountryData { countryName = "France",     landmarkTheme = "Eiffel Tower (Paris)",             environment = "City streets, gardens, cafes",             unlockCost = 2500, isUnlocked = false });
        allCountries.Add(new CountryData { countryName = "UAE (Dubai)",landmarkTheme = "Burj Khalifa",                     environment = "Modern city, glass towers, desert",        unlockCost = 3000, isUnlocked = false });
        allCountries.Add(new CountryData { countryName = "Brazil",     landmarkTheme = "Christ the Redeemer (Rio)",        environment = "Jungle, beaches, carnival vibes",          unlockCost = 3500, isUnlocked = false });
        allCountries.Add(new CountryData { countryName = "USA",        landmarkTheme = "Statue of Liberty (New York)",     environment = "Skyscrapers, Central Park, highways",      unlockCost = 5000, isUnlocked = false });
    }

    // ── Persistence ───────────────────────────────────────────────────────────

    private void LoadUnlockState()
    {
        foreach (var country in allCountries)
        {
            if (country.unlockCost == 0) { country.isUnlocked = true; continue; }
            country.isUnlocked = PlayerPrefs.GetInt(PREF_COUNTRY_UNLOCKED + country.countryName, 0) == 1;
        }
    }

    private void SaveCountryUnlocked(string countryName)
    {
        PlayerPrefs.SetInt(PREF_COUNTRY_UNLOCKED + countryName, 1);
        PlayerPrefs.Save();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public CountryData GetCountryData(string countryName)
        => allCountries.Find(c => c.countryName == countryName);

    public bool IsCountryUnlocked(string countryName)
    {
        CountryData c = GetCountryData(countryName);
        return c != null && c.isUnlocked;
    }

    /// <summary>Spend coins to unlock a country; persists unlock state.</summary>
    public bool TryUnlockCountry(string countryName)
    {
        CountryData country = GetCountryData(countryName);
        if (country == null || country.isUnlocked) return false;

        if (!CoinEconomy.Instance.TrySpend(country.unlockCost, $"Unlock {countryName}"))
        {
            Debug.Log($"[WorldMapManager] Not enough coins to unlock {countryName}.");
            return false;
        }

        country.isUnlocked = true;
        SaveCountryUnlocked(countryName);
        onCountryUnlocked?.Invoke(countryName);
        Debug.Log($"[WorldMapManager] Unlocked {countryName} for {country.unlockCost} coins.");
        return true;
    }

    public void SetCurrentCountry(string countryName)
    {
        CountryData country = GetCountryData(countryName);
        if (country == null || !country.isUnlocked)
        {
            Debug.LogError($"[WorldMapManager] Cannot set current country to '{countryName}': null or locked.");
            return;
        }
        currentCountry = country;
        PlayerPrefs.SetString(PREF_CURRENT_COUNTRY, countryName);
        PlayerPrefs.Save();
        onCurrentCountryChanged?.Invoke(countryName);
        Debug.Log($"[WorldMapManager] Current country: {countryName}");
    }

    public void UpdateMiniMapProgress(float progress)
    {
        // UIManager.Instance.UpdateMiniMap(currentCountry.countryName, progress);
        Debug.Log($"[WorldMapManager] Mini-map {currentCountry.countryName}: {progress * 100:F0}%");
    }
}
