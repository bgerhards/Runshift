using Godot;
using Godot.Collections;

namespace GraplingProject.player.GrapplingHook;

public static class GrappleDebug
{
    public static void LogRaycastMiss(Vector3 from, Vector3 to, float maxDistance)
    {
        GD.Print("=== RAYCAST MISS ===");
        GD.Print("Ray From: " + from);
        GD.Print("Ray To: " + to);
        GD.Print("Max Raycast Distance: " + maxDistance);
        GD.Print("No objects hit within range");
    }

    public static void LogRaycastHit(Dictionary result, Vector3 rayFrom, float maxDistance)
    {
        var hitPosition = (Vector3)result["position"];
        var hitNormal = (Vector3)result["normal"];
        var hitObject = (Node3D)result["collider"];
        var hitDistance = rayFrom.DistanceTo(hitPosition);

        GD.Print("=== RAYCAST HIT ===");
        GD.Print("Hit Distance: " + hitDistance.ToString("F2") + " units (max: " + maxDistance + ")");
        GD.Print("Hit Position: " + hitPosition);
        GD.Print("Hit Normal: " + hitNormal);
        GD.Print("Hit Object Name: " + hitObject.Name);
        GD.Print("Hit Object Type: " + hitObject.GetType().Name);
        GD.Print("Hit Object Path: " + hitObject.GetPath());
        GD.Print("Hit Object Groups: " + string.Join(", ", hitObject.GetGroups()));
    }

    public static void DrawHitMarker(Vector3 hitPosition, SceneTree tree, float duration = 2.0f)
    {
        var hitMarker = new MeshInstance3D
        {
            Mesh = new SphereMesh { Radius = 0.1f, Height = 0.2f },
            MaterialOverride = new StandardMaterial3D
            {
                ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
                AlbedoColor = new Color(0, 1, 0)
            }
        };

        tree.Root.AddChild(hitMarker);
        hitMarker.GlobalPosition = hitPosition;

        tree.CreateTimer(duration).Timeout += () =>
        {
            if (GodotObject.IsInstanceValid(hitMarker))
                hitMarker.QueueFree();
        };
    }
}

