# QA Test Cases Reference — Desert Dash: Camel Runner

**Document:** Comprehensive manual test case reference for M8 QA phase  
**Status:** Ready for device testing  
**Last Updated:** 2026-05-07  
**Audience:** QA Engineers, Device Testing Teams, Beta Testers

---

## Overview

This document provides a comprehensive list of manual test cases organized by game feature. These test cases support device testing, manual validation, and regression testing across all platforms (Android, iOS).

**Usage:**
- Use during device testing to verify each feature works correctly
- Check off items as you test them
- Report failures in the issue tracker with reference to test case ID
- Re-run test cases after bug fixes to verify resolution

---

## 1. Core Gameplay Mechanics

### 1.1 Player Movement & Lane Changing

**TC-1.1.1: Swipe Left Lane Change**
- Precondition: Game running, player in center lane
- Action: Swipe left on screen
- Expected: Player moves to left lane smoothly
- Pass Criteria: [ ] Lane change completes in < 0.5s

**TC-1.1.2: Swipe Right Lane Change**
- Precondition: Game running, player in center lane
- Action: Swipe right on screen
- Expected: Player moves to right lane smoothly
- Pass Criteria: [ ] Lane change completes in < 0.5s

**TC-1.1.3: Lane Change Cooldown**
- Precondition: Game running
- Action: Swipe left, then immediately swipe right
- Expected: Second swipe rejected until cooldown expires
- Pass Criteria: [ ] 200ms cooldown enforced

**TC-1.1.4: Multiple Lane Changes**
- Precondition: Game running
- Action: Alternate swipes left and right 5 times
- Expected: Player moves smoothly between lanes without glitches
- Pass Criteria: [ ] All moves successful, no stuck state

**TC-1.1.5: Lane Boundary Validation**
- Precondition: Player in left lane
- Action: Swipe left again
- Expected: No movement (already at boundary)
- Pass Criteria: [ ] Player stays in left lane

---

### 1.2 Jump Mechanics

**TC-1.2.1: Swipe Up Jump**
- Precondition: Game running, player on ground
- Action: Swipe up on screen
- Expected: Player jumps and lands after brief airtime
- Pass Criteria: [ ] Jump height appropriate (~0.5s airtime)

**TC-1.2.2: Jump Over Low Obstacle**
- Precondition: Game running, low obstacle approaching in player's lane
- Action: Swipe up to jump over obstacle
- Expected: Player clears obstacle without collision
- Pass Criteria: [ ] No collision damage taken

**TC-1.2.3: Jump Cooldown**
- Precondition: Game running
- Action: Jump, then immediately swipe up again
- Expected: Second jump rejected until cooldown expires
- Pass Criteria: [ ] 400ms cooldown enforced

**TC-1.2.4: Jump While Lane Changing**
- Precondition: Game running
- Action: Swipe left then quickly swipe up
- Expected: Both actions queue, player moves and jumps
- Pass Criteria: [ ] Movement and jump both execute

---

### 1.3 Slide Mechanics

**TC-1.3.1: Swipe Down Slide**
- Precondition: Game running, player on ground
- Action: Swipe down on screen
- Expected: Player slides and lowers profile for 0.8s
- Pass Criteria: [ ] Slide duration correct (~0.8s)

**TC-1.3.2: Slide Under Tall Obstacle**
- Precondition: Game running, tall obstacle approaching in player's lane
- Action: Swipe down to slide under obstacle
- Expected: Player slides under obstacle without collision
- Pass Criteria: [ ] No collision damage taken

**TC-1.3.3: Slide Duration**
- Precondition: Game running, player sliding
- Action: Observe slide duration
- Expected: Slide lasts approximately 0.8 seconds
- Pass Criteria: [ ] Duration matches specification

---

### 1.4 Continuous Forward Movement

**TC-1.4.1: Automatic Forward Movement**
- Precondition: Game running
- Action: Don't input any swipes, observe player
- Expected: Player moves forward automatically
- Pass Criteria: [ ] Movement continuous and smooth

**TC-1.4.2: Speed Increase with Level Progression**
- Precondition: Game running for 30+ seconds
- Action: Observe movement speed over time
- Expected: Speed gradually increases with progression
- Pass Criteria: [ ] Speed increases smoothly (no sudden jumps)

**TC-1.4.3: Speed Consistency**
- Precondition: Game running
- Action: Measure distance covered in 10 seconds
- Expected: Consistent distance per time
- Pass Criteria: [ ] Speed variation < 5%

---

## 2. Obstacle System

### 2.1 Obstacle Types & Collision

**TC-2.1.1: Rock/Boulder Collision**
- Precondition: Game running, rock approaching in player's lane
- Action: Don't avoid, collide with rock
- Expected: Player loses health/game over
- Pass Criteria: [ ] Correct damage applied

**TC-2.1.2: Cactus Jump Over**
- Precondition: Game running, small cactus in lane
- Action: Jump over cactus
- Expected: Player clears obstacle
- Pass Criteria: [ ] No collision, health preserved

**TC-2.1.3: Tall Obstacle Slide Under**
- Precondition: Game running, tall obstacle in lane
- Action: Slide under obstacle
- Expected: Player passes under safely
- Pass Criteria: [ ] No collision, health preserved

**TC-2.1.4: Scorpion Moving Obstacle**
- Precondition: Game running, scorpion approaching
- Action: Collide with scorpion
- Expected: Damage taken
- Pass Criteria: [ ] Correct damage applied

**TC-2.1.5: Ruins/Pillar Blocking Lane**
- Precondition: Game running, large ruins blocking entire lane
- Action: Change lanes to avoid
- Expected: Player moves to adjacent lane safely
- Pass Criteria: [ ] Lane change successful before collision

---

### 2.2 Obstacle Variety & Density

**TC-2.2.1: Multiple Obstacles In View**
- Precondition: Game running for 20+ seconds (increased difficulty)
- Action: Observe obstacle count on screen
- Expected: Multiple obstacles visible simultaneously
- Pass Criteria: [ ] Density matches difficulty level

**TC-2.2.2: Obstacle Spacing**
- Precondition: Game running
- Action: Measure time between obstacles
- Expected: Adequate time to react (gap increases early, decreases late)
- Pass Criteria: [ ] Spacing feels fair and challenging

**TC-2.2.3: Mixed Obstacle Types**
- Precondition: Game running for 30+ seconds
- Action: Observe obstacle variety
- Expected: Mix of rocks, cacti, scorpions, ruins
- Pass Criteria: [ ] Variety present, not repetitive

---

## 3. Collectibles & Scoring

### 3.1 Collectible Types

**TC-3.1.1: Date Pickup (1 point)**
- Precondition: Game running, date visible in lane
- Action: Move into lane with date
- Expected: Date collected, score += 1
- Pass Criteria: [ ] Score incremented correctly

**TC-3.1.2: Silver Coin Pickup (3 points)**
- Precondition: Game running, silver coin in lane
- Action: Move into lane with coin
- Expected: Coin collected, score += 3
- Pass Criteria: [ ] Score incremented correctly

**TC-3.1.3: Gem Pickup (10 points)**
- Precondition: Game running, gem visible
- Action: Collect gem
- Expected: Score += 10
- Pass Criteria: [ ] High point value awarded

**TC-3.1.4: Golden Date Pickup (5 points)**
- Precondition: Game running, golden date appears briefly
- Action: Quickly move into lane to collect
- Expected: Score += 5
- Pass Criteria: [ ] Timed collectible works, score correct

**TC-3.1.5: Mystery Box Pickup (15-50 points)**
- Precondition: Game running, mystery box visible
- Action: Collect mystery box
- Expected: Random points (15-50) awarded
- Pass Criteria: [ ] Point value within range

---

### 3.2 Score Accumulation

**TC-3.2.1: Score Persistence**
- Precondition: Game running, score = 100
- Action: Play for 30 seconds, collect more items
- Expected: Score continues increasing
- Pass Criteria: [ ] Score accumulation continuous

**TC-3.2.2: Final Score Display**
- Precondition: Game over
- Action: Look at game over screen
- Expected: Final score displayed prominently
- Pass Criteria: [ ] Score visible and correct

**TC-3.2.3: High Score Tracking**
- Precondition: Complete game session with score > previous high
- Action: Complete run, check high score
- Expected: New high score saved
- Pass Criteria: [ ] High score updated (if applicable)

---

### 3.3 Score to Coins Conversion

**TC-3.3.1: Conversion Rate**
- Precondition: Game complete with score = 300
- Action: Check coins earned
- Expected: 300 score ÷ 150 = 2 coins
- Pass Criteria: [ ] Conversion accurate (150:1 ratio)

**TC-3.3.2: Fractional Coin Rounding**
- Precondition: Game complete with score = 160
- Action: Check coins earned
- Expected: 160 ÷ 150 = 1.067 → 1 coin (rounded down)
- Pass Criteria: [ ] Rounding correct

**TC-3.3.3: Coin Balance Update**
- Precondition: Start with 100 coins, play and earn
- Action: Complete game, earn coins
- Expected: Coin balance increases correctly
- Pass Criteria: [ ] Balance reflected in wallet

---

## 4. Power-ups

### 4.1 Speed Boost (Magic Carpet)

**TC-4.1.1: Speed Boost Activation**
- Precondition: Game running, speed boost approaching
- Action: Move into lane with boost
- Expected: Player movement speed increases, effect visible
- Pass Criteria: [ ] Speed increase apparent

**TC-4.1.2: Invincibility During Boost**
- Precondition: Speed boost active
- Action: Deliberately hit obstacle
- Expected: No damage taken, player unaffected
- Pass Criteria: [ ] Invincibility working

**TC-4.1.3: Boost Duration**
- Precondition: Speed boost active
- Action: Observe duration
- Expected: Effect lasts approximately 5-10 seconds
- Pass Criteria: [ ] Duration matches specification

**TC-4.1.4: Boost Expiration**
- Precondition: Speed boost expiring
- Action: Wait for boost to wear off
- Expected: Speed returns to normal, invincibility ends
- Pass Criteria: [ ] Normal state restored

---

### 4.2 Shield (Scarab Shell)

**TC-4.2.1: Shield Activation**
- Precondition: Game running, shield approaching
- Action: Collect shield
- Expected: Shield icon appears, indicating protection
- Pass Criteria: [ ] Shield visual indicator present

**TC-4.2.2: Collision Protection**
- Precondition: Shield active
- Action: Hit obstacle
- Expected: Shield absorbs damage, breaks, health preserved
- Pass Criteria: [ ] One collision negated

**TC-4.2.3: Shield Duration**
- Precondition: Shield collected
- Action: Play normally
- Expected: Shield active until next obstacle hit
- Pass Criteria: [ ] Shield persists correctly

**TC-4.2.4: Single-Use Confirmation**
- Precondition: Shield active, hit obstacle, shield breaks
- Action: Hit another obstacle
- Expected: Damage taken (shield already used)
- Pass Criteria: [ ] Shield one-time only

---

### 4.3 Magnet (Oasis Breeze)

**TC-4.3.1: Magnet Activation**
- Precondition: Game running, magnet approaching
- Action: Collect magnet
- Expected: Magnet icon appears, collectibles attract
- Pass Criteria: [ ] Auto-collection of nearby items visible

**TC-4.3.2: Auto-Collection of Dates**
- Precondition: Magnet active, dates scattered in lane
- Action: Don't move to dates, observe
- Expected: Dates automatically collected without movement
- Pass Criteria: [ ] Passive collection works

**TC-4.3.3: Auto-Collection of Coins**
- Precondition: Magnet active, coins visible
- Action: Observe coin collection
- Expected: Coins auto-collected
- Pass Criteria: [ ] Coin collection automatic

**TC-4.3.4: Magnet Duration**
- Precondition: Magnet active
- Action: Play for 5-10 seconds
- Expected: Magnet effect lasts limited time
- Pass Criteria: [ ] Duration approximately 5-10 seconds

---

### 4.4 Double Coins (Golden Scarab)

**TC-4.4.1: Double Coin Activation**
- Precondition: Game running, double coin boost approaching
- Action: Collect double coin power-up
- Expected: Multiplier icon appears, next coins worth 2x
- Pass Criteria: [ ] Visual indicator present

**TC-4.4.2: Doubled Value Collection**
- Precondition: Double coin active, silver coin (3 points) visible
- Action: Collect coin
- Expected: Score += 6 (3 × 2)
- Pass Criteria: [ ] Multiplier correctly applied

**TC-4.4.3: Multiplier Duration**
- Precondition: Double coin active
- Action: Play for 5-10 seconds, collect items
- Expected: All items worth 2x during duration
- Pass Criteria: [ ] Multiplier lasts appropriate time

**TC-4.4.4: Multiplier Expiration**
- Precondition: Double coin expiring
- Action: Collect coin after expiration
- Expected: Normal value (no multiplier)
- Pass Criteria: [ ] Multiplier properly disabled

---

## 5. Health & Lives System

### 5.1 Health Management

**TC-5.1.1: Health Display**
- Precondition: Game running
- Action: Look at health bar
- Expected: Health bar visible showing current health
- Pass Criteria: [ ] Health UI clearly visible

**TC-5.1.2: Collision Damage**
- Precondition: Game running, player at full health
- Action: Hit obstacle without protection
- Expected: Health decreases (damage taken)
- Pass Criteria: [ ] Health loss visible

**TC-5.1.3: Multiple Collisions**
- Precondition: Game running
- Action: Hit obstacles multiple times
- Expected: Health decreases with each hit
- Pass Criteria: [ ] Cumulative damage working

**TC-5.1.4: Game Over at Zero Health**
- Precondition: Health depleted
- Action: Wait for final collision
- Expected: Game over screen appears
- Pass Criteria: [ ] Game ends correctly

---

### 5.2 Health Recovery

**TC-5.2.1: Natural Health Recovery**
- Precondition: Game running, health reduced
- Action: Play without taking damage for 30 seconds
- Expected: Health may recover naturally (if enabled)
- Pass Criteria: [ ] Recovery rate appropriate (if implemented)

**TC-5.2.2: Health Pack Item**
- Precondition: Game running, health pack visible
- Action: Collect health pack
- Expected: Health increases
- Pass Criteria: [ ] Health restored correctly

---

## 6. Game States & UI

### 6.1 Main Menu

**TC-6.1.1: Menu Display**
- Precondition: App launched
- Action: Wait for menu to appear
- Expected: Main menu visible with buttons
- Pass Criteria: [ ] All UI elements present

**TC-6.1.2: Start Game Button**
- Precondition: Main menu visible
- Action: Tap "Start Game" button
- Expected: Gameplay begins, game scene loads
- Pass Criteria: [ ] Game starts successfully

**TC-6.1.3: Settings Button**
- Precondition: Main menu visible
- Action: Tap "Settings" button
- Expected: Settings menu appears
- Pass Criteria: [ ] Settings accessible

**TC-6.1.4: Shop Button**
- Precondition: Main menu visible
- Action: Tap "Shop" button
- Expected: Shop/unlock screen appears
- Pass Criteria: [ ] Shop opens

---

### 6.2 Gameplay UI

**TC-6.2.1: Score Display During Game**
- Precondition: Game running
- Action: Observe score counter
- Expected: Score displayed and updates with collectibles
- Pass Criteria: [ ] Score visible and accurate

**TC-6.2.2: Health Bar Display**
- Precondition: Game running
- Action: Look at health bar
- Expected: Health bar shows current health
- Pass Criteria: [ ] Health clearly indicated

**TC-6.2.3: Mini-Map Display**
- Precondition: Game running
- Action: Look for mini-map
- Expected: Small world map visible in corner
- Pass Criteria: [ ] Mini-map shows progress

**TC-6.2.4: Pause Functionality**
- Precondition: Game running
- Action: Tap pause button
- Expected: Game pauses, pause menu appears
- Pass Criteria: [ ] Game stops, menu appears

**TC-6.2.5: Resume After Pause**
- Precondition: Game paused
- Action: Tap resume button
- Expected: Game continues from paused state
- Pass Criteria: [ ] Game resumes correctly

---

### 6.3 Game Over Screen

**TC-6.3.1: Game Over Display**
- Precondition: Game ended (health = 0)
- Action: Observe game over screen
- Expected: Screen shows final score, high score, restart button
- Pass Criteria: [ ] All information displayed

**TC-6.3.2: Restart Button**
- Precondition: Game over screen visible
- Action: Tap restart button
- Expected: New game begins
- Pass Criteria: [ ] Game resets and starts fresh

**TC-6.3.3: Score Comparison**
- Precondition: Game over screen visible
- Action: Compare final score to high score
- Expected: High score highlighted if beaten
- Pass Criteria: [ ] High score comparison clear

---

## 7. Economy System

### 7.1 Coin Economy

**TC-7.1.1: Starting Coins**
- Precondition: New account
- Action: Check coin balance
- Expected: 500 free starter coins
- Pass Criteria: [ ] Correct initial balance

**TC-7.1.2: Coin Earning**
- Precondition: Coin balance = 100
- Action: Play game, earn coins
- Expected: Coin balance increases
- Pass Criteria: [ ] Coins correctly added

**TC-7.1.3: Coin Spending on Revive**
- Precondition: Game over, coins available
- Action: Revive using coins
- Expected: Coins deducted, game continues
- Pass Criteria: [ ] Spending correct

**TC-7.1.4: Insufficient Coins**
- Precondition: Coin balance = 10, revive costs 50
- Action: Attempt revive
- Expected: Action blocked, ad prompt appears
- Pass Criteria: [ ] Insufficient funds handled

---

### 7.2 Unlock System

**TC-7.2.1: Country Lock Display**
- Precondition: Egypt level completed, world map visible
- Action: Look at other countries
- Expected: Countries show lock icons and unlock costs
- Pass Criteria: [ ] Lock status clear

**TC-7.2.2: Unlock with Sufficient Coins**
- Precondition: Coins = 600, Jordan unlock costs 500
- Action: Tap Jordan country
- Expected: Payment confirmed, country unlocked
- Pass Criteria: [ ] Unlock successful, coins deducted (100 remaining)

**TC-7.2.3: Unlock with Insufficient Coins**
- Precondition: Coins = 200, China unlock costs 1200
- Action: Tap China country
- Expected: "Not enough coins" prompt, ad suggestion
- Pass Criteria: [ ] Insufficient funds message displayed

**TC-7.2.4: Skin Unlock**
- Precondition: Egypt level, shop open, Pharaoh skin costs 300 coins
- Action: Unlock Pharaoh skin
- Expected: Skin purchased, coins deducted
- Pass Criteria: [ ] Transaction successful

**TC-7.2.5: Already Unlocked Content**
- Precondition: Pharaoh skin already unlocked
- Action: View shop
- Expected: Skin shows "Equipped" or "Owned" status
- Pass Criteria: [ ] Status correctly shown

---

### 7.3 Ad Reward System

**TC-7.3.1: Ad Menu Access**
- Precondition: Main menu visible
- Action: Tap "Watch Ads" or "Earn Coins"
- Expected: Ad reward menu appears with tiered options
- Pass Criteria: [ ] Menu displays correctly

**TC-7.3.2: Quick Watch (15 sec)**
- Precondition: Ad menu open, coins available for reward
- Action: Watch 15-second ad
- Expected: Coins awarded (3x multiplier worth)
- Pass Criteria: [ ] Correct reward given

**TC-7.3.3: Standard Watch (30 sec)**
- Precondition: Ad menu open
- Action: Watch 30-second ad
- Expected: Coins awarded (4x multiplier worth, ~200-300 coins)
- Pass Criteria: [ ] Correct reward given

**TC-7.3.4: Premium Watch (60 sec or 2 ads)**
- Precondition: Ad menu open
- Action: Watch premium ad(s)
- Expected: Coins + bonus power-up awarded
- Pass Criteria: [ ] Full reward given

**TC-7.3.5: Ad Skip Penalty**
- Precondition: Ad playing
- Action: Skip/close ad before completion
- Expected: No reward given
- Pass Criteria: [ ] No coins for skipped ad

**TC-7.3.6: Offline Mode Coin Blocking**
- Precondition: Device offline, coin balance depleted
- Action: Try to earn coins
- Expected: Prompt "Go online to earn more coins"
- Pass Criteria: [ ] Ad earning blocked offline

---

## 8. Difficulty & Progression

### 8.1 Speed Progression

**TC-8.1.1: Initial Speed**
- Precondition: Game start
- Action: Measure forward movement speed
- Expected: Slow initial speed for learning
- Pass Criteria: [ ] Speed appropriate for beginners

**TC-8.1.2: Speed Increase Over Time**
- Precondition: Game running 30+ seconds
- Action: Measure speed again
- Expected: Speed noticeably faster
- Pass Criteria: [ ] Smooth progression, no jumps

**TC-8.1.3: Speed Ceiling**
- Precondition: Game running 5+ minutes
- Action: Observe if speed continues increasing
- Expected: Speed reaches ceiling (max difficulty)
- Pass Criteria: [ ] Speed capped appropriately

---

### 8.2 Obstacle Density

**TC-8.2.1: Low Initial Density**
- Precondition: Game start
- Action: Count obstacles visible in first 20 seconds
- Expected: Few obstacles, easy to avoid
- Pass Criteria: [ ] Learning phase not overwhelming

**TC-8.2.2: Increasing Density**
- Precondition: Game running 30+ seconds
- Action: Count obstacles
- Expected: More obstacles, closer together
- Pass Criteria: [ ] Density increases smoothly

**TC-8.2.3: Max Density Challenge**
- Precondition: Game running 5+ minutes
- Action: Observe obstacle pattern
- Expected: Many obstacles, tight spacing, challenging
- Pass Criteria: [ ] Late-game challenging but fair

---

## 9. Thief System (Egypt Level)

### 9.1 Thief Spawning

**TC-9.1.1: Thief Appearance**
- Precondition: Egypt level, playing for 20+ seconds
- Action: Observe for thief appearance
- Expected: Thief appears ahead of player
- Pass Criteria: [ ] Thief spawns at correct position

**TC-9.1.2: Thief Types**
- Precondition: Playing Egypt level for 2+ minutes
- Action: Observe thief types
- Expected: Mix of DesertBandit, Ninja, Pirate, ShadowThief
- Pass Criteria: [ ] All 4 types appear

**TC-9.1.3: Thief Movement**
- Precondition: Thief visible
- Action: Observe thief behavior
- Expected: Thief runs/moves toward player
- Pass Criteria: [ ] Movement animated and visible

---

### 9.2 Thief Collision & Evasion

**TC-9.2.1: Thief Hit Detection**
- Precondition: Thief approaching, in player's lane
- Action: Don't avoid thief
- Expected: Collision detected, damage taken
- Pass Criteria: [ ] Thief causes damage

**TC-9.2.2: Thief Evasion - Lane Change**
- Precondition: Thief approaching in player's lane
- Action: Swipe to adjacent lane
- Expected: Player moves, avoids thief
- Pass Criteria: [ ] No collision, thief passes

**TC-9.2.3: Thief Evasion - Jump**
- Precondition: Thief approaching, jump available
- Action: Jump over thief
- Expected: Player clears thief
- Pass Criteria: [ ] No collision, jump successful

**TC-9.2.4: Thief Invincibility Window**
- Precondition: Speed boost active, thief approaching
- Action: Collide with thief during boost
- Expected: No damage (invincibility active)
- Pass Criteria: [ ] Invincibility protects from thieves

---

## 10. Settings & Preferences

### 10.1 Audio Settings

**TC-10.1.1: Music Toggle**
- Precondition: Settings menu open
- Action: Toggle music on/off
- Expected: Background music plays/stops
- Pass Criteria: [ ] Audio toggle works

**TC-10.1.2: SFX Toggle**
- Precondition: Settings menu open
- Action: Toggle sound effects
- Expected: Sound effects enabled/disabled
- Pass Criteria: [ ] SFX toggle works

**TC-10.1.3: Volume Control**
- Precondition: Settings menu, audio enabled
- Action: Adjust volume slider
- Expected: Volume changes proportionally
- Pass Criteria: [ ] Volume levels adjustable

---

### 10.2 Graphics Settings

**TC-10.2.1: Graphics Quality Selection**
- Precondition: Settings menu open
- Action: Select high/medium/low quality
- Expected: Graphics adjust immediately
- Pass Criteria: [ ] Quality setting changes

**TC-10.2.2: FPS Display Toggle**
- Precondition: Settings menu
- Action: Enable FPS counter
- Expected: FPS counter appears on screen
- Pass Criteria: [ ] FPS display toggleable

---

## 11. Performance & Stability

### 11.1 FPS & Frame Rate

**TC-11.1.1: Sustained 60 FPS**
- Precondition: Galaxy A12 device, game running
- Action: Monitor FPS for 1 minute
- Expected: FPS maintains ~60 (±5)
- Pass Criteria: [ ] Consistent 60 FPS

**TC-11.1.2: FPS During Heavy Action**
- Precondition: Game running, many obstacles + effects
- Action: Observe FPS when screen busy
- Expected: FPS stays ≥ 55
- Pass Criteria: [ ] Performance stable under load

**TC-11.1.3: No Frame Tearing**
- Precondition: Game running
- Action: Watch for visual tearing
- Expected: No visual tearing or artifacts
- Pass Criteria: [ ] Visually smooth, no tearing

---

### 11.2 Memory & Crashes

**TC-11.2.1: Memory Stability**
- Precondition: Game running for 10+ minutes
- Action: Monitor memory usage (via Profiler or device settings)
- Expected: Memory usage stable (not continuously growing)
- Pass Criteria: [ ] No memory leaks

**TC-11.2.2: No Crashes During Play**
- Precondition: Game running
- Action: Play continuously for 30+ minutes
- Expected: No crashes or freezes
- Pass Criteria: [ ] Stability verified

**TC-11.2.3: Resume After Pause - Memory**
- Precondition: Game paused, resumed multiple times
- Action: Pause/resume 5+ times
- Expected: Memory stable, no leaks
- Pass Criteria: [ ] State management clean

---

### 11.3 Recovery from Errors

**TC-11.3.1: Graceful Ad Failure**
- Precondition: Ad fails to load
- Action: Wait for timeout
- Expected: Graceful handling, option to retry or skip
- Pass Criteria: [ ] No crash, user can continue

**TC-11.3.2: Network Loss During Ad**
- Precondition: Network interrupted during ad
- Action: Observe behavior
- Expected: Graceful handling, no reward if incomplete
- Pass Criteria: [ ] Proper error handling

---

## 12. Cross-Platform Compatibility

### 12.1 Android Versions

**TC-12.1.1: Android 8 (API 26)**
- Precondition: Moto G7 (Android 9 baseline)
- Action: Run game normally
- Expected: Fully functional
- Pass Criteria: [ ] All features work

**TC-12.1.2: Android 9 (API 28)**
- Precondition: Moto G7 device
- Action: Full play session
- Expected: No compatibility issues
- Pass Criteria: [ ] Full compatibility

**TC-12.1.3: Android 11 (API 30)**
- Precondition: Galaxy A12 device
- Action: Full play session
- Expected: No compatibility issues
- Pass Criteria: [ ] Full compatibility

**TC-12.1.4: Android 12 (API 31)**
- Precondition: Pixel 4a device
- Action: Full play session
- Expected: No compatibility issues
- Pass Criteria: [ ] Full compatibility

---

### 12.2 Device Resolutions

**TC-12.2.1: 720×1600 Resolution (Galaxy A12)**
- Precondition: Galaxy A12, game running
- Action: Observe UI scaling and readability
- Expected: All text readable, UI properly scaled
- Pass Criteria: [ ] 720×1600 fully supported

**TC-12.2.2: 1080×2340 Resolution (Pixel 4a)**
- Precondition: Pixel 4a device
- Action: Observe UI and rendering quality
- Expected: High-quality rendering, crisp text
- Pass Criteria: [ ] 1080×2340 supported

**TC-12.2.3: Notch/Safe Area Handling**
- Precondition: Device with notch or cutout
- Action: Observe UI positioning
- Expected: UI avoids notch, properly positioned
- Pass Criteria: [ ] Safe area respected

---

## 13. Regression Test Checklist

Use this section to track regression testing after bug fixes.

### Essential Features (Always Retest After Any Fix)

- [ ] TC-1.1.1 — Lane change left
- [ ] TC-1.1.2 — Lane change right
- [ ] TC-1.2.1 — Jump
- [ ] TC-1.3.1 — Slide
- [ ] TC-2.1.1 — Obstacle collision
- [ ] TC-3.1.1 — Collectible pickup
- [ ] TC-4.1.1 — Speed boost
- [ ] TC-5.1.1 — Health display
- [ ] TC-6.2.4 — Pause/resume
- [ ] TC-11.2.1 — Memory stability

### After Economy Bug Fixes

- [ ] TC-3.3.1 — Score to coins conversion
- [ ] TC-7.1.2 — Coin earning
- [ ] TC-7.2.2 — Country unlock
- [ ] TC-7.3.2 — Ad rewards

### After Performance Optimization

- [ ] TC-11.1.1 — 60 FPS maintenance
- [ ] TC-11.1.2 — FPS under load
- [ ] TC-11.2.1 — Memory stability
- [ ] TC-11.2.2 — No crashes

### After Thief System Changes

- [ ] TC-9.1.1 — Thief spawn
- [ ] TC-9.1.2 — Thief types
- [ ] TC-9.2.1 — Thief collision
- [ ] TC-9.2.2 — Thief evasion

---

## 14. Test Result Tracking

### Test Session Template

```
**Test Date:** ____________
**Device:** ____________
**OS Version:** ____________
**Build Version:** ____________
**Tester Name:** ____________
**Session Duration:** ____________

### Results Summary
- Total Tests: [X]
- Passed: [X]
- Failed: [X]
- Blocked: [X]
- Pass Rate: [X]%

### Failed Tests (Detail Each)
1. Test ID: TC-X.X.X
   - Issue: [Describe failure]
   - Severity: P0/P1/P2/P3
   - Bug ID: [If created]

### Notes & Observations
[Any additional notes about gameplay, performance, or user experience]

### Signed Off By
Tester: ____________
Date: ____________
```

---

## Appendix: Test Case ID Legend

| Prefix | Category |
|--------|----------|
| TC-1.x | Core Gameplay |
| TC-2.x | Obstacles |
| TC-3.x | Collectibles & Scoring |
| TC-4.x | Power-ups |
| TC-5.x | Health System |
| TC-6.x | Game States & UI |
| TC-7.x | Economy |
| TC-8.x | Difficulty & Progression |
| TC-9.x | Thief System |
| TC-10.x | Settings |
| TC-11.x | Performance |
| TC-12.x | Cross-Platform |

---

## References

- GDD.md — Game Design Document
- M8_QA_EXECUTION_PROCEDURES.md — Testing framework
- QA_TEST_PLAN_M8.md — Automated test specifications
- DEVICE_TESTING_PROCEDURES.md — Device lab setup

---

**Document Version:** 1.0  
**Status:** Ready for Device Testing  
**Owner:** QA Team  
**Last Updated:** 2026-05-07
