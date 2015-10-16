using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RechargeAction : Endeavour {

    private Label powerStation;

    public RechargeAction(RobotController controller, List<Goal> goals, Label target) : base(controller, goals, target.gameObject) {
        powerStation = target;
        this.name = "recharge";
        requiredComponents = new System.Type[] { typeof(HoverJet) };
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

    protected override float getCost() {
        return 0f;
    }
}
