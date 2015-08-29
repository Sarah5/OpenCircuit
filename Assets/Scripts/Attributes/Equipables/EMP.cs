using UnityEngine;
using System.Collections;

public class EMP : Item {

	public override void invoke(GameObject target) {
		print ("EMP invoked");
	}

    public override void onEquip(Inventory equipper) {
        print("EMP equiped");
    }

    public override void onUnequip(Inventory equipper) {
        print("EMP unequiped");
    }


}
