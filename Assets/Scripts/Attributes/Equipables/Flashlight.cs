using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Items/Flashlight")]
public class Flashlight : Item {

	Light light = null;

	public override void invoke(Inventory invoker) {
		if (light == null || light.enabled)
			turnOff();
		else
			turnOn();
    }

	public override void onEquip(Inventory equipper) {
		base.onEquip(equipper);
		createLight(equipper.getPlayer().cam.gameObject);
	}

	public override void onUnequip(Inventory equipper) {
		base.onUnequip(equipper);
		deleteLight();
	}

	protected void turnOn() {
		light.enabled = true;
	}

	protected void turnOff() {
		if (light == null)
			return;
		light.enabled = false;
	}

	protected void createLight(GameObject target) {
		deleteLight();
		light = target.AddComponent<Light>();
		light.enabled = false;
		light.type = LightType.Spot;
		light.range = 40;
		light.spotAngle = 60;
		light.color = Color.white;
		light.intensity = 0.5f;
//		light.bounceIntensity = 1;
		light.shadows = LightShadows.Soft;
//		light.shadowStrength = 1
		light.renderMode = LightRenderMode.Auto;
	}

	protected void deleteLight() {
		if (light == null)
			return;
		GameObject.Destroy(light);
		light = null;
	}
}
