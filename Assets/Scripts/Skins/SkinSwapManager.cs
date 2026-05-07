using UnityEngine;

/// <summary>
/// Handles visual swapping of camel skin when SkinManager equips a skin.
/// Listens to SkinManager.onSkinEquipped and swaps materials and/or mesh variants.
///
/// Approach:
///   - All skins share the same base mesh and rigging
///   - Only materials are swapped per skin variant
///   - This minimizes memory overhead and instantiation overhead
/// </summary>
public class SkinSwapManager : MonoBehaviour
{
    [System.Serializable]
    public class SkinMaterialSet
    {
        public string skinName;
        public Material material;
    }

    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private SkinMaterialSet[] skinMaterials;

    private void Start()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        // Subscribe to skin changes
        SkinManager.onSkinEquipped += OnSkinEquipped;

        // Apply initial skin
        if (SkinManager.Instance != null && SkinManager.Instance.equippedSkin != null)
            OnSkinEquipped(SkinManager.Instance.equippedSkin.skinName);
    }

    private void OnDestroy()
    {
        SkinManager.onSkinEquipped -= OnSkinEquipped;
    }

    /// <summary>
    /// Swap the mesh material to match the equipped skin.
    /// Called whenever SkinManager equips a new skin.
    /// </summary>
    private void OnSkinEquipped(string skinName)
    {
        if (meshRenderer == null)
        {
            Debug.LogWarning("[SkinSwapManager] SkinnedMeshRenderer not found on " + gameObject.name);
            return;
        }

        // Find the material set for this skin
        foreach (var set in skinMaterials)
        {
            if (set.skinName == skinName)
            {
                if (set.material != null)
                {
                    // Create instance so we don't modify the asset
                    Material matInstance = new Material(set.material);
                    meshRenderer.material = matInstance;
                    Debug.Log($"[SkinSwapManager] Equipped skin: {skinName}");
                    return;
                }
            }
        }

        Debug.LogWarning($"[SkinSwapManager] No material found for skin '{skinName}'");
    }
}
