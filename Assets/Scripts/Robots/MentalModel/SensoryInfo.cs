using UnityEngine;
using System.Collections;

public class SensoryInfo {

	private int sensorCount;
	private Vector3 position;
	private Vector3? direction;

	public SensoryInfo(Vector3 position, Vector3? direction, int sightingCount) {
		this.sensorCount = sightingCount;
		this.position = position;
		this.direction = direction;
	}

	public void updatePosition(Vector3 pos) {
		this.position = pos;
	}

	public void updateDirection(Vector3? dir) {
		this.direction = dir;
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

	public Vector3? getDirection() {
		return direction;
	}
}
