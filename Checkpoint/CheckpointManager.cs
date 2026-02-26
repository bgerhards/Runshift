using Godot;

namespace GraplingProject.Checkpoint;

public partial class CheckpointManager : Node
{
    private int _playerCheckpoint = 0;

    private static readonly Vector3[] CheckpointPositions =
    [
        new(0f, 1.5f, 4f),
        new(20f, 2f, -60f)
    ];
    
    public void SetCheckpoint(int checkpoint)
    {
        GD.Print("Setting Player Checkpoint");
        GD.Print(checkpoint);
        _playerCheckpoint = checkpoint;
    }
    public Vector3 GetCheckpointVector3()
    {
        GD.Print(_playerCheckpoint);
        GD.Print(CheckpointPositions[_playerCheckpoint]);
        return CheckpointPositions[_playerCheckpoint];
    }
}
