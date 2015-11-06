using UnityEngine;

public class RobotMessage {

	public enum MessageType {
		TARGET_SIGHTED, TARGET_LOST, ACTION
	}

	string message;
	MessageType type;
	LabelHandle target;
	Vector3 targetPos;

	public RobotMessage(MessageType type, string message, LabelHandle target, Vector3 targetPos) {
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

	public MessageType Type {
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
