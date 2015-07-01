
public class EventMessage {

	private string type;
	private RobotInterest targetInfo;

	public EventMessage(string type, RobotInterest target) {
		this.type = type;
		this.targetInfo = target;
	}

	public string Type {
		get {
			return type;
		}
	}

	public RobotInterest Target {
		get {
			return targetInfo;
		}
	}
}