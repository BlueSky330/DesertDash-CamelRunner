# Kenney.nl CC0 Asset Sourcing — M3 Characters Production

**Date**: 2026-05-07  
**Decision**: CEO directive to use CC0 assets instead of Meshy.ai  
**Timeline**: May 7-15 (production intake phase)

## Assets to Source

### 1. Animal Pack Deluxe
- **URL**: https://kenney.nl/assets/animal-pack-deluxe
- **Purpose**: Base camel mesh
- **Status**: [TODO] Download & extract
- **Format**: .blend, .fbx, .obj (we'll use .fbx)
- **Use**: Import to Blender, retexture to sandy tan, add accessories

### 2. Character Pack
- **URL**: https://kenney.nl/assets/character-pack
- **Purpose**: Base humanoid mesh for thieves
- **Status**: [TODO] Download & extract
- **Format**: .blend, .fbx, .obj (we'll use .fbx)
- **Use**: Import to Blender, create 4 thief variants via material tinting

## Workflow

### Phase 1: Asset Sourcing (Today - May 7)
1. [ ] Download Animal Pack Deluxe ZIP from Kenney.nl
2. [ ] Download Character Pack ZIP from Kenney.nl
3. [ ] Extract both to `Assets/Models/Kenney.nl/`
4. [ ] Verify camel mesh in Animal Pack
5. [ ] Verify character base in Character Pack
6. [ ] Document findings in this log

### Phase 2: Camel Import & Retexture (May 8-10)
1. [ ] Select best camel mesh from Animal Pack
2. [ ] Import to Blender
3. [ ] Retexture to sandy tan/brown (match design spec)
4. [ ] Rig using setup_camel_rig.py (or manual rig)
5. [ ] Add accessories (goggles, saddle, bridle) via sculpting/materials
6. [ ] Export FBX with rig & animations

### Phase 3: Camel Skins (May 10-12)
1. [ ] Create 4 skin material variants (Pharaoh, Racing, Mummy, Golden)
2. [ ] Each skin = material override on base mesh
3. [ ] No new geometry per skin

### Phase 4: Animations (May 13-15)
1. [ ] Create 7 animation clips in Unity Animator
2. [ ] Run, Jump, Slide, Lane switches, Hit reaction, Idle
3. [ ] Test on camel with PlayerController

### Phase 5: Thief Characters (May 13-15)
1. [ ] Import character base from Character Pack
2. [ ] Create 4 thief variants via material tinting
3. [ ] Desert Bandit, Ninja, Pirate, Shadow Thief

### Phase 6: Integration & Commit (May 15)
1. [ ] Wire camel to PlayerController
2. [ ] Wire thieves to ThiefSpawner
3. [ ] Test full gameplay loop
4. [ ] Commit to repo

## Asset Source URLs & Licenses

**Kenney.nl Assets**  
- License: CC0 1.0 Universal (public domain)
- Author: Kenney (https://kenney.nl)
- Attribution: Not required, but appreciated

Links:
- Animal Pack Deluxe: https://kenney.nl/assets/animal-pack-deluxe
- Character Pack: https://kenney.nl/assets/character-pack
- All Kenney assets: https://kenney.nl/assets

## Notes

- All Kenney assets are low-poly and mobile-friendly
- Tri counts typically < 500 per character (well under our 2,000 budget)
- Assets come in multiple formats; we'll use FBX for maximum compatibility
- Retexturing is key to match design spec and maintain Egyptian/MENA theme
