using UnityEngine;
using System.Collections;

namespace Vox {

	public class BlurMutator : Mutator {

		public int blurRadius = 3;
		public bool overwriteSubstance = false;
		public float strength;
		public Vector3 worldPosition;
		public float worldRadius;

		public BlurMutator(Tree control, Vector3 worldPosition, float worldRadius, float strength) {
			this.strength = strength;
			this.worldPosition = worldPosition;
			this.worldRadius = worldRadius;
		}

		public override Application setup(Tree target) {
			float radius = worldRadius / target.voxelSize();
			Vector3 radiusCube = new Vector3(radius, radius, radius);
			Vector3 center = target.transform.InverseTransformPoint(worldPosition) / target.voxelSize();
			Vector3 exactMin = center - radiusCube;
			Vector3 exactMax = center + radiusCube;
			BlurApp app = new BlurApp();
			app.tree = target;
			app.min = new Index(target.maxDetail, (uint)exactMin.x, (uint)exactMin.y, (uint)exactMin.z);
			app.max = new Index(target.maxDetail, (uint)exactMax.x, (uint)exactMax.y, (uint)exactMax.z);
			app.minRadius = radius - 1;
			app.maxRadius = radius + 1;
			app.position = center;
			app.radius = radius;
			app.setOriginal(target);
			return app;
		}

		protected override Action mutate(Application app, Index pos, VoxelBlock parent) {
			BlurApp bApp = (BlurApp)app;
			float voxelSize = LocalMutator.calculateVoxelSize(app, pos);
			Vector3 diff = LocalMutator.calculateDiff(bApp.position, pos, voxelSize);

			float disSqr = diff.sqrMagnitude;
			float maxRadius = bApp.radius + voxelSize;
			float maxRadSqr = maxRadius * maxRadius;
			if (disSqr > maxRadSqr)
				return new Action(false, false);

			if (pos.depth < app.tree.maxDetail)
				return new Action(true, false);

			float dis = Mathf.Sqrt(disSqr);
			float actualStrength = strength * (1 - (dis / bApp.radius));
			if (actualStrength <= 0)
				return new Action(false, false);
			byte newOpacity = calculateOpacity(bApp.original, pos.x - app.min.x, pos.y - app.min.y, pos.z - app.min.z, actualStrength);
			Voxel original = parent.children[pos.xLocal, pos.yLocal, pos.zLocal].toVoxel();

			parent.children[pos.xLocal, pos.yLocal, pos.zLocal] = new Voxel(original.averageMaterialType(), newOpacity);
			return new Action(false, true);
		}

		protected class BlurApp: LocalMutator.LocalApplication {
			public float minRadius, maxRadius;
			public Voxel[,,] original;
			public float radius;

			public void setOriginal(Tree target) {
				original = target.getArray((int)min.x, (int)min.y, (int)min.z, (int)max.x + 1, (int)max.y + 1, (int)max.z + 1);
			}
		}

		protected byte calculateOpacity(Voxel[,,] original, uint x, uint y, uint z, float strength) {
			double opacity = original[x, y, z].averageOpacity();
			int minX = Mathf.Max((int)x - blurRadius, 0);
			int maxX = Mathf.Min((int)x + blurRadius, original.GetLength(0));
			int minY = Mathf.Max((int)y - blurRadius, 0);
			int maxY = Mathf.Min((int)y + blurRadius, original.GetLength(1));
			int minZ = Mathf.Max((int)z - blurRadius, 0);
			int maxZ = Mathf.Min((int)z + blurRadius, original.GetLength(2));
			int count = 0;
			for (int xi = minX; xi < maxX; ++xi) {
				for (int yi = minY; yi < maxY; ++yi) {
					for (int zi = minZ; zi < maxZ; ++zi) {
						++count;
						Vector3 diff = new Vector3(x - xi, y - yi, z - zi);
						float dis = diff.magnitude;
						Voxel value = original[xi, yi, zi];
						if (dis < 0.5f || value == null)
							continue;
						float factor = Mathf.Max((1 - dis / blurRadius) * strength * 0.1f, 0);
						opacity = opacity * (1 - factor) + value.averageOpacity() * factor;
					}
				}
			}
			return (byte)Mathf.Min((float)opacity, byte.MaxValue);
		}
	}
}