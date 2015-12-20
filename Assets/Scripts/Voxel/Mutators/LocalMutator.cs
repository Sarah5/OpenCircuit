using UnityEngine;
using System.Collections;

namespace Vox {
	public abstract class LocalMutator: Mutator {

		protected Vector3 position;

		protected override Action mutate(Application app, Index p, VoxelBlock parent) {
			LocalApplication lApp = (LocalApplication)app;
			float voxelSize = (1 << (app.tree.maxDetail - p.depth));
			Vector3 diff = lApp.position - new Vector3(p.x + 0.5f, p.y + 0.5f, p.z + 0.5f) * voxelSize;
			return mutate(lApp, p, parent, diff);
		}

		protected abstract Action mutate(LocalApplication app, Index p, VoxelBlock parent, Vector3 diff);

		protected class LocalApplication: Application {
			public Vector3 position;
		}

	}
}