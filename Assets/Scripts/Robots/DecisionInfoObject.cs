using UnityEngine;
using System.Collections;

public class DecisionInfoObject {
	private string name;
	private float priority;
	private bool chosen;


	public DecisionInfoObject(string name, float priority, bool chosen) {
		this.name = name;
		this.priority = priority;
		this.chosen = chosen;
	}

	public string getTitle() {
		return name;
	}

	public float getPriority() {
		return priority;
	}

	public bool isChosen() {
		return chosen;
	}
}

