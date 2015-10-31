
public class EventMessage {

	private string type;
	private LabelHandle targetInfo;

	public EventMessage(string type, LabelHandle target) {
		this.type = type;
		this.targetInfo = target;
	}

	public string Type {
		get {
			return type;
		}
	}

	public LabelHandle Target {
		get {
			return targetInfo;
		}
	}
}