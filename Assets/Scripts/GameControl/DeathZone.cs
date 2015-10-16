using UnityEngine;
using System.Collections;

public class DeathZone : MonoBehaviour {

	public void OnTriggerEnter(Collider other) {
		Player player = other.gameObject.GetComponent<Player>();
		if (player == null)
			return;
		player.die();
	}
}
