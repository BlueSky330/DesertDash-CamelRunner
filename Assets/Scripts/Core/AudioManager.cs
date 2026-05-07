using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// AudioManager — event-driven, AudioSource-pooled audio system.
///
/// Architecture:
///   - Static C# events on GameAudioEvents allow any system (Player, UI,
///     Collectibles, Health) to fire audio without a direct reference.
///   - An AudioSource pool of SFX_POOL_SIZE sources handles overlapping SFX.
///   - Music uses a dedicated looping AudioSource.
///   - Settings toggles (volume + mute) persist via PlayerPrefs and are called
///     from UIManager's settings panel.
///
/// Animation event hookup (stub — wire after M3 character models land):
///   - Animator's "Jump_Start" event → AudioManager.Instance.PlayJumpSFX()
///   - Animator's "Footstep" event → AudioManager.Instance.PlayFootstepSFX()
///   These are direct calls from Animation Events on the character Animator.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    // -----------------------------------------------------------------------
    // SFX Pool
    // -----------------------------------------------------------------------
    private const int SFX_POOL_SIZE = 6;
    private AudioSource[] _sfxPool;
    private int _sfxPoolIndex = 0;

    // -----------------------------------------------------------------------
    // Music source
    // -----------------------------------------------------------------------
    private AudioSource _musicSource;

    // -----------------------------------------------------------------------
    // Inspector — Background Music per country
    // -----------------------------------------------------------------------
    [Header("Background Music (per country)")]
    public AudioClip egyptBGM;
    public AudioClip jordanBGM;
    public AudioClip indiaBGM;
    public AudioClip chinaBGM;
    public AudioClip italyBGM;
    public AudioClip peruBGM;
    public AudioClip franceBGM;
    public AudioClip uaeBGM;
    public AudioClip brazilBGM;
    public AudioClip usaBGM;

    private Dictionary<string, AudioClip> _countryBGMs;

    // -----------------------------------------------------------------------
    // Inspector — Sound Effects
    // -----------------------------------------------------------------------
    [Header("Gameplay SFX")]
    public AudioClip jumpSFX;
    public AudioClip slideSFX;
    public AudioClip collectCoinsSFX;
    public AudioClip collectDatesSFX;
    public AudioClip collectGemsSFX;
    public AudioClip powerUpActivateSFX;
    public AudioClip collisionSFX;
    public AudioClip camelGruntSFX;

    [Header("Game State SFX")]
    public AudioClip gameOverSFX;
    public AudioClip milestoneReachedSFX;
    public AudioClip countryUnlockedSFX;
    public AudioClip healthRestoredSFX;
    public AudioClip healthLowWarningSFX;

    [Header("Thief SFX")]
    public AudioClip thiefAppearsSFX;
    public AudioClip thiefStealsSFX;
    public AudioClip escapeThiefSFX;

    [Header("UI SFX")]
    public AudioClip uiClickSFX;

    // -----------------------------------------------------------------------
    // Lifecycle
    // -----------------------------------------------------------------------
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildPool();
        BuildMusicSource();
        BuildCountryMap();
    }

    void Start()
    {
        ApplyStoredSettings();
        SubscribeToEvents();
        PlayBackgroundMusic("Egypt");
    }

    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    // -----------------------------------------------------------------------
    // Initialisation helpers
    // -----------------------------------------------------------------------
    private void BuildPool()
    {
        _sfxPool = new AudioSource[SFX_POOL_SIZE];
        for (int i = 0; i < SFX_POOL_SIZE; i++)
        {
            var go = new GameObject($"SFX_Pool_{i}");
            go.transform.SetParent(transform);
            _sfxPool[i] = go.AddComponent<AudioSource>();
            _sfxPool[i].playOnAwake = false;
        }
    }

    private void BuildMusicSource()
    {
        var go = new GameObject("Music_Source");
        go.transform.SetParent(transform);
        _musicSource = go.AddComponent<AudioSource>();
        _musicSource.loop = true;
        _musicSource.playOnAwake = false;
    }

    private void BuildCountryMap()
    {
        _countryBGMs = new Dictionary<string, AudioClip>
        {
            { "Egypt",       egyptBGM    },
            { "Jordan",      jordanBGM   },
            { "India",       indiaBGM    },
            { "China",       chinaBGM    },
            { "Italy",       italyBGM    },
            { "Peru",        peruBGM     },
            { "France",      franceBGM   },
            { "UAE (Dubai)", uaeBGM      },
            { "Brazil",      brazilBGM   },
            { "USA",         usaBGM      }
        };
    }

    private void ApplyStoredSettings()
    {
        _musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        SetSFXPoolVolume(PlayerPrefs.GetFloat("SFXVolume", 0.8f));
        _musicSource.mute = PlayerPrefs.GetInt("MusicMute", 0) == 1;
        bool sfxMuted = PlayerPrefs.GetInt("SFXMute", 0) == 1;
        foreach (var src in _sfxPool) src.mute = sfxMuted;
    }

    // -----------------------------------------------------------------------
    // Event subscription (event-driven architecture)
    // Other systems raise GameAudioEvents; AudioManager listens.
    // -----------------------------------------------------------------------
    private void SubscribeToEvents()
    {
        GameAudioEvents.OnPlayJump          += PlayJumpSFX;
        GameAudioEvents.OnPlaySlide         += PlaySlideSFX;
        GameAudioEvents.OnPlayCollectCoins  += PlayCollectCoinsSFX;
        GameAudioEvents.OnPlayCollectDates  += PlayCollectDatesSFX;
        GameAudioEvents.OnPlayCollectGems   += PlayCollectGemsSFX;
        GameAudioEvents.OnPlayPowerUp       += PlayPowerUpActivateSFX;
        GameAudioEvents.OnPlayCollision     += PlayCollisionSFX;
        GameAudioEvents.OnPlayHealthRestored  += PlayHealthRestoredSFX;
        GameAudioEvents.OnPlayHealthWarning += PlayHealthLowWarningSFX;
        GameAudioEvents.OnPlayGameOver      += PlayGameOverSFX;
        GameAudioEvents.OnPlayMilestone     += PlayMilestoneReachedSFX;
        GameAudioEvents.OnPlayCountryUnlocked += PlayCountryUnlockedSFX;
        GameAudioEvents.OnPlayThiefAppears  += PlayThiefAppearsSFX;
        GameAudioEvents.OnPlayThiefSteals   += PlayThiefStealsSFX;
        GameAudioEvents.OnPlayEscapeThief   += PlayEscapeThiefSFX;
        GameAudioEvents.OnPlayUIClick       += PlayUIClickSFX;
        GameAudioEvents.OnChangeBGM         += PlayBackgroundMusic;
    }

    private void UnsubscribeFromEvents()
    {
        GameAudioEvents.OnPlayJump          -= PlayJumpSFX;
        GameAudioEvents.OnPlaySlide         -= PlaySlideSFX;
        GameAudioEvents.OnPlayCollectCoins  -= PlayCollectCoinsSFX;
        GameAudioEvents.OnPlayCollectDates  -= PlayCollectDatesSFX;
        GameAudioEvents.OnPlayCollectGems   -= PlayCollectGemsSFX;
        GameAudioEvents.OnPlayPowerUp       -= PlayPowerUpActivateSFX;
        GameAudioEvents.OnPlayCollision     -= PlayCollisionSFX;
        GameAudioEvents.OnPlayHealthRestored  -= PlayHealthRestoredSFX;
        GameAudioEvents.OnPlayHealthWarning -= PlayHealthLowWarningSFX;
        GameAudioEvents.OnPlayGameOver      -= PlayGameOverSFX;
        GameAudioEvents.OnPlayMilestone     -= PlayMilestoneReachedSFX;
        GameAudioEvents.OnPlayCountryUnlocked -= PlayCountryUnlockedSFX;
        GameAudioEvents.OnPlayThiefAppears  -= PlayThiefAppearsSFX;
        GameAudioEvents.OnPlayThiefSteals   -= PlayThiefStealsSFX;
        GameAudioEvents.OnPlayEscapeThief   -= PlayEscapeThiefSFX;
        GameAudioEvents.OnPlayUIClick       -= PlayUIClickSFX;
        GameAudioEvents.OnChangeBGM         -= PlayBackgroundMusic;
    }

    // -----------------------------------------------------------------------
    // Core playback
    // -----------------------------------------------------------------------
    private void PlayPooledSFX(AudioClip clip)
    {
        if (clip == null) return;
        AudioSource src = _sfxPool[_sfxPoolIndex];
        _sfxPoolIndex = (_sfxPoolIndex + 1) % SFX_POOL_SIZE;
        src.clip = clip;
        src.Play();
    }

    public void PlayBackgroundMusic(string countryName)
    {
        if (_countryBGMs.TryGetValue(countryName, out AudioClip clip) && clip != null)
        {
            if (_musicSource.clip == clip && _musicSource.isPlaying) return;
            _musicSource.clip = clip;
            _musicSource.Play();
            Debug.Log($"[AudioManager] Playing BGM: {countryName}");
        }
        else
        {
            Debug.LogWarning($"[AudioManager] No BGM for country: {countryName}");
        }
    }

    // -----------------------------------------------------------------------
    // Public SFX methods — called directly by Animation Events (stubs)
    // or internally via event subscriptions above.
    // -----------------------------------------------------------------------
    public void PlayJumpSFX()            => PlayPooledSFX(jumpSFX);
    public void PlaySlideSFX()           => PlayPooledSFX(slideSFX);
    public void PlayCollectCoinsSFX()    => PlayPooledSFX(collectCoinsSFX);
    public void PlayCollectDatesSFX()    => PlayPooledSFX(collectDatesSFX);
    public void PlayCollectGemsSFX()     => PlayPooledSFX(collectGemsSFX);
    public void PlayPowerUpActivateSFX() => PlayPooledSFX(powerUpActivateSFX);
    public void PlayHealthRestoredSFX()  => PlayPooledSFX(healthRestoredSFX);
    public void PlayHealthLowWarningSFX()=> PlayPooledSFX(healthLowWarningSFX);
    public void PlayGameOverSFX()        => PlayPooledSFX(gameOverSFX);
    public void PlayMilestoneReachedSFX()=> PlayPooledSFX(milestoneReachedSFX);
    public void PlayCountryUnlockedSFX() => PlayPooledSFX(countryUnlockedSFX);
    public void PlayThiefAppearsSFX()    => PlayPooledSFX(thiefAppearsSFX);
    public void PlayThiefStealsSFX()     => PlayPooledSFX(thiefStealsSFX);
    public void PlayEscapeThiefSFX()     => PlayPooledSFX(escapeThiefSFX);
    public void PlayUIClickSFX()         => PlayPooledSFX(uiClickSFX);

    /// <summary>
    /// Collision plays two sounds: thud + camel grunt (sequential).
    /// Camel grunt is the ElevenLabs-generated voice clip.
    /// </summary>
    public void PlayCollisionSFX()
    {
        PlayPooledSFX(collisionSFX);
        PlayPooledSFX(camelGruntSFX);
    }

    // -----------------------------------------------------------------------
    // Animation Event stubs — wire these to the character Animator after
    // M3 character models are delivered.
    //
    // How to wire: open the Animator window, select a keyframe in the Jump
    // clip, add an Animation Event, and point it to AudioManager.Instance
    // or use a forwarding component on the character GameObject.
    // -----------------------------------------------------------------------

    /// <summary>STUB — Called by Animator "Jump_Audio" event.</summary>
    public void AnimEvent_Jump()         => PlayJumpSFX();

    /// <summary>STUB — Called by Animator "Slide_Audio" event.</summary>
    public void AnimEvent_Slide()        => PlaySlideSFX();

    /// <summary>STUB — Called by Animator "Collision_Audio" event.</summary>
    public void AnimEvent_Collision()    => PlayCollisionSFX();

    // -----------------------------------------------------------------------
    // Settings — called by UIManager settings panel toggles
    // -----------------------------------------------------------------------
    public void SetMusicVolume(float volume)
    {
        _musicSource.volume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("MusicVolume", _musicSource.volume);
    }

    public void SetSFXVolume(float volume)
    {
        SetSFXPoolVolume(Mathf.Clamp01(volume));
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void ToggleMusicMute(bool mute)
    {
        _musicSource.mute = mute;
        PlayerPrefs.SetInt("MusicMute", mute ? 1 : 0);
    }

    public void ToggleSFXMute(bool mute)
    {
        foreach (var src in _sfxPool) src.mute = mute;
        PlayerPrefs.SetInt("SFXMute", mute ? 1 : 0);
    }

    private void SetSFXPoolVolume(float volume)
    {
        foreach (var src in _sfxPool) src.volume = volume;
    }
}
