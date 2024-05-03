using System.Collections.Generic;

namespace VeraNet.Objects.Scenes.Timers;

/// <summary>
/// Represents an interval timer of a scene.
/// </summary>
public class IntervalTimer : Timer
{
    /// <summary>
    /// The time interval in hours or minutes.
    /// The interval is in minutes if <see cref="IsMinutes"/> is true.
    /// </summary>
    public int Interval { get; init; }
    
    /// <summary>
    /// Specifies if the interval is in minutes.
    /// </summary>
    public bool IsMinutes { get; init; }
    public override TimerType Type => TimerType.Interval;

    internal override Dictionary<string, object> ToJson()
    {
        var dict = base.ToJson();
        dict.Add("interval", GetInterval());
        return dict;
    }
    
    private string GetInterval()
    {
        return $"{Interval}{(IsMinutes ? "m" : "h")}";
    }
    
    public override string ToString()
    {
        return $"{Name} ({GetInterval()})";
    }
}