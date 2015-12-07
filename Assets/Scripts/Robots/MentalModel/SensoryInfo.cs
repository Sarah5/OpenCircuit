using UnityEngine;
using System.Collections;

public class SensoryInfo {

	private int sensorCount;
	private Vector3 position;
	private Vector3? direction;
	private System.DateTime lastSighting;

	public SensoryInfo(Vector3 position, Vector3? direction, System.DateTime time, int sightingCount) {
		this.sensorCount = sightingCount;
		this.position = position;
		this.direction = direction;
		this.lastSighting = time;

	}

	public void updatePosition(Vector3 pos) {
		this.position = pos;
	}

	public void updateDirection(Vector3? dir) {
		this.direction = dir;
	}

	public void updateTime(System.DateTime time) {
		this.lastSighting = time;
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

	public System.DateTime getSightingTime() {
		return lastSighting;
	}
}
