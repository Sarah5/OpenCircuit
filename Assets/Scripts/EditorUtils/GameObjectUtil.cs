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
		if (path == null) {
			return null;
		}
        GameObject ob = GameObject.Find(path);
        if (ob == null) {
            Debug.LogError("Broken object pointer: " + path);
            return null;
        }
        T test = ob.GetComponent<T> ();
		if (test == null) {
			Debug.LogError("Broken object pointer: " +path);
		}
		return test;
		//return (Label)GameObject.Find (path);
	}
}