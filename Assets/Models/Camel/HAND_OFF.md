# Phase 1 Hand-Off: Ready for Blender Execution

**Date**: 2026-05-07  
**Issue**: AIG-9 — M3: Characters & Animations  
**Character**: Camel (default model)  
**Status**: ✅ READY FOR BLENDER ARTIST

---

## What You're Taking Over

**Phase 1 Scope**: Create a production-ready rigged low-poly Camel model  
**Timeline**: May 15-21, 2026 (1 week)  
**Owner**: Blender Artist/Operator  
**Deliverable**: `Assets/Models/Camel/Camel_Default.fbx`

---

## Files You Have

All in `Assets/Models/Camel/`:

| File | Purpose | Size |
|------|---------|------|
| **START_HERE.md** | 5-minute quick start | 145 lines |
| **generate_camel_base_mesh.py** | Blender Python script (creates mesh) | 191 lines |
| **setup_camel_rig.py** | Blender Python script (creates rig) | 208 lines |
| **PHASE_1_EXECUTION.md** | Full step-by-step guide (8 steps) | 247 lines |

**Total**: 791 lines of executable code + guides

---

## What's Expected

### Success = Meeting These Criteria

Before Phase 2 (animations, May 22), your work must satisfy ALL:

- [ ] **Camel_Default.fbx** created and saved to `Assets/Models/Camel/`
- [ ] **Silhouette**: Clearly reads as a camel (no ambiguity)
- [ ] **Accessories Visible**:
  - Aviator goggles on forehead
  - Colorful Moroccan saddle blanket with tassels
  - Red bridle around snout
- [ ] **Expressions Work**:
  - Happy (default grin, eyes slightly squinted)
  - Startled (wide eyes, raised eyebrows)
  - Determined (narrow eyes, clenched look)
- [ ] **Polygon Count**: Verified <1,200 triangles
- [ ] **Rig Quality**:
  - All 13 bones deform mesh smoothly
  - No sharp creases or unwanted deformations
  - All bone rotations feel natural
- [ ] **UV Layout**: Efficient 1024x1024 atlas, no overlaps
- [ ] **Materials Render**:
  - Sandy tan body with brown accents
  - Red bridle visible
  - Colorful saddle with patterns
  - Glossy eyes with highlights
- [ ] **Unity Import**:
  - FBX imports cleanly (no error console spam)
  - Rig is active and testable
  - Blendshapes appear in Shape Keys inspector
  - No missing materials

---

## How to Execute

**Estimated Time**: 4-5 days (May 17-21)

1. **Setup** (30 min)
   - Open Blender
   - Go to Scripting workspace
   - Ensure Python 3.9+

2. **Generate Mesh** (5 min)
   - Open `generate_camel_base_mesh.py`
   - Run (▶ button or Alt+P)
   - Output: `Camel_Default` mesh in viewport
   - Should see: ~120 verts, ~200 faces (~400 tris)

3. **Setup Rig** (5 min)
   - Keep `Camel_Default` selected
   - Open `setup_camel_rig.py`
   - Run
   - Output: `Camel_Armature` with 13 bones + materials + vertex groups

4. **Sculpt & Refine** (2-3 days)
   - Enter Sculpt Mode
   - Match concept art (see reference below)
   - Add details: goggles, saddle folds, bridle curves
   - Stay under 1,200 tris (monitor in Wireframe)
   - See Step 3 in PHASE_1_EXECUTION.md for detailed sculpting tips

5. **Weight Paint** (1-2 days)
   - Enter Weight Paint Mode
   - Paint influence for each of 13 bones
   - Test deformation: rotate bones in Pose Mode
   - Neck should curve, legs should bend at joints
   - See Step 5 in PHASE_1_EXECUTION.md for detailed guide

6. **Export** (15 min)
   - File > Export > FBX
   - Settings: FBX 2020, Include Mesh/Armature/Shape Keys
   - Save as: `Camel_Default.fbx`
   - See Step 6 in PHASE_1_EXECUTION.md for exact settings

7. **Verify in Unity** (30 min)
   - Import FBX into Unity
   - Drag into scene
   - Test rig: Select bones, rotate in Inspector
   - Verify tri count: Window > Stats (<1,200)
   - Check for clipping
   - See Step 7 in PHASE_1_EXECUTION.md for checklist

---

## Reference Materials

**Concept Art**: `Assets/Textures/ConceptArt/camel_camel_concept.png`  
**Design Spec**: `docs/M3_Production/M3_PRODUCTION_DESIGN.md` (Section 2)  
**Color Palette**: 
- Sandy tan body: RGB (210, 180, 140)
- Brown accents: RGB (139, 90, 43)
- Red bridle: RGB (200, 50, 50)
- Saddle colors: Vibrant reds, golds, turquoise

**Style Reference**: Crossy Road / Subway Surfers
- Low-poly, chunky silhouettes
- Exaggerated proportions (big head, squat legs, long neck)
- Strong colors, good contrast
- Readable character immediately

---

## Key Metrics

| Metric | Target | Verify |
|--------|--------|--------|
| Tri Count | <1,200 | Window > Stats |
| Bone Count | 13 | Outliner |
| Materials | 4 (Body, Saddle, Bridle, Eyes) | Shading Editor |
| Expressions | 3 (Happy, Startled, Determined) | Shape Keys panel |
| UV Atlas | 1024x1024, no overlaps | UV Editor |
| File Size | ~2-5 MB | File properties |

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Mesh deforms incorrectly | Re-weight paint, ensure ALL verts assigned to bones |
| Tri count too high | Use Decimate modifier, reduce sculpt detail |
| Blendshapes don't work | Ensure created AFTER Basis shape key |
| FBX import fails | Check "Include Armature" in export, verify file path |
| Missing materials | Auto-generate on import or create manually in Unity |
| Clipping during rotation | Adjust weight values, retest in Pose Mode |

See PHASE_1_EXECUTION.md "Troubleshooting" section for more.

---

## What Happens Next

**On Completion** (Est. May 21):
- Report back: "Phase 1 complete, Camel_Default.fbx ready"
- Share tri count, any notes
- Phase 2 (animations) begins May 22

**If Blocked**:
- Check PHASE_1_EXECUTION.md Step 8 (blockers section)
- Reference M3_PRODUCTION_DESIGN.md for design decisions
- Contact if design assumptions need clarification

---

## Key Commitments

✅ **All scripts are tested and functional**  
✅ **Documentation is complete and step-by-step**  
✅ **No dependencies on other work**  
✅ **Clear acceptance criteria provided**  
✅ **Timeline is realistic (4-5 days)**  

You have everything needed. Go create!

---

**Hand-Off Date**: 2026-05-07  
**Phase 1 Deadline**: 2026-05-21  
**Next Phase**: Phase 2 Animations (May 22-25)  
**Full Milestone**: 2026-06-11 (4-week M3 timeline)

🎯 Ready to execute.
