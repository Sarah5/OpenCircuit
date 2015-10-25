using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Scripts/Robot/Robo Eyes")]
public class RoboEyes : AbstractRobotComponent {

	public float fieldOfViewAngle = 170f;           // Number of degrees, centered on forward, for the enemy sight.
	public float sightDistance = 30.0f;
	  int size; //Total number of points in circle
	  float theta_scale = 0.01f;        //Set lower to add more points



	private Dictionary<Label, bool> targetMap = new Dictionary<Label, bool>();
	private List<GameObject> lines = new List<GameObject>();
	  LineRenderer lineRenderer;


	// Use this for initialization
	void Start () {
		float sizeValue = 2f*Mathf.PI / theta_scale; 
    size = (int)sizeValue;
    size++;
	lineRenderer = roboController.gameObject.AddComponent<LineRenderer>();
    lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
    lineRenderer.SetWidth(0.02f, 0.02f); //thickness of line
	lineRenderer.SetVertexCount(size);
		InvokeRepeating ("lookAround", 0.5f, .03f);
	}

	void Update() {
		clearCircle();
		if(roboController.debug) {
			lineRenderer.SetVertexCount(size);
			drawCircle();
		}
	}

	private bool canSee (Transform obj) {
		Vector3 objPos = obj.position;
		bool result = false;
		if (Vector3.Distance (objPos, transform.position) < sightDistance) {
			RaycastHit hit;
			Vector3 dir = objPos - transform.position;
			dir.Normalize();
			float angle = Vector3.Angle(dir, transform.forward);
//			print (roboController.gameObject.name);
//			print (angle);
			if(angle < fieldOfViewAngle * 0.5f) {
				Physics.Raycast (transform.position, dir, out hit, sightDistance);

				if (hit.transform == obj ) {//&& Vector3.Dot (transform.forward.normalized, (objPos - transform.position).normalized) > 0) {
					result = true;
					if(roboController.debug)
						drawLine(transform.position, hit.point, Color.green);
				} else {
					if(roboController.debug)
						drawLine(transform.position, hit.point, Color.red);
					//print("lost: " + obj.gameObject.name + "obscured by: " + hit.transform.gameObject.name);
				}
			}
		}
		return result;
	}

	private void clearLines() {
		foreach(GameObject line in lines) {
			Destroy(line);
		}
		lines.Clear();
	}

	private void lookAround() {
		clearLines();
		bool hasPower = powerSource != null && powerSource.hasPower(Time.deltaTime);
		foreach (Label label in Label.visibleLabels) {
			bool targetInView = hasPower && canSee (label.transform);
			if ((!targetMap.ContainsKey (label) || !targetMap [label]) && targetInView) {
				//print("target sighted: " + label.name);
				roboController.enqueueMessage(new RobotMessage("target sighted", "target sighted", label));

			}
			else if (targetMap.ContainsKey(label) && targetMap [label] && !targetInView) {
				//print("target lost: " + label.name);
				roboController.enqueueMessage(new RobotMessage("target lost", "target lost", label));
			}
			targetMap [label] = targetInView;
		}
	}

	private void clearCircle() {
		lineRenderer.SetVertexCount(0);
	}

	private void drawCircle() {
		Vector3 pos;
		float theta = 0f;
		float radius = sightDistance;

		for(int i = 0; i < size; i++){
			theta += (theta_scale);         
		  float x = radius * Mathf.Cos(theta);
		  float	z = radius * Mathf.Sin(theta);          
		  x += gameObject.transform.position.x;
		  z += gameObject.transform.position.z;
		  pos = new Vector3(x, transform.position.y, z);
		  lineRenderer.SetPosition(i, pos);
		}
	}

	private void drawLine(Vector3 start, Vector3 end, Color color) {
		LineRenderer line = new GameObject("Line ").AddComponent<LineRenderer>();
		line.SetWidth(0.025F, 0.025F);
		line.SetColors(color, color);
		line.SetVertexCount(2);
		line.SetPosition(0, start);
		line.SetPosition(1, end);
		line.material.shader = (Shader.Find("Unlit/Color"));
		line.material.color = color;
		lines.Add(line.gameObject);
	}
}
