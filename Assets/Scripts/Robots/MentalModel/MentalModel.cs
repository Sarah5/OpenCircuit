using UnityEngine;
using System.Collections.Generic;

public class MentalModel {

	Dictionary<Label, SensoryInfo> targetSightings = new Dictionary<Label, SensoryInfo>();

	List<MentalModelUpdateListener> listeners = new List<MentalModelUpdateListener> ();

	public void addSighting(Label target, Vector3 position) {
		if (targetSightings.ContainsKey (target)) {
			SensoryInfo info = targetSightings[target];
			
			if (info.getSightings() == 0) {
				// We have to increment the sighting count before we notify listeners
				info.addSighting();
				notifyListenersTargetFound(target);
			}
			else {
				// Keep this. See above comment
				info.addSighting();
			}
			info.updatePosition(position);

		} else {
			targetSightings[target] = new SensoryInfo(position, 1);
			notifyListenersTargetFound(target);
		}
	}

	public void removeSighting(Label target, Vector3 position) {
		if (targetSightings.ContainsKey (target)) {
			SensoryInfo info = targetSightings[target];

			info.removeSighting();
			if (info.getSightings() < 1) {
				notifyListenersTargetLost (target);
			}
			info.updatePosition(position);
		} else {
			//Realistically we should never get here. This case is stupid.
			targetSightings[target] = new SensoryInfo(position, 0);
			notifyListenersTargetLost (target);
			Debug.LogWarning("Target '" + target.name + "' that was never found has been lost. Shenanigans?");
		}
	}

	public bool canSee(Label target) {
		return targetSightings.ContainsKey(target) && targetSightings[target].getSightings() > 0;
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