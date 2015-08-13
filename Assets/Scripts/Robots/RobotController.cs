using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotController : MonoBehaviour {

	private HashSet<Action> availableActions = new HashSet<Action> (new ActionComparer());
	private List<RobotInterest> trackedTargets = new List<RobotInterest> ();
	public RobotInterest[] locations;

	private HashSet<Action> currentActions = new HashSet<Action>();
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
				foreach( Action action in currentActions) {
					action.onMessage(message);
				}
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

	private void evaluateActions() {
		PriorityQueue actionQueue = new PriorityQueue ();
		foreach (Action action in currentActions) {
			if (action != null) {
				if (action.isStale ()) {
					//print("stop executing: " + action.getName());
					action.stopExecution ();
					staleActions.Add (action);	
				} else {
					//print("enqueue: " + action.getName());
					
					actionQueue.Enqueue (action);
				}
			}
			
		}
		foreach (Action action in availableActions) {
			if (!action.isStale()) {
				actionQueue.Enqueue(action);
			}
			else {
				//print("mark as stale: " + action.getName());

				staleActions.Add(action);
			}
		}
		
		foreach (Action action in staleActions) {
			//print ("removing " + action.getName());
			availableActions.Remove(action);
			currentActions.Remove(action);
		}
		staleActions.Clear ();
		HashSet<Action> proposedActions = new HashSet<Action> ();
		
		//if (currentAction != (Action)actionQueue.peek()) {
		//	if (currentAction != null) {
		//		currentAction.stopExecution();
		//		currentAction = null;
		//	}
		//}
		Dictionary<System.Type, int> componentMap = getComponentMap ();
		while (actionQueue.Count > 0) {
			if (((Action)actionQueue.peek()).canExecute(componentMap)) {
				Action action = (Action)actionQueue.Dequeue();
				//print("propose: " + action.getName());
				proposedActions.Add(action);
				//action.execute();
			}
			else {
				actionQueue.Dequeue();
			}
		}
		
		List<Action> toExecute = new List<Action> ();
		foreach (Action action in proposedActions) {
			if (currentActions.Contains(action)) {
				currentActions.Remove(action);
				
			}
			else {
				toExecute.Add(action);
			}
		}
		
		foreach (Action action in currentActions) {
			//print("stop executing: " + action.getName());
			action.stopExecution();
		}
		
		foreach (Action action in toExecute) {
			//print("start executing: " + action.getName());
			
			action.execute();
		}
		currentActions = proposedActions;
	}

	private Dictionary<System.Type, int> getComponentMap() {
		Dictionary<System.Type, int> componentMap = new Dictionary<System.Type, int>();
		AbstractRobotComponent [] components = GetComponentsInChildren<AbstractRobotComponent> ();
		foreach (AbstractRobotComponent component in components) {
			if (componentMap.ContainsKey(component.GetType())) {
				int count = componentMap[component.GetType()];
				++count;
				componentMap[component.GetType()] = count;
			}
			else {
				componentMap[component.GetType()] = 1;
			}
		}
		return componentMap;
	}

	private void sightingLost(RobotInterest target) {

		if (externalMentalModel != null) {
			externalMentalModel.removeSighting(target);
		}
		mentalModel.removeSighting (target);
	}

	private void sightingFound(RobotInterest target) {
		if (externalMentalModel != null) {
			//print("adding target " + target.Type);
			externalMentalModel.addSighting(target);
			//print (externalMentalModel.canSee(target));
		}
		mentalModel.addSighting (target);
	}

	public MentalModel getMentalModel() {
		if (externalMentalModel == null) {
			return mentalModel;
		} else {
			//print ("external mental model");

			return externalMentalModel; 
		}
	}
}