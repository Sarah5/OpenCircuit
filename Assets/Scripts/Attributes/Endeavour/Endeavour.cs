using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/**
 * Something an AI can endeavour to accomplish, or more simply put, execute
 */
[System.Serializable]
public abstract class Endeavour : Prioritizable {

	public List<Goal> goals = new List<Goal>();
	
	protected string name;
	protected float priority;

	protected RobotController controller;

	protected System.Type[] requiredComponents;

	public Endeavour(RobotController controller, List<Goal> goals) {
		this.controller = controller;
		this.goals = goals;
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
		float finalPriority = 0;
		foreach (Goal goal in goals) {
			Dictionary<string, Goal> robotGoals = controller.getGoals ();
			if (robotGoals.ContainsKey(goal.name)) {
				finalPriority += goal.priority * robotGoals[goal.name].priority;
			}
		}
		float cost = getCost ();
		//if (cost > 0 || cost < 0) {
		//	Debug.Log("cost is: " + cost + " for " + name);
		//}
		return finalPriority - cost;
	}

	protected abstract float getCost ();
	
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
