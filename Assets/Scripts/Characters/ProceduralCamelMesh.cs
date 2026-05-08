using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Procedurally generates a low-poly camel mesh using the Unity Mesh API.
/// No external model assets required — all geometry is defined in C#.
///
/// Produces a Crossy Road–style silhouette:
///   Body, hump, neck, head, 4 legs, tail — combined into a single flat-shaded mesh.
///
/// Usage:
///   Attach to a GameObject; mesh builds automatically on Awake (play mode) or
///   call Build() explicitly from an Editor script / inspector button.
///
/// Coordinate convention:
///   Feet at Y=0, camel faces +Z (forward). Units are Unity metres.
///
/// Accessory attachment points (auto-created child transforms):
///   saddleBlanketSlot  — above the hump
///   aviatorGogglesSlot — at the forehead
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public class ProceduralCamelMesh : MonoBehaviour
{
    // Sandy tan #C8A96E → R=0.784, G=0.663, B=0.431
    public static readonly Color CamelBaseColor = new Color(0.784f, 0.663f, 0.431f, 1f);

    [Header("Accessory Slots (auto-created by Build)")]
    public Transform saddleBlanketSlot;
    public Transform aviatorGogglesSlot;

    [Header("Material Override (null = create procedurally)")]
    public Material bodyMaterial;

    // Cached head-centre position so EnsureAccessorySlots() can place the goggle slot.
    private Vector3 _headCentre;

    // ──────────────────────────────────────────────────────────────────────────
    // Unity lifecycle
    // ──────────────────────────────────────────────────────────────────────────

    void Awake()
    {
        // Only auto-build at runtime; Editor rebuilds should go through Build().
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        Build();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Generate (or regenerate) the camel mesh and assign the material.
    /// Safe to call from Editor scripts, inspector buttons, and at runtime.
    /// </summary>
    public void Build()
    {
        var mf = GetOrAdd<MeshFilter>();
        var mr = GetOrAdd<MeshRenderer>();

        mf.sharedMesh = GenerateMesh();

        if (bodyMaterial == null)
            bodyMaterial = CreateFlatMaterial();
        mr.sharedMaterial = bodyMaterial;

        EnsureAccessorySlots();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Mesh generation
    // ──────────────────────────────────────────────────────────────────────────

    private Mesh GenerateMesh()
    {
        // Camel stands with feet at Y=0, facing +Z.
        //
        // Part proportions from spec (length × height × width):
        //   Body: 1.0 × 0.6 × 0.5   (Z × Y × X)
        //   Neck: 0.15 × 0.5 × 0.15  (X × Y × Z, then rotated)
        //   Head: 0.35 × 0.25 × 0.25
        //   Hump: 0.3 × 0.3 × 0.25
        //   Legs: 0.12 × 0.5 × 0.12  (each)
        //   Tail: 0.05 × 0.25 × 0.05

        const float legHeight = 0.5f;
        const float bodyHalfH = 0.3f;          // half of 0.6
        const float bodyHalfZ = 0.5f;          // half of 1.0 (front/rear extents)
        float bodyY = legHeight + bodyHalfH;   // 0.8 — body centre Y

        var parts = new List<(Vector3 centre, Vector3 size, Quaternion rot)>();

        // ── Body ──────────────────────────────────────────────────────────────
        parts.Add((
            new Vector3(0f, bodyY, 0f),
            new Vector3(0.5f, 0.6f, 1.0f),
            Quaternion.identity
        ));

        // ── Hump ─────────────────────────────────────────────────────────────
        // Sits atop the body, slightly toward rear (Z = -0.05)
        float bodyTopY = bodyY + bodyHalfH;   // 1.1
        parts.Add((
            new Vector3(0f, bodyTopY + 0.15f, -0.05f),
            new Vector3(0.25f, 0.3f, 0.3f),
            Quaternion.identity
        ));

        // ── Neck ─────────────────────────────────────────────────────────────
        // Base at upper-front of body face, angled 30° forward (Euler X = -30°)
        var neckRot  = Quaternion.Euler(-30f, 0f, 0f);
        var neckBase = new Vector3(0f, bodyY + 0.1f, bodyHalfZ);
        // Centre of neck box is half-way along the neck's local Y axis
        var neckCentre = neckBase + neckRot * new Vector3(0f, 0.25f, 0f);
        var neckTip    = neckBase + neckRot * new Vector3(0f, 0.5f,  0f);
        parts.Add((neckCentre, new Vector3(0.15f, 0.5f, 0.15f), neckRot));

        // ── Head ─────────────────────────────────────────────────────────────
        // Attached at neck tip, slight forward tilt
        var headRot = Quaternion.Euler(-10f, 0f, 0f);
        _headCentre = neckTip + headRot * new Vector3(0f, 0.125f, 0f);
        parts.Add((_headCentre, new Vector3(0.35f, 0.25f, 0.25f), headRot));

        // ── Front legs ───────────────────────────────────────────────────────
        // Leg centres: Y=0.25 so feet are at Y=0 and tops at Y=0.5 (body bottom)
        const float legXOff = 0.16f;
        const float legZOff = 0.32f;
        var legSize = new Vector3(0.12f, legHeight, 0.12f);
        parts.Add((new Vector3(-legXOff, 0.25f,  legZOff), legSize, Quaternion.identity));
        parts.Add((new Vector3( legXOff, 0.25f,  legZOff), legSize, Quaternion.identity));

        // ── Rear legs ────────────────────────────────────────────────────────
        parts.Add((new Vector3(-legXOff, 0.25f, -legZOff), legSize, Quaternion.identity));
        parts.Add((new Vector3( legXOff, 0.25f, -legZOff), legSize, Quaternion.identity));

        // ── Tail ─────────────────────────────────────────────────────────────
        // Base at rear face of body, angles backward-downward (Euler X = +25°)
        var tailBase = new Vector3(0f, bodyY - 0.05f, -bodyHalfZ);
        var tailRot  = Quaternion.Euler(25f, 0f, 0f);
        var tailCentre = tailBase + tailRot * new Vector3(0f, -0.125f, 0f);
        parts.Add((tailCentre, new Vector3(0.05f, 0.25f, 0.05f), tailRot));

        return CombineBoxes(parts);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Mesh utilities
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Combine a list of (centre, size, rotation) box descriptors into one
    /// flat-shaded mesh. Each face has its own split vertices so normals are
    /// constant across the face (Crossy Road aesthetic).
    /// </summary>
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

        var mesh = new Mesh { name = "ProceduralCamel" };
        mesh.SetVertices(allVerts);
        mesh.SetNormals(allNormals);
        mesh.SetUVs(0, allUVs);
        mesh.SetTriangles(allTris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    /// <summary>
    /// Append 24 flat-shaded vertices (4 per face × 6 faces) for one box.
    /// UV coordinates use planar projection per face for texture support.
    /// Winding: counter-clockwise when viewed from outside each face (Unity default).
    /// </summary>
    private static void AppendBox(
        List<Vector3> verts, List<Vector3> normals, List<Vector2> uvs,
        List<int> tris, Matrix4x4 mtx, Vector3 size)
    {
        float hx = size.x * 0.5f;
        float hy = size.y * 0.5f;
        float hz = size.z * 0.5f;

        var c = new Vector3[]
        {
            new Vector3(-hx, -hy, -hz), // 0 left-bottom-back
            new Vector3( hx, -hy, -hz), // 1 right-bottom-back
            new Vector3( hx,  hy, -hz), // 2 right-top-back
            new Vector3(-hx,  hy, -hz), // 3 left-top-back
            new Vector3(-hx, -hy,  hz), // 4 left-bottom-front
            new Vector3( hx, -hy,  hz), // 5 right-bottom-front
            new Vector3( hx,  hy,  hz), // 6 right-top-front
            new Vector3(-hx,  hy,  hz), // 7 left-top-front
        };

        // Each face: normal, 4 corner indices wound CCW from outside, UV corners
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

    // ──────────────────────────────────────────────────────────────────────────
    // Material
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Create a matte flat-shaded material in sandy tan (#C8A96E).
    /// Tries URP Lit first, falls back to Standard.
    /// </summary>
    public static Material CreateFlatMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit")
                     ?? Shader.Find("Standard");

        var mat = new Material(shader) { color = CamelBaseColor };

        // Apply camel texture from Resources if available
        Texture2D tex = Resources.Load<Texture2D>("Egypt/camel_idle");
        if (tex != null) mat.mainTexture = tex;

        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0f);
        if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0f);
        if (mat.HasProperty("_Metallic"))   mat.SetFloat("_Metallic",   0f);
        return mat;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Accessory slots
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>Ensure both accessory attachment-point child transforms exist.</summary>
    private void EnsureAccessorySlots()
    {
        // Saddle blanket: centred above body/hump
        if (saddleBlanketSlot == null)
            saddleBlanketSlot = FindOrCreateSlot("[Slot] SaddleBlanket",
                new Vector3(0f, 1.3f, -0.05f));

        // Aviator goggles: at the camel's forehead
        if (aviatorGogglesSlot == null)
            aviatorGogglesSlot = FindOrCreateSlot("[Slot] AviatorGoggles",
                _headCentre + new Vector3(0f, 0.1f, 0.1f));
    }

    private Transform FindOrCreateSlot(string slotName, Vector3 localPos)
    {
        // Reuse existing child on repeated Build() calls
        Transform existing = transform.Find(slotName);
        if (existing != null) return existing;

        var go = new GameObject(slotName);
        go.transform.SetParent(transform, worldPositionStays: false);
        go.transform.localPosition = localPos;
        return go.transform;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────────

    private T GetOrAdd<T>() where T : Component
    {
        T comp = GetComponent<T>();
        return comp != null ? comp : gameObject.AddComponent<T>();
    }
}
