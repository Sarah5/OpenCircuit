using UnityEngine;
using System.Collections;

namespace Vox {
	public class BlurModifier : Modifier {

		public const int blurRadius = 3;
		
		public bool overwriteSubstance;

		protected Voxel[,,] original;
		protected float strength;
		protected Vector3 center;
		protected float radius;
		
		public BlurModifier(VoxelTree control, Vector3 worldPosition, float worldRadius, float strength, bool updateMesh)
		: base(control, updateMesh) {
			this.strength = strength;
			Vector3 radiusCube = new Vector3(worldRadius, worldRadius, worldRadius) / control.voxelSize();
			Vector3 min = control.transform.InverseTransformPoint(worldPosition) / control.voxelSize() - radiusCube -Vector3.one *0.5f;
			Vector3 max = min + radiusCube *2;
			center = (min + max) / 2;
			radius = center.x - min.x;
			setMinMax(min, max);
			setOriginal();
			MonoBehaviour.print (min);
			apply();
		}
		
		
		protected override VoxelHolder modifyVoxel(VoxelHolder original, int x, int y, int z) {
			float dis = (center - new Vector3(x, y, z)).magnitude;
			float actualStrength = strength *(1 -(dis /radius));
			if (actualStrength <= 0)
				return original;
			byte newOpacity = calculateOpacity(x -minX, y -minY, z -minZ, actualStrength);
			return new Voxel(original.averageMaterialType(), newOpacity);
		}

		protected void setOriginal() {
			original = control.getArray(minX, minY, minZ, maxX +1, maxY +1, maxZ +1);
		}

		protected byte calculateOpacity(int x, int y, int z, float strength) {
			double opacity = original[x, y, z].averageOpacity();
			int minX = Mathf.Max(x -blurRadius, 0);
			int maxX = Mathf.Min(x +blurRadius, original.GetLength(0));
			int minY = Mathf.Max(y -blurRadius, 0);
			int maxY = Mathf.Min(y +blurRadius, original.GetLength(1));
			int minZ = Mathf.Max(z -blurRadius, 0);
			int maxZ = Mathf.Min(z +blurRadius, original.GetLength(2));
			int count = 0;
			for(int xi=minX; xi<maxX; ++xi) {
				for(int yi=minY; yi<maxY; ++yi) {
					for(int zi=minZ; zi<maxZ; ++zi) {
						++count;
						Vector3 diff = new Vector3(x -xi, y -yi, z -zi);
						float dis = diff.magnitude;
						Voxel value = original[xi, yi, zi];
						if (dis < 0.5f || value == null)
							continue;
						float factor = Mathf.Max((1 -dis /blurRadius) *strength *0.1f, 0);
						opacity = opacity *(1 -factor) +value.averageOpacity() *factor;
					}
				}
			}
			return (byte) Mathf.Min((float)opacity, byte.MaxValue);
		}
	}
}