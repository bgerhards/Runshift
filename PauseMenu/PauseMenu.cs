using Godot;
using System;

public partial class PauseMenu : PanelContainer
{
    [Export] public Button ResumeButton;
    [Export] public Button QuitButton;

    private const string MainMenuScenePath = "res://MainMenu/main-menu.tscn";

    public override void _Ready()
    {
        ResumeButton.Pressed += HandleResumeButton;
        QuitButton.Pressed += HandleQuitButton;
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        HandlePauseMenuToggle(@event);
    }

    private void HandlePauseMenuToggle(InputEvent @event)
    {
        if (@event is not InputEventKey { Keycode: Key.Escape, Pressed: true }) return;
        TogglePauseMenu();
    }
    private void HandleResumeButton()
    {
        TogglePauseMenu();
    }
    
    private void HandleQuitButton()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile(MainMenuScenePath);
    }

    private void TogglePauseMenu()
    {
        GetTree().Paused = !GetTree().Paused;
        Visible = GetTree().Paused;
        Input.MouseMode = GetTree().Paused ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
    }
}
