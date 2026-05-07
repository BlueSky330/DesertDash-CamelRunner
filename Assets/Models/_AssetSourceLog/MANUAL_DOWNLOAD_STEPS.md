# Manual Asset Download — Kenney.nl CC0 Packs

## Download Instructions

### Step 1: Animal Pack Deluxe
1. Open browser: https://kenney.nl/assets/animal-pack-deluxe
2. Click "Free Download" button (green)
3. Save ZIP to: `Assets/Models/Kenney.nl/animal-pack-deluxe.zip`
4. Extract to: `Assets/Models/Kenney.nl/AnimalPack/`

**What you'll get:**
- Multiple animal models (camel, pig, cow, etc.)
- Formats: .blend, .fbx, .obj
- We need the camel mesh → look for files named `camel*` or `animal_camel*`

### Step 2: Character Pack
1. Open browser: https://kenney.nl/assets/character-pack
2. Click "Free Download" button (green)
3. Save ZIP to: `Assets/Models/Kenney.nl/character-pack.zip`
4. Extract to: `Assets/Models/Kenney.nl/CharacterPack/`

**What you'll get:**
- Base humanoid character models
- Formats: .blend, .fbx, .obj
- We need basic character → look for `character_male*` or `character_humanoid*`

### Step 3: Verify Extraction
```bash
ls -la Assets/Models/Kenney.nl/AnimalPack/
ls -la Assets/Models/Kenney.nl/CharacterPack/
```

## Asset Selection Guide

### From Animal Pack Deluxe
Look for a camel model file. Kenney packs typically have:
- `camel.fbx` or `animal_camel.fbx`
- `camel.blend`
- Metadata about tri count (~100-300 tris typical)

**Select**: The FBX version for importing to Blender

### From Character Pack
Look for humanoid base character:
- `character_male.fbx` or `character.fbx`
- `character_male.blend`
- Multiple pose variations

**Select**: The T-pose variant (standing neutral)

## Verification Checklist

Once extracted, verify:
- [ ] `AnimalPack/` contains .fbx files
- [ ] `CharacterPack/` contains .fbx files
- [ ] At least 1 camel mesh identified
- [ ] At least 1 character mesh identified
- [ ] No missing dependencies
- [ ] File permissions readable (755+)

## Next Steps

After extraction, proceed to:
1. `Assets/Models/Camel/PHASE_1_EXECUTION.md` — Blender import + retexture
2. Review `Assets/Models/_AssetSourceLog/KENNEY_SOURCING.md` for full timeline

## CC0 License Info

All Kenney.nl assets are **CC0 1.0 Universal (Public Domain)**:
- Free to use, modify, redistribute
- No attribution required (but appreciated)
- Source: https://kenney.nl

---

**Last updated**: 2026-05-07  
**Status**: Manual download steps documented — awaiting asset extraction
