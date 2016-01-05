using UnityEngine;
using System.Collections;

namespace Vox {
	public abstract class LocalMutator: Mutator {

		protected Vector3 position;

		protected override Action mutate(Application app, Index p, VoxelBlock parent) {
			LocalApplication lApp = (LocalApplication)app;
			float voxelSize = calculateVoxelSize(app, p);
			Vector3 diff = calculateDiff(lApp.position, p, voxelSize);
			return mutate(lApp, p, parent, diff, voxelSize);
		}

		public abstract Action mutate(LocalApplication app, Index p, VoxelBlock parent, Vector3 diff, float voxelSize);

		public class LocalApplication: Application {
			public Vector3 position;
		}

		public static float calculateVoxelSize(Application app, Index p) {
			return 1 << (app.tree.maxDetail - p.depth);
		}

		public static Vector3 calculateDiff(Vector3 position, Index p, float voxelSize) {
			Vector3 diff = position - new Vector3(p.x + 0.5f, p.y + 0.5f, p.z + 0.5f) * voxelSize;
			return diff;
		}

	}
}