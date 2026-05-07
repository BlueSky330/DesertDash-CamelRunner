# Thief Prefab Setup Specification

**Issue:** [AIG-84](/AIG/issues/AIG-84)  
**Date:** 2026-05-07  
**Status:** In Progress  

---

## Overview

This document specifies the exact steps to create 4 thief character prefabs (DesertBandit, NinjaThief, Pirate, ShadowThief) for Camel Runner. Each prefab follows the CAMEL_ASSET_NAMING_CONVENTIONS.md spec and integrates with ThiefSpawner.cs.

---

## Folder Structure (Required)

```
Assets/Models/Egypt/Characters/Thieves/
├── DesertBandit/
│   ├── Thief_DesertBandit.fbx
│   ├── Mat_DesertBandit.mat
│   ├── Thief_DesertBandit_Idle.anim
│   ├── Thief_DesertBandit_Run.anim
│   ├── Thief_DesertBandit_AnimatorController.controller
│   └── Thief_DesertBandit.prefab
├── NinjaThief/
│   ├── Thief_NinjaThief.fbx
│   ├── Mat_NinjaThief.mat
│   ├── Thief_NinjaThief_Idle.anim
│   ├── Thief_NinjaThief_Run.anim
│   ├── Thief_NinjaThief_AnimatorController.controller
│   └── Thief_NinjaThief.prefab
├── Pirate/
│   ├── Thief_Pirate.fbx
│   ├── Mat_Pirate.mat
│   ├── Thief_Pirate_Idle.anim
│   ├── Thief_Pirate_Run.anim
│   ├── Thief_Pirate_AnimatorController.controller
│   └── Thief_Pirate.prefab
└── ShadowThief/
    ├── Thief_ShadowThief.fbx
    ├── Mat_ShadowThief.mat
    ├── Thief_ShadowThief_Idle.anim
    ├── Thief_ShadowThief_Run.anim
    ├── Thief_ShadowThief_AnimatorController.controller
    └── Thief_ShadowThief.prefab
```

---

## Prefab Component Specification

Each thief prefab must contain these components in this hierarchy:

### GameObject: `Thief_{ThiefType}`

| Component | Settings | Purpose |
|-----------|----------|---------|
| **Transform** | Default | Root node |
| **Animator** | Controller: `Thief_{Type}_AnimatorController.controller` | Drives animations (Idle, Run) |
| **CapsuleCollider** | Radius: 0.4, Height: 1.8, Center: (0, 0.9, 0), **Is Trigger: TRUE** | Enemy detection / collision |
| **Rigidbody** | Body Type: Dynamic, Gravity: OFF, Constraints: Freeze all rotation | Kinematic movement |
| **SkinnedMeshRenderer** | Mesh: from `Thief_{Type}.fbx`, Material: `Mat_{ThiefType}.mat` | Visual representation |
| **ThiefAI.cs** | *(Not yet created)* | AI behavior script |
| **Tag** | Set to: `"Enemy"` | For PlayerController hit detection |

### Child GameObject: `[Model]` (SkinnedMeshRenderer parent)

Automatically created from FBX import. Ensure:
- Parent is root `Thief_{ThiefType}` GameObject
- SkinnedMeshRenderer attached
- Material assigned: `Mat_{ThiefType}.mat`

---

## Animation Controller Configuration

Each thief needs a dedicated Animator Controller with:

### Parameters (4 required)

```
- IsRunning (bool) → drives Idle ↔ Run transition
- Jump (trigger) → (optional, for future)
- Slide (trigger) → (optional, for future)
- Hit (trigger) → (optional, for future)
```

### State Machine

```
┌─────────────────────────┐
│  Idle (default)         │
│  Play: Thief_{Type}_Idle.anim
└────────────┬────────────┘
             │ IsRunning=true
             ↓
┌─────────────────────────┐
│  Run (looping)          │
│  Play: Thief_{Type}_Run.anim
└────────────┬────────────┘
             │ IsRunning=false
             ↑
```

**Transitions:**
- Idle → Run: Condition `IsRunning == true`
- Run → Idle: Condition `IsRunning == false`
- Both can exit to Future trigger states (Hit, Catch, etc.)

---

## Material Specification

Each thief material follows PBR setup:

### `Mat_{ThiefType}.mat` (Standard Shader)

| Property | Value | Notes |
|----------|-------|-------|
| Albedo (Diffuse) | `Tex_{ThiefType}_Diffuse.png` | Character-specific texture |
| Normal Map | `Tex_Common_Normal.png` (shared) | Shared normal map |
| Metallic | 0.0 | No metallic reflection |
| Smoothness | 0.5 | Matte finish |
| Receive Shadows | FALSE (mobile optimization) | Disable for performance |

---

## Performance Targets

- **Polygon Count:** < 1000 triangles per thief (confirmed in mesh stats)
- **Texture Size:** 256×256 or smaller (mobile-optimized)
- **Collider:** CapsuleCollider (cheap, fits humanoid shape)
- **Rigidbody:** Kinematic, no gravity (no physics simulation needed)

---

## Step-by-Step Prefab Creation (Artist Workflow)

### For Each Thief Type (DesertBandit, NinjaThief, Pirate, ShadowThief):

#### 1. Import FBX Model
- Place `Thief_{Type}.fbx` in `Assets/Models/Egypt/Characters/Thieves/{Type}/`
- Configure import settings:
  - **Model Tab:** Mesh Compression: High, Read/Write: OFF, Optimize Mesh: ON
  - **Rig Tab:** Animation Type: Generic, Avatar Definition: Create From This Model
  - **Animation Tab:** Import Animation: ON, Anim Compression: Optimal
- Click Import

#### 2. Create Animator Controller
- Right-click in folder → Create → Animator Controller
- Name: `Thief_{Type}_AnimatorController`
- Open controller, add parameters: `IsRunning` (bool)
- Create states: Idle, Run
- Wire transitions (see State Machine section above)
- Assign animation clips: Idle → `Thief_{Type}_Idle.anim`, Run → `Thief_{Type}_Run.anim`

#### 3. Create Material
- Right-click in folder → Create → Material
- Name: `Mat_{Type}`
- Shader: Standard
- Assign Albedo texture: `Tex_{Type}_Diffuse.png`
- Assign Normal Map: Shared normal texture
- Set Smoothness: 0.5
- Uncheck: Receive Shadows

#### 4. Create Prefab
- In Scene, create empty GameObject: `Thief_{Type}`
- Add components (see table above):
  - Animator → assign `Thief_{Type}_AnimatorController`
  - CapsuleCollider → radius 0.4, height 1.8, center (0, 0.9, 0), **Is Trigger: TRUE**
  - Rigidbody → Body Type: Dynamic, Gravity: OFF, Freeze Rotation X/Y/Z
  - SkinnedMeshRenderer (from FBX) → Material: `Mat_{Type}`
- Set Tag: "Enemy"
- Drag GameObject into folder → Create Prefab
- Delete from scene

#### 5. Validate & Test
- Open prefab, verify all components present
- Play in Editor → check animations transition correctly
- Mesh stats: confirm < 1000 tris
- In Scene, instantiate prefab → verify collider visible (wireframe)

---

## Integration Testing

After all 4 prefabs created:

### Test Checklist

- [ ] ThiefSpawner has all 4 prefabs assigned in inspector
- [ ] Run game → spawn thief at Ahead position
- [ ] Verify: Thief plays Idle, switches to Run when approaching player
- [ ] Verify: CapsuleCollider detects player (triggers hit logic)
- [ ] Verify: No animation jitter or missing materials
- [ ] Verify: Mobile device (or emulation) renders all 4 without stutter

---

## Dependencies

- **Requires:** FBX models for each thief (DesertBandit, NinjaThief, Pirate, ShadowThief)
- **Requires:** Animator animation clips (Idle, Run for each type)
- **Requires:** Diffuse textures for each type
- **Requires:** Shared normal map
- **Blocked by:** ThiefAI.cs component (not yet created — needed for behavior)
- **Wired to:** ThiefSpawner.cs (expects these prefabs in list)

---

## Next Actions

1. **Create Prefab for DesertBandit** (Child issue: AIG-84.1)
2. **Create Prefab for NinjaThief** (Child issue: AIG-84.2)
3. **Create Prefab for Pirate** (Child issue: AIG-84.3)
4. **Create Prefab for ShadowThief** (Child issue: AIG-84.4)
5. **Wire ThiefSystem.SpawnRandomThief()** (Child issue: AIG-84.5 — Unity Developer)
6. **Create Test Scene & Verify Spawn** (Child issue: AIG-84.6)

---

## Notes

- All animations must have **Loop Pose: Enabled** (Idle, Run loops continuously)
- Rigidbody must be **kinematic** — no physics solver needed
- CapsuleCollider **must be trigger** for detection, not physical collision
- Animator parameters are **case-sensitive** — use exact names: `IsRunning`
