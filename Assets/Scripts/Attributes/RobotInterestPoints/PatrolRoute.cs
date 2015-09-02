using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Scripts/Labels/Patrol Route")]
public class PatrolRoute : Label {

	public new EndeavourFactory[] endeavours = new EndeavourFactory[1] {new Patrol()};

	void Start() {
		Type = "patrolRoute";
		//possibleActions.Add (new PursueAction (this));
		//points = GetComponentsInChildren<RoutePoint> ();
		/*for (int i = 0; i < points.Length; i++) {
			if (i < points.Length - 1) {
				points[i].Next = points[i+1];
			}
			else {
				points[i].Next = points[0];
			}
		}*/
	}

	//public override Endeavour [] getAvailableEndeavours (RobotController controller) {
		//List<Endeavour> actions = base.getAvailableEndeavours (controller);
		//actions.Add(new PatrolAction(controller, this));
		//return endeavours;
	//}
}
