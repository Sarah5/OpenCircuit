using UnityEngine;
using System.Collections;

[System.Serializable]
public class Drop : EndeavourFactory {

	public const float NUM_STRIPES = 8f;
	public const float LENGTH = 1f;


	public static Color COLOR_ONE = Color.black;
	public static Color COLOR_TWO = Color.yellow;

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
		//Gizmos.DrawRay(new Ray(parent.transform.position + (parent.transform.forward * .1f), parent.transform.forward));
		for (int i = 0; i < NUM_STRIPES; i ++) {
			Gizmos.color = i % 2 == 0 ? COLOR_ONE : COLOR_TWO;
			Vector3 startPos = parent.transform.position + (parent.transform.forward * .1f) + ((i * (LENGTH/NUM_STRIPES))*parent.transform.forward);
			Vector3 endPos = startPos + (((LENGTH/NUM_STRIPES))*parent.transform.forward);
			Gizmos.DrawLine(startPos, endPos);
		}
	}
}
