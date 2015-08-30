using UnityEngine;
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
}
