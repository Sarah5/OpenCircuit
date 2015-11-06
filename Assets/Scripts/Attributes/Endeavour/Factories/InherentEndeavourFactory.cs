using UnityEngine;
using System.Collections;

public abstract class InherentEndeavourFactory : EndeavourFactory {

	public abstract bool isApplicable(LabelHandle labelHandle);

	public abstract Endeavour constructEndeavour(RobotController controller, LabelHandle target);

	public override Endeavour constructEndeavour(RobotController controller) {
		return null;
	}

}
