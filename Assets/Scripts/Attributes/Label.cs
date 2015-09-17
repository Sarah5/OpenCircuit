	using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[AddComponentMenu("Scripts/Labels/Label")]
public class Label : MonoBehaviour, ISerializationCallbackReceiver {
	
	[System.NonSerialized]
	public static readonly HashSet<Label> labels = new HashSet<Label>();

	[System.NonSerialized]
	public static readonly HashSet<Label> visibleLabels = new HashSet<Label>();

	[SerializeField]
	public byte[] serializedData;

	[System.NonSerialized]
	public EndeavourFactory[] endeavours = new EndeavourFactory[0];
	[System.NonSerialized]
	public Operation[] operations = new Operation[0];

	[System.NonSerialized]
	private Dictionary<System.Type, List<Operation>> triggers = new Dictionary<System.Type, List<Operation>>();

	//Properties handled by Unity serialization
	public bool isVisible = true;
	public float threatLevel = 0;

	public void Awake() {
		Label.labels.Add(this);
		if (isVisible) {
			visibleLabels.Add(this);
		}
		triggers = new Dictionary<System.Type, List<Operation>>();
		foreach(Operation op in operations) {
			addOperation(op, op.getTriggers());
		}

		foreach (EndeavourFactory endeavour in endeavours) {
			endeavour.setParent(this);
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
		List<Endeavour> availableEndeavours = new List<Endeavour> ();
		foreach (EndeavourFactory endeavour in endeavours) {
			availableEndeavours.Add(endeavour.constructEndeavour(controller));
		}
		return availableEndeavours;
	}

	public void OnDestroy() {
		Label.labels.Remove(this);
	}

	public void OnBeforeSerialize() {
		lock(this) {
			//if (operations == null)
			//	operations = new Operation[0];
			//if(endeavours == null)
			//	endeavours = new EndeavourFactory[0];
			//print("howdy bang bang!");
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
			endeavours = (EndeavourFactory[]) formatter.Deserialize(stream);

			foreach (EndeavourFactory factory in endeavours) {
				if(factory != null) {
					factory.setParent(this);
					if(factory.goals == null) {
						factory.goals = new List<Goal>();
					}
				}
			}
			stream.Close();
		}
	}
}