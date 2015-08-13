using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Action : Prioritizable {

	protected string name;
	protected float priority;

	protected RobotController controller;

	protected System.Type[] requiredComponents;

	public Action(RobotController controller) {
		this.controller = controller;
	}

	public abstract bool isStale();
	public abstract void execute ();
	public abstract void stopExecution();

	public bool canExecute (Dictionary<System.Type, int> availableComponents) {
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

	public bool Equals(Action action) {
		return controller == action.controller && name.Equals(action.name);
	}

	public abstract void onMessage(RobotMessage message);
}
