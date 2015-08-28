using UnityEngine;
using System.Collections;
using System.IO;

[System.Serializable]
public class EMPOp : Operation {
	
	private static System.Type[] triggers = new System.Type[] {
		typeof(InteractTrigger),
	};
	
	public override System.Type[] getTriggers() {
		return triggers;
	}
	
	public override void perform(GameObject instigator, Trigger trig) {
		//parent.GetComponent<RobotController>().enabled = false;
		//AbstractPowerSource [] powerSources = GameObject.FindObjectsOfType<AbstractPowerSource> ();
		foreach (AbstractPowerSource source in AbstractPowerSource.powerSources) {
			if (Vector3.Distance(parent.transform.position, source.transform.position) < 10f) {
//				Debug.Log ("perform");
				source.disable(5);
			}
		}
	}
}
