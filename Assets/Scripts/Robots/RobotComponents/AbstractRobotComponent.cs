using UnityEngine;
using System.Collections;

public abstract class AbstractRobotComponent : MonoBehaviour {

	protected AbstractPowerSource powerSource;
	protected RobotController roboController;
	protected bool isOccupied = false;
    public float powerDrawRate = 5f;

	// Use this for initialization
	void Awake () {
		roboController = GetComponentInParent<RobotController> ();
        if (roboController == null) {
            Debug.LogWarning("Robot component '" + name + "' is not attached to a robot controller!");
        } else {
            powerSource = roboController.GetComponentInChildren<AbstractPowerSource>();

        }
        if (roboController == null) {
            Debug.LogWarning("Robot component '" + name + "' has no power source!");
        }

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
