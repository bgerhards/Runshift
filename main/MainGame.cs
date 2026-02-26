using Godot;
using System;
using GraplingProject.Checkpoint;

public partial class MainGame : Node3D
{
    public override void _Ready()
    {
        GD.Print("Readying the main game");
        GD.Print("Starting new game with checkpoint 0");
        GetNode<CheckpointManager>("/root/CheckpointManager").SetCheckpoint(0);
        Input.MouseMode = Input.MouseModeEnum.Captured;
        GD.Print(Input.MouseMode);
    }
}
