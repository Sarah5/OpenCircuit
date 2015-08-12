using UnityEngine;
using System.Collections;

public class MovieScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		((MovieTexture)GetComponent<Renderer>().material.mainTexture).Play();
	}
}
