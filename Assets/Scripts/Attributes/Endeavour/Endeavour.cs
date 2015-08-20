using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/**
 * Something AI can endeavour to accomplish, or more simply put, execute
 */
[System.Serializable]
public abstract class Endeavour : Prioritizable {
	
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
}
