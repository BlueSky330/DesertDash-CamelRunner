# Thief Variant Design Specification

**Issue:** [AIG-80](/AIG/issues/AIG-80)  
**Date:** 2026-05-07  
**Asset Source:** Kenney.nl Character Pack (humanoid base)  
**Target:** 4 thief variants with material + texture variations  

---

## Overview

This document specifies the visual design and material approach for 4 thief character variants derived from a base Kenney.nl humanoid mesh. Each variant uses the same base geometry with distinct material swaps and retexturing to create unique visual identities.

**Design Philosophy:** Low-poly cartoonish aesthetic, < 2000 tris per variant, material-based differentiation

---

## Base Model Selection

### Kenney Character Pack — Suitable Models

From Kenney.nl Character Pack, select a humanoid base with:
- ✓ ~500-1000 tris (allows room for 4 variants under 2000 tris total per thief)
- ✓ Clear silhouette (works at small on-screen scale)
- ✓ Simple body structure (head, torso, arms, legs)
- ✓ Neutral pose (ready for animation)

**Recommended:** Look for male or unisex humanoid model labeled "Basic Human" or "Soldier" variant

---

## Thief Variant Specifications

### 1. Desert Bandit

**Visual Theme:** Middle Eastern / North African desert raider  
**Key Visual Elements:**
- Hooded head covering (wraps around face)
- Sandy/tan robes over simple tunic
- Coin purse/pouch at waist (visual accent)
- Covered face (mysterious, only eyes visible)
- Weathered, sun-worn appearance

**Material Approach:**
- **Base Texture:** Sandy tan (Albedo: #D4A76A, RGB 212 167 106)
- **Cloth (robes):** Matte tan linen texture
- **Metal (coin pouch):** Gold/bronze metallic accent
- **Accessories:** Rope, leather wraps around arms/legs

**Texture Specifications:**
- Diffuse: 256×256, sandy fabric pattern
- Normal: Linen weave pattern (shared across variants where possible)
- Metallic: Small gold accents on coin pouch (255 metallic on pouch area)

**Silhouette:** Bulky robes create wider silhouette than base, appears heavier/weathered

---

### 2. Ninja Thief

**Visual Theme:** Stealthy Asian-inspired night assassin  
**Key Visual Elements:**
- All-black outfit (stealth aesthetic)
- Fitted, athletic body suit (movement-ready)
- Throwing stars visible on chest/back (visual flavor)
- Mask covering lower face (classic ninja design)
- Headband with white accents (forehead)

**Material Approach:**
- **Base Texture:** Deep black (Albedo: #1A1A1A, RGB 26 26 26)
- **Cloth (suit):** Matte black fabric with subtle weave
- **Metal (throwing stars):** Silver/steel metallic shine
- **Accents:** White cloth (mask, headband) with texture contrast

**Texture Specifications:**
- Diffuse: 256×256, tight fabric pattern (small weave)
- Normal: Tightly woven ninja cloth texture
- Metallic: Silver for throwing star props (visible on model)

**Silhouette:** Sleek and fitted, minimal volume, designed for agility

---

### 3. Pirate

**Visual Theme:** Classic Caribbean seafarer with personality  
**Key Visual Elements:**
- Oversized pirate hat (distinctive silhouette)
- Eye patch (classic pirate feature)
- Tattered, patched clothing (worn, adventurous look)
- Gold/brass accents (belt buckle, buttons)
- Striped shirt visible under coat
- Weathered rope and chain accessories

**Material Approach:**
- **Hat:** Dark navy/burgundy (Albedo: #3D1F1F) with gold band
- **Clothing:** Layered (striped shirt: #FFFFFF + #8B4513 red-brown)
- **Coat:** Darker burgundy/brown (#5C4033) with patched texture
- **Metal:** Brass/gold (belt, buttons, buckles) — highly metallic
- **Rope/Chain:** Tan/brown rope texture on shoulders

**Texture Specifications:**
- Diffuse: 256×256, multiple layers (stripes for shirt, patches for coat)
- Normal: Worn fabric, frayed edges
- Metallic: Gold for all metal hardware (brassy appearance)

**Silhouette:** Bulky and eccentric, hat adds height, creates memorable profile

---

### 4. Shadow Thief

**Visual Theme:** Supernatural/ghostly dark entity  
**Key Visual Elements:**
- Flowing, semi-transparent cloak/robe
- Glowing eyes (magical appearance)
- Dark purple/blue color scheme (otherworldly)
- Minimal facial features (mysterious, not fully human)
- Ethereal, floating aesthetic (aura effect)

**Material Approach:**
- **Base Texture:** Dark purple-blue (Albedo: #2A1A3D, RGB 42 26 61)
- **Cloak:** Semi-transparent with gradient to black edges
- **Eyes:** Glowing cyan/bright blue (emissive material)
- **Aura:** Subtle glow/halo effect around silhouette (edge detection or transparency gradient)
- **Overall:** Darker, more mysterious than other variants

**Material Settings:**
- **Transparency:** Alpha blend for cloak (opacity 0.6-0.8)
- **Emissive:** Bright cyan eyes (emission strength: 2.0)
- **Diffuse:** 256×256, flowing cloth pattern with gradient

**Texture Specifications:**
- Diffuse: Dark purple-blue with flowing fabric pattern
- Normal: Flowing cape/cloak texture
- Emissive: Bright cyan for eye regions only

**Silhouette:** Taller-appearing due to flowing cloak, ethereal and hard to read

---

## Technical Specifications

### Polygon Budget

| Variant | Base Mesh | Additions | Total | Status |
|---------|-----------|-----------|-------|--------|
| **Desert Bandit** | 500-600 | Coin pouch (+100) | ~700 | ✓ Under budget |
| **Ninja Thief** | 500-600 | Throwing stars (+100) | ~700 | ✓ Under budget |
| **Pirate** | 500-600 | Hat (+150) | ~750 | ✓ Under budget |
| **Shadow Thief** | 500-600 | Cloak (geometry) (+200) | ~800 | ✓ Under budget |

**Total per variant:** < 1000 tris (allows headroom within 2000 tri limit for LOD versions if needed)

### Texture Requirements

**All variants share:**
- Single 256×256 diffuse atlas (OR 4 separate 256×256 textures, one per variant)
- Shared normal map (256×256)

**Option A (Recommended):** Separate 256×256 diffuse per variant
- Simpler material setup
- Easier to modify individual variants
- Material: `Mat_DesertBandit.mat`, etc.

**Option B:** 512×512 atlas with all 4 variants
- Fewer materials, more memory-efficient
- Requires proper UV layout
- Better for very high-instance scenarios (not needed for this game)

---

## Workflow: Creating Variants from Kenney Base

### Step 1: Import Base Humanoid from Kenney
1. Download Kenney.nl Character Pack (free, no license issues)
2. Select male/unisex humanoid model (~500-600 tris)
3. Export as FBX or import directly to Blender
4. Name: `Thief_BaseHumanoid.fbx`

### Step 2: Create Desert Bandit Variant
1. **In Blender (or 3D tool):**
   - Duplicate base mesh
   - Add hood geometry (simple plane/polys extending from head)
   - Add coin pouch (simple cube/sphere with geometry)
   - Retopologize/simplify to stay under 700 tris
   - UV unwrap for 256×256 texture
2. **Create Material in Blender:**
   - Sandy tan base color
   - Normal map: Linen weave
   - Metallic: 0.8 for coin pouch, 0.0 for cloth
3. **Export:** `Thief_DesertBandit.fbx`
4. **Create Texture:** Bake or paint 256×256 diffuse map

### Step 3: Create Ninja Thief Variant
1. **In Blender:**
   - Duplicate base mesh
   - Simplify/retopologize to fitted body suit (remove volume)
   - Add throwing stars as props (flat geometry on chest)
   - Add mask/headband (simple cloth geometry)
   - Target: ~700 tris
2. **Create Material:**
   - Deep black Albedo
   - Tight weave normal map
   - Metallic: 0.9 for throwing stars, 0.0 for cloth
3. **Export:** `Thief_NinjaThief.fbx`
4. **Create Texture:** Black fabric with subtle weave pattern

### Step 4: Create Pirate Variant
1. **In Blender:**
   - Duplicate base mesh
   - Add pirate hat (cone-shaped, ~150 tris)
   - Add layered clothing (coat, striped shirt details)
   - Add eye patch (simple plane on face)
   - Add belt/buckle (metallic accent geometry)
   - Target: ~750 tris
2. **Create Material:**
   - Burgundy/brown base for coat
   - White + red-brown stripes for shirt
   - Gold metallic for buckles/buttons
3. **Export:** `Thief_Pirate.fbx`
4. **Create Texture:** Layered pattern with stripes and patches

### Step 5: Create Shadow Thief Variant
1. **In Blender:**
   - Duplicate base mesh
   - Add flowing cloak geometry (simple planes, arranged around body)
   - Simplify features (minimal facial details)
   - Make body less detailed, focus on silhouette
   - Target: ~800 tris (allows for cloak complexity)
2. **Create Material:**
   - Dark purple-blue Albedo
   - Flowing fabric normal map
   - Emissive material for eyes (cyan, bright)
3. **Export:** `Thief_ShadowThief.fbx`
4. **Create Texture:** Dark purple gradient with flowing pattern

---

## Folder Structure & File Layout

```
Assets/Models/Egypt/Characters/Thieves/
├── DesertBandit/
│   ├── Thief_DesertBandit.fbx
│   ├── Tex_DesertBandit_Diffuse.png
│   ├── Mat_DesertBandit.mat
│   ├── Thief_DesertBandit_AnimatorController.controller
│   └── Thief_DesertBandit.prefab
├── NinjaThief/
│   ├── Thief_NinjaThief.fbx
│   ├── Tex_NinjaThief_Diffuse.png
│   ├── Mat_NinjaThief.mat
│   ├── Thief_NinjaThief_AnimatorController.controller
│   └── Thief_NinjaThief.prefab
├── Pirate/
│   ├── Thief_Pirate.fbx
│   ├── Tex_Pirate_Diffuse.png
│   ├── Mat_Pirate.mat
│   ├── Thief_Pirate_AnimatorController.controller
│   └── Thief_Pirate.prefab
└── ShadowThief/
    ├── Thief_ShadowThief.fbx
    ├── Tex_ShadowThief_Diffuse.png
    ├── Mat_ShadowThief.mat
    ├── Thief_ShadowThief_AnimatorController.controller
    └── Thief_ShadowThief.prefab
```

---

## Shared Assets

- **Normal Map:** `Tex_Common_Normal.png` (256×256) — shared across all variants
- **Animator Controller Parameters:** Identical for all variants (IsRunning, Jump, Slide, Hit)
- **Animation Clips:** Same run/idle cycles per thief (variations in style TBD)

---

## Quality Checklist

Before committing FBX exports:

- [ ] All 4 variants imported without errors
- [ ] Triangle count validated (DesertBandit ~700, NinjaThief ~700, Pirate ~750, ShadowThief ~800)
- [ ] Normals facing outward
- [ ] UVs unwrapped and optimized (256×256 texel density)
- [ ] Materials assigned with correct Albedo/Normal/Metallic
- [ ] Silhouettes visually distinct (each recognizable at small scale)
- [ ] Animations play correctly on each variant (run cycle matches timing)
- [ ] Mobile rendering (check on device or emulation) — no stutter

---

## Next Actions

1. **Model Creation (Blender/Maya):**
   - Download Kenney Character Pack
   - Select base humanoid
   - Create 4 variants per specs above
   - Export as FBX to their folders

2. **Texture Baking/Creation:**
   - Bake or paint 256×256 diffuse textures for each variant
   - Create/assign shared normal map

3. **Material Setup:**
   - Create .mat files in Unity
   - Assign textures and metallic properties per variant

4. **Prefab Generation:**
   - Use ThiefPrefabGenerator utility (from AIG-84 infrastructure)
   - Or manually build prefabs with FBX + materials + animator setup

5. **Animation & Testing:**
   - Import run/idle animation clips
   - Test each variant in-game

---

## Notes

- **Kenney assets are CC0** — no licensing issues, free to use/modify
- **Variants use same animator parameters** — switching between thieves just changes mesh/material
- **Material swapping is efficient** — only materials differ at runtime, not geometry or rig
- **Performance target:** All 4 variants visible on-screen without frame drops (tested on mobile device)
