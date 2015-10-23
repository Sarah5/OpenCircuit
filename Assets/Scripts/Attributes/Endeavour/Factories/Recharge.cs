using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

[System.Serializable]
public class Recharge : EndeavourFactory {

	public float rechargePoint = 1f;

    public override Endeavour constructEndeavour(RobotController controller) {
		Battery battery = controller.GetComponentInChildren<Battery>();
		if (parent == null || battery == null) {
            return null;
        }
		RechargeAction action = new RechargeAction(controller, goals, parent, battery);
		action.rechargePoint = rechargePoint;
        return action;
    }

	public override void doGUI() {
		rechargePoint = EditorGUILayout.FloatField("Recharge Point", rechargePoint);
		base.doGUI();

	}
}
