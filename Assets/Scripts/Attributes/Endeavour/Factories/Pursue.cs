using UnityEngine;
using System.Collections;

[System.Serializable]
public class Pursue : EndeavourFactory {

	public override Endeavour constructEndeavour (RobotController controller) {
		if (parent == null) {
			return null;
		}
		return new PursueAction(controller, parent);
	}

	public override void doGUI() {
	}
}
