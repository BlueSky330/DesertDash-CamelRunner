# M3 Production Design: Characters & Animations
**Issue**: AIG-9  
**Timeline**: 2026-05-15 → 2026-06-11 (4 weeks)  
**Author**: Artist1  
**Status**: In Production

---

## 1. Production Strategy

### Approach: Sequential with Parallel Prep (Option A)
- **Rationale**: Shared rig validation critical before skin generation. Sequential mitigates rework on all skins due to base rig issues.
- **Risk Mitigation**: Early animation testing ensures rig compatibility before downstream assets.

### Timeline Breakdown

| Phase | Duration | Deliverable | Status |
|-------|----------|-------------|--------|
| **Phase 1: Camel Default + Rig** | May 15-21 (1 week) | Default model, skeleton, test rig | 🟡 Starting |
| **Phase 2: Animations** | May 22-25 (4 days) | All 7 animations on shared rig | ⬜ Pending |
| **Phase 3: Skins + Thieves** | May 26-Jun 4 (10 days) | 4 skins, 4 thief characters | ⬜ Pending |
| **Phase 4: QA & Export** | Jun 5-11 (1 week) | Performance testing, final FBX exports | ⬜ Pending |

---

## 2. Camel Default Model Spec

### Visual Description
- **Base**: Low-poly camel, sandy tan/brown, cartoonish proportions (Crossy Road style)
- **Head**: Big, expressive eyes (bright/glossy), open grin, slight underbite
- **Accessories**:
  - **Goggles**: Aviator-style, positioned on forehead (removable in rig for expressions)
  - **Saddle**: Colorful Moroccan blanket with geometric patterns, tassels hanging down
  - **Bridle**: Red leather, visible around snout area
- **Expressions** (baked into geometry + rig blendshapes):
  - Happy (default, open grin)
  - Startled (wider eyes, raised eyebrows)
  - Determined (squinting, clenched teeth look)
- **Proportions**: Exaggerated, slightly squat legs, long neck
- **Color**: Sandy tan (RGB ~210, 180, 140) with brown accents (RGB ~139, 90, 43)

### Technical Specs
- **Polygon Count**: Target <1,200 tris (safety margin below 2,000 limit)
- **Bone Count**: ~15-20 bones (head, neck, body, 4 legs, tail, expression bones)
- **Materials**: 3-4 materials (body, saddle, bridle, eyes/accessories)
- **UVs**: Single 1024x1024 texture atlas
- **Export Format**: FBX 2020 with embedded rig, no animations at this stage

### Production Steps
1. **AI Generation** (Leonardo.AI / Meshy AI): Generate low-poly base using prompt
2. **Manual Cleanup**: Remove excess geometry, ensure performance budget
3. **Topology Check**: Verify quads for animation deformation
4. **Rigging**: Create skeleton, weight painting for leg/neck movement
5. **Blendshapes**: Add expression shapes (happy/startled/determined)
6. **Material Setup**: Create materials for body, saddle, bridle, face
7. **UV Layout**: Ensure all materials fit 1024x1024 atlas

---

## 3. Animation Specification

### Shared Rig Requirement
- All animations use **single Camel skeleton** (no per-skin rig variations)
- Each skin is material/mesh swap, NOT rig swap
- Rig must handle all 7 animations without clipping on any skin

### Animation List

| Animation | Duration | Loop | Notes |
|-----------|----------|------|-------|
| **Run Cycle** | 0.6s | Yes | Forward motion, rhythmic leg/tail movement |
| **Lane Switch Left** | 0.4s | No | Slight lean left, hip shift |
| **Lane Switch Right** | 0.4s | No | Slight lean right, hip shift |
| **Jump (Launch → Peak → Land)** | 0.8s | No | Crouch → spring → arc → impact |
| **Slide/Duck** | 0.5s | No | Body lowers, front lean, quick recovery |
| **Hit Reaction / Stumble** | 0.7s | No | Flinch backward, dizzy shake, recovery |
| **Idle / Menu Pose** | 1.0s | Yes | Standing, slight weight shift, breathing sway |

### Animation Quality Standards
- **Smoothness**: No popping or T-pose transitions
- **Looping**: Run cycle and Idle loop seamlessly (no frame discontinuity)
- **Blend Safety**: Each animation starts/ends in neutral pose for safe blending
- **Performance**: All animations baked (no IK, no constraints for runtime performance)

---

## 4. Skins Specification

All skins use the **same skeleton and rig** as Camel Default. Differences are mesh/material only.

### Skin 1: Pharaoh Camel
- **Gear**: Gold Nemes headdress (striped cloth), ornate gold collar, blue sapphire gem on chest, golden anklets
- **Color Shift**: Golds dominate; body remains tan with gold accents
- **Material**: Add shiny/metallic properties to gold elements
- **Mesh Changes**: Headdress adds ~300 tris, collar ~150 tris, anklets ~100 tris (stay under 2,000 total)

### Skin 2: Racing Camel
- **Gear**: Yellow/black racing goggles on face, black/yellow racing harness, striped anklet bands (bright neon colors)
- **Color Shift**: Neon yellow (RGB ~255, 255, 0), gloss black accents
- **Material**: Glossy/metallic for racing effect
- **Mesh Changes**: Goggles on face ~200 tris, harness ~250 tris

### Skin 3: Mummy Camel
- **Gear**: Full bandage wrap (cream/beige), glowing yellow eyes, tattered wrappings floating mid-run (dynamic particles or procedural animation)
- **Color Shift**: Cream/tan, darker shadows in wrap folds
- **Material**: Matte/cloth texture for bandages, glowing emissive for eyes
- **Mesh Changes**: Bandage wraps add ~400 tris, may include floating strips as separate mesh

### Skin 4: Golden Camel
- **Gear**: Fully golden body, geometric engraved patterns (etched look), multicolored gem accents (red, blue, green), tassels hanging
- **Color Shift**: Pure gold (RGB ~255, 215, 0), gem colors bright and saturated
- **Material**: Metallic/reflective, gems use emissive
- **Mesh Changes**: Engraved patterns ~200 tris, gems ~150 tris, tassels ~100 tris

### Skin Generation Process
1. Duplicate Camel Default mesh in Blender
2. Add/modify mesh elements (headdress, harness, bandages, gems)
3. Update materials/colors in Unity (same skeleton, swapped prefabs)
4. Verify rig deformation on all animations (no clipping)
5. Export as separate FBX prefabs with same rig reference

---

## 5. Thief Characters Specification

Four unique character models, separate from Camel rig. Each has its own rig (simplified, 8-10 bones).

### Thief 1: Desert Bandit
- **Silhouette**: Hooded humanoid, sandy robes, face covered, coin bag slung across chest
- **Colors**: Sandy tan, dark browns, red accent rope
- **Size**: ~60% Camel scale
- **Tris Budget**: <1,500
- **Animations Needed**: Idle walk, run, lunge/chase, hit reaction

### Thief 2: Ninja Thief
- **Silhouette**: Sleek black outfit, mask covering face, throwing stars visible (hand/belt)
- **Colors**: Matte black, dark grey, metallic silver stars
- **Size**: ~55% Camel scale
- **Tris Budget**: <1,500
- **Animations Needed**: Idle, run, dash/pursuit, throw animation

### Thief 3: Pirate
- **Silhouette**: Classic pirate hat, eye patch, tattered coat, cutlass visible
- **Colors**: Navy blue, brown leather, gold buckles
- **Size**: ~65% Camel scale
- **Tris Budget**: <1,600
- **Animations Needed**: Idle, run, sword swing, hit reaction

### Thief 4: Shadow Thief
- **Silhouette**: Ghostly, semi-transparent, elongated limbs, glowing eyes (yellow/white)
- **Colors**: Dark grey/purple translucent, bright glowing eyes
- **Size**: ~60% Camel scale
- **Tris Budget**: <1,500
- **Animations Needed**: Idle float, glide/run, lunge, disappear/fade

### Thief Rigging
- Each thief has a **unique skeleton** (not shared with Camel)
- Simplified rigs: ~8-10 bones (pelvis, spine, chest, head, 2 arms, 2 legs)
- Baked animations (no IK)

---

## 6. Export & Integration Standards

### FBX Export Settings
- **Version**: FBX 2020 or later
- **Animation**: Embedded in FBX (animations baked, no references)
- **Rig**: Include armature/skeleton, NO IK handles
- **Naming Convention**:
  - Camel: `Camel_Default.fbx`, `Camel_Pharaoh.fbx`, etc.
  - Thieves: `Thief_Bandit.fbx`, `Thief_Ninja.fbx`, etc.
- **Folder Structure**:
  ```
  Assets/Models/
  ├── Camel/
  │   ├── Camel_Default.fbx
  │   ├── Camel_Pharaoh.fbx
  │   ├── Camel_Racing.fbx
  │   ├── Camel_Mummy.fbx
  │   └── Camel_Golden.fbx
  └── Thieves/
      ├── Thief_Bandit.fbx
      ├── Thief_Ninja.fbx
      ├── Thief_Pirate.fbx
      └── Thief_Shadow.fbx
  ```

### Unity Import Settings
- **Rig Type**: Humanoid (for Camel), Generic (for Thieves)
- **Animation Type**: All animations embedded, no external anim controller
- **Materials**: Auto-generate in import or provide custom materials
- **Optimization**: Enable mesh compression, read/write disabled

---

## 7. Quality Checklist

### Camel Default (Before Skins)
- [ ] Model geometry under 1,200 tris
- [ ] Rig deforms smoothly (test with a simple run pose)
- [ ] Expressions (happy/startled/determined) render correctly
- [ ] All 3-4 materials applied (body, saddle, bridle, eyes)
- [ ] Goggles, saddle, bridle visually match concept art
- [ ] UV layout fits 1024x1024 without overlap
- [ ] Exported FBX imports cleanly into Unity

### All Animations
- [ ] All 7 animations loop/blend correctly
- [ ] Run cycle is seamless
- [ ] Lane switches feel responsive (quick, snappy)
- [ ] Jump arc is natural, landing has impact
- [ ] Slide motion is smooth, no clipping
- [ ] Hit reaction reads as pain/surprise
- [ ] Idle breathing is subtle and continuous

### Skins (per skin)
- [ ] Visually distinct from other skins
- [ ] No mesh clipping during any animation
- [ ] Materials render as intended (gold shine, neon brightness, etc.)
- [ ] Under 2,000 tris total with all accessories

### Thieves (per thief)
- [ ] Silhouette clearly reads as intended character type
- [ ] Unique from other thieves
- [ ] Under 1,500 tris
- [ ] Rig supports basic chase/pursuit animations

---

## 8. Reference & Style Guide

### Crossy Road / Subway Surfers Reference
- **Polygon Style**: Chunky, readable silhouettes; avoid thin or spiky details
- **Proportions**: Exaggerated (big heads, squat bodies)
- **Animation**: Bouncy, snappy; full body movement (shoulders, hips)
- **Colors**: Vibrant, saturated; strong contrast between character and background
- **Eyes**: Large, expressive, often with specular highlights for "life"

### Color Palette (from GDD)
- **Dominant Warms**: Gold (RGB 255, 215, 0), Orange (RGB 255, 140, 0), Terracotta (RGB 205, 92, 0)
- **Sky/Accent**: Bright blue (RGB 0, 150, 255), Lush green (RGB 50, 205, 50)
- **Neutrals**: Sandy tan (RGB 210, 180, 140), Dark brown (RGB 139, 90, 43)

---

## 9. Blockers & Dependencies

### Dependencies
- **Camel Default** must complete before Animations can be finalized (rig structure is fixed)
- **Camel + Animations** must complete before Skins validation (skins test on animated rig)
- **No external blockers** (art is independent of coding)

### Known Risks
- **Risk 1**: AI generation may require heavy cleanup (geometry not matching low-poly standard)
  - **Mitigation**: Manual retopology if needed; ensure <1,200 tris regardless
- **Risk 2**: Shared rig may clip on some skins during certain animations
  - **Mitigation**: Weight paint adjustments, mesh optimization per skin
- **Risk 3**: Animation rigging takes longer than estimated
  - **Mitigation**: Use simplified rigs on thieves; prioritize Camel animations first

---

## 10. Next Steps

1. **[IN PROGRESS]** Generate Camel Default base mesh (Leonardo.AI/Meshy AI)
2. **[PENDING]** Manual cleanup, rigging, material setup
3. **[PENDING]** Test rig with placeholder run animation
4. **[PENDING]** Generate all 7 animations
5. **[PENDING]** Create 4 skins using default mesh as base
6. **[PENDING]** Generate 4 thief characters and rigs
7. **[PENDING]** Final QA, tri count verification, FBX export
8. **[PENDING]** Commit to Assets/Models/ with production notes

---

**Document Status**: Active Production  
**Last Updated**: 2026-05-07  
**Next Review**: After Camel Default + Rig Completion (Est. 2026-05-21)
