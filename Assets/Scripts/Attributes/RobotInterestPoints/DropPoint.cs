using UnityEngine;
using System.Collections;

public class DropPoint : Label {

	void Start() {
		Type = "dropPoint";
	}
	
	protected override bool isVisible()  {
		return false;
	}
}
