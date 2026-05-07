using System;

/// <summary>
/// Static event bus for audio triggers.
///
/// Usage — raise events from any game system without a direct AudioManager reference:
///   GameAudioEvents.OnPlayJump?.Invoke();
///   GameAudioEvents.OnChangeBGM?.Invoke("Jordan");
///
/// AudioManager subscribes to all events in its Start() and unsubscribes in OnDestroy().
/// </summary>
public static class GameAudioEvents
{
    // Gameplay SFX
    public static Action OnPlayJump;
    public static Action OnPlaySlide;
    public static Action OnPlayCollectCoins;
    public static Action OnPlayCollectDates;
    public static Action OnPlayCollectGems;
    public static Action OnPlayPowerUp;
    public static Action OnPlayCollision;

    // Health / state
    public static Action OnPlayHealthRestored;
    public static Action OnPlayHealthWarning;

    // Game state
    public static Action OnPlayGameOver;
    public static Action OnPlayMilestone;
    public static Action OnPlayCountryUnlocked;

    // Thief
    public static Action OnPlayThiefAppears;
    public static Action OnPlayThiefSteals;
    public static Action OnPlayEscapeThief;

    // UI
    public static Action OnPlayUIClick;

    // BGM change (passes country name string)
    public static Action<string> OnChangeBGM;
}
