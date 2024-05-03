using System.Collections.Generic;

namespace VeraNet.Objects.Scenes;

/// <summary>
/// Represents a timer of the scene, witch can be of different types.
/// <seealso cref="Timers.IntervalTimer"/>
/// <seealso cref="Timers.WeekTimer"/>
/// <seealso cref="Timers.MonthTimer"/>
/// <seealso cref="Timers.AbsoluteTimer"/>
/// </summary>
public abstract class Timer
{
    /// <summary>
    /// The timer identifier.
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// The name of the timer.
    /// </summary>
    public string Name { get; init; }
    
    /// <summary>
    /// If the timer is enabled.
    /// </summary>
    public bool Enabled { get; init; }

    /// <summary>
    /// The type of the timer.
    /// </summary>
    public abstract TimerType Type { get; }
    
    /// <summary>
    /// Converts the timer to a dictionary to be serialized to JSON.
    /// </summary>
    internal virtual Dictionary<string, object> ToJson()
    {
        return new Dictionary<string, object>
        {
            { "id", this.Id },
            { "name", this.Name },
            { "enabled", this.Enabled ? "1" : "0" },
            { "type", this.Type }
        };
    }
}

/// <summary>
/// Represents the type of the timer.
/// </summary>
public enum TimerType
{
    Interval = 1,
    DayOfWeek = 2,
    DayOfMonth = 3,
    Absolute = 4
}