using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Player/Interact")]
public class Interact : MonoBehaviour {

	private Player myPlayer;
	private const float grabDistance = 2.5f;
	private Camera playerCam;
	private GameObject target;
	private Color prevColor;

	// Use this for initialization
	void Start () {
		myPlayer = this.GetComponent<Player> ();
		playerCam = myPlayer.cam;
	}

	void Update() {
		updateTarget();
	}

	private void updateTarget() {
		GameObject trgt = null;
		trgt = reach();
		if(trgt != null && Vector3.Distance(trgt.GetComponent<Collider>().ClosestPointOnBounds(transform.position), transform.position) >= 1.5f) {
			trgt = null;
		}

		if(target == trgt) return;

		if(target != null) {
			unHighlight();
		}

		target = trgt;
		if(trgt != null && hasInteractable(trgt)) {
			highlight();
		}
	}

	private void highlight() {
		if(target.GetComponent<Renderer>() == null) return;
		prevColor = target.GetComponent<Renderer>().material.color;
		target.GetComponent<Renderer>().material.color = Color.green;
	}

	private void unHighlight() {
		if(target.GetComponent<Renderer>() == null) return;
		target.GetComponent<Renderer>().material.color = prevColor;
	}

	public void interact() {
		Grab grabber = myPlayer.grabber;
		Inventory inventory = myPlayer.inventory;
		if (grabber.hasObject ()) {
			grabber.dropObject ();
		} else {
			Vector3 point;
			GameObject nearest = reach (out point);
			if (nearest != null && !inventory.take(nearest)) {
				Label trgt;
				bool canInteract = hasInteractable(nearest, out trgt);
				if(canInteract) {
					InteractTrigger trig = new InteractTrigger();
					trig.setPoint(point);
					trgt.sendTrigger(nearest, trig);
				} else {
					grabber.GrabObject(nearest, point);
				}
            }
		}
	}

	private bool hasInteractable(GameObject obj, out Label target) {
		Label item = obj.GetComponent<Label>();
		if(item != null) {
			target = item;
			return item.hasOperationType(typeof(InteractTrigger));
		}
		target = null;
		return false;
	}

	private bool hasInteractable(GameObject obj) {
		Label output;
		return hasInteractable(obj, out output);
	}

	private GameObject reach() {Vector3 fake; return reach(out fake); }
	private GameObject reach(out Vector3 position) {
		GameObject finalHit = null;
		position = Vector3.zero;
		Ray ray = playerCam.ScreenPointToRay (new Vector3 (playerCam.pixelWidth / 2, playerCam.pixelHeight / 2));
		RaycastHit[] hits = Physics.RaycastAll(ray, grabDistance);
		float distance = grabDistance;

		foreach(RaycastHit hit in hits) {
			if ((hit.collider.ClosestPointOnBounds(transform.position) -transform.position).sqrMagnitude > grabDistance *grabDistance) continue;
			if (hit.distance > distance) continue;
			if (hit.collider.isTrigger) continue;
			finalHit = hit.collider.gameObject;
			distance = hit.distance;
			position = hit.point;
		}
		return finalHit;
	}
}
