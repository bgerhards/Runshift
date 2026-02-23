using Godot;

namespace GraplingProject.player;

public partial class Player : CharacterBody3D
{
    [Export] public float Speed = 5.0f;
    [Export] public float JumpVelocity = 4.5f;
    [Export] public float MouseSensitivity = 0.002f;
    [Export] public float MaxRaycastDistance = 200.0f;
    [Export] public float GrappleSpeed = 14.0f;
    [Export] public bool Debugging;
    [Export] public Node3D Head;

    private bool _grappling;
    private Vector3 _grapplePosition;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        CheckForMouseClick(@event);
        CheckForMouseMovement(@event);
    }

    private void CheckForMouseMovement(InputEvent @event)
    {
        if (@event is not InputEventMouseMotion mouseMotion) return;
        RotateY(-mouseMotion.Relative.X * MouseSensitivity);

        Head.RotateX(-mouseMotion.Relative.Y * MouseSensitivity);

        var rotation = Head.Rotation;
        rotation.X = Mathf.Clamp(rotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(90));
        Head.Rotation = rotation;
    }

    private void CheckForMouseClick(InputEvent @event)
    {
        if (@event is not InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true }) return;

        HandleGrapplingHook();
    }

    private void HandleGrapplingHook()
    {
        if (_grappling)
        {
            _grappling = false;
            return;
        }

        var camera = GetNode<Camera3D>("Head/Camera3D");
        var physicsRayQueryParameters3D = new PhysicsRayQueryParameters3D();
        physicsRayQueryParameters3D.From = camera.GlobalTransform.Origin;
        physicsRayQueryParameters3D.To =
            physicsRayQueryParameters3D.From - camera.GlobalTransform.Basis.Z * MaxRaycastDistance;

        var spaceState = GetWorld3D().DirectSpaceState;
        var result = spaceState.IntersectRay(physicsRayQueryParameters3D);

        if (Debugging & result.Count <= 0)
        {
            GD.Print("=== RAYCAST MISS ===");
            GD.Print("Ray From: " + physicsRayQueryParameters3D.From);
            GD.Print("Ray To: " + physicsRayQueryParameters3D.To);
            GD.Print("Max Raycast Distance: " + MaxRaycastDistance);
            GD.Print("No objects hit within range");
            return;
        }

        var hitObject = (Node3D)result["collider"];

        if (!hitObject.GetGroups().Contains("Hookable")) return;

        _grappling = true;
        _grapplePosition = physicsRayQueryParameters3D.To;
        GD.Print("Grapple hit: " + hitObject.Name);

        if (!Debugging) return;
        var hitPosition = (Vector3)result["position"];
        var hitNormal = (Vector3)result["normal"];
        var hitDistance = physicsRayQueryParameters3D.From.DistanceTo(hitPosition);

        GD.Print("=== RAYCAST HIT ===");
        GD.Print("Hit Distance: " + hitDistance.ToString("F2") + " units (max: " + MaxRaycastDistance + ")");
        GD.Print("Hit Position: " + hitPosition);
        GD.Print("Hit Normal: " + hitNormal);
        GD.Print("Hit Object Name: " + hitObject.Name);
        GD.Print("Hit Object Type: " + hitObject.GetType().Name);
        GD.Print("Hit Object Path: " + hitObject.GetPath());
        GD.Print("Hit Object Groups: " + string.Join(", ", hitObject.GetGroups()));

        DrawHitPointDebug(hitPosition);
    }

    private void DrawHitPointDebug(Vector3 hitPosition)
    {
        var hitMarker = new MeshInstance3D();
        hitMarker.Mesh = new SphereMesh() { Radius = 0.1f, Height = 0.2f };

        var material = new StandardMaterial3D();
        material.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
        material.AlbedoColor = new Color(0, 1, 0);
        hitMarker.MaterialOverride = material;

        GetTree().Root.AddChild(hitMarker);
        hitMarker.GlobalPosition = hitPosition;

        GetTree().CreateTimer(2.0).Timeout += () =>
        {
            if (IsInstanceValid(hitMarker))
            {
                hitMarker.QueueFree();
            }
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_grappling)
        {
            HandlePlayerGrappling();
        }
        else
        {
            HandlePlayerMovement(delta);
        }
    }

    private void HandlePlayerGrappling()
    {
        var toGrapple = _grapplePosition - GlobalPosition;
        var directionToGrapple = toGrapple.Normalized();
        Velocity = directionToGrapple * GrappleSpeed;

        MoveAndSlide();

        var toGrappleAfterMove = _grapplePosition - GlobalPosition;
        var closeEnough = toGrappleAfterMove.Length() < 1.5f;
        var overshot = toGrapple.Dot(toGrappleAfterMove) < 0;

        if (closeEnough || overshot)
        {
            _grappling = false;
        }
    }

    private void HandlePlayerMovement(double delta)
    {
        var moveSpeed = Input.IsActionPressed("KEY_SPRINT") ? Speed * 1.5f : Speed;
        var velocity = Velocity;

        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        var inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        var direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * moveSpeed;
            velocity.Z = direction.Z * moveSpeed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, moveSpeed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, moveSpeed);
        }

        Velocity = velocity;
        MoveAndSlide();
        if (!(Position.Y <= -5)) return;
        Position = new Vector3(0, 1.5f, 4);
        RotationDegrees = new Vector3(0, 0, 0);
        Head.RotationDegrees = new Vector3(0, 0, 0);
    }
}
