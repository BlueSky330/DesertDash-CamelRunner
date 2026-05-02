# Game Design Document: Desert Dash: Camel Runner

## 1. Game Overview

*   **Game Name**: Desert Dash: Camel Runner
*   **Genre**: Endless Runner / Hyper-Casual
*   **Platform**: Mobile (Android primary, iOS secondary)
*   **Target Audience**: Casual gamers, particularly in the Egyptian/MENA market, looking for quick, engaging gameplay sessions. Suitable for all ages.

## 2. Story/Theme

The game is set in a stylized, vibrant Egyptian desert. The protagonist is a charming, slightly goofy camel named "Kamil." Kamil has a penchant for sweet dates and has accidentally stumbled into a massive sandstorm while foraging. The goal is to run as far as possible to escape the encroaching sandstorm, collecting dates and dodging desert hazards along the way. The tone is lighthearted, fast-paced, and visually appealing.

## 3. Core Gameplay Mechanics

*   **Endless Runner**: The camel runs forward automatically. The player's objective is to survive as long as possible to achieve a high score.
*   **Controls**: Simple swipe controls:
    *   **Swipe Left/Right**: Change lanes (3-lane system).
    *   **Swipe Up**: Jump over low obstacles.
    *   **Swipe Down**: Slide under high obstacles.
*   **Obstacles**: Various desert-themed hazards that end the run upon collision.
*   **Collectibles**: Items scattered across the lanes that provide points or currency.
*   **Power-ups**: Temporary boosts that aid survival or increase score.

## 4. Level Design

*   **Progression**: The game features a single, endless level that becomes progressively more difficult.
*   **Difficulty Curve**: The running speed gradually increases over time. The density and complexity of obstacle patterns also increase.
*   **Procedural Generation**: The environment and obstacle placements are procedurally generated using pre-designed "chunks" or segments to ensure each run feels unique while maintaining a balanced difficulty.
*   **Environments/Zones**: While endless, the background scenery subtly shifts to provide visual variety (e.g., from sandy dunes to rocky canyons, to an oasis area).

## 5. Characters

*   **Main Character**: Kamil the Camel (default skin).
*   **Unlockable Skins/Characters**:
    *   "Pharaoh Kamil" (wearing a headdress).
    *   "Racing Camel" (sleek, wearing racing gear).
    *   "Mummy Camel" (wrapped in bandages).
    *   "Golden Camel" (premium skin).

## 6. Obstacles and Hazards

*   **Rocks/Boulders**: Stationary obstacles requiring a lane change or jump.
*   **Cacti**: Stationary obstacles; some might be tall requiring a slide, others short requiring a jump.
*   **Scorpions**: Moving obstacles that might scurry across a lane.
*   **Ruins/Pillars**: Large obstacles that block entire lanes.
*   **Sand Twisters**: Temporary hazards that obscure vision or push the camel to another lane.

## 7. Power-ups

*   **Speed Boost (Magic Carpet)**: Temporarily increases speed and makes the camel invincible to obstacles.
*   **Shield (Scarab Shell)**: Protects the camel from one collision.
*   **Magnet (Oasis Breeze)**: Automatically attracts all nearby dates and coins for a short duration.
*   **Double Coins (Golden Scarab)**: Doubles the value of all collected coins for a limited time.

## 8. Collectibles

*   **Dates**: The primary collectible, acting as the basic score multiplier and minor currency.
*   **Coins**: The main in-game currency used for purchasing upgrades and standard skins.
*   **Gems**: Premium currency (rarely found in-game, mostly purchased) used for exclusive skins and revives.

## 9. Monetization Strategy: Player-Driven Rewarded Ads (Online/Offline System)

The game employs a "Player-Driven Rewarded Ads" monetization model, ensuring an uninterrupted player experience while providing ample opportunities for engagement and revenue generation. Forced interstitial ads are entirely removed.

### Key Design Principles
*   **No Forced Ads**: Players are never interrupted by ads during gameplay.
*   **Play > Watch**: Players should always feel like they are playing more than watching. The ratio should be at least 10:1 (10 minutes of play for every 1 minute of ads).
*   **Generous Rewards**: Rewards must be meaningful enough that players feel watching the ad is highly worthwhile.
*   **Addictive Loop**: The goal is to make players want to keep playing, using ads as a tool to extend their playtime and enjoyment.
*   **Strict Completion**: If a player skips an ad at any point (even the last second), they receive NOTHING.
*   **Starter Bonus**: Players begin with 500 free coins, providing enough currency for the first 10-15 minutes of play before needing to consider ads.

### "Watch & Earn" Tiered System
The reward from watching ads provides meaningful play time, not just a few coins.

| Tier | Ad Duration | Reward | Play Time Earned |
| :--- | :--- | :--- | :--- |
| **Quick Watch** | ~15 seconds | Coins x3 multiplier (enough for ~3 minutes of play) | ~3 minutes |
| **Standard Watch** | ~30 seconds | Coins x4 multiplier (enough for ~5-7 minutes of play) | ~5-7 minutes |
| **Premium Watch** | ~60 seconds (or 2 ads back-to-back) | Coins x4 multiplier + bonus power-up (enough for ~7-10 minutes of play) | ~7-10 minutes |

### Coin Economy & Play Time Estimates
To ensure the play time estimates are accurate, the coin economy is structured as follows:
*   **Average Coin Spend**: Players spend approximately 30-50 coins per minute of gameplay (on revives, pre-game boosts, or saving for skins).
*   **Quick Watch Reward**: Yields ~100-150 coins (30-50 coins/min * 3 mins).
*   **Standard Watch Reward**: Yields ~200-300 coins (30-50 coins/min * 5-7 mins).
*   **Premium Watch Reward**: Yields ~300-400 coins + a free power-up (valued at ~100 coins).
*   **Coin-Based Economy**: All in-game purchases (skins, power-ups, revives) are bought using in-game coins.

### Online vs. Offline Coin System

**Online Mode**:
*   Players watch ads to earn coins, which are immediately saved to their balance.
*   The "Watch & Earn" menu is available with tiered rewards (Quick/Standard/Premium).
*   Coins accumulate in the player's wallet, allowing them to play and earn simultaneously.

**Offline Mode**:
*   Players can continue to play the game using their saved coin balance.
*   Coins are spent during gameplay for power-ups, revives, and other premium features.
*   Players CANNOT earn new coins while offline (no ads are available).
*   When the coin balance runs out, a message will prompt the player: "Go online to earn more coins!"
*   Players can still enjoy the basic game for free (running and collecting dates for score) but will be unable to use coin-dependent premium features without an internet connection.

**The Addiction Loop**:
This system creates a healthy engagement loop:
1.  **Online**: Players watch ads to earn coins.
2.  **Offline**: Players use their earned coins to play and spend them on in-game features.
3.  **Run out of Coins**: When coins are depleted, players are encouraged to go back online.
4.  **Repeat**: The cycle continues, driving sustained engagement and ad revenue.

### In-App Purchases (IAP)
Remain as an option for players who prefer to purchase coins or exclusive items directly, bypassing ad viewing.
*   **Coin Packs (Pricing in EGP - Estimated)**:
    *   Small Pouch: 20 EGP
    *   Large Sack: 50 EGP
    *   Treasure Chest: 150 EGP
*   **Gem Packs (Pricing in EGP - Estimated)**:
    *   Handful of Gems: 30 EGP
    *   Box of Gems: 80 EGP
    *   Vault of Gems: 250 EGP
*   **"No Ads" Premium Pass**: 100 EGP (Removes all rewarded ad options, providing a premium, ad-free experience).
*   **Exclusive Skins**: Ranging from 50 EGP to 150 EGP.
*   **Weekly Challenges/Passes**: A "Desert Explorer Pass" (approx. 80 EGP/month) offering daily login bonuses, exclusive weekly challenges with high rewards, and a unique monthly skin.

**Recommended Ad Networks**:
*   **Primary**: Google AdMob
*   **Secondary**: Unity Ads
*   **Mediation**: ironSource/LevelPlay (for optimizing ad revenue across multiple networks)

## 10. UI/UX Design

*   **Main Menu**: Clean, vibrant interface featuring the selected camel character. Buttons for "Play," "Shop," "Characters," and "Settings."
*   **Gameplay HUD**: Minimalist design. Score/Distance in the top center, collected coins/dates in the top right, active power-up icons with timers on the side.
*   **Shop**: Easy-to-navigate grid layout for purchasing upgrades, skins, and currency.
*   **Settings**: Options to toggle music, sound effects, and restore purchases.

## 11. Audio Design

*   **Background Music**: Upbeat, fast-paced track incorporating traditional Middle Eastern instruments (oud, darbuka) with a modern, energetic electronic beat.
*   **Sound Effects**:
    *   **Jump/Slide**: Light, cartoonish "whoosh" sounds.
    *   **Collect Dates/Coins**: Satisfying "ding" or "pop" sounds.
    *   **Power-up Activation**: Magical, echoing chime.
    *   **Collision**: A comical "thud" followed by a disappointed camel grunt.
    *   **UI Clicks**: Crisp, sandy "click" sounds.

## 12. Art Style

*   **Visual Direction**: Stylized, low-poly 3D or vibrant, high-resolution 2D vector art. The aesthetic should be colorful, inviting, and slightly cartoonish.
*   **Color Palette**: Warm, sun-drenched colors. Dominant hues of gold, orange, and terracotta for the sand, contrasted with bright blues for the sky and lush greens for oasis elements.
*   **Theme**: Egyptian desert, incorporating stylized elements like pyramids in the background, ancient ruins, and desert flora.

## 13. Technical Requirements

*   **Engine**: Unity (ideal for mobile 3D/2D and AI integration).
*   **Target Devices**: Broad compatibility. Optimized for mid-range Android devices (e.g., equivalent to Samsung Galaxy A series) and iPhone 8 or newer.
*   **Performance Targets**: Consistent 60 FPS on target devices to ensure smooth swipe controls and reaction times.

## 14. Development Timeline (8-12 Days)

This timeline assumes the use of the fully automated AI pipeline (Jules, Claude Code, Midjourney, etc.).

*   **Day 1: Project Setup & Core Mechanics**: Initialize Unity project, implement basic endless runner controller (3-lane movement, jump, slide).
*   **Day 2: Procedural Generation**: Develop the system for spawning level chunks and basic obstacles.
*   **Day 3: Asset Generation (Visuals)**: AI generates 3D models/2D sprites for the camel, basic obstacles (rocks, cacti), and environment textures.
*   **Day 4: Asset Integration & Animation**: Import visual assets into Unity, apply basic animations (running, jumping, sliding, collision).
*   **Day 5: Collectibles & Power-ups**: Implement dates, coins, and the logic for the four core power-ups.
*   **Day 6: UI/UX Implementation**: Build the main menu, gameplay HUD, and game over screens.
*   **Day 7: Audio Generation & Integration**: AI generates background music and sound effects; integrate them into the game events.
*   **Day 8: Monetization Integration**: Implement ad SDKs (e.g., Unity Ads, AdMob) for interstitials and rewarded videos. Set up basic IAP structure.
*   **Day 9: QA & Balancing**: Automated AI testing for bugs, performance issues, and difficulty balancing (adjusting speed curve and obstacle density).
*   **Day 10: Polish & Refinement**: Address QA findings, add visual polish (particle effects for sand, power-ups), and finalize UI.
*   **Day 11: Client Review & Final Tweaks**: Present the near-final build to the client, implement any minor requested changes.
*   **Day 12: App Store Preparation**: Generate store assets (icons, screenshots, descriptions) using AI, prepare final builds for Android and iOS submission.
