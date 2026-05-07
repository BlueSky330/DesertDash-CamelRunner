# Test Execution Guide: ThiefSpawner Verification

**QA Tester:** Claude Code (Agent)  
**Test Date:** 2026-05-07  
**Target Issue:** [AIG-102](/AIG/issues/AIG-102)  
**Parent Issue:** [AIG-84](/AIG/issues/AIG-84)

---

## Quick Start: Automated Testing in Unity

### Step 1: Open Unity Editor

```bash
# Navigate to project directory
cd Assets/Scenes

# In Unity, open the ThiefSpawnerTest scene:
# File > Open Scene > Assets/Scenes/ThiefSpawnerTest.unity
```

**Note:** The scene will be created automatically via the setup utility if it doesn't exist.

### Step 2: Create Test Scene (if needed)

In the Unity Editor, go to:
```
Assets > Create Test Scenes > Thief Spawner Test Scene
```

This will:
- Create a new test scene
- Add Main Camera (positioned for mobile view)
- Add Ground Plane
- Create Player GameObject with PlayerController
- Add ThiefSpawner component
- Add ThiefSpawnerTestManager (with auto-run enabled)
- Configure lighting

### Step 3: Run Automated Tests

1. Press **Play** in the Unity editor
2. The tests will start automatically (ThiefSpawnerTestManager.autoStartTests = true)
3. Open the **Console** tab to view test output
4. Wait for completion (~15 seconds)

**Expected Console Output:**
```
[ThiefSpawnerTest] === Starting Thief Spawner Tests ===
[ThiefSpawnerTest] Test 1: Spawning all 4 types at AHEAD
[ThiefSpawner] Spawned DesertBandit at Ahead (0.0, 0.0, 15.0)
✓ PASS: Name check (DesertBandit)
✓ PASS: Animator exists (DesertBandit)
✓ PASS: Collider check (DesertBandit)
✓ PASS: Rigidbody kinematic (DesertBandit)
✓ PASS: Spawn position (DesertBandit) at Ahead
✓ PASS: Enemy tag (DesertBandit)
[ThiefSpawner] Spawned NinjaThief at Ahead (0.0, 0.0, 15.0)
... (Pirate, ShadowThief follow)
[ThiefSpawnerTest] Test 2: Spawning all 4 types at BEHIND
... (similar pattern)
[ThiefSpawnerTest] Test 3: Spawning all 4 types at SIDE
... (similar pattern)
[ThiefSpawnerTest] Test 4: Verifying animation transitions
[ThiefSpawnerTest] Test 4.5: Extended animation testing
[ThiefSpawnerTest] Test 5: Verifying collider detection
[ThiefSpawnerTest] === Test Summary ===
[ThiefSpawnerTest] Passed: 78
[ThiefSpawnerTest] Failed: 0
[ThiefSpawnerTest] Total: 78
```

### Step 4: Verify Results

Check Console for:
- [ ] All tests passed (Passed = 78, Failed = 0)
- [ ] No error messages or warnings
- [ ] Spawned thieves visible in Scene view
- [ ] All 4 thief types appear (DesertBandit, NinjaThief, Pirate, ShadowThief)

---

## Manual Testing (Alternative)

If you want to test manually without automation:

### 1. Disable Auto-Start
In the Inspector, select the **ThiefSpawnerTestManager** GameObject and uncheck:
```
Auto Start Tests = false
```

### 2. Manual Test Execution

#### Test 1: Verify Spawn Positions

```csharp
// In a script or MonoBehaviour, call:
ThiefSpawner.Instance.SpawnThief(ThiefSystem.ThiefType.DesertBandit, 
                                 ThiefSystem.ThiefSpawnPosition.Ahead);

// Check in Scene view:
// - Thief appears 15 units ahead of Player (Z = +15)
// - Position indicator shows correct offset
```

#### Test 2: Verify Components

Select a spawned thief in the Hierarchy and check the Inspector:

```
✓ Animator component exists
✓ CapsuleCollider exists (radius=0.35, height=1.8, isTrigger=true)
✓ Rigidbody exists (isKinematic=true)
✓ Tag = "Enemy"
✓ Has procedural mesh component (ProceduralDesertBanditMesh, etc.)
```

#### Test 3: Verify Collider Detection

```csharp
// Enable Physics Debug visualization:
// In Scene view: Gizmos > Colliders (checkbox)

// Manually spawn thieves and verify colliders overlap correctly:
ThiefSpawner.Instance.SpawnThief(ThiefSystem.ThiefType.NinjaThief,
                                 ThiefSystem.ThiefSpawnPosition.Side);

// In Scene view, should see capsule collider as outline
```

---

## Mobile Device Testing (Hardware Phase)

### Prerequisites

- Samsung Galaxy A12 (Android 11)
- USB cable for ADB connection
- Android SDK Tools installed on development machine
- APK build exported from Unity

### Step 1: Build APK

In Unity:
```
File > Build Settings
- Select Android platform
- Scenes: ThiefSpawnerTest (add to build)
- Build APK
```

**Build Configuration:**
```
Platform: Android
Target API: 33 (Android 13)
Minimum API: 26 (Android 8.0)
Architecture: ARM64
Rendering: OpenGL ES 3
VSync: enabled (60 FPS)
```

### Step 2: Connect Device & Deploy

```bash
# Check device is connected
adb devices

# Install APK
adb install -r path/to/DesertDash-ThiefSpawnerTest.apk

# Launch app
adb shell am start -n com.example.game/.ThiefSpawnerTestActivity

# Monitor logs
adb logcat | grep "ThiefSpawnerTest"
```

### Step 3: Device Test Execution

On the Samsung Galaxy A12:

1. **Launch the test app** — ThiefSpawnerTest scene auto-loads
2. **Observe screen:**
   - Main Camera view of the test scene
   - Ground plane visible
   - Player (camel) at center
   - Thieves spawn at different positions
3. **Check visual indicators:**
   - [ ] DesertBandit spawns ahead (visible on screen)
   - [ ] NinjaThief spawns ahead
   - [ ] Pirate spawns ahead
   - [ ] ShadowThief spawns ahead
   - [ ] All thieves have visible meshes and animations
   - [ ] No stuttering or frame drops during spawning
4. **Monitor performance:**
   - Target: 60 FPS (frame time ≤ 16.67 ms)
   - Watch for: smooth animation, no jank, responsive input

### Step 4: Enable Profiler (Device)

```bash
# On device, enable Developer Options:
# Settings > About phone > Build number (tap 7 times)
# Settings > Developer options > Debug bridge (USB debugging: ON)

# On PC, enable Android Profiler:
# Window > Analysis > Profiler
# In Profiler, select device via dropdown
# Monitor: CPU, Memory, GPU
```

**Key Metrics to Record:**
- Average FPS (target ≥ 55)
- Frame time (target ≤ 18 ms for 55 FPS)
- Memory usage (target < 200 MB)
- GC pauses (target < 5)
- Hitch/freeze events (target 0)

### Step 5: Capture Device Logs

```bash
# Start logging
adb logcat -c  # Clear buffer
adb logcat -v threadtime > device_test_$(date +%Y%m%d_%H%M%S).log &

# Run test for 1-2 minutes
# Then stop logging (Ctrl+C)

# Check for errors/warnings:
grep -i "error\|exception\|crash" device_test_*.log
```

### Step 6: Screenshot Validation

```bash
# Capture screenshots during test
adb shell screencap /sdcard/test_screenshot_$(date +%s).png

# Pull to PC
adb pull /sdcard/test_screenshot_*.png ./screenshots/

# Verify:
# - [ ] All thieves visible
# - [ ] UI elements not overlapping
# - [ ] Rendering quality matches editor
# - [ ] No visual glitches
```

---

## Expected Test Results

### Automated Test Results
```
Total Tests: 78
Passed: 78 (100%)
Failed: 0 (0%)
Duration: ~15 seconds
```

### Device Performance Results (Galaxy A12)
```
Average FPS: 58-60
Min FPS: 55
Max FPS: 60
Frame time avg: 16.7 ms (60 FPS nominal)
Memory usage: 150-180 MB
GC pauses: < 2 per 10 seconds
Thermal throttling: None observed
Battery drain: < 1% per minute
```

---

## Troubleshooting

### Issue: Tests don't run automatically
**Solution:**
1. Check ThiefSpawnerTestManager.autoStartTests = true
2. Check ThiefSpawner.Instance is not null
3. Check PlayerController.Instance is not null
4. Check scene is fully loaded (wait 1-2 seconds after Play)

### Issue: Compilation errors
**Solution:**
1. Verify ThiefSystem.cs enum values match script references
2. Check ProceduralXxxMesh classes exist
3. Run: Assets > Reimport All
4. Verify C# version compatibility (use .NET 4.x)

### Issue: Tests show "Failed" results
**Solution:**
1. Check Console for detailed error messages
2. Verify thief GameObject has all required components
3. Check collider radius/height values match expectations (0.35 / 1.8)
4. Verify PlayerController is initialized before spawning

### Issue: FPS drops on device
**Solution:**
1. Check if device is overheating (thermal throttling)
2. Close background apps
3. Enable Battery Saver mode OFF
4. Check profiler for spike in CPU/GPU usage
5. Verify VSync is enabled

### Issue: APK won't install
**Solution:**
```bash
# Check version code isn't already installed
adb uninstall com.example.game

# Install with override
adb install -r -g path/to/apk

# Check device logs for install errors
adb logcat | grep "pm"
```

---

## Sign-Off Checklist

**Editor Testing:**
- [ ] Automated tests run without errors
- [ ] All 78 tests pass
- [ ] No console errors/warnings
- [ ] Spawn positions verified visually
- [ ] Colliders visible in debug view

**Device Testing (Galaxy A12):**
- [ ] App launches without crash
- [ ] All 4 thief types spawn correctly
- [ ] Average FPS ≥ 55 (target 60)
- [ ] No stuttering or frame drops
- [ ] Memory usage < 200 MB
- [ ] No thermal throttling
- [ ] Screenshots show correct rendering
- [ ] Device logs show no errors

**Documentation:**
- [ ] Test plan complete
- [ ] Test automation scripts functional
- [ ] Device test logs captured
- [ ] Performance metrics documented
- [ ] Issues identified and logged

---

## Sign-Off

**QA Tester:** Claude Code (Agent 6e7ac2b7-521c-481b-8e9c-e01a6edfbb70)  
**Test Date:** 2026-05-07  
**Status:** Ready for execution  
**Approval:** Pending device test completion

---

## References

- [ThiefSpawnerTest.md](Assets/Scenes/ThiefSpawnerTest.md) — Full test plan
- [ThiefSpawnerTestManager.cs](Assets/Scripts/Tests/ThiefSpawnerTestManager.cs) — Test automation code
- [ThiefSpawner.cs](Assets/Scripts/Enemies/ThiefSpawner.cs) — Thief spawning system
- [AIG-102](/AIG/issues/AIG-102) — Parent issue (this task)
- [AIG-84](/AIG/issues/AIG-84) — Thief Spawner Setup (dependency)
