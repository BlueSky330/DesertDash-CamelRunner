# M2 Egypt Level Assets — Production Pipeline

**Timeline:** 2026-05-18 → 2026-06-11 (3.5 weeks)  
**Status:** In Production  
**Agent:** Artist2  
**Last Updated:** 2026-05-07

## Overview

Production pipeline for creating low-poly 3D game assets for the Egypt level in Desert Dash: Camel Runner. Assets are generated using Leonardo.Ai / Meshy AI, then imported and processed in Unity as texture-atlased prefabs.

---

## Art Specifications

| Property | Value |
|----------|-------|
| Style | Low-poly 3D, stylized geometric, colorful cartoonish |
| Color Palette | Sand beige, warm yellow, gold, terracotta, sunlit orange, palm green, oasis blue |
| Polygon Budget | TBD per class (mobile-optimized) |
| Texture Format | PNG, UV-mapped, texture-atlased (512×512 or 1024×1024) |
| Lighting | Warm golden-hour, bright midday sun, soft shadows |
| Engine | Unity 2022.3+ with correct pivots/scales for 3-lane layout |
| Total Budget | <50MB for entire Egypt asset set |

---

## Asset Classes & Generation Prompts

### 1. Road Modules (8+ unique segments)

**Purpose:** Procedurally repeating road segments that form the 3-lane endless runner path.  
**Target Poly Count:** 500–1,500 per segment  
**Quantity:** 8+ unique variants

#### Segment Variations:
- Straight road (baseline)
- Gentle left curve
- Gentle right curve
- Banked left turn
- Banked right turn
- Incline up
- Incline down
- Decorated section (temple markers, obelisks on edges)

#### Generation Prompt Template:
```
Generate a low-poly 3D game asset: 3-lane desert sand road segment 
for an endless runner game. Style: stylized, cartoonish, colorful. 
Dimensions: approximately 10 units long, 5 units wide, 0.5-1 unit tall. 
Features: sand-textured lanes, visible lane markings (subtle gold/tan lines), 
natural sand dune edges on left/right sides, golden-hour lighting with warm shadows. 
Color palette: sand beige (#D4A574), warm yellow (#FFC93C), gold accents. 
Poly count: under 1,500 triangles. Export as .obj or .fbx with clean topology. 
[VARIATION: Straight | Left Curve | Right Curve | Incline Up | Incline Down | Banked Left | Banked Right | Decorated Temple Section]
```

---

### 2. Sand Dune Side-Pieces

**Purpose:** Environmental framing and lane-edge definition.  
**Target Poly Count:** 300–800 per piece  
**Quantity:** 6 variants (left/right/background × standard/large)

#### Variations:
- Left dune (small)
- Left dune (large)
- Right dune (small)
- Right dune (large)
- Background dune (far distance, low-poly)
- Dune with rocky outcrop

#### Generation Prompt Template:
```
Generate a low-poly 3D sand dune side-piece for a 3-lane endless runner game. 
Style: stylized, cartoonish, low-poly. Purpose: lanes edge definition and 
environmental framing. Dimensions: 3–5 units wide, 2–6 units tall, 1–2 units deep. 
Features: natural sand dune slopes, subtle shadows showing dune contours, 
golden-hour lighting. Color palette: sand beige (#D4A574), warm yellow (#FFC93C), 
tan (#C2A878). Include [LEFT | RIGHT | BACKGROUND] variant. 
Poly count: under 800 triangles. Export as .obj or .fbx.
```

---

### 3. Oasis/Water Module

**Purpose:** Collectible health-restore zone with visual appeal.  
**Target Poly Count:** 2,000–3,000  
**Quantity:** 1 primary asset + 1-2 visual variants

#### Features:
- Water body (turquoise/cyan)
- Palm trees (2-3 per oasis)
- Rocks/boulders around edges
- Grass/reeds
- Subtle water animation supports

#### Generation Prompt Template:
```
Generate a low-poly 3D oasis environment module for an endless runner game. 
Style: stylized, cartoonish, colorful. Dimensions: 8–12 units wide, 6–10 units tall, 
4–6 units deep. Features: turquoise water body with ripple texture, 2–3 stylized 
palm trees, surrounding rocks/boulders, grass/reed details, golden-hour lighting 
with warm sun. Color palette: oasis blue (#0088CC), palm green (#66BB6A), 
sand beige (#D4A574), warm yellow (#FFC93C). Poly count: under 3,000 triangles. 
Export as .obj or .fbx with modular pieces (water + palms + rocks separate).
```

---

### 4. Temple Gate

**Purpose:** Decorative landmark with hieroglyph-inspired details.  
**Target Poly Count:** 2,500–3,500  
**Quantity:** 1 main asset + 1-2 scaling variants

#### Features:
- Symmetrical gate structure (2 pillars + overhead beam)
- Hieroglyph reliefs on surfaces
- Decorative carved details
- Warm stone texture (sandstone)

#### Generation Prompt Template:
```
Generate a low-poly 3D ancient Egyptian temple gate for a game background. 
Style: stylized, cartoonish, colorful. Dimensions: 6–8 units wide, 8–12 units tall, 
2–3 units deep. Features: two tall pillars with hieroglyph-inspired carvings, 
overhead beam/lintel connecting pillars, decorative relief details (ankhs, scarabs, 
lotus flowers), warm sandstone texture, golden-hour lighting. Color palette: 
terracotta (#E2725B), warm sand (#D4A574), gold accents (#FFD700). 
Poly count: under 3,500 triangles. Export as .obj or .fbx. Include 2–3 scale 
variants for background layering.
```

---

### 5. Obelisk Set

**Purpose:** Iconic Egyptian landmarks for mid-ground visual interest.  
**Target Poly Count:** 800–1,500 per variant  
**Quantity:** 3 scale variants (small, medium, large)

#### Variants:
- Small obelisk (foreground/collectible zone)
- Medium obelisk (mid-ground)
- Large obelisk (background)

#### Features:
- Tapered square pillar
- Pointed pyramidal top
- Hieroglyph details
- Warm stone color

#### Generation Prompt Template:
```
Generate a low-poly 3D Egyptian obelisk monument. Style: stylized, cartoonish, 
colorful. Features: tapered square pillar, pointed pyramidal top, 
hieroglyph-carved details on all faces, warm sandstone texture, 
golden-hour lighting with soft shadows. Color palette: terracotta (#E2725B), 
warm sand (#D4A574), gold highlights (#FFD700). 
Create [SMALL: 2-3 units tall | MEDIUM: 5-7 units tall | LARGE: 10-15 units tall]. 
Poly count: under 1,500 triangles per variant. Export as .obj or .fbx.
```

---

### 6. Pyramid Background Set

**Purpose:** Distance landscape elements (near, mid, far LOD).  
**Target Poly Count:** 500–5,000 depending on LOD  
**Quantity:** 3 LOD variants + 2-3 size variations

#### LOD Breakdown:
- **Near (playfield):** 3,000–5,000 polys, high detail
- **Mid (mid-ground):** 1,500–2,500 polys, medium detail
- **Far (background):** 300–800 polys, silhouette only

#### Variants:
- Small pyramid (foreground)
- Large pyramid (mid-ground)
- Pyramid cluster (3+ distant pyramids as single asset)

#### Generation Prompt Template:
```
Generate a low-poly 3D Egyptian pyramid for game background. 
Style: stylized, cartoonish, colorful, painterly. 
Features: geometric pyramidal form, subtle sand-colored surface variations, 
golden-hour warm lighting, soft shadows. Color palette: sand beige (#D4A574), 
warm yellow (#FFC93C), gold accents (#FFD700), shadow tones (#A68A64). 
Create [NEAR/HIGH-LOD: detailed stone blocks | MID/MEDIUM-LOD: simplified facets | FAR/LOW-LOD: silhouette]. 
Poly count: [NEAR: 5,000 | MID: 2,000 | FAR: 500]. Export as .obj or .fbx. 
Include small, large, and cluster variations.
```

---

### 7. Sphinx Prop

**Purpose:** Iconic mid-ground landmark and visual anchor.  
**Target Poly Count:** 4,000–6,000  
**Quantity:** 1 main asset

#### Features:
- Human head with Egyptian headdress
- Leonine body (reclining pose)
- Hieroglyph details
- Warm stone color
- Detailed facial features

#### Generation Prompt Template:
```
Generate a low-poly 3D Egyptian Sphinx monument for a game. 
Style: stylized, cartoonish, colorful. Dimensions: 10–15 units long, 6–8 units tall, 
3–4 units wide (reclining pose). Features: human face with Egyptian headdress (nemes), 
leonine body, hieroglyph reliefs on chest/base, warm sandstone texture, golden-hour 
lighting. Color palette: terracotta (#E2725B), warm sand (#D4A574), gold (#FFD700), 
shadow tones. Poly count: under 6,000 triangles. Export as .obj or .fbx with 
clean topology.
```

---

### 8. Palm Tree Variations

**Purpose:** Vegetation detail, collectible visual interest.  
**Target Poly Count:** 400–1,200 per variant  
**Quantity:** 3+ shape variations

#### Variants:
- Tall palm (thin trunk, dense fronds)
- Bushy palm (thick trunk, spreading fronds)
- Cluster palm (multiple trunks)
- Dead/gnarled palm (obstacle variant)

#### Features:
- Stylized trunk (cylindrical or tapered)
- Frond clusters (not individual leaves)
- Natural wear/color variation
- Palm green color

#### Generation Prompt Template:
```
Generate a low-poly 3D stylized palm tree for a desert game. 
Style: stylized, cartoonish, colorful, low-poly. Features: [TALL: thin trunk with dense upright fronds | 
BUSHY: thick trunk with spreading lateral fronds | CLUSTER: multiple thin trunks with shared frond crown]. 
Color palette: palm green (#66BB6A), trunk brown (#8B6F47), highlights yellow-green (#9CCC65). 
Height: 3–8 units depending on variant. Poly count: under 1,200 triangles. 
Export as .obj or .fbx. Include frond variations for visual interest.
```

---

### 9. Cactus Variations

**Purpose:** Lane obstacles with gameplay significance (jump/slide).  
**Target Poly Count:** 200–1,000 per variant  
**Quantity:** 4+ variants

#### Variants:
- Tall cactus (slide-under obstacle, 2–3 units tall)
- Short cactus (jump-over obstacle, 0.5–1 unit tall)
- Wide barrel cactus (2-wide blocker)
- Cluster cactus (grouping)

#### Features:
- Cylindrical or barrel body
- Spine details
- Natural color gradation
- Desert-accurate appearance (simplified)

#### Generation Prompt Template:
```
Generate a low-poly 3D desert cactus for an endless runner game. 
Style: stylized, cartoonish, colorful, low-poly. Features: [TALL: 2–3 units tall, spiny columns | 
SHORT: 0.5–1 unit tall, barrel shape | WIDE: 2 units wide, blocky barrel | CLUSTER: 3+ varied cacti grouped]. 
Color palette: cactus green (#7CB342), spine beige (#D4A574), shadow green (#558B2F). 
Poly count: under 1,000 triangles. Export as .obj or .fbx. Include spine variations.
```

---

### 10. Rock/Boulder Set

**Purpose:** Lane-blocking and lane-edge obstacles.  
**Target Poly Count:** 300–1,500 per variant  
**Quantity:** 5+ unique shapes

#### Variants:
- Small boulder (lane edge, collectible zone)
- Medium rock (lane blocker)
- Large boulder (multi-lane blocker)
- Jagged rock outcrop
- Stacked rocks

#### Features:
- Irregular natural shape
- Texture variation (cracks, weathering)
- Color gradation
- Desert-accurate color (grey, tan, brown)

#### Generation Prompt Template:
```
Generate a low-poly 3D desert rock/boulder for a game. 
Style: stylized, cartoonish, colorful, low-poly. Purpose: obstacle or lane-edge decoration. 
Features: [SMALL: 1–2 units, rounded | MEDIUM: 2–3 units, irregular | LARGE: 4–5 units, multi-faceted | 
OUTCROP: 3–4 units, jagged | STACK: varied sizes grouped]. Natural weathering, cracks, color variation. 
Color palette: rock grey (#78909C), tan (#C2A878), shadow brown (#5D4037). 
Poly count: under 1,500 triangles. Export as .obj or .fbx. Create 5+ unique shapes for visual variety.
```

---

### 11. Egyptian Decorative Prop Set

**Purpose:** Environmental richness and thematic detail.  
**Target Poly Count:** 100–800 per prop  
**Quantity:** 8–10 unique props

#### Props:
- Ornamental urn (2–3 variants)
- Scarab stone (carved stone scarab)
- Torch (with flame)
- Golden statue (small pharaoh figure)
- Ankh symbol (relief or 3D)
- Cartouche (hieroglyph plaque)
- Offering bowl
- Incense burner

#### Generation Prompt Template:
```
Generate a low-poly 3D Egyptian decorative prop for a game environment. 
Style: stylized, cartoonish, colorful, low-poly. Purpose: environmental detail and thematic richness. 
Prop type: [ORNAMENTAL URN | SCARAB STONE | TORCH | GOLDEN STATUE | ANKH | CARTOUCHE | OFFERING BOWL | INCENSE BURNER]. 
Color palette: terracotta (#E2725B), gold (#FFD700), warm sand (#D4A574). 
Size: 0.5–3 units depending on prop. Poly count: under 800 triangles. 
Export as .obj or .fbx. Include detail variations for uniqueness.
```

---

## Generation Workflow

### Phase 1: Core Assets (Week 1–1.5)
1. **Road Modules** (8 unique segments)
2. **Dune Side-Pieces** (6 variants)
3. **Cactus Variations** (4+ variants)
4. **Palm Trees** (3+ variants)

### Phase 2: Landmarks (Week 1.5–2.5)
1. **Pyramid Set** (3 LOD variants + size variations)
2. **Obelisk Set** (3 scale variants)
3. **Sphinx Prop**
4. **Temple Gate**

### Phase 3: Specialties & Details (Week 2.5–3)
1. **Oasis Module**
2. **Rock/Boulder Set** (5+ variants)
3. **Decorative Props** (8–10 unique pieces)

### Phase 4: Import & Integration (Week 3–3.5)
1. Import all assets into Unity
2. Set up prefabs with correct pivots/scales
3. Create texture atlas
4. Optimize for mobile
5. Verify acceptance criteria

---

## Texture Atlas Strategy

- **Atlas Size:** 1024×1024 (or 2× 512×512 if needed)
- **Padding:** 4–8 pixels between UV islands
- **Compression:** PNG with mipmaps enabled
- **Total Budget:** <50MB for entire Egypt set
- **Naming Convention:** `egypt_atlas_[category]_[variant].png`

---

## Import & Prefab Setup

### Naming Convention:
```
Assets/Models/Egypt/
  ├── Road/
  │   ├── road_segment_01.fbx → Prefab: Road_Segment_01
  │   ├── road_segment_02.fbx → Prefab: Road_Segment_02
  │   └── ... (8+ total)
  ├── Props/
  │   ├── pyramid_near.fbx → Prefab: Pyramid_Near
  │   ├── obelisk_medium.fbx → Prefab: Obelisk_Medium
  │   └── ... (all props)
  └── Environment/
      ├── dune_left.fbx → Prefab: Dune_Left
      ├── oasis.fbx → Prefab: Oasis_Main
      └── ...
```

### Prefab Requirements:
- Correct world-space pivot (center of road segment = origin)
- Scale: 1 unit = 1 meter in-game
- Rotation: Y-axis forward (running direction)
- Material: Linked to shared `egypt_atlas` texture
- Collision: Bounds-based or mesh colliders for obstacles
- Tags: `Road`, `Obstacle`, `Landmark`, `Decoration`

---

## Quality Verification Checklist

- [ ] All assets generated with specified poly counts
- [ ] Textures UV-mapped and atlased
- [ ] Total memory < 50MB
- [ ] Visual style matches concept art (stylized, colorful, cartoonish)
- [ ] Color palette consistent across all assets
- [ ] Lighting realistic (golden-hour warmth)
- [ ] Pivots correct for 3-lane layout
- [ ] All prefabs imported and functional in Unity
- [ ] Min 8 road segments + all props created
- [ ] Visual quality matches or exceeds reference screenshot

---

## Next Steps

1. **Immediate:** Begin Phase 1 asset generation (Road + Dunes + Cacti + Palms)
2. **Parallel:** Document texture atlas layout and UV islands
3. **Follow-up:** Import Phase 1 assets into Unity and verify integration
4. **Final:** Complete Phases 2–4 per schedule

---

**Document Version:** 1.0  
**Last Updated:** 2026-05-07  
**Status:** Active — Production in progress
