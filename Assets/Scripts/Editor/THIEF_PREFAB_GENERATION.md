# Thief Prefab Generation Guide

**Issue:** [AIG-95](/AIG/issues/AIG-95), [AIG-96](/AIG/issues/AIG-96), [AIG-97](/AIG/issues/AIG-97), [AIG-98](/AIG/issues/AIG-98)  
**Status:** Ready to generate  
**Last Updated:** 2026-05-07

---

## Quick Start

### Option A: Auto-Generate Placeholders (Recommended)

1. **Ensure Unity is open** with the Camel Runner project loaded
2. **Verify folders exist:** `Assets/Models/Egypt/Characters/Thieves/{DesertBandit,NinjaThief,Pirate,ShadowThief}/`
3. **From menu:** `Tools → Camel Runner → Generate Thief Placeholder Prefabs`
4. **Result:** All 4 thief prefabs created with placeholder geometry, colliders, and animator setup

**Output:**
```
Assets/Models/Egypt/Characters/Thieves/
├── DesertBandit/
│   ├── Thief_DesertBandit.prefab ✓ Created
│   └── Thief_DesertBandit_AnimatorController.controller ✓ Created
├── NinjaThief/
│   ├── Thief_NinjaThief.prefab ✓ Created
│   └── Thief_NinjaThief_AnimatorController.controller ✓ Created
├── Pirate/
│   ├── Thief_Pirate.prefab ✓ Created
│   └── Thief_Pirate_AnimatorController.controller ✓ Created
└── ShadowThief/
    ├── Thief_ShadowThief.prefab ✓ Created
    └── Thief_ShadowThief_AnimatorController.controller ✓ Created
```

### Option B: Validate Generated Prefabs

Run this to check all 4 prefabs have correct components:

**Menu:** `Tools → Camel Runner → Validate Thief Prefabs`

**Output Example:**
```
✓ Valid: DesertBandit
✓ Valid: NinjaThief
✓ Valid: Pirate
✓ Valid: ShadowThief
Validation complete: 4/4 prefabs valid
```

---

## What Gets Created

### Placeholder Prefab Structure

Each generated prefab includes:

| Component | Config | Purpose |
|-----------|--------|---------|
| **Animator** | Empty controller (ready for anim clips) | Animation playback |
| **CapsuleCollider** | Radius 0.4, Height 1.8, Center (0, 0.9, 0), **Trigger: TRUE** | Enemy detection |
| **Rigidbody** | Kinematic, Gravity OFF, Freeze Rotation | Kinematic movement |
| **Model (Capsule)** | Colored placeholder geometry | Visual representation |
| **Tag** | "Enemy" | For PlayerController hit detection |

### Animator Controller Setup

Each controller includes 4 parameters (ready for animation wiring):
- `IsRunning` (bool) — idle/run state
- `Jump` (trigger) — reserved for future
- `Slide` (trigger) — reserved for future
- `Hit` (trigger) — reserved for future

### Placeholder Colors (for visual distinction)

- **DesertBandit:** Tan (0.9, 0.7, 0.4)
- **NinjaThief:** Dark gray (0.2, 0.2, 0.2)
- **Pirate:** Brown (0.4, 0.2, 0.1)
- **ShadowThief:** Dark blue (0.1, 0.1, 0.2)

---

## Next Steps After Generation

### 1. Test Placeholder Spawning

```csharp
// In PlayMode, confirm prefabs instantiate without errors:
ThiefSpawner spawner = FindObjectOfType<ThiefSpawner>();
spawner.SpawnThief(ThiefSystem.ThiefType.DesertBandit, ThiefSystem.ThiefSpawnPosition.Ahead);
```

### 2. Replace Placeholder Models with Real FBX

Once character models are ready:

1. **Import FBX:** Place `Thief_{Type}.fbx` in the thief folder
2. **Open prefab** in editor
3. **Delete placeholder capsule** from the prefab hierarchy
4. **Drag FBX model** onto the prefab as child
5. **Adjust collider** if needed (should match model height/width)
6. **Save prefab**

### 3. Add Real Animations

Once animation clips are created:

1. **Create animator clips** (Idle, Run) in the thief folder
2. **Open animator controller** (double-click: `Thief_{Type}_AnimatorController.controller`)
3. **Create states:** Idle, Run
4. **Assign animation clips**
5. **Wire transitions:** Idle ↔ Run on `IsRunning` bool

---

## Troubleshooting

### "Folder not found" Error

**Issue:** `Assets/Models/Egypt/Characters/Thieves/{Type}/` doesn't exist

**Fix:**
```
Create folders manually or run:
Assets/Models/Egypt/Characters/Thieves/
├── DesertBandit/
├── NinjaThief/
├── Pirate/
└── ShadowThief/
```

### Validation shows "Invalid"

**Issue:** Prefab missing required component

**Fix:**
1. Open prefab in editor
2. Verify it has: Animator, CapsuleCollider, Rigidbody, Tag "Enemy"
3. Regenerate: `Tools → Camel Runner → Generate Thief Placeholder Prefabs`

### Prefab already exists

**Issue:** Trying to generate but prefab already created

**Behavior:** Generator will overwrite with fresh placeholder (safe to re-run)

---

## Integration with ThiefSpawner

Once prefabs are generated and assigned:

```csharp
// In ThiefSpawner Inspector (already hooked up):
Thief Prefabs (size 4):
  [0] DesertBandit → Thief_DesertBandit.prefab
  [1] NinjaThief  → Thief_NinjaThief.prefab
  [2] Pirate      → Thief_Pirate.prefab
  [3] ShadowThief → Thief_ShadowThief.prefab
```

Run game → ThiefSystem triggers spawner → correct prefab instantiates

---

## Notes

- **Placeholder geometry is just for testing** — real models will replace the capsule
- **All animator parameters are case-sensitive** — use exact names above
- **CapsuleCollider must be trigger** — no physical collision needed (kinematic movement)
- **Safe to regenerate** — existing prefabs will be overwritten with fresh setup
