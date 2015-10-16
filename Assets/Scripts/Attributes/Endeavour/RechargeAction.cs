using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RechargeAction : Endeavour {

    private Label powerStation;
	private Battery battery;

    public RechargeAction(RobotController controller, List<Goal> goals, Label target, Battery battery) : base(controller, goals, target.gameObject) {
        powerStation = target;
        this.name = "recharge";
        requiredComponents = new System.Type[] { typeof(HoverJet) };
		this.battery = battery;
    }

    public override void execute() {
        base.execute();
        HoverJet jet = controller.GetComponentInChildren<HoverJet>();
        if (jet != null) {
            jet.setTarget(powerStation);
            jet.setAvailability(false);
        }
    }

    public override bool isStale() {
        return powerStation == null;
    }

    public override void onMessage(RobotMessage message) {
      
    }

    public override void stopExecution() {
        base.stopExecution();
        HoverJet jet = controller.GetComponentInChildren<HoverJet>();
        if (jet != null) {
            jet.setTarget(null);
            jet.setAvailability(true);
        }
    }

	public override float getPriority() {
		float batteryPercent = ((battery.currentCapacity / battery.maximumCapacity)+.2f);
		Debug.Log(batteryPercent);
		return base.getPriority() / batteryPercent;
	}

    protected override float getCost() {
		HoverJet jet = controller.GetComponentInChildren<HoverJet>();
		if(jet != null) {
			jet.calculatePathCost(powerStation);
		}
        return 0f;
    }
}
