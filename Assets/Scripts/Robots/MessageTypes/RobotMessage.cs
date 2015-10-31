using UnityEngine;

public class RobotMessage {

	string message;
	string type;
	LabelHandle target;
	Vector3 targetPos;

	public RobotMessage(string type, string message, LabelHandle target, Vector3 targetPos) {
		this.type = type;
		this.message = message;
		this.target = target;
		this.targetPos = targetPos;
	}

	public Vector3 TargetPos {
		get { return targetPos; }
		set { targetPos = value; }
	}

	public string Message {
		get {
			return message;
		}
		set {
			message = value;
		}
	}

	public string Type {
		get {
			return type;
		}
		set {
			type = value;
		}
	}

	public LabelHandle Target {
		get {
			return target;
		}
		set {
			target = value;
		}
	}
}
