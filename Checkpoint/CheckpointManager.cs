using Godot;

namespace GraplingProject.Checkpoint;

public partial class CheckpointManager : Node
{
    private int _playerCheckpoint = 0;

    private static readonly Vector3[] CheckpointPositions =
    [
        new(0f, 6.1f, 4f),       // 0: Starting area
        new(20f, 2f, -60f),      // 1: End of original level
        new(75f, 15.0f, -72f),   // 2: City outskirts (Bldg05)
        new(45f, 23.0f, -112f),  // 3: Rising heights (Bldg10)
        new(-2f, 31.0f, -150f),  // 4: Downtown core (Bldg15)
        new(15f, 41.0f, -182f),  // 5: Skyline (Bldg19)
        new(46f, 51.0f, -200f),  // 6: Tower gauntlet (Bldg23)
        new(50f, 81.0f, -228f),  // 7: The Spire summit (SpireTop)
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
