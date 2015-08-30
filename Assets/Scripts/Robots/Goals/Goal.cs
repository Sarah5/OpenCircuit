using UnityEngine;
using System.Collections;

[System.Serializable]
public class Goal {

	public string name = "";
	public float priority = 1;

	public Goal(string name, float priority) {
		this.name = name;
		this.priority = priority;
	}
}
