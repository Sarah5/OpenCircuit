using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[InitializeOnLoad]
class VoxelEditorProgressController {

	private static int jobCount;

	static VoxelEditorProgressController() {
		EditorApplication.update += Update;
	}

	public static void Update() {
		if (Vox.VoxelThread.getStartJobCount() > 0)
			jobCount = Vox.VoxelThread.getStartJobCount();
		
		foreach (Vox.Tree tree in new List<Vox.Tree>(Vox.Tree.generatingTrees)) {
			tree.Update();
		}
	}

	public static float getGenerationProgress(Vox.Tree editor) {
		return 1 -(float)(editor.getJobCount() +Vox.VoxelThread.getJobCount()) /jobCount *2;
	}
}
