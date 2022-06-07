using System.Collections.Generic;

public class Events
{
    public Events(List<Event> events)
    {
        this.events = events;
    }

    public List<Event> events { get; }
}

public class Event
{
    public Event(string type, string data)
    {
        this.type = type;
        this.data = data;
    }

    public string data { get; }
    public string type { get; }
}
