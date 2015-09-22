using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(RobotController))]
public class RobotControllerGUI : Editor {

    private bool status = true;
    private int size = 0;

    void onEnable() {
        size = this.serializedObject.FindProperty("goals").arraySize;
    }

	public override void OnInspectorGUI() {
        serializedObject.Update();
        SerializedProperty goals = serializedObject.FindProperty("goals");
        status = UnityEditor.EditorGUILayout.Foldout(status, "Goals");
		if(status && goals != null) {

			int newSize = UnityEditor.EditorGUILayout.IntField("Size:", goals.arraySize);
			if(newSize != size) {
				if(newSize < goals.arraySize) {
					while(newSize < goals.arraySize) {
						goals.DeleteArrayElementAtIndex(goals.arraySize - 1);
					}
				} else if(newSize > goals.arraySize) {
					while(newSize > goals.arraySize) {
						goals.InsertArrayElementAtIndex(goals.arraySize);
					}
				}
				size = newSize;
			}
				EditorGUILayout.Separator();

				for(int i = 0; i < goals.arraySize; i++) {
					EditorGUILayout.PropertyField(goals.GetArrayElementAtIndex(i).FindPropertyRelative("type"));
					EditorGUILayout.PropertyField(goals.GetArrayElementAtIndex(i).FindPropertyRelative("priority"));
					EditorGUILayout.Separator();
				}
		}
        EditorGUILayout.PropertyField(serializedObject.FindProperty("locations"), true);
        serializedObject.ApplyModifiedProperties();
	}
}
