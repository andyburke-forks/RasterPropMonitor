using System;

namespace JSI.Core
{
    public class EventData {
        public string key { get; internal set; }
        public int propID { get; internal set; }
        public string buttonName { get; internal set; }
        public int pageNumber { get; internal set; }
        public int numericID { get; internal set; }
    }

    public class Event : EventArgs
    {
        public Guid id { get; internal set; }
        public string type { get; internal set; }
        public Guid vessel_id { get; internal set; }
        public EventData data { get; internal set; }
        public Event( Guid id, string type, Guid vessel_id, EventData data )
        {
            this.id = ( id == Guid.Empty ? Guid.NewGuid() : id );
            this.type = type;
            this.vessel_id = vessel_id;
            this.data = data;
        }
    }


    // data:
    // propID = thatProp.propID, // always present
    // buttonName = buttonName, // always present
    // thatPage_pageNumber = thatPage.pageNumber, // -1 or a page number
    // numericID = -1 // -1 or a numericID


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
