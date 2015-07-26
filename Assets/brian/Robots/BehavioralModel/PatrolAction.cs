using UnityEngine;
using System.Collections;

public class PatrolAction : Action {

	private PatrolRoute route;

	public PatrolAction(PatrolRoute route) {
		this.route = route;
		this.name = "patrol";
		this.priority = 1;
	}

	public override bool canExecute (RobotController controller) {
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		return jet != null && !jet.isAvailable ();
	}

	public override void execute (RobotController controller){
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		if (jet != null) {
			jet.setTarget(route.getNearest(controller.transform.position));
			jet.setAvailability(false);
		}
	}

	public override void stopExecution(RobotController controller){
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		if (jet != null) {
			jet.setAvailability(true);
			jet.setTarget(null);
		}
	}


}
