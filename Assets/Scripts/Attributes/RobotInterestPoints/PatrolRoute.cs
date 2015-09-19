using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Scripts/Labels/Patrol Route")]
public class PatrolRoute : Label {

	public new EndeavourFactory[] endeavours = new EndeavourFactory[1] {new Patrol()};

}
