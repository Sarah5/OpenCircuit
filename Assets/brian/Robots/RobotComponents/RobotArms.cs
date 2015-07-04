	using UnityEngine;
using System.Collections;

public class RobotArms : AbstractRobotComponent {
	public AudioClip footsteps;
	private AudioSource footstepEmitter;

	
	void Start() {
		footstepEmitter = gameObject.AddComponent<AudioSource>();
		footstepEmitter.enabled = true;
		footstepEmitter.loop = false;
	}

	void OnCollisionEnter(Collision collision) {
		footstepEmitter.PlayOneShot(footsteps, 1);
		attachTarget (collision.gameObject);

	}

	private void attachTarget(GameObject obj) {
		RobotInterest target = obj.GetComponent<RobotInterest> ();
		if (target != null) {
			Rigidbody rigidbody = obj.GetComponent<Rigidbody> ();
			if (rigidbody != null) {
				//rigidbody.useGravity = false;
				rigidbody.isKinematic = true;
				
			}
			target.transform.parent = transform;
			target.transform.localPosition = new Vector3(0, .5f, .85f);
		}
	}

}
