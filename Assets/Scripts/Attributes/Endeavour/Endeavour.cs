using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/**
 * Something AI can endeavour to accomplish, or more simply put, execute
 */
[System.Serializable]
public abstract class Endeavour : InspectorListElement, Prioritizable {
	
	private static string[] typeNames = null;

	// list of Endeavour types that can be selected in the Label Inspector
	public static readonly System.Type[] types = new System.Type[] {
		typeof(PatrolAction),
	};

	// Members required for endeavor execution
	protected string name;
	protected float priority;

	protected RobotController controller;

	protected System.Type[] requiredComponents;

	public Endeavour(RobotController controller) {
		this.controller = controller;
	}

	public abstract bool isStale();
	public abstract void execute ();
	public abstract void stopExecution();
	public abstract void onMessage(RobotMessage message);

	public virtual bool canExecute (Dictionary<System.Type, int> availableComponents) {
		foreach (System.Type type in requiredComponents) {
			if (availableComponents.ContainsKey(type)) {
				int numAvailable = availableComponents[type];
				if (numAvailable > 0) {
					--numAvailable;
					availableComponents[type] = numAvailable;
				}
				else {
					return false;
				}
			}
		}
		return true;
	}

	public float getPriority() {
		return priority;
	}
	
	public string getName() {
		return name;
	}
	
	public RobotController getController() {
		return controller;
	}
	
	public bool Equals(Endeavour endeavour) {
		return controller == endeavour.controller && name.Equals(endeavour.name);
	}

	// Members required for display
	public virtual void doGUI() {
		priority = UnityEditor.EditorGUILayout.FloatField("Priority", priority);
	}


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
