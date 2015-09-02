using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotController : MonoBehaviour {

	private HashSet<Endeavour> availableEndeavours = new HashSet<Endeavour> (new EndeavourComparer());
	private List<Label> trackedTargets = new List<Label> ();
	public Label[] locations;
	public Dictionary<string, Goal> goals = new Dictionary<string, Goal>();

	private HashSet<Endeavour> currentEndeavours = new HashSet<Endeavour>();
	private List<Endeavour> staleEndeavours = new List<Endeavour> ();

	MentalModel mentalModel = new MentalModel ();
	MentalModel externalMentalModel = null;
	
	Queue<RobotMessage> messageQueue = new Queue<RobotMessage>();

	private bool dirty = false;

	void Start() {
		goals.Add("protection", new Goal("protection", 1));
		goals.Add("offense", new Goal("offense", 1));
		goals.Add ("self-preservation", new Goal ("self-preservation", 1));

		MeshRenderer gameObjectRenderer = GetComponent<MeshRenderer>();
		foreach (Label location in locations) {
			if (location == null) {
				Debug.LogWarning("Null location attached to AI with name: " + gameObject.name);
				continue;
			}
			sightingFound(location);
			trackTarget(location);
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
				foreach( Endeavour action in currentEndeavours) {
					action.onMessage(message);
				}
			}
		}
		if (dirty) {
			evaluateActions();
		}
	}



	public void notify (EventMessage message){
		if (message.Type.Equals ("target found")) {
			trackTarget(message.Target);
		} else if (message.Type.Equals ("target lost")) {
			//print ("target lost: " + message.Target.Type);
			trackedTargets.Remove(message.Target);
		}
		dirty = true;
	}
	
	public void addEndeavour(Endeavour action) {
		availableEndeavours.Add (action);
		evaluateActions ();
	}

	public void trackTarget(Label target) {
		//print ("adding target: " + target.name);
		trackedTargets.Add (target);
		foreach (Endeavour action in target.getAvailableEndeavours(this)) {
			//print ("add action: " + action.getName());
			availableEndeavours.Add(action);
		}
		dirty = true;
	}

	public bool knowsTarget(Label target) {
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

	public Dictionary<string, Goal> getGoals() {
		return goals;
	}

	private void evaluateActions() {
		dirty = false;
		PriorityQueue endeavourQueue = new PriorityQueue ();
		foreach (Endeavour action in currentEndeavours) {
			if (action != null) {
				if (action.isStale ()) {
					//print("stop executing: " + action.getName());
					action.stopExecution ();
					staleEndeavours.Add (action);	
				} else {
					//print("enqueue: " + action.getName());
					
					endeavourQueue.Enqueue (action);
				}
			}
			
		}
		foreach (Endeavour action in availableEndeavours) {
			if (!action.isStale()) {
				endeavourQueue.Enqueue(action);
			}
			else {
				//print("mark as stale: " + action.getName());
				
				staleEndeavours.Add(action);
			}
		}
		
		foreach (Endeavour action in staleEndeavours) {
			//print ("removing " + action.getName());
			availableEndeavours.Remove(action);
			currentEndeavours.Remove(action);
		}
		staleEndeavours.Clear ();
		HashSet<Endeavour> proposedEndeavours = new HashSet<Endeavour> ();
		
		//if (currentAction != (Action)actionQueue.peek()) {
		//	if (currentAction != null) {
		//		currentAction.stopExecution();
		//		currentAction = null;
		//	}
		//}
		Dictionary<System.Type, int> componentMap = getComponentMap ();
		while (endeavourQueue.Count > 0) {
			if (((Endeavour)endeavourQueue.peek()).canExecute(componentMap)) {
				Endeavour action = (Endeavour)endeavourQueue.Dequeue();
				//print("propose: " + action.getName());
				proposedEndeavours.Add(action);
				//action.execute();
			}
			else {
				endeavourQueue.Dequeue();
			}
		}
		
		List<Endeavour> toExecute = new List<Endeavour> ();
		foreach (Endeavour action in proposedEndeavours) {
			if (currentEndeavours.Contains(action)) {
				currentEndeavours.Remove(action);
				
			}
			else {
				toExecute.Add(action);
			}
		}
		
		foreach (Endeavour action in currentEndeavours) {
			//print("stop executing: " + action.getName());
			action.stopExecution();
		}
		
		foreach (Endeavour action in toExecute) {
			//print("start executing: " + action.getName());
			
			action.execute();
		}
		currentEndeavours = proposedEndeavours;
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

	private void sightingLost(Label target) {

		if (externalMentalModel != null) {
			externalMentalModel.removeSighting(target);
		}
		mentalModel.removeSighting (target);
	}

	private void sightingFound(Label target) {
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