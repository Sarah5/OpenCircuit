using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class EndeavourFactory : InspectorListElement {

	public List<Goal> goals = new List<Goal> ();
	private bool status = false;
	private int size = 0;

	[System.NonSerialized]
	protected Label parent;

	private static string[] typeNames = null;

	public static readonly System.Type[] types = new System.Type[] {
		typeof(Patrol),
		typeof(Pursue),
		typeof(Drop)
	};

	public void setParent(Label parent) {
		this.parent = parent;
	}

	public abstract Endeavour constructEndeavour (RobotController controller);

	InspectorListElement InspectorListElement.doListElementGUI() {
		int selectedType = System.Array.FindIndex(types, OP => OP == GetType());
		int newSelectedType = EditorGUILayout.Popup(selectedType, getTypeNames());
		if (newSelectedType != selectedType) {
			return (EndeavourFactory) EndeavourFactory.types[newSelectedType].GetConstructor(new System.Type[0]).Invoke(new object[0]);
		}
		
		doGUI();
		return this;
	}

	public static EndeavourFactory constructDefault(Label parent) {
		EndeavourFactory factory = (EndeavourFactory) types[0].GetConstructor(new System.Type[0]).Invoke(new object[0]);
		factory.goals = new List<Goal> ();
		//factory.initialize();
		//factory.setParent (parent);
		return factory;
	}

	public virtual void doGUI () {
		status = UnityEditor.EditorGUILayout.Foldout (status, "Goals");
		
		if (status && goals != null) {
			//UnityEditor.EditorGUIUtility.LookLikeControls();
			size = UnityEditor.EditorGUILayout.IntField ("Size:", goals.Count);
			EditorGUILayout.Separator();

			foreach (Goal goal in goals) {
				goal.name = EditorGUILayout.TextField("Name", goal.name);
				goal.priority = EditorGUILayout.FloatField("Priority", goal.priority);
				EditorGUILayout.Separator();
			}
			if (size < goals.Count) {
				goals.RemoveRange (size, goals.Count - size);
				//pointsPaths.RemoveRange(size, getPoints().Count - size);
			}
			while (size > goals.Count) {
				goals.Add(new Goal("", 0));
				//pointsPaths.Add(null);
			}
		}
	}

	public virtual void drawGizmo() {

	}
	
	private static string[] getTypeNames() {
		if (typeNames == null || typeNames.Length != types.Length) {
			typeNames = new string[types.Length];
			for(int i=0; i<typeNames.Length; ++i) {
				typeNames[i] = types[i].FullName;
			}
		}
		return typeNames;
	}
}