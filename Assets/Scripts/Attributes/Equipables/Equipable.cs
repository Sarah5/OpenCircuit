using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Equipable : MonoBehaviour {

    protected float reachDistance = 3f;

	void Awake() {
//		gameObject.AddComponent<pickupEvent>().item = this;
	}

	public virtual void onPickup(InteractTrigger e) {
//		Player.getInstance().carryPickup(gameObject, e.getPoint());
	}

	public virtual void attack() {
		int counter = 0;
		foreach (AnimationState clip in GetComponent<Animation>()) {
			if (counter == 1) {
				this.GetComponent<Animation>().Play (clip.name);
			}
			counter++;
		}
	}

	protected virtual void trigger() {
		RaycastHit col = reach();
		if (col.collider != null) {
			Label evH = col.collider.GetComponent<Label>();
			if (evH != null) {
				evH.sendTrigger(gameObject, buildTrigger());
			}
		}
	}

    protected abstract Trigger buildTrigger();

	protected RaycastHit reach() { Vector3 fake; return reach(out fake); }
	protected RaycastHit reach(out Vector3 position) {
		RaycastHit finalHit = new RaycastHit();
		position = Vector3.zero;
		Ray ray = new Ray (this.transform.parent.position, this.transform.parent.forward);
		RaycastHit[] hits = Physics.RaycastAll(ray, reachDistance);
		float distance = reachDistance;
		
		foreach(RaycastHit hit in hits) {
			if (hit.distance > distance) continue;
			if (hit.collider.isTrigger) continue;
			finalHit = hit;
			distance = hit.distance;
			position = hit.point;
		}
		return finalHit;
	}

	public abstract void invoke(GameObject target);

    protected abstract Trigger getTrigger();
}

[System.Serializable]
public class pickupOperation : Operation {

	private static System.Type[] triggers = new System.Type[] {
		typeof(InteractTrigger),
	};

	public Equipable item;

	public override System.Type[] getTriggers() {
		return triggers;
	}

	public override void perform(GameObject instigator, Trigger trig) {
		InteractTrigger trigger = (InteractTrigger)trig;
		item.onPickup(trigger);
	}
}