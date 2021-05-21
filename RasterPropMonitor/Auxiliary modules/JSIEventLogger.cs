using System;
using UnityEngine;

namespace JSI
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class JSIEventLogger : MonoBehaviour
    {
        public void Start()
        {
            if (!HighLogic.LoadedSceneIsFlight)
            {
                return;
            }

            //JUtil.LogMessage(this, "JSI.Events.onEvent: logging" );
            Debug.Log("JSI.Events.onEvent: logging");

            Core.Events.onEvent += onEvent;
        }

        public void OnDestroy()
        {
            Core.Events.onEvent -= onEvent;
        }

        public static void onEvent( Core.Event _event )
        {
            //JUtil.LogMessage( null, "JSI.Events.onEvent: name: {0} vessel_id: {1}", _event.name, (_event.vessel_id != Guid.Empty ? _event.vessel_id.ToString() : "<null>" ));
            Debug.Log("JSI.Events.onEvent: name: " + _event.name + " vessel_id: " + (_event.vessel_id != Guid.Empty ? _event.vessel_id.ToString() : "<null>"));
        }
    }
}
