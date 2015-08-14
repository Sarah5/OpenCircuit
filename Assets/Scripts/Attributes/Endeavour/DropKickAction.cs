using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropKickAction : Endeavour {

	Label target;

	public DropKickAction(RobotController controller, Label target) : base(controller) {
		this.name = "dropKick";
		this.priority = 1;
		this.target = target;
		requiredComponents = new System.Type[] {typeof(HoverJet)};
	}

	public override bool canExecute (Dictionary<System.Type, int> availableComponents) {
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
