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
            new Vector3(0f, 0.15f * h, 0f),
            new Vector3(1.2f * w, 0.30f * h, 1.0f * w),
            Quaternion.identity));

        // ── Lower column block ────────────────────────────────────────────────
        parts.Add((
            new Vector3(0f, 0.60f * h, 0f),
            new Vector3(0.70f * w, 0.60f * h, 0.70f * w),
            Quaternion.identity));

        // ── Mid column block — slightly offset for broken look ────────────────
        parts.Add((
            new Vector3(0.05f * w, 1.10f * h, -0.04f * w),
            new Vector3(0.65f * w, 0.55f * h, 0.65f * w),
            Quaternion.Euler(0f, 3f, 0f)));

        // ── Upper broken stump — narrower, tilted ─────────────────────────────
        parts.Add((
            new Vector3(-0.06f * w, 1.60f * h, 0.03f * w),
            new Vector3(0.55f * w, 0.45f * h, 0.55f * w),
            Quaternion.Euler(1.5f, -5f, 1f)));

        // ── Broken top fragment — small tilted block at peak ──────────────────
        parts.Add((
            new Vector3(0.08f * w, 2.00f * h, -0.05f * w),
            new Vector3(0.40f * w, 0.20f * h, 0.40f * w),
            Quaternion.Euler(8f, 12f, 4f)));

        // ── Toppled side slab — fallen block leaning against base ────────────
        parts.Add((
            new Vector3(0.60f * w, 0.18f * h, 0.10f * w),
            new Vector3(0.80f * w, 0.14f * h, 0.55f * w),
            Quaternion.Euler(0f, 20f, 15f)));

        // ── Rubble piece 1 ────────────────────────────────────────────────────
        parts.Add((
            new Vector3(-0.50f * w, 0.08f * h, 0.20f * w),
            new Vector3(0.35f * w, 0.12f * h, 0.30f * w),
            Quaternion.Euler(0f, -25f, -8f)));

        // ── Rubble piece 2 ────────────────────────────────────────────────────
        parts.Add((
            new Vector3(0.30f * w, 0.07f * h, -0.45f * w),
            new Vector3(0.28f * w, 0.10f * h, 0.22f * w),
            Quaternion.Euler(0f, 40f, 5f)));

        // ── Decorative carved panel (hieroglyph slot on lower column) ─────────
        parts.Add((
            new Vector3(0f, 0.58f * h, 0.36f * w),
            new Vector3(0.42f * w, 0.36f * h, 0.04f * w),
            Quaternion.identity));

        // ── Carved mark 1 (raised symbol on panel) ───────────────────────────
        parts.Add((
            new Vector3(-0.08f * w, 0.62f * h, 0.385f * w),
            new Vector3(0.10f * w, 0.10f * h, 0.02f * w),
            Quaternion.identity));

        // ── Carved mark 2 ─────────────────────────────────────────────────────
        parts.Add((
            new Vector3(0.08f * w, 0.52f * h, 0.385f * w),
            new Vector3(0.08f * w, 0.08f * h, 0.02f * w),
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
