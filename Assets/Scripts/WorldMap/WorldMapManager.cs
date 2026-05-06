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
        InitializeCountries();
        // Set initial country to Egypt
        SetCurrentCountry("Egypt");
    }

    private void InitializeCountries()
    {
        allCountries.Add(new CountryData { countryName = "Egypt", landmarkTheme = "Pyramids of Giza & Sphinx", environment = "Desert, sand dunes, Nile", unlockCost = 0, isUnlocked = true });
        allCountries.Add(new CountryData { countryName = "Jordan", landmarkTheme = "Petra (Treasury)", environment = "Rocky desert, canyons", unlockCost = 500, isUnlocked = false });
        allCountries.Add(new CountryData { countryName = "India", landmarkTheme = "Taj Mahal", environment = "Colorful streets, temples, jungle", unlockCost = 800, isUnlocked = false });
        allCountries.Add(new CountryData { countryName = "China", landmarkTheme = "Great Wall of China", environment = "Mountains, bamboo forests, pagodas", unlockCost = 1200, isUnlocked = false });
        allCountries.Add(new CountryData { countryName = "Italy", landmarkTheme = "Colosseum (Rome)", environment = "Cobblestone streets, vineyards, ruins", unlockCost = 1500, isUnlocked = false });
        allCountries.Add(new CountryData { countryName = "Peru", landmarkTheme = "Machu Picchu", environment = "Mountains, jungle, Inca ruins", unlockCost = 2000, isUnlocked = false });
        allCountries.Add(new CountryData { countryName = "France", landmarkTheme = "Eiffel Tower (Paris)", environment = "City streets, gardens, cafes", unlockCost = 2500, isUnlocked = false });
        allCountries.Add(new CountryData { countryName = "UAE (Dubai)", landmarkTheme = "Burj Khalifa", environment = "Modern city, glass towers, desert", unlockCost = 3000, isUnlocked = false });
        allCountries.Add(new CountryData { countryName = "Brazil", landmarkTheme = "Christ the Redeemer (Rio)", environment = "Jungle, beaches, carnival vibes", unlockCost = 3500, isUnlocked = false });
        allCountries.Add(new CountryData { countryName = "USA", landmarkTheme = "Statue of Liberty (New York)", environment = "Skyscrapers, Central Park, highways", unlockCost = 5000, isUnlocked = false });
    }

    public CountryData GetCountryData(string countryName)
    {
        return allCountries.Find(country => country.countryName == countryName);
    }

    public bool IsCountryUnlocked(string countryName)
    {
        CountryData country = GetCountryData(countryName);
        return country != null && country.isUnlocked;
    }

    public bool TryUnlockCountry(string countryName)
    {
        CountryData country = GetCountryData(countryName);
        if (country != null && !country.isUnlocked)
        {
            if (CollectibleSystem.Instance.SpendCoins(country.unlockCost))
            {
                country.isUnlocked = true;
                onCountryUnlocked?.Invoke(countryName);
                Debug.Log($"Unlocked {countryName} for {country.unlockCost} coins.");
                return true;
            }
            else
            {
                Debug.Log($"Not enough coins to unlock {countryName}.");
                // UIManager.Instance.ShowAdPromptForCoins(); // Placeholder for UI interaction
            }
        }
        return false;
    }

    public void SetCurrentCountry(string countryName)
    {
        CountryData country = GetCountryData(countryName);
        if (country != null && country.isUnlocked)
        {
            currentCountry = country;
            onCurrentCountryChanged?.Invoke(countryName);
            Debug.Log($"Current country set to: {countryName}");
        }
        else
        {
            Debug.LogError($"Attempted to set current country to {countryName}, but it is either null or locked.");
        }
    }

    // Placeholder for mini-map display logic
    public void UpdateMiniMapProgress(float progress)
    {
        // UIManager.Instance.UpdateMiniMap(currentCountry.countryName, progress);
        Debug.Log($"Mini-map progress for {currentCountry.countryName}: {progress * 100:F0}%");
    }
}
