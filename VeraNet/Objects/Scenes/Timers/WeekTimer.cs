using System;
using System.Collections.Generic;

namespace VeraNet.Objects.Scenes.Timers;

/// <summary>
/// Represents a timer of a scene that is relative to specific days of the week.
/// </summary>
public class WeekTimer : Timer
{
    /// <summary>
    /// The days of the week when the timer should be triggered.
    /// </summary>
    public List<DayOfWeek> Days { get; init; }
    
    public override TimerType Type => TimerType.DayOfWeek;

    internal override Dictionary<string, object> ToJson()
    {
        var dict = base.ToJson();
        dict.Add("days_of_week", GetDays());
        return dict;
    }
    
    private string GetDays()
    {
        return string.Join(",", Days);
    }
    
    public override string ToString()
    {
        return $"{Name} ({GetDays()})";
    }
    
}