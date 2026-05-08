using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Procedurally generates a low-poly Egyptian desert palm tree obstacle mesh.
/// Follows the pattern established by ProceduralCamelMesh (AIG-173).
///
/// Two submeshes:
///   SubMesh 0 (Trunk)  — warm brown  #8B5E3C  R=0.545 G=0.369 B=0.235
///   SubMesh 1 (Fronds) — desert green #5B8C5A  R=0.357 G=0.549 B=0.353
///
/// Geometry budget (< 600 tri target):
///   9 trunk segments    × 12 tris = 108
///   8 bark rings        × 12 tris =  96
///   8 fronds            × 12 tris =  96
///   8 frond tips        × 12 tris =  96
///   3 coconut clusters  × 12 tris =  36
///   Total                         = 432 tris
///
/// Dimensions: height 4.5 units, trunk base 0.5 units, frond spread ~3.5 unit radius.
///
/// Usage:
///   Attach to a GameObject; mesh builds on Awake (play mode) or call Build() from Editor.
///   FBX export target: Assets/Models/Egypt/Props/palm_tree.fbx
///
/// Coordinate convention:
///   Base at Y=0, trunk grows upward (+Y). Obstacle faces +Z. Units are Unity metres.
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public class ProceduralPalmTreeMesh : MonoBehaviour
{
    // Warm brown trunk #8B5E3C
    public static readonly Color TrunkColor  = new Color(0.545f, 0.369f, 0.235f, 1f);
    // Muted desert green #5B8C5A
    public static readonly Color FrondColor  = new Color(0.357f, 0.549f, 0.353f, 1f);

    private const int SM_TRUNK = 0;
    private const int SM_FROND = 1;
    private const int SM_COUNT = 2;

    [Header("Material Overrides (null = created procedurally)")]
    public Material matTrunk;
    public Material matFrond;

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

        if (matTrunk == null) matTrunk = CreateFlatMaterial(TrunkColor, metallic: 0f, smoothness: 0f);
        if (matFrond == null) matFrond = CreateFlatMaterial(FrondColor, metallic: 0f, smoothness: 0f);

        mr.sharedMaterials = new[] { matTrunk, matFrond };
    }

    private Mesh GenerateMesh()
    {
        var trunkVerts   = new List<Vector3>();
        var trunkNormals = new List<Vector3>();
        var trunkTris    = new List<int>();

        var frondVerts   = new List<Vector3>();
        var frondNormals = new List<Vector3>();
        var frondTris    = new List<int>();

        // ── Trunk: 9 tapered segments, Y=0 → Y=4.5 ───────────────────────────
        // Width tapers from 0.5 at base to 0.16 at crown.
        float[] segWidths = { 0.50f, 0.44f, 0.38f, 0.33f, 0.28f, 0.24f, 0.20f, 0.18f, 0.16f };
        const float segH  = 0.5f;

        for (int i = 0; i < segWidths.Length; i++)
        {
            float cy = (i + 0.5f) * segH;
            float w  = segWidths[i];
            var mtx  = Matrix4x4.TRS(new Vector3(0f, cy, 0f), Quaternion.identity, Vector3.one);
            AppendBox(trunkVerts, trunkNormals, trunkTris, mtx, new Vector3(w, segH, w));
        }

        // ── Bark rings: thin bands at each segment boundary ───────────────────
        // 8 rings between segments (Y=0.5 … Y=4.0).
        float[] ringYs = { 0.5f, 1.0f, 1.5f, 2.0f, 2.5f, 3.0f, 3.5f, 4.0f };
        foreach (float ry in ringYs)
        {
            int idx = Mathf.Clamp(Mathf.FloorToInt(ry / segH), 0, segWidths.Length - 1);
            float rw = segWidths[idx] + 0.04f;
            var mtx  = Matrix4x4.TRS(new Vector3(0f, ry, 0f), Quaternion.identity, Vector3.one);
            AppendBox(trunkVerts, trunkNormals, trunkTris, mtx, new Vector3(rw, 0.07f, rw));
        }

        // ── Fronds: 8 leaves fan out from crown at Y=4.5 ─────────────────────
        // Alternate between longer (even index) and slightly shorter drooping fronds.
        const float crownY    = 4.5f;
        const float frondLen  = 1.8f;   // main segment length → ~3.5 unit spread at tip
        const float frondW    = 0.18f;
        const float frondH    = 0.07f;
        const float tipLen    = 0.90f;
        const float tipW      = 0.09f;

        for (int f = 0; f < 8; f++)
        {
            float azimuth  = f * 45f;
            float dropAngle = (f % 2 == 0) ? 28f : 38f;   // alternating droop

            // Frond main segment: rotate outward (Y) then droop (X)
            var rot    = Quaternion.Euler(-dropAngle, azimuth, 0f);
            Vector3 segCentre = new Vector3(0f, crownY, 0f) + rot * new Vector3(0f, 0f, frondLen * 0.5f);
            AppendBox(frondVerts, frondNormals, frondTris,
                      Matrix4x4.TRS(segCentre, rot, Vector3.one),
                      new Vector3(frondW, frondH, frondLen));

            // Frond tip: extends from end of main segment, droops more steeply
            var tipRot     = Quaternion.Euler(-(dropAngle + 22f), azimuth, 0f);
            Vector3 segEnd = new Vector3(0f, crownY, 0f) + rot * new Vector3(0f, 0f, frondLen);
            Vector3 tipCentre = segEnd + tipRot * new Vector3(0f, 0f, tipLen * 0.5f);
            AppendBox(frondVerts, frondNormals, frondTris,
                      Matrix4x4.TRS(tipCentre, tipRot, Vector3.one),
                      new Vector3(tipW, frondH * 0.65f, tipLen));
        }

        // ── Coconuts: 3 small dark spheroids clustered at crown ───────────────
        var coconutOffsets = new Vector3[]
        {
            new Vector3( 0.09f, crownY - 0.10f,  0.07f),
            new Vector3(-0.09f, crownY - 0.16f, -0.05f),
            new Vector3( 0.02f, crownY - 0.06f, -0.11f),
        };
        // Coconuts use trunk submesh (dark brown appearance from trunk material)
        foreach (var cOff in coconutOffsets)
        {
            AppendBox(trunkVerts, trunkNormals, trunkTris,
                      Matrix4x4.TRS(cOff, Quaternion.identity, Vector3.one),
                      new Vector3(0.18f, 0.20f, 0.18f));
        }

        // ── Combine into mesh with two submeshes ──────────────────────────────
        var allVerts   = new List<Vector3>(trunkVerts.Count + frondVerts.Count);
        var allNormals = new List<Vector3>(trunkNormals.Count + frondNormals.Count);
        allVerts.AddRange(trunkVerts);   allVerts.AddRange(frondVerts);
        allNormals.AddRange(trunkNormals); allNormals.AddRange(frondNormals);

        int trunkVertCount = trunkVerts.Count;
        var offsetFrondTris = new List<int>(frondTris.Count);
        foreach (int t in frondTris) offsetFrondTris.Add(t + trunkVertCount);

        var mesh = new Mesh { name = "ProceduralPalmTree" };
        mesh.SetVertices(allVerts);
        mesh.SetNormals(allNormals);
        mesh.subMeshCount = SM_COUNT;
        mesh.SetTriangles(trunkTris, SM_TRUNK);
        mesh.SetTriangles(offsetFrondTris, SM_FROND);
        mesh.RecalculateBounds();
        return mesh;
    }

    // ── Shared utility: flat-shaded material ──────────────────────────────────

    public static Material CreateFlatMaterial(Color color, float metallic = 0f, float smoothness = 0f)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit")
                     ?? Shader.Find("Standard");
        var mat = new Material(shader) { color = color };
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
        if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", smoothness);
        if (mat.HasProperty("_Metallic"))   mat.SetFloat("_Metallic",   metallic);
        return mat;
    }

    // ── Shared utility: append one flat-shaded box to vertex/normal/tri lists ─

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
