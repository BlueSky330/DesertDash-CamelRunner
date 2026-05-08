using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Procedurally generates a low-poly Egyptian gold coin collectible mesh.
/// Follows the pattern established by ProceduralCamelMesh (AIG-173).
///
/// Two submeshes:
///   SubMesh 0 (Coin body)  — Egyptian gold   #D4AF37  R=0.831 G=0.686 B=0.216
///   SubMesh 1 (Eye of Ra)  — deeper bronze   #8B6914  R=0.545 G=0.412 B=0.078
///
/// Geometry budget (< 200 tri target):
///   8 sector wedges      × 12 tris =  96
///   2 face caps          × 12 tris =  24
///   1 Eye of Ra oval     × 12 tris =  12
///   2 eyebrow bars       × 12 tris =  24
///   Total                          = 156 tris
///
/// Dimensions: diameter 0.5 units, thickness 0.1 units (coin lies flat in XZ plane).
///
/// Auto-spin:
///   When autoSpin is enabled the coin rotates around +Y at spinSpeed deg/s.
///   OnEnable resets orientation so object-pool reuse starts from a clean state.
///
/// Usage:
///   Attach to a coin prefab; mesh builds on Awake (play mode) or call Build() from Editor.
///   FBX export target: Assets/Models/Egypt/Props/coin_gold.fbx
///
/// Coordinate convention:
///   Coin disc lies in XZ-plane (disc faces look along ±Y). Units are Unity metres.
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public class ProceduralCoinMesh : MonoBehaviour
{
    // Egyptian gold #D4AF37
    public static readonly Color CoinColor   = new Color(0.831f, 0.686f, 0.216f, 1f);
    // Deep bronze #8B6914
    public static readonly Color SymbolColor = new Color(0.545f, 0.412f, 0.078f, 1f);

    private const int SM_COIN   = 0;
    private const int SM_SYMBOL = 1;
    private const int SM_COUNT  = 2;

    [Header("Material Overrides (null = created procedurally)")]
    public Material matCoin;
    public Material matSymbol;

    [Header("Auto-Spin")]
    public bool  autoSpin  = true;
    public float spinSpeed = 180f;  // degrees per second

    void Awake()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        Build();
    }

    void OnEnable()
    {
        // Reset rotation on pool reuse so coins don't carry stale orientation
        transform.localRotation = Quaternion.identity;
    }

    void Update()
    {
        if (autoSpin && Application.isPlaying)
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.Self);
    }

    public void Build()
    {
        var mf = GetOrAdd<MeshFilter>();
        var mr = GetOrAdd<MeshRenderer>();

        mf.sharedMesh = GenerateMesh();

        if (matCoin   == null) matCoin   = CreateCoinMaterial(CoinColor);
        if (matSymbol == null) matSymbol = CreateCoinMaterial(SymbolColor);

        mr.sharedMaterials = new[] { matCoin, matSymbol };
    }

    private Mesh GenerateMesh()
    {
        var coinVerts   = new List<Vector3>();
        var coinNormals = new List<Vector3>();
        var coinTris    = new List<int>();

        var symVerts    = new List<Vector3>();
        var symNormals  = new List<Vector3>();
        var symTris     = new List<int>();

        const float radius = 0.25f;   // diameter 0.5 units
        const float thick  = 0.10f;   // spec: 0.1 unit thickness
        const float halfT  = thick * 0.5f;

        // ── Disc body: 8 sector wedge boxes forming an octagonal disc ─────────
        // Each wedge spans the full diameter along its local Z, narrowing in X.
        // Rotating each wedge by 45° steps and overlapping produces the disc rim.
        const float wedgeW = 0.18f;
        const float wedgeD = radius * 2f;  // = 0.5

        for (int s = 0; s < 8; s++)
        {
            float angle = s * 45f;
            var rot     = Quaternion.Euler(0f, angle, 0f);
            // Centre at origin; wedge extends symmetrically ±radius along local Z
            AppendBox(coinVerts, coinNormals, coinTris,
                      Matrix4x4.TRS(Vector3.zero, rot, Vector3.one),
                      new Vector3(wedgeW, thick, wedgeD));
        }

        // ── Face caps: two thin square discs capping the octagon ──────────────
        float capSize = radius * 1.55f;  // slightly inside the rim wedges
        AppendBox(coinVerts, coinNormals, coinTris,
                  Matrix4x4.TRS(new Vector3(0f,  halfT, 0f), Quaternion.identity, Vector3.one),
                  new Vector3(capSize, 0.012f, capSize));
        AppendBox(coinVerts, coinNormals, coinTris,
                  Matrix4x4.TRS(new Vector3(0f, -halfT, 0f), Quaternion.identity, Vector3.one),
                  new Vector3(capSize, 0.012f, capSize));

        // ── Eye of Ra symbol on top face (slightly raised) ────────────────────
        float symY = halfT + 0.013f;    // just proud of the top cap

        // Almond-shaped eye: one flat wide box
        AppendBox(symVerts, symNormals, symTris,
                  Matrix4x4.TRS(new Vector3(0f, symY, 0f), Quaternion.identity, Vector3.one),
                  new Vector3(0.13f, 0.013f, 0.06f));

        // Left eyebrow bar
        AppendBox(symVerts, symNormals, symTris,
                  Matrix4x4.TRS(new Vector3(-0.04f, symY, 0.052f),
                                Quaternion.Euler(0f, -26f, 0f), Vector3.one),
                  new Vector3(0.055f, 0.011f, 0.022f));

        // Right eyebrow bar
        AppendBox(symVerts, symNormals, symTris,
                  Matrix4x4.TRS(new Vector3( 0.04f, symY, 0.052f),
                                Quaternion.Euler(0f,  26f, 0f), Vector3.one),
                  new Vector3(0.055f, 0.011f, 0.022f));

        // ── Combine into mesh with two submeshes ──────────────────────────────
        var allVerts   = new List<Vector3>(coinVerts.Count + symVerts.Count);
        var allNormals = new List<Vector3>(coinNormals.Count + symNormals.Count);
        allVerts.AddRange(coinVerts);   allVerts.AddRange(symVerts);
        allNormals.AddRange(coinNormals); allNormals.AddRange(symNormals);

        int coinVertCount = coinVerts.Count;
        var offsetSymTris = new List<int>(symTris.Count);
        foreach (int t in symTris) offsetSymTris.Add(t + coinVertCount);

        var mesh = new Mesh { name = "ProceduralCoin" };
        mesh.SetVertices(allVerts);
        mesh.SetNormals(allNormals);
        mesh.subMeshCount = SM_COUNT;
        mesh.SetTriangles(coinTris,      SM_COIN);
        mesh.SetTriangles(offsetSymTris, SM_SYMBOL);
        mesh.RecalculateBounds();
        return mesh;
    }

    // Coin gets slight metallic sheen to read as precious metal
    private static Material CreateCoinMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit")
                     ?? Shader.Find("Standard");
        var mat = new Material(shader) { color = color };
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0.35f);
        if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0.35f);
        if (mat.HasProperty("_Metallic"))   mat.SetFloat("_Metallic",   0.70f);
        return mat;
    }

    private static void AppendBox(
        List<Vector3> verts, List<Vector3> normals, List<int> tris,
        Matrix4x4 mtx, Vector3 size)
    {
        float hx = size.x * 0.5f, hy = size.y * 0.5f, hz = size.z * 0.5f;

        var c = new Vector3[]
        {
            new Vector3(-hx, -hy, -hz), new Vector3( hx, -hy, -hz),
            new Vector3( hx,  hy, -hz), new Vector3(-hx,  hy, -hz),
            new Vector3(-hx, -hy,  hz), new Vector3( hx, -hy,  hz),
            new Vector3( hx,  hy,  hz), new Vector3(-hx,  hy,  hz),
        };

        var faces = new (Vector3 n, int a, int b, int c2, int d)[]
        {
            (Vector3.up,      3, 7, 6, 2),
            (Vector3.down,    0, 1, 5, 4),
            (Vector3.forward, 4, 5, 6, 7),
            (Vector3.back,    1, 0, 3, 2),
            (Vector3.right,   5, 1, 2, 6),
            (Vector3.left,    0, 4, 7, 3),
        };

        foreach (var (n, a, b, c2, d) in faces)
        {
            int faceBase = verts.Count;
            Vector3 wn = mtx.MultiplyVector(n).normalized;
            verts.Add(mtx.MultiplyPoint3x4(c[a]));
            verts.Add(mtx.MultiplyPoint3x4(c[b]));
            verts.Add(mtx.MultiplyPoint3x4(c[c2]));
            verts.Add(mtx.MultiplyPoint3x4(c[d]));
            normals.Add(wn); normals.Add(wn); normals.Add(wn); normals.Add(wn);
            tris.Add(faceBase);     tris.Add(faceBase + 1); tris.Add(faceBase + 2);
            tris.Add(faceBase);     tris.Add(faceBase + 2); tris.Add(faceBase + 3);
        }
    }

    private T GetOrAdd<T>() where T : Component
    {
        T comp = GetComponent<T>();
        return comp != null ? comp : gameObject.AddComponent<T>();
    }
}
