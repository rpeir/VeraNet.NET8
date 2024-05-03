using System;
using System.Collections.Generic;

namespace VeraNet.Objects.Scenes.Timers;

/// <summary>
/// Represents an absolute timer of a scene.
/// </summary>
public class AbsoluteTimer : Timer
{
    /// <summary>
    /// The time of the timer.
    /// </summary>
    public DateTime Time { get; init; }
    
    public override TimerType Type => TimerType.Absolute;
    
    internal override Dictionary<string, object> ToJson()
    {
        var dict = base.ToJson();
        dict.Add("time", GetTime());
        dict.Add("day", GetDay());
        return dict;
    }
    
    private string GetTime()
    {
        return Time.ToString("H:m:s");
    }

    private string GetDay()
    {
        return Time.ToString("d/M/yyyy");
    }
    
    public override string ToString()
    {
        return $"{Name} ({GetTime()})";
    }
}