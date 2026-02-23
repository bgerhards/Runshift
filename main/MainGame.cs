using Godot;
using System;

public partial class MainGame : Node3D
{
    public override void _Ready()
    {
        GD.Print("Readying the main game");
        Input.MouseMode = Input.MouseModeEnum.Captured;
        GD.Print(Input.MouseMode);
    }
}
