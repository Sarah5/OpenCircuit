using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Robot/Hover Jet")]
public class HoverJet : AbstractRobotComponent {

	private Label target = null;

	private NavMeshAgent nav;
	
	private Animation myAnimator;

	private bool matchTargetRotation = false;

	public float animSpeedAdjust = 1f;

	public void setTarget(Label target, bool matchRotation = false) {
		this.target = target;
		matchTargetRotation = matchRotation;
	}

	public bool hasTarget() {
		return target != null;
	}

	void Start() {
		myAnimator = GetComponent<Animation> ();
		nav = roboController.GetComponent<NavMeshAgent> ();
	}

	void Update () {
		if (powerSource == null) {
			return;
		}
		if (target != null) {
			float xzDist = Vector2.Distance(new Vector2(roboController.transform.position.x, roboController.transform.position.z),
			                                new Vector2(target.transform.position.x, target.transform.position.z));
			float yDist = Mathf.Abs(roboController.transform.position.y - target.transform.position.y);
			if (xzDist < .5f && yDist < 1f) {
				if(!matchTargetRotation || (1 - Vector3.Dot(roboController.transform.forward, target.transform.forward) < .000001)) {
					roboController.enqueueMessage(new RobotMessage("action", "target reached", target));
				} else {
					roboController.transform.rotation = Quaternion.RotateTowards(Quaternion.LookRotation(roboController.transform.forward), Quaternion.LookRotation(target.transform.forward), nav.angularSpeed * Time.deltaTime);
				}
			}

			if (nav.enabled) {
				nav.SetDestination (target.transform.position);
				if (myAnimator != null) {
				if (!myAnimator.isPlaying) {
					myAnimator.Play();
				}

				myAnimator["Armature.003|Armature.003Action"].speed = nav.velocity.magnitude * animSpeedAdjust;//, nav.velocity * animSpeedAdjust, nav.velocity * animSpeedAdjust);
				}
			}

			//if (!powerSource.drawPower (5 * Time.deltaTime)){
			nav.enabled = powerSource.drawPower (5 * Time.deltaTime);
		//}
		}
	}

	public float calculatePathCost(Label target) {
		//Debug.Log ("evaluating path cost");

		float cost = 0;
		NavMeshPath path = new NavMeshPath ();
		if (nav.enabled) {
			nav.CalculatePath (target.transform.position, path);
		}

		foreach (Label item in roboController.getTrackedTargets()) {
			if(item.hasTag(TagEnum.Threat)) {
				//print ("checking path cost against item: " + item.name);
				//print ("target threatLevel " + item.threatLevel);
				float threatLevel = item.getTag(TagEnum.Threat).severity;
				float minDist = -1;
				foreach(Vector3 vertex in path.corners) {
					float curDist = Vector3.Distance(vertex, item.transform.position);
					if(minDist == -1) {
						minDist = curDist;
					} else if(curDist < minDist) {
						minDist = curDist;
					}
				}
				cost += threatLevel / minDist;
			}
		}
		//if (cost > 0) {
		//	print ("path cost: " + cost);
		//}
		return cost;	
	}
}
