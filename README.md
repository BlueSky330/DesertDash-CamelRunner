# Desert Dash: Camel Runner

An adventurous 2.5D endless runner game where players guide Kamil the Camel on a thrilling World Tour across 10 iconic countries. Dodge unique obstacles, collect valuable items, and unlock new destinations, all within a stylized low-poly 3D environment. The game emphasizes quick reflexes, simple controls, and a rewarding player-driven monetization loop.

## Project Status

This project is now **READY FOR CLIENT REVIEW**.

All phases of development, from initial game design to app store preparation and final packaging, have been completed. The repository contains all necessary scripts, configurations, and documentation to proceed with building and launching the game.

## How to Open in Unity

1.  **Clone the Repository**: If you haven't already, clone this repository to your local machine:
    ```bash
    git clone https://github.com/BlueSky330/DesertDash-CamelRunner.git
    ```
2.  **Open Unity Hub**: Launch Unity Hub.
3.  **Add Project**: Click on the "Add" button and navigate to the `DesertDash-CamelRunner` folder you just cloned. Select the folder and click "Add Project."
4.  **Open Project**: Select the "Desert Dash: Camel Runner" project in Unity Hub and click "Open Project" to launch it in the Unity Editor.

## Current Development Status

### Implemented:

*   **Unity Project Structure**: Standard Unity folders (`Assets/Scripts`, `Assets/Scenes`, `Assets/Prefabs`, `Assets/Textures/ConceptArt`, `Assets/Audio`, `Assets/StoreAssets`, etc.) are set up.
*   **.gitignore**: A standard Unity `.gitignore` file is included to manage version control.
*   **Core Scripts**:
    *   `PlayerController.cs`: Handles 3-lane movement, jump, and slide mechanics.
    *   `GameManager.cs`: Manages game state (start, end, score) and scene loading.
    *   `LevelGenerator.cs`: Implements procedural chunk-based level generation.
    *   `ObstacleSpawner.cs`: Spawns obstacles within the lanes.
    *   `CollectibleSpawner.cs`: Spawns dates, coins, and gems.
*   **Placeholder Scenes**: Markdown files representing `MainMenuScene`, `GameplayScene`, and `GameOverScene` are created. Actual Unity scene files will be created and configured within the Unity Editor.
*   **Updated GDD**: The Game Design Document (`GDD.md`) has been significantly updated to reflect the 2.5D game format, World Tour system, detailed gameplay flow, and the refined offline/online "Player-Driven Rewarded Ads" monetization strategy.
*   **AI Asset Specifications**: Detailed prompts for visual assets (`visual_asset_prompts.md`) and audio assets (`audio_asset_prompts.md`) have been created.
*   **Regenerated Concept Art**: All camel character concepts (Kamil, Pharaoh, Racing, Mummy, Golden) have been regenerated in a consistent low-poly 3D style, matching the provided reference images.
*   **Gameplay System Scripts (Phase 3)**:
    *   `CollectibleSystem.cs`: Manages collectibles, score, and score-to-coin conversion.
    *   `PowerUpManager.cs`: Handles various power-ups including the Anti-Thief Shield.
    *   `ThiefSystem.cs`: Manages random thief spawns, their behavior, and coin stealing.
    *   `HealthSystem.cs`: Implements camel health, decay, natural and ad-based recovery.
    *   `MilestoneSystem.cs`: Manages checkpoints and progress saving.
    *   `WorldMapManager.cs`: Handles country data, unlock status, and mini-map display.
    *   `AdManager.cs`: Manages rewarded ads, tiered rewards, and health restoration ads.
    *   `CoinEconomy.cs`: Manages coin wallet, offline spending, and purchase system.
    *   `SkinManager.cs`: Manages camel skins, their functional benefits, and purchase/equip.
    *   `UIManager.cs`: Manages all UI elements.
    *   `OfflineManager.cs`: Detects online/offline status and manages related game features.
*   **Audio System (Phase 4)**:
    *   `audio_specs.md`: Detailed specifications and generation prompts for background music and sound effects.
    *   `AudioManager.cs`: Script for managing background music per country and playing sound effects.
*   **Ad SDK Integration (Phase 4)**:
    *   `AdSDKIntegration.cs`: Placeholder script for Google AdMob and Unity Ads SDK integration, updated with content filtering comments.
    *   `AdMediation.cs`: Placeholder script for ironSource/LevelPlay ad mediation.
    *   `ad_config.json`: Configuration file for ad unit IDs, frequency, reward amounts, and content filtering settings.
    *   `AD_SETUP_GUIDE.md`: Guide for setting up AdMob and Unity Ads accounts, obtaining IDs, testing, and content filtering configuration.
*   **QA Testing Framework (Phase 5)**:
    *   `GameplayTester.cs`: Automated script for simulating player input and testing core gameplay mechanics.
    *   `EconomyBalanceTester.cs`: Script for verifying coin earning/spending rates and overall economy balance.
    *   `PerformanceTester.cs`: Script for monitoring FPS, memory usage, and other performance metrics.
    *   `AdTester.cs`: Script for simulating ad interactions and verifying reward granting and content filtering.
*   **Performance Optimization (Phase 5)**:
    *   `PerformanceOptimizer.cs`: Script outlining object pooling, LOD, texture/audio compression, and shader optimization strategies.
*   **Difficulty Balancing (Phase 5)**:
    *   `DifficultyManager.cs`: Script for managing speed curves, obstacle density, and thief frequency.
*   **Polish & Visual Effects (Phase 5)**:
    *   `VisualEffects.cs`: Script for managing particle effects, screen effects (low health warning), and camera effects (thief zoom).
*   **App Store Preparation (Phase 6)**:
    *   `Assets/StoreAssets/store_listing.md`: Google Play Store listing details.
    *   `Assets/StoreAssets/apple_store_listing.md`: Apple App Store listing details.
    *   `Assets/StoreAssets/screenshot_prompts.md`: AI prompts for store screenshots, feature graphic, and app icon.
    *   `Assets/StoreAssets/app_icon_concept.png`: Generated app icon concept art.
    *   `build_config.md`: Android and iOS build settings, signing requirements, and build steps.
    *   `LAUNCH_CHECKLIST.md`: Comprehensive checklist for app launch.
    *   `PROJECT_SUMMARY.md`: Full overview of the project, scripts, architecture, and future steps.

### Next Steps:

*   Client review of all generated documentation and code.
*   Manual integration of AI-generated assets into Unity scenes.
*   Thorough manual testing and debugging of all implemented systems.
*   Building and deployment to app stores.
