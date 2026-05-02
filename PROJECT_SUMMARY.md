# Project Summary: Desert Dash: Camel Runner

## Overview

"Desert Dash: Camel Runner" is a fully automated, AI-generated 2.5D endless runner game designed for mobile platforms (Android and iOS). The game features Kamil the Camel on a World Tour across 10 iconic countries, dodging obstacles, collecting treasures, and unlocking new destinations. The project was built using a comprehensive AI-driven pipeline, from initial market research and game design to asset generation, coding, and QA testing.

The core monetization strategy revolves around a "Player-Driven Rewarded Ads" model, ensuring a non-intrusive experience where players voluntarily watch ads to earn in-game currency, which can be spent on skins, power-ups, and unlocking new countries.

## Architecture & Scripts

The game is built in Unity using C# and follows a modular architecture. Here is a breakdown of the key scripts and their purposes:

### Core Gameplay
*   `PlayerController.cs`: Manages camel movement (3-lane switching, jumping, sliding).
*   `GameManager.cs`: Controls the overall game state (start, pause, game over, score tracking).
*   `LevelGenerator.cs`: Handles procedural, chunk-based level generation for endless gameplay.
*   `ObstacleSpawner.cs`: Spawns various obstacles (rocks, cacti, ruins) dynamically.
*   `CollectibleSpawner.cs`: Spawns collectibles (dates, coins, gems) along the lanes.

### Systems & Mechanics
*   `CollectibleSystem.cs`: Manages the collection of items, score calculation, and score-to-coin conversion.
*   `PowerUpManager.cs`: Handles the activation and effects of power-ups (Magic Carpet, Scarab Shell, etc.).
*   `ThiefSystem.cs`: Manages random thief encounters, their behavior, and coin stealing mechanics.
*   `HealthSystem.cs`: Implements the camel's health decay and recovery (natural and ad-based).
*   `MilestoneSystem.cs`: Manages checkpoints within levels for progress saving.
*   `WorldMapManager.cs`: Handles country data, unlock status, and the mini-map display.
*   `SkinManager.cs`: Manages unlockable camel skins and their functional benefits.

### Economy & Monetization
*   `CoinEconomy.cs`: Manages the player's coin wallet, offline spending, and online earning.
*   `AdManager.cs`: Handles the logic for tiered rewarded ads and health restoration ads.
*   `AdSDKIntegration.cs`: Integrates Google AdMob and Unity Ads SDKs.
*   `AdMediation.cs`: Sets up ironSource/LevelPlay for ad mediation.
*   `OfflineManager.cs`: Detects online/offline status and restricts ad-based earning when offline.

### UI & Polish
*   `UIManager.cs`: Manages all user interface elements (Main Menu, HUD, Shop, Game Over screen).
*   `AudioManager.cs`: Controls background music (country-specific) and sound effects.
*   `VisualEffects.cs`: Manages particle effects, screen overlays, and camera zooms.

### QA & Optimization
*   `GameplayTester.cs`: Automated script for simulating player input and testing mechanics.
*   `EconomyBalanceTester.cs`: Script for verifying the balance of coin earning and spending.
*   `PerformanceTester.cs`: Script for monitoring FPS and memory usage.
*   `AdTester.cs`: Script for simulating ad interactions and verifying rewards.
*   `PerformanceOptimizer.cs`: Outlines strategies for object pooling, LOD, and compression.
*   `DifficultyManager.cs`: Manages the dynamic increase in game difficulty over time.

### Architecture Diagram (Text-Based)

```text
[GameManager] <--> [UIManager]
      |                 |
      v                 v
[PlayerController]  [WorldMapManager]
      |                 |
      v                 v
[LevelGenerator]    [SkinManager]
      |                 |
      v                 v
[ObstacleSpawner]   [PowerUpManager]
      |                 |
      v                 v
[CollectibleSpawner] [ThiefSystem]
      |                 |
      v                 v
[CollectibleSystem] [HealthSystem]
      |                 |
      v                 v
[CoinEconomy] <---> [AdManager] <--> [AdSDKIntegration] / [AdMediation]
      |
      v
[OfflineManager]
```

## How to Open in Unity and Continue Development

1.  **Clone the Repository**: `git clone https://github.com/BlueSky330/DesertDash-CamelRunner.git`
2.  **Open Unity Hub**: Add the cloned folder as a new project.
3.  **Open Project**: Launch the project in the Unity Editor.
4.  **Scenes**: Navigate to `Assets/Scenes` to find the main scenes (MainMenu, Gameplay, GameOver).
5.  **Scripts**: All logic is located in `Assets/Scripts`.

## Using AI Coding Assistants (Jules / Claude Code)

This project is designed to be maintained and expanded using AI coding assistants.

*   **Jules (Google)**: As the primary coding agent integrated with GitHub, Jules can be used to review pull requests, suggest optimizations, and automatically generate boilerplate code for new features.
*   **Claude Code (Local WSL)**: Running locally on the client's always-on laptop, Claude Code acts as the backup agent. It can be triggered via a listener script to handle complex logic refactoring, debug specific issues, or generate new scripts based on updated GDD requirements.

To use them effectively:
1.  Provide clear, detailed prompts referencing specific scripts or mechanics in the GDD.
2.  Ask the AI to explain its changes before applying them.
3.  Use the automated testing scripts (`GameplayTester.cs`, etc.) to verify the AI's work.

## Adding More Countries

The World Tour system is designed for easy expansion. To add a new country:

1.  **Update `WorldMapManager.cs`**: Add the new country's data (name, unlock price, specific obstacles) to the country list.
2.  **Create Assets**: Generate new background art, specific obstacles, and background music for the new country using the AI prompts provided in Phase 2 and Phase 4.
3.  **Update UI**: Ensure the new country appears on the World Map selection screen.
4.  **Adjust Difficulty**: Use `DifficultyManager.cs` to fine-tune the challenge level for the new country.

## Estimated Revenue Projections

Revenue projections depend heavily on user acquisition and retention. Based on the "Player-Driven Rewarded Ads" model:

*   **Assumptions**:
    *   Average Play Session: 15 minutes.
    *   Ads Watched per Session: 2-3 (mix of Quick, Standard, and Premium).
    *   eCPM (Effective Cost Per Mille): $5 - $15 (varies by region, higher in US/Europe, lower in MENA).
*   **Calculation (Example)**:
    *   10,000 Daily Active Users (DAU).
    *   2.5 ads/user/day = 25,000 ad impressions/day.
    *   At $10 eCPM: (25,000 / 1000) * $10 = $250/day.
    *   Monthly Revenue: ~$7,500.
*   **Note**: These are rough estimates. Actual revenue will vary based on market penetration, player engagement, and ad network performance.

## Next Steps After Launch

1.  **Marketing & User Acquisition**: Launch targeted ad campaigns on social media (Facebook, Instagram, TikTok) focusing on the Egyptian and MENA markets initially, then expanding globally.
2.  **Monitor Analytics**: Use tools like Google Analytics for Firebase or Unity Analytics to track player behavior, retention rates, and economy balance.
3.  **Regular Updates**: Release updates every 2-4 weeks adding new countries, skins, or special events to keep players engaged.
4.  **Community Engagement**: Build a community on Discord or Reddit to gather feedback and build loyalty.
5.  **New Games**: Utilize the `AI-Game-Factory` pipeline to rapidly prototype and develop new games based on market trends.
