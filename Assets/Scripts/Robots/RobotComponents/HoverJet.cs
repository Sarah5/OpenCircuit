using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Robot/Hover Jet")]
public class HoverJet : AbstractRobotComponent {

	private Label target = null;

	private NavMeshAgent nav;
	

	private Animation myAnimator;

	public float animSpeedAdjust = 1f;

	public void setTarget(Label target) {
		this.target = target;
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
				roboController.enqueueMessage (new RobotMessage ("action", "target reached", target));
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
			//print ("checking path cost against item: " + item.name);
			if (Mathf.Abs(item.threatLevel) > .001) {
				//print ("target threatLevel " + item.threatLevel);
				float minDist = -1;
				foreach (Vector3 vertex in path.corners) {
					float curDist = Vector3.Distance(vertex, item.transform.position);
					if (minDist == -1) {
						minDist = curDist;
					}
					else if (curDist < minDist) {
						minDist = curDist;
					}
				}
				cost +=  item.threatLevel/minDist;
			}
		}
		//if (cost > 0) {
		//	print ("path cost: " + cost);
		//}
		return cost;	
	}
}
