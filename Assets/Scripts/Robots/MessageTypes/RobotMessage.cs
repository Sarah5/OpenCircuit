using UnityEngine;

public class RobotMessage {

	string message;
	string type;
	Label target;
	Vector3 targetPos;

	public RobotMessage(string type, string message, Label target, Vector3 targetPos) {
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

	public Label Target {
		get {
			return target;
		}
		set {
			target = value;
		}
	}
}
