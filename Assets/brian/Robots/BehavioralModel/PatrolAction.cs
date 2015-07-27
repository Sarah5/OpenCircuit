using UnityEngine;
using System.Collections;

public class PatrolAction : Action {

	private PatrolRoute route;
	private RoutePoint currentDestination;

	public PatrolAction(RobotController controller, PatrolRoute route) : base(controller) {
		this.route = route;
		this.name = "patrol";
		this.priority = 1;
	}

	public override bool canExecute () {
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		return jet != null && !jet.isAvailable ();
	}

	public override void execute (){
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		if (jet != null) {
			currentDestination = route.getNearest(controller.transform.position);
			jet.setTarget(currentDestination);
			jet.setAvailability(false);
		}
	}

	public override void stopExecution(){
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		if (jet != null) {
			jet.setAvailability(true);
			jet.setTarget(null);
		}
	}

	public override void onMessage(RobotMessage message) {
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		if (jet != null) {
			currentDestination = currentDestination.Next;
			jet.setTarget(currentDestination);
		}
	}


}
