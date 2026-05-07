using UnityEngine;

/// <summary>
/// Implements 3 expression blendshapes for the procedural camel:
///   1. Happy — head scale up, mouth area lifted
///   2. Startled — eyes widened, eyebrows raised
///   3. Determined — eyes narrowed, head forward tilt
///
/// Since the procedural mesh is generated at runtime, blendshapes are created
/// by applying targeted vertex deformations to the mesh. The deformations
/// are stored as secondary mesh variants or applied via shader/material properties.
///
/// Implementation approach:
///   - Create separate mesh variants or use mesh morphing at runtime
///   - Expose blending parameter (0-1 range for smooth interpolation)
///   - Update MeshFilter when expression changes
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(ProceduralCamelMesh))]
public class CamelExpressionBlendshapes : MonoBehaviour
{
    /// <summary>
    /// Current expression blend weight (0 = neutral, 1 = full expression).
    /// Can animate/interpolate this value for smooth expression transitions.
    /// </summary>
    [Range(0f, 1f)]
    public float HappyBlend = 0f;

    [Range(0f, 1f)]
    public float StartledBlend = 0f;

    [Range(0f, 1f)]
    public float DeterminedBlend = 0f;

    // Cached references
    private ProceduralCamelMesh _camelMesh;
    private MeshFilter _meshFilter;
    private Mesh _baseMesh;
    private Mesh _happyMesh;
    private Mesh _startledMesh;
    private Mesh _determinedMesh;

    void Awake()
    {
        _camelMesh = GetComponent<ProceduralCamelMesh>();
        _meshFilter = GetComponent<MeshFilter>();

        // Base mesh is created by ProceduralCamelMesh.Build()
        _baseMesh = _meshFilter.sharedMesh;

        // Generate expression variants
        GenerateExpressionMeshes();
    }

    void Update()
    {
        // Update mesh blend based on current weights
        UpdateMeshBlend();
    }

    /// <summary>
    /// Generate the 3 expression mesh variants from the base mesh.
    /// Each variant applies targeted vertex deformations.
    /// </summary>
    private void GenerateExpressionMeshes()
    {
        if (_baseMesh == null) return;

        // Clone base mesh for each expression
        _happyMesh = Instantiate(_baseMesh);
        _happyMesh.name = "ProceduralCamel_Happy";

        _startledMesh = Instantiate(_baseMesh);
        _startledMesh.name = "ProceduralCamel_Startled";

        _determinedMesh = Instantiate(_baseMesh);
        _determinedMesh.name = "ProceduralCamel_Determined";

        // Apply deformations to each expression variant
        ApplyHappyDeformation(_happyMesh);
        ApplyStartledDeformation(_startledMesh);
        ApplyDeterminedDeformation(_determinedMesh);

        // Recalculate bounds for proper rendering
        _happyMesh.RecalculateBounds();
        _startledMesh.RecalculateBounds();
        _determinedMesh.RecalculateBounds();
    }

    /// <summary>
    /// Happy expression: Scale head up ~10%, lift front leg positions slightly.
    /// </summary>
    private void ApplyHappyDeformation(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Bounds bounds = mesh.bounds;

        // Identify approximate head region (upper forward area)
        // Head is roughly at Y > 1.0, Z > 0.0
        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].y > 1.0f && vertices[i].z > -0.05f)
            {
                // Scale head vertices outward from center
                Vector3 headLocal = vertices[i] - bounds.center;
                headLocal *= 1.1f;
                vertices[i] = bounds.center + headLocal;
            }

            // Lift front leg area slightly (happy stance)
            if (vertices[i].z > 0.2f && vertices[i].y < 0.6f)
            {
                vertices[i].y += 0.05f;
            }
        }

        mesh.vertices = vertices;
    }

    /// <summary>
    /// Startled expression: Widen eyes/head (scale head on X axis),
    /// raise eyebrow region (displace upper head vertices upward).
    /// </summary>
    private void ApplyStartledDeformation(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Bounds bounds = mesh.bounds;

        for (int i = 0; i < vertices.Length; i++)
        {
            // Head region (Y > 0.9, Z > -0.1)
            if (vertices[i].y > 0.9f && vertices[i].z > -0.1f)
            {
                // Widen head on X axis
                float xOffset = vertices[i].x * 0.15f; // 15% outward
                vertices[i].x += xOffset;

                // Raise upper head vertices (eyebrows, crown)
                if (vertices[i].y > 1.15f)
                {
                    vertices[i].y += 0.08f;
                }
            }

            // Rear hump: slightly lower (contrast with raised head)
            if (vertices[i].y > 1.1f && vertices[i].z < 0.1f)
            {
                vertices[i].y -= 0.03f;
            }
        }

        mesh.vertices = vertices;
    }

    /// <summary>
    /// Determined expression: Narrow eyes (compress head on X axis),
    /// tilt head forward slightly (rotate around local X).
    /// </summary>
    private void ApplyDeterminedDeformation(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Bounds bounds = mesh.bounds;
        Vector3 headCenter = new Vector3(0, 1.15f, 0); // Approximate head center

        for (int i = 0; i < vertices.Length; i++)
        {
            // Head region
            if (vertices[i].y > 0.9f && vertices[i].z > -0.1f)
            {
                // Narrow eyes: compress on X axis
                if (vertices[i].y > 1.0f)
                {
                    vertices[i].x *= 0.85f;
                }

                // Tilt head forward: lift Z (forward is positive Z)
                // Vertices further from head center rotate more
                float relativeY = vertices[i].y - headCenter.y;
                vertices[i].z += relativeY * 0.05f;
            }

            // Legs: slightly bent (determined stance)
            if (vertices[i].y < 0.5f && Mathf.Abs(vertices[i].x) > 0.1f)
            {
                vertices[i].z -= 0.02f; // Slight backward lean to legs
            }
        }

        mesh.vertices = vertices;
    }

    /// <summary>
    /// Interpolate between base, happy, startled, and determined meshes
    /// based on current blend weights.
    /// </summary>
    private void UpdateMeshBlend()
    {
        // For now, use the heaviest weighted expression
        // In future, could interpolate between multiple blends
        float maxBlend = Mathf.Max(HappyBlend, StartledBlend, DeterminedBlend);

        if (maxBlend < 0.01f)
        {
            _meshFilter.sharedMesh = _baseMesh;
        }
        else if (HappyBlend >= StartledBlend && HappyBlend >= DeterminedBlend)
        {
            // Interpolate between base and happy
            Mesh blend = InterpolateMeshes(_baseMesh, _happyMesh, HappyBlend);
            _meshFilter.sharedMesh = blend;
        }
        else if (StartledBlend >= DeterminedBlend)
        {
            // Interpolate between base and startled
            Mesh blend = InterpolateMeshes(_baseMesh, _startledMesh, StartledBlend);
            _meshFilter.sharedMesh = blend;
        }
        else
        {
            // Interpolate between base and determined
            Mesh blend = InterpolateMeshes(_baseMesh, _determinedMesh, DeterminedBlend);
            _meshFilter.sharedMesh = blend;
        }
    }

    /// <summary>
    /// Linearly interpolate between two meshes based on blend factor (0-1).
    /// Returns a new Mesh at the given blend ratio.
    /// </summary>
    private Mesh InterpolateMeshes(Mesh baseMesh, Mesh targetMesh, float blend)
    {
        if (blend <= 0f) return baseMesh;
        if (blend >= 1f) return targetMesh;

        Vector3[] baseVerts = baseMesh.vertices;
        Vector3[] targetVerts = targetMesh.vertices;

        if (baseVerts.Length != targetVerts.Length)
            return baseMesh; // Safety check

        Vector3[] blendVerts = new Vector3[baseVerts.Length];
        for (int i = 0; i < baseVerts.Length; i++)
        {
            blendVerts[i] = Vector3.Lerp(baseVerts[i], targetVerts[i], blend);
        }

        var blendMesh = new Mesh { name = "ProceduralCamel_Blend" };
        blendMesh.SetVertices(blendVerts);
        blendMesh.SetNormals(baseMesh.normals);
        blendMesh.SetTriangles(baseMesh.triangles, 0);
        blendMesh.RecalculateBounds();
        return blendMesh;
    }

    /// <summary>
    /// Helper to set an expression by name.
    /// Usage: camel.SetExpression("Happy", 1f);
    /// </summary>
    public void SetExpression(string expressionName, float weight)
    {
        weight = Mathf.Clamp01(weight);

        switch (expressionName.ToLower())
        {
            case "happy":
                HappyBlend = weight;
                StartledBlend = 0f;
                DeterminedBlend = 0f;
                break;
            case "startled":
                HappyBlend = 0f;
                StartledBlend = weight;
                DeterminedBlend = 0f;
                break;
            case "determined":
                HappyBlend = 0f;
                StartledBlend = 0f;
                DeterminedBlend = weight;
                break;
        }
    }
}
