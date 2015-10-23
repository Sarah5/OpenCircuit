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

	void OnSceneGUI() {
		if(!Application.isPlaying)
			return;
		Handles.BeginGUI();
		serializedObject.Update();
		RobotController robot = (RobotController)target;

		string buffer = "";
		for(int i = 0; i < robot.lines.Count; i++) {
			buffer += robot.lines[i].Trim() + "\n";
		}

		Vector3 pos;
		pos = robot.transform.position;

		Font font = UnityEditor.AssetDatabase.LoadAssetAtPath<Font>("Assets/GUI/Courier.ttf");
		GUIStyle debugStyle = new GUIStyle(GUI.skin.textArea);
		debugStyle.font = font;
		debugStyle.fontSize = 14;
		GUIStyle debugLabelStyle = new GUIStyle(GUI.skin.label);
		debugLabelStyle.font = font;
		debugLabelStyle.fontSize = 14;
		Vector2 size = debugStyle.CalcSize(new GUIContent(buffer));
		size.y -= debugStyle.lineHeight;
		Rect rectangle = new Rect(5, 20, size.x, size.y);


		GUILayout.Window(2, rectangle, (id) => {

			GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(size.x)};
			GUILayout.TextArea(buffer, debugStyle, options);

			Battery battery = robot.GetComponentInChildren<Battery>();
			if(battery != null) {

				battery.currentCapacity = GUILayout.HorizontalSlider(battery.currentCapacity, 0, battery.maximumCapacity, options);
				GUILayout.BeginHorizontal(options);
				GUILayout.Label("0", debugLabelStyle);
				GUILayout.Label(battery.currentCapacity + "", debugLabelStyle);
				GUILayout.Label(battery.maximumCapacity + "", debugLabelStyle);
				GUILayout.EndHorizontal();
			}


		}, robot.name);

		Handles.EndGUI();
	}

	public override void OnInspectorGUI() {
        serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("debug"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("reliability"));
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
