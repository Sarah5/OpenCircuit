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
	protected readonly GUIContent[] brushes = {new GUIContent("Sphere"), new GUIContent("Rectangle")};

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
//		ob.Update();
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

		editor.Update();
	}

	protected void doMaskGUI() {
		// mask list
		SerializedProperty voxelMasks = ob.FindProperty("masks");
		EditorGUILayout.PropertyField(voxelMasks, new GUIContent("Sculpting Masks"), true);
	}

	protected void doSculptGUI() {
		Vox.VoxelEditor editor = (Vox.VoxelEditor)target;

		// brush list
		editor.selectedBrush = GUILayout.Toolbar(editor.selectedBrush, brushes, GUILayout.MinHeight(20));
		
		// brush material type
		string[] materials = new string[editor.voxelMaterials.Length];
		for(int i=0; i<materials.Length; ++i)
			materials[i] = editor.voxelMaterials[i].name;
		
		// brush size
		switch(editor.selectedBrush) {
		case 0:
			GUILayout.BeginHorizontal();
			GUILayout.Label("Sphere Radius", GUILayout.ExpandWidth(false));
			editor.sphereBrushSize = GUILayout.HorizontalSlider(editor.sphereBrushSize, 0, 100);
			editor.sphereBrushSize = EditorGUILayout.FloatField(editor.sphereBrushSize, GUILayout.MaxWidth(64));
			if (editor.sphereBrushSize < 0)
				editor.sphereBrushSize = 0;
			GUILayout.EndHorizontal();

			editor.sphereBrushMaterial = (byte)GUILayout.SelectionGrid(editor.sphereBrushMaterial, materials, 1);
			break;

		case 1:
			GUILayout.BeginHorizontal();
			GUILayout.Label("Dimensions");
			editor.cubeBrushDimensions.x = EditorGUILayout.FloatField(editor.cubeBrushDimensions.x);
			editor.cubeBrushDimensions.y = EditorGUILayout.FloatField(editor.cubeBrushDimensions.y);
			editor.cubeBrushDimensions.z = EditorGUILayout.FloatField(editor.cubeBrushDimensions.z);
//			SerializedProperty cubeBrushDimensions = ob.FindProperty("cubeBrushDimensions");
//			EditorGUILayout.PropertyField(cubeBrushDimensions, new GUIContent("Rectangle Brush Dimensions"), true);
			GUILayout.EndHorizontal();

			editor.cubeBrushMaterial = (byte)GUILayout.SelectionGrid(editor.cubeBrushMaterial, materials, 1);
			break;
		}

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
		string generateButtonName = editor.hasVoxelData()? "Regenerate": "Generate";
		if (GUILayout.Button(generateButtonName)) {
			if (editor.voxelMaterials.Length < 1) {
				EditorUtility.DisplayDialog("Invalid Generation Parameters", "There must be at least one voxel material defined to generate the voxel object.", "OK");
			} else if (EditorUtility.DisplayDialog(generateButtonName +" Voxels?", "Are you sure you want to generate the voxel terain from scratch?", "Yes", "No")) {
				editor.wipe();
				editor.init();
				editor.generateRenderers();
			}
		}
		if (editor.hasVoxelData()) {
			if (GUILayout.Button("Clear")) {
				if (EditorUtility.DisplayDialog("Clear Voxels?", "Are you sure you want to clear all voxel data?", "Yes", "No")) {
					editor.wipe();
				}
			}
			if (GUILayout.Button("Reskin")) {
				if (EditorUtility.DisplayDialog("Regenerate Voxel Meshes?", "Are you sure you want to regenerate all voxel meshes?", "Yes", "No")) {
					editor.generateRenderers();
				}
			}
			if (GUILayout.Button("Export")) {
				editor.export(EditorUtility.SaveFilePanel("Choose File to Export To", "", "Voxels", "vox"));
			}
		}
		if (GUILayout.Button("Import")) {
			if (!editor.import(EditorUtility.OpenFilePanel("Choose File to Export To", "", "vox"))) {
				EditorUtility.DisplayDialog("Wrong Voxel Format", "The file you chose was an unknown or incompatible voxel format version.", "OK");
			}
		}
		lock(editor.renderers) {
			EditorGUILayout.LabelField("Chunk Count: " + editor.renderers.Count);
		}
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
					subtractBrush(editor, HandleUtility.GUIPointToWorldRay(Event.current.mousePosition));
				} else {
					addBrush(editor, HandleUtility.GUIPointToWorldRay(Event.current.mousePosition));
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

	protected static void addBrush(Vox.VoxelEditor editor, Ray mouseLocation) {
		Vector3 point = getRayCollision(mouseLocation).point;
		switch(editor.selectedBrush) {
		case 0:
			new Vox.SphereModifier(editor, point, editor.sphereBrushSize, new Vox.Voxel(editor.sphereBrushMaterial, byte.MaxValue), true);
			break;
		case 1:
			new Vox.CubeModifier(editor, point, editor.cubeBrushDimensions, new Vox.Voxel(editor.cubeBrushMaterial, byte.MaxValue), true);
			break;
		}
	}
	
	protected static void subtractBrush(Vox.VoxelEditor editor, Ray mouseLocation) {
		Vector3 point = getRayCollision(mouseLocation).point;
		switch(editor.selectedBrush) {
		case 0:
			new Vox.SphereModifier(editor, point, editor.sphereBrushSize, new Vox.Voxel(0, byte.MinValue), true);
			break;
		case 1:
			new Vox.CubeModifier(editor, point, editor.cubeBrushDimensions, new Vox.Voxel(0, byte.MinValue), true);
			break;
		}
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
