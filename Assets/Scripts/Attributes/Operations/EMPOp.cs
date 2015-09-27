using UnityEngine;
using System.Collections;
using System.IO;

[System.Serializable]
public class EMPOp : Operation {

	public float range = 10f;
	public float time = 5f;
	
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
			if (Vector3.Distance(parent.transform.position, source.transform.position) < range) {
				source.disable(time);
			}
		}
	}

#if UNITY_EDITOR
    public override void doGUI () {
		range = UnityEditor.EditorGUILayout.FloatField ("Range", range);
		time = UnityEditor.EditorGUILayout.FloatField ("Duration (s)", time);
	}
#endif
}
