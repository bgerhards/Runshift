using Godot;

namespace GraplingProject.LevelGui;

public partial class MessageHandler : Node
{
    [Export] public VBoxContainer MessageContianer;
    [Export] public float DisplayDuration = 5.0f;

    private static readonly Color MessageColor = new(1f, 1f, 1f);

    public override void _Ready()
    {
        var signalManager = GetNode<SignalManager.SignalManager>("/root/SignalManager");
        signalManager.ScreenMessage += OnScreenMessage;
    }

    private void OnScreenMessage(string message)
    {
        var label = new Label
        {
            Text = message,
            HorizontalAlignment = HorizontalAlignment.Right,
        };
        label.AddThemeColorOverride("font_color", MessageColor);
        label.AddThemeFontSizeOverride("font_size", 20);

        MessageContianer.AddChild(label);

        var timer = GetTree().CreateTimer(DisplayDuration);
        timer.Timeout += () =>
        {
            if (!IsInstanceValid(label)) return;
            var tween = CreateTween();
            tween.TweenProperty(label, "modulate:a", 0.0f, 0.5f);
            tween.TweenCallback(Callable.From(() =>
            {
                if (IsInstanceValid(label))
                {
                    label.QueueFree();
                }
            }));
        };
    }
}
