using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Patrol : EndeavourFactory {

	[System.NonSerialized]
	public List<Label> points = null;
	private string[] pointsPaths = new string[0];
	private bool status = false;
	private int size = 0;

	public override Endeavour constructEndeavour (RobotController controller) {
		if (parent == null || getPoints() == null || getPoints().Count == 0) {
			if(getPoints().Count == 0) {
				Debug.LogWarning("Patrol route '"+parent.name+"' has no route points");
			}
			return null;
		}
		return new PatrolAction(controller, goals, getPoints());
	}

	public List<Label> getPoints() {
		if (points == null || points.Count < getPointsPaths().Length) {
			points = new List<Label>();
			foreach (string path in getPointsPaths()) {
				if (path != null && !path.Equals("")) {
					points.Add (GameObjectUtil.GetGameObject<Label> (path));
				} else {
					points.Add(null);
				}
			}
		}
		return points;
 	}

	private string[] getPointsPaths() {
		if(pointsPaths == null) {
			pointsPaths = new string[0];
		}
		return pointsPaths;
	}

	public override void doGUI() {
		base.doGUI ();
		status = UnityEditor.EditorGUILayout.Foldout (status, "Points");

		if (status) {
			size = UnityEditor.EditorGUILayout.IntField("Size:", getPoints().Count);
			if(size < getPoints().Count) {
				getPoints().RemoveRange(size, getPoints().Count - size);
				string [] temp = new string[size];
				Array.Copy(pointsPaths, 0, temp, 0, size);
				pointsPaths = temp;
			}

			else if(size > getPoints().Count) {
				while(size > getPoints().Count) {
					getPoints().Add(null);
				}
				string[] temp = new string[size];
				if(pointsPaths.Length != 0) {
					Array.Copy(pointsPaths, 0, temp, 0, pointsPaths.Length - 1);
				}
				for(int i = pointsPaths.Length; i < size; i++) {
					temp[i] = null;
				}
				pointsPaths = temp;
			}

			for (int i = 0; i < getPoints().Count; i++) {
				getPoints()[i] = (Label)UnityEditor.EditorGUILayout.ObjectField(getPoints()[i], typeof(Label), true);
				if (getPoints()[i] != null) {
					getPointsPaths()[i] = GameObjectUtil.GetPath(getPoints()[i]);
				}
			}
		}
	}

	public override void drawGizmo() {
		Color COLOR_ONE = Color.black;
		Color COLOR_TWO = Color.green;

		for(int i = 0; i < getPoints().Count; ++i) {
			int NUM_STRIPES = 8;
			Label current = getPoints()[i];
			Label next = (i == getPoints().Count -1 ) ? getPoints()[0] : getPoints()[i+1] ;
			if(next == null || current == null) {
				return;
			}
			float LENGTH = Vector3.Distance(current.transform.position, next.transform.position);
			Vector3 dir = next.transform.position - current.transform.position;
			dir.Normalize();
			for(int j = 0; j < NUM_STRIPES * LENGTH; j = j + 2) {
				Gizmos.color = j % 2 == 0 ? COLOR_ONE : COLOR_TWO;
				Vector3 startPos = current.transform.position + (j * dir/NUM_STRIPES);				
				Vector3 endPos = startPos + dir/NUM_STRIPES;
				if(Vector3.Distance(current.transform.position, endPos) > LENGTH) {
					endPos = next.transform.position;
				}
				Gizmos.DrawLine(startPos, endPos);
				if(j % 8 == 0) {
					Handles.color = Color.white;
					Handles.ConeCap(0, startPos, Quaternion.LookRotation(dir), .15f);
				}
			}
		}
	}
}
