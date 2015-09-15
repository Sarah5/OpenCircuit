using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Patrol : EndeavourFactory {

	[System.NonSerialized]
	public List<Label> points = null;
	//private List<string> pointsPaths;
	private string[] pointsPaths = new string[0];
	private bool status = false;
	private int size = 0;

	public override Endeavour constructEndeavour (RobotController controller) {
		if (getPoints() == null) {
			Debug.Log ("points null");
		}
		if (parent == null || getPoints() == null) {
			return null;
		}
		//Goal[] goals = new Goal[1];
		//goals [0] = new Goal ("protection", 1);
		return new PatrolAction(controller, goals, getPoints());
	}

	public List<Label> getPoints() {
		if (points == null) {
			points = new List<Label>();
			if (pointsPaths.Length > 0) {
				foreach (string path in pointsPaths) {
					if (path != null) {
						points.Add (GameObjectUtil.GetGameObject<Label> (path));
					}
				}
			}
		}
		return points;
 	}

	public override void doGUI() {
		base.doGUI ();
		status = UnityEditor.EditorGUILayout.Foldout (status, "Points");

		if (status && getPoints() != null) {
			//UnityEditor.EditorGUIUtility.LookLikeControls();
			size = UnityEditor.EditorGUILayout.IntField("Size:", getPoints().Count);
			if(size < getPoints().Count) {
				getPoints().RemoveRange(size, getPoints().Count - size);
				string [] temp = new string[size];
				Array.Copy(pointsPaths, 0, temp, 0, size);
				pointsPaths = temp;
				//pointsPaths.RemoveRange(size, getPoints().Count - size);
			}

			if(size > getPoints().Count) {
				string[] temp = new string[size];
				if(pointsPaths.Length != 0) {
					Array.Copy(pointsPaths, 0, temp, 0, size);
				}
				for(int i = pointsPaths.Length; i < size; i++) {
					temp[i] = "";
				}
				pointsPaths = temp;
			}
			while(size > getPoints().Count) {
				getPoints().Add(null);

			}


			for (int i = 0; i < points.Count; i++) {
				getPoints()[i] = (Label)UnityEditor.EditorGUILayout.ObjectField(points[i], typeof(Label), true);
				if (getPoints()[i] != null) {
					pointsPaths[i] = GameObjectUtil.GetPath(getPoints()[i]);
					//Debug.Log(pointsPaths[i]);
				}
			}

		}
	}
}
