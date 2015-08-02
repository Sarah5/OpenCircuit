using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PriorityQueue {
	
	List<Prioritizable> list = new List<Prioritizable>();

	public void Enqueue (Prioritizable p) {
		if (p == null) {
			throw new NullReferenceException("PriorityQueue does not accept null objects");
		}
		float min = 0;
		if (list.Count == 0) {
				list.Add(p);
		}
		else {
			min = p.getPriority();
			for (int j = 0; j < list.Count; j++) {
				float dist = list[j].getPriority();
				if (dist < min) {
						list.Insert(j, p);
						break;
				}
				else if (j == list.Count - 1) {
					list.Add(p);
						break;
				}
			}
		}
	}

	public Prioritizable Dequeue() {
		Prioritizable p = list[0];
		list.RemoveAt(0);
		return p;
	}

	public Prioritizable peek() {
		Prioritizable result = null;
		if (list.Count > 0) {
			result = list[0];
		}
		return result;
	}

	public int Count { get {
		return list.Count;
	} }

}
