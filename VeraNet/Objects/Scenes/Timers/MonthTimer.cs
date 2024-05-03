using System.Collections.Generic;

namespace VeraNet.Objects.Scenes.Timers;

/// <summary>
/// Represents a timer of a scene that is relative to specific days of the month.
/// </summary>
public class MonthTimer : RelativeTimer
{
    /// <summary>
    /// The days of the month when the timer should be triggered.
    /// </summary>
    public List<int> Days { get; init; }
    
    public override TimerType Type => TimerType.DayOfWeek;
    
    private string GetDays()
    {
        return string.Join(",", Days);
    }
    
    internal override Dictionary<string, object> ToJson()
    {
        var dict = base.ToJson();
        dict.Add("days_of_month", GetDays());
        return dict;
    }
    
    public override string ToString()
    {
        return $"{Name} ({GetDays()})";
    }
}