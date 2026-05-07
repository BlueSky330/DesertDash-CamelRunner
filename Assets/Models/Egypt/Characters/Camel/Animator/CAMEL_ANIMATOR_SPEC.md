# Camel Animator Controller Specification

**Status:** Template created (AIG-85)  
**Last Updated:** 2026-05-07  
**Blocking Issue:** [AIG-36](link) — Meshy AI mesh generation  

## Overview

This document describes the Camel Animator Controller template for the Camel Runner endless runner game. The controller defines animation parameters, state machine, and blend tree structure required for character animation.

---

## Parameters

| Name | Type | Triggered By | Purpose |
|------|------|---|---------|
| `IsRunning` | **Bool** | `PlayerController.OnGameStarted()` / `OnGameStopped()` | Controls Idle ↔ Run transition |
| `Jump` | **Trigger** | `PlayerController.TryJump()` | Triggers one-shot Jump animation |
| `Slide` | **Trigger** | `PlayerController.TrySlide()` | Triggers one-shot Slide animation |
| `Hit` | **Trigger** | `PlayerController.OnControllerColliderHit()` | Triggers Hit Reaction animation |

✅ **Parameters match PlayerController.cs** (lines 63-66)

---

## State Machine Structure

```
                    [IsRunning=true]
               ←————————————————————→
               |                     |
            ┌──▼─────┐          ┌────▼──┐
            │  IDLE  │          │ RUN   │
            └────┬───┘          └────┬──┘
                 │                    │
        ┌────────┼────────────────┬───┘
        │        │                │
   Jump │    Slide │          Hit │
        │        │                │
        └────┬───┴────────────┬───┘
             │                │
        ┌────▼──┐     ┌───────▼───┐
        │ JUMP  │     │ SLIDE     │
        └────┬──┘     └───────┬───┘
             │                │
             └────────┬───────┘
                  [exitTime: 0.95]
            [IsRunning bool]
                      │
                 Idle/Run

             ┌─────────────┐
             │     HIT     │
             │  REACTION   │
             └──────┬──────┘
                    │
           [exitTime: 0.95]
           [IsRunning bool]
                    │
              Idle ↔ Run
```

---

## States & Animations

| State | Animation | Frames | Loop | Notes |
|-------|-----------|--------|------|-------|
| **Idle** | Idle | 60 | ✓ | Gentle sway + breathing (from AIG-27) |
| **Run** | Run Cycle (blend tree) | 36 | ✓ | Loopable, Happy blend 0.5 (60 FPS) |
| **Jump** | Jump | 48 | ✗ | Parabolic arc 0→-0.3→+2.0 Y-translate |
| **Slide** | Slide | 30 | ✗ | Y compress -0.8 units, duration 0.8s |
| **Hit** | Hit Reaction | 42 | ✗ | Startled→Determined blend sequence |

📌 **Pending:** Animations will be imported from FBX after AIG-36 mesh generation.

---

## Blend Tree (Run State)

**Type:** 1D Linear Blend

**Parameter:** Speed (0–2 range, for future use)

**Blend Points:**
- 0.0: Run Cycle (walking speed)
- 1.0: Run Cycle (normal speed)
- 2.0: Run Cycle (fast speed)

**Purpose:** Allow smooth animation speed variation without changing game speed. Currently single animation will be used; blend tree prepared for future speed variants.

---

## Transitions & Exit Times

### Idle ↔ Run
- **Idle → Run:** Instant (duration 0.0s), triggered by `IsRunning = true`
- **Run → Idle:** Smooth (duration 0.1s), triggered by `IsRunning = false`

### One-Shot States (Jump/Slide/Hit)
- **Entry:** Instant (duration 0.0s) from Idle or Run
- **Exit:** At `exitTime: 0.95` (allows animation to complete)
- **Return Logic:** 
  - If `IsRunning = true` → Return to Run
  - If `IsRunning = false` → Return to Idle

---

## Setup Instructions

### 1. Generate Controller (Editor Menu)

Open the Unity Editor and go to:

```
Tools → Camel Runner → Setup Camel Animator
```

This will create:
- `Assets/Models/Egypt/Characters/Camel/Animator/CamelAnimator.controller`
- All parameters, states, transitions, and blend tree

### 2. Verify Structure

Run the verification menu to confirm correct setup:

```
Tools → Camel Runner → Verify Camel Animator
```

Expected output:
```
✓ IsRunning (bool):  ✓ Present
✓ Jump (trigger):    ✓ Present
✓ Slide (trigger):   ✓ Present
✓ Hit (trigger):     ✓ Present

States: 5 total
  Expected: Idle, Run, Jump, Slide, Hit
```

---

## Animation Import (Next Steps)

**Blocking Issue:** [AIG-36](link) — Awaiting Meshy AI mesh generation

### Timeline
1. ✅ AIG-85 (This task) — Animator Controller template created
2. 🔴 AIG-36 — Generate Camel_Base.fbx via Meshy AI (5-10 min)
3. → AIG-26 Phase 1 — Model, rig, bake animations into FBX (~3-4 hours)
4. → AIG-81.1 — Import FBX with animations to Unity
5. → Assign animation clips to states

### For Developer: FBX Import Settings

When importing `Camel_Base.fbx` with baked animations:

```csharp
// Expected clips in FBX
- Idle (loop)
- Run (loop)
- Jump (one-shot)
- Slide (one-shot)
- Hit Reaction (one-shot)
```

**Import Configuration:**
- ✅ Rig: Humanoid (or Generic if custom rig)
- ✅ Animation: Import animations enabled
- ✅ Blend shapes: As needed
- ✅ Legacy: Disabled

**Assignment:**
```
Idle state.motion    = Camel_Base → Idle
Run state.motion     = Camel_Base → Run (or blend tree clips)
Jump state.motion    = Camel_Base → Jump
Slide state.motion   = Camel_Base → Slide
Hit state.motion     = Camel_Base → Hit Reaction
```

---

## Verification Checklist

Before handing off animations:

- [x] Animator Controller created at correct path
- [x] All 4 parameters added (IsRunning, Jump, Slide, Hit)
- [x] All 5 states created (Idle, Run, Jump, Slide, Hit)
- [x] Transitions configured with correct conditions
- [x] Blend tree prepared on Run state
- [x] Parameter names match PlayerController.cs
- [ ] Animation clips imported from FBX (AIG-36 dependent)
- [ ] Animation clips assigned to states
- [ ] Test in gameplay: Idle state on startup
- [ ] Test transitions: IsRunning bool works
- [ ] Test one-shots: Jump/Slide/Hit trigger correctly

---

## Related Issues & Documents

- **[AIG-26](link)** — Camel character 5-phase sequential pipeline
- **[AIG-27](link)** — 7 animations specification
- **[AIG-36](link)** — Meshy AI mesh generation (blocker)
- **[AIG-81](link)** — Asset integration (parent)
- **[AIG-81.1](link)** — Rig wiring
- **[AIG-81.2](link)** — Animation import
- **[AIG-81.3](link)** — Material & skin setup
- **[AIG-81.4](link)** — Animator parameters (this task)
- **CAMEL_ASSET_NAMING_CONVENTIONS.md** — Asset folder & naming specs
- **PlayerController.cs** — Animation parameter consumer (lines 63-66, 370-371)

---

## Notes for Future

- **Speed Blend Tree:** Currently prepared but unused. PlayerController.cs does not expose speed parameter. If dynamic animation speed becomes a requirement, update PlayerController to set Speed parameter based on DifficultyManager velocity.

- **Blend Shapes:** If blended animations needed (e.g., Happy/Nervous blend on Run cycle as per AIG-27 spec), configure in AnimatorStateMachine layer blending after FBX import.

- **Layer Support:** Base layer shown above. Additional layers (e.g., upper body, IK constraints) can be added if needed for refinement.

---

**Generated:** 2026-05-07  
**By:** Artist1 (AIG-85)  
**Template Status:** Ready for animation import
