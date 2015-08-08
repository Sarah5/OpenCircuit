using UnityEngine;
using System.Collections;

public class PowerGenerator : AbstractPowerSource {
	public override bool drawPower (float amount){
		return true; 
	}
}