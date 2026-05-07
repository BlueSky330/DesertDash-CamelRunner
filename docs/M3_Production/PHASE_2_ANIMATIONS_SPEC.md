# Phase 2: Animation Specifications

**Timeline**: May 22-25, 2026 (4 days)  
**Owner**: Animator/Rigger  
**Dependency**: Phase 1 (Camel_Default.fbx must be complete)  
**Deliverable**: Camel_Default.fbx with all 7 animations embedded

---

## Overview

Phase 2 takes the rigged Camel_Default.fbx from Phase 1 and adds 7 essential animations. All animations share the same skeleton (no rig modifications). Animations are baked (no IK, no constraints) and must blend smoothly with each other.

---

## Animation Specifications

### 1. Run Cycle (Loopable)

**Duration**: 0.6 seconds  
**Loop**: YES (seamless repeat)  
**Trigger**: Continuous during gameplay

**Description**:
- Camel runs forward with rhythmic leg/tail movement
- Head bobs slightly, shoulders sway
- Legs alternate in running pattern (RFL → LBL → LFL → RBL)
- Tail swings side-to-side with leg rhythm

**Key Frames**:
- Frame 0: Idle pose (starting position)
- Frame 5: Right front leg forward, left hind leg back
- Frame 10: Opposite (left front forward, right hind back)
- Frame 15: Return to frame 5 (half cycle)

**Looping Requirement**: Frame 0 and Frame 15 must be **identical** (so animation seamlessly loops with no pop)

**Quality Checks**:
- Legs don't penetrate ground plane
- Head/neck move naturally with body momentum
- Tail swings smoothly, not jittery
- Speed looks appropriate for a camel (medium pace, not frantic)

---

### 2. Lane Switch Left (Non-looping)

**Duration**: 0.4 seconds  
**Loop**: NO (plays once per input)  
**Trigger**: Player swipes left

**Description**:
- Camel leans/shifts left while moving forward
- Weight transfers to right side
- Head tilts slightly left
- Upper body rotates left

**Key Frames**:
- Frame 0: Neutral run position
- Frame 8: Maximum left lean (45° rotation on spine/chest)
- Frame 16: Return to neutral

**Blend Safety**: Start and end at neutral running pose (must match run cycle pose)

**Quality Checks**:
- Feels responsive (snappy, not sluggish)
- Doesn't break stride (continues forward momentum)
- Hip/spine rotation is natural
- No foot sliding (feet stay planted during lean)

---

### 3. Lane Switch Right (Non-looping)

**Duration**: 0.4 seconds  
**Loop**: NO (plays once per input)  
**Trigger**: Player swipes right

**Description**:
- Mirror of Lane Switch Left
- Camel leans/shifts right
- Weight transfers to left side
- Head tilts slightly right
- Upper body rotates right

**Key Frames**:
- Frame 0: Neutral run position
- Frame 8: Maximum right lean (45° rotation on spine/chest, opposite of left)
- Frame 16: Return to neutral

**Blend Safety**: Start and end at neutral running pose

**Quality Checks**:
- Symmetrical with left switch (mirror animation)
- Feels equally responsive
- Natural weight shift
- No foot sliding

---

### 4. Jump (Non-looping)

**Duration**: 0.8 seconds  
**Loop**: NO (plays once per input)  
**Trigger**: Player swipes up

**Description**: Launch → Peak → Land  
- **Launch phase** (0-0.2s): Crouch down, legs compress, ready to spring
- **Peak phase** (0.2-0.5s): Airborne, legs extended, slight forward arc, tail up for balance
- **Landing phase** (0.5-0.8s): Impact on ground, slight knee bend, recovery to run

**Key Frames**:
- Frame 0: Running position
- Frame 5: Fully crouched (legs bent, body lower)
- Frame 10: Highest point in arc (legs extended, body at peak height)
- Frame 20: Landing (feet touch ground, knees bend for impact)

**Arc Quality**: Parabolic arc (not straight up, slight forward trajectory)

**Landing Recovery**: Must transition back into run cycle smoothly

**Quality Checks**:
- Feels weighty (jump has momentum, not floaty)
- Landing has impact (body compresses on contact)
- Legs don't penetrate during landing
- Clears obstacles (jump height ~0.3 units)
- Can chain multiple jumps smoothly

---

### 5. Slide/Duck (Non-looping)

**Duration**: 0.5 seconds  
**Loop**: NO (plays once per input)  
**Trigger**: Player swipes down

**Description**:
- Crouch low to slide under high obstacles
- Head and body compress downward
- Front legs bend, back legs extend slightly
- Forward momentum continues (doesn't stop)

**Key Frames**:
- Frame 0: Running position
- Frame 8: Maximum crouch (body ~50% lower, head near ground level)
- Frame 12: Return to running position

**Height Reduction**: Body height should reduce to ~70% of normal (clears ~30cm obstacles)

**Quality Checks**:
- Quick, snappy (no slow transition)
- Doesn't break run momentum
- Body stays low for entire duration
- Smooth return to running position
- Legs don't clip through body

---

### 6. Hit Reaction / Stumble (Non-looping)

**Duration**: 0.7 seconds  
**Loop**: NO (plays once on collision)  
**Trigger**: Camel collides with obstacle

**Description**:
- Flinch backward from impact
- Brief dizzy/shake animation
- Recover to running position
- Should convey pain/surprise

**Key Frames**:
- Frame 0: Running position
- Frame 3: Flinch back (spine rotates backward, head jerks back)
- Frame 7: Shake (rapid twitch left-right, like dizzy effect)
- Frame 14: Recovery back to running position

**Shake Effect**: Frame 8-10, alternate body tilt left-right rapidly (3-4 small twitches)

**Recovery**: Smoothly transition back into run cycle

**Quality Checks**:
- Clearly reads as "hit" (not a dance move)
- Conveys impact and recovery
- Doesn't look overly dramatic (stays grounded)
- Blends back into run cycle
- Appropriate for casual game tone

---

### 7. Idle / Menu Pose (Loopable)

**Duration**: 1.0 seconds  
**Loop**: YES (seamless repeat)  
**Trigger**: Camel standing still (menu, ready screen)

**Description**:
- Camel stands in relaxed pose
- Slight weight shift (breathing effect)
- Ears twitch occasionally
- Tail gently sways
- Conveys patience and readiness

**Key Frames**:
- Frame 0: Standing neutral pose (feet planted, spine straight, arms at side)
- Frame 10: Slight lean left (subtle hip shift, weight on right leg)
- Frame 20: Back to neutral
- Frame 30: Slight lean right (opposite of frame 10)

**Breathing Effect**: Chest rises/falls slightly (0.1 unit oscillation)

**Looping Requirement**: Frame 0 and Frame 30 must be **identical**

**Micro-animations** (optional, can layer):
- Frame 12-14: Ears twitch inward slightly
- Frame 25-27: Tail sways gently

**Quality Checks**:
- Looks alive (not frozen/T-pose)
- Subtle (not overly animated)
- Breathing is gentle, natural rhythm
- Loops seamlessly
- Appropriate for UI/menu screens

---

## Technical Requirements

### All Animations
- **Rig Used**: Camel_Default skeleton (13 bones, no modifications)
- **Key Frame Spacing**: Linear interpolation between key frames (no easing curves unless specified)
- **Loop Transitions**: Looping animations must have identical first/last frames
- **Blend Points**: Non-looping animations must start/end at neutral/compatible poses
- **Baking**: All animations baked (no IK, no constraints, no drivers)
- **File Format**: Export as FBX 2020 with animations embedded

### Performance Constraints
- **Frame Rate**: All animations timed for 60 FPS playback
- **Bone Count**: Use only the 13 bones from Camel_Default rig (no additional bones)
- **Animation Data**: No texture/material animation (only skeletal)

---

## Blending Strategy

Animations must blend smoothly when transitions occur:

| From | To | Blend Length | Notes |
|------|----|----|-------|
| Run → Lane Switch | 0.1s | Already moving in same direction |
| Run → Jump | 0.05s | Snappy transition (jump is urgent) |
| Run → Slide | 0.05s | Snappy (dodge is urgent) |
| Jump/Slide → Run | 0.1s | Recovery should feel natural |
| Jump → Jump | 0.0s | Can chain immediately |
| Hit Reaction → Run | 0.1s | Allow recovery time |
| Idle → Run | 0.15s | Smooth start from standstill |
| Run → Idle | 0.15s | Smooth stop to standstill |

---

## Verification Checklist

Before exporting, verify:

- [ ] All 7 animations created in Blender
- [ ] Looping animations (Run, Idle) loop seamlessly (no frame pop)
- [ ] All animations stay within bone constraints (13 bones only)
- [ ] No IK handles in scene (all animations baked)
- [ ] All animations playable in Blender timeline (no errors)
- [ ] Tri count unchanged (Phase 1: <1,200 tris, still valid)
- [ ] Materials/colors unchanged from Phase 1
- [ ] FBX exports with all 7 animations embedded
- [ ] Animations blend smoothly in Unity Animator preview

---

## Export Instructions

1. **In Blender**: Select all 7 animations in Action Editor
2. **File > Export > FBX**
   - File: `Assets/Models/Camel/Camel_Default.fbx` (overwrite Phase 1 FBX)
   - Include: Mesh, Armature, Animations
   - FBX 2020 format
3. **Verify**: Import into Unity, test all animations in preview
4. **Hand-Off**: Notify Phase 3 animator that Phase 2 is complete

---

## Quality Acceptance Criteria

Phase 2 is complete when:

- [ ] All 7 animations created
- [ ] Run cycle is seamless loop
- [ ] Lane switches feel responsive
- [ ] Jump has clear launch/peak/land phases
- [ ] Slide is quick and effective
- [ ] Hit reaction clearly reads as impact
- [ ] Idle looks alive and natural
- [ ] All animations blend smoothly
- [ ] Animations export without errors
- [ ] Tri count remains <1,200
- [ ] Unity import shows all 7 animations in Animator

---

## Dependencies

**Incoming Dependency**: Camel_Default.fbx from Phase 1 (due May 21)  
**Outgoing Dependency**: Camel_Default.fbx with animations to Phase 3 (due May 25)

---

## Timeline

| Date | Task | Status |
|------|------|--------|
| May 22 | Receive Camel_Default.fbx, start Run cycle | ⬜ Pending |
| May 23 | Complete all basic animations (Run, Lane switches, Idle) | ⬜ Pending |
| May 24 | Complete action animations (Jump, Slide, Hit reaction) | ⬜ Pending |
| May 25 | Verify, test blends, export final FBX | ⬜ Pending |

---

**Created**: 2026-05-07  
**Status**: Ready for Phase 1 completion → Phase 2 handoff  
**Next**: Phase 3 (4 skins + 4 thieves, May 26-Jun 4)
