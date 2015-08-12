using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotController : MonoBehaviour {

	private HashSet<Action> availableActions = new HashSet<Action> (new ActionComparer());
	private List<RobotInterest> trackedTargets = new List<RobotInterest> ();
	public RobotInterest[] locations;

	private Action currentAction = null;
	private List<Action> staleActions = new List<Action> ();

	MentalModel mentalModel = new MentalModel ();
	MentalModel externalMentalModel = null;
	
	Queue<RobotMessage> messageQueue = new Queue<RobotMessage>();


	void Start() {
		MeshRenderer gameObjectRenderer = GetComponent<MeshRenderer>();
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
				evaluateActions();
			}
			else if (message.Type.Equals("action")) {
				currentAction.onMessage(message);
			}
		}
	}

	public void evaluateActions() {
		PriorityQueue actionQueue = new PriorityQueue ();
		//foreach (Action action in currentActions) {
		if (currentAction != null) {
			if (currentAction.isStale()) {
				currentAction.stopExecution();
				currentAction = null;
			}
			else {
				actionQueue.Enqueue (currentAction);
			}
		}
		//}
		foreach (Action action in availableActions) {
			if (!action.isStale()) {
				actionQueue.Enqueue(action);
			}
			else {
				staleActions.Add(action);
			}
		}

		foreach (Action action in staleActions) {
			//print ("removing " + action.getName());
			availableActions.Remove(action);
		}
		staleActions.Clear ();

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
			else {
				actionQueue.Dequeue();
			}
		}
	}

	public void notify (EventMessage message){
		if (message.Type.Equals ("target found")) {
			trackTarget(message.Target);
		} else if (message.Type.Equals ("target lost")) {
			//print ("target lost: " + message.Target.Type);
			trackedTargets.Remove(message.Target);
		}
		evaluateActions();
	}
	
	public void addAction(Action action) {
		availableActions.Add (action);
		evaluateActions ();
	}

	public void trackTarget(RobotInterest target) {
		//print ("adding target: " + target.name);
		trackedTargets.Add (target);
		foreach (Action action in target.getAvailableActions(this)) {
			//print ("add action: " + action.getName());
			availableActions.Add(action);
		}
	}

	public bool knowsTarget(RobotInterest target) {
		return getMentalModel ().canSee (target);
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
}