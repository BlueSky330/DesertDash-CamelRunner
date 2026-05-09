using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Procedurally generates a low-poly Egyptian desert boulder obstacle mesh.
/// Follows the pattern established by ProceduralCamelMesh (AIG-173).
///
/// Single submesh — desert sandstone #9B8468  R=0.608 G=0.518 B=0.408
///
/// Geometry budget (< 400 tri target):
///   1 main body         × 12 tris =  12
///   4 side lobes        × 12 tris =  48
///   1 top cap           × 12 tris =  12
///   4 corner slabs      × 12 tris =  48
///   5 surface bumps     × 12 tris =  60
///   3 base rubble slabs × 12 tris =  36
///   1 hieroglyph panel  × 12 tris =  12
///   3 hieroglyph marks  × 12 tris =  36
///   Total                         = 264 tris
///
/// Bounding box: approx 2 × 2 × 2 Unity metres (imposing obstacle silhouette).
///
/// Usage:
///   Attach to a GameObject; mesh builds on Awake (play mode) or call Build() from Editor.
///   FBX export target: Assets/Models/Egypt/Props/rock_large.fbx
///
/// Coordinate convention:
///   Base at Y=0, obstacle faces +Z. Units are Unity metres.
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public class ProceduralRockMesh : MonoBehaviour
{
    // Desert sandstone #9B8468 — R=0.608, G=0.518, B=0.408
    public static readonly Color RockColor = new Color(0.608f, 0.518f, 0.408f, 1f);

    [Header("Material Override (null = created procedurally)")]
    public Material bodyMaterial;

    // ── Geometry constants (Unity metres) ─────────────────────────────────────

    // Main body
    private const float MainBody_CentreY  = 1.00f;
    private const float MainBody_Width    = 2.00f;
    private const float MainBody_Height   = 1.80f;
    private const float MainBody_Depth    = 1.80f;

    // Side lobe 1 (+X)
    private const float Lobe1_X = 0.75f;  private const float Lobe1_Y = 0.85f;  private const float Lobe1_Z =  0.22f;
    private const float Lobe1_W = 0.90f;  private const float Lobe1_H = 1.50f;  private const float Lobe1_D = 1.10f;
    private const float Lobe1_TiltY =  14f;  private const float Lobe1_TiltZ =  6f;

    // Side lobe 2 (-X)
    private const float Lobe2_X = -0.75f; private const float Lobe2_Y = 0.90f;  private const float Lobe2_Z = -0.15f;
    private const float Lobe2_W =  0.85f; private const float Lobe2_H = 1.40f;  private const float Lobe2_D =  1.20f;
    private const float Lobe2_TiltY = -10f;  private const float Lobe2_TiltZ = -5f;

    // Side lobe 3 (+Z)
    private const float Lobe3_X =  0.20f; private const float Lobe3_Y = 0.80f;  private const float Lobe3_Z =  0.70f;
    private const float Lobe3_W =  1.30f; private const float Lobe3_H = 1.30f;  private const float Lobe3_D =  0.80f;
    private const float Lobe3_TiltX =  4f;   private const float Lobe3_TiltY =   6f;

    // Side lobe 4 (-Z)
    private const float Lobe4_X = -0.10f; private const float Lobe4_Y = 0.88f;  private const float Lobe4_Z = -0.65f;
    private const float Lobe4_W =  1.20f; private const float Lobe4_H = 1.25f;  private const float Lobe4_D =  0.90f;
    private const float Lobe4_TiltX = -3f;   private const float Lobe4_TiltY =  -8f;

    // Top cap
    private const float TopCap_X =  0.08f; private const float TopCap_Y = 1.90f; private const float TopCap_Z = -0.06f;
    private const float TopCap_W =  1.40f; private const float TopCap_H = 0.30f; private const float TopCap_D =  1.30f;
    private const float TopCap_TiltX = 3f; private const float TopCap_TiltY = 12f; private const float TopCap_TiltZ = 4f;

    // Corner slabs (ground-contact reinforcement)
    private const float Corner_Height = 0.20f;
    private const float Corner1_X =  0.60f; private const float Corner1_Z =  0.50f;
    private const float Corner1_W =  0.50f; private const float Corner1_H = 0.40f; private const float Corner1_D = 0.60f;
    private const float Corner1_TiltY =  18f;

    private const float Corner2_X = -0.60f; private const float Corner2_Z =  0.50f;
    private const float Corner2_W =  0.50f; private const float Corner2_H = 0.40f; private const float Corner2_D = 0.60f;
    private const float Corner2_TiltY = -18f;

    private const float Corner3_X =  0.55f; private const float Corner3_Z = -0.45f;
    private const float Corner3_W =  0.45f; private const float Corner3_H = 0.38f; private const float Corner3_D = 0.55f;
    private const float Corner3_TiltY = -14f;

    private const float Corner4_X = -0.55f; private const float Corner4_Z = -0.45f;
    private const float Corner4_W =  0.45f; private const float Corner4_H = 0.38f; private const float Corner4_D = 0.55f;
    private const float Corner4_TiltY =  14f;

    // Surface bumps
    private const float Bump1_X =  0.45f; private const float Bump1_Y = 1.70f; private const float Bump1_Z =  0.35f;
    private const float Bump1_W =  0.40f; private const float Bump1_H = 0.28f; private const float Bump1_D =  0.36f;
    private const float Bump1_TiltX = -5f; private const float Bump1_TiltY =  22f; private const float Bump1_TiltZ =  9f;

    private const float Bump2_X = -0.35f; private const float Bump2_Y = 1.55f; private const float Bump2_Z = -0.40f;
    private const float Bump2_W =  0.36f; private const float Bump2_H = 0.24f; private const float Bump2_D =  0.44f;
    private const float Bump2_TiltX =  5f; private const float Bump2_TiltY = -18f; private const float Bump2_TiltZ = -7f;

    private const float Bump3_X =  0.10f; private const float Bump3_Y = 1.78f; private const float Bump3_Z =  0.55f;
    private const float Bump3_W =  0.44f; private const float Bump3_H = 0.24f; private const float Bump3_D =  0.32f;
    private const float Bump3_TiltX = -4f; private const float Bump3_TiltY =  10f; private const float Bump3_TiltZ =  5f;

    private const float Bump4_X = -0.55f; private const float Bump4_Y = 1.42f; private const float Bump4_Z =  0.30f;
    private const float Bump4_W =  0.32f; private const float Bump4_H = 0.20f; private const float Bump4_D =  0.40f;
    private const float Bump4_TiltX =  7f; private const float Bump4_TiltY = -28f; private const float Bump4_TiltZ =  3f;

    private const float Bump5_X =  0.30f; private const float Bump5_Y = 1.60f; private const float Bump5_Z = -0.50f;
    private const float Bump5_W =  0.36f; private const float Bump5_H = 0.22f; private const float Bump5_D =  0.28f;
    private const float Bump5_TiltX = -5f; private const float Bump5_TiltY =  20f; private const float Bump5_TiltZ = -6f;

    // Base rubble slabs
    private const float Rubble_Y    = 0.07f;
    private const float Rubble1_X   =  0.85f; private const float Rubble1_Z   =  0.30f;
    private const float Rubble1_W   =  0.55f; private const float Rubble1_H   =  0.13f; private const float Rubble1_D   =  0.70f;
    private const float Rubble1_TiltY =  22f;

    private const float Rubble2_X   = -0.70f; private const float Rubble2_Z   = -0.20f;
    private const float Rubble2_W   =  0.65f; private const float Rubble2_H   =  0.11f; private const float Rubble2_D   =  0.55f;
    private const float Rubble2_TiltY = -20f;

    private const float Rubble3_X   =  0.10f; private const float Rubble3_Z   =  0.80f;
    private const float Rubble3_W   =  0.90f; private const float Rubble3_H   =  0.10f; private const float Rubble3_D   =  0.44f;
    private const float Rubble3_TiltY =   6f;

    // Hieroglyph panel
    private const float Panel_CentreY  = 1.00f;
    private const float Panel_OffsetZ  = 0.92f;
    private const float Panel_Width    = 0.60f;
    private const float Panel_Height   = 0.80f;
    private const float Panel_Depth    = 0.08f;

    // Cartouche bar marks (interior lines on panel)
    private const float Mark_OffsetZ   = 0.965f;
    private const float Mark_Width1    = 0.44f;  private const float Mark_Width2 = 0.40f;
    private const float Mark_Height    = 0.07f;
    private const float Mark_Depth     = 0.025f;
    private const float MarkTop_Y      = 1.22f;
    private const float MarkMid_Y      = 1.00f;
    private const float MarkBot_Y      = 0.78f;

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
        var parts = new List<(Vector3 centre, Vector3 size, Quaternion rot)>();

        // ── Main body: slightly squashed boulder ──────────────────────────────
        parts.Add((new Vector3(0f, MainBody_CentreY, 0f),
                   new Vector3(MainBody_Width, MainBody_Height, MainBody_Depth),
                   Quaternion.identity));

        // ── Side lobes: overlap main body for irregular lumpy silhouette ───────
        parts.Add((new Vector3(Lobe1_X, Lobe1_Y, Lobe1_Z),
                   new Vector3(Lobe1_W, Lobe1_H, Lobe1_D),
                   Quaternion.Euler(0f, Lobe1_TiltY, Lobe1_TiltZ)));
        parts.Add((new Vector3(Lobe2_X, Lobe2_Y, Lobe2_Z),
                   new Vector3(Lobe2_W, Lobe2_H, Lobe2_D),
                   Quaternion.Euler(0f, Lobe2_TiltY, Lobe2_TiltZ)));
        parts.Add((new Vector3(Lobe3_X, Lobe3_Y, Lobe3_Z),
                   new Vector3(Lobe3_W, Lobe3_H, Lobe3_D),
                   Quaternion.Euler(Lobe3_TiltX, Lobe3_TiltY, 0f)));
        parts.Add((new Vector3(Lobe4_X, Lobe4_Y, Lobe4_Z),
                   new Vector3(Lobe4_W, Lobe4_H, Lobe4_D),
                   Quaternion.Euler(Lobe4_TiltX, Lobe4_TiltY, 0f)));

        // ── Top cap: rougher upper surface capping the boulder ─────────────────
        parts.Add((new Vector3(TopCap_X, TopCap_Y, TopCap_Z),
                   new Vector3(TopCap_W, TopCap_H, TopCap_D),
                   Quaternion.Euler(TopCap_TiltX, TopCap_TiltY, TopCap_TiltZ)));

        // ── Corner slabs: ground-contact reinforcement for stability ───────────
        parts.Add((new Vector3(Corner1_X, Corner_Height, Corner1_Z),
                   new Vector3(Corner1_W, Corner1_H, Corner1_D),
                   Quaternion.Euler(0f, Corner1_TiltY, 0f)));
        parts.Add((new Vector3(Corner2_X, Corner_Height, Corner2_Z),
                   new Vector3(Corner2_W, Corner2_H, Corner2_D),
                   Quaternion.Euler(0f, Corner2_TiltY, 0f)));
        parts.Add((new Vector3(Corner3_X, Corner_Height, Corner3_Z),
                   new Vector3(Corner3_W, Corner3_H, Corner3_D),
                   Quaternion.Euler(0f, Corner3_TiltY, 0f)));
        parts.Add((new Vector3(Corner4_X, Corner_Height, Corner4_Z),
                   new Vector3(Corner4_W, Corner4_H, Corner4_D),
                   Quaternion.Euler(0f, Corner4_TiltY, 0f)));

        // ── Surface bumps: small protrusions for rocky texture ─────────────────
        parts.Add((new Vector3(Bump1_X, Bump1_Y, Bump1_Z),
                   new Vector3(Bump1_W, Bump1_H, Bump1_D),
                   Quaternion.Euler(Bump1_TiltX, Bump1_TiltY, Bump1_TiltZ)));
        parts.Add((new Vector3(Bump2_X, Bump2_Y, Bump2_Z),
                   new Vector3(Bump2_W, Bump2_H, Bump2_D),
                   Quaternion.Euler(Bump2_TiltX, Bump2_TiltY, Bump2_TiltZ)));
        parts.Add((new Vector3(Bump3_X, Bump3_Y, Bump3_Z),
                   new Vector3(Bump3_W, Bump3_H, Bump3_D),
                   Quaternion.Euler(Bump3_TiltX, Bump3_TiltY, Bump3_TiltZ)));
        parts.Add((new Vector3(Bump4_X, Bump4_Y, Bump4_Z),
                   new Vector3(Bump4_W, Bump4_H, Bump4_D),
                   Quaternion.Euler(Bump4_TiltX, Bump4_TiltY, Bump4_TiltZ)));
        parts.Add((new Vector3(Bump5_X, Bump5_Y, Bump5_Z),
                   new Vector3(Bump5_W, Bump5_H, Bump5_D),
                   Quaternion.Euler(Bump5_TiltX, Bump5_TiltY, Bump5_TiltZ)));

        // ── Base rubble: low flat slabs at ground level ───────────────────────
        parts.Add((new Vector3(Rubble1_X, Rubble_Y, Rubble1_Z),
                   new Vector3(Rubble1_W, Rubble1_H, Rubble1_D),
                   Quaternion.Euler(0f, Rubble1_TiltY, 0f)));
        parts.Add((new Vector3(Rubble2_X, Rubble_Y, Rubble2_Z),
                   new Vector3(Rubble2_W, Rubble2_H, Rubble2_D),
                   Quaternion.Euler(0f, Rubble2_TiltY, 0f)));
        parts.Add((new Vector3(Rubble3_X, Rubble_Y, Rubble3_Z),
                   new Vector3(Rubble3_W, Rubble3_H, Rubble3_D),
                   Quaternion.Euler(0f, Rubble3_TiltY, 0f)));

        // ── Hieroglyph panel: raised rectangular relief on +Z face ────────────
        // Simulates carved cartouche on the facing side of the boulder.
        parts.Add((new Vector3(0f, Panel_CentreY, Panel_OffsetZ),
                   new Vector3(Panel_Width, Panel_Height, Panel_Depth),
                   Quaternion.identity));

        // Three horizontal bar marks (cartouche interior lines)
        parts.Add((new Vector3(0f, MarkTop_Y, Mark_OffsetZ),
                   new Vector3(Mark_Width1, Mark_Height, Mark_Depth),
                   Quaternion.identity));
        parts.Add((new Vector3(0f, MarkMid_Y, Mark_OffsetZ),
                   new Vector3(Mark_Width2, Mark_Height, Mark_Depth),
                   Quaternion.identity));
        parts.Add((new Vector3(0f, MarkBot_Y, Mark_OffsetZ),
                   new Vector3(Mark_Width1, Mark_Height, Mark_Depth),
                   Quaternion.identity));

        return CombineBoxes(parts);
    }

    private static Mesh CombineBoxes(List<(Vector3 centre, Vector3 size, Quaternion rot)> parts)
    {
        var verts   = new List<Vector3>();
        var normals = new List<Vector3>();
        var uvs     = new List<Vector2>();
        var tris    = new List<int>();

        foreach (var (centre, size, rot) in parts)
            AppendBox(verts, normals, uvs, tris, Matrix4x4.TRS(centre, rot, Vector3.one), size);

        var mesh = new Mesh { name = "ProceduralRock" };
        mesh.SetVertices(verts);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Material CreateFlatMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit")
                     ?? Shader.Find("Standard");
        var mat = new Material(shader) { color = RockColor };

        // Apply rock texture from Resources if available
        Texture2D tex = Resources.Load<Texture2D>("Egypt/rock_obstacle");
        if (tex != null) mat.mainTexture = tex;

        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0f);
        if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0f);
        if (mat.HasProperty("_Metallic"))   mat.SetFloat("_Metallic",   0f);
        return mat;
    }

    private static void AppendBox(
        List<Vector3> verts, List<Vector3> normals, List<Vector2> uvs,
        List<int> tris, Matrix4x4 mtx, Vector3 size)
    {
        float hx = size.x * 0.5f, hy = size.y * 0.5f, hz = size.z * 0.5f;

        var c = new Vector3[]
        {
            new Vector3(-hx, -hy, -hz), new Vector3( hx, -hy, -hz),
            new Vector3( hx,  hy, -hz), new Vector3(-hx,  hy, -hz),
            new Vector3(-hx, -hy,  hz), new Vector3( hx, -hy,  hz),
            new Vector3( hx,  hy,  hz), new Vector3(-hx,  hy,  hz),
        };

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

    private T GetOrAdd<T>() where T : Component
    {
        T comp = GetComponent<T>();
        return comp != null ? comp : gameObject.AddComponent<T>();
    }
}
