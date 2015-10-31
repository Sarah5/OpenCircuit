using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Scripts/Robot/Hover Jet")]
public class HoverJet : AbstractRobotComponent {

	public float distanceCost = 1;

	private Label target = null;

	private NavMeshAgent nav;
	
	private Animation myAnimator;

	private bool matchTargetRotation = false;

	public float animSpeedAdjust = 1f;

    public float powerDrawRate = 5f;

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
			Debug.LogWarning(roboController.name + " is missing a power source.");
			return;
		}
		if (target != null) {
			float xzDist = Vector2.Distance(new Vector2(roboController.transform.position.x, roboController.transform.position.z),
			                                new Vector2(target.transform.position.x, target.transform.position.z));
			float yDist = Mathf.Abs(roboController.transform.position.y - target.transform.position.y);
			if (xzDist < .5f && yDist < 1.2f) {
				if(!matchTargetRotation || (1 - Vector3.Dot(roboController.transform.forward, target.transform.forward) < .0001f)) {
					roboController.enqueueMessage(new RobotMessage("action", "target reached", target.labelHandle, target.transform.position));
					target = null;
					return;
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

				myAnimator["Armature.003|walk"].speed = nav.velocity.magnitude * animSpeedAdjust;//, nav.velocity * animSpeedAdjust, nav.velocity * animSpeedAdjust);
				}
			}

			//if (!powerSource.drawPower (5 * Time.deltaTime)){
		//}
		}
		nav.enabled = powerSource.drawPower(powerDrawRate * Time.deltaTime);
	}

	public float calculatePathCost(Label target) {
		//Debug.Log ("evaluating path cost");

		float cost = 0;
		NavMeshPath path = new NavMeshPath ();
		if (nav.enabled) {
			nav.CalculatePath (target.transform.position, path);
		}
		List<Vector3> corners = new List<Vector3>(path.corners);
		corners.Add(target.transform.position);
		//corners
		float pathLength = 0;
		foreach (LabelHandle item in roboController.getTrackedTargets()) {
				//print ("checking path cost against item: " + item.name);
				//print ("target threatLevel " + item.threatLevel);
				float minDist = -1;
				//Vector3 prevVertex;
				//Debug.Log("numCorners: " + corners.Count);
				for(int i = 0; i < corners.Count; i++) {
					Vector3 vertex = corners[i];
					if(i > 0) {
						//Debug.Log("adding path length");
						pathLength += Vector3.Distance(corners[i - 1], vertex);
					}
					float curDist = Vector3.Distance(vertex, item.label.transform.position);
					if(minDist == -1) {
						minDist = curDist;
					} else if(curDist < minDist) {
						minDist = curDist;
					}
				}
				if(item.label.hasTag(TagEnum.Threat)) {
					float threatLevel = item.label.getTag(TagEnum.Threat).severity;
					cost += threatLevel / minDist;
				}
		}
		//if (cost > 0) {
		//	print ("path cost: " + cost);
		//}
		//Debug.Log("cost for target '" + target.name + "': " + (cost + (pathLength * distanceCost)));
		return cost + (pathLength * distanceCost);	
	}

	public bool canReach(Label target) {
		if(nav.enabled) {
			//Debug.Log("got here");
			NavMeshPath path = new NavMeshPath ();

			nav.CalculatePath(target.transform.position, path);
			List<Vector3> corners = new List<Vector3>(path.corners);
			corners.Add(target.transform.position);
			for(int i = 0; i < corners.Count-1; i++) {
				//Debug.Log("in the loop");
				NavMeshHit hit = new NavMeshHit();
				
				if (NavMesh.Raycast(corners[i], corners[i+1], out hit, NavMesh.AllAreas)) {
					//Debug.Log("Raycast hit mask: " + hit.mask);
					return false;
				}
			}
			return true;
		} else {
			return false;
		}
	}
}
