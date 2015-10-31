using UnityEngine;
using System.Collections;

public class AudioEvent : RobotMessage {

    private Vector3 source;
    private Tag descriptor;
    
    public AudioEvent(Vector3 source, Tag descriptor, Vector3 sourcePos) : base("audioEvent", "sound heard", null, sourcePos) {
        this.source = source;
        this.descriptor = descriptor;
    }

    public void broadcast(float volume) {
        foreach (AudioSensor sensor in AudioSensor.sensors) {
           // Debug.Log(30f * volume);
            if (Vector3.Distance(sensor.transform.position, source) < (30f * volume)) {
                sensor.processAudioEvent(this);
            }
        }
    }

    public Tag getTag() {
        return descriptor;
    }

    public Vector3 getSourcePosition() {
        return source;
    }
}
