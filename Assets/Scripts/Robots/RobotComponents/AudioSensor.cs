using UnityEngine;
using System.Collections;

public class AudioSensor : AbstractRobotComponent, AudioEventListener {

	private bool hasPower;

	void AudioEventListener.processAudioEvent(AudioEvent eventMessage) {
		if(hasPower) {

		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		hasPower = powerSource.drawPower(powerDrawRate * Time.deltaTime);
	}
}
