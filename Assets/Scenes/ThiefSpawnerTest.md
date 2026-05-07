# ThiefSpawnerTest Scene Setup

**Purpose:** Verify Thief Spawner functionality including spawn positions, animations, and collision detection.

**Test Date:** 2026-05-07  
**Test ID:** AIG-102  
**QA Tester:** Claude Code (QATester Agent)

---

## Scene Configuration

### Hierarchy

```
ThiefSpawnerTest
├── Main Camera
├── Ground Plane
├── Player (with PlayerController)
│   ├── Character Model (Camel)
│   └── CamelAnimationController
├── ThiefSpawner (empty GameObject with ThiefSpawner script)
├── ThiefSpawnerTestManager (with test automation script)
└── Canvas (for test UI - optional)
```

### GameObjects & Components

#### Main Camera
- **Position:** (0, 1.5, -10)
- **Rotation:** (10, 0, 0)
- **Component:** Camera (Main)
- **Clipping Planes:** Near=0.1, Far=1000
- **Field of View:** 60

#### Ground Plane
- **Model:** Cube (scale: 20, 0.1, 20)
- **Position:** (0, -0.5, 0)
- **Material:** Ground material
- **Collider:** Box Collider (for reference only, not needed for thieves)
- **Tag:** Ground

#### Player GameObject
- **Position:** (0, 0, 0)
- **Components:**
  - CharacterController
    - Radius: 0.25
    - Height: 1.8
    - Center: (0, 0.9, 0)
  - Animator (with animation controller)
  - CamelAnimationController (child or this object)
  - PlayerController script
    - Lane Width: 2.0
    - Lane Change Speed: 12
    - Jump Force: 12
    - Forward Speed: 10
    - Gravity: 22

**Child:** Camel 3D Model
- **Model:** Camel_Default prefab (or equivalent)
- **Position:** (0, 0, 0) relative to Player
- **Scale:** (1, 1, 1)
- **Rotation:** (0, 0, 0)
- **Component:** CamelAnimationController
  - Has Animator Parameter Hashes:
    - AnimIsRunning
    - AnimJump
    - AnimSlide
    - AnimHit

#### ThiefSpawner GameObject
- **Position:** (0, 0, 0)
- **Components:**
  - ThiefSpawner script
    - Ahead Distance: 15
    - Behind Distance: -10
    - Side Offset X: 5

#### ThiefSpawnerTestManager GameObject
- **Position:** (0, 0, 0)
- **Components:**
  - ThiefSpawnerTestManager script
    - Auto Start Tests: true
    - Spawn Test Delay: 1.0 second

---

## Test Cases

### Test 1: All 4 Thieves Spawn at AHEAD Position
**Expected Behavior:**
- Spawn position Z ≈ 15 units ahead of player
- All 4 types spawn: DesertBandit, NinjaThief, Pirate, ShadowThief
- Each has collider (CapsuleCollider, isTrigger=true)
- Each has Animator component
- Each has "Enemy" tag
- Each is named correctly (e.g., "Thief_DesertBandit_12345")

**Verification:**
- [ ] Console logs show 4 successful spawns
- [ ] Position logs show Z ≈ 15 (with tolerance ±1)
- [ ] Debug visualization shows thieves ahead of player
- [ ] No errors or warnings in console

---

### Test 2: All 4 Thieves Spawn at BEHIND Position
**Expected Behavior:**
- Spawn position Z ≈ -10 units behind player
- All 4 types spawn successfully
- Same component/tag verification as Test 1

**Verification:**
- [ ] Console logs show 4 successful spawns
- [ ] Position logs show Z ≈ -10 (with tolerance ±1)
- [ ] Debug visualization shows thieves behind player
- [ ] No errors or warnings

---

### Test 3: All 4 Thieves Spawn at SIDE Position
**Expected Behavior:**
- Spawn position X ≈ ±5 units (randomly alternates)
- Spawn position Z ≈ 5 units forward
- All 4 types spawn successfully

**Verification:**
- [ ] Console logs show 4 successful spawns
- [ ] Position logs show X ≈ ±5, Z ≈ 5
- [ ] Spawns alternate left/right
- [ ] No errors or warnings

---

### Test 4: Animation Transitions (Idle→Run)
**Expected Behavior:**
- Thief GameObjects have Animator components
- Animator controller loaded successfully
- Animation parameters can be set (IsRunning, etc.)

**Verification:**
- [ ] All thieves have Animator component
- [ ] Animator controller is not null
- [ ] Console shows "✓ PASS: Animator controller" for all types

**Note:** Full animation playback requires animation clips and controllers to be configured in the animator controller asset.

---

### Test 5: Collider Detection
**Expected Behavior:**
- Thief colliders are CapsuleColliders
- Colliders are set as triggers (isTrigger = true)
- Radius: 0.35 (±0.05)
- Height: 1.8 (±0.1)
- Rigidbodies are kinematic

**Verification:**
- [ ] All thieves have CapsuleCollider
- [ ] All colliders are triggers
- [ ] Console shows "✓ PASS: Collider radius" for all types
- [ ] Console shows "✓ PASS: Collider height" for all types
- [ ] All Rigidbodies are kinematic (no dynamic physics)

---

## Mobile Rendering & Performance Test

### Test 6: 60 FPS Target on Mid-Range Android Device

**Test Device:** Samsung Galaxy A12 (Android 11, 720×1600, 3GB RAM)

**Setup:**
1. Build APK and deploy to device
2. Open ThiefSpawnerTest scene
3. Run test with Profiler enabled
4. Monitor frame time and FPS

**Expected Results:**
- Average FPS: ≥ 55 FPS (target 60)
- Frame time: ≤ 18 ms (1000/55)
- No frame drops below 45 FPS during spawning
- No stuttering during animation playback
- Memory: < 150 MB (scene-only, no accumulated garbage)

**Profiler Metrics to Check:**
- CPU: Player.Update + ThiefSpawner updates
- GPU: Mesh rendering, draw calls
- Memory: Active objects, GC allocations
- Physics: Collider count, trigger counts

**Pass Criteria:**
- [ ] 90%+ of frames ≥ 55 FPS
- [ ] No frame time spikes > 30 ms
- [ ] Stable rendering during 10-spawn test cycle
- [ ] Memory usage stable (no growth over 30 seconds)

---

## Test Execution Steps

### Option 1: Automated Test (Recommended)
1. Open ThiefSpawnerTest scene in Unity
2. Press Play
3. ThiefSpawnerTestManager runs automatically
4. Check Console for test results
5. All test logs should show "✓ PASS" or "✗ FAIL"

### Option 2: Manual Test
1. Open ThiefSpawnerTest scene
2. Set ThiefSpawnerTestManager.autoStartTests = false
3. Press Play
4. Manually call methods via inspector or code
5. Verify results visually

### Option 3: Device Testing
1. Build APK with ThiefSpawnerTest as startup scene
2. Deploy to Samsung Galaxy A12 (or other target device)
3. Launch and observe:
   - Thieves appear at correct positions
   - No stuttering during spawning
   - Performance stable at 60 FPS
4. Check device logs via `adb logcat`

---

## Expected Console Output

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
[ThiefSpawner] Spawned Ninja at Ahead (0.0, 0.0, 15.0)
✓ PASS: Name check (Ninja)
✓ PASS: Animator exists (Ninja)
✓ PASS: Collider check (Ninja)
✓ PASS: Rigidbody kinematic (Ninja)
✓ PASS: Spawn position (Ninja) at Ahead
✓ PASS: Enemy tag (Ninja)
... (Pirate, ShadowThief follow same pattern)

[ThiefSpawnerTest] Test 2: Spawning all 4 types at BEHIND
... (same pattern for Behind position)

[ThiefSpawnerTest] Test 3: Spawning all 4 types at SIDE
... (same pattern for Side position)

[ThiefSpawnerTest] Test 4: Verifying animation transitions
✓ PASS: Animator controller for Thief_DesertBandit_12345
... (for each spawned thief)

[ThiefSpawnerTest] Test 5: Verifying collider detection
✓ PASS: Collider radius for Thief_DesertBandit_12345
✓ PASS: Collider height for Thief_DesertBandit_12345
... (for each spawned thief)

[ThiefSpawnerTest] === Test Summary ===
[ThiefSpawnerTest] Passed: 78
[ThiefSpawnerTest] Failed: 0
[ThiefSpawnerTest] Total: 78
```

---

## Failure Troubleshooting

| Issue | Solution |
|-------|----------|
| Thieves not spawning | Check ThiefSpawner.Instance is initialized; verify ThiefMeshTypes dictionary has all 4 types |
| Wrong spawn position | Verify aheadDistance, behindDistance, sideOffsetX are correctly set |
| Colliders missing | Check BuildThief() adds CapsuleCollider with correct radius/height |
| Animator missing | Verify thieves have Animator component and controller assigned |
| Test UI freezes | Check for infinite loops or blocking coroutines |
| 60 FPS not achieved | Profile CPU/GPU; check mesh complexity of procedural characters |

---

## Sign-Off Checklist

- [ ] All 4 thief types spawn correctly
- [ ] Spawn positions are accurate (±1 unit tolerance)
- [ ] Colliders are present and properly configured
- [ ] Animators are present and functional
- [ ] Mobile device rendering stable at 60 FPS
- [ ] No console errors or warnings
- [ ] Test automation runs to completion
- [ ] Device testing completed on Samsung Galaxy A12
- [ ] All edge cases tested (rapid spawning, simultaneous spawns, etc.)

---

## Document History

| Date | Version | Author | Notes |
|------|---------|--------|-------|
| 2026-05-07 | 1.0 | Claude Code | Initial test scene setup |

