using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HoldAction : Endeavour {

	Label target;

	public HoldAction(RobotController controller, Label target) : base(controller) {
		this.target = target;
		this.name = "grab";
		this.priority = 10;
		requiredComponents = new System.Type[] {typeof(RobotArms)};
	}

	public override bool canExecute (Dictionary<System.Type, int> availableComponents) {
		return target != null && base.canExecute (availableComponents);
	}

	public override void execute (){
		RobotArms arms = controller.GetComponentInChildren<RobotArms> ();
		if (arms != null) {
			arms.attachTarget(target);
			arms.setAvailability(false);
		}
	}

	public override void stopExecution(){
		RobotArms arms = controller.GetComponentInChildren<RobotArms> ();
		if (arms != null) {
			arms.setAvailability(true);
			arms.dropTarget();
		}
	}

	public override bool isStale() {
		return target == null;
	}

	public void setTarget(Label target) {
		this.target = target;
	}

	public override void onMessage(RobotMessage message) {
	}
}
