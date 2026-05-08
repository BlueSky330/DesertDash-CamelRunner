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

        // ── Main body: slightly squashed 2×2×2 boulder ────────────────────────
        parts.Add((new Vector3(0f, 1.0f, 0f),
                   new Vector3(2.0f, 1.8f, 1.8f),
                   Quaternion.identity));

        // ── Side lobes: overlap main body for irregular lumpy silhouette ───────
        parts.Add((new Vector3( 0.75f, 0.85f,  0.22f),
                   new Vector3(0.90f, 1.50f, 1.10f),
                   Quaternion.Euler(0f,  14f,  6f)));
        parts.Add((new Vector3(-0.75f, 0.90f, -0.15f),
                   new Vector3(0.85f, 1.40f, 1.20f),
                   Quaternion.Euler(0f, -10f, -5f)));
        parts.Add((new Vector3( 0.20f, 0.80f,  0.70f),
                   new Vector3(1.30f, 1.30f, 0.80f),
                   Quaternion.Euler( 4f,   6f,  0f)));
        parts.Add((new Vector3(-0.10f, 0.88f, -0.65f),
                   new Vector3(1.20f, 1.25f, 0.90f),
                   Quaternion.Euler(-3f,  -8f,  0f)));

        // ── Top cap: rougher upper surface capping the boulder ─────────────────
        parts.Add((new Vector3( 0.08f, 1.90f, -0.06f),
                   new Vector3(1.40f, 0.30f, 1.30f),
                   Quaternion.Euler(3f, 12f, 4f)));

        // ── Corner slabs: ground-contact reinforcement for stability ───────────
        parts.Add((new Vector3( 0.60f, 0.20f,  0.50f),
                   new Vector3(0.50f, 0.40f, 0.60f),
                   Quaternion.Euler(0f,  18f, 0f)));
        parts.Add((new Vector3(-0.60f, 0.20f,  0.50f),
                   new Vector3(0.50f, 0.40f, 0.60f),
                   Quaternion.Euler(0f, -18f, 0f)));
        parts.Add((new Vector3( 0.55f, 0.20f, -0.45f),
                   new Vector3(0.45f, 0.38f, 0.55f),
                   Quaternion.Euler(0f, -14f, 0f)));
        parts.Add((new Vector3(-0.55f, 0.20f, -0.45f),
                   new Vector3(0.45f, 0.38f, 0.55f),
                   Quaternion.Euler(0f,  14f, 0f)));

        // ── Surface bumps: small protrusions for rocky texture ─────────────────
        parts.Add((new Vector3( 0.45f, 1.70f,  0.35f),
                   new Vector3(0.40f, 0.28f, 0.36f),
                   Quaternion.Euler(-5f,  22f,  9f)));
        parts.Add((new Vector3(-0.35f, 1.55f, -0.40f),
                   new Vector3(0.36f, 0.24f, 0.44f),
                   Quaternion.Euler( 5f, -18f, -7f)));
        parts.Add((new Vector3( 0.10f, 1.78f,  0.55f),
                   new Vector3(0.44f, 0.24f, 0.32f),
                   Quaternion.Euler(-4f,  10f,  5f)));
        parts.Add((new Vector3(-0.55f, 1.42f,  0.30f),
                   new Vector3(0.32f, 0.20f, 0.40f),
                   Quaternion.Euler( 7f, -28f,  3f)));
        parts.Add((new Vector3( 0.30f, 1.60f, -0.50f),
                   new Vector3(0.36f, 0.22f, 0.28f),
                   Quaternion.Euler(-5f,  20f, -6f)));

        // ── Base rubble: low flat slabs at ground level ───────────────────────
        parts.Add((new Vector3( 0.85f, 0.07f,  0.30f),
                   new Vector3(0.55f, 0.13f, 0.70f),
                   Quaternion.Euler(0f, 22f, 0f)));
        parts.Add((new Vector3(-0.70f, 0.07f, -0.20f),
                   new Vector3(0.65f, 0.11f, 0.55f),
                   Quaternion.Euler(0f, -20f, 0f)));
        parts.Add((new Vector3( 0.10f, 0.07f,  0.80f),
                   new Vector3(0.90f, 0.10f, 0.44f),
                   Quaternion.Euler(0f,   6f, 0f)));

        // ── Hieroglyph panel: raised rectangular relief on +Z face ────────────
        // Simulates carved cartouche on the facing side of the boulder.
        parts.Add((new Vector3(0f, 1.0f, 0.92f),
                   new Vector3(0.60f, 0.80f, 0.08f),
                   Quaternion.identity));

        // Three horizontal bar marks (cartouche interior lines)
        parts.Add((new Vector3(0f, 1.22f, 0.965f),
                   new Vector3(0.44f, 0.07f, 0.025f),
                   Quaternion.identity));
        parts.Add((new Vector3(0f, 1.00f, 0.965f),
                   new Vector3(0.40f, 0.07f, 0.025f),
                   Quaternion.identity));
        parts.Add((new Vector3(0f, 0.78f, 0.965f),
                   new Vector3(0.44f, 0.07f, 0.025f),
                   Quaternion.identity));

        return CombineBoxes(parts);
    }

    private static Mesh CombineBoxes(List<(Vector3 centre, Vector3 size, Quaternion rot)> parts)
    {
        var verts   = new List<Vector3>();
        var normals = new List<Vector3>();
        var tris    = new List<int>();

        foreach (var (centre, size, rot) in parts)
            AppendBox(verts, normals, tris, Matrix4x4.TRS(centre, rot, Vector3.one), size);

        var mesh = new Mesh { name = "ProceduralRock" };
        mesh.SetVertices(verts);
        mesh.SetNormals(normals);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Material CreateFlatMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit")
                     ?? Shader.Find("Standard");
        var mat = new Material(shader) { color = RockColor };
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0f);
        if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0f);
        if (mat.HasProperty("_Metallic"))   mat.SetFloat("_Metallic",   0f);
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
