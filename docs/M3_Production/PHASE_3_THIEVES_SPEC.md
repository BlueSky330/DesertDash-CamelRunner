# Phase 3B: Thief Characters Specification

**Timeline**: May 26-Jun 4, 2026 (parallel with Phase 3A, but Thieves can start immediately)  
**Owner**: 3D Artist (Blender) — can be different artist than Skins  
**Dependency**: Phase 1 (for reference rigging approach only, no technical dependency)  
**Deliverable**: 4 thief character FBX files (Thief_Bandit.fbx, Thief_Ninja.fbx, Thief_Pirate.fbx, Thief_Shadow.fbx)

---

## Overview

Phase 3B creates 4 unique antagonist characters that appear as obstacles in-game. Unlike Camel skins, each Thief is a **completely new character model** with:
- Unique mesh and silhouette
- **Individual skeleton/rig** (NOT shared with Camel)
- Simplified rigs (8-10 bones vs. Camel's 15-20)
- Basic animations (idle walk, run, pursuit, hit reaction)

All thieves follow the **Crossy Road / Subway Surfers style** (chunky, readable silhouettes, exaggerated proportions) and must stay under 1,500 tris each for mobile performance.

---

## Thief 1: Desert Bandit

**Character Concept**: Hooded desert robber, mysterious, sandy desert aesthetic.

### Visual Specification

**Silhouette**:
- Hooded humanoid figure (~60% Camel scale)
- Covered face (only eyes visible)
- Loose robes/cloak draped over body
- Coin bag slung across chest
- Overall impression: "mysterious desert thief"

**Color Palette**:
- Primary: Sandy tan (RGB 210, 180, 140) — robes
- Secondary: Dark brown (RGB 101, 67, 33) — hood, accents
- Accent: Red rope (RGB 200, 50, 50) — tied around waist
- Eyes: Bright (RGB 255, 255, 255 with dark pupils)

**Proportions**:
- Head: Slightly oversized (cartoonish style)
- Body: Narrow, draped in cloth
- Legs: Medium-length, visible below robes
- Overall height: ~1.6-1.8x Camel height

### Mesh Construction

**Total Polygon Budget**: <1,500 tris

**Components**:

1. **Head & Face** (~150 tris)
   - Round head shape
   - Hooded head covering (not separate mesh, part of head geometry)
   - Eyes: 2 bright circles
   - Face coverage: mouth area covered by robes
   - Vertices: ~75-80

2. **Body/Torso** (~250 tris)
   - Central body cylinder/tapered shape
   - Loose robes draped around torso
   - Coin bag hanging on chest (mesh element, ~30 tris)
   - Vertices: ~125-140

3. **Arms** (~200 tris, both)
   - Thin, elongated arms (human-like)
   - Hands visible (simple hand shapes, ~20 tris each)
   - Draped sleeves from robes
   - Vertices: ~100-110 total

4. **Legs** (~300 tris, both)
   - Two legs (separate meshes for clarity)
   - Visible below robe hem
   - Feet: Simple flat shapes
   - Vertices: ~150-160 total

5. **Robes/Cloak** (~300 tris)
   - Draped cloth over shoulders and back
   - Billowing effect (dynamic geometry, not rigged)
   - Tapers toward ankles
   - Vertices: ~150-170

6. **Hood** (~100 tris)
   - Separate hood mesh covering head
   - Drapes down back
   - Creates mysterious silhouette
   - Vertices: ~50-60

**Total: ~1,300-1,400 tris** ✅ Within budget

### Rigging

**Skeleton Structure** (8 bones):
```
Armature (root)
├── Hips (pelvis)
│   ├── Spine (lower back)
│   │   ├── Chest (upper back)
│   │   │   ├── Head
│   │   │   ├── ArmL (shoulder to elbow)
│   │   │   │   └── ForearmL (forearm to wrist)
│   │   │   └── ArmR
│   │   │       └── ForearmR
│   ├── LegL (hip to knee)
│   │   └── ShinL (knee to ankle)
│   └── LegR
│       └── ShinR
```

**Weight Painting**:
- Hips: Root bone, full weight
- Spine/Chest: Blend with Hips (0.7/0.3)
- Head: Full weight to Head bone
- Arms: Full weight to respective arm bones
- Legs: Full weight to respective leg bones
- Robes: Blend Spine + Chest (0.6/0.4) for natural drape
- Hood: Blend Head + Spine (0.8/0.2)

**Pose Test**:
1. Rotate Hips 45° → body leans, looks natural
2. Move Head → head moves, hood follows
3. Rotate ArmL → arm swings, sleeve follows
4. Move LegL forward → leg extends, robe drapes

### Materials

1. **Robe Material**
   - Color: Sandy tan (RGB 210, 180, 140)
   - Texture: Cloth weave pattern
   - Metallic: 0.0
   - Roughness: 0.85

2. **Hood Material**
   - Color: Dark brown (RGB 101, 67, 33)
   - Texture: Cloth with shadows
   - Roughness: 0.9

3. **Coin Bag Material**
   - Color: Brown leather (RGB 139, 90, 43)
   - Texture: Leather grain
   - Metallic: 0.2 (slight reflection from gold coins inside)
   - Roughness: 0.4

4. **Eyes Material**
   - Color: Bright white (RGB 255, 255, 255) with black pupils
   - Metallic: 0.1 (slight shine)
   - Emissive: None

5. **Rope/Accent Material**
   - Color: Red (RGB 200, 50, 50)
   - Texture: Twine/rope pattern
   - Roughness: 0.8

### Animations (Basic Idle Kit)

**Timeline for Phase 3B**: Thieves use **simplified animations** (not full Phase 2 set). Create:

1. **Idle Stance** (loopable, 1.5s)
   - Standing pose, slight weight shift
   - Hands resting at sides
   - Head tilts occasionally
   - Loop seamlessly

2. **Walk/Patrol** (loopable, 1.0s)
   - Slow walking motion
   - Arm swing, slight head bob
   - Ready to chase

3. **Chase/Pursuit Run** (loopable, 0.6s)
   - Faster running animation
   - More aggressive arm movement
   - Leaning forward slightly

4. **Hit/Flinch Reaction** (non-looping, 0.5s)
   - Quick recoil backward
   - Head snap
   - Recovery to idle

### UV Layout

**Texture Atlas**: 512x512 (smaller than Camel, simpler character)

**Allocation**:
- Robes: ~250x250 pixels (cloth can tile)
- Head/Hood: ~150x150 pixels
- Legs: ~150x100 pixels
- Hands: ~100x100 pixels
- Coin bag + rope: ~100x100 pixels
- Padding/margins: Fill remaining space

### Rig Verification Checklist

- [ ] All bones assigned, no orphaned vertices
- [ ] Weight painting smooth (no hard edges between bones)
- [ ] Idle animation: figure stands upright, slight sway
- [ ] Chase animation: legs move, arms swing, looks energetic
- [ ] Hit animation: recoil is snappy, recovery is smooth
- [ ] Robes deform naturally during all animations
- [ ] No clipping of robe through legs
- [ ] Head rotates freely without body interference

### Export Checklist

- [ ] Total tri count < 1,500
- [ ] All materials assigned and visible
- [ ] Skeleton exported (8 bones)
- [ ] Animations embedded in FBX (idle, chase, hit)
- [ ] No IK handles or constraints (baked only)
- [ ] FBX version: 2020
- [ ] File name: `Thief_Bandit.fbx`

### Success Criteria

✅ Silhouette reads as "hooded desert thief"  
✅ < 1,500 tris  
✅ Cloth draping looks natural  
✅ Animations are smooth and readable  
✅ Eyes are expressive and visible  

---

## Thief 2: Ninja Thief

**Character Concept**: Sleek ninja assassin, dark and nimble.

### Visual Specification

**Silhouette**:
- Sleek black humanoid (~55% Camel scale)
- Masked face (only eyes visible)
- Form-fitting black outfit
- Throwing stars visible (on belt/hand)
- Katana or throwing star accent
- Overall impression: "swift, deadly ninja"

**Color Palette**:
- Primary: Matte black (RGB 20, 20, 20) — outfit
- Secondary: Dark grey (RGB 60, 60, 60) — shading, accents
- Accent: Metallic silver (RGB 200, 200, 200) — throwing stars
- Eyes: Red/orange (RGB 255, 100, 50) — ninja mask eyes, glowing

**Proportions**:
- Head: Smaller than Bandit (alert, quick)
- Body: Lean, athletic build
- Arms: Long and sinewy
- Overall height: ~1.5x Camel height

### Mesh Construction

**Total Polygon Budget**: <1,500 tris

**Components**:

1. **Head & Mask** (~120 tris)
   - Round head covered by ninja mask
   - Eyes: 2 orange/red circles with black pupils
   - Mask covers entire face
   - Vertices: ~60-70

2. **Body/Torso** (~250 tris)
   - Sleek torso, form-fitting suit
   - Chest piece (slightly raised geometry)
   - Belt around waist
   - Vertices: ~120-130

3. **Arms** (~180 tris, both)
   - Long, lean arms
   - Gloved hands
   - Throwing stars held in hand (or on belt)
   - Vertices: ~90-100

4. **Legs** (~280 tris, both)
   - Form-fitting leg suit
   - Visible feet/shoes
   - Slightly spread stance (ready position)
   - Vertices: ~140-150

5. **Throwing Stars** (~100 tris)
   - 3-4 star-shaped objects (on belt or in hand)
   - ~25 tris each, metallic
   - Vertices: ~50-60

6. **Cape/Scarf** (~150 tris)
   - Short ninja scarf around neck
   - Drapes down back dynamically
   - Creates movement effect
   - Vertices: ~75-80

**Total: ~1,080-1,200 tris** ✅ Within budget

### Rigging

**Skeleton Structure** (9 bones — slightly more complex for arm/throwing mechanics):
```
Armature (root)
├── Hips
│   ├── Spine
│   │   ├── Chest
│   │   │   ├── Head
│   │   │   ├── ArmL
│   │   │   │   ├── ForearmL
│   │   │   │   └── HandL (for throwing star aiming)
│   │   │   └── ArmR
│   │   │       ├── ForearmR
│   │   │       └── HandR
│   ├── LegL
│   │   └── ShinL
│   └── LegR
│       └── ShinR
```

**Weight Painting**:
- Full weight assignments per bone type
- Hands: Extra bone for throwing star aiming
- Cape: Blend Chest (70%) + Spine (30%)

### Materials

1. **Suit Material** (body)
   - Color: Black (RGB 10, 10, 10)
   - Texture: Smooth, sleek fabric
   - Metallic: 0.1
   - Roughness: 0.3 (slight sheen)

2. **Mask Material**
   - Color: Black (RGB 20, 20, 20)
   - Texture: Tight cloth
   - Roughness: 0.4

3. **Eyes Material**
   - Color: Red/orange (RGB 255, 100, 50)
   - Emissive: 0.5 (glowing effect)
   - Metallic: 0.0

4. **Throwing Stars Material**
   - Color: Metallic silver (RGB 200, 200, 200)
   - Metallic: 0.95
   - Roughness: 0.1 (sharp reflections)
   - Emissive: None

5. **Cape Material**
   - Color: Black (RGB 20, 20, 20)
   - Texture: Cloth with slight translucency
   - Metallic: 0.0
   - Roughness: 0.8

### Animations (Basic Idle Kit)

1. **Idle Stance** (loopable, 1.5s)
   - Ready fighting position (feet apart, slightly crouched)
   - Hands near body, ready to strike
   - Alert expression

2. **Dash/Sprint Run** (loopable, 0.5s)
   - Quick, nimble running
   - Arms pulled in (agile)
   - Fast leg movement

3. **Throw Animation** (non-looping, 0.4s)
   - Wind-up arm motion
   - Star leaves hand
   - Return to ready stance

4. **Hit/Flinch** (non-looping, 0.5s)
   - Quick backward flip
   - Recovery to ready stance
   - Stays combat-ready

### UV Layout

**Texture Atlas**: 512x512

**Allocation**:
- Suit: ~300x250 pixels
- Head/mask: ~150x150 pixels
- Legs: ~150x100 pixels
- Stars: ~100x100 pixels
- Cape: ~100x150 pixels

### Export Checklist

- [ ] Total tri count < 1,500
- [ ] All materials black and grey (cohesive look)
- [ ] Eyes glow with orange/red emissive
- [ ] Throwing stars metallic and visible
- [ ] Cape flows naturally
- [ ] 9 bones, all assigned
- [ ] Animations: idle, dash, throw, hit
- [ ] File name: `Thief_Ninja.fbx`

### Success Criteria

✅ Silhouette reads as "ninja assassin"  
✅ < 1,500 tris  
✅ Black suit is sleek and modern  
✅ Glowing eyes are intimidating  
✅ Throwing stars are prominent  

---

## Thief 3: Pirate

**Character Concept**: Classic swashbuckler pirate, adventurous antagonist.

### Visual Specification

**Silhouette**:
- Swaggering pirate humanoid (~65% Camel scale, largest thief)
- Pirate hat (tricorn or large brim)
- Eye patch visible
- Tattered coat/cape
- Cutlass sword visible
- Overall impression: "roguish, dangerous pirate"

**Color Palette**:
- Primary: Navy blue (RGB 0, 30, 100) — coat
- Secondary: Brown leather (RGB 139, 90, 43) — belt, boots
- Accent: Gold (RGB 255, 215, 0) — buckles, sword hilt
- Skin: Tan (RGB 210, 180, 140)
- Eyes: Menacing (RGB 50, 50, 50 with glint)

**Proportions**:
- Head: Average size
- Body: Robust, barrel-chested
- Arms: Strong, with cutlass
- Overall height: ~1.7x Camel height (largest thief)

### Mesh Construction

**Total Polygon Budget**: <1,600 tris

**Components**:

1. **Head & Face** (~150 tris)
   - Round head with weathered features
   - Eye patch (left eye covered)
   - Visible right eye (menacing)
   - Mouth: grin or snarl
   - Vertices: ~75-80

2. **Body/Torso** (~250 tris)
   - Barrel-chested torso
   - Tattered coat over chest (separate mesh element, ~80 tris)
   - Visible belt with large buckle (metal, ~30 tris)
   - Vertices: ~120-130

3. **Arms** (~200 tris, both)
   - Strong, muscular arms
   - Hands gripping cutlass
   - Rolled-up sleeves
   - Vertices: ~100-110

4. **Legs** (~250 tris, both)
   - Two legs in boots
   - Thick boots with buckles
   - Stance: slightly spread (confident)
   - Vertices: ~120-130

5. **Pirate Hat** (~100 tris)
   - Tricorn pirate hat with feather
   - Sits on head (separate mesh)
   - Gold skull emblem on front
   - Vertices: ~50-60

6. **Cutlass Sword** (~150 tris)
   - Curved pirate sword
   - Held in right hand
   - Gold hilt with detail
   - Metallic blade
   - Vertices: ~75-80

7. **Cape/Coat Tails** (~100 tris)
   - Flowing cape from coat
   - Tattered edges
   - Drapes naturally
   - Vertices: ~50-60

**Total: ~1,200-1,400 tris** ✅ Within budget

### Rigging

**Skeleton Structure** (9 bones):
```
Armature (root)
├── Hips
│   ├── Spine
│   │   ├── Chest
│   │   │   ├── Head
│   │   │   ├── ArmL
│   │   │   │   ├── ForearmL
│   │   │   │   └── HandL
│   │   │   └── ArmR
│   │   │       ├── ForearmR
│   │   │       └── HandR (holding sword)
│   ├── LegL
│   │   └── ShinL
│   └── LegR
│       └── ShinR
```

**Weight Painting**:
- Hat: Full weight to Head
- Sword: Rigged to HandR (swings with hand)
- Cape: Blend Chest (60%) + Spine (40%)
- Coat: Blend Chest (80%) + Spine (20%)

### Materials

1. **Coat Material**
   - Color: Navy blue (RGB 0, 30, 100)
   - Texture: Tattered cloth, worn
   - Metallic: 0.0
   - Roughness: 0.9

2. **Skin Material**
   - Color: Tan (RGB 210, 180, 140)
   - Texture: Weathered, sun-tanned
   - Roughness: 0.7

3. **Hat Material**
   - Color: Navy blue (RGB 0, 30, 100)
   - Texture: Felt with feather detail
   - Roughness: 0.85

4. **Belt & Buckles**
   - Color: Brown leather (RGB 139, 90, 43)
   - Buckle: Gold (RGB 255, 215, 0), metallic 0.8
   - Roughness: 0.3

5. **Sword Material**
   - Blade: Metallic steel (RGB 150, 150, 180), metallic 0.9, roughness 0.05
   - Hilt: Gold (RGB 255, 215, 0), metallic 0.8, roughness 0.2

### Animations (Basic Idle Kit)

1. **Idle Swagger** (loopable, 1.5s)
   - Standing with confidence
   - Slight sway, hand on hip
   - Sword hanging at side
   - Menacing expression

2. **Charge/Pursuit Run** (loopable, 0.6s)
   - Running with sword ready
   - Sword arm raised slightly
   - Aggressive leaning

3. **Sword Swing** (non-looping, 0.5s)
   - Raising sword
   - Swinging down/across
   - Return to ready

4. **Hit/Stumble** (non-looping, 0.6s)
   - Recoil from hit
   - Sword wavers
   - Recovery with renewed determination

### UV Layout

**Texture Atlas**: 512x512

**Allocation**:
- Coat: ~300x250 pixels (worn cloth can tile)
- Head/skin: ~150x150 pixels
- Legs: ~150x100 pixels
- Hat: ~100x100 pixels
- Sword/buckles: ~120x100 pixels
- Cape: ~100x150 pixels

### Export Checklist

- [ ] Total tri count < 1,600
- [ ] Navy blue coat is primary color
- [ ] Eye patch visible on left eye
- [ ] Sword is clear and metallic
- [ ] Gold buckles and hilt shine
- [ ] Hat is well-positioned
- [ ] Cape flows naturally
- [ ] 9 bones, all assigned
- [ ] Animations: swagger, charge, swing, hit
- [ ] File name: `Thief_Pirate.fbx`

### Success Criteria

✅ Silhouette reads as "classic pirate"  
✅ < 1,600 tris  
✅ Sword is prominent and impressive  
✅ Navy blue and gold colors are vibrant  
✅ Confident swagger in idle animation  

---

## Thief 4: Shadow Thief

**Character Concept**: Supernatural ghostly enemy, mysterious and eerie.

### Visual Specification

**Silhouette**:
- Semi-transparent ghostly humanoid (~60% Camel scale)
- Elongated, ethereal limbs
- Glowing yellow eyes
- Wispy/smoky body effect
- Overall impression: "magical, dangerous specter"

**Color Palette**:
- Primary: Dark translucent grey (RGB 80, 80, 100, Alpha 0.6)
- Secondary: Purple shadows (RGB 100, 50, 150, Alpha 0.5)
- Accent: Bright yellow eyes (RGB 255, 255, 100)
- Glow: Blue/purple aura (RGB 150, 100, 255)

**Proportions**:
- Head: Small and round
- Body: Elongated and wispy
- Limbs: Extra-long, stretchy
- Overall height: ~1.8x Camel height (tallest thief, stretched appearance)

### Mesh Construction

**Total Polygon Budget**: <1,500 tris

**Components**:

1. **Head** (~100 tris)
   - Simple round shape
   - Smooth, featureless
   - Eyes only visible feature (glowing)
   - Vertices: ~50-60

2. **Body/Torso** (~200 tris)
   - Elongated, tapered torso
   - Smoky appearance (use alpha transparency)
   - No detailed geometry (smooth silhouette)
   - Vertices: ~100-110

3. **Arms** (~200 tris, both)
   - Extra-long, stretchy arms
   - End in wispy fingers
   - Slight glow at edges
   - Vertices: ~100-110

4. **Legs** (~150 tris, both)
   - Elongated legs
   - Fade out toward feet (transparent)
   - No feet (disappears into mist)
   - Vertices: ~75-80

5. **Eyes** (~40 tris)
   - Two large glowing yellow orbs
   - Separate mesh (emissive)
   - Vertices: ~20-25

6. **Aura/Glow Effect** (~100 tris)
   - Surrounding glow mesh (separate)
   - Wireframe or particle-like
   - Creates "magical presence"
   - Vertices: ~50-60

**Total: ~790-920 tris** ✅ Well within budget (room for extra glow effects)

### Rigging

**Skeleton Structure** (8 bones, simplified):
```
Armature (root)
├── Hips
│   ├── Spine
│   │   ├── Chest
│   │   │   ├── Head
│   │   │   ├── ArmL
│   │   │   │   └── ForearmL
│   │   │   └── ArmR
│   │   │       └── ForearmR
│   ├── LegL
│   ├── LegR
```

**Weight Painting**:
- Head: Full weight to Head
- Arms: Full weight to arm bones
- Legs: Full weight to leg bones
- Body/Aura: Root bone (minimal deformation, mostly position-based)
- Eyes: Rigged to Head, always look forward

**Deformation Notes**:
- Keep body deformation subtle (ghostly doesn't move like solid objects)
- Arms stretch but body stays relatively fixed
- Aura glow follows body but lags slightly (ethereal effect)

### Materials

1. **Body Material**
   - Color: Dark grey/purple (RGB 80, 80, 100)
   - Alpha: 0.6 (semi-transparent)
   - Texture: Smooth gradient (no detail texture)
   - Metallic: 0.0
   - Roughness: 0.5
   - Special shader: Additive blend for ghostly look

2. **Shadow/Edge Material**
   - Color: Purple (RGB 100, 50, 150)
   - Alpha: 0.4 (more transparent)
   - Used for outline/edge definition
   - Shader: Additive glow

3. **Eyes Material**
   - Color: Bright yellow (RGB 255, 255, 100)
   - Emissive: 0.8 (strong glow)
   - Metallic: 0.0
   - Alpha: 1.0 (fully opaque)
   - Special: Bloom/glow shader for eye effect

4. **Aura Material**
   - Color: Blue/purple (RGB 150, 100, 255)
   - Emissive: 0.4 (subtle glow around body)
   - Alpha: 0.3 (very translucent)
   - Shader: Additive blend, creates ethereal halo
   - Procedural animation: Pulsing glow (optional, adds realism)

### Special Effects

**Ghostly Aura Animation** (Optional):
- Pulsing glow intensity (0.3-0.6 range)
- Frequency: 0.5 Hz (slow, eerie)
- Shader-based (not mesh animation)

**Transparency Fade**:
- Legs fade out toward feet (alpha gradient)
- Creates "fading into mist" effect
- Use vertex color or alpha gradient texture

### Animations (Basic Idle Kit)

1. **Idle Float** (loopable, 2.0s)
   - Hovering in place
   - Slight vertical bob
   - Arms floating at sides
   - Eerie, slow movement

2. **Glide/Drift Run** (loopable, 0.8s)
   - Floating forward smoothly
   - No leg movement (ghostly)
   - Arms trail slightly
   - Speed: slower than other thieves (ethereal)

3. **Lunge/Grab** (non-looping, 0.5s)
   - Sudden forward lunge
   - Arms extend outward
   - Scary attack pose
   - Quick and threatening

4. **Fade/Disappear** (non-looping, 0.4s)
   - Transparency increases
   - Shrinks slightly
   - Disappears after hit
   - Recovery: fade back in

### UV Layout

**Texture Atlas**: 256x256 (minimal, mostly procedural)

**Allocation**:
- Body gradient: ~150x150 pixels (gradients can be procedural)
- Eyes: ~50x50 pixels
- Aura glow: Procedural (no UV needed)
- Remaining: Padding

### Rig Verification Checklist

- [ ] All bones assigned, no orphaned vertices
- [ ] Head position correct, eyes look forward
- [ ] Arms stretch realistically (no clipping through body)
- [ ] Legs fade out at feet
- [ ] Body stays mostly rigid (ghostly behavior)
- [ ] Eyes glow clearly visible
- [ ] Aura glow surrounds entire figure
- [ ] Animations: idle float, glide, lunge, fade

### Export Checklist

- [ ] Total tri count < 1,500 (expect ~800-900)
- [ ] All transparency materials working
- [ ] Eyes emissive glow bright and visible
- [ ] Aura surrounds body completely
- [ ] Ghostly appearance is clear (not solid/opaque)
- [ ] 8 bones, all assigned
- [ ] Animations: float, glide, lunge, fade
- [ ] Special materials: transparency and emissive working
- [ ] File name: `Thief_Shadow.fbx`

### Success Criteria

✅ Silhouette reads as "supernatural specter"  
✅ < 1,500 tris  
✅ Transparency makes character feel ethereal  
✅ Eyes glow menacingly  
✅ Aura effect adds magical atmosphere  

---

## Phase 3B Summary & Delivery

### Total Deliverables

| Thief | File | Tri Count | Key Features |
|-------|------|-----------|--------------|
| Desert Bandit | Thief_Bandit.fbx | <1,500 | Hooded, robes, coin bag |
| Ninja | Thief_Ninja.fbx | <1,500 | Black suit, throwing stars, glowing eyes |
| Pirate | Thief_Pirate.fbx | <1,600 | Navy coat, hat, cutlass sword, gold accents |
| Shadow Thief | Thief_Shadow.fbx | <1,500 | Transparent ghostly, glowing eyes, aura glow |

### Timeline (Parallel with Phase 3A Skins)

- **Day 1-2** (May 26-27): Model Bandit + Ninja thieves
- **Day 3-4** (May 28-29): Model Pirate + Shadow thief
- **Day 5** (May 30-31): Rig testing, animation creation, polish
- **Day 6** (Jun 1): Final export and verification
- **Jun 2-4**: Buffer for adjustments, quality pass

### Quality Gate

All 4 thieves must:
- ✅ Be visually distinct from each other
- ✅ Be under tri count limits (1,500-1,600 each)
- ✅ Have smooth rigging and weight painting
- ✅ Have basic animations (idle, pursuit, reaction)
- ✅ Have all materials rendering correctly
- ✅ Be readable silhouettes (Crossy Road style)

### Hand-Off Criteria

When Phase 3B is complete:
- All 4 Thief FBX files committed to git
- Each character fully rigged and animated
- All materials and special effects working
- Tri counts verified
- Ready for Phase 4 (QA & Final Export)

### Design Diversity

The 4 thieves represent different gameplay themes:
- **Bandit**: Desert/survival theme
- **Ninja**: Speed/agility theme
- **Pirate**: Adventure/treasure theme
- **Shadow**: Supernatural/mystery theme

Each is instantly recognizable and fits the Egyptian/MENA world of Camel Runner while offering visual variety for players.
