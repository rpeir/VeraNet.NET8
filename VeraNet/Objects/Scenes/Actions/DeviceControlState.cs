namespace VeraNet.Objects.Scenes.Actions;

/// <summary>
/// Represents a device's control state.
/// </summary>
public class DeviceControlState
{
    /// <summary>
    /// The name / description of the state.
    /// </summary>
    public string Label { get; init; }
    
    /// <summary>
    /// The control code of the state.
    /// </summary>
    public string ControlCode { get; init; }
    
    /// <summary>
    /// The command to be executed with this state.
    /// </summary>
    public DeviceCommand Command { get; init; }
}