using Godot;

namespace GraplingProject.MainMenu;

public partial class MainMenu : Node2D
{
    [Export] public Button StartButton;
    [Export] public Button QuitButton;

    private const string MainGameScenePath = "res://main/main.tscn";
    
    public override void _Ready()
    {
        StartButton.Pressed += HandleStartButton;
        QuitButton.Pressed += HandleQuitButton;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    private void HandleQuitButton()
    {
        GetTree().Quit();
    }

    private void HandleStartButton()
    {
        GetTree().ChangeSceneToFile(MainGameScenePath);
    }
}
