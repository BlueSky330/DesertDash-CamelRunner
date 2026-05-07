# Material Templates & Skin Swapping — Test Guide

## Overview
This system allows the player camel to swap materials/meshes when a skin is equipped via the `SkinManager.onSkinEquipped` event.

## Components

### SkinMaterialTemplate.cs
- **Location:** `Assets/Scripts/Skins/SkinMaterialTemplate.cs`
- **Purpose:** Scriptable object that defines color/texture variants for each skin
- **Usage:** Create templates in Resources/Skins/ folder for each skin variant
- **Features:**
  - Supports multiple material variants per skin (Default, Dark, Light)
  - Can retrieve materials by index or name
  - Automatically created by SkinMaterialSetup editor utility

### SkinRenderer.cs
- **Location:** `Assets/Scripts/Skins/SkinRenderer.cs`
- **Purpose:** Component that applies materials when skins are equipped
- **Attachment:** Add to any GameObject with a Renderer component
- **Behavior:**
  - Subscribes to `SkinManager.onSkinEquipped` event
  - Loads skin templates from Resources/Skins/
  - Swaps material when event fires

### PlayerController.cs (Updated)
- **New Methods:**
  - `ApplySkin(string skinName)` — swaps all renderers to skin material
  - `OnSkinEquipped(string skinName)` — event callback
- **Integration:** Subscribes to SkinManager.onSkinEquipped in Start()
- **Behavior:** Updates all child renderers when skin changes

## Setup Instructions

### 1. Create Test Materials (One-time setup)

1. Open Unity Editor
2. Navigate to **Tools > Camel Runner > Setup Skin Materials**
3. This creates:
   - Color-coded materials for each skin (Tan, Gold, Black, Off-white, Bright Gold)
   - 3 variants per skin (Default, Dark, Light)
   - SkinMaterialTemplate assets in Resources/Skins/

### 2. Test Without Full Model (Editor Testing)

Create a simple test scene:

```
1. Create a new scene: Assets/Scenes/SkinSwapTest.unity
2. Create hierarchy:
   - Canvas (UI for skin buttons)
   - Player (empty GameObject at origin)
     - CamelProxy (child cube/primitive with Renderer)
3. On Player:
   - Add CharacterController component
   - Add Animator component (can use any controller)
   - Add PlayerController script
   - Add SkinRenderer script (attach to CamelProxy)
4. Create or link SkinManager:
   - If not in scene, create empty GameObject
   - Add SkinManager script
   - Mark as DontDestroyOnLoad
5. Create UI buttons to call SkinManager.Instance.EquipSkin("Skin Name")
```

### 3. Test Steps

**In Editor Play Mode:**

1. Select a skin via button (e.g., "Camel (Base)")
   - Proxy cube material should change to tan color
   - Console should log: `[PlayerController] Applied skin 'Camel (Base)' with material`

2. Switch to another skin (e.g., "Pharaoh Camel")
   - Proxy cube material should change to gold color
   - Event callback fires: `[SkinManager] Equipped: Pharaoh Camel`
   - `[PlayerController] Applied skin 'Pharaoh Camel' with material`

3. Verify all 5 skins swap correctly:
   - Camel (Base) → Tan
   - Pharaoh Camel → Gold
   - Racing Camel → Black
   - Mummy Camel → Off-white
   - Golden Camel → Bright Gold

4. Switch scenes and back to verify persistence

### 4. Integration with Full Model

When the camel 3D model (Camel_Default.fbx from AIG-26) is ready:

1. Replace the CamelProxy cube with the actual model prefab
2. Ensure the model has a Renderer component
3. The same material swapping system will work automatically
4. No code changes needed — just swap the visual

### 5. Troubleshooting

**"Skin template not found" warning:**
- Run Tools > Camel Runner > Setup Skin Materials again
- Verify templates exist in Assets/Resources/Skins/

**Materials not swapping:**
- Check that PlayerController has a renderer attached or children with renderers
- Verify SkinManager is in scene and initialized
- Check console for event callback logs

**Prefab/Model Swap Notes:**
- To swap player model: Replace CamelProxy/child with actual camel model prefab
- PlayerController.ApplySkin() gets all child renderers, so it works with multi-mesh models
- Material persists across model swaps as long as SkinMaterialTemplate exists

## Next Steps

1. **Test in Editor:** Create SkinSwapTest.unity scene and verify material swapping
2. **Integration:** When camel model arrives (AIG-26), replace proxy with actual model
3. **Variants:** Add texture variants and normal maps to SkinMaterialTemplate as art assets complete
4. **Store UI:** Connect to store/shop system for skin purchases

## Files Created

```
Assets/Scripts/Skins/
  ├── SkinMaterialTemplate.cs (NEW)
  ├── SkinRenderer.cs (NEW)
  ├── SkinManager.cs (EXISTING - comment line 158 addressed)
  └── Editor/
      └── SkinMaterialSetup.cs (NEW)

Assets/Materials/Skins/ (created by setup utility)
  ├── Camel_Base/
  ├── Pharaoh_Camel/
  ├── Racing_Camel/
  ├── Mummy_Camel/
  └── Golden_Camel/

Assets/Resources/Skins/ (created by setup utility)
  ├── SkinTemplate_Camel_Base.asset
  ├── SkinTemplate_Pharaoh_Camel.asset
  ├── SkinTemplate_Racing_Camel.asset
  ├── SkinTemplate_Mummy_Camel.asset
  └── SkinTemplate_Golden_Camel.asset
```
