using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Test/Movie Script")]
public class MovieScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		((MovieTexture)GetComponent<Renderer>().material.mainTexture).Play();
	}
}
