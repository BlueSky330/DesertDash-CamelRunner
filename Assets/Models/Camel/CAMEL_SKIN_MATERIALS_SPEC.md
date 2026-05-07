# Camel Skin Material Variants Specification

**Task:** AIG-78 — AIG-9.2: Create 4 Camel Skin Material Variants  
**Platform:** Unity 2022.3.62f1 LTS  
**Scope:** Material overrides only (no geometry changes)  
**Target:** Crossy Road / Subway Surfers aesthetic (low-poly, colorful, cartoonish)

---

## Overview

Four distinct material variants are applied to the base camel mesh via material overrides. Each variant maintains the same mesh topology and bone structure, with only color, texture, and visual properties changing.

All materials use Unity's Standard Shader or custom Shader Graph for cartoonish rendering with:
- Flat or simple gradient colors (no photorealistic textures)
- Metallic accents where specified (gold, gems)
- Emission for glowing effects (mummy eyes)
- Albedo, Normal, Metallic, and Smoothness maps where needed

---

## Material Variants

### 1. Pharaoh Camel

**Visual Identity:** Regal Egyptian theme with gold and blue colors, ornate accessories

**Color Palette:**
- Body: Tan/sand (#C4A574)
- Primary Accent: Gold (#FFD700)
- Secondary Accent: Royal Blue (#4169E1)
- Gem Color: Sapphire Blue (#0F52BA)
- Details: Black (#1a1a1a) for outlines/shadows

**Component Breakdown:**

| Body Part | Material | Color | Metallic | Smoothness | Notes |
|-----------|----------|-------|----------|------------|-------|
| Head/Body | Tan Base | #C4A574 | 0.0 | 0.5 | Matte sandy texture |
| Nemes Headdress | Gold/Blue Striped | Gold #FFD700 + Blue #4169E1 | 0.8 | 0.7 | Striped pattern, shiny metal |
| Gold Collar | Polished Gold | #FFD700 | 1.0 | 0.8 | High metallic, reflects light |
| Collar Gem (center) | Sapphire | #0F52BA | 1.0 | 0.9 | Highly polished, gem-like |
| Wrist/Ankle Bands | Gold | #FFD700 | 0.9 | 0.7 | Metallic gold rings |
| Eyes | Bright Eyes | #FFFFF0 + #000000 | 0.0 | 0.8 | White with black pupil |
| Mouth | Tan Base | #C4A574 | 0.0 | 0.5 | Matches body |

**Texture Details:**
- Gold surfaces: Slight brushed metal normal map (diagonal grain pattern)
- Sapphire gem: Faceted normal map to simulate gem cuts
- Headdress stripes: UV-mapped 50/50 gold/blue pattern

**Shader:** Standard (Metallic setup) or custom "Cartoonish Gold" shader graph for exaggerated shine

---

### 2. Racing Camel

**Visual Identity:** High-speed athlete with racing gear, yellow and black color scheme

**Color Palette:**
- Body: Tan/sand (#C4A574)
- Primary Accent: Racing Yellow (#FFD700)
- Secondary Accent: Matte Black (#1a1a1a)
- Visor: Dark Gray (#4a4a4a)
- Stripes: High-contrast yellow/black

**Component Breakdown:**

| Body Part | Material | Color | Metallic | Smoothness | Notes |
|-----------|----------|-------|----------|------------|-------|
| Head/Body | Tan Base | #C4A574 | 0.0 | 0.5 | Matte sandy |
| Racing Goggles | Black Plastic | #1a1a1a | 0.2 | 0.4 | Matte, sports look |
| Goggle Visor | Reflective Yellow | #FFD700 | 0.6 | 0.7 | Tinted visor, slightly reflective |
| Racing Harness (back) | Black/Yellow Striped | #1a1a1a + #FFD700 | 0.1 | 0.3 | Diagonal stripe pattern |
| Harness Trim | Racing Yellow | #FFD700 | 0.0 | 0.4 | Matte racing stripe |
| Leg Wraps | Striped (alternating) | #FFD700 / #1a1a1a | 0.0 | 0.5 | Tight striped bands around legs |
| Eyes | Bright Eyes | #FFFFF0 + #000000 | 0.0 | 0.8 | Alert expression |
| Mouth | Tan Base | #C4A574 | 0.0 | 0.5 | Matches body |

**Texture Details:**
- Goggles: Glossy black plastic appearance
- Racing harness: Bold diagonal stripe UV layout (45° angle)
- Leg wraps: Vertical stripe bands (every other stripe band is yellow/black)

**Shader:** Standard (minimal metallic) or "Sports Gear" shader graph for matte sports equipment look

---

### 3. Mummy Camel

**Visual Identity:** Ancient wrapped mummy with supernatural glowing eyes

**Color Palette:**
- Body: Wrapped White (#F5F5F5)
- Wrapping: Off-white (#E8E8E8) with tan/brown shadows (#8B7355)
- Eyes: Glowing Yellow (#FFFF00)
- Depth Shadows: Dark Brown (#3E3E3E)
- Tattered edges: Brown (#654321)

**Component Breakdown:**

| Body Part | Material | Color | Metallic | Smoothness | Emission | Notes |
|-----------|----------|-------|----------|------------|----------|-------|
| Wrapped Body | White Linen | #F5F5F5 | 0.0 | 0.3 | 0.0 | Cloth texture with wrinkles |
| Wrap Shadows | Brown Shadow | #8B7355 | 0.0 | 0.2 | 0.0 | Deep shadows in wrap creases |
| Wrap Edges | Tattered Brown | #654321 | 0.0 | 0.2 | 0.0 | Loose, torn wrap edges |
| Left Eye | Glowing Yellow | #FFFF00 | 0.0 | 0.8 | 2.0 | Bright emission for supernatural look |
| Right Eye | Glowing Yellow | #FFFF00 | 0.0 | 0.8 | 2.0 | Synchronized with left eye |
| Nostril Shadow | Dark Brown | #3E3E3E | 0.0 | 0.1 | 0.0 | Deep nasal shadow |
| Mouth | Brown Shadow | #8B7355 | 0.0 | 0.2 | 0.0 | Concealed by wrappings |

**Texture Details:**
- Wrapped body: Linen cloth normal map with tight weave pattern
- Shadows: Hand-painted into the wrapped geometry using vertex colors or baked ambient occlusion
- Glowing eyes: Emission map at intensity 2.0 for supernatural glow
- Tattered edges: Normal map showing loose thread/fiber direction

**Shader:** Standard with Emission enabled, or "Glowing Mummy" custom shader graph for enhanced eye glow

**Special Notes:**
- Eye glow should be visible in dark scenes (emission doesn't require light)
- Wrapping should suggest ancient, weathered cloth texture
- Consider bloom post-processing to enhance eye glow effect

---

### 4. Golden Camel

**Visual Identity:** Fully gold body with luxury, gemstone accents and decorative tassels

**Color Palette:**
- Primary Gold: Bright Gold (#FFD700)
- Shadow Gold: Deep Gold (#B8860B)
- Gem 1 (chest): Ruby Red (#E0115F)
- Gem 2 (flank): Emerald Green (#50C878)
- Gem 3 (head): Sapphire Blue (#0F52BA)
- Tassel: Gold (#FFD700) with thread details
- Geometric Pattern: Gold (#FFD700) with shadow (#B8860B)

**Component Breakdown:**

| Body Part | Material | Color | Metallic | Smoothness | Notes |
|-----------|----------|-------|----------|------------|-------|
| Main Body | Polished Gold | #FFD700 | 1.0 | 0.85 | High shine, fully metallic |
| Body Shadows | Deep Gold | #B8860B | 1.0 | 0.7 | Shadow definition in creases |
| Chest Gem | Ruby | #E0115F | 1.0 | 0.95 | Faceted gem appearance |
| Flank Gem | Emerald | #50C878 | 1.0 | 0.95 | Faceted gem appearance |
| Head Gem | Sapphire | #0F52BA | 1.0 | 0.95 | Faceted gem appearance |
| Neck Tassel | Gold Thread | #FFD700 | 0.9 | 0.6 | Hanging tassel with cloth feel |
| Geometric Pattern | Pattern Gold/Shadow | #FFD700 / #B8860B | 0.8 | 0.75 | Embossed diamond/triangle pattern |
| Eyes | Bright Eyes | #FFFFF0 + #000000 | 0.0 | 0.8 | Contrasting white/black |
| Mouth | Deep Gold | #B8860B | 1.0 | 0.7 | Matches shadow tone |

**Texture Details:**
- Main body: Brushed gold normal map with directional grain
- Gem stones: Faceted normal maps to simulate multi-faceted cut stones
- Geometric pattern: UV-mapped or hand-painted pattern overlay showing diamond/triangle designs
- Tassels: Separate small mesh or texture detail hanging from neck

**Shader:** Standard (Metallic) with high metallic value and smoothness, or "Luxury Gold" custom shader graph for exaggerated metallic shine

**Special Notes:**
- Gems should have high specular highlights
- Geometric pattern can be applied via emission or normal map for visual pop
- Consider using specular highlights to enhance jewelry appearance

---

## Material Application Workflow

### Setup Steps (once Camel_Default.fbx is imported):

1. **Import FBX** into Assets/Models/Camel/
2. **Create Material Folder** at Assets/Materials/Camel/Skins/
3. **Create Four Material Files:**
   - Camel_Pharaoh.mat
   - Camel_Racing.mat
   - Camel_Mummy.mat
   - Camel_Golden.mat
4. **Apply Materials:**
   - Default/Inspector: Camel_Pharaoh.mat (standard variant)
   - Create Prefab Variants for Racing, Mummy, Golden with appropriate material swaps
5. **Validate in Scene:**
   - Test each skin variant in the game scene
   - Verify colors match concept art
   - Check metallics and glows in gameplay lighting
   - Test on mobile target device if possible

### Material Naming Convention:
```
Assets/Materials/Camel/Skins/
├── Camel_Pharaoh.mat
├── Camel_Racing.mat
├── Camel_Mummy.mat
└── Camel_Golden.mat
```

### Prefab Variants:
```
Assets/Prefabs/
├── Camel_Default.prefab (uses Camel_Pharaoh material)
├── Camel_Pharaoh.prefab (variant)
├── Camel_Racing.prefab (variant)
├── Camel_Mummy.prefab (variant)
└── Camel_Golden.prefab (variant)
```

---

## Technical Requirements

**Engine:** Unity 2022.3.62f1 LTS  
**Shader:** Unity Standard Shader (built-in) or Shader Graph  
**Target Platform:** Mobile (iOS/Android)  
**Performance Budget:** Single material per character variant (no dynamic switching needed)  
**Texture Budget:** Keep metallic/normal maps at 512x512 or smaller

---

## Validation Checklist

- [ ] All four materials created and assigned
- [ ] Colors match concept art references
- [ ] Metallic values produce appropriate shine
- [ ] Mummy eye glow is visible in dark lighting
- [ ] Materials render correctly on mobile device
- [ ] No z-fighting or texture bleeding
- [ ] Performance acceptable (< 0.5ms per character)
- [ ] Prefab variants ready for character spawning system

---

## Notes & Future Extensions

- If animation adds procedural clothing/attachment meshes later, additional materials may be needed for accessories
- Current spec assumes single shared texture for all skins; if unique textures needed per skin, update texture budget
- Emission on Mummy eyes may need bloom adjustment based on final lighting setup
- Metallic gems on Pharaoh and Golden skins benefit from screen-space reflections or planar reflections if performance allows
