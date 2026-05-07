using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Procedurally generates a low-poly ShadowThief character mesh using the Unity Mesh API.
/// Follows the pattern established by ProceduralNinjaThiefMesh (AIG-187) and ProceduralCamelMesh (AIG-173).
///
/// Produces a slim dark humanoid silhouette (~1.83 units tall) with a ghostly smoky bottom half:
///   Head (dark block), torso, slim arms — skinned to a 10-bone skeleton.
///   Bottom legs replaced by tapered "smoke trail" pyramids fading downward.
///
/// Color specification (per AIG-198 ShadowThief Design Spec):
///   Body / outfit  #1A1A2E  very dark navy-gray  — SubMesh 0
///   Eyes           #00FFFF  glowing cyan         — SubMesh 1 (emissive)
///   Cape           #16213E  dark navy, 70% alpha — SubMesh 2 (transparent)
///
/// Usage:
///   Attach to a GameObject; mesh builds automatically on Awake (play mode) or
///   call Build() explicitly from an Editor script (e.g. ShadowThiefPrefabGenerator).
///
/// Coordinate convention:
///   Smoke tip at Y=0, character faces +Z (forward). Units are Unity metres.
///
/// Geometry budget:
///   Pelvis box                    × 12 tris =  12
///   Chest box                     × 12 tris =  12
///   Head box (sphere approx)      × 12 tris =  12
///   Arms (4 boxes)               × 12 tris =  48
///   Eye disks (2 × 8 segs)        × 8 tris  =  16
///   Smoke pyramids (4 × 4 tris)   × 4 tris  =  16
///   Cape box                      × 12 tris =  12
///   Total                                    ≈ 128 tris  (well within the &lt;1 200 target)
///
/// Skeleton (10 bones, standard thief hierarchy):
///   Root → Pelvis → Spine → Chest → Neck → Head
///                              └─ L_Arm
///                              └─ R_Arm
///          └─ L_Leg  (drives left smoke trail)
///          └─ R_Leg  (drives right smoke trail)
///
/// Child slots (created on Build):
///   glowSlot — empty transform near eyes, for attaching particle/glow effects
///   capeSlot — empty transform at cape origin, for runtime cape swaps
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public class ProceduralShadowThiefMesh : MonoBehaviour
{
    // ── Color constants ────────────────────────────────────────────────────────

    /// <summary>#1A1A2E — very dark navy-gray body.</summary>
    public static readonly Color ColorBody = new Color(0.102f, 0.102f, 0.180f, 1f);

    /// <summary>#00FFFF — glowing cyan eyes.</summary>
    public static readonly Color ColorEyes = new Color(0f, 1f, 1f, 1f);

    /// <summary>#16213E at 70% alpha — semi-transparent dark cape.</summary>
    public static readonly Color ColorCape = new Color(0.086f, 0.129f, 0.243f, 0.7f);

    // ── SubMesh indices ────────────────────────────────────────────────────────

    private const int SM_BODY  = 0;
    private const int SM_EYES  = 1;
    private const int SM_CAPE  = 2;
    private const int SM_COUNT = 3;

    // ── Bone indices ───────────────────────────────────────────────────────────

    private const int BONE_ROOT   = 0;
    private const int BONE_PELVIS = 1;
    private const int BONE_SPINE  = 2;
    private const int BONE_CHEST  = 3;
    private const int BONE_HEAD   = 4;
    private const int BONE_L_ARM  = 5;
    private const int BONE_R_ARM  = 6;
    private const int BONE_L_LEG  = 7;   // drives left smoke trail
    private const int BONE_R_LEG  = 8;   // drives right smoke trail
    private const int BONE_NECK   = 9;
    private const int BONE_COUNT  = 10;

    // ── Inspector fields ───────────────────────────────────────────────────────

    [Header("Material Overrides (null = created procedurally)")]
    public Material matBody;
    public Material matEyes;
    public Material matCape;

    /// <summary>Bone transforms populated by BuildSkeleton(); exposed for external animation rigs.</summary>
    [HideInInspector] public Transform[] Bones;

    /// <summary>Empty transform near eyes — attach glow/particle effects here.</summary>
    [HideInInspector] public Transform glowSlot;

    /// <summary>Empty transform at cape origin — attach or swap cape effects here.</summary>
    [HideInInspector] public Transform capeSlot;

    // ──────────────────────────────────────────────────────────────────────────
    // Unity lifecycle
    // ──────────────────────────────────────────────────────────────────────────

    void Awake()
    {
        // Auto-build at runtime; Editor builds are driven by ShadowThiefPrefabGenerator.
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        Build();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Generate (or regenerate) the ShadowThief mesh, 10-bone skeleton, slot transforms, and materials.
    /// Safe to call from Editor scripts, inspector buttons, and at runtime.
    /// </summary>
    public void Build()
    {
        Bones = BuildSkeleton();

        // Named child slots for runtime effect attachment.
        // Positioned at head-front (glow) and cape-center (cape).
        glowSlot = GetOrCreateSlot("glowSlot", new Vector3(0f,  1.67f,  0.22f));
        capeSlot = GetOrCreateSlot("capeSlot", new Vector3(0f,  1.05f, -0.145f));

        var smr             = GetOrAdd<SkinnedMeshRenderer>();
        smr.sharedMesh      = GenerateMesh(Bones);
        smr.bones           = Bones;
        smr.rootBone        = Bones[BONE_ROOT];
        smr.sharedMaterials = BuildMaterials();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Skeleton construction
    // ──────────────────────────────────────────────────────────────────────────

    private Transform[] BuildSkeleton()
    {
        // Bone positions defined in T-pose world space (smoke tip at Y=0, facing +Z).
        // Parent index -1 means the bone's parent is the mesh root transform.
        //
        // Hierarchy:
        //   Root(0) → Pelvis(1) → Spine(2) → Chest(3) → Neck(9) → Head(4)
        //                                           └─ L_Arm(5)
        //                                           └─ R_Arm(6)
        //              └─ L_Leg(7)  [smoke trail]
        //              └─ R_Leg(8)  [smoke trail]
        var boneData = new (string name, Vector3 pos, int parent)[]
        {
            /* 0 ROOT   */ ("Root",   new Vector3( 0.00f, 0.00f, 0f), -1),
            /* 1 PELVIS */ ("Pelvis", new Vector3( 0.00f, 0.55f, 0f),  0),
            /* 2 SPINE  */ ("Spine",  new Vector3( 0.00f, 0.78f, 0f),  1),
            /* 3 CHEST  */ ("Chest",  new Vector3( 0.00f, 1.00f, 0f),  2),
            /* 4 HEAD   */ ("Head",   new Vector3( 0.00f, 1.45f, 0f),  9),  // child of Neck
            /* 5 L_ARM  */ ("L_Arm",  new Vector3(-0.24f, 1.05f, 0f),  3),
            /* 6 R_ARM  */ ("R_Arm",  new Vector3( 0.24f, 1.05f, 0f),  3),
            /* 7 L_LEG  */ ("L_Leg",  new Vector3(-0.08f, 0.55f, 0f),  1),
            /* 8 R_LEG  */ ("R_Leg",  new Vector3( 0.08f, 0.55f, 0f),  1),
            /* 9 NECK   */ ("Neck",   new Vector3( 0.00f, 1.25f, 0f),  3),
        };

        // Remove previously generated bone children to allow safe re-build.
        var toDestroy = new List<Transform>();
        foreach (Transform child in transform)
            if (child.name.StartsWith("[Bone]"))
                toDestroy.Add(child);
        foreach (var t in toDestroy)
        {
            if (Application.isPlaying) Destroy(t.gameObject);
            else Object.DestroyImmediate(t.gameObject);
        }

        // Create bone transforms at their T-pose world positions.
        var bones = new Transform[BONE_COUNT];
        for (int i = 0; i < BONE_COUNT; i++)
        {
            var go = new GameObject($"[Bone] {boneData[i].name}");
            go.transform.position = boneData[i].pos;
            bones[i] = go.transform;
        }

        // Wire up the parent hierarchy.
        for (int i = 0; i < BONE_COUNT; i++)
        {
            int p = boneData[i].parent;
            bones[i].SetParent(p < 0 ? transform : bones[p], worldPositionStays: true);
        }

        return bones;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Mesh generation
    // ──────────────────────────────────────────────────────────────────────────

    private Mesh GenerateMesh(Transform[] bones)
    {
        var verts   = new List<Vector3>();
        var normals = new List<Vector3>();
        var tris    = new List<int>[SM_COUNT];
        var weights = new List<BoneWeight>();

        for (int i = 0; i < SM_COUNT; i++) tris[i] = new List<int>();

        // Local helper — adds a box (all 24 verts weighted 100% to boneIdx) into submesh sm.
        void Box(Vector3 c, Vector3 size, int boneIdx, int sm, Quaternion? rot = null)
        {
            AppendBox(verts, normals, tris[sm], weights,
                      Matrix4x4.TRS(c, rot ?? Quaternion.identity, Vector3.one),
                      size, boneIdx);
        }

        // Local helper — adds a flat polygon disk facing +Z into submesh sm.
        void Disk(Vector3 c, float radius, int segs, int boneIdx, int sm)
        {
            AppendDisk(verts, normals, tris[sm], weights, c, radius, segs, boneIdx);
        }

        // Local helper — adds a 4-sided smoke pyramid (base at top, tip pointing down) into sm.
        void SmokePyramid(Vector3 baseCenter, float baseHW, float baseHD, Vector3 tip, int boneIdx, int sm)
        {
            AppendSmokePyramid(verts, normals, tris[sm], weights,
                               baseCenter, baseHW, baseHD, tip, boneIdx);
        }

        // ── Lower torso / pelvis (Y: 0.55 → 0.85) ─────────────────────────────
        Box(new Vector3(0f, 0.70f, 0f), new Vector3(0.34f, 0.30f, 0.20f), BONE_PELVIS, SM_BODY);

        // ── Upper torso / chest (Y: 0.85 → 1.45) ─────────────────────────────
        // Spec: torso ~0.38 wide × 0.90 tall × 0.25 deep (slim, ghostly silhouette).
        Box(new Vector3(0f, 1.15f, 0f), new Vector3(0.38f, 0.60f, 0.25f), BONE_CHEST, SM_BODY);

        // ── Head — approximate sphere r≈0.19 as flat-shaded cube 0.38³ ────────
        // Center at Y=1.64 (torso top 1.45 + radius 0.19).
        Box(new Vector3(0f, 1.64f, 0f), new Vector3(0.38f, 0.38f, 0.38f), BONE_HEAD, SM_BODY);

        // ── Left arm ──────────────────────────────────────────────────────────
        Box(new Vector3(-0.24f, 1.10f, 0f), new Vector3(0.09f, 0.22f, 0.09f), BONE_L_ARM, SM_BODY); // upper
        Box(new Vector3(-0.24f, 0.85f, 0f), new Vector3(0.08f, 0.20f, 0.08f), BONE_L_ARM, SM_BODY); // forearm

        // ── Right arm ─────────────────────────────────────────────────────────
        Box(new Vector3( 0.24f, 1.10f, 0f), new Vector3(0.09f, 0.22f, 0.09f), BONE_R_ARM, SM_BODY);
        Box(new Vector3( 0.24f, 0.85f, 0f), new Vector3(0.08f, 0.20f, 0.08f), BONE_R_ARM, SM_BODY);

        // ── Glowing cyan eyes — disks on front face of head ───────────────────
        // Head front face at Z = 0 + 0.19; disks at Z = 0.22 to avoid z-fighting.
        Disk(new Vector3(-0.08f, 1.67f, 0.22f), 0.060f, 8, BONE_HEAD, SM_EYES);
        Disk(new Vector3( 0.08f, 1.67f, 0.22f), 0.060f, 8, BONE_HEAD, SM_EYES);

        // ── Smoke trail geometry (replace legs) ───────────────────────────────
        // Left: primary plume — wide base at hip, tapering to tip at Y=0.
        SmokePyramid(
            new Vector3(-0.08f, 0.55f, 0f), 0.07f, 0.07f,
            new Vector3(-0.03f, 0.00f, 0f),
            BONE_L_LEG, SM_BODY);
        // Left: secondary plume — narrower, slightly offset for layered wispy look.
        SmokePyramid(
            new Vector3(-0.05f, 0.35f, 0.01f), 0.045f, 0.045f,
            new Vector3(-0.02f, 0.00f, 0f),
            BONE_L_LEG, SM_BODY);

        // Right: primary plume.
        SmokePyramid(
            new Vector3( 0.08f, 0.55f, 0f), 0.07f, 0.07f,
            new Vector3( 0.03f, 0.00f, 0f),
            BONE_R_LEG, SM_BODY);
        // Right: secondary plume.
        SmokePyramid(
            new Vector3( 0.05f, 0.35f, 0.01f), 0.045f, 0.045f,
            new Vector3( 0.02f, 0.00f, 0f),
            BONE_R_LEG, SM_BODY);

        // ── Cape — semi-transparent panel behind torso ─────────────────────────
        // Thin box behind the chest (Z negative = back). Uses SM_CAPE for alpha material.
        Box(new Vector3(0f, 1.05f, -0.145f), new Vector3(0.40f, 0.70f, 0.03f), BONE_CHEST, SM_CAPE);

        // ── Assemble mesh ──────────────────────────────────────────────────────
        var mesh = new Mesh { name = "ProceduralShadowThief" };
        mesh.SetVertices(verts);
        mesh.SetNormals(normals);
        mesh.subMeshCount = SM_COUNT;
        for (int i = 0; i < SM_COUNT; i++)
            mesh.SetTriangles(tris[i], i);

        // Skin weights: 100% to a single bone per vertex group (hard-painted).
        mesh.boneWeights = weights.ToArray();

        // Bind poses: each bone's inverse world transform combined with the mesh root's
        // local→world matrix. Required so Unity deforms vertices correctly at runtime.
        var bindPoses = new Matrix4x4[BONE_COUNT];
        for (int i = 0; i < BONE_COUNT; i++)
            bindPoses[i] = bones[i].worldToLocalMatrix * transform.localToWorldMatrix;
        mesh.bindposes = bindPoses;

        mesh.RecalculateBounds();
        return mesh;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Geometry helpers
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Appends one flat-shaded box (24 split vertices, 12 triangles) into the running lists.
    /// All 24 vertices receive a 100% BoneWeight to <paramref name="boneIdx"/>.
    /// Winding: CCW from outside each face (Unity default, matching ProceduralCamelMesh).
    /// </summary>
    private static void AppendBox(
        List<Vector3> verts, List<Vector3> normals, List<int> tris, List<BoneWeight> weights,
        Matrix4x4 mtx, Vector3 size, int boneIdx)
    {
        float hx = size.x * 0.5f, hy = size.y * 0.5f, hz = size.z * 0.5f;

        // 8 local-space corners.
        var c = new Vector3[]
        {
            new Vector3(-hx, -hy, -hz), // 0 LBB
            new Vector3( hx, -hy, -hz), // 1 RBB
            new Vector3( hx,  hy, -hz), // 2 RTB
            new Vector3(-hx,  hy, -hz), // 3 LTB
            new Vector3(-hx, -hy,  hz), // 4 LBF
            new Vector3( hx, -hy,  hz), // 5 RBF
            new Vector3( hx,  hy,  hz), // 6 RTF
            new Vector3(-hx,  hy,  hz), // 7 LTF
        };

        // Six faces: (outward local normal, four quad corners wound CCW from outside).
        var faces = new (Vector3 n, int a, int b, int c2, int d)[]
        {
            (Vector3.up,      3, 7, 6, 2), // +Y top
            (Vector3.down,    0, 1, 5, 4), // -Y bottom
            (Vector3.forward, 4, 5, 6, 7), // +Z front
            (Vector3.back,    1, 0, 3, 2), // -Z back
            (Vector3.right,   5, 1, 2, 6), // +X right
            (Vector3.left,    0, 4, 7, 3), // -X left
        };

        BoneWeight bw = MakeBoneWeight(boneIdx);

        foreach (var (n, a, b, c2, d) in faces)
        {
            int faceBase = verts.Count;
            Vector3 wn   = mtx.MultiplyVector(n).normalized;

            verts.Add(mtx.MultiplyPoint3x4(c[a]));
            verts.Add(mtx.MultiplyPoint3x4(c[b]));
            verts.Add(mtx.MultiplyPoint3x4(c[c2]));
            verts.Add(mtx.MultiplyPoint3x4(c[d]));

            normals.Add(wn); normals.Add(wn); normals.Add(wn); normals.Add(wn);
            weights.Add(bw); weights.Add(bw); weights.Add(bw); weights.Add(bw);

            // Two triangles from the quad (CCW: 0→1→2 and 0→2→3).
            tris.Add(faceBase);     tris.Add(faceBase + 1); tris.Add(faceBase + 2);
            tris.Add(faceBase);     tris.Add(faceBase + 2); tris.Add(faceBase + 3);
        }
    }

    /// <summary>
    /// Appends a filled polygon disk (fan of <paramref name="segments"/> triangles) facing +Z.
    /// All vertices receive a 100% BoneWeight to <paramref name="boneIdx"/>.
    /// The disk lies in the XY plane at <paramref name="centre"/>; normal = Vector3.forward.
    /// </summary>
    private static void AppendDisk(
        List<Vector3> verts, List<Vector3> normals, List<int> tris, List<BoneWeight> weights,
        Vector3 centre, float radius, int segments, int boneIdx)
    {
        int baseVert  = verts.Count;
        BoneWeight bw = MakeBoneWeight(boneIdx);

        // Centre vertex.
        verts.Add(centre);
        normals.Add(Vector3.forward);
        weights.Add(bw);

        // Ring vertices going CCW (increasing angle) → normal points +Z.
        for (int i = 0; i < segments; i++)
        {
            float angle = (i / (float)segments) * Mathf.PI * 2f;
            verts.Add(centre + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f));
            normals.Add(Vector3.forward);
            weights.Add(bw);
        }

        // Fan triangles: (centre, ring[i], ring[i+1]) — CCW from +Z → face visible from front.
        for (int i = 0; i < segments; i++)
        {
            int curr = baseVert + 1 + i;
            int next = baseVert + 1 + (i + 1) % segments;
            tris.Add(baseVert); // centre
            tris.Add(curr);
            tris.Add(next);
        }
    }

    /// <summary>
    /// Appends a 4-sided smoke pyramid: square base at <paramref name="baseCenter"/> pointing down
    /// to <paramref name="tip"/>. Produces 4 flat-shaded triangular faces (12 vertices, 4 triangles).
    /// Geometry represents the ghostly smoke trail replacing the ShadowThief's legs.
    /// All vertices receive a 100% BoneWeight to <paramref name="boneIdx"/>.
    /// </summary>
    private static void AppendSmokePyramid(
        List<Vector3> verts, List<Vector3> normals, List<int> tris, List<BoneWeight> weights,
        Vector3 baseCenter, float baseHalfWidth, float baseHalfDepth, Vector3 tip, int boneIdx)
    {
        BoneWeight bw = MakeBoneWeight(boneIdx);

        // 4 square base corners at baseCenter's Y level.
        Vector3 bl = baseCenter + new Vector3(-baseHalfWidth, 0f, -baseHalfDepth); // back-left
        Vector3 br = baseCenter + new Vector3( baseHalfWidth, 0f, -baseHalfDepth); // back-right
        Vector3 fl = baseCenter + new Vector3(-baseHalfWidth, 0f,  baseHalfDepth); // front-left
        Vector3 fr = baseCenter + new Vector3( baseHalfWidth, 0f,  baseHalfDepth); // front-right

        // Emit one flat-shaded triangle: 3 unique split vertices, face normal from cross product.
        void Tri(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            int b = verts.Count;
            Vector3 n = Vector3.Cross(v1 - v0, v2 - v0).normalized;
            verts.Add(v0); verts.Add(v1); verts.Add(v2);
            normals.Add(n); normals.Add(n); normals.Add(n);
            weights.Add(bw); weights.Add(bw); weights.Add(bw);
            tris.Add(b); tris.Add(b + 1); tris.Add(b + 2);
        }

        // 4 triangular side faces, wound CCW from outside (tip at bottom → base at top).
        Tri(fl, fr, tip); // front  face
        Tri(fr, br, tip); // right  face
        Tri(br, bl, tip); // back   face
        Tri(bl, fl, tip); // left   face
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Materials
    // ──────────────────────────────────────────────────────────────────────────

    private Material[] BuildMaterials()
    {
        if (matBody == null) matBody = CreateMat(ColorBody, 0f,   0f);
        if (matEyes == null) matEyes = CreateMatEmissive(ColorEyes);
        if (matCape == null) matCape = CreateMatTransparent(ColorCape);
        return new[] { matBody, matEyes, matCape };
    }

    /// <summary>Creates a URP Lit/Standard material with the given albedo, metallic and smoothness.</summary>
    public static Material CreateMat(Color albedo, float metallic, float smoothness)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit")
                     ?? Shader.Find("Standard");
        var mat = new Material(shader) { color = albedo };
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
        if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", smoothness);
        if (mat.HasProperty("_Metallic"))   mat.SetFloat("_Metallic",   metallic);
        return mat;
    }

    /// <summary>
    /// Creates a URP Lit material with emissive glow for the glowing cyan eyes.
    /// Sets emission color to <paramref name="albedo"/> for a self-illuminated appearance.
    /// </summary>
    public static Material CreateMatEmissive(Color albedo)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit")
                     ?? Shader.Find("Standard");
        var mat = new Material(shader) { color = albedo };
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0.8f);
        if (mat.HasProperty("_Metallic"))   mat.SetFloat("_Metallic",   0f);

        // Enable emission on the material.
        mat.EnableKeyword("_EMISSION");
        if (mat.HasProperty("_EmissionColor"))
            mat.SetColor("_EmissionColor", albedo);

        return mat;
    }

    /// <summary>
    /// Creates a URP Lit material with alpha blending for the semi-transparent cape.
    /// Sets surface type to Transparent so the alpha channel is respected.
    /// </summary>
    public static Material CreateMatTransparent(Color albedo)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit")
                     ?? Shader.Find("Standard");
        var mat = new Material(shader) { color = albedo };

        // URP: surface type 1 = Transparent; blend mode Alpha.
        if (mat.HasProperty("_Surface"))
        {
            mat.SetFloat("_Surface", 1f);          // Transparent
            mat.SetFloat("_Blend",   0f);           // Alpha blend
            mat.SetInt("_SrcBlend",  (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend",  (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite",    0);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
        else
        {
            // Standard shader fallback: Fade mode.
            mat.SetFloat("_Mode", 2f); // Fade
            mat.SetInt("_SrcBlend",  (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend",  (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite",    0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }

        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0.2f);
        if (mat.HasProperty("_Metallic"))   mat.SetFloat("_Metallic",   0f);

        return mat;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Utilities
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>Returns a BoneWeight with 100% influence from <paramref name="idx"/>.</summary>
    private static BoneWeight MakeBoneWeight(int idx) => new BoneWeight
    {
        boneIndex0 = idx, weight0 = 1f,
        boneIndex1 = 0,   weight1 = 0f,
        boneIndex2 = 0,   weight2 = 0f,
        boneIndex3 = 0,   weight3 = 0f,
    };

    private T GetOrAdd<T>() where T : Component
    {
        T comp = GetComponent<T>();
        return comp != null ? comp : gameObject.AddComponent<T>();
    }

    private Transform GetOrCreateSlot(string slotName, Vector3 localPos)
    {
        Transform existing = transform.Find(slotName);
        if (existing != null) return existing;

        var go = new GameObject(slotName);
        go.transform.SetParent(transform, worldPositionStays: false);
        go.transform.localPosition = localPos;
        return go.transform;
    }
}
