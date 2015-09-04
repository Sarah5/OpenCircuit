using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Robot/Power Generator")]
public class PowerGenerator : AbstractPowerSource {
	public override bool drawPower (float amount){
		return base.drawPower(amount) && true; 
	}
}