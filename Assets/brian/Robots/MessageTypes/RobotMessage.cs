public class RobotMessage {

	string source;
	string type;
	RobotInterest target;

	public RobotMessage(string src, string type, RobotInterest target) {
		this.source = src;
		this.type = type;
		this.target = target;
	}


	public string Source {
		get {
			return source;
		}
		set {
			source = value;
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
