using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotInterest : MonoBehaviour {

	public static List<RobotInterest> interestPoints = new List<RobotInterest>();

	// Use this for initialization
	void Start () {
		interestPoints.Add (this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
