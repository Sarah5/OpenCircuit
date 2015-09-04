using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Items/EMP")]
public class EMP : Item {

	public override void invoke(Inventory invoker) {
		print ("EMP invoked");
	}

    public override void onEquip(Inventory equipper) {
        print("EMP equiped");
    }

    public override void onUnequip(Inventory equipper) {
        print("EMP unequiped");
    }


}
