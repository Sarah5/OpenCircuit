using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Label : MonoBehaviour, ISerializationCallbackReceiver {
	
	[System.NonSerialized]
	public static readonly HashSet<Label> labels = new HashSet<Label>();

	[System.NonSerialized]
	public static readonly HashSet<Label> visibleLabels = new HashSet<Label>();

	public byte[] serializedData;

	[System.NonSerialized]
	public Endeavour[] endeavours = new Endeavour[0];
	[System.NonSerialized]
	public Operation[] operations = new Operation[0];

	[System.NonSerialized]
	private Dictionary<System.Type, List<Operation>> triggers = new Dictionary<System.Type, List<Operation>>();

	[System.NonSerialized]
	public string Type = "";

	public void Awake() {
		Label.labels.Add(this);
		if (isVisible()) {
			visibleLabels.Add(this);
		}
		triggers = new Dictionary<System.Type, List<Operation>>();
		foreach(Operation op in operations) {
			addOperation(op, op.getTriggers());
		}
	}

	public bool hasOperationType(System.Type type) {
		return triggers.ContainsKey(type);
	}

	public bool sendTrigger(GameObject instigator, Trigger trig) {
        System.Type type = trig.GetType();
		if (triggers.ContainsKey(type)) {
			foreach (Operation e in (List<Operation>)triggers[type]) {
                e.perform(instigator, trig);
            }
			return true;
		}
		return false;
	}

	public void addOperation(Operation operation, System.Type[] triggerTypes) {
        if (operation != null) {
			operation.setParent(this);
            foreach (System.Type element in triggerTypes) {
				if (!triggers.ContainsKey(element)) {
					triggers[element] = new List<Operation>();
                }
				((List<Operation>)triggers[element]).Add(operation);
            }
		}
	}

	public virtual List<Endeavour> getAvailableEndeavours (RobotController controller) {
		List<Endeavour> endeavour = new List<Endeavour> ();
		return endeavour;
	}

	virtual protected bool isVisible() {
		return true;
	}
	
	public void OnDestroy() {
		Label.labels.Remove(this);
	}

	public void OnBeforeSerialize() {
		lock(this) {
			if (operations == null)
				operations = new Operation[0];
			MemoryStream stream = new MemoryStream();
			BinaryFormatter formatter = new BinaryFormatter();

			formatter.Serialize(stream, operations);
			formatter.Serialize(stream, endeavours);

			serializedData = stream.ToArray();
			stream.Close();
		}
	}
	
	public void OnAfterDeserialize() {
		lock(this) {
			MemoryStream stream = new MemoryStream(serializedData);
			BinaryFormatter formatter = new BinaryFormatter();
			
			operations = (Operation[]) formatter.Deserialize(stream);
			endeavours = (Endeavour[]) formatter.Deserialize(stream);
			stream.Close();
		}
	}
}