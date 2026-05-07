# M8 QA Test Plan — Desert Dash: Camel Runner

**Timeline:** 2026-05-07 → 2026-07-16 (10 weeks)  
**Phase:** Pre-Launch Quality Assurance  
**QA Tester:** Claude Code (Agent)  
**Last Updated:** 2026-05-07

---

## 1. Automated Test Baseline

All automated tests located in `Assets/Tests/`.

### EditMode Tests (Economy Balance)
- **File:** `Assets/Tests/EditMode/EconomyBalanceTests.cs`
- **Test Count:** 14 tests
- **Coverage:**
  - ✅ Score-to-coin conversion (150 pts = 1 coin)
  - ✅ Ad reward structure (FreeShop: 100 coins, DoubleCoins)
  - ✅ Unlock cost fairness (Jordan: 500, India: 800, USA: 5000)
  - ✅ Coin spending prevention (no negative balance)
  - ✅ Economy progression validation

### PlayMode Tests (Gameplay Mechanics)
- **File:** `Assets/Tests/PlayMode/GameplayTests.cs`
- **Test Count:** 15 tests
- **Coverage:**
  - ✅ Player movement and lane switching
  - ✅ Jump and slide mechanics
  - ✅ Collectible pickup and score accumulation
  - ✅ Health decay (1%/min initial, 2.5%/min after 30min)
  - ✅ Health recovery (natural and ad-based)
  - ✅ Power-up activation
  - ✅ System integration (score + health interaction)

### Existing System Tests
- `Assets/Tests/EditMode/CollectibleSystemTests.cs` — 8 tests
- `Assets/Tests/EditMode/HealthSystemTests.cs` — 5 tests
- `Assets/Tests/EditMode/PowerUpManagerTests.cs` — 4 tests
- `Assets/Tests/EditMode/GameManagerTests.cs` — 3 tests

**Total Test Count:** 49 unit/integration tests

---

## 2. Performance Profiling (60 FPS Target)

### Baseline Metrics

| Metric | Target | Status |
|--------|--------|--------|
| Average FPS (continuous play) | 60 FPS | To test |
| FPS on collision | ≥ 55 FPS | To test |
| FPS during collectible spawn | ≥ 55 FPS | To test |
| Memory footprint (30min gameplay) | < 300 MB | To test |
| GC pause < 33ms | 95%+ frames | To test |
| Battery drain (1hr continuous) | < 10% | To test |

### Performance Test Procedure

1. **Load Test Scene:** Egypt level, continuous 60+ minutes
2. **Profiler Setup:**
   - Enable Unity Profiler (CPU, Memory, GPU)
   - Monitor frame time breakdown
   - Track GC allocation and spike frequency
3. **Device Targets:**
   - Primary: Samsung Galaxy A12 (Hexa-core 2.3GHz, 3GB RAM)
   - Secondary: Galaxy A51 (2.0GHz, 4GB RAM)
4. **Test Duration:** 60 minutes continuous play
5. **Log Outputs:**
   - PerformanceTester.cs FPS/memory report
   - Android Profiler data
   - Battery percentage delta

### Optimization Checklist

- [ ] Object pooling active (obstacles, collectibles, effects)
- [ ] Draw call count < 100 (batching/instancing verified)
- [ ] Texture memory within budget (compressed, max 1024×1024)
- [ ] Shader usage validated (no expensive shaders in hot path)
- [ ] GC allocation < 5MB per frame
- [ ] LOD system operational (distant terrain/objects reduced)
- [ ] Audio streaming (not loading all at once)
- [ ] Physics collision count reasonable (no unnecessary colliders)

---

## 3. Device Matrix Testing

### Minimum Coverage (5+ device configurations)

#### Android Testing

| Device | OS | Resolution | RAM | Status |
|--------|----|----|-----|--------|
| Samsung Galaxy A12 | Android 11 | 720×1600 | 3GB | Primary |
| Samsung Galaxy A51 | Android 10 | 1080×2340 | 4GB | Secondary |
| Xiaomi Redmi 9A | Android 10 | 720×1600 | 3GB | Budget |
| Google Pixel 4a | Android 12 | 1080×2340 | 6GB | High-end |
| Samsung Galaxy S10 Lite | Android 11 | 720×1600 | 3GB | Mid-range |
| ONEPLUS 8 | Android 11 | 1080×2340 | 8GB | Flagship |
| Motorola Moto G7 | Android 9 | 720×1560 | 4GB | Legacy |

**Scope:** OS versions 8.0–14+

#### iOS Testing (if Xcode available)

| Device | OS | Status |
|--------|----|----|
| iPhone 11 | iOS 15+ | Smoke test |
| iPhone SE (3rd gen) | iOS 15+ | Smoke test |
| iPad Air (5th gen) | iPadOS 15+ | Smoke test |

### Per-Device Test Checklist

For each device, verify:

- [ ] Game launches without crash
- [ ] Main menu responsive, all buttons clickable
- [ ] Egypt level loads and runs at 60 FPS
- [ ] Player movement (swipe left/right) responsive
- [ ] Jump and slide mechanics work
- [ ] Collectibles spawn and are touchable
- [ ] Obstacles appear and collisions detected
- [ ] Score/health HUD readable and updates
- [ ] Ad dialogs load and close cleanly
- [ ] Game Over screen appears and restart works
- [ ] Settings menu accessible
- [ ] No visual glitches (overlapping UI, cut-off text)
- [ ] Touch input latency acceptable (< 50ms)
- [ ] Audio plays (music and SFX)
- [ ] Battery consumption reasonable (< 15% per hour)

### Device Testing Reports

Create one report per device:
- Screenshots of main menu, gameplay, game over
- FPS readings (min/avg/max)
- Memory usage
- Crash/error logs (if any)
- Issues discovered

---

## 4. Visual Quality & Egypt Level Sign-Off

### Reference Comparison

- [ ] Egypt level background matches reference art
- [ ] Parallax scrolling smooth and properly layered
- [ ] Camel animation fluid (no frame skips)
- [ ] Obstacle models visually distinct (rock, cactus, ruins)
- [ ] Collectible sprites clear and readable
- [ ] Particle effects (dust, impacts) appropriate density
- [ ] UI text crisp and properly aligned
- [ ] Color palette consistent with Egyptian theme

### Known Reference Issues (if any)

Document any discrepancies between build and reference:
- Asset swaps (temporary placeholders vs. final)
- Animation timing differences
- Shader effects not yet enabled

---

## 5. Bug Triage & Fix Cycle

### Bug Report Template

```markdown
## Title: [P0/P1/P2] Brief description

**Steps to Reproduce:**
1. ...
2. ...
3. ...

**Expected Behavior:**
...

**Actual Behavior:**
...

**Device/OS:**
Samsung Galaxy A12 / Android 11

**Screenshot/Video:**
[Attachment]

**Frequency:** 100% reproducible / Intermittent (~5% of attempts)

**Severity Impact:**
- P0: Game crash, soft lock, broken core loop
- P1: Major feature broken (score not tracked, ads don't load, health not decaying)
- P2: UI glitch, minor issue, edge case
```

### Triage Process

1. **Report Phase:** Identify issue, capture reproduction steps, screenshot/video
2. **Assign Phase:** Forward to Lead Unity Developer with template
3. **Verification Phase:** Wait for fix commit/build
4. **Retest Phase:** Verify fix on same device
5. **Document Phase:** Update bug log with resolution

### Bug Log

Maintain running tally:

| ID | Title | Severity | Device | Status | Fix PR |
|----|-------|----------|--------|--------|--------|
| BUG-001 | [P0] Game crash on obstacle collision | P0 | Galaxy A12 | Fixed | #123 |
| BUG-002 | [P1] Score not converting to coins | P1 | All | Fixed | #124 |
| BUG-003 | [P2] Overlap text on Shop button | P2 | Galaxy A51 | Fixed | #125 |

**Success Criteria:** 0 P0/P1 bugs remaining at launch

---

## 6. Difficulty Curve & Economy Balance Verification

### Difficulty Progression

Test over 30+ minute run:
- [ ] Obstacle spawn rate increases smoothly
- [ ] Obstacle density at 30min feels challenging but fair
- [ ] Power-ups appear at reasonable frequency
- [ ] No sudden difficulty spikes

### Economy Balance

- [ ] Players earn ~200-300 coins per 10-minute run (baseline)
- [ ] Ad watching is voluntary, not forced
- [ ] Unlock costs feel proportional (Jordan affordable early, USA takes time)
- [ ] Shop prices make sense relative to earn rate
- [ ] No economy exploit (score conversion, ad abuse, etc.)

### Player Retention Metrics (Post-Launch)

- Daily play session duration ≥ 10 minutes
- Day 7 retention ≥ 30%
- No P2 economy complaints in early reviews

---

## 7. Edge Cases & Stress Tests

### Network & Offline

- [ ] Game runs without internet connection
- [ ] Ad-based rewards are skipped offline (no fake coins)
- [ ] Player data persists across offline sessions
- [ ] Network restored: sync works without conflict

### Interruptions

- [ ] Phone call interrupts game → pause, resume works
- [ ] Notification arrives → game pauses, resumes cleanly
- [ ] Device sleeps → wake up preserves game state
- [ ] Low battery mode (< 10% battery) → game still playable at reduced FPS

### High Load Conditions

- [ ] Low memory device (2GB RAM): game doesn't crash, acceptable FPS
- [ ] Max collectibles on screen: no lag
- [ ] Multiple obstacle spawns: collision detection accurate
- [ ] Thief encounters x5 in 1 run: handling correct

---

## 8. Sign-Off & Acceptance

### QA Acceptance Criteria

- [ ] All 49 automated tests pass
- [ ] 60 FPS maintained on Galaxy A12 for 60-minute run
- [ ] Device matrix: ≥ 7 devices tested, no P0 issues
- [ ] Egypt level visual quality approved
- [ ] 0 P0/P1 bugs remaining
- [ ] Difficulty curve feels fair
- [ ] Economy is balanced (not grindy, not too easy)
- [ ] No memory leaks detected
- [ ] Battery drain within acceptable range

### Release Decision

**Green Light (Approved for Release):**
- All acceptance criteria met
- Player feedback positive
- No blockers from partner integrations

**Yellow Light (With Caution):**
- Minor P2 issues remaining (documented in known issues)
- Performance acceptable but not optimal on lowest-tier devices
- Requires immediate post-launch patch plan

**Red Light (Hold Release):**
- P0/P1 bugs remain
- Economy balance severely broken
- Performance < 45 FPS on target device
- Visual quality issues

---

## 9. Timeline & Milestones

| Week | Milestone | Deliverable |
|------|-----------|-------------|
| W1 (May 7-13) | Automated tests baseline | All 49 tests passing |
| W2 (May 14-20) | Performance profiling | Baseline metrics + optimization report |
| W3 (May 21-27) | Device matrix testing | Reports for 5+ devices |
| W4-6 (May 28 - Jun 17) | Bug triage cycles (2+ rounds) | Bug log with fixes verified |
| W7-8 (Jun 18 - Jul 1) | Visual QA + polish | Egypt level sign-off |
| W9-10 (Jul 2-16) | Final verification + sign-off | Release readiness |

---

## 10. Appendix: File References

- Test files: `Assets/Tests/{EditMode,PlayMode}/*.cs`
- Performance tester: `Assets/Scripts/Testing/PerformanceTester.cs`
- Game systems: `Assets/Scripts/{Core,Economy,Player,World,Ads}/*.cs`
- Build guides: `APK_BUILD_GUIDE.md`, `AD_SETUP_GUIDE.md`

---

**Status:** In Progress  
**Last QA Comment:** [AIG-14](/AIG/issues/AIG-14#comment-d91fde39)  
**Next Review:** Post-test run results
