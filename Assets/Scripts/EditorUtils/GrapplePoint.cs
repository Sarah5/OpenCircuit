using UnityEngine;
using System.Collections;

[AddComponentMenu("Scripts/Items/GrapplePoint")]
public class GrapplePoint : MonoBehaviour {

	protected const float DRAW_RADIUS = 0.5f;

	public void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(transform.position, DRAW_RADIUS);
		Gizmos.color = Color.white;
	}
}
