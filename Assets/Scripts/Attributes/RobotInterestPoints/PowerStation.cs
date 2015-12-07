using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerStation : MonoBehaviour
{
    public float rechargeRate = 20f;
	public float itemRechargeRate = 1f;
	public float requiredChargingProximity = 1.5f;
	private List<RechargableItem> rechargables;
	private GameObject client;

	public AudioClip rechargeItemSound;
	private AudioSource soundEmitter;

	void Awake() {
		soundEmitter = gameObject.AddComponent<AudioSource>();
	}

    void OnTriggerStay(Collider collision) {
        Battery battery = collision.gameObject.GetComponent<Battery>();
        if (battery != null) {
            battery.addPower(rechargeRate * Time.deltaTime);
        }
    }

	public void rechargeItems(List<RechargableItem> rechargableItems, GameObject holder) {
		rechargables = rechargableItems;
		client = holder;
		InvokeRepeating("rechargeNextItem", itemRechargeRate, itemRechargeRate);
	}

	private void rechargeNextItem() {
		if(rechargables == null || rechargables.Count == 0 || client == null || Vector3.Distance(transform.position, client.transform.position) > requiredChargingProximity) {
			CancelInvoke();
			rechargables = null;
			client = null;
			return;
		}
		RechargableItem item = rechargables[0];
		item.recharge();
		rechargables.RemoveAt(0);
		//TODO play a sound
		if(rechargeItemSound != null) {
			soundEmitter.PlayOneShot(rechargeItemSound);
		}
	}
}