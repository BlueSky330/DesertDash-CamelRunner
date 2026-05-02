using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance { get; private set; }

    // Power-up durations
    public float magicCarpetDuration = 5f;
    public float magnetDuration = 7f;
    public float doubleCoinsDuration = 10f;

    // Power-up states
    public bool isMagicCarpetActive { get; private set; }
    public bool isMagnetActive { get; private set; }
    public bool isDoubleCoinsActive { get; private set; }
    public int antiThiefShieldCount { get; private set; } = 0;

    public delegate void OnPowerUpActivated(PowerUpType type, float duration);
    public static event OnPowerUpActivated onPowerUpActivated;

    public delegate void OnPowerUpDeactivated(PowerUpType type);
    public static event OnPowerUpDeactivated onPowerUpDeactivated;

    public delegate void OnAntiThiefShieldChanged(int count);
    public static event OnAntiThiefShieldChanged onAntiThiefShieldChanged;

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

    public void ActivatePowerUp(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.MagicCarpet:
                StartCoroutine(MagicCarpetRoutine());
                break;
            case PowerUpType.ScarabShell:
                // Scarab Shell is a one-time use shield, handled by PlayerController on collision
                // For now, we just indicate it's available.
                Debug.Log("Scarab Shell acquired. Player is protected from one collision.");
                break;
            case PowerUpType.OasisBreeze:
                StartCoroutine(MagnetRoutine());
                break;
            case PowerUpType.GoldenScarab:
                StartCoroutine(DoubleCoinsRoutine());
                break;
            case PowerUpType.AntiThiefShield:
                AddAntiThiefShield();
                break;
        }
        onPowerUpActivated?.Invoke(type, GetPowerUpDuration(type));
    }

    private float GetPowerUpDuration(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.MagicCarpet:
                return magicCarpetDuration;
            case PowerUpType.OasisBreeze:
                return magnetDuration;
            case PowerUpType.GoldenScarab:
                return doubleCoinsDuration;
            default:
                return 0f;
        }
    }

    IEnumerator MagicCarpetRoutine()
    {
        if (isMagicCarpetActive) yield break; // Prevent multiple activations
        isMagicCarpetActive = true;
        Debug.Log("Magic Carpet Activated!");
        // PlayerController will handle invincibility and speed boost
        yield return new WaitForSeconds(magicCarpetDuration);
        isMagicCarpetActive = false;
        Debug.Log("Magic Carpet Deactivated.");
        onPowerUpDeactivated?.Invoke(PowerUpType.MagicCarpet);
    }

    IEnumerator MagnetRoutine()
    {
        if (isMagnetActive) yield break;
        isMagnetActive = true;
        Debug.Log("Magnet Activated!");
        // CollectibleSpawner or PlayerController will handle attraction
        yield return new WaitForSeconds(magnetDuration);
        isMagnetActive = false;
        Debug.Log("Magnet Deactivated.");
        onPowerUpDeactivated?.Invoke(PowerUpType.OasisBreeze);
    }

    IEnumerator DoubleCoinsRoutine()
    {
        if (isDoubleCoinsActive) yield break;
        isDoubleCoinsActive = true;
        Debug.Log("Double Coins Activated!");
        // CollectibleSystem will handle double coin value
        yield return new WaitForSeconds(doubleCoinsDuration);
        isDoubleCoinsActive = false;
        Debug.Log("Double Coins Deactivated.");
        onPowerUpDeactivated?.Invoke(PowerUpType.GoldenScarab);
    }

    public void AddAntiThiefShield()
    {
        // Cost 20% of current coins
        int cost = Mathf.FloorToInt(CollectibleSystem.Instance.currentCoins * 0.20f);
        if (CollectibleSystem.Instance.SpendCoins(cost))
        {
            antiThiefShieldCount++;
            onAntiThiefShieldChanged?.Invoke(antiThiefShieldCount);
            Debug.Log($"Anti-Thief Shield acquired for {cost} coins. Total: {antiThiefShieldCount}");
        }
        else
        {
            Debug.Log("Not enough coins to buy Anti-Thief Shield.");
        }
    }

    public bool UseAntiThiefShield()
    {
        if (antiThiefShieldCount > 0)
        {
            antiThiefShieldCount--;
            onAntiThiefShieldChanged?.Invoke(antiThiefShieldCount);
            Debug.Log("Anti-Thief Shield used.");
            return true;
        }
        return false;
    }

    public enum PowerUpType
    {
        MagicCarpet,
        ScarabShell,
        OasisBreeze,
        GoldenScarab,
        AntiThiefShield
    }
}
