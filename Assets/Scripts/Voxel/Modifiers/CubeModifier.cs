using UnityEngine;
using System.Collections;

namespace Vox {

	public class CubeModifier : Modifier {

		public VoxelHolder value;

		public CubeModifier(VoxelTree control, Vector3 worldPosition, Vector3 worldDimensions, VoxelHolder value, bool updateMesh)
			: base(control, updateMesh) {
			this.value = value;
			Vector3 dimensions = worldDimensions / control.voxelSize();
			min = control.transform.InverseTransformPoint(worldPosition) / control.voxelSize() - dimensions /2 - Vector3.one * (control.voxelSize() / 2);
			max = min + dimensions;
			apply();
		}


		protected override VoxelHolder modifyVoxel(VoxelHolder original, int x, int y, int z) {
			return new Voxel(value.toVoxel());
		}

	}
}
