using UnityEngine;
using System.Collections;

public class HoldAction : Action {

	RobotInterest target;

	public HoldAction(RobotController controller, RobotInterest target) : base(controller) {
		this.target = target;
		this.name = "grab";
		this.priority = 10;
	}

	public override bool canExecute () {
		if (target == null) {
			return false;
		}
		RobotArms arms = controller.GetComponentInChildren<RobotArms> ();
		return arms != null && !arms.isAvailable ();
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

	public void setTarget(RobotInterest target) {
		this.target = target;
	}

	public override void onMessage(RobotMessage message) {
	}
}
