using UnityEngine;
using System.Collections.Generic;

public abstract class Event : MonoBehaviour {

    protected List<string> triggers = new List<string>();

	void Start () {
		EventHandler handler = GetComponent<EventHandler>();
		if (handler == null) {
			handler = gameObject.AddComponent<EventHandler>();
		}
		handler.addEvent(this, triggers);
	}

	public abstract void initiate(GameObject instigator, Trigger trig);
	
}
