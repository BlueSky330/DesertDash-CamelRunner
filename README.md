# Desert Dash: Camel Runner

An endless runner game where players guide a charming camel through a stylized desert landscape, dodging obstacles, collecting dates, and outrunning sandstorms. The game emphasizes quick reflexes and simple controls, designed for rapid AI-driven development.

## Project Status

This project is currently in **Phase 2: AI Asset Generation**. The Game Design Document (GDD) has been updated with a new player-driven rewarded ads monetization strategy. Detailed AI prompts for visual and audio assets have been created, and placeholder concept art has been generated.

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

*   **Unity Project Structure**: Standard Unity folders (`Assets/Scripts`, `Assets/Scenes`, `Assets/Prefabs`, `Assets/Textures/ConceptArt`, etc.) are set up.
*   **.gitignore**: A standard Unity `.gitignore` file is included to manage version control.
*   **Core Scripts**:
    *   `PlayerController.cs`: Handles 3-lane movement, jump, and slide mechanics.
    *   `GameManager.cs`: Manages game state (start, end, score) and scene loading.
    *   `LevelGenerator.cs`: Implements procedural chunk-based level generation.
    *   `ObstacleSpawner.cs`: Spawns obstacles within the lanes.
    *   `CollectibleSpawner.cs`: Spawns dates, coins, and gems.
*   **Placeholder Scenes**: Markdown files representing `MainMenuScene`, `GameplayScene`, and `GameOverScene` are created. Actual Unity scene files will be created and configured within the Unity Editor.
*   **Updated GDD**: The Game Design Document (`GDD.md`) has been updated with the "Player-Driven Rewarded Ads" monetization strategy.
*   **AI Asset Specifications**: Detailed prompts for visual assets (`visual_asset_prompts.md`) and audio assets (`audio_asset_prompts.md`) have been created.
*   **Placeholder Concept Art**: Initial concept art for characters, environment, obstacles, collectibles, power-up icons, and UI elements has been generated and saved in `Assets/Textures/ConceptArt/`.

### Next Steps:

*   Create actual Unity scene files and set up basic UI elements.
*   Integrate initial AI-generated assets (camel model, basic obstacles).
*   Refine player controls and physics.
*   Implement collision detection and basic game over conditions.
*   Begin generating actual game-ready assets based on the detailed prompts.
