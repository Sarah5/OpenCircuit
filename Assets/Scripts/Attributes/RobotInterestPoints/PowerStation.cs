using UnityEngine;
using System.Collections;

public class PowerStation : MonoBehaviour
{
    public float rechargeRate = 20f;

    void OnTriggerStay(Collider collision) {
        Battery battery = collision.gameObject.GetComponent<Battery>();
        if (battery != null) {
            battery.addPower(rechargeRate * Time.deltaTime);
        }
    }
}