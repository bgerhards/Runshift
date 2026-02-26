using Godot;

namespace GraplingProject.Checkpoint;

public partial class Checkpoint : Area3D
{
    [Export] public int CheckpointNumber = 0;

    public void _on_body_entered(Node3D body){
        GetNode<CheckpointManager>("/root/CheckpointManager").SetCheckpoint(CheckpointNumber);
    }
}
