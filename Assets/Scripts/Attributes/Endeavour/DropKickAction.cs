using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropKickAction : Endeavour {

	Label dropPoint;

	public DropKickAction(RobotController controller, List<Goal> goals, Label dropPoint) : base(controller, goals, dropPoint.getGameObject()) {
		this.name = "dropKick";
		this.dropPoint = dropPoint;
		requiredComponents = new System.Type[] {typeof(HoverJet)};
	}

	public override bool canExecute () {
		RobotArms arms = controller.GetComponentInChildren<RobotArms> ();
		return (arms != null) && (dropPoint != null) && (arms.hasTarget());
	}

	public override void execute (){
        base.execute();
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		if (jet != null) {
			jet.setTarget(dropPoint, true);
			jet.setAvailability(false);
		}
	}

	public override void stopExecution(){
        base.stopExecution();
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

	protected override float getCost() {
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		if (jet != null) {
			return jet.calculatePathCost(dropPoint);
		}
		return 0;
	}
}
