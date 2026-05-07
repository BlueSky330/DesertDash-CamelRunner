using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Procedurally generates a low-poly Pirate obstacle character mesh using the Unity Mesh API.
/// Follows the pattern established by ProceduralCamelMesh (AIG-173).
///
/// Produces a lean humanoid silhouette (~1.85 units tall) with classic pirate styling:
///   Head (tan skin), tricorn hat (3-piece navy box assembly), navy coat,
///   tan shirt cuffs, black boots, red sash, and a flat black eye patch.
///
/// Color specification (per AIG-199):
///   Navy coat / hat   #1A237E  dark navy     — SubMesh 0
///   Tan shirt / skin  #C8A474  warm tan      — SubMesh 1
///   Black boots       #111111  near-black    — SubMesh 2
///   Red sash          #CC2200  crimson red   — SubMesh 3
///
/// Usage:
///   Attach to a GameObject; mesh builds automatically on Awake (play mode) or
///   call Build() explicitly from an Editor script (e.g. PiratePrefabGenerator).
///
/// Coordinate convention:
///   Feet at Y=0, character faces +Z (forward). Units are Unity metres.
///
/// Geometry budget:
///   20 body/hat boxes × 12 tris = 240 tris  (well within the &lt;1 500 target)
///
/// Child slots:
///   weaponSlot — parented to R_Arm bone, positioned at right hand
///   hatSlot    — parented to Head bone, positioned at hat crown top
///
/// Skeleton (10 bones):
///   Root → Pelvis → Spine → Chest → Neck → Head
///                              └─ L_Arm
///                              └─ R_Arm
///          └─ L_Leg
///          └─ R_Leg
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public class ProceduralPirateMesh : MonoBehaviour
{
    // ── Color constants ────────────────────────────────────────────────────────

    /// <summary>#1A237E — dark navy coat and hat.</summary>
    public static readonly Color ColorNavy  = new Color(0.102f, 0.137f, 0.494f, 1f);
    /// <summary>#C8A474 — warm tan shirt and skin.</summary>
    public static readonly Color ColorTan   = new Color(0.784f, 0.643f, 0.455f, 1f);
    /// <summary>#111111 — near-black boots and eye patch.</summary>
    public static readonly Color ColorBlack = new Color(0.067f, 0.067f, 0.067f, 1f);
    /// <summary>#CC2200 — crimson red sash.</summary>
    public static readonly Color ColorRed   = new Color(0.800f, 0.133f, 0f,     1f);

    // ── SubMesh indices ────────────────────────────────────────────────────────

    private const int SM_NAVY  = 0;
    private const int SM_TAN   = 1;
    private const int SM_BLACK = 2;
    private const int SM_RED   = 3;
    private const int SM_COUNT = 4;

    // ── Bone indices ───────────────────────────────────────────────────────────

    private const int BONE_ROOT   = 0;
    private const int BONE_PELVIS = 1;
    private const int BONE_SPINE  = 2;
    private const int BONE_CHEST  = 3;
    private const int BONE_NECK   = 4;
    private const int BONE_HEAD   = 5;
    private const int BONE_L_ARM  = 6;
    private const int BONE_R_ARM  = 7;
    private const int BONE_L_LEG  = 8;
    private const int BONE_R_LEG  = 9;
    private const int BONE_COUNT  = 10;

    // ── Inspector fields ───────────────────────────────────────────────────────

    [Header("Material Overrides (null = created procedurally)")]
    public Material matNavy;
    public Material matTan;
    public Material matBlack;
    public Material matRed;

    /// <summary>Bone transforms populated by BuildSkeleton(); exposed for external animation rigs.</summary>
    [HideInInspector] public Transform[] Bones;

    /// <summary>Right-hand slot for weapon attachment (sword, pistol, etc.).</summary>
    [HideInInspector] public Transform weaponSlot;
    /// <summary>Head-top slot for hat attachment or hat-removal effects.</summary>
    [HideInInspector] public Transform hatSlot;

    // ──────────────────────────────────────────────────────────────────────────
    // Unity lifecycle
    // ──────────────────────────────────────────────────────────────────────────

    void Awake()
    {
        // Auto-build at runtime; Editor builds are driven by PiratePrefabGenerator.
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        Build();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Generate (or regenerate) the Pirate mesh, 10-bone skeleton, child slots, and materials.
    /// Safe to call from Editor scripts, inspector buttons, and at runtime.
    /// </summary>
    public void Build()
    {
        Bones = BuildSkeleton();
        BuildSlots();

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
        // Bone positions defined in T-pose world space (feet at Y=0, facing +Z).
        // Parent index -1 means the bone's parent is the mesh root transform.
        //
        // Hierarchy:
        //   Root(0) → Pelvis(1) → Spine(2) → Chest(3) → Neck(4) → Head(5)
        //                                           └─ L_Arm(6)
        //                                           └─ R_Arm(7)
        //              └─ L_Leg(8)
        //              └─ R_Leg(9)
        var boneData = new (string name, Vector3 pos, int parent)[]
        {
            /* 0 ROOT   */ ("Root",   new Vector3( 0.00f, 0.00f, 0f), -1),
            /* 1 PELVIS */ ("Pelvis", new Vector3( 0.00f, 0.60f, 0f),  0),
            /* 2 SPINE  */ ("Spine",  new Vector3( 0.00f, 0.82f, 0f),  1),
            /* 3 CHEST  */ ("Chest",  new Vector3( 0.00f, 1.04f, 0f),  2),
            /* 4 NECK   */ ("Neck",   new Vector3( 0.00f, 1.22f, 0f),  3),
            /* 5 HEAD   */ ("Head",   new Vector3( 0.00f, 1.40f, 0f),  4),
            /* 6 L_ARM  */ ("L_Arm",  new Vector3(-0.30f, 1.08f, 0f),  3),
            /* 7 R_ARM  */ ("R_Arm",  new Vector3( 0.30f, 1.08f, 0f),  3),
            /* 8 L_LEG  */ ("L_Leg",  new Vector3(-0.10f, 0.60f, 0f),  1),
            /* 9 R_LEG  */ ("R_Leg",  new Vector3( 0.10f, 0.60f, 0f),  1),
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
    // Child slot construction
    // ──────────────────────────────────────────────────────────────────────────

    private void BuildSlots()
    {
        // weaponSlot: at right hand (below R_Arm, end of forearm).
        weaponSlot = EnsureSlot("weaponSlot", Bones[BONE_R_ARM], new Vector3(0.30f, 0.60f, 0f));
        // hatSlot: at top of tricorn crown, for runtime hat-swap/particle effects.
        hatSlot    = EnsureSlot("hatSlot",    Bones[BONE_HEAD],  new Vector3(0f,    1.83f, 0f));
    }

    private Transform EnsureSlot(string slotName, Transform parent, Vector3 worldPos)
    {
        for (int i = 0; i < parent.childCount; i++)
            if (parent.GetChild(i).name == slotName)
                return parent.GetChild(i);

        var go = new GameObject(slotName);
        go.transform.SetParent(parent, worldPositionStays: false);
        go.transform.position = worldPos;
        return go.transform;
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

        // ── Lower torso / coat tails (Y: 0.60 → 0.90) ───────────────────────
        // Slightly wider than chest for a flared-coat silhouette.
        Box(new Vector3(0f, 0.75f, 0f),  new Vector3(0.38f, 0.30f, 0.28f), BONE_PELVIS, SM_NAVY);

        // ── Upper torso / coat chest (Y: 0.90 → 1.18) ───────────────────────
        Box(new Vector3(0f, 1.04f, 0f),  new Vector3(0.40f, 0.28f, 0.28f), BONE_CHEST, SM_NAVY);

        // ── Red sash / waist belt ─────────────────────────────────────────────
        Box(new Vector3(0f, 0.895f, 0f), new Vector3(0.39f, 0.08f, 0.29f), BONE_PELVIS, SM_RED);

        // ── Head — tan skin (Y: 1.28 → 1.60, radius ≈ 0.20) ─────────────────
        Box(new Vector3(0f, 1.44f, 0f),  new Vector3(0.38f, 0.32f, 0.34f), BONE_HEAD, SM_TAN);

        // ── Left arm — navy coat sleeve, tan cuff ─────────────────────────────
        Box(new Vector3(-0.30f, 1.04f, 0f),  new Vector3(0.11f, 0.26f, 0.12f), BONE_L_ARM, SM_NAVY); // upper
        Box(new Vector3(-0.30f, 0.74f, 0f),  new Vector3(0.10f, 0.26f, 0.10f), BONE_L_ARM, SM_TAN);  // forearm/cuff

        // ── Right arm ────────────────────────────────────────────────────────
        Box(new Vector3( 0.30f, 1.04f, 0f),  new Vector3(0.11f, 0.26f, 0.12f), BONE_R_ARM, SM_NAVY);
        Box(new Vector3( 0.30f, 0.74f, 0f),  new Vector3(0.10f, 0.26f, 0.10f), BONE_R_ARM, SM_TAN);

        // ── Left leg — navy trousers + black boot ─────────────────────────────
        Box(new Vector3(-0.10f, 0.44f, 0f),    new Vector3(0.13f, 0.30f, 0.14f), BONE_L_LEG, SM_NAVY);  // thigh
        Box(new Vector3(-0.10f, 0.19f, 0f),    new Vector3(0.11f, 0.24f, 0.12f), BONE_L_LEG, SM_BLACK); // shin boot
        Box(new Vector3(-0.10f, 0.05f, 0.04f), new Vector3(0.12f, 0.10f, 0.22f), BONE_L_LEG, SM_BLACK); // boot foot

        // ── Right leg ────────────────────────────────────────────────────────
        Box(new Vector3( 0.10f, 0.44f, 0f),    new Vector3(0.13f, 0.30f, 0.14f), BONE_R_LEG, SM_NAVY);
        Box(new Vector3( 0.10f, 0.19f, 0f),    new Vector3(0.11f, 0.24f, 0.12f), BONE_R_LEG, SM_BLACK);
        Box(new Vector3( 0.10f, 0.05f, 0.04f), new Vector3(0.12f, 0.10f, 0.22f), BONE_R_LEG, SM_BLACK);

        // ── Eye patch — thin black box on left side of face front ─────────────
        // Head front face sits at Z ≈ +0.17; patch at Z = 0.185 avoids z-fighting.
        Box(new Vector3(-0.09f, 1.46f, 0.185f), new Vector3(0.13f, 0.09f, 0.01f), BONE_HEAD, SM_BLACK);

        // ── Tricorn hat — 3-piece navy box assembly ───────────────────────────
        // 1. Wide flat brim: significantly wider than head for classic silhouette.
        Box(new Vector3(0f, 1.63f, 0f),     new Vector3(0.62f, 0.05f, 0.55f), BONE_HEAD, SM_NAVY);
        // 2. Hat crown body (sits centred atop brim).
        Box(new Vector3(0f, 1.74f, 0f),     new Vector3(0.30f, 0.16f, 0.26f), BONE_HEAD, SM_NAVY);
        // 3. Front peak — forward portion of brim angled upward.
        Box(new Vector3(0f,     1.665f,  0.23f), new Vector3(0.24f, 0.055f, 0.17f),
            BONE_HEAD, SM_NAVY, Quaternion.Euler(-28f, 0f, 0f));
        // 4. Left back peak — left rear brim angled upward.
        Box(new Vector3(-0.23f, 1.665f, -0.06f), new Vector3(0.17f, 0.055f, 0.24f),
            BONE_HEAD, SM_NAVY, Quaternion.Euler(0f, 30f, 28f));
        // 5. Right back peak — right rear brim angled upward.
        Box(new Vector3( 0.23f, 1.665f, -0.06f), new Vector3(0.17f, 0.055f, 0.24f),
            BONE_HEAD, SM_NAVY, Quaternion.Euler(0f, -30f, -28f));

        // ── Assemble mesh ─────────────────────────────────────────────────────
        var mesh = new Mesh { name = "ProceduralPirate" };
        mesh.SetVertices(verts);
        mesh.SetNormals(normals);
        mesh.subMeshCount = SM_COUNT;
        for (int i = 0; i < SM_COUNT; i++)
            mesh.SetTriangles(tris[i], i);

        // Skin weights: 100% to a single bone per vertex group (hard-painted).
        mesh.boneWeights = weights.ToArray();

        // Bind poses: each bone's inverse world transform, combined with the mesh root's
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

    // ──────────────────────────────────────────────────────────────────────────
    // Materials
    // ──────────────────────────────────────────────────────────────────────────

    private Material[] BuildMaterials()
    {
        if (matNavy  == null) matNavy  = CreateMat(ColorNavy,  0f,   0f);    // matte cloth
        if (matTan   == null) matTan   = CreateMat(ColorTan,   0f,   0f);    // matte skin
        if (matBlack == null) matBlack = CreateMat(ColorBlack, 0.1f, 0.1f);  // leather boots
        if (matRed   == null) matRed   = CreateMat(ColorRed,   0f,   0f);    // matte sash
        return new[] { matNavy, matTan, matBlack, matRed };
    }

    /// <summary>Creates a Lit/Standard material with the given albedo, metallic and smoothness.</summary>
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
}
