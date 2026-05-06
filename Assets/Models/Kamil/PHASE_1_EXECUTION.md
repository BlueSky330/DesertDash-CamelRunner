# Phase 1 Execution: Kamil Default Model + Rig

**Timeline**: May 15-21, 2026  
**Target**: Production-ready rigged low-poly Kamil model (<1200 tris)  
**Status**: 🟡 In Progress - Ready for Blender Production

---

## Execution Steps (Blender Workflow)

### Step 1: Generate Base Geometry
Run `generate_kamil_base_mesh.py` in Blender to create low-poly camel mesh.

**How**:
```
Blender > Scripting workspace
1. Open "generate_kamil_base_mesh.py"
2. Run script (▶ button or Alt+P)
3. Output: "Kamil_Default" mesh object in scene
```

**What it creates**:
- Low-poly camel body (body, neck, head, snout, legs, tail)
- Big, exaggerated proportions (Crossy Road style)
- Eye sockets and saddle base (ready for details)
- Sandy tan material applied
- Approximately 100-150 vertices, <300 faces

**Expected output**:
```
✓ Kamil base mesh created!
  - Vertices: ~120
  - Faces: ~200
  - Approximate tris: ~400
```

---

### Step 2: Setup Rigging Structure
Run `setup_kamil_rig.py` to create skeleton and weight paint groups.

**How**:
```
Blender > Scripting workspace (with Kamil_Default mesh selected)
1. Open "setup_kamil_rig.py"
2. Run script
3. Output: "Kamil_Armature" with bone structure
```

**What it creates**:
- 13-bone skeleton (Root, Hips, Spine, Chest, Neck, Head, Eyes, 4 Legs, Tail)
- Vertex groups for weight painting
- Blendshapes for expressions (Happy, Startled, Determined)
- 4 materials (Body, Saddle, Bridle, Eyes)
- Armature modifier linked to mesh

**Next action**: Weight paint each vertex group to control deformation.

---

### Step 3: Manual Refinement (Sculpting)
Enter Sculpt Mode to refine details and match concept art.

**In Sculpt Mode**:
1. **Face**: Add expression to happy grin, shape eyes
2. **Accessories**: Sculpt goggles indents, saddle shape, bridle curves
3. **Proportions**: Adjust leg thickness, neck curve
4. **Details**: Add subtle fur texture (if needed)

**Tri-count check**: Keep under 1,200 tris total
- Use Topology > Display Wireframe to monitor density
- Remesh tool if needed to reduce geometry

---

### Step 4: UV Layout & Texturing
Create efficient UV map for 1024x1024 atlas.

**Blender UV Editor**:
1. Enter Edit Mode
2. Mark seams (U > Mark Seams) for clean UV islands
3. Unwrap (U > Unwrap)
4. Optimize layout (Pack Islands)
5. Target: All UVs fit in 1024x1024, no overlaps, good texel density

**Materials**:
- Body: Sandy tan base + brown accents
- Saddle: Colorful geometric patterns (reds, golds, turquoise)
- Bridle: Red leather
- Eyes: Glossy black with white highlights

---

### Step 5: Weight Painting
Define how bones deform the mesh.

**Per-bone weight painting**:
1. Enter Weight Paint Mode
2. For each bone (Head, Neck, FrontLegs, BackLegs, Tail):
   - Select bone in armature
   - Paint red on geometry that follows that bone (brush > draw)
   - Blue on geometry that doesn't move
3. Test deformation:
   - Rotate bones in Pose Mode
   - Mesh should follow smoothly

**Key areas to watch**:
- Neck deformation (should curve naturally)
- Leg joints (bend at knee, not throughout)
- Tail movement (independent swing)
- Head-neck transition (smooth blend)

---

### Step 6: Create Blendshapes
Setup expression shapes for Happy, Startled, Determined.

**In Shape Key Editor**:
1. Select Kamil_Default mesh
2. Add new shape keys: "Happy", "Startled", "Determined"
3. For each:
   - Enter Edit Mode with shape key selected
   - Move/scale verts to match expression
   - Test with slider in properties

**Expressions to capture**:
- **Happy**: Wide open mouth, eyes slightly squinted, cheeks up
- **Startled**: Eyes wide open, eyebrows raised, mouth closed
- **Determined**: Eyebrows down, eyes narrow, teeth clenched look

---

### Step 7: Export FBX
Export as production-ready FBX with rig and blendshapes.

**Export Settings** (File > Export > FBX):
```
- Format: FBX 2020
- Selection: OFF (export all)
- Include:
  ✓ Mesh
  ✓ Armature
  ✓ Shape Keys (for blendshapes)
  OFF Animations (none yet)
- File path: Assets/Models/Kamil/Kamil_Default.fbx
```

**Verify export**:
- Rig is included (no separate armature)
- Blendshapes embedded
- <1200 tri tri-count
- Clean node hierarchy (no empty groups)

---

### Step 8: Unity Import & Verification
Import FBX into Unity and verify it works.

**Unity Import Settings** (Assets/Models/Kamil/Kamil_Default.fbx):
```
Model Tab:
- Rig: Humanoid
- Animation Type: All Animations (embedde)
- Normals: Import Normals
- Optimization: ON

Materials Tab:
- Location: Embedded Materials
```

**Test in scene**:
1. Drag Kamil_Default prefab into scene
2. Rotate armature bones in inspector (Pose Mode)
3. Test blendshapes in inspector
4. Verify no clipping, smooth deformation
5. Check tri count: Window > Stats

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Mesh deforms incorrectly | Re-do weight painting, ensure all verts assigned to bones |
| Tri count too high | Use Decimate modifier, reduce detail in sculpt |
| UV seams visible | Use normal maps from high-poly or improve seam placement |
| Blendshapes don't work | Ensure shape keys created AFTER Basis, test slider values |
| FBX import loses rig | Check "Include Armature" in export, verify Humanoid rig type in Unity |

---

## Acceptance Checklist

Before moving to Phase 2 (Animations), verify:

- [ ] Model silhouette is clear camel (recognizable immediately)
- [ ] Accessories visible (goggles, saddle blanket, bridle)
- [ ] Happy expression reads naturally (grin, eyes)
- [ ] Tri count verified <1,200
- [ ] Rig deforms smoothly (test all bones in Pose mode)
- [ ] Blendshapes work (happy, startled, determined all visible)
- [ ] UV layout efficient (1024x1024, no overlaps)
- [ ] Materials render correctly (colors match palette)
- [ ] FBX imports cleanly into Unity
- [ ] No red material warnings in Unity console

---

## Files in This Phase

```
Assets/Models/Kamil/
├── generate_kamil_base_mesh.py        ← Run this FIRST
├── setup_kamil_rig.py                 ← Run this SECOND
├── PHASE_1_EXECUTION.md               ← This guide
├── Kamil_Default.fbx                  ← Final output
└── Kamil_Default.blender              ← (Optional) Save .blend file
```

---

## Timeline

- **Start**: May 15, 2026
- **Milestone 1 (May 17)**: Base mesh generated, sculpting started
- **Milestone 2 (May 19)**: Weight painting complete, rigged tested
- **Milestone 3 (May 21)**: FBX exported, Unity verified
- **End**: May 21, 2026

---

## Next Phase (May 22-25)

Once Kamil_Default.fbx is production-ready:
1. Create all 7 animations (run, jump, slide, etc.)
2. Test each on Kamil rig
3. Ensure smooth blending between animations
4. Proceed to Phase 3 (Skins)

See M3_PRODUCTION_DESIGN.md Section 3 for animation specs.

---

**Status**: 🟡 Ready for execution  
**Owner**: Artist (Blender operator)  
**Blockers**: None  
**Last Updated**: 2026-05-07
