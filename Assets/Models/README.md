# M3 Character Assets — Production Folder

**Status**: Phase 1 In Progress (2026-05-15 → 2026-05-21)  
**Issue**: AIG-9  
**Timeline**: 4 weeks (2026-05-15 → 2026-06-11)

---

## Folder Structure

```
Assets/Models/
├── Kamil/                    # Kamil the Camel (main character)
│   ├── Kamil_Default.fbx    # Base model (IN PROGRESS)
│   ├── Kamil_Pharaoh.fbx    # Skin (pending)
│   ├── Kamil_Racing.fbx     # Skin (pending)
│   ├── Kamil_Mummy.fbx      # Skin (pending)
│   └── Kamil_Golden.fbx     # Skin (pending)
└── Thieves/                  # Antagonist characters
    ├── Thief_Bandit.fbx     # Desert Bandit (pending)
    ├── Thief_Ninja.fbx      # Ninja Thief (pending)
    ├── Thief_Pirate.fbx     # Pirate (pending)
    └── Thief_Shadow.fbx     # Shadow Thief (pending)
```

---

## Phase 1: Kamil Default (In Progress)

### Status
- [x] Design specification complete (M3_PRODUCTION_DESIGN.md)
- [x] Generation prompt prepared (Kamil_Default_Generation_Prompt.md)
- [ ] AI generation (Leonardo.AI / Meshy AI)
- [ ] Manual cleanup & retopology
- [ ] Rigging & weight painting
- [ ] Blendshapes (happy, startled, determined)
- [ ] Material setup & texturing
- [ ] UV layout (1024x1024 atlas)
- [ ] FBX export & Unity import verification

### What's Next
1. **Generate mesh** using prompt from `docs/M3_Production/Kamil_Default_Generation_Prompt.md`
   - Tools: Leonardo.AI, Meshy AI, or Blender procedural modeling
   - Target: Recognize camel silhouette, goggles, saddle, bridle visible
   
2. **Cleanup in Blender**
   - Retopology: Clean quad topology for deformation
   - Target tri count: <1,200 tris
   - Remove unnecessary details while maintaining silhouette
   
3. **Rigging**
   - Create skeleton: ~15-20 bones (head, neck, body, 4 legs, tail, optional facial)
   - Weight painting for smooth deformation
   - Add blendshapes for expressions (happy, startled, determined)
   
4. **Material Setup**
   - Body: Sandy tan with brown accents
   - Saddle: Colorful Moroccan geometric patterns
   - Bridle: Red leather
   - Eyes: Glossy black with white highlights
   
5. **Export**
   - Format: FBX 2020
   - Include: Mesh, skeleton, blendshapes
   - No animations at this stage
   - Verify import in Unity

### Reference Materials
- Concept art: `Assets/Textures/ConceptArt/kamil_camel_concept.png`
- Design doc: `docs/M3_Production/M3_PRODUCTION_DESIGN.md` (Section 2: Kamil Default Model Spec)
- Generation prompt: `docs/M3_Production/Kamil_Default_Generation_Prompt.md`
- Style reference: Crossy Road / Subway Surfers (low-poly, exaggerated proportions, vibrant colors)

---

## Acceptance Criteria

### Kamil Default Acceptance
- [ ] Model matches description (goggles on forehead, Moroccan saddle, red bridle, happy expression)
- [ ] Polygon count verified <1,200 tris
- [ ] Geometry is animation-ready (clean topology, proper deformation)
- [ ] All 3-4 materials render correctly (body, saddle, bridle, eyes/face)
- [ ] Rig deforms smoothly through full range of motion
- [ ] FBX imports cleanly into Unity without errors
- [ ] Visual quality matches or exceeds concept art

### All Skins Acceptance (After Phase 1 → Phase 3)
- [ ] All 4 skins created and visually distinct
- [ ] Each skin uses same Kamil skeleton (no rig variations)
- [ ] All skins stay under 2,000 tris with accessories
- [ ] No clipping during any animation

### All Thieves Acceptance (After Phase 1 → Phase 3)
- [ ] All 4 thief models created (Bandit, Ninja, Pirate, Shadow)
- [ ] Each clearly reads as intended character type
- [ ] Each under 1,500 tris
- [ ] Unique from each other visually

---

## Technical Specifications

### Performance Budget
- **Kamil Base**: <1,200 tris
- **Kamil + Accessories (Skins)**: <2,000 tris
- **Thieves**: <1,500 tris each

### Export Format
- **File Type**: FBX 2020
- **Include**: Mesh geometry, skeleton, materials (references)
- **Exclude**: IK handles, constraints, external anim references
- **Naming**: `Character_Variant.fbx` (e.g., `Kamil_Pharaoh.fbx`)

### Unity Import Settings
```
FBX Import:
  - Rig Type: Humanoid (Kamil), Generic (Thieves)
  - Animation Type: Enabled (animations embedded in FBX)
  - Materials: Auto-generate or custom
  - Normals: Import Normals
  - Optimization: Enabled (read/write disabled if possible)
```

---

## Timeline Milestones

| Date | Phase | Deliverable | Status |
|------|-------|-------------|--------|
| **May 15-21** | Phase 1 | Kamil Default + Rig | 🟡 In Progress |
| **May 22-25** | Phase 2 | All 7 Animations | ⬜ Pending |
| **May 26-Jun 4** | Phase 3 | 4 Skins + 4 Thieves | ⬜ Pending |
| **Jun 5-11** | Phase 4 | QA & Final Export | ⬜ Pending |

---

## Git Commit History

- `ad0bf99` (2026-05-07): Add M3 production design & Kamil generation prompt

---

## Questions or Blockers?

If you encounter issues:
1. Check the full M3_PRODUCTION_DESIGN.md for detailed specifications
2. Review concept art and style guide references
3. Refer to GDD.md for overall art direction and color palette

---

**Last Updated**: 2026-05-07  
**Next Update**: After Kamil Default model completion (Est. 2026-05-21)
