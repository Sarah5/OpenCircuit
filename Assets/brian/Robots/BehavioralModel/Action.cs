using UnityEngine;
using System.Collections;

public abstract class Action {

	protected string name;
	protected float priority;

	public abstract bool canExecute (RobotController controller);
	public abstract void execute (RobotController controller);
	public abstract void stopExecution(RobotController controller);


}
