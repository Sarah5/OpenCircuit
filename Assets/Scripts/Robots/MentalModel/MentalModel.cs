using UnityEngine;
using System.Collections.Generic;

public class MentalModel {

	Dictionary<Label, int> targetSightings = new Dictionary<Label, int> ();

	List<MentalModelUpdateListener> listeners = new List<MentalModelUpdateListener> ();

	public void addSighting(Label target) {
		if (targetSightings.ContainsKey (target)) {
			if (targetSightings[target] == 0) {
				targetSightings [target]++;

				notifyListenersTargetFound(target);
			}
			else {
				targetSightings [target]++;
			}
		} else {
			targetSightings[target] = 1;
			notifyListenersTargetFound(target);
		}
	}

	public void removeSighting(Label target) {
		if (targetSightings.ContainsKey (target)) {
			targetSightings [target]--;
			if (targetSightings [target] < 1) {
				notifyListenersTargetLost (target);
			}
		} else {
			targetSightings[target] = 0;
			notifyListenersTargetLost (target);

		}
	}

	public bool canSee(Label target) {
		return targetSightings.ContainsKey(target) && targetSightings[target] > 0;
	}

	public void notifyListenersTargetFound(Label target) {
		for (int i = 0; i < listeners.Count; i++) {
			listeners[i].notifySighting(target);
		}
	}

	public void notifyListenersTargetLost(Label target) {
		for (int i = 0; i < listeners.Count; i++) {
			listeners[i].notifySightingLost(target);
		}
	}

	public void addUpdateListener(MentalModelUpdateListener listener) {
		listeners.Add (listener);
	}
}