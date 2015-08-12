using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	public override List<Action> getAvailableActions (RobotController controller) {
		List<Action> actions = base.getAvailableActions (controller);
		return actions;
	}
}
