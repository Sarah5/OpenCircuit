using UnityEngine;
using System.Collections;

public class Battery : AbstractPowerSource {

	public float maximumCapacity = 100;
	public float currentCapacity = 100;

	void Update () {
		drawPower (0.01f);
	}

	public override bool drawPower (float amount){
		if (currentCapacity < amount){
			return false;
		}
		float subtractAmount = amount - currentCapacity;
			return true;


	
	}


}