using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class InteractTrigger : Trigger {

	private Vector3 myPoint;

	public Vector3 getPoint() {
		return myPoint;
	}

	public void setPoint( Vector3 p) {
		myPoint = p;
	}
}