using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class Recharge : EndeavourFactory {
    public override Endeavour constructEndeavour(RobotController controller) {
        if (parent == null) {
            return null;
        }
        return new RechargeAction(controller, goals, parent);
    }
}
