using System.Collections.Generic;
using System.Linq;

namespace VeraNet.Objects.Scenes;

/// <summary>
/// Class that represents a scene trigger.
/// </summary>
public class Trigger
{
    /// <summary>
    /// Represents the name of the trigger.
    /// </summary>
    public string Name { get; init; }
    
    /// <summary>
    /// Represents if the trigger is enabled.
    /// </summary>
    public bool Enabled { get; init; }
    
    /// <summary>
    /// Represents the device id of the trigger.
    /// </summary>
    public int DeviceId { get; init; }
    
    /// <summary>
    /// Represents the id of the trigger.
    /// This corresponds to the id of event in the eventList2 in the static file of the device.
    /// This can be obtained with <c>data_request?id=static</c>
    /// </summary>
    public int Template { get; init; }
    
    /// <summary>
    /// Represents the arguments of the trigger.
    /// The key is the id of the argument and the value is the value of the argument.
    /// This should match the arguments of the event in the eventList2 in the static file of the device.
    /// </summary>
    public Dictionary<int, string> Arguments { get; init; }
    
    /// <summary>
    /// Returns a dictionary with the trigger properties to be serialized to JSON.
    /// </summary>
    /// <returns>A dictionary with the trigger properties, the key is the property name and the value is the property value.</returns>
    public Dictionary<string, object> ToJson()
    {   
        // Convert the arguments to a list of dictionaries
        var args = this.Arguments.Select(arg => new Dictionary<string, string> { { "id", arg.Key.ToString() }, { "value", arg.Value } }).ToList();
        
        // Create a dictionary with the trigger properties
        var dict = new Dictionary<string, object>
        {
            { "name", this.Name },
            { "enabled", this.Enabled ? 1 : 0 },
            { "template", this.Template },
            { "device", DeviceId },
            { "arguments", args }
        };
        // Serialize the dictionary to a json string
        return dict;
    }
}