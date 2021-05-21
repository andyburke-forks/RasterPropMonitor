using System;

namespace JSI.Core
{
    public class Event : EventArgs
    {
        public string name { get; internal set; }
        public Guid vessel_id { get; internal set; }
        public object data { get; internal set; }
        public Event(string name, Guid vessel_id, object data)
        {
            this.name = name;
            this.vessel_id = vessel_id;
            this.data = data;
        }
    }


    public delegate void EventHandler(Event _event);

    public static class Events
    {
        public static event EventHandler onEvent;

        public static void Emit( Event _event )
        {
            if (onEvent != null)
            {
                onEvent( _event );
            }
        }
    }
}
