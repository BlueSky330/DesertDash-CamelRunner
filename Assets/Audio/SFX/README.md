# Audio/SFX — Expected Files

Place CC0-licensed SFX files here. After adding files, run **Tools > Camel Runner > Wire AudioManager Clips** to assign them in the Inspector.

| Field on AudioManager | Expected Filename | Description |
|---|---|---|
| jumpSFX | `sfx_jump.ogg` | Whoosh — camel jump |
| slideSFX | `sfx_slide.ogg` | Whoosh variant — camel slide |
| collectCoinsSFX | `sfx_coin_collect.ogg` | Ding/pop — silver coin pickup |
| collectDatesSFX | `sfx_date_collect.ogg` | Soft pop — date/golden-date pickup |
| collectGemsSFX | `sfx_gem_collect.ogg` | Bright chime — gem / mystery box |
| powerUpActivateSFX | `sfx_powerup.ogg` | Magical chime — any power-up activation |
| collisionSFX | `sfx_collision.ogg` | Comical thud — obstacle hit |
| camelGruntSFX | `sfx_camel_grunt.ogg` | Grunt/moan — plays alongside collision |
| gameOverSFX | `sfx_game_over.ogg` | Game-over sting |
| milestoneReachedSFX | `sfx_milestone.ogg` | Fanfare — milestone hit |
| countryUnlockedSFX | `sfx_country_unlocked.ogg` | Celebration — country unlocked |
| healthRestoredSFX | `sfx_health_restore.ogg` | Heal chime |
| healthLowWarningSFX | `sfx_health_warning.ogg` | Warning beep — health below 25% |
| thiefAppearsSFX | `sfx_thief_appears.ogg` | Sneak — thief enters screen |
| thiefStealsSFX | `sfx_thief_steals.ogg` | Coin steal — thief takes coins |
| escapeThiefSFX | `sfx_escape_thief.ogg` | Victory sting — player escapes thief |
| uiClickSFX | `sfx_ui_click.ogg` | Sandy click — all UI buttons |

**Format:** OGG Vorbis, mono, 44.1 kHz, 96–128 kbps.

The game runs silently without errors if these files are absent — `AudioManager` gracefully skips null clips.
