# Camel Runner Character Asset Naming Conventions

**Last Updated:** 2026-05-07  
**Version:** 1.0  
**Target:** Unity Mobile (iOS/Android)  
**Performance Budget:** < 2000 tris per character (all LODs combined)

---

## 1. Folder Structure

All character assets follow this hierarchy to maintain organization and support Unity's meta file system:

```
Assets/
├── Models/Egypt/Characters/
│   ├── Camel/
│   │   ├── Models/
│   │   │   ├── Camel_Base.fbx (rigged, animated)
│   │   │   └── Camel_Base@animations.fbx (if using separate animation file)
│   │   ├── Materials/
│   │   │   ├── Mat_Camel_Base.mat
│   │   │   ├── Mat_Camel_Pharaoh.mat
│   │   │   ├── Mat_Camel_Racing.mat
│   │   │   ├── Mat_Camel_Mummy.mat
│   │   │   └── Mat_Camel_Golden.mat
│   │   ├── Textures/
│   │   │   ├── Tex_Camel_Base_Diffuse.png
│   │   │   ├── Tex_Camel_Pharaoh_Diffuse.png
│   │   │   ├── Tex_Camel_Racing_Diffuse.png
│   │   │   ├── Tex_Camel_Mummy_Diffuse.png
│   │   │   ├── Tex_Camel_Golden_Diffuse.png
│   │   │   ├── Tex_Camel_Normal.png (shared)
│   │   │   └── Tex_Camel_Metallic.png (optional, shared)
│   │   ├── Animations/
│   │   │   ├── Camel_Run.anim
│   │   │   ├── Camel_Idle.anim
│   │   │   ├── Camel_Jump.anim
│   │   │   ├── Camel_Slide.anim
│   │   │   ├── Camel_Hit.anim
│   │   │   ├── Camel_LaneL.anim
│   │   │   └── Camel_LaneR.anim
│   │   ├── Animator/
│   │   │   └── Camel_AnimatorController.controller
│   │   └── Prefabs/
│   │       ├── Camel_Base.prefab
│   │       ├── Camel_Pharaoh.prefab
│   │       ├── Camel_Racing.prefab
│   │       ├── Camel_Mummy.prefab
│   │       └── Camel_Golden.prefab
│   │
│   └── Thieves/
│       ├── DesertBandit/
│       │   ├── Thief_DesertBandit.fbx
│       │   ├── Thief_DesertBandit_Idle.anim
│       │   ├── Thief_DesertBandit_Run.anim
│       │   ├── Mat_DesertBandit.mat
│       │   └── Thief_DesertBandit.prefab
│       ├── NinjaThief/
│       │   ├── Thief_NinjaThief.fbx
│       │   ├── Thief_NinjaThief_Idle.anim
│       │   ├── Thief_NinjaThief_Run.anim
│       │   ├── Mat_NinjaThief.mat
│       │   └── Thief_NinjaThief.prefab
│       ├── Pirate/
│       │   ├── Thief_Pirate.fbx
│       │   ├── Thief_Pirate_Idle.anim
│       │   ├── Thief_Pirate_Run.anim
│       │   ├── Mat_Pirate.mat
│       │   └── Thief_Pirate.prefab
│       └── ShadowThief/
│           ├── Thief_ShadowThief.fbx
│           ├── Thief_ShadowThief_Idle.anim
│           ├── Thief_ShadowThief_Run.anim
│           ├── Mat_ShadowThief.mat
│           └── Thief_ShadowThief.prefab
```

---

## 2. Model Files (FBX)

### Naming Pattern: `{CharacterName}_Base.fbx`

| Asset | Name | Requirements |
|-------|------|--------------|
| **Camel Base Model** | `Camel_Base.fbx` | Rigged, all 7 animations baked in, PBR-ready |
| **Desert Bandit** | `Thief_DesertBandit.fbx` | Rigged, run + idle animations |
| **Ninja Thief** | `Thief_NinjaThief.fbx` | Rigged, run + idle animations |
| **Pirate** | `Thief_Pirate.fbx` | Rigged, run + idle animations |
| **Shadow Thief** | `Thief_ShadowThief.fbx` | Rigged, run + idle animations |

### FBX Import Settings (Unity)

```
Model Tab:
  - Mesh Compression: High
  - Read/Write Enabled: OFF (unless runtime mesh manipulation needed)
  - Optimize Mesh: ON
  - Generate Colliders: OFF (use Unity capsules instead)
  - Model Importer → Rig Tab:
    - Animation Type: Humanoid (Camel) / Generic (Thieves)
    - Avatar Definition: Create From This Model
  - Model Importer → Animation Tab:
    - Import Animation: ON
    - Bake into Pose: OFF
    - Anim. Compression: Optimal
```

---

## 3. Materials & Textures

### Material Naming: `Mat_{CharacterName}_{VariantName}.mat`

| Skin Variant | Material File | Diffuse Texture | Notes |
|---|---|---|---|
| Camel (Base) | `Mat_Camel_Base.mat` | `Tex_Camel_Base_Diffuse.png` | Standard desert tan |
| Pharaoh | `Mat_Camel_Pharaoh.mat` | `Tex_Camel_Pharaoh_Diffuse.png` | Gold + jeweled ornaments |
| Racing | `Mat_Camel_Racing.mat` | `Tex_Camel_Racing_Diffuse.png` | Sleek, aerodynamic stripes |
| Mummy | `Mat_Camel_Mummy.mat` | `Tex_Camel_Mummy_Diffuse.png` | Wrapped linen appearance |
| Golden | `Mat_Camel_Golden.mat` | `Tex_Camel_Golden_Diffuse.png` | Shiny gold coat |

### Texture Specifications

- **Format:** PNG (24-bit RGB for diffuse, 32-bit RGBA for transparency)
- **Dimensions:** Power of 2 (1024×1024, 512×512, 256×256) — NO 2048×2048 for mobile
- **Normal Map:** `Tex_Camel_Normal.png` (shared across all skins)
- **Compression:** 
  - Diffuse: RGB Compressed DXT1 (or ETC2 for Android)
  - Normal: RGB Compressed DXT5 (or ETC2 for Android)
  - Transparency: RGBA 16-bit (if needed)

### Material Setup (Shader Standard)

- Use **Standard** or **Standard (Specular setup)** shader
- Properties:
  - Albedo: Diffuse texture (variant-specific)
  - Normal Map: Shared `Tex_Camel_Normal.png`
  - Metallic: 0.0 (or from texture if PBR workflow)
  - Smoothness: 0.5 (adjustable per skin)
- **Mobile Optimization:**
  - Disable receive shadows if not critical
  - Use mobile-optimized variants of Standard shader

---

## 4. Animations

### Animation File Naming: `{CharacterName}_{AnimationName}.anim`

**Camel Animations:**

| Animation | File Name | Frames | FPS | Loop | Notes |
|-----------|-----------|--------|-----|------|-------|
| Run | `Camel_Run.anim` | 36 | 60 | YES | Happy blend @ 0.5, smooth stride |
| Idle | `Camel_Idle.anim` | 60 | 60 | YES | Gentle sway + breathing |
| Jump | `Camel_Jump.anim` | 48 | 60 | NO | Arc: 0 → -0.3 → +2.0 Y |
| Slide | `Camel_Slide.anim` | 30 | 60 | NO | Compress Y by -0.8 units |
| Hit Reaction | `Camel_Hit.anim` | 42 | 60 | NO | Startled → Determined blend |
| Lane Left | `Camel_LaneL.anim` | 24 | 60 | NO | Hips -1.0 X, spine ±20° |
| Lane Right | `Camel_LaneR.anim` | 24 | 60 | NO | Hips +1.0 X, spine ±20° |

**Thief Animations:**

| Animation | File Pattern | Frames | FPS | Notes |
|-----------|--------------|--------|-----|-------|
| Idle | `Thief_{Type}_Idle.anim` | 40 | 60 | Varies per thief type |
| Run | `Thief_{Type}_Run.anim` | 40 | 60 | Varies per thief type |
| Catch Animation | `Thief_{Type}_Catch.anim` | 30 | 60 | Optional: thief catches player |

### Import Settings (Animation Clips)

- Clip Editor:
  - Loop Pose: Enabled for loopable animations (Run, Idle)
  - Loop Pose: Disabled for one-shots (Jump, Slide, Hit)
  - Root Motion: OFF (PlayerController drives movement)

---

## 5. Animator Controller

### File: `Camel_AnimatorController.controller`

**Parameters:**
- `IsRunning` (bool) — drives Idle ↔ Run transition
- `Jump` (trigger) — one-time jump trigger
- `Slide` (trigger) — one-time slide trigger
- `Hit` (trigger) — one-time hit reaction

**State Machine (Camel):**

```
┌─────────────────────────┐
│     Idle (default)      │
│  Play: Camel_Idle.anim  │
└────────────┬────────────┘
             │ IsRunning=true
             ↓
┌─────────────────────────┐
│  Run (looping)          │
│  Play: Camel_Run.anim   │
└────────────┬────────────┘
             │ IsRunning=false
             ↑
         ┌───┴────┐
         │        │
      Jump ↓      ↓ Slide
    ┌─────┴──┐ ┌──┴────┐
    │ Jump   │ │ Slide  │
    │ (24f)  │ │ (18f)  │
    └────┬───┘ └───┬────┘
         └────┬────┘
              ↓
    Any State → Hit (42f)
```

---

## 6. Prefabs

### Camel Skin Prefabs

**Naming:** `Camel_{SkinVariant}.prefab`

**Required Components:**
```
GameObject: "Camel_{SkinVariant}"
├── Transform
├── Animator (using Camel_AnimatorController.controller)
├── CharacterController (Height: 1.8, Center: (0, 0.9, 0))
├── PlayerController.cs
├── [Model - SkinnedMeshRenderer]
│   └── Material: Mat_Camel_{SkinVariant}.mat
```

**Prefab Variants:**
- `Camel_Base.prefab` (template - use as base for others)
- `Camel_Pharaoh.prefab` (variant of Base)
- `Camel_Racing.prefab` (variant of Base)
- `Camel_Mummy.prefab` (variant of Base)
- `Camel_Golden.prefab` (variant of Base)

### Thief Prefabs

**Naming:** `Thief_{ThiefType}.prefab`

**Required Components:**
```
GameObject: "Thief_{ThiefType}"
├── Transform
├── Animator (using Thief_{Type}_AnimatorController.controller)
├── CapsuleCollider (trigger, for thief detection)
├── Rigidbody (isKinematic=true, gravity off)
├── [Model - SkinnedMeshRenderer]
│   └── Material: Mat_{ThiefType}.mat
├── ThiefAI.cs (custom, not yet created)
```

**Prefabs:**
- `Thief_DesertBandit.prefab`
- `Thief_NinjaThief.prefab`
- `Thief_Pirate.prefab`
- `Thief_ShadowThief.prefab`

---

## 7. Performance Budget

### Polygon Count Targets

**Camel (All Skins):**
- Mesh: ≤ 1200 triangles
- Normal maps + textures: ≤ 800 tri equivalent memory
- **Total per camel:** < 2000 tris

**Thieves (Each Type):**
- Mesh: ≤ 1000 triangles
- **Total per thief:** < 2000 tris

### Memory Footprint (Runtime)

| Asset Type | Size Budget | Notes |
|---|---|---|
| Camel Base Model | 2.0 MB (GPU VRAM) | All skins share mesh, vary materials only |
| 5 Camel Materials | 0.5 MB | 256×256 diffuse textures × 5 |
| Thief Models × 4 | 2.0 MB | 4 separate meshes, ~500 tri each |
| Animations (all) | 0.3 MB | 7 camel + 8 thief anims in archive |

**Total Character Budget:** ≤ 5.0 MB (loading at startup)

---

## 8. Validation Checklist

Before committing assets to the repo, verify:

- [ ] All FBX files imported with correct rig & animation settings
- [ ] Animator Controller has all 4 parameters (IsRunning, Jump, Slide, Hit)
- [ ] All animations loop correctly in preview
- [ ] Materials assigned to mesh in prefabs
- [ ] Textures compressed (no uncompressed 32-bit in final build)
- [ ] CharacterController collider dimensions match rigged model height
- [ ] Camel prefab tri count ≤ 2000 (use Profiler → Mesh Stats)
- [ ] Thief prefabs tri count ≤ 2000 each
- [ ] SkinManager.availableSkins matches prefab list (5 skins)
- [ ] ThiefSystem thief types match prefab variants (4 types)
- [ ] All prefabs instantiate without null reference errors
- [ ] Mobile platform (Android/iOS) rendering passes (device or Editor mobile emulation)

---

## 9. Integration Points

### PlayerController Hook

```csharp
// In PlayerController, when wiring model on-scene:
Animator anim = GetComponent<Animator>();
anim.SetBool(Animator.StringToHash("IsRunning"), true/false);
```

### SkinManager Hook

```csharp
// In SkinManager.EquipSkin():
public void EquipSkin(string skinName)
{
    // ... existing logic ...
    PlayerController.Instance.SwapCharacterModel(equippedSkin.skinPrefab);
    // (ApplySkin method placeholder at line 158)
}
```

### ThiefSystem Hook

```csharp
// In ThiefSystem.SpawnRandomThief():
ThiefType spawnedType = availableThieves[Random.Range(0, availableThieves.Count)];
Instantiate(GetThiefPrefab(spawnedType), spawnPosition, Quaternion.identity);
```

---

## 10. Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2026-05-07 | Initial spec. Folder structure, naming conventions, animator parameters, material templates, performance budgets |

