using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Background Music Clips (per country)")]
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

    private Dictionary<string, AudioClip> countryBGMs;

    [Header("Sound Effect Clips")]
    public AudioClip jumpSFX;
    public AudioClip slideSFX;
    public AudioClip collectDatesSFX;
    public AudioClip collectCoinsSFX;
    public AudioClip collectGemsSFX;
    public AudioClip powerUpActivateSFX;
    public AudioClip collisionSFX;
    public AudioClip camelGruntSFX;
    public AudioClip thiefAppearsSFX;
    public AudioClip thiefStealsSFX;
    public AudioClip escapeThiefSFX;
    public AudioClip healthLowWarningSFX;
    public AudioClip healthRestoredSFX;
    public AudioClip gameOverSFX;
    public AudioClip milestoneReachedSFX;
    public AudioClip countryUnlockedSFX;
    public AudioClip uiClickSFX;

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

        countryBGMs = new Dictionary<string, AudioClip>()
        {
            { "Egypt", egyptBGM },
            { "Jordan", jordanBGM },
            { "India", indiaBGM },
            { "China", chinaBGM },
            { "Italy", italyBGM },
            { "Peru", peruBGM },
            { "France", franceBGM },
            { "UAE (Dubai)", uaeBGM },
            { "Brazil", brazilBGM },
            { "USA", usaBGM }
        };
    }

    void Start()
    {
        // Load volume settings from PlayerPrefs or set defaults
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        musicSource.mute = PlayerPrefs.GetInt("MusicMute", 0) == 1;
        sfxSource.mute = PlayerPrefs.GetInt("SFXMute", 0) == 1;

        // Example: Play initial background music (e.g., Egypt)
        PlayBackgroundMusic("Egypt");
    }

    public void PlayBackgroundMusic(string countryName)
    {
        if (countryBGMs.ContainsKey(countryName) && countryBGMs[countryName] != null)
        {
            musicSource.clip = countryBGMs[countryName];
            musicSource.Play();
            Debug.Log($"Playing BGM for {countryName}");
        }
        else
        {
            Debug.LogWarning($"No BGM found for country: {countryName}");
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void ToggleMusicMute(bool mute)
    {
        musicSource.mute = mute;
        PlayerPrefs.SetInt("MusicMute", mute ? 1 : 0);
    }

    public void ToggleSFXMute(bool mute)
    {
        sfxSource.mute = mute;
        PlayerPrefs.SetInt("SFXMute", mute ? 1 : 0);
    }

    // Example usage for SFX (can be called from other scripts)
    public void PlayJumpSFX() { PlaySFX(jumpSFX); }
    public void PlaySlideSFX() { PlaySFX(slideSFX); }
    public void PlayCollectDatesSFX() { PlaySFX(collectDatesSFX); }
    public void PlayCollectCoinsSFX() { PlaySFX(collectCoinsSFX); }
    public void PlayCollectGemsSFX() { PlaySFX(collectGemsSFX); }
    public void PlayPowerUpActivateSFX() { PlaySFX(powerUpActivateSFX); }
    public void PlayCollisionSFX() { PlaySFX(collisionSFX); PlaySFX(camelGruntSFX); }
    public void PlayThiefAppearsSFX() { PlaySFX(thiefAppearsSFX); }
    public void PlayThiefStealsSFX() { PlaySFX(thiefStealsSFX); }
    public void PlayEscapeThiefSFX() { PlaySFX(escapeThiefSFX); }
    public void PlayHealthLowWarningSFX() { PlaySFX(healthLowWarningSFX); }
    public void PlayHealthRestoredSFX() { PlaySFX(healthRestoredSFX); }
    public void PlayGameOverSFX() { PlaySFX(gameOverSFX); }
    public void PlayMilestoneReachedSFX() { PlaySFX(milestoneReachedSFX); }
    public void PlayCountryUnlockedSFX() { PlaySFX(countryUnlockedSFX); }
    public void PlayUIClickSFX() { PlaySFX(uiClickSFX); }
}
