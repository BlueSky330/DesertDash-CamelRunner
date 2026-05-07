# M8 QA Execution Procedures — Complete Testing Guide

**Milestone:** M8 — Polish & QA (Pre-Launch)  
**Timeline:** 2026-05-07 → 2026-07-16 (10 weeks)  
**QA Lead:** Claude Code (QATester Agent)  
**Status:** Ready for Execution  
**Last Updated:** 2026-05-07

---

## Overview

This document consolidates all QA testing procedures for the M8 phase. It includes:
- Automated test execution procedures
- Device testing guidelines
- Performance profiling protocols
- Visual quality inspection procedures
- Bug triage and fix cycles
- Economy and difficulty balance validation

**Total Testing Scope:** 49+ automated tests + manual validation across 7+ Android devices + iOS smoke test

---

## Part 1: Automated Test Execution

### 1.1 EditMode Tests (Offline, Fast)

**Location:** `Assets/Tests/EditMode/`

**Test Files & Coverage:**

| File | Test Count | Coverage |
|------|-----------|----------|
| EconomyBalanceTests.cs | 14 | Score→coin conversion, IAP, ad rewards, unlock costs |
| CollectibleSystemTests.cs | 8 | Collectible spawning, pickup mechanics, score accumulation |
| HealthSystemTests.cs | 5 | Health decay, recovery, damage events |
| PowerUpManagerTests.cs | 4 | Power-up activation, buff application, duration |
| GameManagerTests.cs | 3 | Game state, pause/resume, game over |
| **Total EditMode** | **34 tests** | Core game logic |

**Execution (in Unity Editor):**

```bash
# Method 1: Using Test Runner UI
Window > Testing > Test Runner
├─ Select "EditMode" tab
├─ Click "Run All"
└─ Wait ~30 seconds for completion

# Method 2: Command Line (if available)
unity -runTests -testPlatform editmode
```

**Expected Results:**
```
✓ 34/34 tests PASS
✓ Execution time: ~30 seconds
✓ No errors or warnings
✓ Log output shows:
  - ScoreConversion: PASS
  - AdReward: PASS
  - UnlockCosts: PASS
  - etc.
```

**Success Criteria:**
- [ ] All 34 tests pass
- [ ] No failed assertions
- [ ] Execution time consistent (< 40 seconds)
- [ ] No debug errors in logs

---

### 1.2 PlayMode Tests (In-Game, Slower)

**Location:** `Assets/Tests/PlayMode/`

**Test Files & Coverage:**

| File | Test Count | Coverage |
|------|-----------|----------|
| GameplayTests.cs | 15 | Player movement, lane switching, obstacles, collectibles, power-ups, health |
| **Total PlayMode** | **15 tests** | Gameplay mechanics |

**Execution (in Unity Editor):**

```bash
# Method 1: Using Test Runner UI
Window > Testing > Test Runner
├─ Select "PlayMode" tab
├─ Click "Run All"
└─ Wait ~2 minutes for completion

# Method 2: Command Line
unity -runTests -testPlatform playmode
```

**Expected Results:**
```
✓ 15/15 tests PASS
✓ Execution time: ~90-120 seconds
✓ No errors or warnings
✓ Coverage includes:
  - Player lane switching
  - Jump mechanics
  - Slide mechanics
  - Obstacle collision detection
  - Collectible pickup
  - Score accumulation
  - Health decay/recovery
  - Power-up activation
```

**Success Criteria:**
- [ ] All 15 tests pass
- [ ] Scene loads and plays correctly
- [ ] No timeout errors
- [ ] All gameplay mechanics verified

---

### 1.3 ThiefSpawner Automated Tests (NEW)

**Location:** `Assets/Scripts/Tests/ThiefSpawnerTestManager.cs`

**Test Count:** 78 tests (comprehensive Thief Spawner verification)

**Execution:**

```bash
# Step 1: Create scene (if needed)
Unity Editor → Assets > Create Test Scenes > Thief Spawner Test Scene

# Step 2: Run tests
Press Play (tests run automatically)
Wait ~15 seconds

# Step 3: Verify results
Check Console tab for:
[ThiefSpawnerTest] Passed: 78
[ThiefSpawnerTest] Failed: 0
```

**Test Categories:**
- Spawn positions (Ahead, Behind, Side)
- All 4 thief types
- Component verification (Animator, Collider, Rigidbody)
- Animation transitions
- Collider detection
- Mobile rendering capability

**Expected Results:**
```
✓ 78/78 tests PASS
✓ Execution time: ~15 seconds
✓ All spawn positions verified
✓ All 4 thief types spawn correctly
```

**Success Criteria:**
- [ ] 78/78 tests pass
- [ ] No console errors
- [ ] All spawn positions logged correctly
- [ ] Thieves visible in scene

**Documentation:** See `Assets/Scenes/ThiefSpawnerTest.md` and `TEST_EXECUTION_GUIDE.md`

---

### 1.4 Complete Automated Test Run (Total)

**Total Test Count:** 49 (existing) + 78 (new) = **127 automated tests**

**Complete Execution Procedure:**

```bash
Step 1: Open Unity Editor
Step 2: Window > Testing > Test Runner

Step 3: Run EditMode tests
├─ Select "EditMode" tab
├─ Click "Run All"
└─ Wait ~30 seconds

Step 4: Run PlayMode tests
├─ Select "PlayMode" tab
├─ Click "Run All"
└─ Wait ~2 minutes

Step 5: Create and run Thief Spawner tests
├─ Assets > Create Test Scenes > Thief Spawner Test Scene
├─ Press Play
└─ Wait ~15 seconds

Step 6: Document results
├─ Screenshot test runner output
├─ Note any failures
└─ Log total time
```

**Expected Summary:**
```
Total Tests: 127
├─ EditMode: 34 PASS
├─ PlayMode: 15 PASS
└─ ThiefSpawner: 78 PASS
Total: 127/127 PASS
Total Time: ~3 minutes
```

**Failure Response:**
If any test fails:
1. Note the test name and error message
2. Check console for details
3. Create bug issue (if appropriate)
4. Rerun to verify consistency
5. Document failure

---

## Part 2: Device Testing (Requires APK Build)

### 2.1 Prerequisites

**Blocking Dependency:**
- ⏳ AIG-248: Asset Database Rebuild (in_review → needed)
- ⏳ AIG-222: Build APK (blocked by AIG-248)
- ⏳ AIG-214: Device Testing (blocked by APK)

**Once AIG-248 is approved:**

```
AIG-248 approved → AIG-222 can build APK 
              → AIG-214 can do profiling
              → AIG-250 can test Thief Spawner on device
              → Device matrix testing can begin
```

### 2.2 Device Target Configurations

**Primary Device (Baseline):**
- Samsung Galaxy A12 (API 30, 720×1600, 3GB RAM)
- Target: 60 FPS baseline
- Duration: 60+ minutes continuous play

**Secondary Devices (Matrix Testing):**
- Galaxy A51 (API 29, 1080×2340, 4GB RAM)
- Pixel 4a (API 31, 1080×2340, 6GB RAM)
- Redmi 9A (API 29, 720×1600, 3GB RAM)
- Galaxy S10 Lite (API 30, 720×1600, 3GB RAM)
- OnePlus 8 (API 30, 1080×2340, 8GB RAM)
- Moto G7 (API 28, 720×1560, 4GB RAM)

**Total:** 7 device configurations covering OS 9-12 and RAM 3-8GB

### 2.3 APK Build Specification

```
Platform: Android
Target API: 33 (Android 13)
Minimum API: 26 (Android 8.0)
Architecture: ARM64
Resolution: 720×1600 (portrait mobile)
Graphics: OpenGL ES 3
VSync: Enabled (60 FPS sync)
Optimization: IL2CPP
Compression: LC
```

### 2.4 Device Testing Procedure

**Test Duration:** 60+ minutes continuous play per device

**Setup:**
```bash
adb devices  # Verify device connected
adb logcat -v threadtime > device_test_$(date +%Y%m%d).log &
```

**Monitoring:**
- FPS: Monitor continuously
- Memory: Unity Profiler
- Thermal: Check device temperature
- Battery: Monitor drain rate

**Expected Performance Metrics:**
| Metric | Target | Acceptable |
|--------|--------|------------|
| Avg FPS | 60 | ≥ 55 |
| Min FPS | 55 | ≥ 50 |
| Frame Time | ≤ 16.67 ms | ≤ 18 ms |
| Memory | < 200 MB | < 250 MB |
| GC Pause | < 5/10s | < 10/10s |
| Throttle | None | No sustained |

**Success Criteria:**
- [ ] Game launches without crash
- [ ] Plays smoothly at 60 FPS for 60+ minutes
- [ ] No memory leaks (stable usage)
- [ ] No thermal throttling
- [ ] No frame drops below 50 FPS
- [ ] Device logs show no errors

**Failure Response:**
- [ ] Identify cause (CPU, GPU, memory, thermal)
- [ ] Capture profiler data
- [ ] Create performance bug issue
- [ ] Document for optimization pass

---

## Part 3: Performance Profiling

### 3.1 Profiling Checklist

**CPU Profiling:**
- [ ] Player.Update() time < 1 ms
- [ ] Physics.FixedUpdate() time < 0.5 ms
- [ ] Rendering time consistent
- [ ] GC allocation < 5 MB per frame
- [ ] No frame time spikes

**GPU Profiling:**
- [ ] Draw call count < 100
- [ ] Batch count < 10 for dynamic objects
- [ ] Texture memory < 100 MB
- [ ] No expensive shaders in hot path
- [ ] Fill rate acceptable

**Memory Profiling:**
- [ ] Heap size stable (no growth)
- [ ] Object pool active (no garbage)
- [ ] Asset unloading working
- [ ] Scene unload clean
- [ ] Peak memory < 250 MB

### 3.2 Profiling Tools

**Unity Profiler:**
```
Window > Analysis > Profiler
├─ CPU tab: Player, Rendering
├─ Memory tab: Heap allocation
├─ GPU tab: Draw calls
└─ Frame Debugger: Draw order
```

**Android Profiler (via Logcat):**
```bash
adb logcat -s "FrameTime:MemoryUsage:ThermalStatus"
```

**Device-Native Tools:**
- Android Profiler (Android Studio)
- Qualcomm Snapdragon Profiler
- Samsung Game Optimizing Service (GOS)

---

## Part 4: Visual Quality & Reference Comparison

### 4.1 Visual Quality Checklist

**Egypt Level Reference:**
- Reference image: [GDD.md - Egypt Level Section]
- [ ] Background matches reference art
- [ ] Parallax scrolling smooth and layered
- [ ] Character models render correctly
- [ ] Obstacles visually distinct
- [ ] Collectibles clearly visible
- [ ] Animations smooth and fluid
- [ ] Text/UI clean and readable

**Platform-Specific Quality:**

**Android 720×1600 (Galaxy A12):**
- [ ] No visual clipping
- [ ] Text legible at 6-inch diagonal
- [ ] Touch targets adequate (48dp+)
- [ ] No overlapping UI elements
- [ ] Color accuracy good (no bloom/blown-out)

**Android 1080×2340 (Pixel 4a):**
- [ ] Texture filtering quality high
- [ ] Anti-aliasing visible (not pixelated)
- [ ] Fine details preserved
- [ ] No scaling artifacts

### 4.2 Visual Inspection Procedure

**Per-Device Review:**
```
1. Launch game on device
2. Navigate through all screens:
   └─ Main Menu
   └─ Settings
   └─ Shop/Unlocks
   └─ Gameplay (30+ seconds)
   └─ Game Over

3. Verify at each:
   ├─ No graphical glitches
   ├─ Text readable
   ├─ UI elements properly positioned
   ├─ Animations smooth
   └─ Colors vibrant (not washed out)

4. Take screenshots of:
   ├─ Main Menu
   ├─ Gameplay sample
   └─ Game Over
```

**Success Criteria:**
- [ ] All screens render without glitches
- [ ] Visual quality matches or exceeds reference
- [ ] No performance-related visual issues (tearing, jank)
- [ ] All text fully readable

---

## Part 5: Difficulty & Economy Balance

### 5.1 Difficulty Curve Validation

**Manual Testing Checklist:**
- [ ] Level 1 (tutorial): Easy, teaches controls
- [ ] Level 5: Medium, introduces challenges
- [ ] Level 10: Moderate, good balance
- [ ] Level 20: Hard, requires skill
- [ ] Level 30+: Very hard, endgame challenge

**Metrics:**
- Speed ramp (obstacles per second increasing smoothly)
- Obstacle density (harder types appear later)
- Thief spawn timing (balanced with player skill progression)

**Validation:**
```
Play for 30+ minutes continuously:
├─ Is progression smooth? (not sudden jumps)
├─ Are failures skill-based? (not RNG-based)
├─ Is challenge fair? (not frustrating)
└─ Does it feel rewarding? (wins are satisfying)
```

### 5.2 Economy Balance Validation

**Automated Tests:** EconomyBalanceTests.cs (14 tests) ✅

**Manual Verification:**
- [ ] Score→Coin ratio feels fair (150 pts = 1 coin)
- [ ] Coins earned at reasonable rate
- [ ] Unlock costs proportionate
  - [ ] Jordan skin: 500 coins (cheap, early)
  - [ ] India skin: 800 coins (mid)
  - [ ] USA skin: 5000 coins (expensive, endgame)
- [ ] Ad rewards valuable (2x coins)
- [ ] Not too grindy (achievable in 1-2 hours)
- [ ] Not too easy (meaningful progression)

**40-Minute Play Test:**
```
1. Play continuously for 40 minutes
2. Track earnings:
   ├─ Coins earned
   ├─ Unlocks available
   └─ Time to first unlock

3. Validate:
   ├─ Earning pace is fun (not boring)
   ├─ Progression clear (unlock achievable)
   ├─ No obvious exploits
   └─ Fair to non-IAP players
```

---

## Part 6: Bug Triage & Fix Cycles

### 6.1 Bug Classification

**P0 (Critical) — Fix before launch:**
- Game crash
- Permanent data loss
- Sequence break (can skip content)
- Pay-to-win exploit

**P1 (High) — Fix this phase:**
- Major gameplay bug (collision missed, spawn failed)
- Performance regression (FPS drop > 10)
- Visual glitch affecting playability
- Incorrect scoring/economy

**P2 (Medium) — Fix post-launch:**
- Minor visual glitch (doesn't block play)
- Edge case bug (rare trigger condition)
- UI spacing issue
- Animation jank (doesn't affect gameplay)

**P3 (Low) — Backlog:**
- Polish/nice-to-have
- Cosmetic issues
- Rare edge cases

### 6.2 Bug Triage Procedure

**During Testing:**
```
1. Encounter bug
2. Document:
   ├─ Reproducibility (always/sometimes/once)
   ├─ Steps to reproduce
   ├─ Expected vs actual behavior
   └─ Screenshot/video if possible
3. Classify (P0/P1/P2/P3)
4. Create issue:
   ├─ Title: [Px] Brief description
   ├─ Description: Full details
   └─ Label: bug, gameplay, visual, etc.
5. Assign to developer
```

### 6.3 Fix Verification

**After Fix:**
```
1. Update issue with fix details
2. Verify fix on next build:
   ├─ Run original reproduction steps
   ├─ Verify bug gone
   ├─ Check for regressions
   └─ Update issue status
3. If fix successful: close issue
4. If issue remains: reopen + comment
```

**Minimum Fix Cycles:** 2 rounds
- Round 1: Bugs found in days 1-5, fixed by day 7
- Round 2: Regressions/edge cases, fixed by day 14

---

## Part 7: Test Execution Timeline

### Phase Breakdown

**Week 1 (May 7-16):** Automated Test Baseline
- [ ] Run all 49 existing tests
- [ ] Run 78 Thief Spawner tests
- [ ] Document results

**Week 2 (May 17-23):** APK Build & Device Setup
- ⏳ Wait for AIG-248 (asset rebuild) approval
- [ ] Build APK once approved
- [ ] Set up device lab
- [ ] Verify ADB connectivity

**Week 3-4 (May 24 - Jun 6):** Performance Profiling
- [ ] Galaxy A12 baseline run (60+ min)
- [ ] Profile CPU/GPU/Memory
- [ ] Document metrics
- [ ] Identify optimization opportunities

**Week 5-6 (Jun 7-20):** Device Matrix Testing
- [ ] Test on 7 device configurations
- [ ] Verify 60 FPS across devices
- [ ] Capture screenshots
- [ ] Document per-device issues

**Week 7-8 (Jun 21 - Jul 4):** Visual Quality & Sign-Off
- [ ] Compare Egypt level to reference
- [ ] Approve visual quality
- [ ] Get stakeholder sign-off

**Week 9-10 (Jul 5-16):** Bug Triage & Final Cycles
- [ ] Triage all bugs from testing
- [ ] Fix P0/P1 bugs
- [ ] Verify fixes
- [ ] Final smoke test

---

## Part 8: Success Criteria Summary

### Acceptance Criteria for M8 QA Sign-Off

**Automated Tests:**
- [ ] All 127 automated tests pass (34 EditMode + 15 PlayMode + 78 ThiefSpawner)
- [ ] No timeout failures
- [ ] Consistent results across runs

**Device Testing:**
- [ ] Tested on 7 device configurations
- [ ] 60 FPS achieved on Galaxy A12 (baseline)
- [ ] 55+ FPS minimum on lower-end devices
- [ ] No memory leaks
- [ ] No thermal throttling

**Performance:**
- [ ] CPU metrics within budget
- [ ] GPU draw calls < 100
- [ ] Memory < 250 MB peak
- [ ] GC pauses < 10/second

**Visual Quality:**
- [ ] Egypt level matches/exceeds reference
- [ ] No visual glitches on any device
- [ ] Text fully readable
- [ ] All animations smooth

**Economy & Difficulty:**
- [ ] Difficulty curve smooth and fair
- [ ] Economy balanced (not grindy, not too easy)
- [ ] No obvious exploits
- [ ] IAP value proportionate

**Bugs:**
- [ ] 0 P0/P1 bugs remaining
- [ ] All P2 bugs logged for post-launch
- [ ] 2+ fix cycles completed

**Sign-Off:**
- [ ] QA approval: Signed off on all criteria
- [ ] Dev lead approval: Tests valid and comprehensive
- [ ] Product approval: Game ready for launch

---

## Part 9: Reference Documents

- **QA_TEST_PLAN_M8.md** — Initial test planning (49 existing tests documented)
- **DEVICE_TESTING_PLAN_M8.md** — Device configuration reference
- **DEVICE_TESTING_PROCEDURES.md** — Device lab setup guide
- **THIEF_SPAWNER_TEST_SUMMARY.md** — Thief Spawner framework details
- **TEST_EXECUTION_GUIDE.md** — Step-by-step execution procedures

---

## Part 10: Current Status & Next Actions

### Completed ✅
- Automated test framework (49 existing tests)
- Thief Spawner test scene (78 tests)
- Documentation and procedures
- Device specifications and targets

### In Progress ⏳
- AIG-248: Asset database rebuild (in_review, CRITICAL)
- AIG-249: Editor test execution (todo, ready now)

### Blocked 🚫
- AIG-250: Device testing (blocked by APK build)
- AIG-214: Profiling run (blocked by APK build)
- Device matrix testing (blocked by APK build)

### Next Steps

**IMMEDIATE (No Build Required):**
1. **Execute editor tests** → Run all 127 automated tests
   - Command: Window > Testing > Test Runner
   - Expected: 100% pass rate
   - Owner: [AIG-249](/AIG/issues/AIG-249)

**AFTER APK AVAILABLE (Blocked by AIG-248):**
2. **Deploy to Galaxy A12** → Run 60+ minute baseline profile
   - Expected: 60 FPS sustained
   - Owner: [AIG-250](/AIG/issues/AIG-250)
3. **Device matrix testing** → Test on 7 configs
4. **Visual quality sign-off**
5. **Bug triage & fix cycles**

---

## Appendix: Quick Reference Commands

```bash
# Run automated tests (Unity Editor)
Window > Testing > Test Runner → Run All

# Create Thief Spawner test scene
Assets > Create Test Scenes > Thief Spawner Test Scene

# Build APK (Android)
File > Build Settings > Android > Build

# Deploy APK to device
adb install -r app.apk

# Launch app on device
adb shell am start -n com.example.game/.MainActivity

# Monitor logs
adb logcat | grep "FrameTime\|Error\|Exception"

# Enable Profiler
Window > Analysis > Profiler

# Stop Profiler session
Profiler > Detach Target
```

---

## Document History

| Version | Date | Author | Notes |
|---------|------|--------|-------|
| 1.0 | 2026-05-07 | Claude Code | Initial comprehensive guide covering all M8 QA procedures |

---

**Status:** 🟢 Ready for Execution  
**Last Updated:** 2026-05-07 16:30 UTC  
**Owner:** Claude Code (QATester Agent)  
**Next Review:** After AIG-248 approval
