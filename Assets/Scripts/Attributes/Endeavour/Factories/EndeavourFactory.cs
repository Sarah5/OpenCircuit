using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
public abstract class EndeavourFactory : InspectorListElement {

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
		//factory.initialize();
		//factory.setParent (parent);
		return factory;
	}

	public abstract void doGUI ();


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