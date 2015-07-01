using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotController : MonoBehaviour {

	MentalModel mentalModel = new MentalModel ();
	MentalModel externalMentalModel = null;

	Queue<RobotMessage> messageQueue = new Queue<RobotMessage>();

	// Update is called once per frame
	void Update () {
		while (messageQueue.Count > 0) {
			RobotMessage message = messageQueue.Dequeue();

			if (message.Type.Equals("target sighted")) {
				sightingFound(message.Target);
				if (message.Target.Type.Equals("player")) {
					turnGreen();
				}

			}
			else if (message.Type.Equals("target lost") && message.Target.Type.Equals("player")) {
				sightingLost(message.Target);
				if (message.Target.Type.Equals("player")) {
					turnRed();
				}
			}
		}
	}

	public void notify (EventMessage message){
		if (message.Type.Equals ("target found") && message.Target.Type.Equals ("player")) {
			turnGreen ();
		} else if (message.Type.Equals ("target lost") && message.Target.Type.Equals ("player")) {
			turnRed();
		}
	}

	public void attachMentalModel(MentalModel model) {
		externalMentalModel = model;
	}

	public void detachMentalModel () {
		externalMentalModel = null;
	}

	public void enqueueMessage(RobotMessage message) {
		messageQueue.Enqueue (message);
	}

	private void sightingLost(RobotInterest target) {
		if (externalMentalModel != null) {
			externalMentalModel.removeSighting(target);
		}
		mentalModel.removeSighting (target);
	}

	private void sightingFound(RobotInterest target) {
		if (externalMentalModel != null) {
			externalMentalModel.addSighting(target);
		}
		mentalModel.addSighting (target);
	}

	private MentalModel getMentalModel() {
		if (externalMentalModel == null) {
			return mentalModel;
		} else {
			return externalMentalModel; 
		}
	}
	
	private void turnRed() {
		Color whateverColor = new Color(255,0,0,1);
		
		MeshRenderer gameObjectRenderer = GetComponent<MeshRenderer>();
		
		Material newMaterial = new Material(Shader.Find("Transparent/Diffuse"));
		
		newMaterial.color = Color.red;
		gameObjectRenderer.material = newMaterial ;
	}

	private void turnGreen() {
		Color whateverColor = new Color(255,0,0,1);
		
		MeshRenderer gameObjectRenderer = GetComponent<MeshRenderer>();
		
		Material newMaterial = new Material(Shader.Find("Transparent/Diffuse"));
		
		newMaterial.color = Color.green;
		gameObjectRenderer.material = newMaterial ;
	}
}