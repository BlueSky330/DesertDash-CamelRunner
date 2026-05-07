# Camel Skin Material Templates

**Version:** 1.0  
**Created:** 2026-05-07

---

## Overview

All 5 camel skin variants share the same base mesh and rig. The difference is purely visual: each skin has a unique material with different textures and colors. This approach minimizes memory overhead and allows instant skin swapping at runtime.

**Material Assignment Strategy:**
- **SkinSwapManager.cs** listens to `SkinManager.onSkinEquipped` event
- When a skin is equipped, it swaps the SkinnedMeshRenderer's material
- No mesh/rig changes needed, only material swap

---

## Skin Definitions

### 1. Camel (Base) — `Mat_Camel_Base`

**Visual Characteristics:**
- Natural tan/brown desert camel
- Subtle texture variation
- Minimal ornaments

**Material Settings:**
```
Shader: Standard (or Mobile/Diffuse for extreme optimization)
Albedo Map: Tex_Camel_Base_Diffuse.png (256×256, sRGB)
Normal Map: Tex_Camel_Normal.png (256×256, Linear)
Metallic: 0.0
Smoothness: 0.4 (rough, sandy appearance)
Tiling: (1.0, 1.0)
Offset: (0.0, 0.0)
```

**Color Tint (if procedural variation):**
- Primary: #D4A574 (tan)
- Secondary: #8B7355 (brown)
- Accent: #F4E4C1 (light belly)

---

### 2. Pharaoh Camel — `Mat_Camel_Pharaoh`

**Visual Characteristics:**
- Royal gold and jeweled aesthetic
- Ornate headdress (baked into mesh)
- Luxurious appearance, befitting Egyptian royalty

**Material Settings:**
```
Shader: Standard (with metallic workflow)
Albedo Map: Tex_Camel_Pharaoh_Diffuse.png (256×256, sRGB)
Normal Map: Tex_Camel_Normal.png (256×256, Linear)
Metallic Map: Tex_Camel_Pharaoh_Metallic.png (256×256, optional)
Metallic: 0.8 (shiny, gold-like)
Smoothness: 0.7 (polished, jeweled)
Tiling: (1.0, 1.0)
Offset: (0.0, 0.0)
```

**Color Tint:**
- Primary: #FFD700 (gold)
- Secondary: #DAA520 (darker gold)
- Accent: #FF6347 (ruby red, for jewels)

---

### 3. Racing Camel — `Mat_Camel_Racing`

**Visual Characteristics:**
- Sleek, aerodynamic stripes
- Sport-inspired color scheme
- Fast-looking design (red/white/blue)

**Material Settings:**
```
Shader: Standard
Albedo Map: Tex_Camel_Racing_Diffuse.png (256×256, sRGB)
Normal Map: Tex_Camel_Normal.png (256×256, Linear)
Metallic: 0.1 (slight sheen)
Smoothness: 0.6 (aerodynamic polish)
Tiling: (1.0, 1.0)
Offset: (0.0, 0.0)
```

**Color Tint:**
- Primary: #FF0000 (racing red)
- Secondary: #FFFFFF (white stripes)
- Accent: #0000FF (blue accent)

---

### 4. Mummy Camel — `Mat_Camel_Mummy`

**Visual Characteristics:**
- Wrapped in linen bandages
- Ancient, mysterious appearance
- Off-white/beige with aged, frayed texture

**Material Settings:**
```
Shader: Standard
Albedo Map: Tex_Camel_Mummy_Diffuse.png (256×256, sRGB)
Normal Map: Tex_Camel_Mummy_Normal.png (256×256, Linear) [can be custom, rougher texture]
Metallic: 0.0
Smoothness: 0.3 (rough, cloth-like)
Tiling: (1.2, 1.2) [slight tiling for wrapping effect]
Offset: (0.0, 0.0)
```

**Color Tint:**
- Primary: #E8D4C4 (aged linen)
- Secondary: #C4A888 (faded brown stains)
- Accent: #8B7355 (dark shadow for depth)

---

### 5. Golden Camel — `Mat_Camel_Golden`

**Visual Characteristics:**
- Shimmering solid gold coat
- Most expensive, premium skin
- High metallic sheen, luxury appearance

**Material Settings:**
```
Shader: Standard (Specular setup for better control)
Albedo Map: Tex_Camel_Golden_Diffuse.png (256×256, sRGB)
Normal Map: Tex_Camel_Normal.png (256×256, Linear)
Metallic Map: Tex_Camel_Golden_Metallic.png (256×256, full white)
Metallic: 1.0 (full metal)
Smoothness: 0.85 (highly polished, mirror-like)
Tiling: (0.8, 0.8) [slightly compressed for compaction effect]
Offset: (0.0, 0.0)
```

**Color Tint:**
- Primary: #FFD700 (pure gold)
- Secondary: #FFA500 (orange-gold variation)
- Accent: #FFFACD (light golden highlights)

---

## Shared Resources

### Normal Map — `Tex_Camel_Normal.png`

A single normal map is used across **all skin variants** to save memory and ensure consistent surface detail.

**Specifications:**
- Resolution: 256×256
- Format: PNG 32-bit (RGBA)
- Compression: DXT5 / ETC2 (Normal compression)
- Usage: All skins reference this map

**Content:**
- Camel fur detail: fine, directional normal flow
- Ridges and creases in the model
- NO color variation (neutral normals)

---

## Thief Materials

### Desert Bandit — `Mat_DesertBandit`

**Visual:** Desert raider with cloth wrappings  
**Shader:** Standard  
**Albedo:** `Tex_DesertBandit_Diffuse.png` (256×256)  
**Metallic:** 0.0  
**Smoothness:** 0.3 (cloth)

### Ninja Thief — `Mat_NinjaThief`

**Visual:** Dark, stealthy ninja  
**Shader:** Standard  
**Albedo:** `Tex_NinjaThief_Diffuse.png` (256×256)  
**Metallic:** 0.2 (armor accents)  
**Smoothness:** 0.5 (polished armor)

### Pirate — `Mat_Pirate`

**Visual:** Swashbuckling pirate  
**Shader:** Standard  
**Albedo:** `Tex_Pirate_Diffuse.png` (256×256)  
**Metallic:** 0.3 (weapons, buckles)  
**Smoothness:** 0.4 (weathered)

### Shadow Thief — `Mat_ShadowThief`

**Visual:** Supernatural, shadow-wreathed thief  
**Shader:** Standard with custom emissive  
**Albedo:** `Tex_ShadowThief_Diffuse.png` (256×256)  
**Emissive:** Slight purple/dark glow for mystique  
**Metallic:** 0.1  
**Smoothness:** 0.6 (mysterious sheen)

---

## Runtime Material Setup (Code)

### SkinSwapManager Inspector Assignment

In the Unity Editor, on the **Camel game object**:

1. Add **SkinSwapManager** component
2. Assign **SkinnedMeshRenderer** reference
3. In the **Skin Materials** array, create 5 entries:
   ```
   [0] Skin Name: "Camel (Base)"      → Material: Mat_Camel_Base
   [1] Skin Name: "Pharaoh Camel"     → Material: Mat_Camel_Pharaoh
   [2] Skin Name: "Racing Camel"      → Material: Mat_Camel_Racing
   [3] Skin Name: "Mummy Camel"       → Material: Mat_Camel_Mummy
   [4] Skin Name: "Golden Camel"      → Material: Mat_Camel_Golden
   ```

### Skin Names (Must Match Exactly)

These names are defined in **SkinManager.InitializeSkins()** and must match exactly:

- `"Camel (Base)"`
- `"Pharaoh Camel"`
- `"Racing Camel"`
- `"Mummy Camel"`
- `"Golden Camel"`

---

## Performance Notes

### Memory Impact

- **Single mesh:** 1.2 MB (shared across all skins)
- **5 materials:** ~100 KB total
- **Textures (5 diffuse):** ~320 KB (256×256 compressed)
- **Normal map (shared):** ~64 KB
- **Total per camel:** ~1.7 MB

### Runtime Swapping

- Material swap is **O(1)** — no mesh modifications
- No instantiation/destruction overhead
- Material instance created at runtime, doesn't affect asset

### Quality Trade-offs

- **Mobile-friendly:** All textures 256×256 or smaller
- **Detail:** Normal map provides surface variation without geometry
- **Cost:** All skins use Standard shader (mobile-optimized)

---

## Testing Checklist

- [ ] All 5 material files exist in project
- [ ] Skin names in SkinManager match material set names
- [ ] SkinSwapManager references assigned correctly
- [ ] Test material swap: equip each skin, verify color change
- [ ] Test memory: Profile → Memory → check material instance count
- [ ] Test on mobile device or iOS/Android emulator
- [ ] Verify no flickering during skin swap
- [ ] Check that normal map isn't overwritten per-skin

---

## Future Enhancements

1. **Variants per skin:** Color variations (red racing camel, blue racing camel)
2. **Procedural tinting:** Shader-based color variation without new textures
3. **Animation overrides:** Different skins could have slightly different run speeds (visual feedback)
4. **Emissive skins:** Golden Camel could have subtle glow in dark scenes

