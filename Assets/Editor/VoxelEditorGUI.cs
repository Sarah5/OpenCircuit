using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;


//[System.Serializable]
//public class VoxelEditorGUI {

//	//int size = 256;
//	//int power = 8;
//	string name;
//	int amount = 1;
//	int unit;

//}


[CustomEditor(typeof(Vox.VoxelEditor))]
public class VoxelEditorGUI : Editor {

	protected const string numForm = "##,0.000";
	protected const string numFormInt = "##,#";
	protected readonly GUIContent[] modes = {new GUIContent("Manage"), new GUIContent("Sculpt"), new GUIContent("Masks")};

	private SerializedObject ob;
	//private bool materialsFoldout = false;
	//private List<Vox.VoxelMaterial> voxelMaterials;
	//	public Vector2 scrollPos = new Vector2(0, 0);
	
	[MenuItem("GameObject/3D Object/Voxel Object")]
	public static void createVoxelObject() {
		GameObject ob = new GameObject();
		ob.name = "Voxel Object";
		ob.AddComponent<Vox.VoxelEditor>();
	}
	
	public void OnEnable() {
		ob = new SerializedObject(target);
	}

	public override void OnInspectorGUI() {
		ob.Update();
		Vox.VoxelEditor editor = (Vox.VoxelEditor)target;

		editor.selectedMode = GUILayout.Toolbar(editor.selectedMode, modes, GUILayout.MinHeight(20));

//		scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MinHeight(400));

		switch (editor.selectedMode) {
		case 0:
			doGeneralGUI ();
			break;
		case 1:
			doSculptGUI();
			break;
		case 2:
			doMaskGUI();
			break;
		}

//		GUILayout.EndScrollView();
		
		// finally, apply the changes
		ob.ApplyModifiedProperties();
	}

	protected void doMaskGUI() {
		// mask list
		SerializedProperty voxelMasks = ob.FindProperty("masks");
		EditorGUILayout.PropertyField(voxelMasks, new GUIContent("Sculpting Masks"), true);
	}

	protected void doSculptGUI() {
		Vox.VoxelEditor editor = (Vox.VoxelEditor)target;

		// brush size
		GUILayout.BeginHorizontal();
		GUILayout.Label("Brush Radius", GUILayout.ExpandWidth(false));
		editor.brushSize = GUILayout.HorizontalSlider(editor.brushSize, 0, 100);
		editor.brushSize = EditorGUILayout.FloatField(editor.brushSize, GUILayout.MaxWidth(64));
		if (editor.brushSize < 0)
			editor.brushSize = 0;
		GUILayout.EndHorizontal();

		// brush material type
		string[] materials = new string[editor.voxelMaterials.Length];
		for(int i=0; i<materials.Length; ++i)
			materials[i] = editor.voxelMaterials[i].name;
		editor.selectedBrushMaterial = (byte)GUILayout.SelectionGrid(editor.selectedBrushMaterial, materials, 1);
	}

	protected void doGeneralGUI() {
		
		// world detail
		SerializedProperty maxDetail = ob.FindProperty("maxDetail");
		EditorGUILayout.PropertyField(maxDetail, new GUIContent("Voxel Power"));
		if (maxDetail.intValue > byte.MaxValue)
			maxDetail.intValue = byte.MaxValue;
		else if (maxDetail.intValue < 4)
			maxDetail.intValue = 4;

		long dimension = 1 << maxDetail.intValue;
		++EditorGUI.indentLevel;
		EditorGUILayout.LabelField("Voxels Per Side", dimension.ToString(numFormInt));
		EditorGUILayout.LabelField("Max Voxel Count", Mathf.Pow(dimension, 3).ToString(numFormInt));
		--EditorGUI.indentLevel;
		EditorGUILayout.Separator();

		// world dimension
		SerializedProperty baseSize = ob.FindProperty("BaseSize");
		EditorGUILayout.PropertyField(baseSize, new GUIContent("WorldSize (m)"));
		if (baseSize.floatValue < 0)
			baseSize.floatValue = 0;
		++EditorGUI.indentLevel;
		EditorGUILayout.LabelField("World Area", Mathf.Pow(baseSize.floatValue / 1000, 2).ToString(numForm) + " square km");
		EditorGUILayout.LabelField("World Volume", Mathf.Pow(baseSize.floatValue / 1000, 3).ToString(numForm) + " cubic km");
		--EditorGUI.indentLevel;
		EditorGUILayout.Separator();

		EditorGUILayout.LabelField("Voxel Size", (baseSize.floatValue / dimension).ToString(numForm) + " m");
		EditorGUILayout.Separator();

		// LOD
		SerializedProperty useLod = ob.FindProperty("useLod");
		EditorGUILayout.PropertyField(useLod, new GUIContent("Use Level of Detail"));
		if (useLod.boolValue) {
			++EditorGUI.indentLevel;
			SerializedProperty lodDetail = ob.FindProperty("lodDetail");
			EditorGUILayout.PropertyField(lodDetail, new GUIContent("Target Level of Detail"));
			if (lodDetail.floatValue > 1000)
				lodDetail.floatValue = 1000;
			else if (lodDetail.floatValue < 0.1f)
				lodDetail.floatValue = 0.1f;
			
			SerializedProperty curLodDetail = ob.FindProperty("curLodDetail");
			if (Application.isPlaying) {
				EditorGUILayout.PropertyField(curLodDetail, new GUIContent("Current Level of Detail"));
			} else {
				EditorGUILayout.PropertyField(curLodDetail, new GUIContent("Starting Level of Detail"));
			}

			if (curLodDetail.floatValue > 1000)
				curLodDetail.floatValue = 1000;
			else if (curLodDetail.floatValue < 0.1f)
				curLodDetail.floatValue = 0.1f;
			--EditorGUI.indentLevel;
		}

		EditorGUILayout.Separator();

		// materials

		SerializedProperty voxelMaterials = ob.FindProperty("voxelMaterials");
		EditorGUILayout.PropertyField(voxelMaterials, new GUIContent("Voxel Materials"), true);

		// procedural stats
		SerializedProperty useHeightmap = ob.FindProperty("useHeightmap");
		EditorGUILayout.PropertyField(useHeightmap, new GUIContent("Use Height Map"));
		if (useHeightmap.boolValue) {
			SerializedProperty heightmaps = ob.FindProperty("heightmaps");
			EditorGUILayout.PropertyField(heightmaps, new GUIContent("Height Maps"), true);
			SerializedProperty heightmapMaterials = ob.FindProperty("heightmapMaterials");
			EditorGUILayout.PropertyField(heightmapMaterials, new GUIContent("Height Map Materials"), true);
			SerializedProperty materialMap = ob.FindProperty("materialMap");
			EditorGUILayout.PropertyField(materialMap, new GUIContent("Material Map"));
		} else {
			SerializedProperty maxChange = ob.FindProperty("maxChange");
			EditorGUILayout.PropertyField(maxChange, new GUIContent("Roughness"));
			if (maxChange.floatValue > 5)
				maxChange.floatValue = 5;
			else if (maxChange.floatValue < 0.01f)
				maxChange.floatValue = 0.01f;
		}
		SerializedProperty createColliders = ob.FindProperty("createColliders");
		EditorGUILayout.PropertyField(createColliders, new GUIContent("Generate Colliders"));
		EditorGUILayout.Separator();

		// static meshes
		SerializedProperty useStaticMeshes = ob.FindProperty("useStaticMeshes");
		EditorGUILayout.PropertyField(useStaticMeshes, new GUIContent("Use Static Meshes"));


		// generation
		Vox.VoxelEditor editor = (Vox.VoxelEditor)target;
		editor.Update();
		string generateButtonName = editor.hasGeneratedData()? "Regenerate": "Generate";
		if (GUILayout.Button(generateButtonName)) {
			if (editor.voxelMaterials.Length < 1) {
				EditorUtility.DisplayDialog("Invalid Generation Parameters", "There must be at least one voxel material defined to generate the voxel object.", "OK");
			} else if (EditorUtility.DisplayDialog(generateButtonName +" Voxels?", "Are you sure you want to generate the voxel terain from scratch?", "Yes", "No")) {
				editor.wipe();
				editor.init();
			}
		}
		if (editor.hasGeneratedData()) {
			if (GUILayout.Button("Clear")) {
				if (EditorUtility.DisplayDialog("Clear Voxels?", "Are you sure you want to clear all voxel data?", "Yes", "No")) {
					editor.wipe();
				}
			}
		}
		EditorGUILayout.LabelField("Chunk Count: " + editor.chunks.Count);
		EditorGUILayout.Separator();
	}

	public void OnSceneGUI() {
		Vox.VoxelEditor editor = (Vox.VoxelEditor)target;
		editor.Update();
		if (editor.selectedMode != 1)
			return;
		int controlId = GUIUtility.GetControlID(FocusType.Passive);
		switch(Event.current.GetTypeForControl(controlId)) {
		case EventType.MouseDown:
			if (Event.current.button == 0) {
				GUIUtility.hotControl = controlId;
				if (Event.current.shift) {
					subtractSphere(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition));
				} else {
					addSphere(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition));
				}
				Event.current.Use();
			}
			break;

		case EventType.MouseUp:
			if (Event.current.button == 0) {
				GUIUtility.hotControl = 0;
				Event.current.Use();
			}
			break;
		}
	}

	protected void addSphere(Ray mouseLocation) {
		Vox.VoxelEditor editor = (Vox.VoxelEditor)target;

		Vector3 point = getRayCollision(mouseLocation).point;

		new Vox.SphereModifier(editor, point, editor.brushSize, new Vox.Voxel(editor.selectedBrushMaterial, byte.MaxValue), true);
	}
	
	protected void subtractSphere(Ray mouseLocation) {
		Vox.VoxelEditor editor = (Vox.VoxelEditor)target;
		
		Vector3 point = getRayCollision(mouseLocation).point;
		
		new Vox.SphereDestroyer(editor, point, editor.brushSize, new Vox.Voxel(0, byte.MinValue), 1, true, true);
	}

	protected static RaycastHit getRayCollision(Ray ray) {
		RaycastHit firstHit = new RaycastHit();
		firstHit.distance = float.PositiveInfinity;
		foreach(RaycastHit hit in Physics.RaycastAll(ray)) {
			if (hit.distance < firstHit.distance) {
				firstHit = hit;
			}
		}
		return firstHit;
	}

}
