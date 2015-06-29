using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotController : MonoBehaviour {

	Queue<RobotMessage> messageQueue = new Queue<RobotMessage>();

	public void enqueueMessage(RobotMessage message) {
		messageQueue.Enqueue (message);
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
