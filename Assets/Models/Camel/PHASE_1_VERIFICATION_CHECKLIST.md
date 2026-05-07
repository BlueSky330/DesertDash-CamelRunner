# Phase 1 Verification Checklist

**Purpose**: Verification gate before Phase 2 animation work begins  
**Owner**: Blender artist completing Phase 1  
**Date**: 2026-05-07  
**Due**: 2026-05-21

---

## Pre-Export Verification (In Blender)

### Mesh Quality
- [ ] **Tri Count**: Open Window > Statistics, verify <1,200 triangles total
- [ ] **Topology**: Switch to Wireframe (Z > Wireframe), confirm quads dominate (no ngons)
- [ ] **Silhouette**: Model reads as camel immediately (big head, squat legs, long neck)
- [ ] **Accessories Visible**:
  - [ ] Goggles geometry present on forehead
  - [ ] Saddle blanket visible on back with distinct geometry
  - [ ] Bridle visible around snout area

### Rig Quality
- [ ] **Bone Count**: Outliner shows exactly 13 bones (Root, Hips, Spine, Chest, Neck, Head, LeftEye, RightEye, LeftFrontLeg, RightFrontLeg, LeftBackLeg, RightBackLeg, Tail)
- [ ] **Bone Structure**: Parent-child hierarchy correct (test by rotating bones in Pose Mode)
- [ ] **Deformation**: 
  - [ ] Neck rotates smoothly (no sharp creases)
  - [ ] Legs bend at joints naturally
  - [ ] Tail swings independently
  - [ ] Head follows neck movement
- [ ] **No T-Pose Clipping**: Rotate all bones 90°, verify no interpenetration

### Expressions (Shape Keys)
- [ ] **Basis Shape Key**: Present and selected
- [ ] **Happy Shape Key**: Created, testable in Shape Key panel, mouth open and eyes squinted
- [ ] **Startled Shape Key**: Created, testable, eyes wide open and eyebrows raised
- [ ] **Determined Shape Key**: Created, testable, eyes narrow and clenched look
- [ ] **Slider Test**: Adjust each shape key slider 0-1, confirm visual changes

### Materials & Texturing
- [ ] **Material Slots**: 4 materials assigned (Body, Saddle, Bridle, Eyes)
- [ ] **Colors Correct**:
  - [ ] Body: Sandy tan (RGB ~210, 180, 140) with brown accents
  - [ ] Saddle: Vibrant colors (reds, golds, turquoise) with geometric patterns
  - [ ] Bridle: Red leather (RGB ~200, 50, 50)
  - [ ] Eyes: Glossy black with white highlights
- [ ] **No Pink/Magenta Materials**: All materials have proper base colors (no missing textures)

### UV Layout
- [ ] **Atlas Fit**: All UVs fit within 0-1 UV space (no islands outside bounds)
- [ ] **No Overlaps**: Each material occupies unique UV space (no texture tiling conflicts)
- [ ] **Density**: Texel density appears consistent across model
- [ ] **Seams**: Minimal visible seams on model surface (no ugly UV boundaries)

---

## FBX Export Verification

### Export Settings (File > Export > FBX)
- [ ] **Format**: FBX 2020 selected
- [ ] **Include Mesh**: ✓ Checked
- [ ] **Include Armature**: ✓ Checked
- [ ] **Include Shape Keys**: ✓ Checked
- [ ] **Animations**: OFF (no animations embedded yet)
- [ ] **File Path**: `Assets/Models/Camel/Camel_Default.fbx`
- [ ] **File Created**: Verify file exists and is 2-5 MB

### Export Output Check
- [ ] **Console Messages**: No red error messages in Blender console
- [ ] **File Size**: ~2-5 MB (reasonable for low-poly model with rig)
- [ ] **Naming**: File is exactly `Camel_Default.fbx`

---

## Unity Import Verification

### Import Settings (Assets/Models/Camel/Camel_Default.fbx)
- [ ] **Right-click FBX** > **Select**
- [ ] **Model Tab**:
  - [ ] Rig > Animation Type: All Animations (or Animation Type enabled)
  - [ ] Rig > Rig Type: Humanoid
  - [ ] Normals: Import Normals
  - [ ] Optimization: Enabled
- [ ] **Materials Tab**: Auto-generate checked (or custom materials applied)
- [ ] **Click Apply**

### Scene Test (Drag into scene)
- [ ] **Import Successful**: No error console spam (red messages)
- [ ] **Model Visible**: Camel_Default appears in viewport
- [ ] **Rig Present**: Inspector shows Armature with bones listed
- [ ] **Materials Loaded**: Model has colors (not all white/gray)

### Rig Deformation Test (Select Camel in Hierarchy)
- [ ] **Inspector > Animator**: Shows Camel_Armature
- [ ] **Rotate Test**: Select bone in Inspector, rotate it (e.g., Head +90° Z)
  - [ ] Head mesh follows bone rotation
  - [ ] No sharp creases or interpenetration
  - [ ] Deformation is smooth and natural
- [ ] **Full Range Test**: Rotate each major bone 90° (Neck, FrontLegs, BackLegs)
  - [ ] All deformations feel correct
  - [ ] No clipping or unwanted behavior

### Shape Keys Test (Material > Shape Keys)
- [ ] **Shape Keys Panel**: Shows Basis, Happy, Startled, Determined
- [ ] **Slider Test**: Drag Happy slider to 1.0
  - [ ] Face visibly changes (grin appears, eyes squint)
- [ ] **Slider Test**: Drag Startled slider to 1.0
  - [ ] Face changes again (eyes wide, eyebrows up)
- [ ] **Slider Test**: Drag Determined slider to 1.0
  - [ ] Face shows determined expression

### Tri Count Verification
- [ ] **Open Window > Stats**
- [ ] **Verify Tri Count**: Shows <1,200 triangles
- [ ] **Record Value**: Note exact tri count for handoff

### Visual Quality Check
- [ ] **Silhouette**: Camel shape is immediately recognizable
- [ ] **Accessories**: Goggles, saddle, bridle all clearly visible
- [ ] **Proportions**: Exaggerated (big head, short legs, long neck) matches concept art
- [ ] **Colors**: Match art direction (warm, sunny colors)
- [ ] **Expression**: Default happy expression reads naturally

---

## Sign-Off Checklist

Complete ALL of the above before proceeding to Phase 2.

### Final Verification
- [ ] **Tri Count Recorded**: _____ tris (must be <1,200)
- [ ] **No Blockers**: All checks passed, no outstanding issues
- [ ] **Reference Matches**: Model visually matches `Assets/Textures/ConceptArt/camel_camel_concept.png`
- [ ] **Timeline On Track**: Completed by May 21, 2026

### Hand-Off Ready
- [ ] Post completion summary to issue: "Phase 1 complete: Camel_Default.fbx ready for animation"
- [ ] Include: Final tri count, any notes, reference to this checklist
- [ ] Notify: Next phase can begin (animator picks up Phase 2, May 22)

---

## If You Get Stuck

**Problem**: Tri count too high  
**Solution**: Use Decimate modifier, reduce sculpt detail, or retopologize

**Problem**: Mesh deforms incorrectly  
**Solution**: Re-weight paint bones, ensure all vertices assigned to a bone

**Problem**: FBX won't import  
**Solution**: Check FBX 2020 export, verify "Include Armature" is checked, file location is correct

**Problem**: Shape keys don't appear in Unity  
**Solution**: Verify "Include Shape Keys" in FBX export, re-import

**Problem**: Materials are missing  
**Solution**: Right-click FBX > Re-import, or manually create materials in Unity matching design

See `PHASE_1_EXECUTION.md` "Troubleshooting" section for more help.

---

## Next Phase Trigger

Once ALL checkboxes are complete:
1. Comment on issue: "Phase 1 verification complete, Camel_Default.fbx ready"
2. Phase 2 (Animations) begins immediately (May 22)
3. Animator uses verified Camel_Default.fbx as base for all 7 animations

---

**Verification Date**: ____________  
**Verified By**: ____________  
**Status**: ☐ PENDING | ☐ PASSED | ☐ FAILED
