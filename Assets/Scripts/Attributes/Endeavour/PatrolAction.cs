using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatrolAction : Endeavour {

	//private PatrolRoute route;
	//private List<Label> points;
	private List<Label> routePoints;
	private int currentDestination;

	public PatrolAction(RobotController controller, List<Goal> goals, List<Label> route) : base(controller, goals) {
		//this.route = route;
		this.name = "patrol";
		requiredComponents = new System.Type[] {typeof(HoverJet)};
		//this.points = route;
		//routePoints = new List<Label> ();
		routePoints = route;
		/*foreach (GameObject point in points) {
			Label label = point.GetComponent<Label>();
			if (label != null) {
				routePoints.Add(label);
			}
		}*/
	}

	public override void execute (){
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		if (jet != null) {
			currentDestination = getNearest(controller.transform.position);
			jet.setTarget(routePoints[currentDestination]);
			jet.setAvailability(false);
		}
	}

	public override void stopExecution(){
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		if (jet != null) {
			jet.setAvailability(true);
			jet.setTarget(null);
		}
	}

	public override void onMessage(RobotMessage message) {
		if (message.Message.Equals ("target reached")) {
			HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
			if (jet != null) {
				++currentDestination;
				if (currentDestination == routePoints.Count) {
					currentDestination = 0;
				}
				if(routePoints[currentDestination] == null) {
					Debug.LogWarning("Robot '" + controller.name + "' has detected a missing patrol route point. ");
					Debug.LogWarning("Robot '" + controller.name + "' halted. ");
				} else {
					jet.setTarget(routePoints[currentDestination]);
				}
			}
		}
	}

	public override bool isStale() {
		return false;
	}

	public int getNearest(Vector3 position) {
		float minDist;
		int index = 0;
		minDist = Vector3.Distance(position, routePoints[0].transform.position);
		for (int i = 0; i < routePoints.Count; i++) {
			if(routePoints[i] == null) {
				Debug.LogWarning("Robot '"+controller.name+"' has detected a missing patrol route point!!!");
				continue;
			}
			float curDist = Vector3.Distance(position, routePoints[i].transform.position);
			if (curDist < minDist) {
				minDist = curDist;
				index = i;
			}
		}
		return index;
	}

	protected override float getCost() {
		HoverJet jet = controller.GetComponentInChildren<HoverJet> ();
		if (jet != null) {
			return jet.calculatePathCost(routePoints[currentDestination]);
		}
		return 0;
	}
}
