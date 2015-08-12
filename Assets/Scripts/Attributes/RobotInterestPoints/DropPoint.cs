using UnityEngine;
using System.Collections;

public class DropPoint : RobotInterest {

	void Awake() {
		Type = "dropPoint";
	}
	
	protected override bool isVisible()  {
		return false;
	}
}
