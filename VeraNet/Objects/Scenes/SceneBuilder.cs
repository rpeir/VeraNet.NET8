#nullable enable
using System.Collections.Generic;
using VeraNet.Objects.Scenes.Actions;

namespace VeraNet.Objects.Scenes;
/// <summary>
/// Class to build a <see cref="Objects.Scene"/>
/// </summary>
public class SceneBuilder(VeraController controller)
{
    public string Name { get; set; }
    
    public Room? Room { get; set; }
    
    public List<Trigger> Triggers { get; set; } = [];

    public List<Timer> Timers { get; set; } = [];
    
    public Dictionary<int, Dictionary<string, List<DeviceCommand>>> Actions { get; set; } = [];
    
    public SceneBuilder WithTrigger(Trigger trigger)
    {
        this.Triggers.Add(trigger);
        return this;
    }
    
    public SceneBuilder WithTimer(Timer timer)
    {
        this.Timers.Add(timer);
        return this;
    }
    
    public SceneBuilder WithAction(int delay, string device, DeviceCommand command)
    {
        // Create the delay dictionary if it doesn't exist
        if (!this.Actions.ContainsKey(delay))
        {
            this.Actions.Add(delay, new Dictionary<string, List<DeviceCommand>>());
        }
        
        // Create the device dictionary if it doesn't exist
        if (!this.Actions[delay].ContainsKey(device))
        {
            this.Actions[delay].Add(device, []);
        }
        
        // Add the action to the device dictionary
        this.Actions[delay][device].Add(command);
        
        return this;
    }
    
    public SceneBuilder WithName(string name)
    {
        this.Name = name;
        return this;
    }
    
    public SceneBuilder WithRoom(Room room)
    {
        this.Room = room;
        return this;
    }
    
    public bool Build()
    {
        if (string.IsNullOrEmpty(this.Name) || this.Room == null) return false;
        
        return controller.CreateScene(this.Name, this.Room, this.Triggers, this.Timers, this.Actions);
    }

}