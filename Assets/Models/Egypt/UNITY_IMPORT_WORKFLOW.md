# Egypt Assets — Unity Import Workflow

## Overview

This document describes the process for importing generated Egypt environment assets into Unity, setting them up as prefabs, creating texture atlases, and verifying quality.

---

## Step 1: Import FBX Assets into Unity

### Folder Structure

```
Assets/Models/Egypt/
  ├── Road/
  │   ├── road_segment_01.fbx
  │   ├── road_segment_02.fbx
  │   └── ... (8+ files)
  ├── Props/
  │   ├── pyramid_near.fbx
  │   ├── obelisk_medium.fbx
  │   ├── sphinx.fbx
  │   ├── temple_gate.fbx
  │   └── ... (20+ props)
  ├── Environment/
  │   ├── dune_left.fbx
  │   ├── dune_right.fbx
  │   ├── oasis_main.fbx
  │   ├── rocks_set.fbx
  │   └── decorative_props.fbx
  ├── Textures/
  │   ├── egypt_atlas_road.png
  │   ├── egypt_atlas_props.png
  │   ├── egypt_atlas_decorative.png
  │   └── ... (other texture atlases)
  └── Prefabs/
      ├── Road/
      ├── Props/
      ├── Environment/
      └── Decorative/
```

### Import Settings

For each `.fbx` file:

1. **Select the asset in Project window**
2. **Inspector → Model tab:**
   - **Model:** `FBX Importer`
   - **Meshes:**
     - ✓ Import Mesh
     - ✓ Import BlendShapes (if present)
     - Vertex Colors: ✓ (if baked lighting)
   - **Geometry:**
     - ✓ Keep Quads
     - Welded Vertices: ✓
   - **Normals & Tangents:**
     - Normals: Import Normals
     - Tangents: Import Normals and Tangents
   - **Materials:**
     - ✓ Import Materials
     - Material Location: `Assets/Models/Egypt/Materials/`
   - **Animation:**
     - ✗ Import Animation (not needed for static props)
   - **Deformation:**
     - ✓ Skins
     - ✓ BlendShapes (if applicable)

3. **Click Apply**

### Quality Verification (per asset)

- [ ] Mesh imports correctly (no missing geometry)
- [ ] Normals look correct (smooth shading where expected)
- [ ] Poly count matches specification (check Mesh Inspector)
- [ ] Material is generated (check Materials folder)

---

## Step 2: Create Materials & Texture Atlas

### Atlas Layout Strategy

Create texture atlases to minimize draw calls and memory usage.

**Atlas 1: Road & Core Environment**
- Size: 1024×1024
- Contents:
  - Road segments (all 8+ variants)
  - Dune side-pieces (all 6 variants)
- UV Padding: 4 pixels

**Atlas 2: Props & Landmarks**
- Size: 1024×1024
- Contents:
  - Pyramids (all LOD variants)
  - Obelisks (all scale variants)
  - Sphinx
  - Temple Gate
- UV Padding: 4 pixels

**Atlas 3: Vegetation & Obstacles**
- Size: 512×512
- Contents:
  - Palm trees (all variants)
  - Cacti (all variants)
  - Rocks/Boulders (all variants)
- UV Padding: 4 pixels

**Atlas 4: Decorative & Specialty**
- Size: 512×512
- Contents:
  - Egyptian decorative props (8–10 items)
  - Oasis details
  - Special effects textures
- UV Padding: 4 pixels

**Total Budget:** 1024×1024×2 + 512×512×2 = ~4.25 MB (under 50 MB total)

### Create Materials

1. **Create Material Folders:**
   ```
   Assets/Models/Egypt/Materials/
     ├── Road/
     ├── Props/
     ├── Environment/
     └── Decorative/
   ```

2. **Create Master Materials** (one per atlas):
   - `EgyptRoad_Mat`
   - `EgyptProps_Mat`
   - `EgyptVegetation_Mat`
   - `EgyptDecorative_Mat`

3. **Material Settings:**
   - Shader: `Standard` or `Unlit` (depending on baked lighting)
   - Albedo: Link to corresponding atlas texture
   - Normal Map: If available
   - Smoothness: 0.3–0.5 (slightly rough for stone/sand)
   - Metallic: 0 (except gold accents: 0.8–1.0)

### Reassign Materials to Imported Assets

For each imported model:

1. **Open the FBX in Hierarchy**
2. **Select the mesh object**
3. **Inspector → Materials:**
   - Assign the correct master material (e.g., `EgyptProps_Mat` for pyramid)
   - Ensure UV coordinates are within atlas bounds

4. **Verify in Scene View:**
   - Texture displays correctly
   - No stretching or overlap
   - Colors match reference concept art

---

## Step 3: Create Prefabs

### Naming Convention

```
Assets/Models/Egypt/Prefabs/
  ├── Road/
  │   ├── Road_Segment_01.prefab
  │   ├── Road_Segment_02.prefab
  │   └── ... (8+ total)
  ├── Props/
  │   ├── Pyramid_Near.prefab
  │   ├── Pyramid_Mid.prefab
  │   ├── Obelisk_Small.prefab
  │   ├── Sphinx_Main.prefab
  │   ├── TempleGate_Main.prefab
  │   └── ... (20+ props)
  ├── Environment/
  │   ├── Dune_Left_Small.prefab
  │   ├── Dune_Right_Large.prefab
  │   ├── Oasis_Main.prefab
  │   ├── RockSet_Medium.prefab
  │   └── ... (10+ environment pieces)
  └── Decorative/
      ├── Urn_01.prefab
      ├── Scarab_Stone.prefab
      ├── Torch_Burning.prefab
      └── ... (8–10 decorative props)
```

### Creating a Prefab

1. **Drag FBX from Project into Hierarchy** (creates instance)
2. **Set up the instance:**
   - Position: (0, 0, 0)
   - Rotation: (0, 0, 0) for road; varies for props
   - Scale: (1, 1, 1)
3. **Add Components:**
   - **Collider:** 
     - Road segments: `CapsuleCollider` (lane boundaries) or `BoxCollider`
     - Obstacles (cacti, rocks): `BoxCollider` with "Is Trigger" ✓
     - Props (pyramids, obelisks): `BoxCollider` (optional, visual reference)
   - **Tags:** 
     - Road: `"Road"`
     - Obstacles: `"Obstacle"`
     - Landmarks: `"Landmark"`
     - Decorative: `"Decoration"`
4. **Drag instance from Hierarchy into Prefabs folder** (creates prefab)
5. **Delete instance from Hierarchy**

### Prefab Configuration

**Road Segments:**
- Collider: `BoxCollider` (2D for lane blocking)
- Is Trigger: ✗
- Pivot: Center of segment
- Scale: Matches 3-lane layout (each lane ~1.6 units wide)

**Obstacles (Cacti, Rocks):**
- Collider: `BoxCollider` or `SphereCollider`
- Is Trigger: ✓ (triggers collision, handled by code)
- Pivot: Center of object
- Layer: `"Obstacle"`

**Landmarks (Pyramids, Obelisks, Sphinx, Gate):**
- Collider: `BoxCollider` (optional, for visual reference)
- Is Trigger: ✗
- Pivot: Center or base depending on visual design
- Layer: `"Landmark"`

**Decorative Props:**
- Collider: ✗ (no collision)
- Pivot: Base of object
- Layer: `"Decoration"`

---

## Step 4: Verify Pivots & Scale

### Road Segments

**Correct Pivot Setup:**
- Origin (0,0,0) at **center of road** horizontally
- Y-axis: at surface level (road top)
- Z-axis: forward direction (running direction)

**Test in LevelGenerator:**
- Create a test scene with 3 road segments placed end-to-end
- Verify no gaps or overlaps
- Check that lanes align (center lane should be continuous)

### Props (Pyramids, Obelisks, Sphinx, etc.)

**Correct Pivot Setup:**
- Origin at **center base** of object
- Y-axis: ground level
- All props should rest on `y=0` plane

**Test in Scene:**
- Place prop on flat plane at `y=0`
- Verify it appears grounded (not floating or sinking)

### Environmental Pieces (Dunes, Oasis)

**Correct Pivot Setup:**
- Origin at **base level** for side-pieces
- Oasis: origin at water surface level
- Dunes: match road height for seamless placement

**Test:**
- Place beside road segments
- Verify alignment and no clipping

---

## Step 5: Texture Atlas & UV Verification

### UV Bounds Check

1. **Select imported model**
2. **Inspector → Preview:**
   - Display Mode: `UV Layout`
   - Verify all UV islands are within `0.0 → 1.0` bounds
   - Check padding (4–8 pixels between islands)

3. **In Texture:**
   - Open atlas texture in image editor
   - Verify each asset occupies distinct region
   - No overlapping UVs

### Atlas Baking (if needed)

If assets come with individual textures:

1. **Use Substance Alchemist or similar:**
   - Import individual textures
   - Arrange into single 1024×1024 atlas
   - Re-UV all models to point to atlas
   - Export atlas + UV-adjusted FBX

2. **Reimport FBX with new UVs**

### Memory Budget Tracking

After each material is set up:

```
Texture Memory = (width × height × 4 bytes per RGBA) / 1,048,576 MB

Example:
1024×1024 = (1024 × 1024 × 4) / 1,048,576 = ~4 MB per atlas
Total budget: < 50 MB
```

Verify:
- [ ] Road & Core atlas: ~4 MB
- [ ] Props & Landmarks: ~4 MB
- [ ] Vegetation & Obstacles: ~2 MB
- [ ] Decorative: ~2 MB
- **Total: ~12 MB** (well under 50 MB budget)

---

## Step 6: Quality Verification

### Checklist per Asset

- [ ] Mesh imports without errors
- [ ] Normals smooth correctly
- [ ] Poly count matches spec
- [ ] Material assigned and visible
- [ ] Texture displays correctly (no stretching)
- [ ] Pivot at correct location
- [ ] Scale (1,1,1) matches reference
- [ ] Colliders (if needed) are set up correctly
- [ ] Tag/Layer assigned
- [ ] Prefab created and functional

### Visual Quality Check

Compare each asset against:
1. **Concept art** (colors, style, proportions)
2. **Reference screenshot** (overall aesthetic matching)
3. **Adjacent assets** (style consistency)

Verify:
- [ ] Color palette matches (sand beige, warm yellow, gold, etc.)
- [ ] Style is stylized & cartoonish (not realistic)
- [ ] Lighting is warm (golden-hour feel)
- [ ] Details are visible but not over-detailed
- [ ] Asset fits 3-lane layout without clipping

---

## Step 7: Integration Testing

### Test in Gameplay Scene

1. **Open `Assets/Scenes/GameplayScene.unity`**
2. **In Hierarchy:** Check that `LevelGenerator` script references are valid
3. **Add test prefabs to prefab list:**
   - `LevelGenerator.cs` has a list of road segment prefabs
   - Add all 8+ road segment prefabs to this list
   - Add decorative props to appropriate lists
4. **Play scene and verify:**
   - [ ] Road segments spawn and connect properly
   - [ ] No gaps or overlaps
   - [ ] Props place correctly
   - [ ] Camera framing is correct
   - [ ] Colliders work (test movement)

### Performance Check

- [ ] FPS stable at target (30+ on mobile device target)
- [ ] No memory spikes when loading Egypt level
- [ ] Textures load smoothly (no streaming artifacts)

---

## Step 8: Final Verification Against Acceptance Criteria

- [ ] Min 8 unique road segment prefabs created ✓
- [ ] All listed environment props created and imported ✓
- [ ] Assets are low-poly and texture-atlased ✓
- [ ] Prefabs have correct pivots and scale for 3-lane layout ✓
- [ ] Visual quality matches or exceeds reference screenshot ✓
- [ ] Total texture memory budget under 50MB for Egypt set ✓

---

## Troubleshooting

### Issue: Mesh doesn't import
- **Solution:** Check FBX is valid (open in Blender to verify)
- **Solution:** Try re-exporting with different settings

### Issue: Material is missing/black
- **Solution:** Check material assignment in Inspector
- **Solution:** Verify texture atlas path is correct
- **Solution:** Re-assign texture to material Albedo slot

### Issue: Model is too dark/light
- **Solution:** Adjust material Smoothness or Metallic values
- **Solution:** Check if vertex colors are baked lighting (disable if not intended)

### Issue: Prefab in scene looks different than expected
- **Solution:** Check scale and rotation (should be 1,1,1 and 0,0,0)
- **Solution:** Verify collider isn't scaled differently than mesh
- **Solution:** Check layer/tag isn't filtering visibility

---

## Next Steps

1. **Complete Phase 1 imports** (Road, Dunes, Cacti, Palms)
2. **Test in LevelGenerator** (verify spawning)
3. **Complete Phase 2 imports** (Pyramids, Obelisks, Sphinx, Temple Gate)
4. **Integrate into level generation system**
5. **Final quality pass and acceptance criteria verification**

---

**Status:** Import workflow documented, ready for Phase 1 imports  
**Timeline:** Week 1–1.5 (Phase 1), Week 1.5–3 (Phases 2–3), Week 3–3.5 (Integration & verification)
