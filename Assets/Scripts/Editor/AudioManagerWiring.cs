using UnityEditor;
using UnityEngine;

/// <summary>
/// M7 Audio — One-click AudioManager clip wiring tool.
///
/// Run via: Tools > Camel Runner > Wire AudioManager Clips
///
/// Workflow:
///   1. Open the Gameplay scene.
///   2. Run this tool — it loads all OGG clips from Assets/Audio/
///      and assigns them to the AudioManager GameObject's public fields.
///   3. Save the scene (Ctrl+S).
///   4. Press Play — verify no "Missing AudioClip" warnings in Console.
///
/// Re-run after replacing placeholder OGGs with real CC0 assets; the tool
/// overwrites the Inspector slots automatically (GUIDs are stable once .meta
/// files are committed to version control).
/// </summary>
public static class AudioManagerWiring
{
    private const string MusicDir = "Assets/Audio/Music";
    private const string SFXDir   = "Assets/Audio/SFX";

    [MenuItem("Tools/Camel Runner/Wire AudioManager Clips")]
    public static void WireAudioManagerClips()
    {
        AudioManager mgr = Object.FindObjectOfType<AudioManager>();
        if (mgr == null)
        {
            EditorUtility.DisplayDialog(
                "AudioManager Not Found",
                "No AudioManager component found in the active scene.\n\n" +
                "1. Open the Gameplay scene.\n" +
                "2. Make sure AudioManager is present on a GameObject.\n" +
                "3. Re-run this tool.",
                "OK");
            return;
        }

        // ── Background Music ─────────────────────────────────────────────
        mgr.egyptBGM  = LoadClip(MusicDir, "egypt_bgm");
        // Phase-2 BGMs — assign when sourced:
        // mgr.jordanBGM = LoadClip(MusicDir, "jordan_bgm");
        // mgr.indiaBGM  = LoadClip(MusicDir, "india_bgm");
        // mgr.chinaBGM  = LoadClip(MusicDir, "china_bgm");
        // mgr.italyBGM  = LoadClip(MusicDir, "italy_bgm");
        // mgr.peruBGM   = LoadClip(MusicDir, "peru_bgm");
        // mgr.franceBGM = LoadClip(MusicDir, "france_bgm");
        // mgr.uaeBGM    = LoadClip(MusicDir, "uae_bgm");
        // mgr.brazilBGM = LoadClip(MusicDir, "brazil_bgm");
        // mgr.usaBGM    = LoadClip(MusicDir, "usa_bgm");

        // ── Gameplay SFX ─────────────────────────────────────────────────
        mgr.jumpSFX             = LoadClip(SFXDir, "sfx_jump");
        mgr.slideSFX            = LoadClip(SFXDir, "sfx_slide");
        mgr.collectCoinsSFX     = LoadClip(SFXDir, "sfx_coin_collect");
        mgr.collectDatesSFX     = LoadClip(SFXDir, "sfx_date_collect");
        mgr.collectGemsSFX      = LoadClip(SFXDir, "sfx_gem_collect");
        mgr.powerUpActivateSFX  = LoadClip(SFXDir, "sfx_powerup");
        mgr.collisionSFX        = LoadClip(SFXDir, "sfx_collision");
        mgr.camelGruntSFX       = LoadClip(SFXDir, "sfx_camel_grunt");

        // ── Game State SFX ───────────────────────────────────────────────
        mgr.gameOverSFX          = LoadClip(SFXDir, "sfx_game_over");
        mgr.milestoneReachedSFX  = LoadClip(SFXDir, "sfx_milestone");
        mgr.countryUnlockedSFX   = LoadClip(SFXDir, "sfx_country_unlocked");
        mgr.healthRestoredSFX    = LoadClip(SFXDir, "sfx_health_restore");
        mgr.healthLowWarningSFX  = LoadClip(SFXDir, "sfx_health_warning");

        // ── Thief SFX ────────────────────────────────────────────────────
        mgr.thiefAppearsSFX  = LoadClip(SFXDir, "sfx_thief_appears");
        mgr.thiefStealsSFX   = LoadClip(SFXDir, "sfx_thief_steals");
        mgr.escapeThiefSFX   = LoadClip(SFXDir, "sfx_escape_thief");

        // ── UI SFX ───────────────────────────────────────────────────────
        mgr.uiClickSFX = LoadClip(SFXDir, "sfx_ui_click");

        EditorUtility.SetDirty(mgr);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            mgr.gameObject.scene);

        int nullCount = CountNullClips(mgr);
        string summary = nullCount == 0
            ? "All clips wired successfully."
            : $"{nullCount} clip(s) still null — check Console for missing files.";

        Debug.Log($"[AudioManagerWiring] Wiring complete. {summary}");

        EditorUtility.DisplayDialog(
            "AudioManager Wired",
            $"Clips assigned to AudioManager.\n\n{summary}\n\n" +
            "Save the scene (Ctrl+S) to persist the wiring.\n\n" +
            "NOTE: Current clips are placeholders. Replace OGG files\n" +
            "with real CC0 assets and re-run this tool.",
            "Save & Close");

        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
    }

    // ── Helpers ──────────────────────────────────────────────────────────

    private static AudioClip LoadClip(string dir, string name)
    {
        string path = $"{dir}/{name}.ogg";
        var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        if (clip == null)
            Debug.LogWarning($"[AudioManagerWiring] Missing clip: {path}");
        return clip;
    }

    private static int CountNullClips(AudioManager mgr)
    {
        int n = 0;
        if (mgr.egyptBGM          == null) n++;
        if (mgr.jumpSFX            == null) n++;
        if (mgr.slideSFX           == null) n++;
        if (mgr.collectCoinsSFX    == null) n++;
        if (mgr.collectDatesSFX    == null) n++;
        if (mgr.collectGemsSFX     == null) n++;
        if (mgr.powerUpActivateSFX == null) n++;
        if (mgr.collisionSFX       == null) n++;
        if (mgr.camelGruntSFX      == null) n++;
        if (mgr.gameOverSFX        == null) n++;
        if (mgr.milestoneReachedSFX== null) n++;
        if (mgr.countryUnlockedSFX == null) n++;
        if (mgr.healthRestoredSFX  == null) n++;
        if (mgr.healthLowWarningSFX== null) n++;
        if (mgr.thiefAppearsSFX    == null) n++;
        if (mgr.thiefStealsSFX     == null) n++;
        if (mgr.escapeThiefSFX     == null) n++;
        if (mgr.uiClickSFX         == null) n++;
        return n;
    }
}
