using UnityEngine;
using System.Collections;

public class PowerStation : MonoBehaviour
{

    void OnTriggerStay(Collider collision) {
        Battery battery = collision.gameObject.GetComponent<Battery>();
        if (battery != null) {
            battery.addPower(20f * Time.deltaTime);
        }
    }
}