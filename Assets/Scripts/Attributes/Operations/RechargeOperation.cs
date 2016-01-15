using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RechargeOperation : Operation {

	private static System.Type[] triggers = new System.Type[] {
		typeof(InteractTrigger),
	};

	public override System.Type[] getTriggers() {
		return triggers;
	}
	
	public override void perform(GameObject instigator, Trigger trig) {
		Inventory inventory = instigator.GetComponent<Inventory>();
		Debug.Log("recharging ");

		if(inventory != null) {
			List<RechargableItem> items = inventory.getItemsExtending<RechargableItem>();
			PowerStation station = parent.GetComponent<PowerStation>();

			List<RechargableItem> toRecharge = new List<RechargableItem>();
			foreach(RechargableItem item in items) {
				if(!item.isCharged()) {
					toRecharge.Add(item);
				}
			}
			if(station != null) {
				station.rechargeItems(toRecharge, instigator);
			}
		}
	}
}
