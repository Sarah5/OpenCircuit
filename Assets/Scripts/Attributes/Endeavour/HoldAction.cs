using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HoldAction : Endeavour {

	Label target;

	bool hasComplained = false;

	public HoldAction(RobotController controller, Label target, GameObject source) : base(controller, new List<Goal>{new Goal(GoalEnum.Offense, 3), new Goal(GoalEnum.Protection, 3)}, source ) {
		this.target = target;
		this.name = "grab";
		requiredComponents = new System.Type[] {typeof(RobotArms)};
	}

	public override bool canExecute (Dictionary<System.Type, int> availableComponents) {
		return target != null && base.canExecute (availableComponents);
	}

	public override void execute (){
        base.execute();
		RobotArms arms = controller.GetComponentInChildren<RobotArms> ();
		if (arms != null) {
			arms.attachTarget(target);
			arms.setAvailability(false);
		}
	}

	public override void stopExecution(){
        base.stopExecution();
		RobotArms arms = controller.GetComponentInChildren<RobotArms> ();
		if (arms != null) {
			arms.setAvailability(true);
			arms.dropTarget();
		}
	}

	public override bool isStale() {
        RobotArms arms = controller.GetComponentInChildren<RobotArms>();

        return target == null || arms == null || (arms.getProposedTarget() != target && !arms.hasTarget());
	}

	public void setTarget(Label target) {
		this.target = target;
	}

	public override void onMessage(RobotMessage message) {
	}

	protected override float getCost() {
		if(!hasComplained) { 
			Debug.LogWarning ("Please remind CryHavoc to implement the cost function for the HoldAction!!!");
			hasComplained = true;
		}
		return 0;
	}
}
