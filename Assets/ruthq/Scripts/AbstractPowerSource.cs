using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AbstractPowerSource : MonoBehaviour {

	public static List<AbstractPowerSource> powerSources = new List<AbstractPowerSource> ();

	bool isDisabled = false;
	float disableTimeRemaining = 0f;

	void Awake() {
		powerSources.Add (this);
	}

	public virtual bool drawPower (float amount) {
		return !isDisabled;
	}

	public void disable(float time) {
		isDisabled = true;
		Invoke ("enable", time);
	}

	public void enable() {
		isDisabled = false;
	}

}