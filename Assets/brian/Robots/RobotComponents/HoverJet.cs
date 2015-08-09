using UnityEngine;
using System.Collections;

public class HoverJet : AbstractRobotComponent {

	public float speed = 1f;

	private RobotInterest target = null;

	private NavMeshAgent nav;

	private AbstractPowerSource powerSource;

	public void setTarget(RobotInterest target) {
		this.target = target;
	}

	public string getTargetType() {
		if (target == null) 
			return "";
		return target.Type;
	}

	public bool hasTarget() {
		return target != null;
	}

	void Start() {
		nav = roboController.GetComponent<NavMeshAgent> ();
		powerSource = roboController.GetComponentInChildren<AbstractPowerSource> ();
	}

	void Update () {
		if (powerSource == null) {
			return;
		}
		if (target != null) {
			if (Vector3.Distance (roboController.transform.position, target.transform.position) < .5f) {
				roboController.enqueueMessage (new RobotMessage ("action", "target reached", target));
			}

			if (nav.enabled)
				nav.SetDestination (target.transform.position);

			//if (!powerSource.drawPower (5 * Time.deltaTime)){
				nav.enabled = powerSource.drawPower (5 * Time.deltaTime);
		//}

		}
	}
}
