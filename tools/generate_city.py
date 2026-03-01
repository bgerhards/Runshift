#!/usr/bin/env python3
"""
City Generator for Grapling Project
Generates tscn sub_resources and nodes for a massive rooftop city extension.
Outputs two files:
  - city_subresources.txt (insert before first [node] line)
  - city_nodes.txt (insert before [node name="Player"] line)
"""

import os

# ============================================================
# MATERIALS
# ============================================================
MATERIALS = {
    "mat_a": (0.32, 0.33, 0.36, 1),   # dark concrete
    "mat_b": (0.62, 0.60, 0.57, 1),   # light concrete
    "mat_c": (0.50, 0.33, 0.22, 1),   # brown brick
    "mat_d": (0.38, 0.45, 0.55, 1),   # blue steel
    "mat_e": (0.20, 0.22, 0.28, 1),   # dark tower
    "mat_f": (0.90, 0.55, 0.15, 1),   # hookable orange
}

# ============================================================
# BUILDINGS: (name, x, y, z, w, h, d, material, checkpoint_num_or_None)
# y = center of box, rooftop = y + h/2
# ============================================================
BUILDINGS = [
    # --- Transition from existing level ---
    ("Transition01", 26, 3, -60, 6, 6, 8, "mat_c", None),

    # --- Zone 1: Low Rises (toward Checkpoint 2) ---
    ("Bldg01", 35, 5, -58, 8, 10, 10, "mat_c", None),
    ("Bldg02", 46, 5.5, -56, 6, 11, 8, "mat_b", None),
    ("Bldg03", 56, 5, -60, 10, 10, 10, "mat_a", None),
    ("Bldg04", 68, 6, -64, 8, 12, 8, "mat_c", None),
    ("Bldg05", 75, 7, -72, 10, 14, 10, "mat_b", 2),

    # --- Zone 2: Rising Heights (toward Checkpoint 3) ---
    ("Bldg06", 82, 9, -80, 6, 18, 6, "mat_d", None),
    ("Bldg07", 75, 8, -88, 8, 16, 8, "mat_a", None),
    ("Bldg08", 65, 10, -95, 10, 20, 10, "mat_b", None),
    ("Bldg09", 55, 10, -105, 8, 20, 8, "mat_c", None),
    ("Bldg10", 45, 11, -112, 10, 22, 10, "mat_a", 3),

    # --- Zone 3: Downtown Core (toward Checkpoint 4) ---
    ("Bldg11", 38, 13, -120, 6, 26, 6, "mat_d", None),
    ("Bldg12", 28, 12, -128, 10, 24, 12, "mat_b", None),
    ("Bldg13", 18, 15, -136, 8, 30, 8, "mat_e", None),
    ("Bldg14", 8, 13, -142, 6, 26, 6, "mat_c", None),
    ("Bldg15", -2, 15, -150, 10, 30, 10, "mat_a", 4),

    # --- Zone 4: Skyline (toward Checkpoint 5) ---
    ("Bldg16", -10, 17.5, -158, 6, 35, 6, "mat_d", None),
    ("Bldg17", -4, 17.5, -168, 8, 35, 8, "mat_e", None),
    ("Bldg18", 6, 20, -175, 6, 40, 6, "mat_a", None),
    ("Bldg19", 15, 20, -182, 10, 40, 10, "mat_b", 5),

    # --- Zone 5: Tower Gauntlet (toward Checkpoint 6) ---
    ("Bldg20", 22, 22.5, -190, 4, 45, 4, "mat_e", None),
    ("Bldg21", 30, 22.5, -196, 4, 45, 4, "mat_d", None),
    ("Bldg22", 38, 25, -192, 4, 50, 4, "mat_e", None),
    ("Bldg23", 46, 25, -200, 6, 50, 6, "mat_a", 6),

    # --- Zone 6: The Spire (Final) ---
    ("SpireBase", 50, 27.5, -210, 12, 55, 12, "mat_d", None),
    ("SpireMid",  50, 32.5, -220, 8, 65, 8, "mat_e", None),
    ("SpireTop",  50, 40, -228, 4, 80, 4, "mat_a", 7),
]

# Decoration buildings (background, no checkpoint, collision_layer=2 still)
DECO_BUILDINGS = [
    ("Deco01", 45, 12.5, -50, 6, 25, 6, "mat_a"),
    ("Deco02", 62, 10, -48, 8, 20, 8, "mat_b"),
    ("Deco03", 90, 15, -75, 6, 30, 6, "mat_d"),
    ("Deco04", 85, 12.5, -95, 10, 25, 10, "mat_c"),
    ("Deco05", 22, 20, -125, 4, 40, 4, "mat_e"),
    ("Deco06", -15, 17.5, -140, 8, 35, 8, "mat_a"),
    ("Deco07", -22, 12.5, -162, 6, 25, 6, "mat_c"),
    ("Deco08", 28, 22.5, -178, 4, 45, 4, "mat_d"),
    ("Deco09", 62, 25, -208, 6, 50, 6, "mat_e"),
    ("Deco10", 38, 20, -218, 8, 40, 8, "mat_a"),
]

# ============================================================
# HOOKABLE TARGETS: (name, x, y, z)
# Placed between buildings that require grappling
# ============================================================
HOOK_TARGETS = [
    # Zone 1 — Y raised to +4 above destination rooftop
    ("Hook01", 30, 14, -59),     # transition -> Bldg01 (rooftop 10)
    ("Hook02", 62, 16, -62),     # Bldg03 -> Bldg04 (rooftop 12)
    ("Hook03", 65, 16, -63),     # closer to Bldg04 (rooftop 12)

    # Zone 1->2
    ("Hook04", 78, 22, -76),     # Bldg05 -> Bldg06 (rooftop 18)
    ("Hook05", 80, 22, -78),     # (rooftop 18)

    # Zone 2
    ("Hook06", 78, 20, -84),     # Bldg06 -> Bldg07 (rooftop 16)
    ("Hook07", 70, 24, -92),     # Bldg07 -> Bldg08 (rooftop 20)
    ("Hook08", 60, 24, -100),    # Bldg08 -> Bldg09 (rooftop 20)

    # Zone 2->3
    ("Hook09", 42, 30, -116),    # Bldg10 -> Bldg11 (rooftop 26)
    ("Hook10", 40, 30, -118),    # (rooftop 26)

    # Zone 3
    ("Hook11", 23, 34, -132),    # Bldg12 -> Bldg13 (rooftop 30)
    ("Hook12", 13, 30, -139),    # Bldg13 -> Bldg14 (rooftop 26)
    ("Hook13", 3, 34, -146),     # Bldg14 -> Bldg15 (rooftop 30)

    # Zone 3->4
    ("Hook14", -6, 39, -154),    # Bldg15 -> Bldg16 (rooftop 35)
    ("Hook15", -7, 39, -163),    # Bldg16 -> Bldg17 (rooftop 35)

    # Zone 4
    ("Hook16", 1, 44, -172),     # Bldg17 -> Bldg18 (rooftop 40)
    ("Hook17", 10, 44, -179),    # Bldg18 -> Bldg19 (rooftop 40)

    # Zone 4->5
    ("Hook18", 18, 49, -186),    # Bldg19 -> Bldg20 (rooftop 45)
    ("Hook19", 26, 49, -193),    # Bldg20 -> Bldg21 (rooftop 45)
    ("Hook20", 34, 54, -194),    # Bldg21 -> Bldg22 (rooftop 50)
    ("Hook21", 42, 54, -196),    # Bldg22 -> Bldg23 (rooftop 50)

    # Zone 5->6 (Spire approach)
    ("Hook22", 48, 59, -205),    # Bldg23 -> SpireBase (rooftop 55)
    ("Hook23", 50, 69, -213),    # SpireBase -> SpireMid (rooftop 65)
    ("Hook24", 50, 69, -215),    # stepping stone (rooftop 65)
    ("Hook25", 50, 84, -224),    # SpireMid -> SpireTop (rooftop 80)
    ("Hook26", 50, 84, -226),    # final approach (rooftop 80)
]

# ============================================================
# MOVING PLATFORMS: (name, start_x, start_y, start_z, end_x, end_y, end_z)
# ============================================================
MOVING_PLATFORMS = [
    ("CityMovPlat01", 60, 20, -100, 56, 20, -104),    # Between Bldg08 and Bldg09
    ("CityMovPlat02", -2, 35, -170, 3, 37, -173),     # Between Bldg17 and Bldg18
    ("CityMovPlat03", 50, 65, -224, 50, 74, -227),    # Spire ascent
]


def collect_unique_sizes(buildings, deco):
    """Collect unique (w, h, d) sizes and assign IDs."""
    sizes = {}
    idx = 1
    for blist in [buildings, deco]:
        for b in blist:
            if len(b) == 8:
                _, _, _, _, w, h, d, _ = b
            else:
                _, _, _, _, w, h, d, _, _ = b
            key = (w, h, d)
            if key not in sizes:
                sizes[key] = f"city_{idx:02d}"
                idx += 1
    return sizes


def generate_sub_resources(sizes):
    """Generate all new sub_resource blocks."""
    lines = []
    lines.append("")

    # Materials
    for mat_id, color in MATERIALS.items():
        lines.append(f'[sub_resource type="StandardMaterial3D" id="{mat_id}"]')
        lines.append(f'albedo_color = Color({color[0]}, {color[1]}, {color[2]}, {color[3]})')
        lines.append("")

    # Box meshes and shapes for each unique size
    for (w, h, d), sid in sorted(sizes.items(), key=lambda x: x[1]):
        lines.append(f'[sub_resource type="BoxMesh" id="bm_{sid}"]')
        lines.append(f'size = Vector3({w}, {h}, {d})')
        lines.append("")
        lines.append(f'[sub_resource type="BoxShape3D" id="bs_{sid}"]')
        lines.append(f'size = Vector3({w}, {h}, {d})')
        lines.append("")

    return lines


def fmt_pos(x, y, z):
    """Format a Transform3D translation."""
    return f"Transform3D(1, 0, 0, 0, 1, 0, 0, 0, 1, {x}, {y}, {z})"


def generate_building_nodes(buildings, deco, sizes, parent="Floors/City"):
    """Generate all building nodes."""
    lines = []

    # Container node
    lines.append(f'[node name="City" type="Node3D" parent="Floors"]')
    lines.append("")

    # Main buildings
    for b in buildings:
        if len(b) == 9:
            name, x, y, z, w, h, d, mat, cp = b
        else:
            name, x, y, z, w, h, d, mat = b
            cp = None

        sid = sizes[(w, h, d)]

        lines.append(f'[node name="{name}" type="StaticBody3D" parent="{parent}"]')
        lines.append(f'transform = {fmt_pos(x, y, z)}')
        lines.append(f'collision_layer = 2')
        lines.append("")

        lines.append(f'[node name="MeshInstance3D" type="MeshInstance3D" parent="{parent}/{name}"]')
        lines.append(f'mesh = SubResource("bm_{sid}")')
        lines.append(f'surface_material_override/0 = SubResource("{mat}")')
        lines.append("")

        lines.append(f'[node name="CollisionShape3D" type="CollisionShape3D" parent="{parent}/{name}"]')
        lines.append(f'shape = SubResource("bs_{sid}")')
        lines.append("")

        # Add checkpoint if specified
        if cp is not None:
            # Checkpoint positioned on top of building
            cp_y_offset = h / 2.0 + 0.5
            lines.append(f'[node name="Checkpoint{cp:03d}" parent="{parent}/{name}" instance=ExtResource("3_3wy1v")]')
            lines.append(f'transform = {fmt_pos(0, cp_y_offset, 0)}')
            lines.append(f'CheckpointNumber = {cp}')
            lines.append("")

    # Decoration buildings
    for b in deco:
        name, x, y, z, w, h, d, mat = b
        sid = sizes[(w, h, d)]

        lines.append(f'[node name="{name}" type="StaticBody3D" parent="{parent}"]')
        lines.append(f'transform = {fmt_pos(x, y, z)}')
        lines.append(f'collision_layer = 2')
        lines.append("")

        lines.append(f'[node name="MeshInstance3D" type="MeshInstance3D" parent="{parent}/{name}"]')
        lines.append(f'mesh = SubResource("bm_{sid}")')
        lines.append(f'surface_material_override/0 = SubResource("{mat}")')
        lines.append("")

        lines.append(f'[node name="CollisionShape3D" type="CollisionShape3D" parent="{parent}/{name}"]')
        lines.append(f'shape = SubResource("bs_{sid}")')
        lines.append("")

    return lines


def generate_hook_nodes(hooks, parent="Floors/City"):
    """Generate hookable target nodes."""
    lines = []

    lines.append(f'[node name="HookTargets" type="Node3D" parent="{parent}"]')
    lines.append("")

    for name, x, y, z in hooks:
        lines.append(f'[node name="{name}" type="StaticBody3D" parent="{parent}/HookTargets" groups=["Hookable"]]')
        lines.append(f'transform = {fmt_pos(x, y, z)}')
        lines.append(f'collision_layer = 4')
        lines.append(f'collision_mask = 3')
        lines.append("")

        lines.append(f'[node name="MeshInstance3D" type="MeshInstance3D" parent="{parent}/HookTargets/{name}"]')
        lines.append(f'mesh = SubResource("BoxMesh_pjrb6")')
        lines.append(f'surface_material_override/0 = SubResource("mat_f")')
        lines.append("")

        lines.append(f'[node name="CollisionShape3D" type="CollisionShape3D" parent="{parent}/HookTargets/{name}"]')
        lines.append(f'shape = SubResource("BoxShape3D_xwkvk")')
        lines.append("")

    return lines


def generate_moving_platform_nodes(platforms, parent="Floors/City"):
    """Generate moving platform nodes."""
    lines = []

    for name, sx, sy, sz, ex, ey, ez in platforms:
        lines.append(f'[node name="{name}" type="Node3D" parent="{parent}"]')
        lines.append("")

        lines.append(f'[node name="AnimatableBody3D" type="AnimatableBody3D" parent="{parent}/{name}"]')
        lines.append(f'transform = {fmt_pos(sx, sy, sz)}')
        lines.append(f'collision_layer = 2')
        lines.append(f'physics_material_override = SubResource("PhysicsMaterial_3wy1v")')
        lines.append(f'script = ExtResource("2_c651c")')
        lines.append(f'StartPosition = Vector3({sx}, {sy}, {sz})')
        lines.append(f'EndPosition = Vector3({ex}, {ey}, {ez})')
        lines.append("")

        lines.append(f'[node name="MeshInstance3D" type="MeshInstance3D" parent="{parent}/{name}/AnimatableBody3D"]')
        lines.append(f'mesh = SubResource("BoxMesh_kfbq2")')
        lines.append("")

        lines.append(f'[node name="CollisionShape3D" type="CollisionShape3D" parent="{parent}/{name}/AnimatableBody3D"]')
        lines.append(f'shape = SubResource("BoxShape3D_4dugh")')
        lines.append("")

    return lines


def main():
    sizes = collect_unique_sizes(BUILDINGS, DECO_BUILDINGS)

    sub_resources = generate_sub_resources(sizes)
    building_nodes = generate_building_nodes(BUILDINGS, DECO_BUILDINGS, sizes)
    hook_nodes = generate_hook_nodes(HOOK_TARGETS)
    platform_nodes = generate_moving_platform_nodes(MOVING_PLATFORMS)

    out_dir = os.path.dirname(os.path.abspath(__file__))

    with open(os.path.join(out_dir, "city_subresources.txt"), "w") as f:
        f.write("\n".join(sub_resources))

    with open(os.path.join(out_dir, "city_nodes.txt"), "w") as f:
        all_nodes = building_nodes + hook_nodes + platform_nodes
        f.write("\n".join(all_nodes))

    # Print checkpoint positions for CheckpointManager.cs
    print("=== CHECKPOINT POSITIONS ===")
    for b in BUILDINGS:
        name, x, y, z, w, h, d, mat, cp = b if len(b) == 9 else (*b, None)
        if cp is not None:
            rooftop_y = y + h / 2.0 + 1.0
            print(f"  Checkpoint {cp}: new({x}f, {rooftop_y}f, {z}f),")

    print(f"\nGenerated {len(sizes)} unique building sizes")
    print(f"Generated {len(BUILDINGS)} main buildings + {len(DECO_BUILDINGS)} decoration buildings")
    print(f"Generated {len(HOOK_TARGETS)} hookable targets")
    print(f"Generated {len(MOVING_PLATFORMS)} moving platforms")

    # Count total sub_resources and nodes
    print(f"Sub-resources: {len(sub_resources)} lines")
    print(f"Nodes: {len(building_nodes) + len(hook_nodes) + len(platform_nodes)} lines")


if __name__ == "__main__":
    main()
