# QA Bug Reporting Template

**Purpose:** Standardized template for QA engineers to report bugs found during testing  
**Used By:** Device testing teams, manual QA, performance profiling teams  
**Status:** Ready for use during M8 testing phase

---

## How to Use This Template

1. **Copy this section** (everything below the line)
2. **Fill in each field** based on the bug you found
3. **Be specific and detailed** — the more info you provide, the faster developers can fix
4. **Include screenshots/videos** when helpful
5. **Submit as a Paperclip issue** with appropriate priority

---

## BUG REPORT TEMPLATE

### Header

**Test Case ID:** [Reference the test case from QA_TEST_CASES_REFERENCE.md, e.g., TC-1.1.1]  
**Device:** [Device name and OS, e.g., Samsung Galaxy A12, Android 11]  
**Build Version:** [APK/build version tested]  
**Test Date:** [YYYY-MM-DD]  
**Tester Name:** [Your name]  

---

### Bug Summary

**Title:** [Brief 1-line description of the bug]

Example titles:
- Player gets stuck in lane after rapid swipes
- Score doesn't update when collecting gems
- Game crashes when power-up expires
- Memory leak detected during 30-minute session

---

### Severity Classification

Select one:
- **P0 — Critical:** Game crash, data loss, sequence break, permanent stuck state
- **P1 — High:** Major gameplay bug, significant performance issue, blocking core feature
- **P2 — Medium:** Minor gameplay bug, rare crash, cosmetic issue affecting UX
- **P3 — Low:** Edge case, polish item, doesn't block gameplay

**Severity Rationale:** [Explain why you chose this level]

Example: "P0 because player cannot progress past level 5 (sequence break) and must restart."

---

### Reproduction Steps

**Precondition:** [What was the state before the bug occurred?]

Example: "Game running for 3 minutes, player has 500 points, no power-ups active"

**Steps to Reproduce:**
1. [First action the tester took]
2. [Second action]
3. [Continue...]

Example:
1. Start game on Egypt level
2. Play normally for 30 seconds
3. Collect 5 collectibles
4. Hit obstacle and take damage
5. [Bug occurs here]

**How Often:** [Select one]
- Always (100% reproducible)
- Usually (80-99%)
- Sometimes (20-79%)
- Rarely (1-19%)
- Once (0.1%)

---

### Expected vs. Actual Behavior

**Expected Behavior:** [What should happen according to game design?]

Example: "Player should take 1 health damage when hitting obstacle"

**Actual Behavior:** [What actually happened?]

Example: "Player took 3 health damage instead of 1"

**Difference:** [Quantify the difference]

Example: "Damage dealt is 3x higher than expected"

---

### Visual Evidence

**Screenshots:** [Attach if available]
- [ ] Screenshot of bug
- [ ] Screenshot of HUD/UI state
- [ ] Screenshot of console/error (if applicable)

**Video:** [Attach if helpful for timing-sensitive bugs]
- [ ] 10-30 second video showing reproduction

**Console Errors:** [Paste any error messages below]

```
[Paste error text here]
```

Example:
```
NullReferenceException: Object reference not set to an instance of an object
  at ThiefSpawner.SpawnThief (ThiefSystem+ThiefType thiefType) (at Assets/Scripts/Enemies/ThiefSpawner.cs:45)
```

---

### Environment Details

**Device Specifications:**
- Model: [e.g., Samsung Galaxy A12]
- OS Version: [e.g., Android 11]
- RAM: [e.g., 3 GB]
- CPU: [e.g., Hexa-core 2.3 GHz]

**Game State:**
- Level: [e.g., Egypt, Country unlocked: Jordan/India/etc.]
- Time in Game: [e.g., 3 minutes 45 seconds]
- Score at Bug Time: [e.g., 1250 points]
- Health: [e.g., 75% / 3 hearts]
- Active Power-ups: [e.g., Speed Boost (expires in 5s)]
- Recent Actions: [e.g., Just collected gem, hit obstacle]

**Network State:**
- [ ] Online (connected to internet)
- [ ] Offline (no connection)
- [ ] Switching (network changed during gameplay)

**Thermal/Battery State:**
- Temperature: [e.g., Normal / Warm / Hot / Throttling]
- Battery: [e.g., 80% / Low Power Mode active]

---

### Impact Analysis

**What Does This Bug Affect?** [Select all that apply]
- [ ] Game progression (can't progress past certain point)
- [ ] Core gameplay (movement, jumping, sliding, lane changes)
- [ ] Collectibles/scoring system
- [ ] Power-up system
- [ ] Health/lives system
- [ ] Audio/music
- [ ] UI/menu
- [ ] Performance/FPS
- [ ] Memory/crashes
- [ ] Multiplayer/social (if applicable)
- [ ] Monetization/ads
- [ ] Device compatibility

**How Many Players Affected?** [Estimate]
- [ ] 1 (me only, might be device-specific)
- [ ] Few (might be config-specific)
- [ ] Many (likely affects most players)
- [ ] All (affects all devices/configurations)

**Play Time Lost:** [Estimate impact]
- Unable to recover from bug without [what action?]
  - Example: "Unable to recover without restarting game"
  - Example: "Lost 5 minutes of progress"

---

### Workaround (if known)

**Does a workaround exist?** [Yes/No]

If yes, describe:
[Describe steps to avoid/work around the bug]

Example: "If you avoid collecting gems for 30 seconds after collecting a collectible, the bug doesn't occur."

---

### Additional Notes

[Any other observations, context, or information useful to the developer]

Examples:
- "This might be related to the recent speed boost changes"
- "Same bug appeared in version 1.2, but was marked as fixed"
- "Bug occurs more frequently with certain power-ups active"
- "Performance degrades significantly before this bug occurs"

---

### Related Issues

[Reference any related bugs or PRs]

Example:
- Related to [AIG-XXX] — Similar issue with collectibles
- Related to [AIG-XXX] — Potential root cause in ThiefSpawner.cs

---

### Tester Checklist

Before submitting, verify:

- [ ] I've tested on the device specified
- [ ] I can reproduce the bug consistently (or noted if random)
- [ ] I've followed the exact reproduction steps
- [ ] I've captured screenshot/video if helpful
- [ ] I've provided all device/environment details
- [ ] I've chosen appropriate severity level
- [ ] I've filled in all required sections
- [ ] I've proofread for clarity and accuracy

---

## Developer Triage Checklist

(For developer review only)

- [ ] Severity level appropriate
- [ ] Reproduction steps clear and complete
- [ ] Environment details sufficient for reproduction
- [ ] Screenshot/evidence helpful
- [ ] Console errors captured
- [ ] Related issues identified
- [ ] Needs more info from tester? [List what's needed]

---

## Common Bug Categories & Examples

### Category: Gameplay Mechanics

| Bug | Title | Example P0 | Example P1 |
|-----|-------|-----------|-----------|
| Movement | Player stuck in lane | Can't move after rapid swipes | Player occasionally doesn't respond to input |
| Jump | Jump doesn't work | Jump button unresponsive | Jump height inconsistent |
| Slide | Slide mechanic broken | Slide doesn't activate | Slide duration varies |
| Obstacles | Collision not detected | Hit invisible obstacle | Collision damage wrong amount |

### Category: Scoring & Economy

| Bug | Title | Example P0 | Example P1 |
|-----|-------|-----------|-----------|
| Collection | Items not counted | Coins disappear when collected | Gems worth wrong points |
| Conversion | Score not converting | Score→coins conversion broken | Fractional coin rounding wrong |
| Display | Score shows wrong | Score shows negative | Score display laggy |

### Category: Power-ups

| Bug | Title | Example P0 | Example P1 |
|-----|-------|-----------|-----------|
| Activation | Power-up doesn't work | Speed boost has no effect | Shield doesn't absorb damage |
| Duration | Effect lasts too long/short | Boost never expires | Duration inconsistent |
| Stacking | Multiple power-ups conflict | Two boosts crash game | Magnet attracts wrong items |

### Category: Performance

| Bug | Title | Example P0 | Example P1 |
|-----|-------|-----------|-----------|
| FPS | Game unplayable | FPS drops below 30 | FPS drops below 50 during action |
| Memory | Memory leak detected | Game crashes after 5 min | Memory grows 50 MB per minute |
| Crash | Game crashes | Crash when accessing menu | Crash when power-up expires |

### Category: UI/UX

| Bug | Title | Example P0 | Example P1 |
|-----|-------|-----------|-----------|
| Menu | Can't navigate menu | Menu buttons don't respond | Menu text overlaps |
| Display | Text unreadable | Text completely cut off | Text slightly misaligned |
| Response | Input lag | 2+ second delay to input | Button presses sometimes skip |

---

## Quick Reference: Severity Guide

| Severity | Block Launch? | Fix Timeline | Example Bugs |
|----------|---------------|-------------|--------------|
| **P0** | YES | Immediate (hours) | Crash, data loss, sequence break |
| **P1** | MAYBE | This sprint | Core mechanic broken, 20% FPS drop |
| **P2** | NO | Next sprint | Minor visual glitch, rare crash |
| **P3** | NO | Post-launch | Polish, edge cases, nice-to-have |

---

## Submitting the Bug Report

### Steps to File in Paperclip

1. **Create new issue:** Paperclip → New Issue
2. **Title:** Copy "Bug Summary > Title" from this template
3. **Description:** Paste the entire filled-out template
4. **Label:** Add "bug" + relevant category (gameplay, performance, ui, etc.)
5. **Priority:** Set to P0/P1/P2/P3 based on severity
6. **Assignee:** Leave unassigned (let PM assign to dev)
7. **Parent Issue:** Link to [AIG-14](/AIG/issues/AIG-14) (M8: Polish & QA)
8. **Blockers:** If this bug blocks other tests, mark as blocker
9. **Attachments:** Add screenshots/videos if helpful

### Example Issue

**Title:** Game crashes when collecting gem after power-up expires (P0)

**Description:** [Full template content]

**Labels:** bug, gameplay, crash  
**Priority:** P0 — Critical  
**Parent:** AIG-14 (M8 phase)

---

## Tips for Effective Bug Reports

✅ **DO:**
- Be specific and detailed
- Include exact reproduction steps
- Provide screenshots/video
- Test on the exact device specified
- Note if bug is reproducible or random
- Include relevant system info
- Reference test case ID

❌ **DON'T:**
- Use vague descriptions ("game is broken")
- Forget reproduction steps
- Report assumptions instead of facts
- Miss screenshots
- Test on multiple devices then say "any device"
- Forget device/OS version
- Write novels (be concise but complete)

---

## Post-Bug-Report Workflow

### During Triage

Developer will:
1. Review bug report
2. Assign severity
3. Ask for clarification if needed
4. Reproduce on their machine
5. Assign to appropriate developer
6. Track fix progress

### During Fix

Developer will:
1. Comment on bug with fix details
2. Test fix on their machine
3. Submit code for review
4. Update issue status

### During Verification

You (Tester) will:
1. Get notified of fix
2. Test on the same device/conditions
3. Verify bug is resolved
4. Comment with verification results
5. Close bug if fixed, reopen if not

---

## Template Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2026-05-07 | Initial template for M8 QA phase |

---

**Questions?** Refer to QA_TEST_CASES_REFERENCE.md for test case IDs, or M8_QA_EXECUTION_PROCEDURES.md for testing procedures.

Good bug reports = Faster fixes = Better quality launch! 🎯
