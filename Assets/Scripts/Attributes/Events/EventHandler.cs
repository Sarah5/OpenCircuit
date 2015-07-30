using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EventHandler : MonoBehaviour {

    private Hashtable events = new Hashtable();

    private EventHandler parent;

//    private static Hashtable poiEvents = new Hashtable();
    public static List<EventHandler> eventHandlers = new List<EventHandler>();

    public void Awake() {
        EventHandler.eventHandlers.Add(this);
    }

	public void sendTrigger(GameObject instigator, Trigger trig) {
        string type = trig.getType();
		if (events.ContainsKey(type)) {
            foreach (Event e in (List<Event>)events[type]) {
                e.initiate(instigator, trig);
            }
		}
	}

	public void addEvent(Event e, List<string> triggers) {
        if (e != null) {
            foreach (string element in triggers) {
                if (!events.ContainsKey(element)) {

                    events[element] = new List<Event>();
                }
//                if (e is POIEvent) {
//                    if (!poiEvents.ContainsKey(element)) {
//                        poiEvents[element] = new List<POIEvent>();
//                        
//                    }
//                    ((List<POIEvent>)poiEvents[element]).Add((POIEvent)e);
//                }
                ((List<Event>)events[element]).Add(e);
            }
		}
	}

//    public List<POIEvent> getPoiEvents(string type) {
//        return (List<POIEvent>)poiEvents[type];
//    }

	public bool hasEventType(string type) {
        return events.ContainsKey(type);
	}
}