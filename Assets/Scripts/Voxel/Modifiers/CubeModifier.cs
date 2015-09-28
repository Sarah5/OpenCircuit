using UnityEngine;
using System.Collections;

namespace Vox {

	public class CubeModifier : Modifier {

		public Voxel value;
		public Vector3 min, max;
		public bool overwriteSubstance = true;
		public bool overwriteShape = true;

        public CubeModifier(VoxelTree control, Vector3 worldPosition, Vector3 worldDimensions, VoxelHolder value, bool updateMesh)
			: base(control, updateMesh) {
			this.value = value.toVoxel();
			Vector3 dimensions = worldDimensions / control.voxelSize();
			min = control.transform.InverseTransformPoint(worldPosition) / control.voxelSize() - dimensions /2 -Vector3.one *0.5f;
			max = min + dimensions;
			setMinMax(min, max);
//			apply();
		}


		protected override VoxelHolder modifyVoxel(VoxelHolder original, int x, int y, int z) {
			double percentInside = 1;
			percentInside *= percentOverlapping(x, min.x);
			percentInside *= percentOverlapping(y, min.y);
			percentInside *= percentOverlapping(z, min.z);
			percentInside *= percentOverlapping(-x, -max.x);
			percentInside *= percentOverlapping(-y, -max.y);
            percentInside *= percentOverlapping(-z, -max.z);
            if (percentInside <= 0.001)
				return original;
			if (percentInside >= 0.999)
				return new Voxel(value.averageMaterialType(), overwriteShape? value.averageOpacity(): original.averageOpacity());
			byte newOpacity = (byte)((original.averageOpacity() * (1 -percentInside) + value.averageOpacity() * (percentInside)));
			byte newSubstance = original.averageMaterialType();
			if (newOpacity >= 2 *original.averageOpacity() ||
			    (overwriteSubstance && percentInside > 0.5))
				newSubstance = value.averageMaterialType();
			if (!overwriteShape)
				newOpacity = original.averageOpacity();
			return new Voxel(newSubstance, newOpacity);
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
