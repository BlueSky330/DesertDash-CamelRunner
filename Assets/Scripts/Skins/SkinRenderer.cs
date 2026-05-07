using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Applies material templates to the player camel when skin is equipped.
/// Subscribes to SkinManager.onSkinEquipped and swaps materials on renderers.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class SkinRenderer : MonoBehaviour
{
    [SerializeField]
    private Dictionary<string, SkinMaterialTemplate> skinTemplates = new Dictionary<string, SkinMaterialTemplate>();

    private Renderer targetRenderer;
    private string currentSkinName;

    void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
    }

    void Start()
    {
        // Subscribe to skin change events
        SkinManager.onSkinEquipped += OnSkinEquipped;

        // Load skin templates from Resources or assigned prefab
        LoadSkinTemplates();

        // Apply current equipped skin if any
        if (SkinManager.Instance != null && SkinManager.Instance.equippedSkin != null)
        {
            ApplySkin(SkinManager.Instance.equippedSkin.skinName);
        }
    }

    void OnDestroy()
    {
        SkinManager.onSkinEquipped -= OnSkinEquipped;
    }

    /// <summary>Event handler: apply material when skin is equipped.</summary>
    private void OnSkinEquipped(string skinName)
    {
        ApplySkin(skinName);
    }

    /// <summary>Apply a skin's material to this renderer.</summary>
    public void ApplySkin(string skinName)
    {
        if (targetRenderer == null) return;

        if (skinTemplates.TryGetValue(skinName, out var template))
        {
            Material mat = template.GetDefaultMaterial();
            if (mat != null)
            {
                targetRenderer.material = mat;
                currentSkinName = skinName;
                Debug.Log($"[SkinRenderer] Applied material for '{skinName}'");
            }
            else
            {
                Debug.LogWarning($"[SkinRenderer] No material found in template for '{skinName}'");
            }
        }
        else
        {
            Debug.LogWarning($"[SkinRenderer] No template found for skin '{skinName}'");
        }
    }

    /// <summary>Load all skin templates (from Resources, assignments, or config).</summary>
    private void LoadSkinTemplates()
    {
        // Load from Resources/Skins/ folder
        var templates = Resources.LoadAll<SkinMaterialTemplate>("Skins");
        foreach (var template in templates)
        {
            if (template != null)
            {
                skinTemplates[template.skinName] = template;
                Debug.Log($"[SkinRenderer] Loaded template for '{template.skinName}'");
            }
        }

        if (skinTemplates.Count == 0)
        {
            Debug.LogWarning("[SkinRenderer] No skin templates found in Resources/Skins/. Create templates and place in Resources/Skins/ folder.");
        }
    }

    /// <summary>Register a skin template manually (useful for dynamic loading).</summary>
    public void RegisterTemplate(SkinMaterialTemplate template)
    {
        if (template != null)
        {
            skinTemplates[template.skinName] = template;
        }
    }
}
