using Godot;
using Godot.Collections;

namespace GraplingProject.player.GrapplingHook;

public class GrapplingHook
{
    private readonly float _maxDistance;
    private readonly float _speed;
    private readonly GrappleRope _rope;

    public bool IsGrappling { get; private set; }
    public Vector3 GrapplePoint { get; private set; }

    public GrapplingHook(float maxDistance, float speed, float ropeRadius)
    {
        _maxDistance = maxDistance;
        _speed = speed;
        _rope = new GrappleRope(ropeRadius);
    }

    public Dictionary TryFireOrCancel(Camera3D camera, PhysicsDirectSpaceState3D spaceState, SceneTree tree)
    {
        if (IsGrappling)
        {
            Cancel();
            return null;
        }

        var result = RaycastForHookable(camera, spaceState);
        if (result == null) return null;

        IsGrappling = true;
        GrapplePoint = (Vector3)result["position"];
        _rope.Create(tree);

        return result;
    }

    public bool IsLookingAtHookable(Camera3D camera, PhysicsDirectSpaceState3D spaceState)
    {
        return RaycastForHookable(camera, spaceState) != null;
    }

    private Dictionary RaycastForHookable(Camera3D camera, PhysicsDirectSpaceState3D spaceState)
    {
        var query = new PhysicsRayQueryParameters3D
        {
            From = camera.GlobalTransform.Origin,
            To = camera.GlobalTransform.Origin - camera.GlobalTransform.Basis.Z * _maxDistance
        };

        var result = spaceState.IntersectRay(query);
        if (result.Count <= 0) return null;

        var hitObject = (Node3D)result["collider"];
        return hitObject.GetGroups().Contains("Hookable") ? result : null;
    }

    public bool ProcessMovement(CharacterBody3D player)
    {
        if (!IsGrappling) return false;

        var toGrapple = GrapplePoint - player.GlobalPosition;
        player.Velocity = toGrapple.Normalized() * _speed;
        player.MoveAndSlide();

        var toGrappleAfterMove = GrapplePoint - player.GlobalPosition;
        var closeEnough = toGrappleAfterMove.Length() < 1.5f;
        var overshot = toGrapple.Dot(toGrappleAfterMove) < 0;

        if (closeEnough || overshot)
        {
            Cancel();
            return false;
        }

        return true;
    }

    public void UpdateRope(Vector3 ropeOrigin)
    {
        _rope.Update(ropeOrigin, GrapplePoint);
    }

    public void Cancel()
    {
        IsGrappling = false;
        _rope.Destroy();
    }
}



