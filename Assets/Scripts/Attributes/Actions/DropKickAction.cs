using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropKickAction : Action {

	RobotInterest target;

	public DropKickAction(RobotController controller, RobotInterest target) : base(controller) {
		this.name = "dropKick";
		this.priority = 1;
		this.target = target;
		requiredComponents = new System.Type[] {typeof(HoverJet)};
	}

	public bool canExecute (Dictionary<System.Type, int> availableComponents) {
		return (target != null) && base.canExecute (availableComponents);
	}

	public override void execute (){
	}

	public override void stopExecution(){
	}

	public override bool isStale() {
		return true;
	}

	public override void onMessage(RobotMessage message) {
	}

}
