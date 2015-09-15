using UnityEngine;
using System.Collections;

[System.Serializable]
public class Drop : EndeavourFactory {

	public override Endeavour constructEndeavour (RobotController controller) {
		if (parent == null) {
			return null;
		}
		//Goal[] goals = new Goal[2];
		//goals[0] = new Goal ("protection", 5);
		//goals[1] = new Goal ("offense", 5);
		//Debug.Log ("get drop");
		return new DropKickAction(controller, goals, parent);
	}

	public override void drawGizmo() {
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(parent.transform.position, .1f);
		Gizmos.color = Color.gray;
		Gizmos.DrawRay(new Ray(parent.transform.position + (parent.transform.forward * .1f), parent.transform.forward));
	}
}
