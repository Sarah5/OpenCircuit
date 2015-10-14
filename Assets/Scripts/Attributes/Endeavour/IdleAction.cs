using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class IdleAction : Endeavour {

    public IdleAction(RobotController controller, List<Goal> goals, Label target) : base(controller, goals, target.gameObject) {

    }

    public override void execute() {

     }

    public override void stopExecution() {

    }

    public override void onMessage(RobotMessage message) {
       
    }

    public override bool isStale() {
      return false;
    }

    protected override float getCost() {
        return 0f;
    }
}
