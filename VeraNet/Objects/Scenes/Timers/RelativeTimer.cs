using System;
using System.Collections.Generic;

namespace VeraNet.Objects.Scenes.Timers;

/// <summary>
/// Represents a timer of a scene that is relative to Sunrise or Sunset.
/// </summary>
public abstract class RelativeTimer : Timer
{
    /// <summary>
    /// The time of the timer.
    /// </summary>
    public TimeSpan Time { get; init; }
    
    /// <summary>
    /// Represents if the timer is relative to Sunrise or Sunset.
    /// </summary>
    public bool IsRelative { get; init; }
    
    /// <summary>
    /// Represents if the timer is relative to Sunset. If false, it is relative to Sunrise.
    /// </summary>
    public bool IsSunset { get; init; }
    
    /// <summary>
    /// Represents if the timer is after the sunrise or sunset. If false, it is before.
    /// </summary>
    public bool IsAfter { get; init; }

    internal new virtual Dictionary<string, object> ToJson()
    {
        var dict = base.ToJson();
        dict.Add("time", GetTime());
        return dict;
    }
    
    private string GetTime()
    {
        var time = Time.ToString("H:m:s");
        
        // if absolute time, return normal time
        if (!IsRelative) return time;
        
        var sign = IsAfter ? "+" : "-";
        
        // if sunset, return time with "T" suffix
        // if sunrise, return time with "R" suffix
        var sunrise = IsSunset ? "T" : "R";
        return $"{sign}{time}{sunrise}";
    }
}
    