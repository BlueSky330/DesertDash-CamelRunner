# Audio Sourcing Guide — Camel Runner (Desert Dash)
**M7 Deliverable | Game Designer spec | 2026-05-07**

All audio must be CC0 / royalty-free for commercial use. Total compressed budget: **< 5 MB** (OGG for Android, AAC for iOS).

---

## Background Music

### Egypt BGM (primary track — used at game start)
**File name:** `egypt_bgm.ogg` / `egypt_bgm.aac`
**Duration:** 90–120 s loopable (seamless loop)
**Style:** Middle Eastern fusion — oud melody + darbuka rhythm + electronic bass. Upbeat, adventurous, 120–130 BPM. Key in D or G maqam Rast.
**Target size:** ≤ 1.2 MB compressed

**Sourcing options (CC0):**
- OpenGameArt.org → search "arabian" / "middle eastern" / "desert"
  - Recommended: Kevin MacLeod — "Carefree" or "Arabian Dream" (CC BY → attribute)
- freesound.org → filter by CC0, search "oud loop" + "darbuka"
- AI composition via Suno.ai or Udio — generate and export; verify license is commercial-free

**Loop point:** Export with a clean loop by matching downbeat. In Unity Audio Import Settings enable **Load Type: Streaming**, **Compression Format: Vorbis**, **Quality: 70**.

---

### Jordan, India, China, Italy, Peru, France, UAE, Brazil, USA BGMs
Apply same style guidelines per `audio_specs.md`. Each track ≤ 600 KB compressed. Source from same CC0 libraries. Phase 2 priority (MVP ships Egypt only).

---

## Sound Effects

All SFX: short (< 3 s), mono or stereo. Import Settings: **Decompress on Load**, **Compression: Vorbis 50%**.

| File name | Description | Target size | Sourcing keywords |
|---|---|---|---|
| `sfx_jump.ogg` | Light whoosh, air sliced. Punchy, 0.3–0.5 s. | ≤ 30 KB | "whoosh", "swipe", "air rush" |
| `sfx_slide.ogg` | Sandy gritty swoosh, 0.4–0.6 s | ≤ 35 KB | "slide", "sand swoosh", "friction" |
| `sfx_coin_collect.ogg` | Crisp metallic ding/chime. 0.2–0.3 s | ≤ 20 KB | "coin", "ding", "pickup chime" |
| `sfx_date_collect.ogg` | Soft satisfying pop/bloop. 0.2 s | ≤ 15 KB | "pop", "bloop", "soft collect" |
| `sfx_gem_collect.ogg` | Bright magical sparkle. 0.3–0.4 s | ≤ 25 KB | "sparkle", "gem", "magic chime" |
| `sfx_powerup.ogg` | Ascending magical chime / synth sweep. 0.5–0.8 s | ≤ 40 KB | "power up", "magic sweep", "ascending chime" |
| `sfx_collision.ogg` | Comical dull thud. 0.3–0.5 s | ≤ 30 KB | "thud", "bump", "cartoon hit" |
| `sfx_camel_grunt.ogg` | Short disappointed camel vocalization ~0.5 s | ≤ 40 KB | **ElevenLabs** (see below) or freesound "camel grunt" |
| `sfx_health_restore.ogg` | Refreshing oasis whoosh / healing chime. 0.6 s | ≤ 40 KB | "heal", "restore", "water splash short" |
| `sfx_health_warning.ogg` | Pulsing muffled heartbeat. 1–2 s loopable | ≤ 50 KB | "heartbeat warning", "pulse low health" |
| `sfx_game_over.ogg` | Comical descending sad trombone / deflate. 1–1.5 s | ≤ 60 KB | "sad trombone", "fail", "wah wah" |
| `sfx_milestone.ogg` | Bright checkpoint chime / short fanfare. 0.8 s | ≤ 50 KB | "checkpoint", "level up chime", "fanfare short" |
| `sfx_country_unlocked.ogg` | Grand celebratory fanfare / orchestral swell. 1.5–2 s | ≤ 100 KB | "fanfare", "triumphant", "unlock jingle" |
| `sfx_thief_appears.ogg` | Dramatic sting / sharp synth stab. 0.4 s | ≤ 30 KB | "danger sting", "alert stab", "warning hit" |
| `sfx_thief_steals.ogg` | Coins scattering / descending coins. 0.6 s | ≤ 40 KB | "coins scatter", "steal", "negative jingle" |
| `sfx_escape_thief.ogg` | Short triumphant ascending jingle. 0.5 s | ≤ 30 KB | "escape", "triumph short", "ascending jingle" |
| `sfx_ui_click.ogg` | Dry sandy tap/click. 0.1–0.2 s | ≤ 10 KB | "ui click", "button tap", "dry click" |

---

## ElevenLabs — Camel Grunt Voice

**Purpose:** `sfx_camel_grunt.ogg` — a short, comical, disappointed camel sound triggered on collision.

**Generation prompt for ElevenLabs Sound Effects API:**
> "A short, comical camel grunt — surprised and slightly disappointed, like a cartoon camel who just bumped into something. Duration: 0.4–0.6 seconds. Tone: cartoonish, family-friendly."

**Steps:**
1. Go to ElevenLabs → Sound Effects (or use API endpoint `/v1/sound-generation`)
2. Paste prompt above. Generate 3–4 variations, pick most cartoonish.
3. Export as MP3, convert to OGG via Audacity or ffmpeg:
   `ffmpeg -i camel_grunt.mp3 -c:a libvorbis -q:a 4 sfx_camel_grunt.ogg`
4. Verify < 40 KB and non-offensive.

**Fallback (if ElevenLabs unavailable):** freesound.org search "camel grunt" filtered CC0.

---

## Unity Import Settings Reference

| Type | Load Type | Compression | Quality |
|---|---|---|---|
| BGM (long) | Streaming | Vorbis | 70 |
| SFX (short) | Decompress on Load | Vorbis | 50 |
| Health warning loop | Compressed in Memory | Vorbis | 50 |

---

## File Placement

```
Assets/Audio/
  Music/
    egypt_bgm.ogg
    jordan_bgm.ogg
    ... (other countries, Phase 2)
  SFX/
    sfx_jump.ogg
    sfx_slide.ogg
    sfx_coin_collect.ogg
    sfx_date_collect.ogg
    sfx_gem_collect.ogg
    sfx_powerup.ogg
    sfx_collision.ogg
    sfx_camel_grunt.ogg
    sfx_health_restore.ogg
    sfx_health_warning.ogg
    sfx_game_over.ogg
    sfx_milestone.ogg
    sfx_country_unlocked.ogg
    sfx_thief_appears.ogg
    sfx_thief_steals.ogg
    sfx_escape_thief.ogg
    sfx_ui_click.ogg
```

---

## AudioManager Wiring (after sourcing)

Drag each clip into the corresponding Inspector slot on the `AudioManager` GameObject in the main scene:

- `egyptBGM` ← `Music/egypt_bgm`
- `jumpSFX` ← `SFX/sfx_jump`
- `collectCoinsSFX` ← `SFX/sfx_coin_collect`
- `camelGruntSFX` ← `SFX/sfx_camel_grunt`
- *(etc. — one-to-one with AudioManager.cs public fields)*

---

## Budget Estimate

| Category | Approx size |
|---|---|
| Egypt BGM | 1.2 MB |
| 9 other BGMs (Phase 2) | ~5 MB |
| 17 SFX files | ~0.7 MB |
| **MVP total (Egypt + SFX)** | **~1.9 MB** |
| **Full game total** | **~6.7 MB → compress to ≤ 5 MB** |

To hit < 5 MB: reduce BGM quality to 60 and use mono for SFX. MVP (Egypt only) fits comfortably.
