using UnityEngine;
using System.Collections;
using System.IO;

[System.Serializable]
public class DisabelOp : Operation {
	
	private static System.Type[] triggers = new System.Type[] {
		typeof(InteractTrigger),
	};
	
	public override System.Type[] getTriggers() {
		return triggers;
	}
	
	public override void perform(GameObject instigator, Trigger trig) {
		parent.GetComponent<RobotController>().enabled = false;
	}
}
