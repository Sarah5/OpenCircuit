using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class Recharge : EndeavourFactory {
    public override Endeavour constructEndeavour(RobotController controller) {
		Battery battery = controller.GetComponentInChildren<Battery>();
		if (parent == null || battery == null) {
            return null;
        }
		
        return new RechargeAction(controller, goals, parent, battery);
    }
}
