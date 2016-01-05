using UnityEngine;
using System.Collections;
using System;

namespace Vox {

	public class CubeMutator : LocalMutator {

		public Voxel value;
		public Vector3 min, max;
		public bool overwriteSubstance = true;
		public bool overwriteShape = true;

		public Vector3 worldPosition;
		private Vector3 worldDimensions;

		public CubeMutator(Tree control, Vector3 worldPosition, Vector3 worldDimensions, VoxelHolder value, bool updateMesh) {
			this.worldPosition = worldPosition;
			this.worldDimensions = worldDimensions;
			this.value = value.toVoxel();
		}

		public override Application setup(Tree target) {
			Vector3 halfDimension = worldDimensions / target.voxelSize() /2f;
			Vector3 center = target.transform.InverseTransformPoint(worldPosition) / target.voxelSize();
			Vector3 exactMin = center - halfDimension;
			Vector3 exactMax = center + halfDimension;

			CubeApp app = new CubeApp();
			app.tree = target;
			app.halfDimension = halfDimension;
			app.min = new Index(target.maxDetail, (uint)exactMin.x, (uint)exactMin.y, (uint)exactMin.z);
			app.max = new Index(target.maxDetail, (uint)exactMax.x, (uint)exactMax.y, (uint)exactMax.z);
			app.position = center;
			return app;
		}

		public override Action mutate(LocalApplication app, Index p, VoxelBlock parent, Vector3 diff, float voxelSize) {
			CubeApp cApp = (CubeApp)app;
			if (p.depth >= app.tree.maxDetail)
				voxelSize *= 0.5f;

			double percentInside = 1;
			bool outside = false;
			bool inside = true;

			percentInside *= 1 - (2 - percentOverlapping(diff.x, cApp.halfDimension.x, voxelSize, ref outside, ref inside)
				- percentOverlapping(-diff.x, cApp.halfDimension.x, voxelSize, ref outside, ref inside));
			if (outside) return new Action(false, false);
			percentInside *= 1 - (2 - percentOverlapping(diff.y, cApp.halfDimension.y, voxelSize, ref outside, ref inside)
				- percentOverlapping(-diff.y, cApp.halfDimension.y, voxelSize, ref outside, ref inside));
			if (outside) return new Action(false, false);
			percentInside *= 1 - (2 - percentOverlapping(diff.z, cApp.halfDimension.z, voxelSize, ref outside, ref inside)
				- percentOverlapping(-diff.z, cApp.halfDimension.z, voxelSize, ref outside, ref inside));
			if (outside) return new Action(false, false);

			if (inside) {
				parent.children[p.xLocal, p.yLocal, p.zLocal] =
					new Voxel(value.averageMaterialType(), overwriteShape ? value.averageOpacity() :
					parent.children[p.xLocal, p.yLocal, p.zLocal].averageOpacity());
				return new Action(false, true);
			}

			if (p.depth < app.tree.maxDetail)
				return new Action(true, false);

			VoxelHolder original = parent.children[p.xLocal, p.yLocal, p.zLocal];
			byte newOpacity = (byte)((original.averageOpacity() * (1 - percentInside) + value.averageOpacity() * (percentInside)));
			byte newSubstance = original.averageMaterialType();
			if (newOpacity >= 2 * original.averageOpacity() ||
				(overwriteSubstance && percentInside > 0.5))
				newSubstance = value.averageMaterialType();
			if (!overwriteShape)
				newOpacity = original.averageOpacity();
			parent.children[p.xLocal, p.yLocal, p.zLocal] = new Voxel(newSubstance, newOpacity);
			return new Action(false, true);
		}

		protected double percentOverlapping(double lower, double upper, double halfVoxelSize, ref bool outside, ref bool inside) {
            if (upper > lower + halfVoxelSize) {
				outside |= false;
				inside &= true;
				return 1;
			} else if (upper < lower - halfVoxelSize) {
				outside = true;
				inside = false;
				return 0;
			}
			outside |= false;
			inside = false;
			return (upper - lower + halfVoxelSize) /halfVoxelSize /2.0;
		}

		protected class CubeApp : LocalApplication {
			public Vector3 halfDimension;
		}

	}
}
