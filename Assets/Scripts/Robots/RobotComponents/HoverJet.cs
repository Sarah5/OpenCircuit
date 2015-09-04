using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Robot/Hover Jet")]
public class HoverJet : AbstractRobotComponent {

	private Label target = null;

	private NavMeshAgent nav;

	private AbstractPowerSource powerSource;

	private Animation myAnimator;

	public float animSpeedAdjust = 1f;

	public void setTarget(Label target) {
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
		myAnimator = GetComponent<Animation> ();
		nav = roboController.GetComponent<NavMeshAgent> ();
		powerSource = roboController.GetComponentInChildren<AbstractPowerSource> ();
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
}
