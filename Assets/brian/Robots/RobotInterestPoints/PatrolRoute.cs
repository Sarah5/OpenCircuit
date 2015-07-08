using UnityEngine;
using System.Collections.Generic;

public class PatrolRoute : RobotInterest {

	private RoutePoint[] points;

	void Awake() {
		Type = "patrolRoute";
		points = GetComponentsInChildren<RoutePoint> ();
		for (int i = 0; i < points.Length; i++) {
			if (i < points.Length - 1) {
				print("setting point");
				points[i].Next = points[i+1];
			}
			else {
				print("setting point");
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

	protected override bool isVisible()  {
		return false;
	}
}
