using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Robot/Robot Arms")]
public class RobotArms : AbstractRobotComponent {
	public AudioClip pickUp;
	public AudioClip drop;

	public Vector3 throwForce = new Vector3(0, 150, 300);
	

	private AudioSource footstepEmitter;

	private Label target = null;
	private HoldAction action;

	
	void Start() {
		action = new HoldAction (roboController, null);
		footstepEmitter = gameObject.AddComponent<AudioSource>();
		footstepEmitter.enabled = true;
		footstepEmitter.loop = false;
	}

	void Update () {
		BoxCollider collider = GetComponent<BoxCollider> ();
		if (powerSource == null || !powerSource.drawPower (5 * Time.deltaTime)) {
			collider.enabled = false;
			dropTarget ();
		}
		else {
			collider.enabled = true;
		}
	}

	void OnTriggerEnter(Collider collision) {
		if (target == null) {
			target = collision.gameObject.GetComponent<Label> ();
			if (target != null) {
				footstepEmitter.PlayOneShot (pickUp, 1);
				action.setTarget(target);
				roboController.addEndeavour(action);
			}
		}
	}

	void OnTriggerExit(Collider collision) {
		if (target != null) {
			action.setTarget (null);
			target = null;
		}

	}

	public bool hasTarget() {
		return target != null;
	}

	public void dropTarget() {
		if (target != null) {
			Rigidbody rigidbody = target.GetComponent<Rigidbody> ();
			if (rigidbody != null) {
				rigidbody.isKinematic = false;
				rigidbody.useGravity = true;
				rigidbody.AddForce(transform.forward * throwForce.z);
				rigidbody.AddForce(transform.up * throwForce.y);
			}
			target.transform.parent = null;
			roboController.enqueueMessage(new RobotMessage("action", "target dropped", target));
			footstepEmitter.PlayOneShot (drop, 1);

			target = null;
			//target.transform.localPosition = new Vector3(0, .5f, .85f);
		}
	}

	public void attachTarget(Label obj) {
		Rigidbody rigidbody = obj.GetComponent<Rigidbody> ();
		if (rigidbody != null) {
			rigidbody.isKinematic = true;
			rigidbody.useGravity = false;
			rigidbody.velocity = new Vector3(0,0,0);
		}
		target.transform.parent = transform;
		target.transform.localPosition = new Vector3(0, .5f, .85f);
		roboController.enqueueMessage(new RobotMessage("action", "target grabbed", target));
	}

}
