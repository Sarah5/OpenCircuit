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
			Vector3 halfDimension = worldDimensions / target.voxelSize() /2;
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

		public override Action mutate(LocalApplication app, Index p, VoxelBlock parent, Vector3 diff) {
			CubeApp cApp = (CubeApp)app;
			double halfVoxelSize = (1 << (app.tree.maxDetail - p.depth)) *0.5;

			// TODO: for huge voxels, this calculation won't be accurate enough to tell us we need to dive deeper into the voxel
			double percentInside = 1;

			percentInside *= 1 - (2 - percentOverlapping(diff.x, cApp.halfDimension.x, halfVoxelSize)
				- percentOverlapping(-diff.x, cApp.halfDimension.x, halfVoxelSize));
			percentInside *= 1 - (2 - percentOverlapping(diff.y, cApp.halfDimension.y, halfVoxelSize)
				- percentOverlapping(-diff.y, cApp.halfDimension.y, halfVoxelSize));
			percentInside *= 1 - (2 - percentOverlapping(diff.z, cApp.halfDimension.z, halfVoxelSize)
				- percentOverlapping(-diff.z, cApp.halfDimension.z, halfVoxelSize));

			if (percentInside <= 0.00000001)
				return new Action(false, false);
			if (percentInside >= 0.99999999) {
				parent.children[p.xLocal, p.yLocal, p.zLocal] =
					new Voxel(value.averageMaterialType(), overwriteShape ? value.averageOpacity() :
					parent.children[p.xLocal, p.yLocal, p.zLocal].averageOpacity());
				return new Action(false, true);
			}

			//MonoBehaviour.print("here? " +percentInside);

			if (p.depth < app.tree.maxDetail)
				return new Action(true, false);

			//MonoBehaviour.print("here!");

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

		protected double percentOverlapping(double lower, double upper, double halfVoxelSize) {
			if (upper > lower + halfVoxelSize)
				return 1;
			else if (upper < lower - halfVoxelSize)
				return 0;
			return (upper - lower + halfVoxelSize) /halfVoxelSize /2.0;
		}

		protected class CubeApp : LocalApplication {
			public Vector3 halfDimension;
		}

	}
}
