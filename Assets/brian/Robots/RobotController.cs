using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotController : MonoBehaviour {

	private HashSet<Action> availableActions = new HashSet<Action> (new ActionComparer());
	private List<RobotInterest> trackedTargets = new List<RobotInterest> ();
	public RobotInterest[] locations;

	//private List<Action> currentActions = new List<Action>();
	private Action currentAction = null;

	MentalModel mentalModel = new MentalModel ();
	MentalModel externalMentalModel = null;
	HoverJet jet = null;
	RobotArms arms = null;
	NavMeshAgent agent = null;

	private Material original;

	Queue<RobotMessage> messageQueue = new Queue<RobotMessage>();


	void Start() {
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
				trackTarget(message.Target);
				evaluateActions();
			}
			else if (message.Type.Equals("target lost")) {
				sightingLost(message.Target);
				trackedTargets.Remove(message.Target);
			}
			else if (message.Type.Equals("action")) {
				currentAction.onMessage(message);
			}

			/*else if (message.Type.Equals("target reached")) {
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
			}*/
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
	}

	public void evaluateActions() {
		PriorityQueue actionQueue = new PriorityQueue ();
		//foreach (Action action in currentActions) {
		if (currentAction != null) {
			actionQueue.Enqueue (currentAction);
		}
		//}
		foreach (Action action in availableActions) {
			actionQueue.Enqueue(action);
		}

		if (currentAction != (Action)actionQueue.peek()) {
			if (currentAction != null) {
				currentAction.stopExecution();
				currentAction = null;
			}

		}
		while (currentAction == null && actionQueue.Count > 0) {
			if (((Action)actionQueue.peek()).canExecute()) {
				currentAction = (Action)actionQueue.Dequeue();
				currentAction.execute();
			}
		}
	}

	public void notify (EventMessage message){
		if (message.Type.Equals ("target found")) {
			trackTarget(message.Target);
			evaluateActions();
		} else if (message.Type.Equals ("target lost")) {
			trackedTargets.Remove(message.Target);
		}
	}

	public void trackTarget(RobotInterest target) {
		print ("adding target: " + target.name);
		trackedTargets.Add (target);
		foreach (Action action in target.getAvailableActions(this)) {
			print ("\t" + action.getName());
			availableActions.Add(action);
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