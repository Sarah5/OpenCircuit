using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Patrol : EndeavourFactory {

	[System.NonSerialized]
	public List<Label> points = null;
	private List<string> pointsPaths = new List<string> ();
	private bool status = false;
	private int size = 0;

	public Patrol() {
		//points.Add (null);
//		initialize ();
	}

	public override Endeavour constructEndeavour (RobotController controller) {
		if (getPoints() == null) {
			Debug.Log ("points null");
		}
		if (parent == null || getPoints() == null) {
			return null;
		}
		return new PatrolAction(controller, getPoints());
	}
	public List<Label> getPoints() {

		if (points == null) {
			points = new List<Label>();
			if (pointsPaths.Count > 0) {
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
		status = UnityEditor.EditorGUILayout.Foldout (status, "Points");

		if (status && getPoints() != null) {
			//UnityEditor.EditorGUIUtility.LookLikeControls();
			size = UnityEditor.EditorGUILayout.IntField("Size:", getPoints().Count);
			for (int i = 0; i < points.Count; i++) {
				getPoints()[i] = (Label)UnityEditor.EditorGUILayout.ObjectField(points[i], typeof(Label), true);
				if (getPoints()[i] != null) {
					pointsPaths[i] = GameObjectUtil.GetPath(getPoints()[i]);
					//Debug.Log(pointsPaths[i]);
				}
			}
			if (size < getPoints().Count) {
				getPoints().RemoveRange (size, getPoints().Count - size);
				pointsPaths.RemoveRange(size, getPoints().Count - size);
			}
			while (size > getPoints().Count) {
				getPoints().Add(null);
				pointsPaths.Add(null);
			}
		}
	}
}
