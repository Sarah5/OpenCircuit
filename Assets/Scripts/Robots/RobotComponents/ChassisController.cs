using UnityEngine;
using System.Collections.Generic;

public class ChassisController : MonoBehaviour {

	public LegController[] legGroup1;
	public LegController[] legGroup2;

	public float stanceWidth = 2;
	public float stanceHeight = 3;
	public float strideLength = 1;
	public float stepHeight = 0.5f;

	public float minMoveSpeed = 0.01f;
	public int minimumFramesPerSwitch = 2;

	public AudioSource footstep;

	private Dictionary<LegController, LegInfo> legInfo = new Dictionary<LegController, LegInfo>();
	private Vector3 lastPos;
	private LegController[] plantedGroup = null;
	private LegController[] steppingGroup = null;
	private int lastSwitch = 0;

	void Update() {
		//if (velocity.sqrMagnitude < minMoveSpeed) {
			// plant all legs
		//} else {
			// select stepping group
		if (plantedGroup == null) {
			plantedGroup = legGroup1;
			steppingGroup = legGroup2;
		}

		float stepPercent = calculateStepPercent(plantedGroup);
		//print(stepPercent);
		bool isSwitching = stepPercent > 0.99f && lastSwitch >= minimumFramesPerSwitch;
		if (isSwitching) {
			LegController[] temp = plantedGroup;
			plantedGroup = steppingGroup;
			steppingGroup = temp;
			lastSwitch = 0;
		} else {
			updateSteppingGroup(steppingGroup, stepPercent);
			++lastSwitch;
		}
		updateLegs(plantedGroup, !isSwitching);
		updateLegs(steppingGroup, false);
	}

	protected float calculateStepPercent(LegController[] plantedGroup) {
		float offsetMagnitude = 0;
		foreach (LegController leg in plantedGroup) {
			LegInfo info = getLegInfo(leg);

			// maybe the following should be added back in?
			//if (info.planted)
			//	continue;

			Vector3 normalizedVelocity = info.getVelocity().normalized;
			Vector3 offset = info.foot - leg.getDefaultPos();
			//print(Vector3.Dot(offset.normalized, -normalizedVelocity));
			offsetMagnitude = Mathf.Max(offsetMagnitude, Vector3.Dot(offset /strideLength, -normalizedVelocity) +1);
		}
		return offsetMagnitude / 2;
	}

	protected void updateSteppingGroup(LegController[] steppingGroup, float stepPercent) {
		//print(stepOffset);
		foreach (LegController leg in steppingGroup) {
			LegInfo info = getLegInfo(leg);
			Vector3 stepOffset = info.getVelocity().normalized * strideLength;
			Vector3 target = Vector3.Lerp(info.getLastPlanted(), leg.getDefaultPos() +stepOffset, stepPercent);
			target.y += (1 -Mathf.Abs(stepPercent - 0.5f) *2) *stepHeight;
			Vector3 diff = (target - info.foot);
			info.foot += diff.normalized * Mathf.Min(Mathf.Max(0.5f, diff.magnitude /8), diff.magnitude);
			//info.foot = target;
		}
	}

	protected void updateLegs(LegController[] group, bool planted) {
		foreach(LegController leg in group) {
			LegInfo info = getLegInfo(leg);
			leg.setPosition(getLegInfo(leg).foot);
			info.setPlanted(planted);
			info.setLastDefault();
		}
	}

	protected LegInfo getLegInfo(LegController leg) {
		LegInfo info;
		if (!legInfo.TryGetValue(leg, out info)) {
			info = new LegInfo(this, leg);
			legInfo.Add(leg, info);
		}
		return info;
	}



	protected class LegInfo {
		public bool planted = false;
		public Vector3 foot = Vector3.zero;
		public Vector3 lastDefault = Vector3.zero;
		public Vector3 lastPlanted = Vector3.zero;
		public ChassisController chassis;
		public LegController leg;

		public LegInfo(ChassisController chassis, LegController leg) {
			this.chassis = chassis;
			this.leg = leg;
			foot = leg.getDefaultPos();
			setLastDefault();
			setLastPlanted();
		}

		public void setPlanted(bool planted) {
			if (!planted && this.planted)
				setLastPlanted();
			if (planted && !this.planted && chassis.footstep != null)
				chassis.footstep.Play(0);
			this.planted = planted;
		}

		public Vector3 getLastPlanted() {
			return leg.transform.TransformPoint(lastPlanted);
		}

		public Vector3 getVelocity() {
			return leg.getDefaultPos() -lastDefault;
		}

		public void setLastDefault() {
			lastDefault = leg.getDefaultPos();
		}

		protected void setLastPlanted() {
			lastPlanted = leg.transform.InverseTransformPoint(foot);
		}
	}
}
