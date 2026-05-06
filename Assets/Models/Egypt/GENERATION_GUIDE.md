# Egypt Assets — Generation Guide

## Quick Reference

This folder contains all Egypt-level 3D assets for Desert Dash: Camel Runner. Use this guide to generate, import, and integrate assets into Unity.

---

## Asset Classes & Quick Prompts

### Phase 1: Core Gameplay Assets (Week 1–1.5)

#### Road Modules (8 variants)

Use with **Leonardo.Ai** or **Meshy.ai** text-to-3D model.

**Base Prompt:**
```
Low-poly 3D game asset: 3-lane desert sand road segment for endless runner. 
Stylized, cartoonish, colorful. 10 units long, 5 units wide, 0.5-1 tall. 
Sand lanes with subtle gold lane markings. Sand dune edges. Golden-hour warm lighting. 
Sand beige, warm yellow, gold accents. Under 1,500 triangles. Clean topology.
```

**Variants (add to base):**
- `[Variation: Straight segment]`
- `[Variation: Gentle left curve]`
- `[Variation: Gentle right curve]`
- `[Variation: Banked left turn]`
- `[Variation: Banked right turn]`
- `[Variation: Incline upward]`
- `[Variation: Incline downward]`
- `[Variation: Decorated temple section with obelisk markers on edges]`

**Export Format:** `.fbx` or `.obj` with MTL file

---

#### Dune Side-Pieces (6 variants)

**Base Prompt:**
```
Low-poly 3D sand dune side-piece for 3-lane endless runner. Stylized, cartoonish. 
3-5 units wide, 2-6 tall, 1-2 deep. Lane edge definition and environmental framing. 
Natural sand dune slopes with subtle shadows. Golden-hour lighting. 
Sand beige, warm yellow, tan. Under 800 triangles.
```

**Variants:**
- `[Variation: Left dune, small]`
- `[Variation: Left dune, large]`
- `[Variation: Right dune, small]`
- `[Variation: Right dune, large]`
- `[Variation: Background dune, far distance, low-poly silhouette]`
- `[Variation: Dune with rocky outcrop]`

**Export:** `.fbx` or `.obj`

---

#### Cactus Variations (4+ variants)

**Base Prompt:**
```
Low-poly 3D desert cactus for endless runner game obstacle. Stylized, cartoonish, colorful. 
Features spiny cylindrical or barrel body. Natural color gradation. 
Cactus green, spine beige, shadow tones. Clean topology.
```

**Variants:**
- `[Variation: Tall cactus, 2-3 units, slide-under obstacle]`
- `[Variation: Short cactus, 0.5-1 unit, jump-over obstacle]`
- `[Variation: Wide barrel cactus, 2 units wide blocker]`
- `[Variation: Cluster cactus, 3+ grouped varied cacti]`
- `[Variation: Withered/dead cactus, gnarled appearance]`

**Export:** `.fbx` or `.obj`

---

#### Palm Tree Variations (3+ variants)

**Base Prompt:**
```
Low-poly 3D stylized palm tree for desert game. Colorful, cartoonish, low-poly. 
Stylized trunk and frond clusters (not individual leaves). Palm green, trunk brown, 
highlights yellow-green. 3-8 units tall. Under 1,200 triangles. Clean topology.
```

**Variants:**
- `[Variation: Tall palm, thin trunk, dense upright fronds]`
- `[Variation: Bushy palm, thick trunk, spreading lateral fronds]`
- `[Variation: Cluster palm, multiple thin trunks, shared crown]`
- `[Variation: Young palm, small, sparse fronds]`

**Export:** `.fbx` or `.obj`

---

### Phase 2: Landmark Assets (Week 1.5–2.5)

#### Pyramid Set (3 LOD + size variants)

**Base Prompt:**
```
Low-poly 3D Egyptian pyramid for game background. Stylized, cartoonish, painterly. 
Geometric pyramidal form with subtle surface variations. Golden-hour warm lighting, 
soft shadows. Sand beige, warm yellow, gold accents, shadow tones. Clean topology.
```

**LOD Variants:**
- `[Variation: Near/High-LOD, detailed stone blocks, 5,000 triangles]`
- `[Variation: Mid/Medium-LOD, simplified facets, 2,000 triangles]`
- `[Variation: Far/Low-LOD, silhouette only, 500 triangles]`

**Size Variants:**
- `[Variation: Small pyramid, foreground]`
- `[Variation: Large pyramid, mid-ground]`
- `[Variation: Pyramid cluster, 3+ distant pyramids as single asset]`

**Export:** `.fbx` or `.obj`

---

#### Obelisk Set (3 scale variants)

**Base Prompt:**
```
Low-poly 3D Egyptian obelisk monument. Stylized, cartoonish, colorful. 
Tapered square pillar with pointed pyramidal top. Hieroglyph-carved details 
on all faces. Warm sandstone texture. Golden-hour lighting with soft shadows. 
Terracotta, warm sand, gold highlights. Under 1,500 triangles per variant.
```

**Variants:**
- `[Variation: Small obelisk, 2-3 units tall, foreground]`
- `[Variation: Medium obelisk, 5-7 units tall, mid-ground]`
- `[Variation: Large obelisk, 10-15 units tall, background]`

**Export:** `.fbx` or `.obj`

---

#### Sphinx Prop

**Prompt:**
```
Low-poly 3D Egyptian Sphinx monument for game. Stylized, cartoonish, colorful. 
Reclining pose: 10-15 units long, 6-8 tall, 3-4 wide. Features human face with 
Egyptian headdress (nemes), leonine body, hieroglyph reliefs on chest/base. 
Warm sandstone texture. Golden-hour lighting. Terracotta, warm sand, gold, shadows. 
Under 6,000 triangles. Clean topology.
```

**Export:** `.fbx` or `.obj`

---

#### Temple Gate

**Prompt:**
```
Low-poly 3D ancient Egyptian temple gate for game background. Stylized, cartoonish. 
6-8 units wide, 8-12 tall, 2-3 deep. Two tall pillars with hieroglyph-inspired 
carvings, overhead beam/lintel, decorative relief (ankhs, scarabs, lotus). 
Warm sandstone texture. Golden-hour lighting. Terracotta, warm sand, gold accents. 
Under 3,500 triangles. Include 2-3 scale variants for layering.
```

**Export:** `.fbx` or `.obj`

---

### Phase 3: Specialty & Detail Assets (Week 2.5–3)

#### Oasis Module

**Prompt:**
```
Low-poly 3D oasis environment module for endless runner. Stylized, cartoonish, colorful. 
8-12 units wide, 6-10 tall, 4-6 deep. Features turquoise water body, 2-3 stylized 
palm trees, rocks/boulders, grass/reed details. Golden-hour lighting with warm sun. 
Oasis blue, palm green, sand beige, warm yellow. Under 3,000 triangles total. 
Export as modular pieces: water + palms + rocks separate for flexibility.
```

**Variants:**
- `[Variation: Standard oasis with 2 palms]`
- `[Variation: Large oasis with 3 palms and larger water body]`
- `[Variation: Rocky oasis, more boulders, fewer palms]`

**Export:** `.fbx` or `.obj` (modular)

---

#### Rock/Boulder Set (5+ unique shapes)

**Base Prompt:**
```
Low-poly 3D desert rock/boulder for game obstacle or decoration. Stylized, 
cartoonish, colorful. Irregular natural shape with texture variation (cracks, weathering), 
color gradation. Rock grey, tan, shadow brown. Clean topology.
```

**Variants:**
- `[Variation: Small boulder, 1-2 units, rounded, lane edge]`
- `[Variation: Medium rock, 2-3 units, irregular, lane blocker]`
- `[Variation: Large boulder, 4-5 units, multi-faceted, multi-lane blocker]`
- `[Variation: Jagged rock outcrop, 3-4 units, sharp angles]`
- `[Variation: Stacked rocks, varied sizes grouped]`
- `[Additional unique shapes for visual variety]`

**Export:** `.fbx` or `.obj`

---

#### Egyptian Decorative Props (8–10 unique)

**Base Prompt:**
```
Low-poly 3D Egyptian decorative prop for game environment. Stylized, cartoonish, 
colorful, low-poly. Environmental detail and thematic richness. 
Terracotta, gold, warm sand. 0.5-3 units. Under 800 triangles.
```

**Props (generate individually):**
- `[Prop: Ornamental urn, variant 1 - narrow neck]`
- `[Prop: Ornamental urn, variant 2 - wide belly]`
- `[Prop: Ornamental urn, variant 3 - decorative handles]`
- `[Prop: Scarab stone, carved stone scarab, sitting pose]`
- `[Prop: Torch, stylized with flame effect support]`
- `[Prop: Golden statue, small pharaoh figure]`
- `[Prop: Ankh symbol, relief or 3D form]`
- `[Prop: Cartouche, hieroglyph plaque, rectangular]`
- `[Prop: Offering bowl, shallow decorative vessel]`
- `[Prop: Incense burner, pedestal with bowl]`

**Export:** `.fbx` or `.obj` (each prop separate)

---

## Generation Workflow

### Using Leonardo.Ai
1. Log in to [Leonardo.ai](https://leonardo.ai)
2. Choose **Text-to-3D Model** tool
3. Copy prompt from this guide into the input field
4. Generate model
5. Review and download as `.fbx` or `.obj`
6. Place in appropriate folder: `Road/`, `Props/`, or `Environment/`

### Using Meshy.ai
1. Log in to [Meshy.ai](https://www.meshy.ai)
2. Choose **Text-to-3D** generation
3. Copy prompt from this guide
4. Generate with appropriate parameters (poly count: low)
5. Download `.fbx` or `.obj`
6. Place in appropriate folder

### Manual Blender Alternative
If automated generation is unavailable:
1. Open Blender
2. Create assets following the prompt specifications manually
3. Ensure poly counts match spec
4. Optimize topology for mobile
5. Export as `.fbx` with clean materials

---

## Import Checklist

After generating each asset:

- [ ] File format: `.fbx` or `.obj` with `.mtl`
- [ ] Poly count within spec (see EGYPT_ASSETS_PIPELINE.md)
- [ ] Texture files included (if generated with textures)
- [ ] Mesh clean (no n-gons, good edge flow)
- [ ] Pivot at origin (0,0,0) for road; center for props
- [ ] Scale: 1 unit = 1 meter
- [ ] Naming: descriptive, matches folder structure

---

## Next: Import into Unity

Once assets are generated:
1. Place `.fbx` files in appropriate `Road/`, `Props/`, or `Environment/` folder
2. Run Unity import process (see UNITY_IMPORT_WORKFLOW.md)
3. Create prefabs with correct pivots and materials
4. Set up texture atlas
5. Verify in-game placement and visual quality

---

**Status:** Phase 1 generation in progress  
**Generated Assets:** 0/21 (Phase 1)  
**Total Assets Required:** 60+ (all phases)

---

## Resources

- **GDD:** `/GDD.md`
- **Production Pipeline:** `/EGYPT_ASSETS_PIPELINE.md`
- **Visual Assets Prompts:** `/visual_asset_prompts.md`
- **Import Workflow:** `UNITY_IMPORT_WORKFLOW.md` (to be created)
