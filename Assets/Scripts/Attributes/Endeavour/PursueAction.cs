using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PursueAction : Endeavour {

	private Label target;

	public PursueAction (RobotController controller, List<Goal> goals, Label target) : base(controller, goals, target.labelHandle) {
		this.target = target;
		this.name = "pursue";
		requiredComponents = new System.Type[] {typeof(HoverJet)};
	}

	public override bool canExecute () {
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
        RobotArms arms = controller.GetComponentInChildren<RobotArms>();
        return arms != null && !arms.hasTarget() && !target.hasTag(TagEnum.Grabbed) && controller.knowsTarget(target.labelHandle) && jet != null && jet.canReach(target);
	}

	public override void execute() {
        base.execute();
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		if (jet != null) {
			jet.pursueTarget(target.labelHandle);
			jet.setAvailability(false);
		}
	}

	public override void stopExecution() {
        base.stopExecution();
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		if (jet != null) {
			jet.setTarget(null);
			jet.setAvailability(true);
		}
	}

	public override bool isStale() {
		return !controller.knowsTarget (target.labelHandle);
	}

	public override void onMessage(RobotMessage message) {

	}

	protected override float getCost() {
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		if (jet != null) {
			return jet.calculatePathCost(target);
		}
		return 0;
	}
}
