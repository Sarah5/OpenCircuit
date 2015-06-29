using UnityEngine;
using System.Collections;

public class AbstractRobotComponent : MonoBehaviour {

	protected RobotController roboController;

	// Use this for initialization
	void Awake () {
		roboController = GetComponent<RobotController> ();

	
	}
}
