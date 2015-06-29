using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoboEyes : AbstractRobotComponent {

	public float sightDistance = 30;

	private Dictionary<RobotInterest, bool> targetMap = new Dictionary<RobotInterest, bool>();

	// Use this for initialization
	void Start () {

		InvokeRepeating ("lookAround", .5f, .01f);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	bool canSee (Transform obj) {
		Vector3 objPos = obj.position;
		bool result = false;
		if (Vector3.Distance (objPos, transform.position) < sightDistance) {
			RaycastHit hit;
			Physics.Raycast (transform.position, objPos - transform.position, out hit, sightDistance);
			if (hit.transform == obj ) {//&& Vector3.Dot (transform.forward.normalized, (objPos - transform.position).normalized) > 0) {
				result = true;
			}
		}
		return result;
	}

	void lookAround() {
		for (int i = 0; i < RobotInterest.interestPoints.Count; i++) {
			bool targetInView = canSee (RobotInterest.interestPoints [i].transform);
			if (!targetMap.ContainsKey (RobotInterest.interestPoints [i]) || !targetMap [RobotInterest.interestPoints[i]] && targetInView) {
				roboController.enqueueMessage(new RobotMessage("eyes", "target sighted", RobotInterest.interestPoints[i]));
				print ("target sighted");
			}
			else if (targetMap.ContainsKey(RobotInterest.interestPoints [i]) && targetMap [RobotInterest.interestPoints[i]] && !targetInView) {
				roboController.enqueueMessage(new RobotMessage("eyes", "target lost", RobotInterest.interestPoints[i]));
				print ("target lost");
			}
			targetMap [RobotInterest.interestPoints [i]] = targetInView;
		}
	}
}
