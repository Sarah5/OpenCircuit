using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PursueAction : Endeavour {

	private Label target;

	public PursueAction (RobotController controller, List<Goal> goals, Label target) : base(controller, goals) {
		this.target = target;
		this.name = "pursue";
		requiredComponents = new System.Type[] {typeof(HoverJet)};
	}

	/*public override bool canExecute() {
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		return jet != null && !jet.isAvailable () && controller.knowsTarget(target);
	}*/

	public override bool canExecute (Dictionary<System.Type, int> availableComponents) {
		//Debug.Log ("pursue action knows target: " + controller.knowsTarget (target));
		return controller.knowsTarget (target) && base.canExecute (availableComponents);//jet != null && !jet.isAvailable () && controller.knowsTarget(target);
	}

	public override void execute() {
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		if (jet != null) {
			jet.setTarget(target);
			jet.setAvailability(false);
		}
	}

	public override void stopExecution() {
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		if (jet != null) {
			jet.setTarget(null);
			jet.setAvailability(true);
		}
	}

	public override bool isStale() {
		return !controller.knowsTarget (target);
	}

	public override void onMessage(RobotMessage message) {
	
	}
}
