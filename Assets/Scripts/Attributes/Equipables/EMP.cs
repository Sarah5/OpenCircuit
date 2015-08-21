using UnityEngine;
using System.Collections;

public class EMP : Equipable {
	protected override Trigger buildTrigger() {
		return null;
	}

	protected override Trigger getTrigger() {
		return null;
	}

	public override void invoke(GameObject target){
		print ("invoked");
	}


}
