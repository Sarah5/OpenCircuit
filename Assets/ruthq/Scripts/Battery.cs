using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Robot/Battery")]
public class Battery : AbstractPowerSource {

	public float maximumCapacity = 100;
	public float currentCapacity = 100;

	void Update () {
		drawPower (1.0f * Time.deltaTime);
	}

	public override bool drawPower (float amount){
		if (currentCapacity < amount){
			return false;
		}
		float subtractAmount = currentCapacity - amount;
		currentCapacity = subtractAmount;
		return base.drawPower(amount) && true;
	}


}