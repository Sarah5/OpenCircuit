using UnityEngine;
using System.Collections.Generic;

public class PatrolRoute : Label {

	private RoutePoint[] points;

	void Start() {
		Type = "patrolRoute";
		//possibleActions.Add (new PursueAction (this));
		points = GetComponentsInChildren<RoutePoint> ();
		for (int i = 0; i < points.Length; i++) {
			if (i < points.Length - 1) {
				points[i].Next = points[i+1];
			}
			else {
				points[i].Next = points[0];
			}
		}
	}

	public RoutePoint getNearest(Vector3 position) {
		float minDist;
		RoutePoint min = points[0];
		minDist = Vector3.Distance(position, points[0].transform.position);
		foreach (RoutePoint point in points) {
			float curDist = Vector3.Distance(position, point.transform.position);
			if (curDist < minDist) {
				minDist = curDist;
				min = point;
			}
		}
		return min;
	}

	public override List<Endeavour> getAvailableEndeavours (RobotController controller) {
		List<Endeavour> actions = base.getAvailableEndeavours (controller);
		actions.Add(new PatrolAction(controller, this));
		return actions;
	}

	protected override bool isVisible()  {
		return false;
	}
}
