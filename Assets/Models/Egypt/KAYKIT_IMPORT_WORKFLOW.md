# KayKit Desert Kit — CEO Directive Import Workflow

**Status:** M2 Egypt Level Assets  
**Timeline:** Immediate (unblock QA)  
**Directive:** Use free KayKit Desert Kit to replace placeholders with real low-poly art

---

## Quick Start (5 minutes)

### Step 1: Download KayKit Desert Kit
1. Go to Unity Asset Store: https://assetstore.unity.com/
2. Search: **"KayKit Desert"**
3. Click: **KayKit Desert Kit** (by Kay Lousberg)
4. Click blue **"Add to My Assets"** button
5. In Unity Editor: Window > Asset Store > Downloads
6. Click **"Import"** on KayKit Desert Kit

### Step 2: Accept Import Dialog
- Accept default import settings
- **DO NOT** uncheck any assets — import everything
- Click **"Import"** button
- Wait 30–60 seconds for import to complete

### Step 3: Organize KayKit Assets
After import completes, KayKit assets will be in `Assets/KayKit_Desert_Course/`.

```
Assets/KayKit_Desert_Course/
├── Models/
│   ├── Cactus/ (short, tall, cluster variants)
│   ├── Dune/ (left, right, background)
│   ├── Obelisk/ (small, medium, large)
│   ├── Palm/ (multiple variants)
│   ├── Pyramid/ (multiple LOD variants)
│   ├── Rock/ (boulders, edge stones)
│   ├── RuinWall/ (pillar, collapsed wall)
│   └── Road/ (if available — sand path section)
├── Materials/
│   ├── Cactus_Mat
│   ├── Desert_Sand_Mat
│   ├── Rock_Mat
│   ├── Palm_Mat
│   └── ... (others)
└── Textures/
    ├── Cactus_Color
    ├── Desert_Sand_Color
    ├── Rock_Color
    └── ... (others)
```

---

## Prefab Creation Workflow (15–20 minutes)

### Assets to Create as Prefabs

Use the structure below. Each prefab goes into `Assets/Prefabs/{Category}/` and is wired into the pools.

#### 1. Road Chunks
**Source:** KayKit Desert Kit → Models/Road/ (if available) or create by placing sand dune edges

- `Assets/Prefabs/Placeholder/RoadChunk_Straight.prefab`
  - Parent: KayKit Dune (left + right) + sand plane
  - Add Collider (convex) for player ground check
  - Tag: `RoadChunk_Straight`
  
- `Assets/Prefabs/Placeholder/RoadChunk_Curved.prefab`
  - Parent: KayKit Dune (curved placement) + sand plane
  - Slightly curved path
  - Tag: `RoadChunk_Curved`

#### 2. Obstacles

- `Assets/Prefabs/Placeholder/Obstacle_Rock.prefab`
  - Source: KayKit Rock → Models/Rock/Rock_Large
  - Add Collider (convex)
  - Add Tag: `Obstacle_Rock`

- `Assets/Prefabs/Placeholder/Obstacle_Cactus.prefab`
  - Source: KayKit Cactus → Models/Cactus/Cactus_Tall
  - Add Collider (convex)
  - Tag: `Obstacle_Cactus`

- `Assets/Prefabs/Placeholder/Obstacle_Pillar.prefab`
  - Source: KayKit RuinWall → Models/RuinWall/Pillar
  - Tag: `Obstacle_Pillar`

- `Assets/Prefabs/Placeholder/Obstacle_Pyramid.prefab`
  - Source: KayKit Pyramid → Models/Pyramid/Pyramid_Small (foreground LOD)
  - Tag: `Obstacle_Pyramid`

- `Assets/Prefabs/Placeholder/Obstacle_Ruins.prefab`
  - Source: KayKit RuinWall → Models/RuinWall/Wall_Collapsed
  - Tag: `Obstacle_Ruins`

#### 3. Collectibles (can reuse placeholder cubes with KayKit colors for now)
- `Collectible_Date`, `Collectible_Coin`, `Collectible_Gem`, etc. — keep as simple colored cubes with KayKit material colors

#### 4. Player
- `Camel_Placeholder` — keep as-is (Art will provide camel model later in M3)

#### 5. Props (Background Parallax)

- `Assets/Prefabs/Placeholder/Prop_Pyramid.prefab`
  - Source: KayKit Pyramid → Models/Pyramid/ (use medium/large variant)
  - Remove Collider (background prop, no collision)
  - Recolor if needed (match warm sand color)

- `Assets/Prefabs/Placeholder/Prop_Obelisk.prefab`
  - Source: KayKit Obelisk → Models/Obelisk/Obelisk_Medium
  - Remove Collider
  - Recolor with gold accents

- `Assets/Prefabs/Placeholder/Prop_SkyPlane.prefab`
  - Keep existing sky plane (or create new with sky-blue quad)

---

## Recoloring for Egypt Palette (10 minutes)

KayKit assets come in default tan/brown. Adapt them to match the Egypt color scheme:

### Target Colors
- **Sand Beige:** `#D4A574` (warm, light)
- **Warm Yellow:** `#FFC93C` (highlights, lane markers)
- **Gold Accents:** `#FFD700` (decorative)
- **Terracotta:** `#E2725B` (stone, pillars)
- **Palm Green:** `#66BB6A` (foliage)
- **Oasis Blue:** `#0088CC` (water, sky)

### How to Recolor
1. Select KayKit material in Assets/KayKit_Desert_Course/Materials/
2. In Inspector, adjust Base Color (Albedo) slider to target color
3. Increase Emission slightly for golden-hour feel
4. Save material
5. Assign to KayKit prefab in scene

Alternative: Create new materials in Assets/Materials/Egypt/ and assign them to KayKit prefabs.

---

## Integration Steps

### Step 1: Create Prefabs from KayKit Models
1. Drag KayKit model (e.g., `Cactus_Tall`) into Hierarchy
2. Adjust position/scale to game dimensions
3. Add Collider (Box or Capsule, match model)
4. Right-click in Hierarchy → Prefabs → Create Prefab
5. Save to `Assets/Prefabs/Placeholder/` with correct name
6. Delete from scene (it's now in prefab)
7. **Repeat for all 5 obstacle types + road chunks + props**

### Step 2: Wire ObjectPool
1. Open `Assets/Scripts/Editor/PlaceholderSceneSetup.cs`
2. Verify PoolConfig array includes all prefab tags (already configured)
3. Run: Tools > Camel Runner > Setup Placeholder Scene
4. Verify all pools are created in the ObjectPool component

### Step 3: Run Egypt Scene Setup
1. Open your Gameplay scene in Unity Editor
2. Run: Tools > Camel Runner > Setup Egypt Level Scene
3. Verify:
   - Lighting applied (warm golden-hour)
   - Background parallax props placed
   - Chunk weights set to 3:1 straight-to-curved
   - ObstacleSpawner tags updated

### Step 4: Test in Play Mode
1. Press Play in Unity Editor
2. Verify:
   - 60 FPS (check Stats panel)
   - Camel stands on sand road
   - Obstacles spawn (Rock, Cactus, Pillar, Pyramid, Ruins)
   - Background pyramids/obelisks scroll (parallax visible)
   - Collectibles spawn and are pickupable
3. Press Space to start game
4. Play for 30 seconds, verify gameplay feel

---

## Fallback: Kenney Assets
If KayKit is not available on the Asset Store:

1. Download **Kenney "Nature Kit"** from kenney.nl (free, CC0)
2. Download **Kenney "Modular Buildings"** for structure blocks
3. Use Cactus, Palm, Rock from Nature Kit
4. Use simple block geometry from Modular Buildings for road/pyramid
5. Same workflow as above (prefab creation → recoloring → scene setup)

---

## Commits & Timeline

Once prefabs are created and scene setup is complete:

```bash
git add Assets/Prefabs/Placeholder/*.prefab
git add Assets/Materials/Egypt/
git add Assets/KayKit_Desert_Course/  (if committing full KayKit)
git commit -m "M2: KayKit integration — Egypt level playable with real low-poly art

- Imported KayKit Desert Kit (free tier)
- Created prefabs for road chunks, obstacles, props
- Recolored to Egypt palette (warm gold, sand beige, terracotta)
- PlaceholderSceneSetup + EgyptSceneSetup wired
- Egypt level playable at 60 FPS
- Ready for QA playtesting

Co-Authored-By: Paperclip <noreply@paperclip.ing>"
```

**Timeline Goal:** Complete by end of business today → Unblock QA for Egypt level testing.

---

## Next Phase: AI-Generated Overlay (Future)

When BM provides Meshy.ai API key:
- AIG-39 through AIG-42 (AI-gen tasks) can proceed in parallel
- Real AI-generated Egypt assets can be created and overlaid
- KayKit serves as fallback if AI generation delays
- **No rework needed** — AI assets will replace KayKit in prefabs incrementally

---

## Questions?
- KayKit not showing up? Check Windows > Asset Store > Downloads
- Prefab won't save? Ensure `Assets/Prefabs/Placeholder/` directory exists
- Materials look wrong? Adjust Base Color + Emission in Material Inspector
- Obstacles not spawning? Verify PoolConfig tag names match prefab tags exactly
