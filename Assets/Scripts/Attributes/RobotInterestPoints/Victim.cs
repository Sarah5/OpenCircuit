using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class Victim : Label {

	void Start() {
	}

	public override List<Endeavour> getAvailableEndeavours (RobotController controller) {
		List<Endeavour> actions = base.getAvailableEndeavours(controller);
		actions.Add(new PursueAction(controller, this));
		return actions;
	}
}
