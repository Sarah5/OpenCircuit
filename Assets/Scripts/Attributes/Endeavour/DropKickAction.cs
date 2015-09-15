using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropKickAction : Endeavour {

	Label dropPoint;

	public DropKickAction(RobotController controller, List<Goal> goals, Label dropPoint) : base(controller, goals) {
		this.name = "dropKick";
		this.priority = 10;
		this.dropPoint = dropPoint;
		requiredComponents = new System.Type[] {typeof(HoverJet)};
	}

	public override bool canExecute (Dictionary<System.Type, int> availableComponents) {
		RobotArms arms = controller.GetComponentInChildren<RobotArms> ();
		return (arms != null) && (dropPoint != null) && (arms.hasTarget()) && base.canExecute (availableComponents);
	}

	public override void execute (){
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		if (jet != null) {
			jet.setTarget(dropPoint, true);
			jet.setAvailability(false);
		}
	}

	public override void stopExecution(){
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		if (jet != null) {
			jet.setTarget(null);
			jet.setAvailability(true);
		}
	}

	public override bool isStale() {
		return dropPoint == null;
	}

	public override void onMessage(RobotMessage message) {
		if (message.Message.Equals ("target reached")) {
			RobotArms arms = controller.GetComponentInChildren<RobotArms> ();
			arms.dropTarget();
		}
	}

}
