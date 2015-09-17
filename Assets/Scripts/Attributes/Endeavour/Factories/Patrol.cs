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
		if (parent == null || getPoints() == null) {
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
}
