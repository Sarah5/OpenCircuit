using UnityEngine;
using System.Collections;

namespace Vox {

	public class SphereModifier : Modifier {

		public VoxelHolder value;
		public bool overwriteSubstance = true;
		public bool overwriteShape = true;

		protected Vector3 center;
		protected float radius;
		protected float minDis;
		protected float maxDis;

		public SphereModifier(VoxelTree control, Vector3 worldPosition, float worldRadius, VoxelHolder value, bool updateMesh)
			: base(control, updateMesh) {
			this.value = value;
			Vector3 radiusCube = new Vector3(worldRadius, worldRadius, worldRadius) / control.voxelSize();
			Vector3 min = control.transform.InverseTransformPoint(worldPosition) / control.voxelSize() - radiusCube - Vector3.one * (control.voxelSize() / 2);
			Vector3 max = min + radiusCube * 2;
			center = (min + max) / 2;
			radius = center.x - min.x;
			minDis = (radius - 1);
			maxDis = (radius + 1);
			setMinMax(min, max);
//			apply();
		}


		protected override VoxelHolder modifyVoxel(VoxelHolder original, int x, int y, int z) {
			float dis = (center - new Vector3(x, y, z)).magnitude;
			if (dis > maxDis)
				return original;
			if (dis < minDis)
				return new Voxel(value.averageMaterialType(), overwriteShape? value.averageOpacity(): original.averageOpacity());
			byte newOpacity = (byte)((original.averageOpacity() * (dis - minDis) + value.averageOpacity() * (maxDis - dis)) / 2);
			byte newSubstance = value.averageMaterialType();
			if (newOpacity >= 2 *original.averageOpacity() ||
			    (overwriteSubstance && dis < radius))
				newSubstance = value.averageMaterialType();
			if (!overwriteShape)
				newOpacity = original.averageOpacity();
			return new Voxel(newSubstance, newOpacity);
		}

	}
}
