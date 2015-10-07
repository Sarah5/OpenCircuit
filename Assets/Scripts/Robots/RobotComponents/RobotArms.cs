using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Scripts/Robot/Robot Arms")]
public class RobotArms : AbstractRobotComponent {

    public static Vector3 HOLD_POSITION = new Vector3(0, .5f, .85f);

    public AudioClip pickUp;
	public AudioClip drop;

	public Vector3 throwForce = new Vector3(0, 150, 300);

	private AudioSource footstepEmitter;

	private Label target = null;
    private Label proposedTarget = null;

    private bool proposedTargetStatus = false;

	
	void Start() {
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

    void FixedUpdate() {
        if (proposedTarget == null && target == null) {
            return;
        }
        if (proposedTarget != null && !proposedTargetStatus) {
            proposedTarget = null;
        }

        if (target != null) {
            if (Vector3.Distance(target.transform.localPosition, HOLD_POSITION) > .0001f) {
                target = null;
            }
        }

        proposedTargetStatus = false;
        
    }

   void OnTriggerEnter(Collider collision) {
        if (target == null) {
            proposedTarget = collision.gameObject.GetComponent<Label>();
            proposedTargetStatus = true;
            if (proposedTarget != null && proposedTarget.hasTag(TagEnum.GrabTarget)) {

                footstepEmitter.PlayOneShot(pickUp, 1);
                roboController.addEndeavour(new HoldAction(roboController, proposedTarget, gameObject));
            }
        }
    }

    void OnTriggerStay(Collider collision) {
       Label label = collision.gameObject.GetComponent<Label>();
       if (label == proposedTarget) {
            proposedTargetStatus = true;
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
		}
	}

    public void attachTarget(Label obj) {
        if (target == null) {
            target = obj;
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
            if (rigidbody != null) {
                rigidbody.isKinematic = true;
                rigidbody.useGravity = false;
                rigidbody.velocity = new Vector3(0, 0, 0);
            }
            target.transform.parent = transform;
            target.transform.localPosition = HOLD_POSITION;//new Vector3(0, .5f, .85f);
            roboController.enqueueMessage(new RobotMessage("action", "target grabbed", target));
        }
    }

    public Label getProposedTarget() {
        return proposedTarget;
    }
}
