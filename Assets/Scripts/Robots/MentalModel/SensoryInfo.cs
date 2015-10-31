using UnityEngine;
using System.Collections;

public class SensoryInfo {

	private int sensorCount;
	private Vector3 position;

	public SensoryInfo(Vector3 position, int sightingCount) {
		this.sensorCount = sightingCount;
		this.position = position;
	}

	public void updatePosition(Vector3 pos) {
		this.position = pos;
	}

	public void addSighting() {
		++sensorCount;
	}

	public void removeSighting() {
		--sensorCount;
	}

	public int getSightings() {
		return sensorCount;
	}

	public Vector3 getPosition() {
		return position;
	}
}
