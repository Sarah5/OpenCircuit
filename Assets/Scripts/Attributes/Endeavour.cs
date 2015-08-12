using UnityEngine;
using System.Collections;

/**
 * Something AI can endeavour to accomplish, or more simply put, execute
 */
[System.Serializable]
public abstract class Endeavour: InspectorListElement {
	
	private static string[] typeNames = null;

	// list of Endeavour types that can be selected in the Label Inspector
	public static readonly System.Type[] types = new System.Type[] {
		typeof(PatrolEndeavour),
	};

	
	public abstract void execute();
	
	public virtual void doGUI() {}


	// editor interface methods
	InspectorListElement InspectorListElement.doListElementGUI() {
		int selectedType = System.Array.FindIndex(types, OP => OP == GetType());
		int newSelectedType = UnityEditor.EditorGUILayout.Popup(selectedType, getTypeNames());
		if (newSelectedType != selectedType) {
			return (Operation) Operation.types[newSelectedType].GetConstructor(new System.Type[0]).Invoke(new object[0]);
		}
		
		doGUI();
		return this;
	}

	public static Endeavour constructDefault() {
		return (Endeavour) types[0].GetConstructor(new System.Type[0]).Invoke(new object[0]);
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
