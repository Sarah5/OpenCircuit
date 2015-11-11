using UnityEngine;
using System.Collections;

public class AutoDoor : MonoBehaviour {

    public float doorHeight = 3.5f;
    private Vector3 stopPosition;
    private Vector3 upPosition;
    public float speed = 1;

	// Use this for initialization
	void Start () {
        stopPosition = transform.position - new Vector3 (0,5,0);
        upPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 vector = transform.position - stopPosition;
       float length = vector.magnitude;
    if(length > .1f) {
            transform.position = transform.position - new Vector3(0, Time.deltaTime * speed, 0);
        }
        if (length < .1f) {
            transform.position = transform.position + new Vector3(0, Time.deltaTime * speed, 0);
        }
	}
}