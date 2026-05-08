using UnityEngine;

/// <summary>
/// Runtime component that applies Egyptian themed textures and colours to all
/// procedural obstacles, the camel, and background elements on scene load.
///
/// Attach to any persistent GameObject (e.g., GameManager) or add to Gameplay scene.
/// Called once on Awake — safe to run from the object pool bootstrap.
///
/// Textures are loaded from Assets/Resources/Egypt/ (included in build automatically).
/// Falls back to existing flat-shaded colours if a texture is not found.
/// </summary>
[DefaultExecutionOrder(-50)]
public class EgyptAssetSetup : MonoBehaviour
{
    [Header("Override Materials (leave null to auto-load from Resources/Egypt/)")]
    public Material camelMaterialOverride;
    public Material rockMaterialOverride;
    public Material ruinsMaterialOverride;
    public Material cactusMaterialOverride;
    public Material coinMaterialOverride;

    // Cache loaded textures so repeated scene object spawns don't hit Resources again
    private static Texture2D _camelTex;
    private static Texture2D _rockTex;
    private static Texture2D _ruinsTex;
    private static Texture2D _cactusTex;
    private static Texture2D _coinTex;
    private static bool _texturesLoaded;

    void Awake()
    {
        if (!_texturesLoaded)
            LoadTextures();
    }

    private static void LoadTextures()
    {
        _camelTex  = Resources.Load<Texture2D>("Egypt/camel_idle");
        _rockTex   = Resources.Load<Texture2D>("Egypt/rock_obstacle");
        _ruinsTex  = Resources.Load<Texture2D>("Egypt/ruins_obstacle");
        _cactusTex = Resources.Load<Texture2D>("Egypt/cactus_obstacle");
        _coinTex   = Resources.Load<Texture2D>("Egypt/collectible_coin");
        _texturesLoaded = true;

        int loaded = (_camelTex != null ? 1 : 0)
                   + (_rockTex  != null ? 1 : 0)
                   + (_ruinsTex != null ? 1 : 0)
                   + (_cactusTex != null ? 1 : 0)
                   + (_coinTex   != null ? 1 : 0);
        Debug.Log($"[EgyptAssetSetup] Loaded {loaded}/5 Egypt textures from Resources.");
    }

    /// <summary>
    /// Apply the appropriate Egypt texture to a newly spawned object's MeshRenderer.
    /// Call this from ObstacleSpawner or ObjectPool when activating an obstacle.
    /// </summary>
    public static void ApplyEgyptTexture(GameObject go, string poolTag)
    {
        if (!_texturesLoaded) LoadTextures();

        Texture2D tex = null;
        switch (poolTag)
        {
            case "Obstacle_Rock":    tex = _rockTex;   break;
            case "Obstacle_Ruins":   tex = _ruinsTex;  break;
            case "Obstacle_Cactus":  tex = _cactusTex; break;
            case "Collectible_Coin":
            case "Collectible_Date": tex = _coinTex;   break;
        }

        if (tex == null) return;

        var mr = go.GetComponentInChildren<MeshRenderer>();
        if (mr != null && mr.sharedMaterial != null)
        {
            // Instance the material so we don't stomp shared prefab material
            mr.material.mainTexture = tex;
        }
    }

    /// <summary>
    /// Retrieve the loaded camel texture (used by CamelPrefabBuilder at runtime).
    /// </summary>
    public static Texture2D CamelTexture
    {
        get
        {
            if (!_texturesLoaded) LoadTextures();
            return _camelTex;
        }
    }
}
