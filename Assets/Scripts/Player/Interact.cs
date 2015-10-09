using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Player/Interact")]
public class Interact : MonoBehaviour {

	public Material highlightMaterial;

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
		GameObject trgt = reach();
		if(trgt != null && Vector3.Distance(trgt.GetComponent<Collider>().ClosestPointOnBounds(transform.position), transform.position) >= 1.5f) {
			trgt = null;
		}

		if(target == trgt) return;

		if(target != null && hasInteractable(target)) {
			unHighlight();
		}

		target = trgt;
		if(trgt != null && hasInteractable(trgt)) {
			highlight();
		}
	}

	private void highlight() {
		Renderer rend = target.GetComponent<Renderer>();
		if(rend == null) return;
		Material[] mats = rend.materials;
		Material[] newMats = new Material[mats.Length * 2];
		System.Array.Copy(mats, newMats, mats.Length);
		for(int i = mats.Length; i < newMats.Length; ++i) {
			newMats[i] = highlightMaterial;
		}
		rend.materials = newMats;
	}

	private void unHighlight() {
		Renderer rend = target.GetComponent<Renderer>();
		if(rend == null) return;
		Material[] mats = rend.materials;
		Material[] newMats = new Material[mats.Length / 2];
		System.Array.Copy(mats, newMats, newMats.Length);
		rend.materials = newMats;
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
