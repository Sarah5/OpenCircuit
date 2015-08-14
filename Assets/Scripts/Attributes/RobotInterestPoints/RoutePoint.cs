﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoutePoint : Label {
	private RoutePoint next;

	void Start() {
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

	public override List<Endeavour> getAvailableEndeavours (RobotController controller) {
		List<Endeavour> actions = base.getAvailableEndeavours (controller);
		return actions;
	}
}