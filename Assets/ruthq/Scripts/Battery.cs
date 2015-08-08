using UnityEngine;
using System.Collections;

public class Battery : AbstractPowerSource {
	public override bool drawPower (float amount){
		return true; 
	}
}
