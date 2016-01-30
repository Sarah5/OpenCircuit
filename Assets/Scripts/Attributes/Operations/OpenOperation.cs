using UnityEngine;

[System.Serializable]
public class OpenOperation : Operation {

	private static System.Type[] triggers = new System.Type[] {
		typeof(InteractTrigger),
	};

	public bool locked;

	public override System.Type[] getTriggers() {
		return triggers;
	}

	public override void perform(GameObject instigator, Trigger trig) {
        AutoDoor door = parent.GetComponent<AutoDoor>();
        //door.switchDoor();
    }

#if UNITY_EDITOR
    public override void doGUI() {
		locked = UnityEditor.EditorGUILayout.Toggle("Locked", locked);
	}
#endif
}
