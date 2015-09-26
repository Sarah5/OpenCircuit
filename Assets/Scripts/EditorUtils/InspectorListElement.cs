using UnityEngine;
using System.Collections;

public interface InspectorListElement {

#if UNITY_EDITOR
	InspectorListElement doListElementGUI();
#endif

}
