using UnityEngine;
using System.Collections;

namespace Vox {

	public class CubeModifier : Modifier {

		public Voxel value;
        public bool overwriteSubstance;

        public CubeModifier(VoxelTree control, Vector3 worldPosition, Vector3 worldDimensions, VoxelHolder value, bool updateMesh)
			: base(control, updateMesh) {
			this.value = value.toVoxel();
			Vector3 dimensions = worldDimensions / control.voxelSize();
			min = control.transform.InverseTransformPoint(worldPosition) / control.voxelSize() - dimensions /2 - Vector3.one * (control.voxelSize() / 2);
			max = min + dimensions;
			apply();
		}


		protected override VoxelHolder modifyVoxel(VoxelHolder original, int x, int y, int z) {
			double percentInside = 1;
			percentInside *= percentOverlapping(x, min.x);
			percentInside *= percentOverlapping(y, min.y);
			percentInside *= percentOverlapping(z, min.z);
			percentInside *= percentOverlapping(-x, -max.x);
			percentInside *= percentOverlapping(-y, -max.y);
            percentInside *= percentOverlapping(-z, -max.z);
            // MonoBehaviour.print(percentInside);
            if (percentInside <= 0.001)
				return original;
			if (percentInside >= 0.999)
				return value;
			byte newOpacity = (byte)((original.averageOpacity() * (1 -percentInside) + value.averageOpacity() * (percentInside)));
			if (newOpacity >= 2 *original.averageOpacity() ||
			    (overwriteSubstance && percentInside > 0.5))
				return new Voxel(value.averageMaterialType(), newOpacity);
			return new Voxel(original.averageMaterialType(), newOpacity);
		}

        protected double percentOverlapping(double lower, double upper) {
			if (upper > lower +0.5)
                return 0;
			else if (upper < lower -0.5)
                return 1;
			return lower -upper +0.5;
        }

    }
}
