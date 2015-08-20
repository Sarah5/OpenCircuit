using UnityEngine;
using System.Collections;

public class GameObjectUtil {
	public static string GetPath (Label gameObject) { 
		string path = "/" + gameObject.name;
		Transform transform = gameObject.transform;
		
		while (transform.parent != null)
		{
			transform = transform.parent;
			path = "/" + transform.gameObject.name + path;
		}
		
		return path;
	}

	public static T GetGameObject<T>(string path) where T : MonoBehaviour {

		T test = GameObject.Find (path).GetComponent<T> ();
		if (test == null) {
			Debug.Log("null object");
		}
		return test;
		//return (Label)GameObject.Find (path);
	}
}
