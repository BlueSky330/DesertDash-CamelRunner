using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Procedurally generates a low-poly DesertBandit character mesh using the Unity Mesh API.
/// Follows the pattern established by ProceduralNinjaThiefMesh (AIG-187) and ProceduralCamelMesh (AIG-173).
/// Implements AIG-197.
///
/// Produces a stocky hooded-robe humanoid silhouette (~1.35 units tall):
///   Hooded robe body, face wraps, cream sash, coin bag (belt), dagger scabbard (back).
///
/// Color specification (per AIG-197):
///   Sandy tan robe   #C8A474  — SubMesh 0
///   Dark brown wraps #3D2010  — SubMesh 1  (face wraps + scabbard)
///   Cream sash       #E8D5A0  — SubMesh 2
///
/// Usage:
///   Attach to a GameObject; mesh builds automatically on Awake (play mode) or
///   call Build() explicitly from an Editor script (e.g. DesertBanditPrefabGenerator).
///
/// Coordinate convention:
///   Feet at Y=0, character faces +Z (forward). Units are Unity metres.
///
/// Geometry budget:
///   1 robe body × 12 tris    =  12
///   1 hood      × 12 tris    =  12
///   1 head      × 12 tris    =  12
///   1 face wrap × 12 tris    =  12
///   2 arms      × 12 tris    =  24
///   2 leg boots × 12 tris    =  24
///   1 sash      × 12 tris    =  12
///   1 coin bag  × 12 tris    =  12
///   1 scabbard  × 12 tris    =  12
///   Total                    ≈ 132 tris  (well within the &lt;1 500 target)
///
/// Skeleton (9 bones):
///   Root → Pelvis → Spine → Chest → Head
///                              └─ L_Arm
///                              └─ R_Arm
///          └─ L_Leg
///          └─ R_Leg
///
/// Child slots (created in Build):
///   weaponSlot — attach point on back for dagger
///   bagSlot    — attach point on belt for coin bag
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public class ProceduralDesertBanditMesh : MonoBehaviour
{
    // ── Color constants ────────────────────────────────────────────────────────

    /// <summary>#C8A474 — sandy tan robe.</summary>
    public static readonly Color ColorRobe  = new Color(0.784f, 0.643f, 0.455f, 1f);
    /// <summary>#3D2010 — dark brown face wraps and scabbard.</summary>
    public static readonly Color ColorWraps = new Color(0.239f, 0.125f, 0.063f, 1f);
    /// <summary>#E8D5A0 — cream sash.</summary>
    public static readonly Color ColorSash  = new Color(0.910f, 0.835f, 0.627f, 1f);

    // ── SubMesh indices ────────────────────────────────────────────────────────

    private const int SM_ROBE  = 0;
    private const int SM_WRAPS = 1;
    private const int SM_SASH  = 2;
    private const int SM_COUNT = 3;

    // ── Bone indices ───────────────────────────────────────────────────────────

    private const int BONE_ROOT   = 0;
    private const int BONE_PELVIS = 1;
    private const int BONE_SPINE  = 2;
    private const int BONE_CHEST  = 3;
    private const int BONE_HEAD   = 4;
    private const int BONE_L_ARM  = 5;
    private const int BONE_R_ARM  = 6;
    private const int BONE_L_LEG  = 7;
    private const int BONE_R_LEG  = 8;
    private const int BONE_COUNT  = 9;

    // ── Inspector fields ───────────────────────────────────────────────────────

    [Header("Material Overrides (null = created procedurally)")]
    public Material matRobe;
    public Material matWraps;
    public Material matSash;

    /// <summary>Transform where a weapon (curved dagger) can be attached. On back.</summary>
    [HideInInspector] public Transform weaponSlot;
    /// <summary>Transform where a coin bag can be attached. On belt.</summary>
    [HideInInspector] public Transform bagSlot;

    /// <summary>Bone transforms populated by BuildSkeleton(); exposed for external animation rigs.</summary>
    [HideInInspector] public Transform[] Bones;

    // ──────────────────────────────────────────────────────────────────────────
    // Unity lifecycle
    // ──────────────────────────────────────────────────────────────────────────

    void Awake()
    {
        // Auto-build at runtime; Editor builds are driven by DesertBanditPrefabGenerator.
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        Build();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Generate (or regenerate) the DesertBandit mesh, skeleton, materials, and child slots.
    /// Safe to call from Editor scripts, inspector buttons, and at runtime.
    /// No per-frame allocations; all allocations happen once inside this method.
    /// </summary>
    public void Build()
    {
        Bones = BuildSkeleton();

        var smr             = GetOrAdd<SkinnedMeshRenderer>();
        smr.sharedMesh      = GenerateMesh(Bones);
        smr.bones           = Bones;
        smr.rootBone        = Bones[BONE_ROOT];
        smr.sharedMaterials = BuildMaterials();

        BuildChildSlots();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Skeleton construction
    // ──────────────────────────────────────────────────────────────────────────

    private Transform[] BuildSkeleton()
    {
        // Bone positions in T-pose world space (feet at Y=0, facing +Z).
        // Stocky proportions: short legs, wide robe torso.
        //
        // Hierarchy:
        //   Root(0) → Pelvis(1) → Spine(2) → Chest(3) → Head(4)
        //                                          └─ L_Arm(5)
        //                                          └─ R_Arm(6)
        //              └─ L_Leg(7)
        //              └─ R_Leg(8)
        var boneData = new (string name, Vector3 pos, int parent)[]
        {
            /* 0 ROOT   */ ("Root",   new Vector3( 0.00f, 0.00f, 0f), -1),
            /* 1 PELVIS */ ("Pelvis", new Vector3( 0.00f, 0.25f, 0f),  0),
            /* 2 SPINE  */ ("Spine",  new Vector3( 0.00f, 0.50f, 0f),  1),
            /* 3 CHEST  */ ("Chest",  new Vector3( 0.00f, 0.75f, 0f),  2),
            /* 4 HEAD   */ ("Head",   new Vector3( 0.00f, 1.10f, 0f),  3),
            /* 5 L_ARM  */ ("L_Arm",  new Vector3(-0.30f, 0.75f, 0f),  3),
            /* 6 R_ARM  */ ("R_Arm",  new Vector3( 0.30f, 0.75f, 0f),  3),
            /* 7 L_LEG  */ ("L_Leg",  new Vector3(-0.13f, 0.25f, 0f),  1),
            /* 8 R_LEG  */ ("R_Leg",  new Vector3( 0.13f, 0.25f, 0f),  1),
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

    private void BuildChildSlots()
    {
        // weaponSlot: on back, at scabbard top — attach point for curved dagger.
        weaponSlot = GetOrCreateSlot("weaponSlot", new Vector3(0.12f, 0.88f, -0.18f));
        // bagSlot: on right hip, at coin bag position.
        bagSlot    = GetOrCreateSlot("bagSlot",    new Vector3(0.22f, 0.52f, 0.06f));
    }

    private Transform GetOrCreateSlot(string slotName, Vector3 localPos)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child.name == slotName) return child;
        }
        var go = new GameObject(slotName);
        go.transform.SetParent(transform, worldPositionStays: false);
        go.transform.localPosition = localPos;
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

        // Local helper — adds a flat-shaded box (24 verts, 12 tris) into submesh sm.
        void Box(Vector3 c, Vector3 size, int boneIdx, int sm, Quaternion? rot = null)
        {
            AppendBox(verts, normals, tris[sm], weights,
                      Matrix4x4.TRS(c, rot ?? Quaternion.identity, Vector3.one),
                      size, boneIdx);
        }

        // ── Robe body / torso (spec: ~0.45×0.85×0.3) ─────────────────────────
        // Wide flowing robe from Y≈0.175 to Y≈1.025; centre at Y=0.60.
        Box(new Vector3(0f, 0.60f, 0f), new Vector3(0.45f, 0.85f, 0.30f), BONE_SPINE, SM_ROBE);

        // ── Head (spec: ~0.22×0.25×0.22 under hood) ──────────────────────────
        Box(new Vector3(0f, 1.225f, 0f), new Vector3(0.22f, 0.25f, 0.22f), BONE_HEAD, SM_ROBE);

        // ── Hood — flat-shaded trapezoid box extending over head ──────────────
        // Slightly wider/deeper than head, angled slightly forward to form hood brow.
        Box(new Vector3(0f, 1.34f, -0.01f), new Vector3(0.28f, 0.13f, 0.27f), BONE_HEAD, SM_ROBE,
            Quaternion.Euler(-5f, 0f, 0f));

        // ── Face wraps — dark brown strip across lower front of head ──────────
        // Sits proud of head front face (head front Z ≈ 0.11), covering lower 55%.
        Box(new Vector3(0f, 1.19f, 0.12f), new Vector3(0.20f, 0.12f, 0.02f), BONE_HEAD, SM_WRAPS);

        // ── Left arm (stubby, emerges from robe sides) ────────────────────────
        Box(new Vector3(-0.305f, 0.68f, 0f), new Vector3(0.13f, 0.28f, 0.13f), BONE_L_ARM, SM_ROBE);

        // ── Right arm ────────────────────────────────────────────────────────
        Box(new Vector3( 0.305f, 0.68f, 0f), new Vector3(0.13f, 0.28f, 0.13f), BONE_R_ARM, SM_ROBE);

        // ── Left leg/boot (stubby, visible below robe hem) ───────────────────
        Box(new Vector3(-0.13f, 0.09f, 0f), new Vector3(0.14f, 0.18f, 0.16f), BONE_L_LEG, SM_ROBE);

        // ── Right leg/boot ────────────────────────────────────────────────────
        Box(new Vector3( 0.13f, 0.09f, 0f), new Vector3(0.14f, 0.18f, 0.16f), BONE_R_LEG, SM_ROBE);

        // ── Cream sash — horizontal band across mid-torso front ───────────────
        // Sits proud of robe front face (robe front Z ≈ 0.15); sash at Z ≈ 0.162.
        Box(new Vector3(0f, 0.62f, 0.161f), new Vector3(0.44f, 0.08f, 0.02f), BONE_SPINE, SM_SASH);

        // ── Coin bag — small box hanging from left side of belt ───────────────
        Box(new Vector3(-0.245f, 0.47f, 0.05f), new Vector3(0.08f, 0.10f, 0.07f), BONE_PELVIS, SM_ROBE);

        // ── Dagger scabbard — thin box on back, angled like a sheathed blade ──
        Box(new Vector3(0.11f, 0.65f, -0.168f), new Vector3(0.05f, 0.30f, 0.04f), BONE_CHEST, SM_WRAPS,
            Quaternion.Euler(12f, 0f, 0f));

        // ── Assemble mesh ─────────────────────────────────────────────────────
        var mesh = new Mesh { name = "ProceduralDesertBandit" };
        mesh.SetVertices(verts);
        mesh.SetNormals(normals);
        mesh.subMeshCount = SM_COUNT;
        for (int i = 0; i < SM_COUNT; i++)
            mesh.SetTriangles(tris[i], i);

        // Skin weights: 100% to a single bone per vertex group (hard-painted).
        mesh.boneWeights = weights.ToArray();

        // Bind poses: each bone's inverse world matrix relative to root.
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
        if (matRobe  == null) matRobe  = CreateMat(ColorRobe,  0f, 0.10f); // matte cloth
        if (matWraps == null) matWraps = CreateMat(ColorWraps, 0f, 0.05f); // very matte
        if (matSash  == null) matSash  = CreateMat(ColorSash,  0f, 0.15f); // slight sheen
        return new[] { matRobe, matWraps, matSash };
    }

    /// <summary>Creates a URP Lit (or Standard fallback) material with the given albedo, metallic and smoothness.</summary>
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
