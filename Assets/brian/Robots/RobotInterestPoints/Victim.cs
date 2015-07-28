using UnityEngine;
using System.Collections;

public class Victim : RobotInterest {

	void Awake() {
		this.name = "player";
		possibleActions.Add (new PursueAction (this));
	}
}
