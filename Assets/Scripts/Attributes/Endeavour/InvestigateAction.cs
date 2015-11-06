using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InvestigateAction : InherentEndeavour {

	private static int expirationTimeSeconds = 20; //Expires in 60 seconds

	private System.DateTime creationTime;
	private bool completed = false;

	public InvestigateAction(RobotController controller, List<Goal> goals, LabelHandle parent)
		: base(controller, goals, parent) {
		creationTime = System.DateTime.Now;
		this.name = "investigate";
		requiredComponents = new System.Type[] { typeof(HoverJet) };
	}



	public override bool isStale() {
		return completed || !((System.DateTime.Now - creationTime).Seconds < InvestigateAction.expirationTimeSeconds);
	}

	public override void onMessage(RobotMessage message) {
		if(message.Type == RobotMessage.MessageType.ACTION) {
			if(message.Target == parent) {
				Debug.Log("investigate action complete");
				completed = true;
			}
		}
	}

	public override bool canExecute() {
		HoverJet jet = controller.GetComponentInChildren<HoverJet>();
		System.Nullable<Vector3> pos = controller.getLastKnownPosition(parent);
		return jet != null && pos != null && jet.canReach(pos.Value);
	}

	public override void execute() {
		base.execute();
		HoverJet jet = controller.GetComponentInChildren<HoverJet>();
		if(jet != null) {
			jet.setTarget(parent);
			jet.setAvailability(false);
		}
	}

	public override void stopExecution() {
		base.stopExecution();
		HoverJet jet = controller.GetComponentInChildren<HoverJet>();
		if(jet != null) {
			jet.setTarget(null);
			jet.setAvailability(true);
		}
	}

	protected override float getCost() {
		HoverJet jet = controller.GetComponentInChildren<HoverJet>();
		if(jet != null) {
			System.Nullable<Vector3> pos = controller.getLastKnownPosition(parent);
			if(pos.HasValue) {
				float cost = jet.calculatePathCost(pos.Value);
				//Debug.Log("investigate path cost: " + cost);
				return cost;
			}
		}
		return 0;
	}
}
