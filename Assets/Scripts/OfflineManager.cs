using UnityEngine;
using System.Collections;

public class OfflineManager : MonoBehaviour
{
    public static OfflineManager Instance { get; private set; }

    public bool isOnline { get; private set; }

    public delegate void OnOnlineStatusChanged(bool online);
    public static event OnOnlineStatusChanged onOnlineStatusChanged;

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
        // Initial check for internet connectivity
        CheckInternetConnectivity();
        // Start a routine to periodically check connectivity
        StartCoroutine(ConnectivityCheckRoutine());
    }

    private void CheckInternetConnectivity()
    {
        bool wasOnline = isOnline;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            isOnline = false;
            Debug.Log("Offline: No internet connection.");
        }
        else
        {
            isOnline = true;
            Debug.Log("Online: Internet connection available.");
        }

        if (isOnline != wasOnline)
        {
            onOnlineStatusChanged?.Invoke(isOnline);
            // UIManager.Instance.ShowOfflineWarning(!isOnline); // Show/hide warning based on status
        }
    }

    private IEnumerator ConnectivityCheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); // Check every 5 seconds
            CheckInternetConnectivity();

            // If offline and coins run out, show warning
            if (!isOnline && CollectibleSystem.Instance.currentCoins <= 0)
            {
                // UIManager.Instance.ShowOfflineWarning(true, "Go online to earn more coins!");
            }
        }
    }

    // This method would be called by other systems (e.g., AdManager, CoinEconomy)
    // to check if online features are available.
    public bool CanAccessOnlineFeatures()
    {
        return isOnline;
    }
}
