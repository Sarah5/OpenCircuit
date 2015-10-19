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

    public float reliability = 5f;

	private HashSet<Endeavour> currentEndeavours = new HashSet<Endeavour>();
	private List<Endeavour> staleEndeavours = new List<Endeavour> ();
	private Dictionary<System.Type, AbstractRobotComponent> componentMap = new Dictionary<System.Type, AbstractRobotComponent> ();

	MentalModel mentalModel = new MentalModel ();
	MentalModel externalMentalModel = null;
	
	Queue<RobotMessage> messageQueue = new Queue<RobotMessage>();

	private bool dirty = false;

	void Start() {
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
			trackedTargets.Remove(message.Target);
		}
		dirty = true;
	}
	
	public void addEndeavour(Endeavour action) {
		availableEndeavours.Add (action);
		evaluateActions ();
	}

	public void trackTarget(Label target) {
		trackedTargets.Add (target);
		foreach (Endeavour action in target.getAvailableEndeavours(this)) {
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
		//print("**************EVALUATE**************");
		dirty = false;
		PriorityQueue endeavourQueue = new PriorityQueue ();
		//print("\tCurrent Endeavours");
		foreach (Endeavour action in currentEndeavours) {
			if (action.isStale ()) {
				//print("\t\t--" + action.getName());
				action.stopExecution ();
				staleEndeavours.Add (action);	
			} else {
				//print("\t\t++" + action.getName());
				//endeavourQueue.Enqueue (action);
				availableEndeavours.Add(action);
			}
		}
		//print("\tAvailable Endeavours");
		foreach (Endeavour action in availableEndeavours) {
			if (!action.isStale()) {
				//print("\t\t++" + action.getName());
				endeavourQueue.Enqueue(action);
			}
			else {
				//print("\t\t--" + action.getName());
				staleEndeavours.Add(action);
			}
		}
		
		foreach (Endeavour action in staleEndeavours) {
			availableEndeavours.Remove(action);
			currentEndeavours.Remove(action);
		}
		staleEndeavours.Clear ();
		HashSet<Endeavour> proposedEndeavours = new HashSet<Endeavour> ();
		
		Dictionary<System.Type, int> componentMap = getComponentUsageMap ();
		//print("\tEvaluate actions");
		while (endeavourQueue.Count > 0) {
			if (((Endeavour)endeavourQueue.peek()).canExecute(componentMap)) {
				Endeavour action = (Endeavour)endeavourQueue.Dequeue();
				//print("\t\t++" + action.getName() + "->" + action.getPriority());
				proposedEndeavours.Add(action);
				availableEndeavours.Remove(action);
			}
			else {
				Endeavour action = (Endeavour)endeavourQueue.Dequeue();
				//print("\t\t--" + action.getName() + "->" + action.getPriority());
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
			action.stopExecution();
		}
		
		foreach (Endeavour action in toExecute) {
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
			externalMentalModel.addSighting(target);
		}
		mentalModel.addSighting (target);
	}

	public MentalModel getMentalModel() {
		if (externalMentalModel == null) {
			return mentalModel;
		} else {
			return externalMentalModel; 
		}
	}
}