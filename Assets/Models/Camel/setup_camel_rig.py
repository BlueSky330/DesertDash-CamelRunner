"""
Camel Default Model - Blender Rigging Setup Script
Author: Artist1
Purpose: Create skeleton structure and weight painting setup for Camel camel
Target: Low-poly rigged model, <1200 tris, ready for animation

Usage in Blender:
1. Create/import Camel base mesh
2. Run this script in Blender Python console
3. Script will create armature, set bone structure, and create weight paint groups
"""

import bpy
import mathutils

class CamelRigSetup:
    def __init__(self):
        self.armature = None
        self.armature_obj = None

    def create_armature(self):
        """Create armature object and add it to scene."""
        bpy.ops.object.armature_add(
            name="Camel_Armature",
            location=(0, 0, 0)
        )
        self.armature_obj = bpy.context.active_object
        self.armature = self.armature_obj.data
        self.armature.display_type = 'STICK'

    def add_bones(self):
        """Add bone structure for Camel rig."""
        bpy.context.view_layer.objects.active = self.armature_obj
        bpy.ops.object.mode_set(mode='EDIT')

        # Define bone hierarchy and positions
        bones_data = {
            "Root": (0, 0, 0),
            "Hips": (0, 0, 0.1),
            "Spine": (0, 0, 0.25),
            "Chest": (0, 0, 0.4),
            "Neck": (0, 0, 0.65),
            "Head": (0, 0, 0.85),
            "LeftEye": (-0.05, 0.1, 0.9),
            "RightEye": (0.05, 0.1, 0.9),
            "LeftFrontLeg": (-0.15, 0, 0.05),
            "RightFrontLeg": (0.15, 0, 0.05),
            "LeftBackLeg": (-0.15, -0.3, 0.05),
            "RightBackLeg": (0.15, -0.3, 0.05),
            "Tail": (0, -0.4, 0.2),
        }

        # Create bones
        for bone_name, (x, y, z) in bones_data.items():
            bone = self.armature.edit_bones.new(bone_name)
            bone.head = (x, y, z)
            bone.tail = (x, y, z + 0.08)  # Give bones some length

        # Set parent-child relationships
        parent_map = {
            "Root": None,
            "Hips": "Root",
            "Spine": "Hips",
            "Chest": "Spine",
            "Neck": "Chest",
            "Head": "Neck",
            "LeftEye": "Head",
            "RightEye": "Head",
            "LeftFrontLeg": "Chest",
            "RightFrontLeg": "Chest",
            "LeftBackLeg": "Hips",
            "RightBackLeg": "Hips",
            "Tail": "Hips",
        }

        for bone_name, parent_name in parent_map.items():
            if parent_name:
                bone = self.armature.edit_bones[bone_name]
                parent_bone = self.armature.edit_bones[parent_name]
                bone.parent = parent_bone

        bpy.ops.object.mode_set(mode='OBJECT')
        print("✓ Bone structure created")

    def add_blendshapes(self):
        """Create shape keys for facial expressions."""
        # This assumes Camel mesh is already in scene
        mesh_obj = None
        for obj in bpy.context.scene.objects:
            if obj.type == 'MESH' and obj.name.startswith('Camel'):
                mesh_obj = obj
                break

        if not mesh_obj:
            print("⚠ Camel mesh not found - skip blendshapes")
            return

        bpy.context.view_layer.objects.active = mesh_obj

        # Add basis shape key
        if mesh_obj.data.shape_keys is None:
            basis = mesh_obj.shape_key_add(name="Basis")
            basis.interpolation = 'KEY_LINEAR'

        # Add expression shape keys
        expressions = ["Happy", "Startled", "Determined"]
        for expr in expressions:
            shape = mesh_obj.shape_key_add(name=expr)
            shape.interpolation = 'KEY_LINEAR'
            print(f"✓ Added blendshape: {expr}")

    def setup_materials(self):
        """Create material slots for Camel."""
        mesh_obj = None
        for obj in bpy.context.scene.objects:
            if obj.type == 'MESH' and obj.name.startswith('Camel'):
                mesh_obj = obj
                break

        if not mesh_obj:
            return

        # Create materials
        materials_config = {
            "Camel_Body": {
                "use_nodes": True,
                "base_color": (0.82, 0.71, 0.55, 1.0),  # Sandy tan
            },
            "Camel_Saddle": {
                "use_nodes": True,
                "base_color": (0.8, 0.2, 0.2, 1.0),  # Red accent
            },
            "Camel_Bridle": {
                "use_nodes": True,
                "base_color": (0.78, 0.20, 0.20, 1.0),  # Dark red
            },
            "Camel_Eyes": {
                "use_nodes": True,
                "base_color": (0.1, 0.1, 0.1, 1.0),  # Black
            },
        }

        for mat_name, config in materials_config.items():
            mat = bpy.data.materials.new(name=mat_name)
            mat.use_nodes = config["use_nodes"]
            mat.node_tree.nodes["Principled BSDF"].inputs[0].default_value = config["base_color"]
            mesh_obj.data.materials.append(mat)
            print(f"✓ Created material: {mat_name}")

    def add_vertex_groups(self):
        """Create vertex groups for weight painting."""
        mesh_obj = None
        for obj in bpy.context.scene.objects:
            if obj.type == 'MESH' and obj.name.startswith('Camel'):
                mesh_obj = obj
                break

        if not mesh_obj:
            return

        bone_names = [
            "Root", "Hips", "Spine", "Chest", "Neck", "Head",
            "LeftEye", "RightEye",
            "LeftFrontLeg", "RightFrontLeg", "LeftBackLeg", "RightBackLeg",
            "Tail"
        ]

        for bone in bone_names:
            mesh_obj.vertex_groups.new(name=bone)

        print(f"✓ Created {len(bone_names)} vertex groups for weight painting")

    def apply_armature_modifier(self):
        """Add armature modifier to mesh."""
        mesh_obj = None
        for obj in bpy.context.scene.objects:
            if obj.type == 'MESH' and obj.name.startswith('Camel'):
                mesh_obj = obj
                break

        if not mesh_obj:
            return

        bpy.context.view_layer.objects.active = mesh_obj
        modifier = mesh_obj.modifiers.new(name="Armature", type='ARMATURE')
        modifier.object = self.armature_obj
        print("✓ Armature modifier applied to mesh")

    def run(self):
        """Execute full rig setup."""
        print("\n=== Starting Camel Rig Setup ===")
        self.create_armature()
        self.add_bones()
        self.add_vertex_groups()
        self.apply_armature_modifier()
        self.add_blendshapes()
        self.setup_materials()
        print("=== Rig Setup Complete ===\n")
        print("Next steps:")
        print("1. Import/create Camel base mesh")
        print("2. Weight paint each vertex group")
        print("3. Test rig with simple animations")
        print("4. Fine-tune bone positions if needed")

# Run setup
if __name__ == "__main__":
    setup = CamelRigSetup()
    setup.run()
