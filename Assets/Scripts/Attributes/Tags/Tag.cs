using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class  Tag : InspectorListElement {

	public static Tag constructDefault() {
		return new Tag((TagEnum)Enum.GetValues(typeof(TagEnum)).GetValue(0), 0);
	}

	public static readonly TagEnum[] tagEnums;

	public TagEnum type;
	public float severity;

	public Tag(TagEnum type, float severity) {
		this.type = type;
		this.severity = severity;
	}

#if UNITY_EDITOR
	InspectorListElement InspectorListElement.doListElementGUI() {
		type = (TagEnum)UnityEditor.EditorGUILayout.Popup((int)type, Enum.GetNames(typeof(TagEnum)));

		doGUI();
		return this;
	}

	private void doGUI() {
		severity = UnityEditor.EditorGUILayout.FloatField("Severity: ", severity);
	}
#endif
}