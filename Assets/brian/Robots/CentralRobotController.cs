using UnityEngine;
using System.Collections.Generic;

public class CentralRobotController : MonoBehaviour, MentalModelUpdateListener {

	public GameObject[] robots;
	
	private List<RobotController> listeners = new List<RobotController>();

	MentalModel mentalModel = new MentalModel();

	// Use this for initialization
	void Start () {
		mentalModel.addUpdateListener (this);
		for (int i = 0; i < robots.Length; i++) {
			RobotAntenna antenna = robots[i].GetComponentInChildren<RobotAntenna>();
			if (antenna != null) {
				listeners.Add(antenna.getController());
				antenna.getController().attachMentalModel(mentalModel);
			}
		}
	}

	public void notifySighting(RobotInterest target) {
		broadcastMessage (new EventMessage ("target found", target));
	}

	public void notifySightingLost(RobotInterest target) {
		broadcastMessage (new EventMessage ("target lost", target));
	}

	private void broadcastMessage(EventMessage message) {
		for (int i = 0; i < listeners.Count; i++) {
			listeners[i].notify(message);
		}
	}
}