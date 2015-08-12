using UnityEngine;
using System.Collections;

public class DropKickAction : Action {

	RobotInterest target;

	public DropKickAction(RobotController controller, RobotInterest target) : base(controller) {
		this.name = "dropKick";
		this.priority = 1;
		this.target = target;
	}

	public override bool canExecute () {
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		return (target != null) && jet != null && !jet.isAvailable ();
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
