using System.Collections.Generic;

namespace VeraNet.Objects.Scenes.Actions;

/// <summary>
/// Represents a device control. A device control can have multiple states, each state have a command to be executed.
/// </summary>
/// <example>
/// To help understand, a light bulb can have device control to toggle the light on and off, and another device control to change the color.
/// A toggle device control of a light bulb can have two states: "on" and "off", each state have a command to execute the action.
/// In other hand, a color device control of a light bulb can have multiple states: "red", "green", "blue", "white", etc.
/// </example>
public class DeviceControl
{
    /// <summary>
    /// The control type of the device control.
    /// </summary>
    /// <example>
    /// multi_state_button, label, variable, etc.
    /// </example>
    public string ControlType { get; init; }
    
    /// <summary>
    /// The states of the device control.
    /// </summary>
    public List<DeviceControlState> States { get; init; }   
}