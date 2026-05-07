# App Icon Generation Specification

**Game**: Desert Dash: Camel Runner  
**Status**: ✅ Ready for Generation  
**Last Updated**: 2026-05-07

---

## Spec Summary

The app icon specification from `docs/launch/APP_ICON_SPEC.md` is **complete and approved**. This document confirms the design brief, color palette, and all required output sizes are finalized and ready for AI image generation.

---

## AI Generation Prompt

Use this prompt with **DALL-E 3**, **Midjourney**, **Leonardo.Ai**, or **Stable Diffusion**:

```
Low-poly geometric app icon, cute cartoon camel running in desert, 
pyramids silhouette background, warm golden sunset colors 
(orange and yellow gradient), flat design, mobile game icon style, 
clean and bold, centered composition, no text.

Color palette: gold (#FFD700), orange (#FF8C00), sand (#EDC9AF), 
sky blue (#87CEEB). 
Camel facing right, profile view, visible hump, active running stance.
Minimalist flat design, not photorealistic. 
Must be recognizable at small sizes (down to 32×32 px).
```

---

## Design Specifications (from APP_ICON_SPEC.md)

### Core Visual Elements
- **Camel**: Silhouette profile facing right, visible hump, active running pose
- **Background**: Desert landscape (pyramid and/or dunes) against sunset gradient
- **Color Palette**:
  - Primary: Gold (#FFD700), Orange (#FF8C00), Sand (#EDC9AF)
  - Secondary: Sky Blue (#87CEEB), Shadow Brown (#8B4513)
  - Accent: White (#FFFFFF)
- **Style**: Flat design with pixel-art influences, minimalist, highly recognizable at small sizes

### Required Output Sizes

| Purpose | Size | Format | Background | Notes |
|---------|------|--------|------------|-------|
| Google Play Main | 512×512 px | PNG-32 | Solid (no alpha) | <1MB, highest quality |
| Apple App Store | 1024×1024 px | PNG-32 | Solid (no alpha) | High resolution |
| Adaptive Icon Foreground | 108×108 px (dp) | PNG-32 | **Transparent** | Critical elements within center 66×66 dp safe zone |
| Adaptive Icon Background | 108×108 px (dp) | PNG-32 | Solid (no alpha) | Sand/gold gradient or solid color |
| Android Launcher | 48×48, 72×72, 96×96, 144×144, 192×192 px | PNG-32 | Solid (no alpha) | Scaled from 512×512 base |
| Feature Graphic (GP) | 1024×500 px | PNG-32 | Solid (no alpha) | Landscape orientation |
| Favicon | 32×32 px | PNG-32 | Transparent or solid | Website/store pages |

---

## Post-Generation Export Steps

Once the AI generates the base icon (512×512 or larger):

### 1. **Solid Background (Main Icons)**
- Remove any transparency and add solid background color (golden sand #EDC9AF or warm orange #FF8C00)
- Verify no anti-aliasing artifacts or rough edges
- Export as PNG-32 (RGBA, 32-bit)

### 2. **Resize to Required Sizes**
Use ImageMagick, Figma, or Photoshop:

```bash
# Example with ImageMagick (if available)
convert icon_512.png -resize 1024x1024 icon_1024.png
convert icon_512.png -resize 108x108 icon_108_fg.png  # (with transparency)
convert icon_512.png -resize 180x180 icon_180_ios.png
```

### 3. **Adaptive Icon Foreground (108×108 with Transparency)**
- Extract camel + any immediate elements
- Add transparent background
- Ensure camel fits within center 66×66 dp safe zone (critical elements only)
- Export with alpha channel

### 4. **Adaptive Icon Background (108×108 Solid)**
- Use gradient or solid sand/gold color matching main icon
- No transparency
- Export as PNG-32

### 5. **Quality Checks**
- [ ] Test icon readability at 32×32 px (smallest size)
- [ ] Verify colors match approved palette (#FFD700, #FF8C00, #EDC9AF, #87CEEB)
- [ ] Confirm camel is recognizable and centered
- [ ] No blur or pixelation at small sizes
- [ ] All file sizes under 500 KB
- [ ] No spelling errors in any text
- [ ] Design is original (not copyrighted)

---

## File Naming & Organization

### Expected Deliverables

Place all final icons in `Assets/AppIcon/` with these filenames:

```
Assets/AppIcon/
├── icon_512.png                    (Google Play main, 512×512)
├── icon_1024.png                   (Apple App Store, 1024×1024)
├── icon_108_foreground.png         (Adaptive foreground, transparent, 108×108)
├── icon_108_background.png         (Adaptive background, solid, 108×108)
├── icon_1024x500_feature.png       (Google Play feature graphic, 1024×500)
├── icon_180_ios.png                (iOS alternative, 180×180)
├── icon_120_ios.png                (iOS alternative, 120×120)
├── icon_58_ios.png                 (iOS alternative, 58×58)
├── icon_32_favicon.png             (Favicon, 32×32)
└── icon_launcher_variants/
    ├── icon_48.png                 (mdpi, 48×48)
    ├── icon_72.png                 (hdpi, 72×72)
    ├── icon_96.png                 (xhdpi, 96×96)
    ├── icon_144.png                (xxhdpi, 144×144)
    └── icon_192.png                (xxxhdpi, 192×192)
```

---

## Tools Available for Generation

### AI Image Generators (Recommended)
- **DALL-E 3**: https://openai.com/dall-e-3 (requires API key or web access)
- **Midjourney**: https://www.midjourney.com (Discord-based)
- **Leonardo.Ai**: https://leonardo.ai (free tier available)
- **Stable Diffusion**: https://stablediffusionweb.com (free web-based)

### Post-Processing Tools
- **ImageMagick**: Command-line image manipulation (if installed: `convert` or `magick`)
- **Figma**: Free design tool with batch export (https://figma.com)
- **GIMP**: Free image editor (open-source)
- **Photoshop**: Professional tool (requires license)
- **TinyPNG**: PNG compression (https://tinypng.com)

---

## Next Actions

1. **Copy the AI generation prompt above** (section "AI Generation Prompt")
2. **Paste into your chosen AI tool** (DALL-E, Midjourney, Leonardo.Ai, or Stable Diffusion)
3. **Generate at 512×512 or higher resolution**
4. **Download the generated image**
5. **Verify quality** (recognizable at small sizes, colors match palette)
6. **Resize to all required dimensions** using ImageMagick, Figma, or Photoshop
7. **Save to `Assets/AppIcon/`** with naming convention above
8. **Verify all files** against acceptance criteria below
9. **Commit and push** to repository

---

## Acceptance Criteria

- [ ] `Assets/AppIcon/icon_512.png` exists, 512×512, PNG-32, no alpha, <1MB
- [ ] `Assets/AppIcon/icon_1024.png` exists, 1024×1024, PNG-32, no alpha
- [ ] `Assets/AppIcon/icon_108_foreground.png` exists, 108×108, PNG-32, transparent background
- [ ] `Assets/AppIcon/icon_108_background.png` exists, 108×108, PNG-32, solid background
- [ ] Camel silhouette is clearly visible in all sizes (especially 32×32 test)
- [ ] Desert/pyramid elements visible in main icons
- [ ] Colors match approved palette
- [ ] All files are PNG-32 format (RGBA)
- [ ] File sizes appropriate (under 500 KB each)
- [ ] Design is original and not copyrighted
- [ ] All files committed to `Assets/AppIcon/` and pushed to repository

---

## Specification Approval

✅ **Specification Status**: APPROVED & FINALIZED  
✅ **Design Brief**: Complete (from APP_ICON_SPEC.md)  
✅ **Color Palette**: Finalized  
✅ **Size Requirements**: All specified  
✅ **Generation Prompt**: Ready to use  
✅ **Ready for Generation**: YES

This specification is approved and ready for immediate implementation.
