using UnityEngine;
using System.Collections;

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
//		GameObject trgt = null;
//		if (!myPlayer.focus.isFocused) {
//			trgt = reach();
//		}
//		if (trgt != null && Vector3.Distance(trgt.GetComponent<Collider>().ClosestPointOnBounds(transform.position), transform.position) >= 1.5f) {
//			trgt = null;
//		} 
//
//		if (target == trgt) return;
//
//		if (target != null) {
//			unHighlight();
//		}
//
//		target = trgt;
//		if (trgt != null) {
//			highlight();
//		}
	}

	private void highlight() {
		if (target.GetComponent<Renderer>() == null) return;
		prevColor = target.GetComponent<Renderer>().material.color;
		target.GetComponent<Renderer>().material.color = Color.green;
	}

	private void unHighlight() {
		if (target.GetComponent<Renderer>() == null) return;
		target.GetComponent<Renderer>().material.color = prevColor;
	}

	public void interact() {
//		Grab grabber = myPlayer.grabber;
//		Equip equipper = myPlayer.equipper;
//		if (grabber.hasObject ()) {
//			grabber.dropObject ();
//		}
//		else if (equipper.hasObject ()) {
//			equipper.dropObject();
//		}
//		else {
			Vector3 point;
			GameObject nearest = reach (out point);
			//if(equipper.equipObject(nearest)) {
			//}
			if (nearest != null){
				EventHandler item = nearest.GetComponent<EventHandler>();
				if (item != null) {
					//print("first point: " + point);
					InteractTrigger trig = new InteractTrigger();
					trig.setPoint(point);
					//trig.myPoint = point;
					//print("trig point: " + trig.point );
					item.sendTrigger(gameObject, trig);
				}
			}
//		}
	}

//	public void altInteract() {
//		myPlayer.grabber.pocketObject(reach ());
//	}

	private GameObject reach() {Vector3 fake; return reach(out fake); }
	private GameObject reach(out Vector3 position) {
		GameObject finalHit = null;
		position = Vector3.zero;
		Ray ray = playerCam.ScreenPointToRay (new Vector3 (playerCam.pixelWidth / 2, playerCam.pixelHeight / 2));
		RaycastHit[] hits = Physics.RaycastAll(ray, grabDistance);
		float distance = grabDistance;
		
		foreach(RaycastHit hit in hits) {
			if (Vector3.Distance(hit.collider.ClosestPointOnBounds(transform.position), transform.position) > 1.5f) continue;
			if (hit.distance > distance) continue;
			if (hit.collider.isTrigger) continue;
			finalHit = hit.collider.gameObject;
			distance = hit.distance;
			position = hit.point;
		}
		//print("dist: " + position);
		return finalHit;
	}
}
