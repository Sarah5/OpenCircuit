
public class EventMessage {

	private string type;
	private Label targetInfo;

	public EventMessage(string type, Label target) {
		this.type = type;
		this.targetInfo = target;
	}

	public string Type {
		get {
			return type;
		}
	}

	public Label Target {
		get {
			return targetInfo;
		}
	}
}