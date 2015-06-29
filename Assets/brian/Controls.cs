using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour {

	private Player myPlayer;
	//private Menu menu;

	public float mouseSensitivity = 1;
	public bool invertLook = false;

	void Awake () {
		myPlayer = this.GetComponent<Player> ();
		//menu = GameObject.FindGameObjectWithTag("Menu").GetComponent<Menu>();
	}

	void Update () {

		/****************MENU****************/
		//if (Input.GetButtonDown("Menu")) {
	//		menu.toggleInGameMenu();
	//	}
	//	if (menu.paused()) {
	//		return;
	//	 }

		/****************MOVEMENT****************/
		myPlayer.mover.setForward(Input.GetAxis("Vertical"));

		myPlayer.mover.setRight(Input.GetAxis("Horizontal"));

		//if (Input.GetButtonDown("Jump")) {
		//	myPlayer.mover.jump();
		//}

		myPlayer.mover.setSprinting(Input.GetButton("Sprint"));


		if (!inGUI()) {
			if (invertLook)
				myPlayer.looker.rotate(Input.GetAxis("Mouse X") * mouseSensitivity, -Input.GetAxis("Mouse Y") * mouseSensitivity);
			else
				myPlayer.looker.rotate(Input.GetAxis("Mouse X") * mouseSensitivity, Input.GetAxis("Mouse Y") * mouseSensitivity);
		}
		
		/****************ACTION****************/

		if (Input.GetButtonDown("Fire1")/* && !Input.GetButton("Fire2")*/) {
			//For now, do nothing!
		}
	}

	bool inGUI() {
		return false;
	}
}
