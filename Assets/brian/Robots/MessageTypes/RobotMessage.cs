public class RobotMessage {

	string message;
	string type;
	RobotInterest target;

	public RobotMessage(string type, string message, RobotInterest target) {
		this.type = type;
		this.message = message;
		this.target = target;
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

	public RobotInterest Target {
		get {
			return target;
		}
		set {
			target = value;
		}
	}
}
