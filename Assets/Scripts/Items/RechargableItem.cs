using UnityEngine;
using System.Collections;

public abstract class RechargableItem : ContextItem {

	public int maxCharges = 1;
	public int charges = 1;

	public void recharge() {
		charges = maxCharges;
	}

	public bool isCharged() {
		return charges == maxCharges;
	}

	public bool useCharge() {
		if(charges > 0) {
			--charges;
			return true;
		}
		return false;
	}
}
