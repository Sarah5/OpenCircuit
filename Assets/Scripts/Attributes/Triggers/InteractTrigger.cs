using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class InteractTrigger : Trigger {
    new public static string triggerType = Trigger.triggerType + "." + new StackFrame().GetMethod().DeclaringType;

	private Vector3 myPoint;

    public override string getType() {
        return triggerType;
    }

	public Vector3 getPoint() {
		return myPoint;
	}

	public void setPoint( Vector3 p) {
		myPoint = p;
	}
}