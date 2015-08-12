using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Victim : RobotInterest {

	void Awake() {
		this.name = "player";
	}

	public override List<Action> getAvailableActions (RobotController controller) {
		List<Action> actions = base.getAvailableActions(controller);
		actions.Add(new PursueAction(controller, this));
		return actions;
	}
}
