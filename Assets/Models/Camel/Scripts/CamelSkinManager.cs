using UnityEngine;

/// <summary>
/// Manages camel skin material variants.
/// Allows runtime switching between Pharaoh, Racing, Mummy, and Golden skins.
/// </summary>
public class CamelSkinManager : MonoBehaviour
{
    public enum CamelSkinVariant
    {
        Pharaoh = 0,
        Racing = 1,
        Mummy = 2,
        Golden = 3
    }

    [SerializeField]
    private Material[] skinMaterials = new Material[4];

    [SerializeField]
    private SkinnedMeshRenderer meshRenderer;

    private CamelSkinVariant currentSkin = CamelSkinVariant.Pharaoh;

    private void OnValidate()
    {
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<SkinnedMeshRenderer>();
        }
    }

    private void Start()
    {
        // Load default skin if not already set
        if (meshRenderer != null && meshRenderer.material == null)
        {
            ApplySkin(CamelSkinVariant.Pharaoh);
        }
    }

    /// <summary>
    /// Apply a skin variant to the camel.
    /// </summary>
    public void ApplySkin(CamelSkinVariant variant)
    {
        if (skinMaterials[(int)variant] == null)
        {
            Debug.LogWarning($"Skin material for {variant} is not assigned!");
            return;
        }

        currentSkin = variant;
        meshRenderer.material = skinMaterials[(int)variant];
        Debug.Log($"Applied camel skin: {variant}");
    }

    /// <summary>
    /// Get the currently active skin variant.
    /// </summary>
    public CamelSkinVariant GetCurrentSkin()
    {
        return currentSkin;
    }

    /// <summary>
    /// Cycle to the next skin variant (useful for testing/UI).
    /// </summary>
    public void CycleNextSkin()
    {
        int nextIndex = ((int)currentSkin + 1) % 4;
        ApplySkin((CamelSkinVariant)nextIndex);
    }

    /// <summary>
    /// Cycle to the previous skin variant (useful for testing/UI).
    /// </summary>
    public void CyclePreviousSkin()
    {
        int prevIndex = ((int)currentSkin - 1 + 4) % 4;
        ApplySkin((CamelSkinVariant)prevIndex);
    }
}
