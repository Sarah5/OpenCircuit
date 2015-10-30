using UnityEngine;
using System.Collections;

public class DecisionInfoObject {
	private string name;
	private float priority;
	private bool chosen;
	private string source;


	public DecisionInfoObject(string name, string source, float priority, bool chosen) {
		this.name = name;
		this.priority = priority;
		this.chosen = chosen;
		this.source = source;
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

	public string getSource() {
		return source;
	}
}

