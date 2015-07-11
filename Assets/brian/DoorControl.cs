using UnityEngine;
using System.Collections;

public class DoorControl : MonoBehaviour {

	Vector3 openPos;
	Vector3 closed;

	bool shouldOpen = false;
	bool shouldClose = false;

	bool isOpen = false;
	bool isClosed = true;

	void Awake() {
		closed = transform.forward;
		openPos = -transform.right;
		//shouldOpen = true;
	}

	void Update() {
		if (shouldOpen) {
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(openPos), 50.0f * Time.deltaTime);
			if (Mathf.Abs(Vector3.Dot(openPos, transform.forward) - 1) < .00001) {
				print ("open finished");
				shouldOpen = false;
				isOpen = true;
				isClosed = false;


			}
		} else if (shouldClose) {
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(closed), 50.0f * Time.deltaTime);
			if (Mathf.Abs(Vector3.Dot(openPos, -transform.right) - 1) < .00001) {
				shouldClose = false;
				isClosed = true;
				isOpen = false;

			}
		}
	}

	public void toggle() {
		if (isClosed) {
			print ("triggering door open");
			shouldOpen = true;
		} else if (isOpen) {
			print ("triggering door close");

			shouldClose = true;
		}

	}

}
