using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;


[CustomEditor(typeof(Label), true)]
public class LabelGUI : Editor {
	
	private SerializedObject ob;

	private bool operationsExpanded = true;
	private bool endeavoursExpanded = true;
	private string[] operationTypeNames;
	
	public void OnEnable() {
		ob = new SerializedObject(target);
		operationTypeNames = new string[Operation.types.Length];
		for(int i=0; i<operationTypeNames.Length; ++i) {
			operationTypeNames[i] = Operation.types[i].FullName;
		}
	}
	
	public override void OnInspectorGUI() {
		Label label = (Label)target;
		label.isVisible = EditorGUILayout.Toggle ("Visible", label.isVisible);
		label.threatLevel = EditorGUILayout.FloatField ("Threat Level", label.threatLevel);
		doOperationList(label);
		doEndeavourList(label);
		
		// finally, apply the changes
		ob.ApplyModifiedProperties();
	}

	public void doOperationList(Label label) {
		operationsExpanded = EditorGUILayout.Foldout (operationsExpanded, "Operations");
		if (!operationsExpanded)
			return;
		
		for(int i=0; i<label.operations.Length; ++i)
			if (label.operations[i] == null)
				label.operations[i] = Operation.constructDefault();
		doArrayGUI(ref label.operations);
	}
	
	public void doEndeavourList(Label label) {
		endeavoursExpanded = EditorGUILayout.Foldout(endeavoursExpanded, "Endeavours");
		if (!endeavoursExpanded) {
			return;
		}

		for (int i=0; i<label.endeavours.Length; ++i) {
			if (label.endeavours [i] == null) {
				label.endeavours [i] = EndeavourFactory.constructDefault (label);
			}
		}
		doArrayGUI(ref label.endeavours);
	}

	private void doArrayGUI<T>(ref T[] array) where T:InspectorListElement {
		GUILayout.BeginHorizontal();
		int newSize = Math.Max(EditorGUILayout.IntField("Count", array.Length), 0);
		if (newSize != array.Length) {
			array = resize(array, newSize);
			return;
		}
		GUILayout.EndHorizontal();
		
		// draw list
		for(int i=0; i<array.Length; ++i) {
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
			GUILayout.BeginHorizontal();
			
			// draw element controls
			GUILayout.BeginVertical();
			if (GUILayout.Button("X", GUILayout.MaxWidth(20), GUILayout.MaxHeight(16))) {
				array = remove(array, i);
				--i;
				continue;
			}
			if (i < array.Length -1 && GUILayout.Button("V", GUILayout.MaxWidth(20), GUILayout.MaxHeight(16))) {
				moveDown(ref array, i);
				--i;
				continue;
			}
			GUILayout.EndVertical();

			// draw element
			GUILayout.BeginVertical();
			array[i] = (T) array[i].doListElementGUI();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
		
		// draw add button
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Add")) {
			array = resize(array, array.Length +1);
		}
		GUILayout.EndHorizontal();
		
	}

	private T[] resize<T>(T[] array, int newSize) {
		if (newSize == array.Length)
			return array;
		T[] newArray = new T[newSize];
		Array.Copy(array, newArray, Math.Min(newSize, array.Length));
		return newArray;
	}

	private T[] remove<T>(T[] array, int indexToRemove) {
		T[] newArray = new T[array.Length -1];
		Array.Copy(array, newArray, indexToRemove);
		Array.Copy(array, indexToRemove +1, newArray, indexToRemove, newArray.Length -indexToRemove);
		return newArray;
	}

	private void moveDown<T>(ref T[] array, int index) {
		T temp = array[index];
		array[index] = array[index +1];
		array[index +1] = temp;
	}
}
