# Phase 1 Execution: START HERE

**Character**: Camel (renamed from Kamil per commit 74435d7)  
**Timeline**: May 15-21, 2026  
**Status**: 🟢 Ready for Blender Execution  
**Owner**: Next available Blender artist/operator

---

## Quick Start (5 Minutes to Running Code)

1. **Open Blender**
   - Ensure Python 3.9+ (Blender 3.0+)
   - Open Scripting workspace

2. **Run mesh generation** (5 min)
   ```bash
   # In Blender Python console:
   exec(open('Assets/Models/Camel/generate_camel_base_mesh.py').read())
   
   # Or just open file and press ▶
   ```
   - Output: `Camel_Default` mesh in viewport
   - Expect: ~120 vertices, ~400 tris

3. **Run rigging** (5 min)
   ```bash
   # In Blender Python console (with Camel_Default selected):
   exec(open('Assets/Models/Camel/setup_camel_rig.py').read())
   
   # Or just open file and press ▶
   ```
   - Output: `Camel_Armature` with 13 bones
   - Expect: Vertex groups, materials, armature modifier ready

4. **Proceed to sculpting & refinement**
   - See `PHASE_1_EXECUTION.md` for detailed steps (Step 3+)

---

## Files in This Directory

| File | Purpose | Lines |
|------|---------|-------|
| **generate_camel_base_mesh.py** | Creates low-poly mesh | 200 |
| **setup_camel_rig.py** | Creates skeleton & rig | 220 |
| **PHASE_1_EXECUTION.md** | Full step-by-step guide | 200+ |
| **START_HERE.md** | This quick reference | |

---

## What These Scripts Do

### generate_camel_base_mesh.py
- Creates procedural low-poly camel geometry
- Includes: body, head, neck, snout, 4 legs, tail, eyes
- Exaggerated proportions (Crossy Road style)
- Material: Sandy tan applied
- **Output**: Camel_Default object, ready for sculpting

### setup_camel_rig.py
- Creates 13-bone skeleton
- Adds vertex groups (one per bone)
- Creates shape keys for expressions (Happy, Startled, Determined)
- Sets up 4 materials (Body, Saddle, Bridle, Eyes)
- Applies armature modifier
- **Output**: Camel_Armature, rigged and ready for weight painting

---

## Next Steps After Running Scripts

See `PHASE_1_EXECUTION.md` Step 3 onwards for:

1. **Sculpt Mode** (2-3 days)
   - Refine geometry to match concept art
   - Add details (goggles, saddle, bridle)
   - Stay under 1,200 tris

2. **Weight Painting** (1-2 days)
   - Paint bone influence on mesh
   - Test deformation in Pose Mode

3. **Export FBX** (15 min)
   - File > Export > FBX 2020
   - Include: Mesh, Armature, Shape Keys
   - Save to: `Camel_Default.fbx`

4. **Verify in Unity** (30 min)
   - Import FBX into Unity
   - Test rig deformation
   - Verify tri count <1,200

---

## Acceptance Criteria

Before moving to Phase 2 (Animations):

- [ ] Camel model visually recognizable
- [ ] Accessories clearly visible (goggles, saddle, bridle)
- [ ] Tri count verified <1,200
- [ ] All bones deform smoothly
- [ ] 3 expressions work (happy, startled, determined)
- [ ] FBX imports cleanly in Unity
- [ ] No red material warnings

See `PHASE_1_EXECUTION.md` for full checklist.

---

## Blockers?

**None.** All scripts are ready to run.

**Questions?**
- Full details: See `PHASE_1_EXECUTION.md`
- Design spec: See `docs/M3_Production/M3_PRODUCTION_DESIGN.md`
- Concept art: See `Assets/Textures/ConceptArt/camel_concept.png`

---

## Timeline

```
Week 1 (May 15-21): Phase 1 [🟢 READY]
  May 15-17: Generate + Sculpt
  May 17-19: Weight paint
  May 19-21: Export + Verify in Unity

Week 2 (May 22-25): Phase 2 [⬜ Pending Phase 1]
  Create 7 animations on rigged Camel

Week 3 (May 26-Jun4): Phase 3 [⬜ Pending Phase 2]
  Create 4 skins + 4 thieves

Week 4 (Jun 5-11): Phase 4 [⬜ Pending Phase 3]
  QA + final exports
```

---

**Created**: 2026-05-07  
**Updated for Camel rename**: 2026-05-07 (commit 74435d7)  
**Status**: 🟢 Ready to Execute — No Blockers
