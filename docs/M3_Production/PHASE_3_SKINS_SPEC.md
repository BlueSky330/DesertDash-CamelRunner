# Phase 3A: Camel Skins Specification

**Timeline**: May 26-Jun 2, 2026 (5-6 days)  
**Owner**: 3D Artist (Blender)  
**Dependency**: Phase 1 (Camel_Default.fbx rig complete) + Phase 2 (animations finalized)  
**Deliverable**: 4 skin FBX files (Camel_Pharaoh.fbx, Camel_Racing.fbx, Camel_Mummy.fbx, Camel_Golden.fbx)

---

## Overview

Phase 3A creates 4 unlockable camel skins by duplicating the base Camel_Default mesh and modifying geometry/materials. All skins use the **same skeleton and rig** as Camel_Default — no skeleton changes, only mesh/material swaps. This ensures all Phase 2 animations work perfectly on every skin without clipping or deformation issues.

**Key Constraint**: Total tri count per skin must stay under 2,000 triangles (including base camel + all accessories).

---

## Skin 1: Pharaoh Camel

**Visual Goal**: Luxurious Egyptian ruler — gold ornaments, royal headdress, gemstone accents.

### Mesh Modifications

**Base Body**:
- Copy Camel_Default mesh
- Keep silhouette identical (same rig compatibility)
- No geometry changes to body/legs/neck
- Material swap only: switch from sandy tan to light gold/tan blend

**New Geometry Components**:

1. **Nemes Headdress** (~250 tris)
   - Striped cloth headdress covering head/forehead (iconic Egyptian style)
   - Mesh structure: 2 main sections (front stripe band + rear cloth drape)
   - Positioned as child object, rigged to `Armature:Head` bone
   - Vertices: Keep under 150 to stay within tri budget

2. **Ornate Collar** (~150 tris)
   - Wide, flat golden collar sitting on shoulders
   - Multi-layered with geometric patterns (chevrons, lines)
   - Rigged to `Armature:Chest` bone
   - Vertices: ~80-90

3. **Sapphire Gem on Chest** (~50 tris)
   - Small blue gem centered on chest
   - Simple gemstone shape (8-12 faceted sides)
   - Rigged to `Armature:Chest` bone
   - Emissive material for subtle glow

4. **Golden Anklets** (~100 tris, 4x ankles)
   - Small ring-like bracelets on all 4 ankles
   - ~25 tris each
   - Rigged to respective leg bones (`Armature:UpperLegL/R`, `Armature:LowerLegL/R`)

**Total New Geometry**: ~550 tris (well under 2,000 limit)

### Material Assignments

1. **Body Material** (base mesh)
   - Color: Light gold/tan blend (RGB 220, 190, 130)
   - Slight metallic sheen (0.3 metallic value)
   - Normal map: Reuse from Camel_Default

2. **Headdress Material**
   - Color: Gold stripes (RGB 255, 215, 0) + cream/white stripes (RGB 240, 240, 220)
   - Pattern: Use procedural or texture-based stripes
   - Metallic: 0.4 (slightly shiny)

3. **Collar Material**
   - Color: Rich gold (RGB 255, 200, 0)
   - Pattern: Engraved lines using normal map
   - Metallic: 0.6 (shiny, reflective)

4. **Gem Material** (sapphire)
   - Color: Deep blue (RGB 25, 82, 177)
   - Metallic: 0.8, Roughness: 0.1 (sharp reflections)
   - Emissive: Blue glow (RGB 50, 150, 255) at 0.3 intensity

5. **Anklets Material**
   - Color: Gold (RGB 255, 215, 0)
   - Metallic: 0.7
   - Roughness: 0.15

### UV Layout

- **Headdress + Collar + Anklets**: New UV space in 1024x1024 atlas
- **Allocation**: Dedicate ~300x300 pixel region to all accessories
- **Overlap**: None (distinct regions for each accessory)
- **Test**: Import into Unity, verify no texture stretching

### Rigging Verification

**Rig Assignments**:
- Headdress: Parent to `Armature:Head`
- Collar: Parent to `Armature:Chest`
- Sapphire: Parent to `Armature:Chest`
- Anklets: Parent to respective leg bones

**Animation Testing**:
1. Play Run Cycle animation → verify no mesh clipping, smooth deformation
2. Play Jump animation → verify headdress/collar don't penetrate body
3. Play Slide animation → verify all accessories move with body
4. Play Hit Reaction → verify no weird mesh stretching

### Export Checklist

- [ ] Total tri count < 2,000 (run in Blender: `Shift+Z` wireframe, count faces)
- [ ] All materials assigned and visible in Blender render
- [ ] Headdress rigged to Head bone, deforms correctly
- [ ] Collar rigged to Chest bone, stays in place during animation
- [ ] Sapphire gem positioned on chest, emissive works
- [ ] All 4 anklets visible and rigged to ankle bones
- [ ] No vertex groups with 0 weight (orphaned vertices)
- [ ] FBX export settings:
  - Armature: YES (include skeleton)
  - Mesh: YES
  - Animations: NO (use Phase 2 animations)
  - FBX version: 2020
  - Export as: `Camel_Pharaoh.fbx`

### Success Criteria

✅ Visually distinct from Camel_Default (clearly "pharaoh" style)  
✅ Under 2,000 tris  
✅ All animations play without clipping  
✅ Headdress, collar, gem, anklets all visible and rigged correctly  
✅ Materials render with correct colors and metallicity  

---

## Skin 2: Racing Camel

**Visual Goal**: High-speed racer — neon colors, sleek aerodynamics, tech-inspired.

### Mesh Modifications

**Base Body**:
- Copy Camel_Default mesh
- Keep silhouette identical
- Material swap: neon yellow (RGB 255, 255, 0) with black accents

**New Geometry Components**:

1. **Racing Goggles on Face** (~200 tris)
   - Large goggles covering eyes (futuristic style)
   - 2 separate lens cups connected by bridge
   - Positions on snout/face area
   - Rigged to `Armature:Head` bone
   - Vertices: ~100-120

2. **Racing Harness** (~250 tris)
   - Sleek chest/body harness with geometric straps
   - Diagonal straps crossing chest, connects to back
   - Rigged to `Armature:Chest` + `Armature:Spine` (blend weights)
   - Vertices: ~130-150

3. **Striped Anklet Bands** (~80 tris, 4x ankles)
   - Neon yellow/black striped rings around each ankle
   - ~20 tris each
   - Rigged to ankle/leg bones
   - Procedural or UV-based stripe pattern

**Total New Geometry**: ~530 tris

### Material Assignments

1. **Body Material**
   - Color: Neon yellow (RGB 255, 255, 0)
   - Metallic: 0.5 (glossy, fast-looking)
   - Roughness: 0.2
   - Slight green/blue tint in shadows

2. **Goggles Material**
   - Lens color: Black (RGB 0, 0, 0) with neon yellow reflections
   - Frame: Yellow (RGB 255, 255, 0), metallic 0.7
   - Lens emissive: Slight glow (10% intensity)

3. **Harness Material**
   - Color: Black (RGB 20, 20, 20) with yellow accents
   - Metallic: 0.4
   - Pattern: Straps using normal map or texture

4. **Anklet Material**
   - Pattern: Neon yellow (RGB 255, 255, 0) + black (RGB 0, 0, 0) stripes
   - Metallic: 0.3
   - Roughness: 0.25

### UV Layout

- **Goggles + Harness + Anklets**: New UV region in 1024x1024 atlas
- **Allocation**: ~350x300 pixel region
- **Stripe pattern**: Use tileable texture for anklets (1:1 scale)

### Rigging Verification

**Rig Assignments**:
- Goggles: Parent to `Armature:Head`
- Harness: Parent to blend (Chest 60%, Spine 40%)
- Anklets: Parent to leg bones

**Animation Testing**:
1. Run Cycle → goggles stay on head, harness moves with body
2. Lane Switch animations → harness twists with body rotation
3. Jump → harness bends/compresses, no clipping
4. Slide → harness compresses against body

### Export Checklist

- [ ] Total tri count < 2,000
- [ ] Goggles centered on face, non-intrusive
- [ ] Harness positioned on chest/spine, flexible during animation
- [ ] Anklet stripes clearly visible
- [ ] Neon yellow color is bright and saturated
- [ ] Black accents contrast well with yellow
- [ ] All bones assigned, no orphaned vertices
- [ ] Export as: `Camel_Racing.fbx`

### Success Criteria

✅ Reads as "race car" aesthetic (sleek, fast)  
✅ Under 2,000 tris  
✅ Goggles don't block visibility in cutscenes  
✅ Harness moves naturally during animations  
✅ Neon colors pop on dark backgrounds  

---

## Skin 3: Mummy Camel

**Visual Goal**: Mystical undead — bandages, glowing eyes, tattered wrappings.

### Mesh Modifications

**Base Body**:
- Copy Camel_Default mesh
- Keep silhouette identical
- Material swap: cream/tan with matte cloth texture

**New Geometry Components**:

1. **Bandage Wraps (Full Coverage)** (~400 tris)
   - Full body bandage wrapping (like Egyptian mummy)
   - Geometry: Layered strips wrapping around torso, limbs, head
   - Key areas: Head wrap, chest wrapping, leg wraps, tail wrap
   - Rigged to matching bones (Head, Chest, Spine, Legs)
   - Vertices: ~200-220

2. **Floating Bandage Strips** (~150 tris)
   - Tattered, floating strips animating with wind/movement
   - 3-4 separate strips positioned around body
   - Parent to root bone, position using constraints or procedural animation
   - Vertices: ~80-100

3. **Glowing Eyes** (~50 tris)
   - Eyes glow with emissive yellow (RGB 255, 255, 100)
   - Separate mesh from head (allows custom shader/glow)
   - Rigged to `Armature:Head`

**Total New Geometry**: ~600 tris

### Material Assignments

1. **Body Material** (wrapped)
   - Color: Cream/beige (RGB 245, 235, 220)
   - Texture: Linen/cloth texture (wrinkles, weave pattern)
   - Metallic: 0.0 (matte)
   - Roughness: 0.9 (cloth-like)

2. **Bandage Wraps**
   - Color: Cream (RGB 240, 230, 210) with darker shadows (RGB 180, 160, 140) in folds
   - Texture: Cloth with slight tears/fraying
   - Roughness: 0.95

3. **Floating Strips**
   - Color: Cream with hints of transparency (alpha-blend in shader)
   - Material: Cloth with fluttering animation
   - Roughness: 0.9

4. **Glowing Eyes**
   - Color: Yellow (RGB 255, 255, 100)
   - Emissive: Strong glow (0.8 intensity)
   - Metallic: 0.0
   - Special shader: Add bloom effect for glow

### Special Effects

**Floating Bandage Animation** (Optional, for appeal):
- If time permits, add procedural wave animation to floating strips
- Use Blender shape keys or simple rig animation
- Wave frequency: 0.5 Hz, amplitude: 5cm
- Fade out over distance (if using particle effects)

### Rigging Verification

**Rig Assignments**:
- Bandage wraps: Parent to respective body bones (Head, Chest, Legs)
- Floating strips: Parent to root (`Armature` object)
- Glowing eyes: Parent to `Armature:Head`

**Animation Testing**:
1. Run Cycle → bandages move with body, floating strips wave
2. Jump → all bandages compress/stretch naturally
3. Slide → bandages bunch up, floating strips flutter
4. Hit Reaction → bandages shake with impact

### UV Layout

- **Bandages + Floating strips**: New UV region
- **Allocation**: ~400x300 pixels (cloth texture can tile)
- **Glow eyes**: Separate material, no UV needed (procedural)

### Export Checklist

- [ ] Total tri count < 2,000 (including floating strips)
- [ ] Bandage wraps fully cover body (visually coherent)
- [ ] Floating strips clearly visible and distinct
- [ ] Eyes glow with yellow emissive
- [ ] Cloth texture visible on bandages
- [ ] All bones assigned, no orphaned vertices
- [ ] If floating animation exists, test playback
- [ ] Export as: `Camel_Mummy.fbx`

### Success Criteria

✅ Reads as "mummy" (full wrapping, mysterious)  
✅ Under 2,000 tris  
✅ Eyes glow distinctly in dark scenes  
✅ Bandages move naturally during animations  
✅ Floating strips add visual interest  

---

## Skin 4: Golden Camel

**Visual Goal**: Luxury item — full gold with gem accents and engraved patterns.

### Mesh Modifications

**Base Body**:
- Copy Camel_Default mesh
- Keep silhouette identical
- Material swap: full metallic gold (RGB 255, 215, 0)

**New Geometry Components**:

1. **Engraved Patterns** (~200 tris)
   - Geometric etchings on body (lines, chevrons, circles)
   - Mesh details: Low-relief engraving (not full mesh modification, mostly normal map)
   - Rigged to body bones
   - Vertices: ~100-120

2. **Gem Accents** (~150 tris)
   - 4-6 small gemstones positioned on body (chest, back, legs)
   - Faceted gem shapes (8-12 sided)
   - ~25 tris each
   - Colors: Red (RGB 200, 0, 0), Blue (RGB 0, 100, 255), Green (RGB 0, 200, 0)
   - Rigged to body bones

3. **Decorative Tassels** (~100 tris)
   - Golden tassels hanging from saddle area
   - 3-4 tassels, each ~25 tris
   - Rigged to `Armature:Spine` bone, allow slight sway

**Total New Geometry**: ~450 tris

### Material Assignments

1. **Body Material** (gold)
   - Color: Rich gold (RGB 255, 215, 0)
   - Metallic: 0.95 (highly reflective)
   - Roughness: 0.1 (shiny, polished surface)
   - Micro-detail: Engraved pattern using normal map (subtle relief)

2. **Gem Materials**
   - **Red Gem**: Color (RGB 200, 0, 0), Metallic 0.8, Roughness 0.05, Emissive 0.2 (subtle glow)
   - **Blue Gem**: Color (RGB 0, 150, 255), Metallic 0.8, Roughness 0.05, Emissive 0.2
   - **Green Gem**: Color (RGB 0, 200, 0), Metallic 0.8, Roughness 0.05, Emissive 0.2
   - Shared shader: Faceted/crystalline appearance

3. **Tassel Material**
   - Color: Gold (RGB 255, 215, 0)
   - Metallic: 0.6
   - Roughness: 0.3 (slightly less shiny than body, soft appearance)
   - Texture: Fine fibrous pattern

### UV Layout

- **Engraving patterns**: Use normal map (no UV space needed)
- **Gems**: Separate UV region, ~200x200 pixels (can reuse same gem UV pattern)
- **Tassels**: ~150x100 pixels

### Rigging Verification

**Rig Assignments**:
- Body engraving: Normal map, no geometry change
- Gems: Parent to nearby body bones (some on Chest, some on Legs)
- Tassels: Parent to `Armature:Spine`, weight paint for slight bounce

**Animation Testing**:
1. Run Cycle → gems catch light, tassels sway slightly
2. Jump → gems and tassels bounce with impact
3. Slide → tassels swing forward slightly
4. Hit Reaction → gems flash with impact

### Special Effects

**Gem Reflections**:
- Use environment mapping shader to reflect surroundings
- High metallic value ensures visible reflections during gameplay
- Test in Unity with different lighting conditions

### Export Checklist

- [ ] Total tri count < 2,000
- [ ] Gold color is saturated and bright
- [ ] Gems positioned clearly on body (not hidden)
- [ ] Engraved pattern visible on gold surface (normal map working)
- [ ] Tassels hang naturally and sway slightly
- [ ] All gems have distinct colors (red, blue, green)
- [ ] All bones assigned, no orphaned vertices
- [ ] Export as: `Camel_Golden.fbx`

### Success Criteria

✅ Reads as "treasure/luxury" (full gold + gems)  
✅ Under 2,000 tris  
✅ Shiny gold reflects light realistically  
✅ Gems are colorful and eye-catching  
✅ Tassels add movement and appeal  

---

## Phase 3A Summary & Delivery

### Total Deliverables

| Skin | File | Tri Count | Key Features |
|------|------|-----------|--------------|
| Pharaoh Camel | Camel_Pharaoh.fbx | <2,000 | Headdress, collar, gem, anklets |
| Racing Camel | Camel_Racing.fbx | <2,000 | Goggles, harness, striped anklets |
| Mummy Camel | Camel_Mummy.fbx | <2,000 | Full bandages, floating strips, glow eyes |
| Golden Camel | Camel_Golden.fbx | <2,000 | Full gold, gems, tassels, engraving |

### Timeline

- **Day 1-2** (May 26-27): Model Pharaoh + Racing skins
- **Day 3-4** (May 28-29): Model Mummy + Golden skins
- **Day 5** (May 30): Animation testing on all skins, rig verification
- **Day 5-6** (May 30-Jun 1): Polish and export all FBX files
- **Jun 1-2**: Buffer for fixes/adjustments

### Quality Gate

All 4 skins must:
- ✅ Play all Phase 2 animations without clipping
- ✅ Be visually distinct from each other
- ✅ Be under 2,000 tris each
- ✅ Have all materials rendering correctly
- ✅ Import cleanly into Unity without errors

### Hand-Off Criteria

When Phase 3A is complete:
- All 4 Camel skin FBX files committed to git
- Each file tested with all 7 Phase 2 animations
- No clipping issues reported
- Tri counts verified in Blender
- Ready for Phase 3B (Thief characters)
