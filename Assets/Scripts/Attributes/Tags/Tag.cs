using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;


[System.Serializable]
public class  Tag : InspectorListElement {

	public static Tag constructDefault() {
		return new Tag((TagEnum)Enum.GetValues(typeof(TagEnum)).GetValue(0), 0);
	}

	public static readonly TagEnum[] tagEnums;

	static Tag() {
		Array values = Enum.GetValues(typeof(TagEnum));
		List<TagEnum> typeList = new List<TagEnum>();
		foreach(object obj in values) {
			typeList.Add((TagEnum)obj);
		}
		tagEnums = typeList.ToArray();
	}

	public TagEnum type;
	public float severity;

	public Tag(TagEnum type, float severity) {
		this.type = type;
		this.severity = severity;
	}

	InspectorListElement InspectorListElement.doListElementGUI() {

		int selectedType = System.Array.FindIndex(tagEnums, OP => OP == type);
		int newSelectedType = EditorGUILayout.Popup(selectedType, Enum.GetNames(typeof(TagEnum)));

		if(newSelectedType != selectedType) {
			return (EndeavourFactory)EndeavourFactory.types[newSelectedType].GetConstructor(new System.Type[0]).Invoke(new object[0]);
		}

		doGUI();
		return this;
	}

	private void doGUI() {
		severity = EditorGUILayout.FloatField("Severity: ", severity);
	}
}