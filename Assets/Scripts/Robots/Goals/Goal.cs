using UnityEngine;
using System.Collections;

[System.Serializable]
public class Goal {

	public GoalEnum type;
	public float priority = 1;

	public Goal(GoalEnum type, float priority) {
		this.type = type;
		this.priority = priority;
	}
}
