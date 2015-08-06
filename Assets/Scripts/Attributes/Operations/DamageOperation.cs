using UnityEngine;
using System.Collections;
using System.IO;

[System.Serializable]
public class DamageOperation : Operation {
	
	private static System.Type[] triggers = new System.Type[] {
		typeof(InteractTrigger),
	};

	public string damageType;
	public float damageAmount;
	
	public override System.Type[] getTriggers() {
		return triggers;
	}
	
	public override void perform(GameObject instigator, Trigger trig) {
		
	}
	
	public override void doGUI() {
		damageType = UnityEditor.EditorGUILayout.TextField("Type", damageType);
		damageAmount = UnityEditor.EditorGUILayout.FloatField("Amount", damageAmount); 
	}
}
