using UnityEngine;

namespace Vox {

	public class LineMutator : LocalMutator {

		public LocalMutator child;
		public Vector3 start, end;

		public LineMutator(Vector3 start, Vector3 end, LocalMutator child) {
			this.start = start;
			this.end = end;
			this.child = child;
		}

		public override Application setup(Tree target) {

			LineApplication app = new LineApplication();
			app.tree = target;
			app.position = start;
			app.childApp = (LocalApplication) child.setup(target);
			// TODO: set min, max and updateMesh.

			return app;
		}

		protected override Action checkMutation(Application app, Index p) {
			LocalApplication lApp = (LocalApplication)app;
			float voxelSize = calculateVoxelSize(app, p);
			Vector3 diff = calculateDiff(lApp.position, p, voxelSize);
			LocalAction action = checkMutation(lApp, p, diff, voxelSize);
			action.voxelSize = voxelSize;
			return action;
		}

		public override LocalAction checkMutation(LocalApplication app, Index p, Vector3 diff, float voxelSize) {
			Vector3 closestPoint = closetPoint(Vector3.zero, start - end, diff);
			Vector3 virtualDiff = diff - closestPoint;
			LocalAction action = child.checkMutation(((LineApplication)app).childApp, p, virtualDiff, voxelSize);
			action.diff = virtualDiff;
			return action;
		}

		public override Voxel mutate(LocalApplication app, Index p, LocalAction action, Voxel original) {
			return child.mutate(((LineApplication)app).childApp, p, action, original);
		}

		protected class LineApplication: LocalApplication {
			public Vector3 start, end;
			public LocalApplication childApp;
		}


		protected Vector3 closetPoint(Vector3 start, Vector3 end, Vector3 point) {
			Vector3 line = end -start;
			float percent = Vector3.Dot(point -start, line) / Vector3.Dot(line, line);
			percent = Mathf.Clamp01(percent);
			Vector3 closest = start +line *percent;
			return closest;
		}

	}
}
