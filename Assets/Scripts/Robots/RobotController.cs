using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Scripts/Robot/Robot Controller")]
public class RobotController : MonoBehaviour {

	private HashSet<Endeavour> availableEndeavours = new HashSet<Endeavour> (new EndeavourComparer());
	private List<Label> trackedTargets = new List<Label> ();
    
	public Label[] locations;
    public Goal[] goals;
	public Dictionary<GoalEnum, Goal> goalMap = new Dictionary<GoalEnum, Goal>();

	private HashSet<Endeavour> currentEndeavours = new HashSet<Endeavour>();
	private List<Endeavour> staleEndeavours = new List<Endeavour> ();
	private Dictionary<System.Type, AbstractRobotComponent> componentMap = new Dictionary<System.Type, AbstractRobotComponent> ();

	MentalModel mentalModel = new MentalModel ();
	MentalModel externalMentalModel = null;
	
	Queue<RobotMessage> messageQueue = new Queue<RobotMessage>();

	private bool dirty = false;

	void Start() {
        /*
		goalMap.Add("protection", new Goal("protection", 1));
		goalMap.Add("offense", new Goal("offense", 1));
		goalMap.Add ("self-preservation", new Goal ("self-preservation", 1));
        */
        foreach(Goal goal in goals) {
            if(!goalMap.ContainsKey(goal.type)) {
                goalMap.Add(goal.type, goal);
            }
        }
		AbstractRobotComponent [] compenents = GetComponentsInChildren<AbstractRobotComponent> ();

		foreach (AbstractRobotComponent component in compenents) {
			componentMap[component.GetType()] = component;
		}

		foreach (Label location in locations) {
			if (location == null) {
				Debug.LogWarning("Null location attached to AI with name: " + gameObject.name);
				continue;
			}
			mentalModel.addSighting(location);
			trackTarget(location);
		}
		InvokeRepeating ("evaluateActions", .1f, .2f);
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

	public Dictionary<GoalEnum, Goal> getGoals() {
		return goalMap;
	}

	public Dictionary<System.Type, AbstractRobotComponent> getComponentMap() {
		return componentMap;
	}

	public List<Label> getTrackedTargets() {
		return trackedTargets;
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
		Dictionary<System.Type, int> componentMap = getComponentUsageMap ();
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

	private Dictionary<System.Type, int> getComponentUsageMap() {
		Dictionary<System.Type, int> componentUsageMap = new Dictionary<System.Type, int>();
		foreach (AbstractRobotComponent component in componentMap.Values) {
			if (componentUsageMap.ContainsKey(component.GetType())) {
				int count = componentUsageMap[component.GetType()];
				++count;
				componentUsageMap[component.GetType()] = count;
			}
			else {
				componentUsageMap[component.GetType()] = 1;
			}
		}
		return componentUsageMap;
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