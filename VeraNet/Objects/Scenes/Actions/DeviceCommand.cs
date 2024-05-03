using System.Collections.Generic;
using System.Linq;

namespace VeraNet.Objects.Scenes.Actions;

/// <summary>
/// Represents a device action. This could be a command to a device. It's the same that can be accessed in the Vera JSON <c>static_data.device_type.Tabs.Controls.states.Command</c> with the request <c>GET /data_request?id=static</c>.
/// </summary>
public class DeviceCommand
{
    /// <summary>
    /// The id of the service of the device that the action belongs to.
    /// </summary>
    public string ServiceId { get; init; }
    
    /// <summary>
    /// The action to be executed.
    /// </summary>
    public string Action { get; init; }
    
    /// <summary>
    /// The parameters of the action. The key is the parameter name and the value is the parameter value.
    /// </summary>
    public Dictionary<string, object> Parameters { get; init; }
    
    /// <summary>
    /// This method converts the command to a dictionary to be serialized to JSON.
    /// It can be used with <see cref="System.Text.Json.JsonSerializer"/>
    /// </summary>
    public Dictionary<string, object> ToJson()
    {
        // main json object
        var json = new Dictionary<string, object>();
        json.Add("service", ServiceId);
        json.Add("action", Action);
        
        // arguments array
        List<Dictionary<string, object>> arguments = [];
        arguments.AddRange(Parameters.Select(argument => 
            new Dictionary<string, object>() { { "name", argument.Key }, { "value", argument.Value } }
        ));

        json.Add("arguments", arguments);
        return json;
    }
}