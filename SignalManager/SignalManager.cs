using Godot;

namespace GraplingProject.SignalManager;

public partial class SignalManager : Node
{
    [Signal]
    public delegate void ScreenMessageEventHandler(string message);

    [Signal]
    public delegate void CheckpointReachedEventHandler(int checkpointNumber);

    public void EmitScreenMessage(string message)
    {
        EmitSignal(SignalName.ScreenMessage, message);
    }

    public void EmitCheckpointReached(int checkpointNumber)
    {
        EmitSignal(SignalName.CheckpointReached, checkpointNumber);
    }
}

