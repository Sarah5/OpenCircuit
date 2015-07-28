using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue {
	
	List<Prioritizable> list = new List<Prioritizable>();

	public void Enqueue (Prioritizable p) {
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
		return list [0];
	}

	public int Count { get {
		return list.Count;
	} }

}
