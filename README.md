# Desert Dash: Camel Runner

An adventurous 2.5D endless runner game where players guide Kamil the Camel on a thrilling World Tour across 10 iconic countries. Dodge unique obstacles, collect valuable items, and unlock new destinations, all within a stylized low-poly 3D environment. The game emphasizes quick reflexes, simple controls, and a rewarding player-driven monetization loop.

## Project Status

This project is currently in **Phase 4: Audio Generation & Ad SDK Integration**. Detailed audio specifications and generation prompts have been created, along with the `AudioManager.cs` script. Placeholder scripts for Google AdMob, Unity Ads, and Ad Mediation have been developed, and an `ad_config.json` file and `AD_SETUP_GUIDE.md` have been created.

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

*   **Unity Project Structure**: Standard Unity folders (`Assets/Scripts`, `Assets/Scenes`, `Assets/Prefabs`, `Assets/Textures/ConceptArt`, `Assets/Audio`, etc.) are set up.
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
    *   `AdSDKIntegration.cs`: Placeholder script for Google AdMob and Unity Ads SDK integration.
    *   `AdMediation.cs`: Placeholder script for ironSource/LevelPlay ad mediation.
    *   `ad_config.json`: Configuration file for ad unit IDs, frequency, and reward amounts.
    *   `AD_SETUP_GUIDE.md`: Guide for setting up AdMob and Unity Ads accounts, obtaining IDs, and testing.

### Next Steps:

*   Create actual Unity scene files and set up basic UI elements.
*   Integrate initial AI-generated assets (camel model, basic obstacles).
*   Refine player controls and physics.
*   Implement collision detection and basic game over conditions.
*   Begin generating actual game-ready assets based on the detailed prompts.
