using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Doors/Door Trigger")]
public class DoorTrigger : MonoBehaviour {

	private AutoDoor control = null;
	private int robotCount = 0;

	void Awake() {
		control = GetComponentInParent<AutoDoor> ();
	}

	void OnTriggerEnter(Collider collision) {
		RobotController controller = collision.GetComponent<RobotController> ();
		if (controller != null) {
			robotCount++;
			control.open();
		}
	}

	void OnTriggerExit(Collider other) {
		RobotController controller = other.GetComponent<RobotController> ();
		if(controller != null) {
			--robotCount;
			if(robotCount == 0) {
				control.close();
			}
		}
	}
}
