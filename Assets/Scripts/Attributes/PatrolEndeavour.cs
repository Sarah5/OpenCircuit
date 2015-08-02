using UnityEngine;
using System.Collections;

[System.Serializable]
public class PatrolEndeavour : Endeavour {

	public float priority;
	
	public override void execute() {
		
	}
	
	public override void doGUI() {
		priority = UnityEditor.EditorGUILayout.FloatField("Priority", priority);
	}
}
