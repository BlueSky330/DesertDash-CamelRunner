# M8 Test Failure Triage Report
**Date:** 2026-05-08  
**QA Tester:** QATester Agent  
**Parent Issue:** [AIG-205](/AIG/issues/AIG-205) (Execute Automated Tests)  
**Triage Task:** [AIG-209](/AIG/issues/AIG-209) (Bug Triage & Fix Cycles)  

---

## Summary

**Total Failures:** 9 (all P2 severity)  
**Classified As:**
- 2 Test setup/initialization issues
- 1 Design clarification needed
- 6 Test environment/framework issues  
- 1 Edge case / boundary condition

**Action Items:**
- 3 subtasks for real bugs (test infrastructure fixes)
- 1 subtask for design decision
- 5 subtasks for environment/build settings

---

## Detailed Failure Classification

### Group 1: CollectibleSystemTests (2 failures)

#### AIG-209-T1: `SpendCoins_SucceedsWhenSufficient`
- **File:** `Assets/Tests/EditMode/CollectibleSystemTests.cs`
- **Root Cause:** Test assumes 500-coin starter balance; implementation starts at 0
- **Classification:** Test setup issue (fixable)
- **Type:** Not a real bug — test framework issue
- **Fix Required:** Initialize test with `AddCoins(500)` before assertion
- **Severity:** P2
- **Action:** Create subtask to fix test initialization

#### AIG-209-T2: `ResetScoreAndCoins_RestoresToDefaults`
- **File:** `Assets/Tests/EditMode/CollectibleSystemTests.cs`
- **Root Cause:** Test expects full reset; implementation preserves coins via persistent storage
- **Classification:** Design mismatch (decision required)
- **Type:** Not a bug — behavior is intentional but test assumption is wrong
- **Decision Needed:** Should `ResetScoreAndCoins()` preserve coins (current) or fully reset them (test expectation)?
- **Severity:** P2
- **Action:** Create subtask for design clarification with Lead Dev

---

### Group 2: EconomyBalanceTests (6 failures)

#### AIG-209-T3 through T8: All UnlockCost Tests (6 tests)
- **Tests:**
  - `UnlockCost_JordanCosts500Coins`
  - `UnlockCost_IndiaCosts800Coins`
  - `UnlockCost_EgyptIsStartingCountry`
  - `UnlockCost_USACosts5000CoinsHighestTier`
  - `Economy_UnlockCostProgressionIsIncreasing`
  - `Economy_AllCountriesHaveDefinedUnlockCosts`

- **File:** `Assets/Tests/EditMode/EconomyBalanceTests.cs`
- **Root Cause:** `WorldMapManager.InitializeCountries()` is in `Start()`, not `Awake()` — EditMode tests don't trigger `Start()` automatically
- **Classification:** Test environment/framework issue (fixable)
- **Type:** Not a game logic bug — test initialization timing issue
- **Fix Required:** Either:
  1. Move `InitializeCountries()` from `Start()` to `Awake()`, OR
  2. Manually call `InitializeCountries()` in test `[SetUp]` method
- **Severity:** P2
- **Impact:** High (6 tests fail due to same root cause)
- **Action:** Create ONE subtask to fix the initialization timing

---

### Group 3: HealthSystemTests (1 failure)

#### AIG-209-T9: `IsLowHealth_FalseAtOrAbove25`
- **File:** `Assets/Tests/EditMode/HealthSystemTests.cs`
- **Root Cause:** Boundary condition mismatch — implementation uses `<= 25f`, test expects `< 25f`
- **Classification:** Edge case / boundary condition (fixable)
- **Type:** Not a game bug — implementation and test disagree on boundary
- **Fix Required:** Clarify boundary logic:
  - If health = 25 should be "low": implementation is correct, fix test
  - If health = 25 should NOT be "low": fix implementation to `< 25f`
- **Severity:** P2
- **Recommendation:** Typically "low health" means strictly below 25 (`< 25`), not at or below
- **Action:** Create subtask to align implementation and test on boundary

---

### Group 4: GameplayTests (1 failure)

#### AIG-209-T10: `HealthRecovery_NoHealthBelowZero`
- **File:** `Assets/Tests/PlayMode/GameplayTests.cs`
- **Root Cause:** `GameManager.EndGame()` calls `SceneManager.LoadScene(GameOverScene)` — scene not in build settings
- **Classification:** Test environment setup issue (fixable)
- **Type:** Not a game bug — test infrastructure missing
- **Fix Required:** Either:
  1. Add `GameOverScene` to **Project Settings > Build Settings**, OR
  2. Mock scene loading in test to prevent load attempt
- **Severity:** P2
- **Recommendation:** Add scene to build settings (more realistic test)
- **Action:** Create subtask to add scene or mock in test

---

## Triage Summary Table

| ID | Test | Classification | Type | Blocker? | Fix Effort | Action |
|----|------|-----------------|------|----------|-----------|--------|
| T1 | SpendCoins_SucceedsWhenSufficient | Test setup | Minor | No | 10 min | Subtask: Fix test init |
| T2 | ResetScoreAndCoins_RestoresToDefaults | Design mismatch | Decision | No | Review | Subtask: Design clarification |
| T3-8 | UnlockCost_* (6 tests) | Framework | Minor | No | 15 min | Subtask: Fix Init timing |
| T9 | IsLowHealth_FalseAtOrAbove25 | Boundary | Minor | No | 10 min | Subtask: Align boundary |
| T10 | HealthRecovery_NoHealthBelowZero | Env setup | Minor | No | 5 min | Subtask: Add scene |

---

## Next Steps

1. ✅ Triage complete — all 9 failures classified
2. **Create 5 subtasks under AIG-209:**
   - AIG-209-S1: Fix CollectibleSystemTests coin initialization
   - AIG-209-S2: Design clarification: ResetScoreAndCoins behavior
   - AIG-209-S3: Fix WorldMapManager initialization timing (fixes 6 tests)
   - AIG-209-S4: Align HealthSystem boundary condition
   - AIG-209-S5: Add GameOverScene to build settings
3. **Move AIG-209 to `in_progress`** (remove AIG-287 blocker)
4. **Assign subtasks** to Lead Unity Developer for fixes

---

## Remaining Correctly Blocked Issues

- **AIG-206** (Performance profiling): Blocked on physical Galaxy A12 hardware
- **AIG-207** (Device matrix): Blocked on physical device lab
- **AIG-208** (Visual QA): Blocked on final Egypt assets (AIG-287)

**Not blocked by anything:** AIG-209 (this bug triage work) ✅
