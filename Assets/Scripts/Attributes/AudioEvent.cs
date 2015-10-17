using UnityEngine;
using System.Collections;

public class AudioEvent {

    private Vector3 source;
    private Tag descriptor;

    public AudioEvent(Vector3 source, Tag descriptor) {
        this.source = source;
        this.descriptor = descriptor;
    }

    public Tag getTag() {
        return descriptor;
    }

    public Vector3 getSourcePosition() {
        return source;
    }
}
