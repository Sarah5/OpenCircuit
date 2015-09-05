using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Scripts/Robot/Robo Eyes")]
public class RoboEyes : AbstractRobotComponent {

	public float fieldOfViewAngle = 140f;           // Number of degrees, centered on forward, for the enemy sight.
	public float sightDistance = 30.0f;

	private Dictionary<Label, bool> targetMap = new Dictionary<Label, bool>();

	// Use this for initialization
	void Start () {
		InvokeRepeating ("lookAround", 0.5f, .03f);
	}

	private bool canSee (Transform obj) {
		Vector3 objPos = obj.position;
		bool result = false;
		if (Vector3.Distance (objPos, transform.position) < sightDistance) {
			RaycastHit hit;
			Vector3 dir = objPos - transform.position;
			dir.Normalize();
			float angle = Vector3.Angle(dir, transform.forward);
//			print (roboController.gameObject.name);
//			print (angle);
			if(angle < fieldOfViewAngle * 0.5f) {
				Physics.Raycast (transform.position, dir, out hit, sightDistance);
				if (hit.transform == obj ) {//&& Vector3.Dot (transform.forward.normalized, (objPos - transform.position).normalized) > 0) {
					result = true;
				}
			}
		}
		return result;
	}

	private void lookAround() {
		bool hasPower = powerSource.drawPower (5 * Time.deltaTime);
		foreach (Label label in Label.visibleLabels) {
			bool targetInView = hasPower && canSee (label.transform);
			if ((!targetMap.ContainsKey (label) || !targetMap [label]) && targetInView) {
				roboController.enqueueMessage(new RobotMessage("target sighted", "target sighted", label));
			}
			else if (targetMap.ContainsKey(label) && targetMap [label] && !targetInView) {
				roboController.enqueueMessage(new RobotMessage("target lost", "target lost", label));
			}
			targetMap [label] = targetInView;
		}
	}
}
