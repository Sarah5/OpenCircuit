public class RobotMessage {

	string message;
	string type;
	Label target;

	public RobotMessage(string type, string message, Label target) {
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

	public Label Target {
		get {
			return target;
		}
		set {
			target = value;
		}
	}
}
