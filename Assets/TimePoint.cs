using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

struct TimePoint
{
    private readonly ConciousnessState _state;
    public ConciousnessState State { get { return _state; } }

    private readonly DateTime _time;
    public DateTime Time { get { return _time; } }

    public TimePoint(ConciousnessState state, DateTime time)
    {
        _state = state;
        _time = time;
    }
}

class TimePointBuilder
{
    public ConciousnessState State { get; set; }
    
    public DateTime Time { get; set; }

    public bool Am { get; set; }

    public TimePointBuilder(ConciousnessState state, DateTime time, bool am)
    {
        State = state;
        Time = time;
        Am = am;
    }

    internal TimePoint ToTimepoint()
    {
        return new TimePoint(State, Time);
    }
}