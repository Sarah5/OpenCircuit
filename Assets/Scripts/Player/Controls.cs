using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Player/Controls")]
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

		if (Input.GetButtonDown("Jump")) {
			myPlayer.mover.jump();
		}

		myPlayer.mover.setSprinting(Input.GetButton("Sprint"));

		myPlayer.mover.setCrouching(Input.GetButton("Crouch"));


		if (!inGUI()) {
			if (invertLook)
				myPlayer.looker.rotate(Input.GetAxis("Look Horizontal") * mouseSensitivity, -Input.GetAxis("Look Vertical") * mouseSensitivity);
			else
				myPlayer.looker.rotate(Input.GetAxis("Look Horizontal") * mouseSensitivity, Input.GetAxis("Look Vertical") * mouseSensitivity);
		}
		
		/****************ACTION****************/

//		if (Input.GetButtonDown("Fire1")/* && !Input.GetButton("Fire2")*/) {
//			// For now, do nothing!
//		}

		if (Input.GetButtonDown ("Interact")) {
			if (inGUI()) {
			} else {
				//print("calling interact method");
				myPlayer.interactor.interact();
			}
		}
//		if (Input.GetButtonDown ("Alt_Interact")) {
//			if (inGUI()) {
//			} else {
//				myPlayer.interactor.altInteract();
//			}
//		}
//		if (Input.GetButtonDown("Fire1") && !Input.GetButton("Fire2")) {
//			if (inGUI ()) {
//			}
//			else {
//				myPlayer.attacker.attack();
//			}
//		}
//		if (Input.GetButton ("Fire2")) {
//			if (inGUI()) {
//				myPlayer.focus.unfocus ();
//			} else {
//				myPlayer.focus.focus ();
//				if (Input.GetButtonDown("Fire1")) {
//					//myPlayer.attacher.attach ();
//                    myPlayer.focus.invoke();
//				}
//			}
//		}
//		else {
//			myPlayer.focus.unfocus();
//		}
//		if (Input.GetAxis ("Rotate") > 0)
//			myPlayer.grabber.increment();
//		else if (Input.GetAxis ("Rotate") < 0)
//			myPlayer.grabber.decrement();
	}

	bool inGUI() {
		return false;
	}
}
