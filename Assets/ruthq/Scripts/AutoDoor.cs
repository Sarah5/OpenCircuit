using UnityEngine;
using System.Collections;

public class AutoDoor : MonoBehaviour {

    public float doorHeight = 3.5f;
    private Vector3 stopPosition;
    private Vector3 upPosition;
    public float speed = 1;
    private bool isMovingUp = false;

    // Use this for initialization
    void Start () {
        stopPosition = transform.position - new Vector3 (0,5,0);
        upPosition = transform.position;
        InvokeRepeating("switchDoor", 0, 8f);
	}

    void switchDoor() {
        isMovingUp = !isMovingUp;
    }
    void moveDown() {
        Vector3 stopVector = transform.position - stopPosition;
        float length = stopVector.magnitude;


        if (length > .1f){ 
            print("this is a string!!!");
            transform.position = transform.position - new Vector3(0, Time.deltaTime * speed, 0);
        }

    }

    void moveUp() {
        Vector3 upVector = transform.position - upPosition;
        float upLength = upVector.magnitude;

        if (upLength > .1f) {
            print("This is another string.");
            transform.position = transform.position + new Vector3(0, Time.deltaTime * speed, 0);
        }

    }
	// Update is called once per frame
	void Update () {

    if (isMovingUp) {
            moveUp();
        }
     else {
            moveDown();
        }

	}
}