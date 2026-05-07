# App Icon Design Specification

**Game**: Desert Dash: Camel Runner  
**Version**: 1.0  
**Status**: Ready for Design / AI Generation

---

## Design Brief

### Visual Concept
A **camel silhouette against a desert sunset** with iconic Egyptian elements (pyramids, sand dunes). The design should be:
- **Instantly recognizable** at small sizes (app drawer, taskbar)
- **Desert-themed** with gold, orange, sand, and blue colors
- **Friendly and approachable** to appeal to casual gamers
- **Game-appropriate** with clear game title integration
- **Culturally respectful** to Egyptian and MENA audiences

### Core Elements
1. **Camel**: Facing right, proud profile, minimalist silhouette
2. **Background**: Desert landscape (pyramid, dunes, or sunset gradient)
3. **Color Palette**: 
   - Primary: Gold (#FFD700), Orange (#FF8C00), Sand (#EDC9AF)
   - Secondary: Sky Blue (#87CEEB), Shadow Brown (#8B4513)
   - Accent: White (#FFFFFF) for contrast
4. **Game Title**: "Desert Dash" or "Camel Runner" integrated subtly
5. **Style**: Pixel art or flat design (consistent with in-game aesthetic)

---

## Required Icon Sizes and Formats

### Google Play Store

| Use Case | Size | Format | Notes |
|----------|------|--------|-------|
| **App Icon (Main)** | 512×512 px | PNG-32 (RGBA) | High resolution; will be scaled down |
| **Feature Graphic** | 1024×500 px | PNG-32 (RGBA) | Landscape orientation for store listing |
| **Adaptive Icon Foreground** | 108×108 px (safe zone: 66×66) | PNG-32 (RGBA) | Camel + title on transparent background |
| **Adaptive Icon Background** | 108×108 px | PNG-32 (RGBA) | Gradient or solid desert color |

### Apple App Store

| Use Case | Size | Format | Notes |
|----------|------|--------|-------|
| **App Icon (Main)** | 1024×1024 px | PNG-32 (RGBA) | Required; do NOT add transparency |
| **App Icon (Alternative)** | 180×180 px, 120×120 px, 58×58 px | PNG-32 (RGBA) | iOS auto-scales from 1024px; optional but recommended |

### Windows / Web / Other

| Use Case | Size | Format | Notes |
|----------|------|--------|-------|
| **Favicon** | 32×32 px | PNG-32 (RGBA) or ICO | For website/store pages |
| **Twitter Card** | 1200×630 px | PNG-32 (RGBA) or JPG | For social media sharing |
| **Marketing Asset** | 512×512 px (square) | PNG-32 (RGBA) | For press kits and promotional materials |

---

## Design Guidelines

### Color Palette Reference

```
Primary Colors:
  Gold:          #FFD700 (RGB: 255, 215, 0)
  Orange:        #FF8C00 (RGB: 255, 140, 0)
  Sand/Tan:      #EDC9AF (RGB: 237, 201, 175)

Secondary Colors:
  Sky Blue:      #87CEEB (RGB: 135, 206, 235)
  Shadow Brown:  #8B4513 (RGB: 139, 69, 19)

Accent Colors:
  White:         #FFFFFF (RGB: 255, 255, 255)
  Dark Gray:     #2C3E50 (RGB: 44, 62, 80)
```

### Camel Design Details

**Shape**: Simplified silhouette for recognition at small sizes
- Profile facing **right** (walking/running direction)
- Head raised, ears prominent
- Hump clearly defined (iconic camel feature)
- Four legs visible or suggested
- Tail visible or suggested for motion

**Posture**: Active stance (suggest movement/speed)
- Slightly forward lean
- Front legs extended
- Ready to run

**Simplification Level**: Medium detail
- NOT photorealistic
- NOT overly abstract (must be recognizable as a camel)
- Pixel art or flat geometric style (matches game aesthetic)

### Background & Landscape

**Primary Background**:
- **Pyramid** (Egyptian icon): Large, centered behind camel, or in corner
- **Desert Dunes**: Rolling sand dunes suggesting scale
- **Sunset Gradient**: Warm gradient from gold at bottom to blue at top

**Alternative Backgrounds**:
- Option A: Camel with sun/pyramid silhouette
- Option B: Camel with dunes and oasis
- Option C: Camel within circular badge with desert border

### Typography (if included)

**Game Title Integration**:
- Placement: Below camel or incorporated into design
- Text: "Desert Dash" or "Camel Runner" (avoid cluttering)
- Font Style: **Bold, sans-serif** (readable at small sizes)
- Options:
  - Simple text below camel
  - Curved text along bottom edge
  - Minimal acronym (e.g., "DD" in corner) for smallest sizes

**Size**: Ensure text remains legible at 32×32 px (smallest deployment size)

---

## Technical Specifications

### Image Properties

#### PNG Specifications
- **Color Mode**: RGBA (with Alpha/Transparency)
- **Bit Depth**: 32-bit (8-bit per channel)
- **Color Space**: sRGB
- **Compression**: PNG-8 or PNG-32 (lossless compression)
- **Transparency**: 
  - Main icon: **NO transparency** for Google Play / Apple App Store (must be solid background)
  - Adaptive icon foreground: **MUST have transparency** (alpha channel)
- **File Size**: Keep under 500 KB per file (larger files slow down distribution)

#### Safe Zones & Padding
- **Center Safe Zone**: Ensure critical elements (camel face, title) are within center 66% of icon
- **Padding**: Leave ~10–15 px padding from edges to avoid clipping
- **Adaptive Icon**: Critical elements must fit within center 66 px of 108×108 px canvas

### No Transparency Rule (App Store / Google Play)
- Final app icon **cannot have transparent background** (appears as white/black in stores)
- **Solution**: Add solid background color (use sand/gold color) OR create gradient
- **Exception**: Adaptive icon **foreground layer MUST have transparency**

### DPI & Resolution
- **DPI**: Provide at 1× scale (96 DPI standard)
- **Scaling**: Designs should look good at both 1× and 2× scales

---

## Design Variations

### Variant 1: Minimalist Silhouette
```
[ Camel silhouette + Pyramid + Sunset gradient ]
- Most recognizable at small sizes
- Clean and modern
- Best for variety of backgrounds
```

### Variant 2: Detailed with Landscape
```
[ Camel + Pyramid + Dunes + Oasis + Sky gradient ]
- More complex, requires testing at small sizes
- Rich visual storytelling
- Risk: May be too busy at 32×32 px
```

### Variant 3: Badge/Circle Design
```
[ Camel centered in circular badge with desert border ]
- Contained, balanced composition
- Works well on all backgrounds
- Easier for Android adaptive icons
```

### Variant 4: Pixel Art Style
```
[ Retro pixel-art camel + desert elements ]
- Matches game's art direction
- Nostalgic, distinctive
- More time-intensive to create at multiple scales
```

**Recommendation**: Start with **Variant 1 or 3** for maximum versatility.

---

## Export Checklist

Before finalizing, verify:

- [ ] **Main Icon (512×512 Google Play)**
  - [ ] Square aspect ratio (1:1)
  - [ ] No transparency (solid background)
  - [ ] Camel, pyramid, and colors clearly visible
  - [ ] Text (if any) legible at 1/4 size (128×128)
  - [ ] Exported as PNG-32

- [ ] **Apple Icon (1024×1024)**
  - [ ] Square aspect ratio (1:1)
  - [ ] No transparency (solid background)
  - [ ] High resolution, sharp details
  - [ ] Exported as PNG-32
  - [ ] Matches Google Play version in design intent

- [ ] **Adaptive Icon Foreground (108×108)**
  - [ ] Camel + title or badge on **transparent background**
  - [ ] Critical elements within center 66×66 px safe zone
  - [ ] Exported as PNG-32 with alpha channel

- [ ] **Adaptive Icon Background (108×108)**
  - [ ] Solid color (sand/gold) or gradient
  - [ ] Square format
  - [ ] Exported as PNG-32

- [ ] **General**
  - [ ] All files are PNG format, RGBA color
  - [ ] File sizes under 500 KB each
  - [ ] No spelling errors in title
  - [ ] Colors match approved palette
  - [ ] Camel is recognizable as a camel
  - [ ] Design is original and not copyrighted

---

## Generation Instructions (for AI Tools)

If using AI (DALL-E, Midjourney, Adobe Firefly, etc.):

**Prompt Template**:

```
Create a mobile game app icon for "Desert Dash: Camel Runner" featuring:

- A camel in profile, facing right, with proud posture and visible hump
- A large Egyptian pyramid in the background
- Rolling desert sand dunes
- A warm sunset gradient from gold/orange at bottom to sky blue at top
- The camel silhouetted against the sun or pyramid
- Color palette: gold (#FFD700), orange (#FF8C00), sand (#EDC9AF), sky blue (#87CEEB)
- Minimalist, flat design style (not photorealistic)
- Game title "Desert Dash" subtly integrated (below camel or at bottom)
- Centered composition, balanced, no clutter
- Must be instantly recognizable at small sizes (down to 32×32 px)
- Square format (512×512 px, can upscale to 1024×1024 for Apple)
- No transparency, solid background
- Suitable for all ages, culturally respectful to Egyptian/MENA audiences

Style: Modern flat design with pixel-art influences, reminiscent of casual mobile games.
```

**Post-Processing**:
1. Generate at 512×512 px or higher
2. Ensure background is solid (no transparency in final version)
3. Test the preview at different sizes (drag to corner)
4. Export as PNG-32

---

## Design Reference Images (Mood Board)

Look at these apps for design inspiration:

| App | Reason | Feature to Emulate |
|-----|--------|-------------------|
| **Alto's Adventure** | Minimalist mountain scene with character | Simple silhouette, effective landscape |
| **Threes!** | Bold, colorful, tile-based | Strong color use, clear at small sizes |
| **Mini Metro** | Geometric, clean design | Simplicity and balance |
| **Dune!** | Desert theme | Warm color palette, sand aesthetic |
| **Crossy Road** | Pixel art, casual style | Character-focused, blocky shapes |

---

## File Delivery

### Folder Structure (Expected)

```
docs/launch/app_icons/
├── desert_dash_icon_512x512.png          (Google Play Main)
├── desert_dash_icon_1024x1024.png        (Apple App Store)
├── desert_dash_adaptive_foreground.png   (Android Adaptive, transparent)
├── desert_dash_adaptive_background.png   (Android Adaptive, solid)
├── desert_dash_feature_graphic.png       (Google Play Feature, 1024×500)
├── desert_dash_icon_favicon.png          (Website, 32×32)
└── ICON_NOTES.txt                        (Designer notes & variations)
```

### Naming Convention

```
desert_dash_icon_[purpose]_[size]x[size].png

Examples:
- desert_dash_icon_main_512x512.png
- desert_dash_icon_apple_1024x1024.png
- desert_dash_icon_adaptive_fg_108x108.png
```

---

## Approval Process

1. **Designer submits** initial icon concepts (2–3 variations)
2. **Monetization Manager reviews** for brand alignment, game theme fit
3. **Game Designer reviews** for consistency with art direction
4. **Test at small sizes** (use browser zoom or screenshot resizer)
5. **Finalize and export** all required sizes
6. **Store submission** includes verified icon files

---

## After Launch: Updates & Variations

Future icon versions can include:
- **Seasonal variants**: Ramadan-themed, holiday-themed icons
- **Seasonal skins**: Feature different camel skins (lion, tiger, etc.)
- **Event promotions**: Special event badges or overlays
- **Regional variants**: Localized elements for different markets

For each variant, maintain the same specifications and technical requirements.

---

## Troubleshooting

### Icon looks blurry at small sizes
- Reduce fine details and complexity
- Ensure high contrast between camel and background
- Avoid thin lines (< 2 px at final size)

### Text is unreadable in the app drawer
- Remove text or use large, bold letters only
- Test at 32×32 px (actual app drawer size)
- Consider using a separate text layer only for 512×512+ versions

### Colors look different on different devices
- Use named colors (sRGB) instead of custom gradients
- Verify colors in a color picker (input hex values)
- Test on multiple phones if possible

### Adaptive icon looks clipped
- Ensure all critical elements are within the 66×66 px safe zone
- The system may apply circular, rounded, or custom masks
- Design foreground as if it will be masked to a circle

### File is too large or won't upload
- Compress PNG files (use tool like TinyPNG)
- Verify file is 32-bit PNG, not a different format
- Ensure no embedded metadata is inflating file size

---

## References & Tools

### Design Tools
- **Figma**: Free, web-based, great for exporting multiple sizes
- **Adobe XD**: Professional design tool
- **Photoshop**: Full control, but steeper learning curve
- **GIMP**: Free alternative to Photoshop

### AI Image Generation
- **DALL-E 3**: High quality, good camel rendering
- **Midjourney**: Artistic styles, consistent results
- **Adobe Firefly**: Integrated with Adobe tools
- **Stable Diffusion**: Open-source, local generation possible

### Optimization & Testing
- **TinyPNG**: Compress PNG files without quality loss
- **ImageOptim** (Mac) or **PNGGauntlet** (Windows): Batch compression
- **Icon Preview Tools**: https://www.figma.com/ or browser zoom tests

### Color Tools
- **Coolors.co**: Generate color palettes
- **Contrast Checker**: https://webaim.org/resources/contrastchecker/
- **Color Names**: https://chir.cat/projects/ntc.js/ (for reference colors)

---

## Final Checklist

Before submission:

- [ ] Icon meets all size requirements (512×512, 1024×1024, etc.)
- [ ] Camel is recognizable and centered
- [ ] Desert/pyramid elements are visible
- [ ] Colors match the approved palette
- [ ] No transparency in main icons (PNG solid background)
- [ ] Adaptive icon foreground has transparency
- [ ] Text (if included) is legible at small sizes
- [ ] Files are PNG-32 format
- [ ] No spelling errors
- [ ] Design is original (not copyrighted)
- [ ] Files are organized and properly named
- [ ] Ready for Google Play and App Store submission

---

**Status**: ✅ Ready for Design / AI Generation

This specification is complete and ready to be passed to a designer or used directly with AI image generation tools.

