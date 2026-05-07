"""
Camel Default Model - Low-Poly Base Mesh Generator
Author: Artist1
Purpose: Generate low-poly camel base mesh in Blender (<1200 tris)

This script creates a procedural low-poly camel with:
- Body, head, neck, legs, tail
- Exaggerated proportions (Crossy Road style)
- Ready for expression blendshapes
- UV-friendly topology

Usage:
1. Open Blender
2. Open Python console (Shift+F4)
3. Run this script
4. Or: import generate_camel_base_mesh; generate_camel_base_mesh.create_camel()
"""

import bpy
import bmesh
from mathutils import Vector

def create_camel_mesh():
    """Create Camel base mesh with low-poly geometry."""

    # Create mesh and object
    mesh = bpy.data.meshes.new("Camel_Base")
    obj = bpy.data.objects.new("Camel_Default", mesh)
    bpy.context.collection.objects.link(obj)
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)

    # Use bmesh for easier geometry creation
    bm = bmesh.new()

    # ===== BODY (main torso) =====
    body_vertices = [
        # Front face
        (-0.2, 0.1, 0.3), (-0.2, -0.1, 0.3),  # top-left, bottom-left
        (0.2, 0.1, 0.3), (0.2, -0.1, 0.3),    # top-right, bottom-right
        # Back face
        (-0.2, 0.1, -0.2), (-0.2, -0.1, -0.2),
        (0.2, 0.1, -0.2), (0.2, -0.1, -0.2),
    ]
    body_verts = [bm.verts.new(v) for v in body_vertices]

    # Create body faces (cube)
    bm.faces.new([body_verts[0], body_verts[1], body_verts[3], body_verts[2]])  # front
    bm.faces.new([body_verts[4], body_verts[5], body_verts[7], body_verts[6]])  # back
    bm.faces.new([body_verts[0], body_verts[4], body_verts[6], body_verts[2]])  # top
    bm.faces.new([body_verts[1], body_verts[5], body_verts[7], body_verts[3]])  # bottom
    bm.faces.new([body_verts[0], body_verts[1], body_verts[5], body_verts[4]])  # left
    bm.faces.new([body_verts[2], body_verts[3], body_verts[7], body_verts[6]])  # right

    # ===== NECK (long, exaggerated) =====
    neck_vertices = [
        (-0.08, 0.05, 0.35), (-0.08, -0.05, 0.35),
        (0.08, 0.05, 0.35), (0.08, -0.05, 0.35),
        (-0.08, 0.05, 0.65), (-0.08, -0.05, 0.65),
        (0.08, 0.05, 0.65), (0.08, -0.05, 0.65),
    ]
    neck_verts = [bm.verts.new(v) for v in neck_vertices]

    bm.faces.new([neck_verts[0], neck_verts[1], neck_verts[3], neck_verts[2]])  # front
    bm.faces.new([neck_verts[4], neck_verts[5], neck_verts[7], neck_verts[6]])  # back
    bm.faces.new([neck_verts[0], neck_verts[4], neck_verts[6], neck_verts[2]])  # top
    bm.faces.new([neck_verts[1], neck_verts[5], neck_verts[7], neck_verts[3]])  # bottom
    bm.faces.new([neck_verts[0], neck_verts[1], neck_verts[5], neck_verts[4]])  # left
    bm.faces.new([neck_verts[2], neck_verts[3], neck_verts[7], neck_verts[6]])  # right

    # ===== HEAD (big, round for expression) =====
    # Simplified sphere-like shape
    head_center = Vector((0, 0, 0.8))
    head_radius = 0.12

    head_verts = []
    # Create cube-ish head with chamfered edges
    head_positions = [
        (-0.1, 0.08, 0.8), (0.1, 0.08, 0.8),    # front top
        (-0.1, -0.08, 0.8), (0.1, -0.08, 0.8),  # front bottom
        (-0.1, 0.08, 0.95), (0.1, 0.08, 0.95),  # back top
        (-0.1, -0.08, 0.95), (0.1, -0.08, 0.95), # back bottom
        (0, 0.12, 0.87),  # top center
        (0, -0.12, 0.87), # bottom center
    ]
    head_verts = [bm.verts.new(v) for v in head_positions]

    # Head faces
    bm.faces.new([head_verts[0], head_verts[2], head_verts[3], head_verts[1]])  # front
    bm.faces.new([head_verts[4], head_verts[6], head_verts[7], head_verts[5]])  # back
    bm.faces.new([head_verts[0], head_verts[1], head_verts[5], head_verts[4]])  # top
    bm.faces.new([head_verts[2], head_verts[3], head_verts[7], head_verts[6]])  # bottom

    # ===== SNOUT (extended forward) =====
    snout_verts = [bm.verts.new(v) for v in [
        (-0.06, 0.04, 0.95), (0.06, 0.04, 0.95),
        (-0.06, -0.04, 0.95), (0.06, -0.04, 0.95),
        (-0.04, 0.02, 1.1), (0.04, 0.02, 1.1),
        (-0.04, -0.02, 1.1), (0.04, -0.02, 1.1),
    ]]
    bm.faces.new([snout_verts[0], snout_verts[2], snout_verts[3], snout_verts[1]])
    bm.faces.new([snout_verts[4], snout_verts[6], snout_verts[7], snout_verts[5]])
    bm.faces.new([snout_verts[0], snout_verts[1], snout_verts[5], snout_verts[4]])
    bm.faces.new([snout_verts[2], snout_verts[3], snout_verts[7], snout_verts[6]])

    # ===== EYES (big, glossy) =====
    # Left eye
    left_eye_verts = [bm.verts.new(v) for v in [
        (-0.12, 0.10, 0.88), (-0.06, 0.10, 0.88),
        (-0.12, 0.12, 0.88), (-0.06, 0.12, 0.88),
        (-0.12, 0.10, 0.92), (-0.06, 0.10, 0.92),
        (-0.12, 0.12, 0.92), (-0.06, 0.12, 0.92),
    ]]
    bm.faces.new([left_eye_verts[0], left_eye_verts[2], left_eye_verts[3], left_eye_verts[1]])

    # Right eye
    right_eye_verts = [bm.verts.new(v) for v in [
        (0.06, 0.10, 0.88), (0.12, 0.10, 0.88),
        (0.06, 0.12, 0.88), (0.12, 0.12, 0.88),
        (0.06, 0.10, 0.92), (0.12, 0.10, 0.92),
        (0.06, 0.12, 0.92), (0.12, 0.12, 0.92),
    ]]
    bm.faces.new([right_eye_verts[0], right_eye_verts[2], right_eye_verts[3], right_eye_verts[1]])

    # ===== LEGS (4 short, squat) =====
    def create_leg(base_x, base_y):
        """Helper to create single leg."""
        leg_verts = [bm.verts.new(v) for v in [
            (base_x - 0.04, base_y - 0.03, 0.1), (base_x + 0.04, base_y - 0.03, 0.1),
            (base_x - 0.04, base_y + 0.03, 0.1), (base_x + 0.04, base_y + 0.03, 0.1),
            (base_x - 0.04, base_y - 0.03, -0.05), (base_x + 0.04, base_y - 0.03, -0.05),
            (base_x - 0.04, base_y + 0.03, -0.05), (base_x + 0.04, base_y + 0.03, -0.05),
        ]]
        bm.faces.new([leg_verts[0], leg_verts[2], leg_verts[3], leg_verts[1]])
        bm.faces.new([leg_verts[4], leg_verts[6], leg_verts[7], leg_verts[5]])
        return leg_verts

    # Front left leg
    create_leg(-0.13, 0.08)
    # Front right leg
    create_leg(0.13, 0.08)
    # Back left leg
    create_leg(-0.13, -0.15)
    # Back right leg
    create_leg(0.13, -0.15)

    # ===== TAIL (curved) =====
    tail_verts = [bm.verts.new(v) for v in [
        (-0.03, -0.25, 0.15), (0.03, -0.25, 0.15),
        (-0.03, -0.35, 0.25), (0.03, -0.35, 0.25),
        (-0.02, -0.45, 0.30), (0.02, -0.45, 0.30),
    ]]
    bm.faces.new([tail_verts[0], tail_verts[1], tail_verts[3], tail_verts[2]])
    bm.faces.new([tail_verts[2], tail_verts[3], tail_verts[5], tail_verts[4]])

    # ===== SADDLE BASE (simple platform on back) =====
    saddle_verts = [bm.verts.new(v) for v in [
        (-0.18, 0.08, 0.4), (0.18, 0.08, 0.4),
        (-0.18, 0.12, 0.4), (0.18, 0.12, 0.4),
        (-0.18, 0.08, 0.0), (0.18, 0.08, 0.0),
        (-0.18, 0.12, 0.0), (0.18, 0.12, 0.0),
    ]]
    # Only visible top face
    bm.faces.new([saddle_verts[0], saddle_verts[2], saddle_verts[3], saddle_verts[1]])

    # Finalize mesh
    bm.to_mesh(mesh)
    bm.free()

    # Set smooth shading
    bpy.ops.object.shade_smooth()

    # Add material slots
    mat_body = bpy.data.materials.new("Camel_Body")
    mat_body.use_nodes = True
    mat_body.node_tree.nodes["Principled BSDF"].inputs[0].default_value = (0.82, 0.71, 0.55, 1.0)
    mesh.materials.append(mat_body)

    print(f"\n✓ Camel base mesh created!")
    print(f"  - Vertices: {len(mesh.vertices)}")
    print(f"  - Faces: {len(mesh.polygons)}")
    print(f"  - Approximate tris: {len(mesh.polygons) * 2}")
    print(f"\nNext steps:")
    print(f"  1. Fine-tune geometry in sculpt mode")
    print(f"  2. Run setup_camel_rig.py to create skeleton")
    print(f"  3. Weight paint for animations")
    return obj

# Run
if __name__ == "__main__":
    create_camel_mesh()
