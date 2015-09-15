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

	public string getTargetType() {
		if (target == null) 
			return "";
		return target.Type;
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
}
