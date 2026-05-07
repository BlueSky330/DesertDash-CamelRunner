using UnityEngine;

/// <summary>
/// Defines material templates for a camel skin variant.
/// Each skin (Base, Pharaoh, Racing, Mummy, Golden) has color/texture variants.
/// </summary>
[CreateAssetMenu(menuName = "Camel Runner/Skin Material Template", fileName = "SkinTemplate_")]
public class SkinMaterialTemplate : ScriptableObject
{
    [System.Serializable]
    public class MaterialVariant
    {
        public string variantName; // e.g., "Default", "Dark", "Light"
        public Material material;
    }

    public string skinName; // e.g., "Camel (Base)", "Pharaoh Camel"
    public MaterialVariant[] variants = new MaterialVariant[1];
    public int defaultVariantIndex = 0;

    /// <summary>Get the default material variant for this skin.</summary>
    public Material GetDefaultMaterial()
    {
        if (variants == null || variants.Length == 0) return null;
        return variants[defaultVariantIndex].material;
    }

    /// <summary>Get a specific material variant by index.</summary>
    public Material GetVariant(int index)
    {
        if (variants == null || index < 0 || index >= variants.Length) return null;
        return variants[index].material;
    }

    /// <summary>Get material variant by name.</summary>
    public Material GetVariantByName(string variantName)
    {
        if (variants == null) return null;
        foreach (var variant in variants)
        {
            if (variant.variantName == variantName && variant.material != null)
                return variant.material;
        }
        return null;
    }
}
