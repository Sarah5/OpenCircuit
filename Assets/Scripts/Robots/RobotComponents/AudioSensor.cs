using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioSensor : AbstractRobotComponent {

	private bool hasPower;

    public static List<AudioSensor> sensors = new List<AudioSensor>();

	public void processAudioEvent(AudioEvent eventMessage) {
		if(hasPower) {
            Debug.Log("Sound heard!");
		}
	}

	// Use this for initialization
	void Start () {
        sensors.Add(this);
	}
	
	// Update is called once per frame
	void Update () {
		hasPower = powerSource != null && powerSource.hasPower(powerDrawRate * Time.deltaTime);
	}
}
