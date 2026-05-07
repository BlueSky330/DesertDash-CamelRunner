# AIG-207: Device Matrix Testing Execution Plan

**Status:** In Progress (W1 - Testing Setup Phase)  
**QA Tester:** Claude Code (Agent)  
**Target Devices:** 7 Android devices  
**Timeline:** May 7-27, 2026 (prep + execution)

## Phase 1: Testing Infrastructure Setup (May 7-10)

### 1.1 APK Build Verification
- [ ] Confirm APK is built and available
- [ ] Verify APK is signed with debug/test key (NOT production)
- [ ] Test APK accessibility (size, format validation)

### 1.2 Device Testing Method Selection
**Option A: Firebase Test Lab (Cloud-based)**
- Supports all 7 device models
- Automated screenshot capture
- Per-device performance metrics
- Security: service account auth, isolated test builds
- **Status:** Requires Firebase setup

**Option B: Android Emulator Farm**
- Quick baseline testing on multiple AVDs
- Performance profiling via Android Profiler
- **Status:** Can start immediately with emulators

**Option C: Physical Device Lab**
- Actual hardware testing for accuracy
- **Status:** Requires device access/coordination

### 1.3 Testing Infrastructure Decision
→ **Recommend:** Start with Option B (Emulator) for baseline + Option A (Firebase) for comprehensive coverage

---

## Phase 2: Device-Specific Test Execution (May 11-20)

### Device Matrix & Test Scope

| # | Device | OS | RAM | Resolution | Test Status |
|---|--------|----|----|---------|-----------|
| 1 | Samsung Galaxy A12 | Android 11 | 3GB | 720×1600 | Pending |
| 2 | Samsung Galaxy A51 | Android 10 | 4GB | 1080×2340 | Pending |
| 3 | Xiaomi Redmi 9A | Android 10 | 3GB | 720×1600 | Pending |
| 4 | Google Pixel 4a | Android 12 | 6GB | 1080×2340 | Pending |
| 5 | Samsung Galaxy S10 Lite | Android 11 | 3GB | 720×1600 | Pending |
| 6 | OnePlus 8 | Android 11 | 8GB | 1080×2340 | Pending |
| 7 | Motorola Moto G7 | Android 9 | 4GB | 720×1560 | Pending |

### Per-Device Test Checklist (Run on each device/emulator)

**Launch & Navigation (5 min)**
- [ ] Game launches without crash
- [ ] Main menu responsive and visible
- [ ] All buttons clickable (Play, Settings, Shop, LeaderBoard)
- [ ] Menu transitions smooth

**Gameplay Mechanics (15 min)**
- [ ] Egypt level loads successfully
- [ ] Player movement (swipe left/right) responsive (< 50ms latency)
- [ ] Jump mechanic works
- [ ] Slide mechanic works
- [ ] Collectibles spawn and are touchable
- [ ] Obstacles appear and collisions detected
- [ ] Score updates on collectible pickup
- [ ] Health bar visible and updates

**Performance Monitoring (10 min per device)**
- [ ] Target FPS: 60 FPS maintained during gameplay
- [ ] Acceptable FPS: ≥ 55 FPS during collisions/spawns
- [ ] Memory usage: < 300 MB for 30-min session
- [ ] No frame drops or stutter

**Ad System (5 min)**
- [ ] Ad dialogs load (test ads, not real)
- [ ] Ad close button works
- [ ] Reward callback fires correctly

**Edge Cases (10 min)**
- [ ] Resume from pause works
- [ ] Game over screen appears correctly
- [ ] Restart functionality works
- [ ] Settings menu accessible and applies changes
- [ ] No visual glitches (overlapping UI, cut-off text)

**Total Time per Device:** ~45 minutes

---

## Phase 3: Results & Reporting (May 21-27)

### Per-Device Deliverables

For each device, produce:

**Test Report**
```
📱 Device: [Model] / Android [OS]
├─ Launch: ✅ PASS
├─ Gameplay: ✅ PASS (minor: see issue BUG-XXX)
├─ Performance:
│  ├─ Avg FPS: 59 FPS
│  ├─ Min FPS: 54 FPS
│  └─ Memory Peak: 285 MB
├─ Ad System: ✅ PASS
└─ Issues Logged: BUG-XXX (High), BUG-YYY (Low)
```

**Screenshots** (3 per device minimum)
- Main menu
- Gameplay (running, collecting)
- Game over / results screen

**Device-Specific Issues** (if any)
- Issue ID / Title
- Steps to reproduce
- Screenshot/video
- Severity (P0/P1/P2)

### Consolidated Device Compatibility Report

**Deliverable:** `DEVICE_COMPATIBILITY_REPORT_M8.md`

Contents:
- ✅ Summary of tested devices (7/7)
- ✅ Pass/fail breakdown per device
- ✅ Performance metrics table
- ✅ Issues found (organized by severity + device)
- ✅ Compatibility assessment (Green/Yellow/Red light)
- ✅ Recommendations for fixes

---

## Acceptance Criteria (AIG-207)

- [ ] APK built successfully
- [ ] Game launches and is playable on all 7 devices
- [ ] Device-specific issues logged with screenshots
- [ ] Performance metrics captured per device
- [ ] Device compatibility report generated

---

## Next Actions

1. **Immediate (Today):** 
   - Request APK build (trigger GitHub Actions workflow) or verify availability
   - Determine device testing method (emulator vs. cloud lab)
   - Create child issues for device testing phases if parallel work needed

2. **Short-term (Next 3 days):**
   - Set up testing environment (emulators or Firebase Test Lab)
   - Execute baseline test on primary device (Galaxy A12)
   - Document any blockers or infrastructure gaps

3. **Ongoing:**
   - Execute device matrix testing
   - Log issues with reproducible steps and screenshots
   - Generate compatibility report

---

**Blocked By:** APK build availability, device testing infrastructure confirmation
**Owner:** Claude Code (QA Tester)
**Depends On:** AIG-205 (automated test baseline) - should be complete before device testing
