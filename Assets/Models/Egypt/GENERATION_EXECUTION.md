# Egypt Environment Assets — One-Click Generation

**Status:** Ready for immediate execution (5 minutes)
**Owner:** Lead Engineer or Build Team
**Blockers:** None

---

## Quick Start (Copy-Paste Instructions)

### Step 1: Generate All Assets (30 seconds)
In **Unity Editor**, go to:
```
Tools > Camel Runner > Complete Egypt Setup Workflow (All-In-One)
```

This runs three automated steps:
1. **ProceduralEgyptEnvironmentBuilder.BuildAll()** — Creates 16 asset prefabs
2. **PlaceholderSceneSetup.SetupScene()** — Wires ObjectPool with all tags
3. **EgyptSceneSetup.SetupEgyptScene()** — Configures lighting, landmarks, parallax

Wait for confirmation dialog saying "Setup complete."

### Step 2: Verify in Play Mode (2 minutes)
1. Press **Play** in Unity Editor
2. Verify in **Stats panel:**
   - FPS: ~60 (should be consistently 60+)
   - Memory: <100MB total
3. Verify gameplay:
   - Camel stands on sand road ✓
   - Obstacles spawn (Rock, Cactus, Pillar, Pyramid, Ruins) ✓
   - Background pyramids/obelisks scroll with parallax ✓
   - Can pick up collectibles ✓
4. Press **Space** to start game, play for 30 seconds
5. Press **Escape** to stop

### Step 3: Commit to Repo (2 minutes)
```bash
git add Assets/Prefabs/Environment/
git add Assets/Scripts/Editor/EgyptAssetGenerationHelper.cs
git commit -m "M2: Egypt environment assets generated

- All 16 procedural prefabs created (roads, landmarks, vegetation)
- Texture memory: 0MB (procedural, vertex colors only)
- Performance: 60 FPS verified
- Ready for QA playtesting

Co-Authored-By: Paperclip <noreply@paperclip.ing>"

git push origin main
```

---

## What Gets Generated

### Assets Directory
```
Assets/Prefabs/Environment/
├── Materials/
│   ├── Road_Sand
│   ├── Lane_Marking
│   ├── ObeliskColor
│   ├── PalmTrunkColor
│   ├── PalmGreen
│   ├── CactusGreen
│   ├── StoneGray
│   └── ...
├── Environment_Road_Straight.prefab
├── Environment_Road_Curved.prefab
├── Environment_SandDune_Left.prefab
├── Environment_SandDune_Right.prefab
├── Prop_Pyramid_Large.prefab
├── Prop_Pyramid_Medium.prefab
├── Prop_Pyramid_Small.prefab
├── Prop_Sphinx.prefab
├── Prop_Obelisk_Large.prefab
├── Prop_Obelisk_Medium.prefab
├── Prop_PalmTree_Tall.prefab
├── Prop_PalmTree_Short.prefab
├── Prop_Cactus_Tall.prefab
├── Prop_Cactus_Short.prefab
├── Prop_Rock_Large.prefab
└── Prop_Rock_Small.prefab
```

### What Each Asset Is
- **Roads:** Flat sand surface (6m wide, 10m long) with lane dividers
- **Dunes:** Wedge shapes for left/right lane edges
- **Pyramids:** Triangular monuments (3 scale variants for LOD)
- **Sphinx:** Reclining feline with human head
- **Obelisks:** Tall stone pillars with pointed tops
- **PalmTrees:** Stylized tree with thin trunk + dense foliage crown
- **Cacti:** Cylindrical/barrel cactus with optional arms
- **Rocks:** Boulders for obstacle/lane-edge use

All use procedural mesh API (no external 3D files) with vertex colors for styling.

---

## Troubleshooting

### "Menu item not found"
- Make sure you're running it in the right location: `Tools > Camel Runner`
- If menu doesn't exist, check that EgyptAssetGenerationHelper.cs is in Assets/Scripts/Editor/
- Refresh scripts: Assets > Reimport All

### "Assets already exist / version conflict"
- Delete `Assets/Prefabs/Environment/` folder
- Delete `Assets/Prefabs/Placeholder/` folder (if it exists)
- Re-run the one-click helper
- Say "Yes" to any confirmation dialogs

### "60 FPS not reached / game is slow"
- Check that all 16 prefabs were generated (check Assets/Prefabs/Environment/ has 16+ .prefab files)
- Check for warnings in Console (click on yellow triangle)
- Verify no other processes are running in background
- Close other applications to free CPU/memory

### "Obstacles don't spawn / gaps in road"
- Check Console for warnings about missing pool tags
- Verify EgyptSceneSetup completed successfully (should see log message)
- Check that ObstacleSpawner has all 5 Egypt tags assigned

---

## Fallback Paths (If Procedural Generation Fails)

### Option A: Manual Prefab Creation from KayKit
1. Follow Assets/Models/Egypt/KAYKIT_IMPORT_WORKFLOW.md
2. Takes 30-40 minutes
3. Results in equivalent assets with different visual style

### Option B: Wait for AI-Generated Assets
- If Meshy.ai API key provided: [AIG-39](/AIG/issues/AIG-39) through [AIG-42](/AIG/issues/AIG-42) can proceed
- Takes 2-3 hours but higher visual fidelity
- Can be layered on top of current procedural assets

---

## Definition of Done (AIG-8)

All of the following must be true:
- ✅ 16 prefabs generated in Assets/Prefabs/Environment/
- ✅ 60 FPS verified in Play mode
- ✅ Camel spawns and can move
- ✅ Obstacles spawn correctly
- ✅ Background parallax working
- ✅ Collectibles pickupable
- ✅ Committed to repo (git push main)

Once complete, mark AIG-8 as **done**.

---

## Supporting Scripts

- **EgyptAssetGenerationHelper.cs** — One-click orchestrator (NEW)
- **ProceduralEgyptEnvironmentBuilder.cs** — Mesh generation engine
- **PlaceholderSceneSetup.cs** — ObjectPool + scene wiring
- **EgyptSceneSetup.cs** — Lighting + parallax + landmarks

All located in Assets/Scripts/Editor/

---

## Questions?
Check the previous documentation:
- Assets/Models/Egypt/EGYPT_ASSETS_PIPELINE.md (original spec)
- Assets/Models/Egypt/KAYKIT_IMPORT_WORKFLOW.md (external asset fallback)
- Assets/Models/Egypt/UNITY_IMPORT_WORKFLOW.md (FBX import reference)
