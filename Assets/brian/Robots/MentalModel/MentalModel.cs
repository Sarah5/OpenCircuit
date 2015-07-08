using UnityEngine;
using System.Collections.Generic;

public class MentalModel {

	Dictionary<RobotInterest, int> targetSightings = new Dictionary<RobotInterest, int> ();

	List<MentalModelUpdateListener> listeners = new List<MentalModelUpdateListener> ();

	public void addSighting(RobotInterest target) {
		if (targetSightings.ContainsKey (target)) {
			if (targetSightings[target] == 0) {
				notifyListenersTargetFound(target);
			}
			targetSightings [target]++;
		} else {
			notifyListenersTargetFound(target);
			targetSightings[target] = 1;
		}
	}

	public void removeSighting(RobotInterest target) {
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

	public bool canSee(RobotInterest target) {
		return targetSightings.ContainsKey(target) && targetSightings[target] > 0;
	}

	public void notifyListenersTargetFound(RobotInterest target) {
		Debug.Log ("Notify target found: " + target.Type);
		for (int i = 0; i < listeners.Count; i++) {
			listeners[i].notifySighting(target);
		}
	}

	public void notifyListenersTargetLost(RobotInterest target) {
		for (int i = 0; i < listeners.Count; i++) {
			listeners[i].notifySightingLost(target);
		}
	}

	public void addUpdateListener(MentalModelUpdateListener listener) {
		listeners.Add (listener);
	}
}