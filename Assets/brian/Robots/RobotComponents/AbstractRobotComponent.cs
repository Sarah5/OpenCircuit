using UnityEngine;
using System.Collections;

public abstract class AbstractRobotComponent : MonoBehaviour {

	protected RobotController roboController;

	// Use this for initialization
	void Awake () {
		roboController = GetComponentInParent<RobotController> ();
	}

	public RobotController getController() {
		return roboController;
	}
}
