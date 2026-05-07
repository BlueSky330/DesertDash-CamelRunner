using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Editor utility to procedurally generate all Egypt environment prefabs
/// using the Unity Mesh API. No external 3D assets required.
///
/// Run: Tools → Camel Runner → Build Egypt Environment
///
/// Generates 16 prefabs to Assets/Prefabs/Environment/:
///   Phase 1 — Road modules    : Road_Straight, Road_Curved, SandDune_Left, SandDune_Right
///   Phase 2 — Landmark props  : Pyramid_Large/Medium/Small, Sphinx, Obelisk_Large/Medium
///   Phase 3 — Vegetation      : PalmTree_Tall/Short, Cactus_Tall/Short, Rock_Large/Small
///
/// Mesh technique: flat-shaded (split vertices per face) matching ProceduralCamelMesh.
/// Pyramids and sand-dune wedges use custom Mesh API geometry;
/// trunks/bodies/legs use CreatePrimitive for runtime-compatible colliders.
/// </summary>
public static class ProceduralEgyptEnvironmentBuilder
{
    private const string PrefabDir = "Assets/Prefabs/Environment";
    private const string MatDir    = "Assets/Prefabs/Environment/Materials";

    // ── Palette ───────────────────────────────────────────────────────────────
    // Hex literals for reference; computed to float below.
    private static readonly Color SandColor       = new Color(0.910f, 0.788f, 0.541f); // #E8C98A
    private static readonly Color SandstoneColor  = new Color(0.831f, 0.659f, 0.353f); // #D4A85A
    private static readonly Color ObeliskColor    = new Color(0.800f, 0.720f, 0.520f); // warm stone
    private static readonly Color PalmTrunkColor  = new Color(0.550f, 0.380f, 0.200f); // brown
    private static readonly Color PalmGreen       = new Color(0.290f, 0.549f, 0.247f); // #4A8C3F
    private static readonly Color CactusGreen     = new Color(0.239f, 0.478f, 0.180f); // #3D7A2E
    private static readonly Color StoneGray       = new Color(0.541f, 0.518f, 0.439f); // #8A8470
    private static readonly Color LaneColor       = new Color(0.950f, 0.880f, 0.650f); // lane stripe

    // ── Entry point ───────────────────────────────────────────────────────────

    [MenuItem("Tools/Camel Runner/Build Egypt Environment")]
    public static void BuildAll()
    {
        EnsureFolders();

        // Phase 1 — Road modules (10m segments matching LevelGenerator chunk layout)
        BuildRoadStraight();
        BuildRoadCurved();
        BuildSandDune("Environment_SandDune_Left",  flipX: false);
        BuildSandDune("Environment_SandDune_Right", flipX: true);

        // Phase 2 — Landmark props
        BuildPyramid("Prop_Pyramid_Large",  8f, 8f, 5f);
        BuildPyramid("Prop_Pyramid_Medium", 5f, 5f, 3f);
        BuildPyramid("Prop_Pyramid_Small",  2f, 2f, 1.5f);
        BuildSphinx();
        BuildObelisk("Prop_Obelisk_Large",  shaftX: 0.30f, shaftZ: 0.30f, shaftH: 4.0f,
                                            tipX:   0.30f, tipZ:   0.30f, tipH:   0.4f);
        BuildObelisk("Prop_Obelisk_Medium", shaftX: 0.25f, shaftZ: 0.25f, shaftH: 2.5f,
                                            tipX:   0.25f, tipZ:   0.25f, tipH:   0.3f);

        // Phase 3 — Vegetation
        BuildPalmTree("Prop_PalmTree_Tall",  trunkHeight: 3.0f);
        BuildPalmTree("Prop_PalmTree_Short", trunkHeight: 1.5f);
        BuildCactus("Prop_Cactus_Tall",  trunkRadius: 0.12f, trunkHeight: 2.0f, hasArms: true);
        BuildCactus("Prop_Cactus_Short", trunkRadius: 0.15f, trunkHeight: 0.6f, hasArms: false);
        BuildRock("Prop_Rock_Large", large: true);
        BuildRock("Prop_Rock_Small", large: false);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[ProceduralEgyptEnvironmentBuilder] All environment prefabs built: " + PrefabDir);
        EditorUtility.DisplayDialog(
            "Egypt Environment Built",
            "All environment prefabs generated in:\n" + PrefabDir,
            "OK");
    }

    // ── Folder setup ──────────────────────────────────────────────────────────

    private static void EnsureFolders()
    {
        EnsureFolder("Assets/Prefabs");
        EnsureFolder(PrefabDir);
        EnsureFolder(MatDir);
    }

    private static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parent     = Path.GetDirectoryName(path).Replace('\\', '/');
            string folderName = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Phase 1 — Road modules
    // ══════════════════════════════════════════════════════════════════════════

    private static void BuildRoadStraight()
    {
        var root = new GameObject("Environment_Road_Straight");

        // Flat sandy road surface: 10m long (Z), 6m wide (3 lanes), 0.1m thick
        var surface = GameObject.CreatePrimitive(PrimitiveType.Cube);
        surface.name = "Surface";
        SetMat(surface, MakeMat("Road_Sand", SandColor));
        DestroyCollider(surface);
        surface.transform.SetParent(root.transform);
        surface.transform.localPosition = Vector3.zero;
        surface.transform.localScale    = new Vector3(6f, 0.1f, 10f);

        // Two lane dividers at X = ±2 (on top of surface)
        CreateLaneDivider(root.transform, -2f);
        CreateLaneDivider(root.transform,  2f);

        SavePrefab(root, "Environment_Road_Straight");
        Object.DestroyImmediate(root);
    }

    private static void BuildRoadCurved()
    {
        var root = new GameObject("Environment_Road_Curved");

        var surface = GameObject.CreatePrimitive(PrimitiveType.Cube);
        surface.name = "Surface";
        SetMat(surface, MakeMat("Road_Sand", SandColor));
        DestroyCollider(surface);
        surface.transform.SetParent(root.transform);
        surface.transform.localPosition    = Vector3.zero;
        surface.transform.localScale       = new Vector3(6f, 0.1f, 10f);
        // Gentle arc approximation: rotate road surface 10° around Y
        surface.transform.localEulerAngles = new Vector3(0f, 10f, 0f);

        CreateLaneDivider(root.transform, -2f);
        CreateLaneDivider(root.transform,  2f);

        SavePrefab(root, "Environment_Road_Curved");
        Object.DestroyImmediate(root);
    }

    private static void CreateLaneDivider(Transform parent, float xOffset)
    {
        var div = GameObject.CreatePrimitive(PrimitiveType.Cube);
        div.name = "LaneDivider";
        SetMat(div, MakeMat("Lane_Marking", LaneColor));
        DestroyCollider(div);
        div.transform.SetParent(parent);
        div.transform.localPosition = new Vector3(xOffset, 0.06f, 0f);
        div.transform.localScale    = new Vector3(0.06f, 0.01f, 10f);
    }

    /// <summary>
    /// Sand dune border: a triangular-prism (wedge) sitting just outside the road.
    /// flipX=false → left border (dune at X=-4), flipX=true → right border (X=+4, mirrored).
    /// Wedge is 2m wide, 1.5m tall, 10m long to match the road segment.
    /// </summary>
    private static void BuildSandDune(string prefabName, bool flipX)
    {
        var root   = new GameObject(prefabName);
        var meshGO = new GameObject("Wedge");
        meshGO.transform.SetParent(root.transform);

        if (flipX)
        {
            // Mirror on X to face inward from the right side
            meshGO.transform.localScale    = new Vector3(-1f, 1f, 1f);
            meshGO.transform.localPosition = new Vector3(4f, 0f, 0f);
        }
        else
        {
            meshGO.transform.localPosition = new Vector3(-4f, 0f, 0f);
        }

        meshGO.AddComponent<MeshFilter>().sharedMesh    = BuildWedgeMesh(2f, 1.5f, 10f);
        meshGO.AddComponent<MeshRenderer>().sharedMaterial = MakeMat("Sand_Dune", SandColor);

        SavePrefab(root, prefabName);
        Object.DestroyImmediate(root);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Phase 2 — Landmark props
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// True quad pyramid mesh (4 triangular sides + square base).
    /// Base centred at origin (Y=0); apex at Y=height.
    /// </summary>
    private static void BuildPyramid(string prefabName, float baseX, float baseZ, float height)
    {
        var root   = new GameObject(prefabName);
        var meshGO = new GameObject("Pyramid");
        meshGO.transform.SetParent(root.transform);
        meshGO.transform.localPosition = Vector3.zero;

        meshGO.AddComponent<MeshFilter>().sharedMesh    = BuildPyramidMesh(baseX, baseZ, height);
        meshGO.AddComponent<MeshRenderer>().sharedMaterial = MakeMat("Pyramid_Sandstone", SandstoneColor);

        SavePrefab(root, prefabName);
        Object.DestroyImmediate(root);
    }

    private static void BuildSphinx()
    {
        var root = new GameObject("Prop_Sphinx");
        var mat  = MakeMat("Sphinx_Sandstone", SandstoneColor);

        // Body: 2×0.8×1 (X×Y×Z), base on ground
        var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Body";
        SetMat(body, mat);
        DestroyCollider(body);
        body.transform.SetParent(root.transform);
        body.transform.localPosition = new Vector3(0f, 0.4f, 0f);
        body.transform.localScale    = new Vector3(2f, 0.8f, 1f);

        // Head sphere: radius = 0.5 → Unity sphere primitive is r=0.5 at scale 1
        var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        SetMat(head, mat);
        DestroyCollider(head);
        head.transform.SetParent(root.transform);
        head.transform.localPosition = new Vector3(0f, 1.1f, 0.4f);
        head.transform.localScale    = Vector3.one * 1.0f; // r=0.5

        // Paw boxes: two flat cubes in front of the body
        CreateSphinxPaw(root.transform, mat, xOffset: -0.45f);
        CreateSphinxPaw(root.transform, mat, xOffset:  0.45f);

        SavePrefab(root, "Prop_Sphinx");
        Object.DestroyImmediate(root);
    }

    private static void CreateSphinxPaw(Transform parent, Material mat, float xOffset)
    {
        var paw = GameObject.CreatePrimitive(PrimitiveType.Cube);
        paw.name = "Paw";
        SetMat(paw, mat);
        DestroyCollider(paw);
        paw.transform.SetParent(parent);
        paw.transform.localPosition = new Vector3(xOffset, 0.15f, 0.7f);
        paw.transform.localScale    = new Vector3(0.4f, 0.3f, 0.5f);
    }

    private static void BuildObelisk(string prefabName,
        float shaftX, float shaftZ, float shaftH,
        float tipX,   float tipZ,   float tipH)
    {
        var root = new GameObject(prefabName);
        var mat  = MakeMat("Obelisk_Stone", ObeliskColor);

        // Vertical shaft
        var shaft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shaft.name = "Shaft";
        SetMat(shaft, mat);
        DestroyCollider(shaft);
        shaft.transform.SetParent(root.transform);
        shaft.transform.localPosition = new Vector3(0f, shaftH * 0.5f, 0f);
        shaft.transform.localScale    = new Vector3(shaftX, shaftH, shaftZ);

        // Pyramid tip on top of shaft
        var tipGO = new GameObject("Tip");
        tipGO.transform.SetParent(root.transform);
        tipGO.transform.localPosition = new Vector3(0f, shaftH, 0f);
        tipGO.AddComponent<MeshFilter>().sharedMesh    = BuildPyramidMesh(tipX, tipZ, tipH);
        tipGO.AddComponent<MeshRenderer>().sharedMaterial = mat;

        SavePrefab(root, prefabName);
        Object.DestroyImmediate(root);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Phase 3 — Vegetation
    // ══════════════════════════════════════════════════════════════════════════

    private static void BuildPalmTree(string prefabName, float trunkHeight)
    {
        var root      = new GameObject(prefabName);
        var trunkMat  = MakeMat("Palm_Trunk", PalmTrunkColor);
        var frondMat  = MakeMat("Palm_Frond", PalmGreen);

        // Trunk: cylinder, radius=0.1 (diameter=0.2).
        // Unity Cylinder default: radius=0.5, height=2 at scale (1,1,1).
        // To get r=0.1: scaleX=scaleZ=0.2; to get desired height: scaleY=height/2.
        var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "Trunk";
        SetMat(trunk, trunkMat);
        DestroyCollider(trunk);
        trunk.transform.SetParent(root.transform);
        trunk.transform.localPosition = new Vector3(0f, trunkHeight * 0.5f, 0f);
        trunk.transform.localScale    = new Vector3(0.2f, trunkHeight * 0.5f, 0.2f);

        // 5 fronds fanning out from trunk top, drooping 35° below horizontal
        float frondLen = trunkHeight * 0.4f + 0.3f; // scale frond length with tree height
        for (int i = 0; i < 5; i++)
            CreatePalmFrond(root.transform, frondMat, trunkHeight, yAngle: i * 72f, length: frondLen);

        SavePrefab(root, prefabName);
        Object.DestroyImmediate(root);
    }

    private static void CreatePalmFrond(Transform parent, Material mat,
        float topY, float yAngle, float length)
    {
        var frond = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frond.name = "Frond";
        SetMat(frond, mat);
        DestroyCollider(frond);
        frond.transform.SetParent(parent);

        // Compute direction: droop 35° below horizontal in the yAngle direction
        Quaternion rot = Quaternion.Euler(-35f, yAngle, 0f);
        Vector3    dir = rot * Vector3.forward;

        frond.transform.localRotation = rot;
        // Centre of frond is half its length along the droop direction from trunk top
        frond.transform.localPosition = new Vector3(0f, topY, 0f) + dir * (length * 0.5f);
        frond.transform.localScale    = new Vector3(0.12f, 0.05f, length);
    }

    private static void BuildCactus(string prefabName, float trunkRadius, float trunkHeight, bool hasArms)
    {
        var root = new GameObject(prefabName);
        var mat  = MakeMat("Cactus_Green", CactusGreen);

        // Main body: vertical cylinder
        float diam   = trunkRadius * 2f;
        float scaleY = trunkHeight * 0.5f; // Unity cylinder height = 2 * scaleY
        var body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        body.name = "Body";
        SetMat(body, mat);
        DestroyCollider(body);
        body.transform.SetParent(root.transform);
        body.transform.localPosition = new Vector3(0f, trunkHeight * 0.5f, 0f);
        body.transform.localScale    = new Vector3(diam, scaleY, diam);

        if (hasArms)
        {
            // Two arms: horizontal piece + upward tip, one per side
            CreateCactusArm(root.transform, mat, armBaseY: trunkHeight * 0.6f, xDir: -1f);
            CreateCactusArm(root.transform, mat, armBaseY: trunkHeight * 0.6f, xDir:  1f);
        }

        SavePrefab(root, prefabName);
        Object.DestroyImmediate(root);
    }

    private static void CreateCactusArm(Transform parent, Material mat, float armBaseY, float xDir)
    {
        // Horizontal elbow: cylinder rotated 90° so it extends along X
        var elbow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        elbow.name = "ArmElbow";
        SetMat(elbow, mat);
        DestroyCollider(elbow);
        elbow.transform.SetParent(parent);
        elbow.transform.localEulerAngles = new Vector3(0f, 0f, 90f); // height axis → X axis
        // Centre at half-way between body and arm tip along X
        elbow.transform.localPosition    = new Vector3(xDir * 0.18f, armBaseY, 0f);
        elbow.transform.localScale       = new Vector3(0.18f, 0.18f, 0.18f); // stub cylinder

        // Vertical arm tip rising from the elbow end
        var tip = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        tip.name = "ArmTip";
        SetMat(tip, mat);
        DestroyCollider(tip);
        tip.transform.SetParent(parent);
        tip.transform.localPosition = new Vector3(xDir * 0.36f, armBaseY + 0.30f, 0f);
        tip.transform.localScale    = new Vector3(0.18f, 0.25f, 0.18f); // height = 2*0.25 = 0.5
    }

    private static void BuildRock(string prefabName, bool large)
    {
        var root = new GameObject(prefabName);
        var mat  = MakeMat("Rock_Gray", StoneGray);

        if (large)
        {
            // Three overlapping scaled/rotated cubes approximate a deformed sphere
            CreateRockBlock(root.transform, mat, new Vector3( 0.00f, 0.40f,  0.00f), new Vector3(1.0f, 0.8f, 0.9f),  0f);
            CreateRockBlock(root.transform, mat, new Vector3( 0.15f, 0.45f,  0.10f), new Vector3(0.7f, 0.7f, 0.8f), 20f);
            CreateRockBlock(root.transform, mat, new Vector3(-0.10f, 0.50f, -0.10f), new Vector3(0.8f, 0.6f, 0.7f), -15f);
        }
        else
        {
            // Single cube, lightly rotated for irregularity
            CreateRockBlock(root.transform, mat, new Vector3(0f, 0.2f, 0f), new Vector3(0.4f, 0.4f, 0.4f), 5f);
        }

        SavePrefab(root, prefabName);
        Object.DestroyImmediate(root);
    }

    private static void CreateRockBlock(Transform parent, Material mat,
        Vector3 pos, Vector3 scale, float yRot)
    {
        var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = "Block";
        SetMat(block, mat);
        DestroyCollider(block);
        block.transform.SetParent(parent);
        block.transform.localPosition    = pos;
        block.transform.localScale       = scale;
        block.transform.localEulerAngles = new Vector3(5f, yRot, -5f);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Custom Mesh Builders
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Square-base pyramid mesh. Base centred at origin (Y=0), apex at Y=height.
    /// Flat-shaded: each face has its own split vertices so normals are face-constant.
    ///
    /// Faces: 4 triangular sides + 1 quad base (downward-facing).
    /// Winding: CCW from outside each face (Unity default front-face culling).
    /// </summary>
    private static Mesh BuildPyramidMesh(float baseX, float baseZ, float height)
    {
        float hx = baseX * 0.5f;
        float hz = baseZ * 0.5f;

        // 4 base corners + apex
        Vector3 apex = new Vector3( 0f,    height,  0f);
        Vector3 bLF  = new Vector3(-hx,    0f,      hz); // left-front
        Vector3 bRF  = new Vector3( hx,    0f,      hz); // right-front
        Vector3 bRB  = new Vector3( hx,    0f,     -hz); // right-back
        Vector3 bLB  = new Vector3(-hx,    0f,     -hz); // left-back

        var verts   = new List<Vector3>();
        var normals = new List<Vector3>();
        var tris    = new List<int>();

        // Adds a flat-shaded triangle; normal = cross(v1-v0, v2-v0)
        void AddTri(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            Vector3 n = Vector3.Cross(v1 - v0, v2 - v0).normalized;
            int b = verts.Count;
            verts.Add(v0); verts.Add(v1); verts.Add(v2);
            normals.Add(n); normals.Add(n); normals.Add(n);
            tris.Add(b); tris.Add(b + 1); tris.Add(b + 2);
        }

        // Adds a flat-shaded quad (two triangles); normal = cross(v1-v0, v3-v0)
        void AddQuad(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Vector3 n = Vector3.Cross(v1 - v0, v3 - v0).normalized;
            int b = verts.Count;
            verts.Add(v0); verts.Add(v1); verts.Add(v2); verts.Add(v3);
            normals.Add(n); normals.Add(n); normals.Add(n); normals.Add(n);
            tris.Add(b); tris.Add(b + 1); tris.Add(b + 2);
            tris.Add(b); tris.Add(b + 2); tris.Add(b + 3);
        }

        // 4 triangular side faces (CCW from outside each face)
        AddTri(bLF, bRF, apex); // front  (+Z outward)
        AddTri(bRF, bRB, apex); // right  (+X outward)
        AddTri(bRB, bLB, apex); // back   (-Z outward)
        AddTri(bLB, bLF, apex); // left   (-X outward)

        // Base quad (normal = -Y, CCW from below)
        // Using Cross(v1-v0, v3-v0): v0=bRF, v1=bLF, v3=bRB → Cross = (0,-1,0)·scalar ✓
        AddQuad(bRF, bLF, bLB, bRB);

        var mesh = new Mesh { name = "Pyramid" };
        mesh.SetVertices(verts);
        mesh.SetNormals(normals);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    /// <summary>
    /// Triangular-prism (wedge) mesh for sand dunes.
    /// The wedge is width units wide (X), depth units long (Z), height units tall.
    /// The slope rises from Y=0 at X=+width/2 to Y=height at X=-width/2.
    /// Flat-shaded; 5 faces: 2 triangular ends + sloped top + bottom quad + left vertical wall.
    /// (The right edge is a zero-height line — no face needed.)
    /// </summary>
    private static Mesh BuildWedgeMesh(float width, float height, float depth)
    {
        float hw = width  * 0.5f;
        float hd = depth  * 0.5f;

        // 6 unique positions: high-left and low-right × front/back
        Vector3 LHB = new Vector3(-hw, height, -hd); // left-high-back
        Vector3 LLB = new Vector3(-hw, 0f,     -hd); // left-low-back
        Vector3 RLB = new Vector3( hw, 0f,     -hd); // right-low-back
        Vector3 LHF = new Vector3(-hw, height,  hd); // left-high-front
        Vector3 LLF = new Vector3(-hw, 0f,      hd); // left-low-front
        Vector3 RLF = new Vector3( hw, 0f,      hd); // right-low-front

        var verts   = new List<Vector3>();
        var normals = new List<Vector3>();
        var tris    = new List<int>();

        void AddTri(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            Vector3 n = Vector3.Cross(v1 - v0, v2 - v0).normalized;
            int b = verts.Count;
            verts.Add(v0); verts.Add(v1); verts.Add(v2);
            normals.Add(n); normals.Add(n); normals.Add(n);
            tris.Add(b); tris.Add(b + 1); tris.Add(b + 2);
        }

        void AddQuad(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Vector3 n = Vector3.Cross(v1 - v0, v3 - v0).normalized;
            int b = verts.Count;
            verts.Add(v0); verts.Add(v1); verts.Add(v2); verts.Add(v3);
            normals.Add(n); normals.Add(n); normals.Add(n); normals.Add(n);
            tris.Add(b); tris.Add(b + 1); tris.Add(b + 2);
            tris.Add(b); tris.Add(b + 2); tris.Add(b + 3);
        }

        // Triangular end faces (CCW from outside)
        AddTri(LHB, RLB, LLB); // back face  (-Z outward)
        AddTri(LHF, LLF, RLF); // front face (+Z outward)

        // Sloped top quad (outward normal = +X +Y direction, up the slope)
        AddQuad(LHB, LHF, RLF, RLB);

        // Bottom quad (normal = -Y; order produces Cross pointing down)
        AddQuad(LLF, LLB, RLB, RLF);

        // Left vertical wall (-X outward)
        AddQuad(LHB, LLB, LLF, LHF);

        var mesh = new Mesh { name = "Wedge" };
        mesh.SetVertices(verts);
        mesh.SetNormals(normals);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Shared helpers
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Loads or creates a flat-matte material at MatDir/matName.mat.
    /// Uses URP Lit if available, falls back to Standard.
    /// </summary>
    private static Material MakeMat(string matName, Color color)
    {
        string path     = MatDir + "/" + matName + ".mat";
        var    existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null)
        {
            existing.color = color;
            EditorUtility.SetDirty(existing);
            return existing;
        }

        var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        var mat    = new Material(shader) { name = matName, color = color };
        // Zero glossiness for flat matte finish
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0f);
        if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0f);
        if (mat.HasProperty("_Metallic"))   mat.SetFloat("_Metallic",   0f);
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    private static void SetMat(GameObject obj, Material mat)
    {
        if (mat == null) return;
        var rend = obj.GetComponent<Renderer>();
        if (rend != null) rend.sharedMaterial = mat;
    }

    private static void DestroyCollider(GameObject obj)
    {
        var col = obj.GetComponent<Collider>();
        if (col != null) Object.DestroyImmediate(col);
    }

    private static void SavePrefab(GameObject go, string prefabName)
    {
        string path = PrefabDir + "/" + prefabName + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(go, path);
    }
}
