# Device Testing Procedures — M8 QA Test Phase

**Purpose:** Standardized testing procedures for device matrix validation  
**Scope:** 7 Android devices, automated + manual testing  
**Created:** 2026-05-07  
**QA Tester:** Claude Code

---

## Part 1: Testing Environment Setup

### Android Emulator Setup (Local Baseline Testing)

Required for testing without physical devices or cloud labs.

#### Prerequisites
- Android SDK Tools (API 30+)
- Android Emulator (included in SDK)
- Test APK file

#### Creating Device Emulators

```bash
# Create emulator AVDs matching target devices
# (Run from Android SDK root directory)

# 1. Galaxy A12 Baseline (720×1600, 3GB RAM, Android 11)
avdmanager create avd \
  -n "GalaxyA12_API30" \
  -k "system-images;android;30;default;armeabi-v7a" \
  -d "720x1600" \
  --ram 2048 \
  --sdcard 512M

# 2. Galaxy A51 (1080×2340, 4GB RAM, Android 10)
avdmanager create avd \
  -n "GalaxyA51_API29" \
  -k "system-images;android;29;default;x86" \
  -d "1080x2340" \
  --ram 2048 \
  --sdcard 512M

# 3. Pixel 4a (1080×2340, 6GB RAM, Android 12)
avdmanager create avd \
  -n "Pixel4a_API31" \
  -k "system-images;android;31;google_apis;x86" \
  -d "1080x2340" \
  --ram 2048 \
  --sdcard 512M

# 4. Redmi 9A (720×1600, 3GB RAM, Android 10)
avdmanager create avd \
  -n "Redmi9A_API29" \
  -k "system-images;android;29;default;x86" \
  -d "720x1600" \
  --ram 2048 \
  --sdcard 512M

# 5. Galaxy S10 Lite (720×1600, 3GB RAM, Android 11)
avdmanager create avd \
  -n "S10Lite_API30" \
  -k "system-images;android;30;default;armeabi-v7a" \
  -d "720x1600" \
  --ram 2048 \
  --sdcard 512M

# 6. OnePlus 8 (1080×2340, 8GB RAM, Android 11)
avdmanager create avd \
  -n "OnePlus8_API30" \
  -k "system-images;android;30;google_apis;x86" \
  -d "1080x2340" \
  --ram 2048 \
  --sdcard 512M

# 7. Moto G7 (720×1560, 4GB RAM, Android 9)
avdmanager create avd \
  -n "MotoG7_API28" \
  -k "system-images;android;28;default;x86" \
  -d "720x1560" \
  --ram 2048 \
  --sdcard 512M
```

#### Running Tests on Emulators

```bash
#!/bin/bash
# run_emulator_tests.sh

DEVICES=(
  "GalaxyA12_API30"
  "GalaxyA51_API29"
  "Pixel4a_API31"
  "Redmi9A_API29"
  "S10Lite_API30"
  "OnePlus8_API30"
  "MotoG7_API28"
)

APK_PATH="path/to/DesertDash-CamelRunner.apk"
RESULTS_DIR="test_results"

mkdir -p "$RESULTS_DIR"

for device in "${DEVICES[@]}"; do
  echo "Testing on $device..."
  
  # Start emulator
  emulator -avd "$device" -no-window &
  EMULATOR_PID=$!
  
  # Wait for emulator to be ready
  adb wait-for-device shell 'while [[ -z $(getprop sys.boot_completed) ]]; do sleep 1; done;'
  
  # Install APK
  adb install -r "$APK_PATH"
  
  # Run test scenario (see Part 2)
  # ... test execution ...
  
  # Capture device info
  adb shell dumpsys meminfo > "$RESULTS_DIR/${device}_meminfo.txt"
  adb shell getprop > "$RESULTS_DIR/${device}_props.txt"
  
  # Stop emulator
  kill $EMULATOR_PID
  
  echo "✓ Completed testing on $device"
  sleep 5
done

echo "All emulator tests completed. Results in $RESULTS_DIR"
```

---

### Firebase Test Lab Setup (Cloud-based Testing)

For automated testing on real devices (recommended for final validation).

#### Prerequisites
- Google Cloud Project
- Firebase Test Lab enabled
- Service account with `firebase.testlab.runner` IAM role
- gcloud CLI configured

#### Configuration

```bash
#!/bin/bash
# setup_firebase_test_lab.sh

PROJECT_ID="aig-desert-dash-project"  # Replace with actual project
SERVICE_ACCOUNT="firebase-test-lab@${PROJECT_ID}.iam.gserviceaccount.com"

# 1. Create service account (one-time)
gcloud iam service-accounts create firebase-test-lab \
  --project="$PROJECT_ID" \
  --display-name="Firebase Test Lab Runner"

# 2. Grant permissions
gcloud projects add-iam-policy-binding "$PROJECT_ID" \
  --member="serviceAccount:${SERVICE_ACCOUNT}" \
  --role="roles/firebase.testlab.runner"

# 3. Create and download key
gcloud iam service-accounts keys create ~/firebase-test-lab-key.json \
  --iam-account="$SERVICE_ACCOUNT" \
  --project="$PROJECT_ID"

# 4. Activate service account
export GOOGLE_APPLICATION_CREDENTIALS=~/firebase-test-lab-key.json
gcloud auth activate-service-account --key-file="$GOOGLE_APPLICATION_CREDENTIALS"

echo "✓ Firebase Test Lab configured. Key at: $GOOGLE_APPLICATION_CREDENTIALS"
```

#### Running Tests on Firebase Test Lab

```bash
#!/bin/bash
# run_firebase_tests.sh

PROJECT_ID="aig-desert-dash-project"
APP_APK="path/to/DesertDash-CamelRunner.apk"
TEST_APK="path/to/DesertDash-CamelRunner-test.apk"  # If using instrumentation tests
RESULTS_BUCKET="gs://${PROJECT_ID}-test-results"

# Device configurations matching target matrix
DEVICES=(
  "model=SamsungGalaxyA12,version=30,locale=en,orientation=portrait"
  "model=SamsungGalaxyA51,version=29,locale=en,orientation=portrait"
  "model=Pixel4a,version=31,locale=en,orientation=portrait"
  "model=RedmiNote9A,version=29,locale=en,orientation=portrait"
  "model=SamsungGalaxyS10,version=30,locale=en,orientation=portrait"
  "model=OnePlus8,version=30,locale=en,orientation=portrait"
  "model=MotoG7,version=28,locale=en,orientation=portrait"
)

for device_config in "${DEVICES[@]}"; do
  echo "Running test on: $device_config"
  
  gcloud firebase test android run \
    --type=instrumentation \
    --app="$APP_APK" \
    --test="$TEST_APK" \
    --device="$device_config" \
    --timeout=900s \
    --num-flaky-test-attempts=3 \
    --project="$PROJECT_ID" \
    --results-dir="${RESULTS_BUCKET}/$(date +%s)"
done

echo "✓ Firebase Test Lab tests queued. Check results at: $RESULTS_BUCKET"
```

---

## Part 2: Manual Test Execution Procedures

### Test Case 1: Launch & Navigation (5 min)

**Objective:** Verify game starts without crash and UI is navigable

**Steps:**
1. Open device (or emulator)
2. Install APK: `adb install -r path/to/apk`
3. Launch app from home screen or: `adb shell am start -n com.skyvision.desertdash/.MainActivity`
4. Observe for crashes or boot errors (watch logcat: `adb logcat | grep "FATAL\|ERROR"`)
5. Verify main menu displays
6. Tap each button (Play, Settings, Shop, LeaderBoard) and verify navigation

**Pass Criteria:**
- ✅ App launches without ANR (Application Not Responding)
- ✅ Main menu visible with all UI elements
- ✅ All buttons responsive (no missed taps)
- ✅ No visual glitches (overlapping text, cut-off elements)

**Failure Symptoms:**
- ❌ App crashes on launch
- ❌ ANR or 5+ second freeze
- ❌ Black screen or corrupted display
- ❌ Buttons not responding to taps

---

### Test Case 2: Gameplay Mechanics (15 min)

**Objective:** Validate core game loop on target device

**Setup:** On main menu, tap "Play"

**Steps:**

1. **Level Load (1 min)**
   - Observe Egypt level loading screen
   - Verify level assets display (background, terrain, obstacles)
   - Confirm FPS counter (if visible) shows ~60 FPS

2. **Player Movement (3 min)**
   - Swipe left: player moves to left lane
   - Swipe right: player moves to right lane
   - Swipe up: player jumps
   - Swipe down: player slides
   - Verify input latency < 50ms (responsive feel)

3. **Obstacle Collision (5 min)**
   - Allow game to run 2-3 minutes
   - Observe obstacle spawn and collision behavior
   - Verify health bar decreases on collision
   - Check FPS dip when collision occurs (should stay ≥ 55 FPS)

4. **Collectible Pickup (3 min)**
   - Move player over coins/jewels
   - Verify score increases
   - Check coin pickup sound plays
   - Confirm collectible disappears after pickup

5. **Power-up Activation (3 min)**
   - Let game run until power-up appears
   - Collect power-up
   - Verify power-up effect (shield, speed boost, etc.)
   - Check duration and expiration

**Pass Criteria:**
- ✅ Movement responsive (input lag < 50ms)
- ✅ Obstacles spawn continuously
- ✅ Health decreases on collision
- ✅ Score increases on collectible pickup
- ✅ FPS ≥ 55 during all actions
- ✅ No crashes or freezes

**Performance Metrics (Capture):**
- Avg FPS: ___
- Min FPS: ___
- Max FPS: ___
- Memory usage: ___ MB

---

### Test Case 3: Ad System (5 min)

**Objective:** Verify ads load and reward system works

**Setup:** In game, lose all health or play until Game Over screen

**Steps:**
1. On Game Over screen, look for ad offer button
2. Tap "Watch Ad" or similar
3. Observe test ad loads
4. Wait for ad to complete (or tap close)
5. Verify reward callback fires (check console logs)
6. Verify game resumes properly

**Pass Criteria:**
- ✅ Ad dialog appears
- ✅ Ad loads and plays
- ✅ Close button responsive
- ✅ Reward granted (coin addition, health restore)
- ✅ Game continues without crash

---

### Test Case 4: Edge Cases (10 min)

**Objective:** Stress test recovery and state management

**Steps:**

1. **Pause/Resume (2 min)**
   - During gameplay, pause (button or system menu)
   - Verify game pauses and UI shows pause screen
   - Resume
   - Verify game continues from same state

2. **Game Over → Restart (2 min)**
   - Play until Game Over
   - Tap Restart button
   - Verify new game loads cleanly
   - Verify score/health reset

3. **Settings Menu (2 min)**
   - Pause game, open Settings
   - Toggle music/SFX on/off
   - Apply changes
   - Resume game and verify changes applied

4. **Device Orientation (2 min)**
   - During gameplay, rotate device 90°
   - Verify UI adapts (portrait → landscape or vice versa)
   - Verify gameplay continues (no freeze)

5. **Low Memory Simulation (2 min)**
   - Open device settings → Developer Options
   - Enable "Limit memory" to minimum
   - Play for 2 minutes
   - Verify no crash (acceptable FPS dip ok)

**Pass Criteria:**
- ✅ Pause/resume maintains game state
- ✅ Restart clears previous session
- ✅ Settings persist after resume
- ✅ Orientation rotation handled gracefully
- ✅ Low memory doesn't crash app

---

## Part 3: Logging & Reporting

### Per-Device Test Report Template

```markdown
# Device Test Report: [Device Name / OS]

**Date:** YYYY-MM-DD  
**Tester:** Claude Code  
**Device:** Samsung Galaxy A12 / Android 11  
**APK Version:** v1.0.0 Build 42  

## Test Results

### Launch & Navigation
- [ ] App launches without crash
- [ ] Main menu responsive
- [ ] All buttons clickable
- **Status:** ✅ PASS

### Gameplay Mechanics
- [ ] Movement responsive
- [ ] Obstacles spawn correctly
- [ ] Collectibles pickup works
- [ ] Health system functional
- [ ] Power-ups work correctly
- **Status:** ✅ PASS

### Performance
- Avg FPS: 59
- Min FPS: 54
- Max FPS: 61
- Memory Peak: 285 MB
- **Status:** ✅ PASS (meets 60 FPS target)

### Ad System
- [ ] Ads load
- [ ] Reward callback fires
- [ ] Game resumes cleanly
- **Status:** ✅ PASS

### Edge Cases
- [ ] Pause/resume works
- [ ] Restart clears state
- [ ] Settings persist
- [ ] Orientation handled
- **Status:** ✅ PASS

## Issues Found

| ID | Title | Severity | Status |
|----|-------|----------|--------|
| BUG-001 | Text overlap on Shop button (720p only) | P2 | Reported |

## Screenshots

- [Main Menu](screenshots/galaxy-a12-menu.png)
- [Gameplay](screenshots/galaxy-a12-gameplay.png)
- [Game Over](screenshots/galaxy-a12-game-over.png)

## Conclusion

✅ **Device passes QA criteria.** Game is stable, performant, and playable on Galaxy A12.

Recommended for green light (no blockers).
```

---

## Part 4: Issue Logging Protocol

When a bug is found during testing:

1. **Immediately pause testing** on that device/test case
2. **Document the issue** using template below
3. **Create child issue** in Paperclip (AIG-XXX)
4. **Include reproduction steps** for developer
5. **Continue testing** (unless P0 blocker)

### Bug Report Template

```markdown
## Title: [P0/P1/P2] Brief description

**Device/OS:** Samsung Galaxy A12 / Android 11

**APK Version:** v1.0.0 Build 42

**Frequency:** 100% reproducible / Intermittent (~X% attempts)

**Severity:** 
- P0: Game crash, soft lock, unplayable
- P1: Major feature broken (movement, ads, health)
- P2: UI glitch, minor, edge case

**Steps to Reproduce:**
1. Load app
2. Navigate to [location]
3. Perform [action]
4. Observe [bug]

**Expected Behavior:**
[What should happen]

**Actual Behavior:**
[What actually happens]

**Screenshot/Video:**
[Attachment: screenshot or logcat output]

**Environment:**
- Device: Galaxy A12
- OS: Android 11
- RAM: 3GB
- APK: DesertDash v1.0.0 Build 42
- Network: [WiFi/Cellular/Offline]
- Battery: [X% remaining]
```

---

## Part 5: Success Criteria Summary

**Device Matrix Testing PASS** when:
- ✅ All 7 devices tested
- ✅ Game launches and is playable on each
- ✅ Performance ≥ 55 FPS on all devices
- ✅ 0 P0 bugs remaining
- ✅ All P1 bugs documented and assigned for fix
- ✅ Comprehensive report generated

**Device Matrix Testing CONDITIONAL** when:
- ⚠️ 6/7 devices pass (1 device fails due to environmental issues)
- ⚠️ Minor P2 issues documented as known issues
- ⚠️ Performance slightly below target but acceptable (≥ 45 FPS)

**Device Matrix Testing FAIL** when:
- ❌ Game unplayable on 2+ devices
- ❌ P0 or P1 bugs present
- ❌ Performance < 45 FPS
- ❌ Incomplete testing on target devices

---

**Next Step:** Once APK is available, execute this procedure on each device and collect results in `DEVICE_TESTING_RESULTS_M8.md`
