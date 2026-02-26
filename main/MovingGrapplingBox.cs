using Godot;
using System;

public partial class MovingGrapplingBox : AnimatableBody3D
{
    [Export] public Vector3 StartPosition;
    [Export] public Vector3 Position2;
    [Export] public Vector3 EndPosition;
    [Export] public float Speed = 4.0f;
    public override void _Ready()
    {
        Move();
    }

    public override void _Process(double delta)
    {
    }

    private void Move()
    {
        var moveTween = CreateTween();
        moveTween.SetProcessMode(Tween.TweenProcessMode.Physics);
        moveTween.SetLoops();

        moveTween.TweenProperty(this, "global_position", StartPosition, Speed)
            .SetTrans(Tween.TransitionType.Linear)
            .SetEase(Tween.EaseType.In)
            .SetDelay(0.5f);

        moveTween.TweenProperty(this, "global_position", Position2, Speed)
            .SetTrans(Tween.TransitionType.Linear)
            .SetDelay(0.5f);

        moveTween.TweenProperty(this, "global_position", EndPosition, Speed)
            .SetTrans(Tween.TransitionType.Linear)
            .SetEase(Tween.EaseType.Out)
            .SetDelay(0.5f);

        moveTween.TweenProperty(this, "global_position", Position2, Speed)
            .SetTrans(Tween.TransitionType.Linear);

        moveTween.TweenProperty(this, "global_position", StartPosition, Speed)
            .SetTrans(Tween.TransitionType.Linear)
            .SetEase(Tween.EaseType.Out);
    }
}