using Godot;
using GraplingProject.Checkpoint;
using GraplingProject.player.GrapplingHook;

namespace GraplingProject.player;

public partial class Player : CharacterBody3D
{
    [Export] public float Speed = 5.0f;
    [Export] public float JumpVelocity = 4.5f;
    [Export] public float MouseSensitivity = 0.002f;
    [Export] public float MaxRaycastDistance = 200.0f;
    [Export] public float GrappleSpeed = 10.0f;
    [Export] public float GrappleRopeRadius = 0.01f;
    [Export] public bool Debugging;
    [Export] public Node3D Head;

    private const float RopeOffsetY = -0.4f;
    private const float RopeOffsetZ = -0.3f;
    private const float RespawnThresholdY = -15f;

    private GrapplingHook.GrapplingHook _grapplingHook;
    private Camera3D _camera;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        _camera = GetNode<Camera3D>("Head/Camera3D");
        _grapplingHook = new GrapplingHook.GrapplingHook(MaxRaycastDistance, GrappleSpeed, GrappleRopeRadius);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
            HandleMouseLook(mouseMotion);

        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
            HandleGrappleInput();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_grapplingHook.IsGrappling)
        {
            if (Input.IsActionJustPressed("ui_accept"))
            {
                _grapplingHook.Cancel();
                return;
            }

            _grapplingHook.ProcessMovement(this);
            _grapplingHook.UpdateRope(GetRopeOrigin());
        }
        else
        {
            HandleMovement(delta);
        }
    }

    private void HandleMouseLook(InputEventMouseMotion mouseMotion)
    {
        RotateY(-mouseMotion.Relative.X * MouseSensitivity);
        Head.RotateX(-mouseMotion.Relative.Y * MouseSensitivity);

        var rotation = Head.Rotation;
        rotation.X = Mathf.Clamp(rotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(90));
        Head.Rotation = rotation;
    }

    private void HandleGrappleInput()
    {
        var spaceState = GetWorld3D().DirectSpaceState;
        var result = _grapplingHook.TryFireOrCancel(_camera, spaceState, GetTree());

        if (result == null) return;

        GD.Print("Grapple hit: " + ((Node3D)result["collider"]).Name);

        if (!Debugging) return;
        GrappleDebug.LogRaycastHit(result, _camera.GlobalTransform.Origin, MaxRaycastDistance);
        GrappleDebug.DrawHitMarker((Vector3)result["position"], GetTree());
    }

    private void HandleMovement(double delta)
    {
        var moveSpeed = Input.IsActionPressed("KEY_SPRINT") ? Speed * 1.5f : Speed;
        var velocity = Velocity;

        if (!IsOnFloor())
            velocity += GetGravity() * (float)delta;

        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
            velocity.Y = JumpVelocity;

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
        CheckRespawn();
    }

    private void CheckRespawn()
    {
        if (Position.Y > RespawnThresholdY) return;
        
        Position = GetNode<CheckpointManager>("/root/CheckpointManager").GetCheckpointVector3();
        RotationDegrees = Vector3.Zero;
        Head.RotationDegrees = Vector3.Zero;
    }

    private Vector3 GetRopeOrigin()
    {
        return _camera.GlobalPosition
               + _camera.GlobalTransform.Basis.Y * RopeOffsetY
               + _camera.GlobalTransform.Basis.Z * RopeOffsetZ;
    }
}
