using UnityEngine;
using System.Collections;

public class RoutePoint : RobotInterest {
	private RoutePoint next;

	void Awake() {
		Type = "routePoint";
	}

	public RoutePoint Next {
		get {
			return next;
		}
		set {
			next = value;
		}
	}

	protected override bool isVisible()  {
		return false;
	}
}
