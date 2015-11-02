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
		
		foreach (Vox.VoxelTree tree in new List<Vox.VoxelTree>(Vox.VoxelTree.generatingTrees)) {
			tree.Update();
		}
	}

	public static float getGenerationProgress(Vox.VoxelTree editor) {
		return 1 -(float)(editor.getJobCount() +Vox.VoxelThread.getJobCount()) /jobCount *2;
	}
}
