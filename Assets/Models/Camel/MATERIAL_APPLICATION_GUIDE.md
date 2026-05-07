# Camel Skin Materials — Application Guide

**Task:** AIG-78 — AIG-9.2: Create 4 Camel Skin Material Variants  
**Date Prepared:** 2026-05-07  
**Status:** Ready for FBX Import

---

## Quick Start (5-10 minutes)

Once `Camel_Default.fbx` is imported into the Unity project:

1. **Create Materials (Auto)**
   - Open Unity Editor
   - In Project window, right-click → Select `Assets > Create Camel Materials > Setup All Skins`
   - This creates all 16 material files in `Assets/Materials/Camel/Skins/`

2. **Import Camel FBX**
   - Import `Camel_Default.fbx` to `Assets/Models/Camel/`
   - Select in Project → Inspector → Materials import settings
   - Ensure "Import Materials" is **enabled**

3. **Assign Materials to Mesh**
   - Select Camel in Hierarchy
   - In Inspector, find SkinnedMeshRenderer
   - Drag/drop `Camel_Pharaoh.mat` to the Material slot
   - Test other variants by swapping materials

4. **Validate & Test**
   - Play scene and verify colors match concept art
   - Check metallic shine on Pharaoh and Golden skins
   - Verify Mummy eye glow is visible

---

## Detailed Workflow

### Step 1: Generate Material Assets

**Location:** `Assets > Create Camel Materials > Setup All Skins` (Editor menu)

**What it does:**
- Creates 16 material files total:
  - 4 main materials (one per variant)
  - 12 sub-materials (3-5 per main variant for different body parts)

**Output Files:**
```
Assets/Materials/Camel/Skins/
├── Camel_Pharaoh.mat              (tan body base)
├── Camel_Pharaoh_Gold.mat         (gold collar & bands)
├── Camel_Pharaoh_GoldGem.mat      (sapphire gem)
├── Camel_Pharaoh_Blue.mat         (blue headdress stripes)
│
├── Camel_Racing.mat               (tan body base)
├── Camel_Racing_Black.mat         (goggles & harness)
├── Camel_Racing_Yellow.mat        (visor & stripes)
│
├── Camel_Mummy.mat                (white linen body)
├── Camel_Mummy_Shadow.mat         (shadow wraps)
├── Camel_Mummy_Eyes.mat           (glowing yellow eyes)
├── Camel_Mummy_Tattered.mat       (tattered wrap edges)
│
├── Camel_Golden.mat               (bright gold body)
├── Camel_Golden_ShadowGold.mat    (deep gold shadows)
├── Camel_Golden_Ruby.mat          (red gem)
├── Camel_Golden_Emerald.mat       (green gem)
├── Camel_Golden_Sapphire.mat      (blue gem)
└── Camel_Golden_Pattern.mat       (geometric patterns)
```

### Step 2: Import Camel FBX

1. Obtain `Camel_Default.fbx` from Kenney.nl or asset source
2. Place in: `Assets/Models/Camel/`
3. In Unity Inspector (with FBX selected):
   - Materials → **Import Materials: ✓ Enabled**
   - Rig → Avatar Definition: **Humanoid** (or custom)
   - Skinned Mesh Renderer → **Enabled**
4. Click Apply
5. Drag into scene or create prefab

### Step 3: Mesh Subdivision Setup (if needed)

If the camel mesh has separate geometry for accessories (collar, goggles, gems, wraps):

1. **Split sub-meshes in Blender** (before FBX export):
   - Separate collar geometry → different material slot
   - Separate headdress → different material slot
   - Separate gems → different material slot
   - etc.

2. **Or apply per-face** in Unity:
   - If FBX imports as single mesh, use Material Slots to assign multiple materials
   - Assign different materials to sub-meshes in Inspector

3. **Recommended approach:** Split in Blender before export for cleaner material management

### Step 4: Create Prefab Variants

```
Assets/Prefabs/Camel/
├── Camel_Default.prefab
│   └── Materials: Camel_Pharaoh (+ sub-materials)
├── Camel_Pharaoh.prefab
│   └── Variant of Default with Pharaoh skin
├── Camel_Racing.prefab
│   └── Variant of Default with Racing skin
├── Camel_Mummy.prefab
│   └── Variant of Default with Mummy skin
└── Camel_Golden.prefab
    └── Variant of Default with Golden skin
```

**Creation Steps:**
1. Create `Camel_Default.prefab` from Camel in scene
2. Right-click → Create → Prefab Variant
3. Rename to `Camel_Pharaoh.prefab`
4. In the variant inspector, override the SkinnedMeshRenderer material
5. Repeat for Racing, Mummy, Golden variants

### Step 5: Apply CamelSkinManager Script

1. Select Camel in Hierarchy
2. Add Component → CamelSkinManager
3. In Inspector, configure:
   - **Mesh Renderer:** Assign the SkinnedMeshRenderer component
   - **Skin Materials [4]:** 
     - [0] = `Camel_Pharaoh.mat`
     - [1] = `Camel_Racing.mat`
     - [2] = `Camel_Mummy.mat`
     - [3] = `Camel_Golden.mat`
4. Save prefab

### Step 6: Test Each Variant

**Editor Testing:**
```csharp
// Quick test in Play mode:
// Press 1/2/3/4 to cycle skins
// Or use CamelSkinManager.ApplySkin(variant)

public class TestCamelSkins : MonoBehaviour
{
    private CamelSkinManager skinManager;

    private void Start()
    {
        skinManager = GetComponent<CamelSkinManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) skinManager.CycleNextSkin();
        if (Input.GetKeyDown(KeyCode.Q)) skinManager.CyclePreviousSkin();
    }
}
```

**Visual Validation Checklist:**
- [ ] Pharaoh: Gold and blue stripes visible, sapphire gem shows metallic shine
- [ ] Racing: Yellow/black color scheme clear, goggles and harness distinct
- [ ] Mummy: White wraps obvious, yellow eyes glow visible even in bright scenes
- [ ] Golden: Full metallic gold appearance, gems (ruby/emerald/sapphire) show color
- [ ] All skins: Colors match concept art, no texture stretching or seams

### Step 7: Runtime Skin Selection

**For Game Logic:**
```csharp
// Example: Spawn camel with specific skin
public GameObject SpawnCamelWithSkin(CamelSkinManager.CamelSkinVariant variant)
{
    GameObject prefab = variant switch
    {
        CamelSkinManager.CamelSkinVariant.Pharaoh => Resources.Load<GameObject>("Prefabs/Camel/Camel_Pharaoh"),
        CamelSkinManager.CamelSkinVariant.Racing => Resources.Load<GameObject>("Prefabs/Camel/Camel_Racing"),
        CamelSkinManager.CamelSkinVariant.Mummy => Resources.Load<GameObject>("Prefabs/Camel/Camel_Mummy"),
        CamelSkinManager.CamelSkinVariant.Golden => Resources.Load<GameObject>("Prefabs/Camel/Camel_Golden"),
        _ => Resources.Load<GameObject>("Prefabs/Camel/Camel_Default")
    };

    GameObject instance = Instantiate(prefab);
    return instance;
}
```

---

## Troubleshooting

### Materials Look Flat/Dull
- **Cause:** Metallic/Smoothness values too low
- **Fix:** Increase _Metallic (0.8-1.0) and _Glossiness (0.7-0.9) for shiny surfaces
- **Verify:** In Scene view with lighting, gems and gold should show specular highlights

### Mummy Eyes Don't Glow
- **Cause:** Emission not enabled on material
- **Fix:** 
  - Select `Camel_Mummy_Eyes.mat`
  - Inspector → Emission checkbox: **✓ Enabled**
  - Set Emission Color: `#FFFF00` at full brightness
  - If still dark, enable HDR and Bloom post-processing in camera

### Colors Don't Match Concept Art
- **Cause:** Incorrect hex color conversion or material color override
- **Fix:**
  - Open material in Inspector
  - Find `_Color` property
  - Compare to CAMEL_SKIN_MATERIALS_SPEC.md hex values
  - Adjust manually if auto-generation had issues

### Sub-Meshes Showing Wrong Color
- **Cause:** FBX imported with wrong material assignment
- **Fix:**
  1. Check FBX has correct sub-mesh structure in Blender
  2. In Unity Inspector (FBX selected) → Materials → Import Materials: ON
  3. Manually assign correct sub-material to each slot

### Performance Issues
- **Cause:** Too many material variants or textures
- **Fix:**
  - Batch materials where possible
  - Use atlased textures if adding detailed maps later
  - Current spec uses only vertex colors + standard shader (optimal)

---

## Integration with Game Systems

### Skin Unlock System
```csharp
public class CamelSkinUnlocks
{
    public bool IsSkinUnlocked(CamelSkinManager.CamelSkinVariant skin)
    {
        return PlayerPrefs.GetInt($"skin_unlocked_{skin}", 
                                (skin == CamelSkinManager.CamelSkinVariant.Pharaoh ? 1 : 0)) == 1;
    }

    public void UnlockSkin(CamelSkinManager.CamelSkinVariant skin)
    {
        PlayerPrefs.SetInt($"skin_unlocked_{skin}", 1);
        PlayerPrefs.Save();
    }
}
```

### Skin Selection UI
```csharp
public class SkinSelectionUI : MonoBehaviour
{
    public void OnSkinButtonClicked(int skinIndex)
    {
        var variant = (CamelSkinManager.CamelSkinVariant)skinIndex;
        var camelManager = FindObjectOfType<CamelSkinManager>();
        camelManager.ApplySkin(variant);
    }
}
```

---

## Deliverables Checklist

- [x] Material specification document (CAMEL_SKIN_MATERIALS_SPEC.md)
- [x] Auto-generation script (CreateCamelMaterials.cs)
- [x] Runtime manager script (CamelSkinManager.cs)
- [x] Application guide (this document)
- [ ] Materials created and tested (pending FBX arrival)
- [ ] Prefab variants created (pending FBX arrival)
- [ ] Integration with game systems (pending game design spec)

---

## Next Steps

**When Camel_Default.fbx arrives:**
1. Import FBX to Assets/Models/Camel/
2. Run `Assets > Create Camel Materials > Setup All Skins` menu
3. Follow Step 3-7 above
4. Validate all four skins match concept art
5. Integrate with character spawning system
6. Test on mobile device
7. Mark AIG-78 complete

**Estimated Time:** 30-45 minutes (excluding testing/validation)

---

**Prepared by:** Artist1  
**Date:** 2026-05-07  
**Status:** Ready for FBX Integration
