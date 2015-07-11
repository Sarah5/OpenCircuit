using UnityEngine;
using System.Collections;

public class BoxOfDeath : MonoBehaviour {

	void OnTriggerEnter(Collider collision) {
		Player player = collision.gameObject.GetComponent<Player> ();
		if (player != null)
			UnityEditor.EditorApplication.isPlaying = false;
	}
		
}
