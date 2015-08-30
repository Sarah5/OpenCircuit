using UnityEngine;
using System.Collections;

public abstract class AbstractRobotComponent : MonoBehaviour {

	protected RobotController roboController;
	protected bool isOccupied = false;

	// Use this for initialization
	void Awake () {
		roboController = GetComponentInParent<RobotController> ();
	}

	public RobotController getController() {
		return roboController;
	}

	public bool isAvailable() {
		return isOccupied;
	}

	public void setAvailability(bool availability) {
		isOccupied = !availability;
	}
}
