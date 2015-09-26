using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ObjectReferenceManager : MonoBehaviour {

	[SerializeField]
	public ReferenceDictionary references;

	//[SerializeField]
	[System.NonSerialized]
	private static ObjectReferenceManager instance;

	public ObjectReferenceManager() {
		if (instance == null) {
			instance = this;
		}
		if (references == null)
			references = new ReferenceDictionary();
	}

	public void Start() {
		if (instance != null && instance != this) {
			GameObject.DestroyImmediate(gameObject);
			Debug.LogError("Cleaned up ref manager!  Go yell at semimono: this should never happen, and it's his fault!");
		}
		gameObject.hideFlags = 0;
	}

	public static ObjectReferenceManager get() {
		if (instance == null) {
			instance = new GameObject("ObjectReferenceManager").AddComponent<ObjectReferenceManager>();
			instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
		}
		return instance;
	}

	public string addReference(Object obReference) {
		if (obReference == null)
			return null;
		string id = System.Guid.NewGuid().ToString();
		references[id] = obReference;
		return id;
	}

	public void updateReference(string id, Object obReference) {
		if (references.ContainsKey(id))
			references[id] = obReference;
		else
			Debug.LogError("Cannot update object reference with non-existant ID: " +id);
	}

	public bool deleteReference(string refId) {
		if (refId == null) return false;
		return references.Remove(refId);
	}

	public T fetchReference<T>(string refId) where T: Object {
		if (refId == null)
			return null;
		Object ob = null;
		if (!references.TryGetValue(refId, out ob)) {
			Debug.LogError("Broken object reference");
		}
		return (T) ob;
	}

	[System.Serializable]
	public class ReferenceDictionary: SerializableDictionary<string, UnityEngine.Object> {}
}
