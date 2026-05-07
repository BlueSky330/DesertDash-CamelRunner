using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Procedurally generates a low-poly Desert Bandit character mesh using the Unity Mesh API.
/// Follows the pattern established by ProceduralNinjaThiefMesh (AIG-187) and ProceduralCamelMesh (AIG-173).
///
/// Produces a hooded robe-wearing humanoid silhouette (~1.60 units tall):
///   Head (hooded), torso (wide robe), bandaged arms, coin pouch, sandaled feet — skinned to a 10-bone skeleton.
///
/// Color specification (per AIG-165 DesertBandit Design Spec):
///   Hooded robe       #D4A574  tan / light brown  — SubMesh 0
///   Accents / belt    #8B6F47  dark brown         — SubMesh 1
///   Coin pouch        #FFD700  gold coins         — SubMesh 2
///   Bandaged hands    #F5F5DC  off-white          — SubMesh 3
///
/// Usage:
///   Attach to a GameObject; mesh builds automatically on Awake (play mode) or
///   call Build() explicitly from an Editor script (e.g. DesertBanditPrefabGenerator).
///
/// Coordinate convention:
///   Feet at Y=0, character faces +Z (forward). Units are Unity metres.
///
/// Geometry budget:
///   13 body boxes (robe/torso/legs) × 12 tris = 156
///   4 bandage wraps (hands/forearms)   × 12   =  48
///   2 coin pouch boxes (waist)         × 12   =  24
///   2 sandals (feet)                   × 12   =  24
///   Total                                    ≈ 252 tris  (well within the <1200 target)
///
/// Skeleton (10 bones, AIG-187 spec):
///   Root → Pelvis → Spine → Chest → Neck → Head
///                              └─ L_Arm
///                              └─ R_Arm
///          └─ L_Leg
///          └─ R_Leg
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public class ProceduralDesertBanditMesh : MonoBehaviour
{
    // ── Color constants ────────────────────────────────────────────────────────

    /// <summary>#D4A574 — hooded robe tan/light brown.</summary>
    public static readonly Color ColorRobe     = new Color(0.831f, 0.647f, 0.455f, 1f);  // #D4A574
    /// <summary>#8B6F47 — dark brown belt/accents.</summary>
    public static readonly Color ColorBrown    = new Color(0.545f, 0.435f, 0.278f, 1f);  // #8B6F47
    /// <summary>#FFD700 — gold coin pouch.</summary>
    public static readonly Color ColorGold     = new Color(1f,     0.843f, 0f,     1f);  // #FFD700
    /// <summary>#F5F5DC — off-white bandaged hands.</summary>
    public static readonly Color ColorBandage  = new Color(0.961f, 0.961f, 0.863f, 1f);  // #F5F5DC

    // ── SubMesh indices ────────────────────────────────────────────────────────

    private const int SM_ROBE    = 0;
    private const int SM_BROWN   = 1;
    private const int SM_GOLD    = 2;
    private const int SM_BANDAGE = 3;
    private const int SM_COUNT   = 4;

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
    private const int BONE_NECK   = 9;
    private const int BONE_COUNT  = 10;

    // ── Inspector fields ───────────────────────────────────────────────────────

    [Header("Material Overrides (null = created procedurally)")]
    public Material matRobe;
    public Material matBrown;
    public Material matGold;
    public Material matBandage;

    // Bone transforms populated by BuildSkeleton(); exposed for external animation rigs.
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
    /// Generate (or regenerate) the DesertBandit mesh, 10-bone skeleton, and materials.
    /// Safe to call from Editor scripts, inspector buttons, and at runtime.
    /// </summary>
    public void Build()
    {
        Bones = BuildSkeleton();

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
        //   Root(0) → Pelvis(1) → Spine(2) → Chest(3) → Neck(9) → Head(4)
        //                                           └─ L_Arm(5)
        //                                           └─ R_Arm(6)
        //              └─ L_Leg(7)
        //              └─ R_Leg(8)
        var boneData = new (string name, Vector3 pos, int parent)[]
        {
            /* 0 ROOT   */ ("Root",   new Vector3( 0.00f, 0.00f, 0f), -1),
            /* 1 PELVIS */ ("Pelvis", new Vector3( 0.00f, 0.60f, 0f),  0),
            /* 2 SPINE  */ ("Spine",  new Vector3( 0.00f, 0.82f, 0f),  1),
            /* 3 CHEST  */ ("Chest",  new Vector3( 0.00f, 1.04f, 0f),  2),
            /* 4 HEAD   */ ("Head",   new Vector3( 0.00f, 1.40f, 0f),  9),  // child of Neck
            /* 5 L_ARM  */ ("L_Arm",  new Vector3(-0.28f, 1.08f, 0f),  3),
            /* 6 R_ARM  */ ("R_Arm",  new Vector3( 0.28f, 1.08f, 0f),  3),
            /* 7 L_LEG  */ ("L_Leg",  new Vector3(-0.09f, 0.60f, 0f),  1),
            /* 8 R_LEG  */ ("R_Leg",  new Vector3( 0.09f, 0.60f, 0f),  1),
            /* 9 NECK   */ ("Neck",   new Vector3( 0.00f, 1.22f, 0f),  3),
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

        // ── Lower torso / hips (Y: 0.60 → 0.90) ─────────────────────────────
        // Wider robe base at hips.
        Box(new Vector3(0f, 0.75f, 0f), new Vector3(0.32f, 0.30f, 0.20f), BONE_PELVIS, SM_ROBE);

        // ── Upper torso / chest (Y: 0.90 → 1.18) ────────────────────────────
        // Wide flowing robe, fuller silhouette than ninja.
        Box(new Vector3(0f, 1.04f, 0f), new Vector3(0.40f, 0.28f, 0.22f), BONE_CHEST, SM_ROBE);

        // ── Head with hood (Y: 1.28 → 1.56) ─────────────────────────────────
        // Hooded head, slightly larger to accommodate hood shape.
        Box(new Vector3(0f, 1.42f, 0f), new Vector3(0.32f, 0.32f, 0.26f), BONE_HEAD, SM_ROBE);

        // ── Left arm — upper part (in robe color) ────────────────────────────
        Box(new Vector3(-0.27f, 1.04f, 0f), new Vector3(0.11f, 0.26f, 0.11f), BONE_L_ARM, SM_ROBE);

        // ── Left arm — bandaged forearm/hand (off-white wraps) ──────────────
        Box(new Vector3(-0.27f, 0.75f, 0f), new Vector3(0.10f, 0.28f, 0.10f), BONE_L_ARM, SM_BANDAGE);

        // ── Right arm — upper part (in robe color) ───────────────────────────
        Box(new Vector3( 0.27f, 1.04f, 0f), new Vector3(0.11f, 0.26f, 0.11f), BONE_R_ARM, SM_ROBE);

        // ── Right arm — bandaged forearm/hand (off-white wraps) ─────────────
        Box(new Vector3( 0.27f, 0.75f, 0f), new Vector3(0.10f, 0.28f, 0.10f), BONE_R_ARM, SM_BANDAGE);

        // ── Left leg — thigh (robe) ──────────────────────────────────────────
        Box(new Vector3(-0.09f, 0.45f, 0f),     new Vector3(0.13f, 0.30f, 0.13f), BONE_L_LEG, SM_ROBE);

        // ── Left leg — shin (robe) ───────────────────────────────────────────
        Box(new Vector3(-0.09f, 0.19f, 0f),     new Vector3(0.11f, 0.24f, 0.11f), BONE_L_LEG, SM_ROBE);

        // ── Left leg — sandal (dark brown) ───────────────────────────────────
        Box(new Vector3(-0.09f, 0.05f, 0.04f),  new Vector3(0.12f, 0.08f, 0.22f), BONE_L_LEG, SM_BROWN);

        // ── Right leg — thigh (robe) ─────────────────────────────────────────
        Box(new Vector3( 0.09f, 0.45f, 0f),    new Vector3(0.13f, 0.30f, 0.13f), BONE_R_LEG, SM_ROBE);

        // ── Right leg — shin (robe) ──────────────────────────────────────────
        Box(new Vector3( 0.09f, 0.19f, 0f),    new Vector3(0.11f, 0.24f, 0.11f), BONE_R_LEG, SM_ROBE);

        // ── Right leg — sandal (dark brown) ───────────────────────────────────
        Box(new Vector3( 0.09f, 0.05f, 0.04f), new Vector3(0.12f, 0.08f, 0.22f), BONE_R_LEG, SM_BROWN);

        // ── Waist belt / cord accent (dark brown) ──────────────────────────────
        Box(new Vector3(0f, 0.905f, 0f),    new Vector3(0.36f, 0.08f, 0.22f), BONE_PELVIS, SM_BROWN);

        // ── Coin pouch — left side of waist (gold) ────────────────────────────
        Box(new Vector3(-0.15f, 0.80f, 0.08f), new Vector3(0.10f, 0.14f, 0.08f), BONE_PELVIS, SM_GOLD);

        // ── Coin pouch — right side of waist (gold) ───────────────────────────
        Box(new Vector3( 0.15f, 0.80f, 0.08f), new Vector3(0.10f, 0.14f, 0.08f), BONE_PELVIS, SM_GOLD);

        // ── Assemble mesh ─────────────────────────────────────────────────────
        var mesh = new Mesh { name = "ProceduralDesertBandit" };
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

    // ──────────────────────────────────────────────────────────────────────────
    // Materials
    // ──────────────────────────────────────────────────────────────────────────

    private Material[] BuildMaterials()
    {
        if (matRobe     == null) matRobe     = CreateMat(ColorRobe,     0f,   0f);    // matte tan robe
        if (matBrown    == null) matBrown    = CreateMat(ColorBrown,    0f,   0f);    // matte dark brown accents
        if (matGold     == null) matGold     = CreateMat(ColorGold,     0.6f, 0.5f);  // metallic gold coins
        if (matBandage  == null) matBandage  = CreateMat(ColorBandage,  0f,   0f);    // matte off-white bandages
        return new[] { matRobe, matBrown, matGold, matBandage };
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
