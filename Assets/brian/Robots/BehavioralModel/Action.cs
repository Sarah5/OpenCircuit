using UnityEngine;
using System.Collections;

public abstract class Action : Prioritizable {

	protected string name;
	protected float priority;

	protected RobotController controller;

	public Action(RobotController controller) {
		this.controller = controller;
	}

	public abstract bool isStale();
	public abstract bool canExecute ();
	public abstract void execute ();
	public abstract void stopExecution();

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
