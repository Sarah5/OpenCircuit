using UnityEngine;
using System.Collections;

[System.Serializable]
public class Drop : EndeavourFactory {

	public override Endeavour constructEndeavour (RobotController controller) {
		if (parent == null) {
			return null;
		}
		return new DropKickAction(controller, parent);
	}
	
	public override void doGUI() {
	}
}
