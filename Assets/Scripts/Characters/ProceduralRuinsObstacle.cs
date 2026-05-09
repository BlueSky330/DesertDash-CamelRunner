using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Procedurally generates a low-poly Egyptian crumbling ruins obstacle mesh.
/// Follows the established pattern from ProceduralRockMesh / ProceduralCamelMesh.
///
/// Geometry: stacked and offset sandstone blocks forming a partial column/wall
/// with broken edges and toppled fragments — readable as "ruins" at mobile scale.
///
/// Bounding box: approx 1.2 × 2.4 × 1.2 Unity metres (tall, narrow obstacle).
///
/// Colour palette:
///   Main sandstone  #C8A96E  R=0.784 G=0.663 B=0.431
///   Shadow stone    #9B7A4A  R=0.608 G=0.478 B=0.290
///   Accent dark     #6B5230  R=0.420 G=0.322 B=0.188
///
/// Usage:
///   Attach to a GameObject; mesh builds on Awake (play mode) or call Build().
///   Pool tag: "Obstacle_Ruins"
///
/// Coordinate convention: base at Y=0, obstacle faces +Z. Units are Unity metres.
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public class ProceduralRuinsObstacle : MonoBehaviour
{
    public static readonly Color SandstoneColor = new Color(0.784f, 0.663f, 0.431f, 1f);
    public static readonly Color ShadowColor    = new Color(0.608f, 0.478f, 0.290f, 1f);

    [Header("Material Override (null = created procedurally)")]
    public Material bodyMaterial;

    [Header("Dimensions")]
    [Range(0.5f, 2f)] public float widthScale  = 1f;
    [Range(0.5f, 2f)] public float heightScale = 1f;

    // ── Geometry constants (all values in Unity metres, scaled by widthScale/heightScale) ──

    // Base slab
    private const float BaseSlab_CentreY  = 0.15f;
    private const float BaseSlab_Width    = 1.20f;
    private const float BaseSlab_Height   = 0.30f;
    private const float BaseSlab_Depth    = 1.00f;

    // Lower column
    private const float LowerCol_CentreY  = 0.60f;
    private const float LowerCol_Width    = 0.70f;
    private const float LowerCol_Height   = 0.60f;

    // Mid column (broken, offset for crumbled look)
    private const float MidCol_OffsetX    =  0.05f;
    private const float MidCol_CentreY    =  1.10f;
    private const float MidCol_OffsetZ    = -0.04f;
    private const float MidCol_Width      =  0.65f;
    private const float MidCol_Height     =  0.55f;
    private const float MidCol_TiltY      =  3f;     // degrees

    // Upper broken stump
    private const float Stump_OffsetX     = -0.06f;
    private const float Stump_CentreY     =  1.60f;
    private const float Stump_OffsetZ     =  0.03f;
    private const float Stump_Width       =  0.55f;
    private const float Stump_Height      =  0.45f;
    private const float Stump_TiltX       =  1.5f;
    private const float Stump_TiltY       = -5f;
    private const float Stump_TiltZ       =  1f;

    // Broken top fragment
    private const float TopFrag_OffsetX   =  0.08f;
    private const float TopFrag_CentreY   =  2.00f;
    private const float TopFrag_OffsetZ   = -0.05f;
    private const float TopFrag_Width     =  0.40f;
    private const float TopFrag_Height    =  0.20f;
    private const float TopFrag_TiltX     =  8f;
    private const float TopFrag_TiltY     =  12f;
    private const float TopFrag_TiltZ     =  4f;

    // Toppled side slab
    private const float ToppledSlab_OffsetX =  0.60f;
    private const float ToppledSlab_CentreY =  0.18f;
    private const float ToppledSlab_OffsetZ =  0.10f;
    private const float ToppledSlab_Width   =  0.80f;
    private const float ToppledSlab_Height  =  0.14f;
    private const float ToppledSlab_Depth   =  0.55f;
    private const float ToppledSlab_TiltY   =  20f;
    private const float ToppledSlab_TiltZ   =  15f;

    // Rubble piece 1
    private const float Rubble1_OffsetX   = -0.50f;
    private const float Rubble1_CentreY   =  0.08f;
    private const float Rubble1_OffsetZ   =  0.20f;
    private const float Rubble1_Width     =  0.35f;
    private const float Rubble1_Height    =  0.12f;
    private const float Rubble1_Depth     =  0.30f;
    private const float Rubble1_TiltY     = -25f;
    private const float Rubble1_TiltZ     = -8f;

    // Rubble piece 2
    private const float Rubble2_OffsetX   =  0.30f;
    private const float Rubble2_CentreY   =  0.07f;
    private const float Rubble2_OffsetZ   = -0.45f;
    private const float Rubble2_Width     =  0.28f;
    private const float Rubble2_Height    =  0.10f;
    private const float Rubble2_Depth     =  0.22f;
    private const float Rubble2_TiltY     =  40f;
    private const float Rubble2_TiltZ     =  5f;

    // Decorative carved panel (hieroglyph slot)
    private const float Panel_CentreY     =  0.58f;
    private const float Panel_OffsetZ     =  0.36f;
    private const float Panel_Width       =  0.42f;
    private const float Panel_Height      =  0.36f;
    private const float Panel_Depth       =  0.04f;

    // Carved mark 1
    private const float Mark1_OffsetX    = -0.08f;
    private const float Mark1_CentreY    =  0.62f;
    private const float Mark1_OffsetZ    =  0.385f;
    private const float Mark1_Width      =  0.10f;
    private const float Mark1_Height     =  0.10f;
    private const float Mark1_Depth      =  0.02f;

    // Carved mark 2
    private const float Mark2_OffsetX    =  0.08f;
    private const float Mark2_CentreY    =  0.52f;
    private const float Mark2_OffsetZ    =  0.385f;
    private const float Mark2_Width      =  0.08f;
    private const float Mark2_Height     =  0.08f;
    private const float Mark2_Depth      =  0.02f;

    void Awake()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        Build();
    }

    public void Build()
    {
        var mf = GetOrAdd<MeshFilter>();
        var mr = GetOrAdd<MeshRenderer>();
        mf.sharedMesh = GenerateMesh();
        if (bodyMaterial == null) bodyMaterial = CreateFlatMaterial();
        mr.sharedMaterial = bodyMaterial;
    }

    private Mesh GenerateMesh()
    {
        float w = widthScale;
        float h = heightScale;

        var parts = new List<(Vector3 centre, Vector3 size, Quaternion rot)>();

        // ── Base slab — wide foundation block ────────────────────────────────
        parts.Add((
            new Vector3(0f, BaseSlab_CentreY * h, 0f),
            new Vector3(BaseSlab_Width * w, BaseSlab_Height * h, BaseSlab_Depth * w),
            Quaternion.identity));

        // ── Lower column block ────────────────────────────────────────────────
        parts.Add((
            new Vector3(0f, LowerCol_CentreY * h, 0f),
            new Vector3(LowerCol_Width * w, LowerCol_Height * h, LowerCol_Width * w),
            Quaternion.identity));

        // ── Mid column block — slightly offset for broken look ────────────────
        parts.Add((
            new Vector3(MidCol_OffsetX * w, MidCol_CentreY * h, MidCol_OffsetZ * w),
            new Vector3(MidCol_Width * w, MidCol_Height * h, MidCol_Width * w),
            Quaternion.Euler(0f, MidCol_TiltY, 0f)));

        // ── Upper broken stump — narrower, tilted ─────────────────────────────
        parts.Add((
            new Vector3(Stump_OffsetX * w, Stump_CentreY * h, Stump_OffsetZ * w),
            new Vector3(Stump_Width * w, Stump_Height * h, Stump_Width * w),
            Quaternion.Euler(Stump_TiltX, Stump_TiltY, Stump_TiltZ)));

        // ── Broken top fragment — small tilted block at peak ──────────────────
        parts.Add((
            new Vector3(TopFrag_OffsetX * w, TopFrag_CentreY * h, TopFrag_OffsetZ * w),
            new Vector3(TopFrag_Width * w, TopFrag_Height * h, TopFrag_Width * w),
            Quaternion.Euler(TopFrag_TiltX, TopFrag_TiltY, TopFrag_TiltZ)));

        // ── Toppled side slab — fallen block leaning against base ────────────
        parts.Add((
            new Vector3(ToppledSlab_OffsetX * w, ToppledSlab_CentreY * h, ToppledSlab_OffsetZ * w),
            new Vector3(ToppledSlab_Width * w, ToppledSlab_Height * h, ToppledSlab_Depth * w),
            Quaternion.Euler(0f, ToppledSlab_TiltY, ToppledSlab_TiltZ)));

        // ── Rubble piece 1 ────────────────────────────────────────────────────
        parts.Add((
            new Vector3(Rubble1_OffsetX * w, Rubble1_CentreY * h, Rubble1_OffsetZ * w),
            new Vector3(Rubble1_Width * w, Rubble1_Height * h, Rubble1_Depth * w),
            Quaternion.Euler(0f, Rubble1_TiltY, Rubble1_TiltZ)));

        // ── Rubble piece 2 ────────────────────────────────────────────────────
        parts.Add((
            new Vector3(Rubble2_OffsetX * w, Rubble2_CentreY * h, Rubble2_OffsetZ * w),
            new Vector3(Rubble2_Width * w, Rubble2_Height * h, Rubble2_Depth * w),
            Quaternion.Euler(0f, Rubble2_TiltY, Rubble2_TiltZ)));

        // ── Decorative carved panel (hieroglyph slot on lower column) ─────────
        parts.Add((
            new Vector3(0f, Panel_CentreY * h, Panel_OffsetZ * w),
            new Vector3(Panel_Width * w, Panel_Height * h, Panel_Depth * w),
            Quaternion.identity));

        // ── Carved mark 1 (raised symbol on panel) ───────────────────────────
        parts.Add((
            new Vector3(Mark1_OffsetX * w, Mark1_CentreY * h, Mark1_OffsetZ * w),
            new Vector3(Mark1_Width * w, Mark1_Height * h, Mark1_Depth * w),
            Quaternion.identity));

        // ── Carved mark 2 ─────────────────────────────────────────────────────
        parts.Add((
            new Vector3(Mark2_OffsetX * w, Mark2_CentreY * h, Mark2_OffsetZ * w),
            new Vector3(Mark2_Width * w, Mark2_Height * h, Mark2_Depth * w),
            Quaternion.identity));

        return CombineBoxes(parts);
    }

    // ── Mesh utilities ─────────────────────────────────────────────────────────

    private static Mesh CombineBoxes(List<(Vector3 centre, Vector3 size, Quaternion rot)> parts)
    {
        var allVerts   = new List<Vector3>();
        var allNormals = new List<Vector3>();
        var allUVs     = new List<Vector2>();
        var allTris    = new List<int>();

        foreach (var (centre, size, rot) in parts)
        {
            Matrix4x4 mtx = Matrix4x4.TRS(centre, rot, Vector3.one);
            AppendBox(allVerts, allNormals, allUVs, allTris, mtx, size);
        }

        var mesh = new Mesh { name = "ProceduralRuins" };
        mesh.SetVertices(allVerts);
        mesh.SetNormals(allNormals);
        mesh.SetUVs(0, allUVs);
        mesh.SetTriangles(allTris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static void AppendBox(
        List<Vector3> verts, List<Vector3> normals, List<Vector2> uvs,
        List<int> tris, Matrix4x4 mtx, Vector3 size)
    {
        float hx = size.x * 0.5f;
        float hy = size.y * 0.5f;
        float hz = size.z * 0.5f;

        var c = new Vector3[]
        {
            new Vector3(-hx, -hy, -hz), // 0
            new Vector3( hx, -hy, -hz), // 1
            new Vector3( hx,  hy, -hz), // 2
            new Vector3(-hx,  hy, -hz), // 3
            new Vector3(-hx, -hy,  hz), // 4
            new Vector3( hx, -hy,  hz), // 5
            new Vector3( hx,  hy,  hz), // 6
            new Vector3(-hx,  hy,  hz), // 7
        };

        // (normal, a, b, c2, d, uvA, uvB, uvC, uvD)
        var faces = new (Vector3 n, int a, int b, int c2, int d,
                         Vector2 uvA, Vector2 uvB, Vector2 uvC2, Vector2 uvD)[]
        {
            (Vector3.up,      3, 7, 6, 2,  new Vector2(0,0), new Vector2(0,1), new Vector2(1,1), new Vector2(1,0)),
            (Vector3.down,    0, 1, 5, 4,  new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1)),
            (Vector3.forward, 4, 5, 6, 7,  new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1)),
            (Vector3.back,    1, 0, 3, 2,  new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1)),
            (Vector3.right,   5, 1, 2, 6,  new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1)),
            (Vector3.left,    0, 4, 7, 3,  new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1)),
        };

        foreach (var (n, a, b, c2, d, uvA, uvB, uvC2, uvD) in faces)
        {
            int faceBase = verts.Count;
            Vector3 wn = mtx.MultiplyVector(n).normalized;

            verts.Add(mtx.MultiplyPoint3x4(c[a]));
            verts.Add(mtx.MultiplyPoint3x4(c[b]));
            verts.Add(mtx.MultiplyPoint3x4(c[c2]));
            verts.Add(mtx.MultiplyPoint3x4(c[d]));

            normals.Add(wn); normals.Add(wn); normals.Add(wn); normals.Add(wn);
            uvs.Add(uvA); uvs.Add(uvB); uvs.Add(uvC2); uvs.Add(uvD);

            tris.Add(faceBase);     tris.Add(faceBase + 1); tris.Add(faceBase + 2);
            tris.Add(faceBase);     tris.Add(faceBase + 2); tris.Add(faceBase + 3);
        }
    }

    public static Material CreateFlatMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit")
                     ?? Shader.Find("Standard");

        // Try to load ruins texture from Resources
        Texture2D tex = Resources.Load<Texture2D>("Egypt/ruins_obstacle");
        var mat = new Material(shader) { color = SandstoneColor };
        if (tex != null) mat.mainTexture = tex;
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0f);
        if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0f);
        if (mat.HasProperty("_Metallic"))   mat.SetFloat("_Metallic",   0f);
        return mat;
    }

    private T GetOrAdd<T>() where T : Component
    {
        T comp = GetComponent<T>();
        return comp != null ? comp : gameObject.AddComponent<T>();
    }
}
