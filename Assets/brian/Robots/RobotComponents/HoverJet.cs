using UnityEngine;
using System.Collections;

public class HoverJet : AbstractRobotComponent {

	public float speed = 1f;

	private RobotInterest target = null;

	private NavMeshAgent nav;

	
	public void setTarget(RobotInterest target) {
		this.target = target;
	}

	void Start() {
		nav = GetComponentInParent<NavMeshAgent> ();
	}

	void Update () {
		if (target != null) {

			NavMeshPath path = new NavMeshPath ();
			if (nav.enabled)
				nav.SetDestination (target.transform.position);
		}
	}
}
