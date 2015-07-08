using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotController : MonoBehaviour {

	//private Dictionary<string, AbstractRobotComponent> abilities
	private List<RobotInterest> trackedTargets = new List<RobotInterest> ();

	MentalModel mentalModel = new MentalModel ();
	MentalModel externalMentalModel = null;
	HoverJet jet = null;

	private Material original;


	Queue<RobotMessage> messageQueue = new Queue<RobotMessage>();


	void Start() {
		jet = GetComponentInChildren<HoverJet>();
		MeshRenderer gameObjectRenderer = GetComponent<MeshRenderer>();
		original = gameObjectRenderer.material;

	}

	// Update is called once per frame
	void Update () {
		while (messageQueue.Count > 0) {
			RobotMessage message = messageQueue.Dequeue();

			if (message.Type.Equals("target sighted")) {
				sightingFound(message.Target);
				if (message.Target.Type.Equals("player")) {
					//lightUp();
					trackedTargets.Add(message.Target);

					/*if (jet != null) {
						jet.setTarget(message.Target);
					}*/
				}
			}
			else if (message.Type.Equals("target lost")) {
				sightingLost(message.Target);
				if (message.Target.Type.Equals("player")) {
					resetMaterial();
					trackedTargets.Remove(message.Target);
					/*if (jet != null) {
						jet.setTarget(null);
					}*/
				}
			}

			else if (message.Type.Equals("target reached")) {
				if (jet != null) {
					jet.setTarget(null);
					if (message.Target.Type.Equals("routePoint")) {
						print ("target reached received and jet != null: " + message.Target.Type);

						jet.setTarget(((RoutePoint)message.Target).Next);
					}
				}
			}

		}

		for (int i = 0; i < trackedTargets.Count; i++) {
			if (trackedTargets[i].Type.Equals("player")) {
				if (jet != null) {
					jet.setTarget(trackedTargets[i]);
				}
				break;
			}
			else if (trackedTargets[i].Type.Equals("patrolRoute")) {
				if (jet != null) {
					jet.setTarget(((PatrolRoute)trackedTargets[i]).getNearest(transform.position));
				}
			}
		}
		if (trackedTargets.Count == 0) {
			if (jet != null) {
				jet.setTarget(null);
			}
		}
	}

	public void notify (EventMessage message){
		print ("notify target: " + message.Target.Type);
		if (message.Type.Equals ("target found") && message.Target.Type.Equals ("player")) {
			trackedTargets.Add(message.Target);
		} else if(message.Type.Equals ("target found") && message.Target.Type.Equals ("patrolRoute")) {
			trackedTargets.Add(message.Target);
			print ("route added");

		} else if (message.Type.Equals ("target lost") && message.Target.Type.Equals ("player")) {
			trackedTargets.Remove(message.Target);

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
	
//	private void lightUp() {
		/*MeshRenderer gameObjectRenderer = GetComponent<MeshRenderer>();
		
		Material newMaterial = new Material(Shader.Find("Transparent/Diffuse"));
		
		newMaterial.color = Color.green;
		gameObjectRenderer.material = newMaterial ;*/
	//}

	private void resetMaterial() {

		MeshRenderer gameObjectRenderer = GetComponent<MeshRenderer>();

		gameObjectRenderer.material = original ;
	}
}