using UnityEngine;
using System.Collections;

public class PursueAction : Action {

	private RobotInterest target;

	public PursueAction(RobotInterest target) {
		this.target = target;
		this.name = "pursue";
		this.priority = 5;
	}

	public override bool canExecute(RobotController controller) {
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		return jet != null && !jet.isAvailable ();
	}
	
	public override void execute(RobotController controller) {
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		if (jet != null) {
			jet.setTarget(target);
			jet.setAvailability(false);
		}
	}

	public override void stopExecution(RobotController controller) {
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		if (jet != null) {
			jet.setTarget(null);
			jet.setAvailability(false);
		}
	}
}
