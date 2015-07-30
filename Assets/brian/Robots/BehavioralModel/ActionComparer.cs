using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ActionComparer : IEqualityComparer<Action> {

	public bool Equals(Action a, Action b) {
		return a.Equals (b);
	}

	public int GetHashCode(Action a) {
		int hash = 17;
		hash = hash * 31 + a.getName ().GetHashCode ();
		hash = hash * 31 + a.getController ().GetHashCode ();
		return hash;
	}
}
