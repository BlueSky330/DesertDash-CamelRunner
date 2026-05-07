# Audio/Music — Expected Files

Place CC0-licensed music files here. After adding files, run **Tools > Camel Runner > Wire AudioManager Clips** to assign them in the Inspector.

| Field on AudioManager | Expected Filename | Notes |
|---|---|---|
| egyptBGM | `egypt_bgm.ogg` | Middle Eastern fusion loop — phase 1 |
| jordanBGM | `jordan_bgm.ogg` | Phase 2 |
| indiaBGM | `india_bgm.ogg` | Phase 2 |
| chinaBGM | `china_bgm.ogg` | Phase 2 |
| italyBGM | `italy_bgm.ogg` | Phase 2 |
| peruBGM | `peru_bgm.ogg` | Phase 2 |
| franceBGM | `france_bgm.ogg` | Phase 2 |
| uaeBGM | `uae_bgm.ogg` | Phase 2 |
| brazilBGM | `brazil_bgm.ogg` | Phase 2 |
| usaBGM | `usa_bgm.ogg` | Phase 2 |

**Format:** OGG Vorbis, stereo, loop-point set, 128–192 kbps.

The game runs silently without errors if these files are absent — `AudioManager` gracefully skips null clips.
