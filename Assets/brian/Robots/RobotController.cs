using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotController : MonoBehaviour {

	private PriorityQueue availableActions = new PriorityQueue ();
	private List<RobotInterest> trackedTargets = new List<RobotInterest> ();
	public RobotInterest[] locations;

	MentalModel mentalModel = new MentalModel ();
	MentalModel externalMentalModel = null;
	HoverJet jet = null;
	RobotArms arms = null;
	NavMeshAgent agent = null;

	private Material original;


	Queue<RobotMessage> messageQueue = new Queue<RobotMessage>();


	void Start() {
		jet = GetComponentInChildren<HoverJet>();
		arms = GetComponentInChildren<RobotArms>();
		agent = GetComponentInChildren<NavMeshAgent>();


		MeshRenderer gameObjectRenderer = GetComponent<MeshRenderer>();
		original = gameObjectRenderer.material;
		foreach (RobotInterest location in locations) {
			sightingFound(location);
			trackedTargets.Add(location);
		}
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
					if (jet != null) {
						jet.setTarget(null);
					}
				}
			}

			else if (message.Type.Equals("target reached")) {
				if (jet != null) {
					jet.setTarget(null);
					if (message.Target.Type.Equals("routePoint")) {
						//print ("target reached received and jet != null: " + message.Target.Type);

						jet.setTarget(((RoutePoint)message.Target).Next);
					}
					else if (message.Target.Type.Equals("dropPoint")) {
						arms.dropTarget();
					}
				}
			}
			else if (message.Type.Equals("target grabbed")) {
				if (jet != null) {
					for (int i = 0; i < trackedTargets.Count; i++) {
						if (trackedTargets[i].Type.Equals("dropPoint")) {
							agent.speed = 10;
							jet.setTarget(trackedTargets[i]);
							break;
						}
					}
				}
				sightingFound(message.Target);
			}
			else if (message.Type.Equals("target dropped")) {
				agent.speed = 3;
				sightingLost(message.Target);
				if (jet != null) {
					jet.setTarget(null);
				}
			}
		}

		for (int i = 0; i < trackedTargets.Count; i++) {
			if (trackedTargets[i].Type.Equals("player")) {
				if (jet != null && !jet.getTargetType().Equals("dropPoint")) {
					jet.setTarget(trackedTargets[i]);
				}
				break;
			}
			else if (trackedTargets[i].Type.Equals("patrolRoute")) {
				if (jet != null && !jet.hasTarget()) {
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
		if (message.Type.Equals ("target found") && message.Target.Type.Equals ("player")) {
			trackedTargets.Add(message.Target);
		} else if(message.Type.Equals ("target found") && message.Target.Type.Equals ("patrolRoute")) {
			trackedTargets.Add(message.Target);
		} else if(message.Type.Equals ("target found") && message.Target.Type.Equals ("dropPoint")) {
			trackedTargets.Add(message.Target);
		} else if (message.Type.Equals ("target lost") && message.Target.Type.Equals ("player")) {
			trackedTargets.Remove(message.Target);
				if (jet != null) {
					if (jet.getTargetType().Equals("player")) {

						jet.setTarget(null);
					}
				}
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