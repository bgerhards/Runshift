using Godot;

namespace GraplingProject.player.GrapplingHook;

public class GrappleRope
{
    private MeshInstance3D _mesh;
    private readonly float _radius;

    public GrappleRope(float radius = 0.01f)
    {
        _radius = radius;
    }

    public bool IsActive => _mesh != null && GodotObject.IsInstanceValid(_mesh);

    public void Create(SceneTree tree)
    {
        Destroy();

        _mesh = new MeshInstance3D
        {
            Mesh = new CylinderMesh
            {
                TopRadius = _radius,
                BottomRadius = _radius,
                Height = 1.0f
            },
            MaterialOverride = new StandardMaterial3D
            {
                ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
                AlbedoColor = new Color(0.6f, 0.4f, 0.1f)
            }
        };

        tree.Root.AddChild(_mesh);
        _mesh.ExtraCullMargin = 16384.0f;
        _mesh.IgnoreOcclusionCulling = true;
    }

    public void Destroy()
    {
        if (!IsActive) return;
        _mesh.QueueFree();
        _mesh = null;
    }

    public void Update(Vector3 from, Vector3 to)
    {
        if (!IsActive) return;

        var ropeVector = to - from;
        var ropeLength = ropeVector.Length();
        if (ropeLength < 0.001f) return;

        var midpoint = (from + to) * 0.5f;
        var yAxis = ropeVector / ropeLength;

        var hint = Mathf.Abs(yAxis.Dot(Vector3.Up)) < 0.99f ? Vector3.Up : Vector3.Right;
        var xAxis = hint.Cross(yAxis).Normalized();
        var zAxis = yAxis.Cross(xAxis).Normalized();

        var basis = new Basis(xAxis, yAxis * ropeLength, zAxis);
        _mesh.GlobalTransform = new Transform3D(basis, midpoint);
    }
}

