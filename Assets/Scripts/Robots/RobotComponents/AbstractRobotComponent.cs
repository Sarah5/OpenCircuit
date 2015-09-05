using UnityEngine;
using System.Collections;

public abstract class AbstractRobotComponent : MonoBehaviour {

	protected AbstractPowerSource powerSource;
	protected RobotController roboController;
	protected bool isOccupied = false;

	// Use this for initialization
	void Awake () {
		roboController = GetComponentInParent<RobotController> ();
		powerSource = roboController.GetComponentInChildren<AbstractPowerSource> ();
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
