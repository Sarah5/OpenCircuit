using UnityEngine;
using System.Collections;
using System;

namespace Vox {

	public class SphereMutator : LocalMutator {

		public Voxel value;
		public bool overwriteSubstance = true;
		public bool overwriteShape = true;
		public float worldRadius;
		public Vector3 worldPosition;

		public SphereMutator(Vector3 worldPosition, float worldRadius, Voxel value) {
			this.value = value;
			this.worldPosition = worldPosition;
			this.worldRadius = worldRadius;
		}

		public override Application setup(Tree target) {

			float radius = worldRadius / target.voxelSize();
			Vector3 radiusCube = new Vector3(radius, radius, radius);
			Vector3 center = target.transform.InverseTransformPoint(worldPosition) / target.voxelSize();
			Vector3 exactMin = center - radiusCube;
			Vector3 exactMax = center + radiusCube;
			SphereApp app = new SphereApp();
			app.tree = target;
			app.min = new Index(target.maxDetail, (uint)exactMin.x, (uint)exactMin.y, (uint)exactMin.z);
			app.max = new Index(target.maxDetail, (uint)exactMax.x, (uint)exactMax.y, (uint)exactMax.z);
			app.minRadius = radius -1;
			app.maxRadius = radius +1;
			app.position = center;
			app.radius = radius;
			return app;
		}

		public override Action mutate(LocalApplication app, Index p, VoxelBlock parent, Vector3 diff) {
			SphereApp sApp = (SphereApp)app;
			float voxelSize = (1 << (app.tree.maxDetail -p.depth));
			float disSqr = diff.sqrMagnitude;
			float maxRadius = sApp.radius + voxelSize * 0.75f;
			float maxRadSqr = maxRadius * maxRadius;
			if (disSqr > maxRadSqr)
				return new Action(false, false);
			float minRadius = Mathf.Max(0, sApp.radius - voxelSize * 0.75f);
			float minRadSqr = minRadius * minRadius;
			if (disSqr < minRadSqr) {
				parent.children[p.xLocal, p.yLocal, p.zLocal] =
					new Voxel(value.averageMaterialType(), overwriteShape ? value.averageOpacity() :
					parent.children[p.xLocal, p.yLocal, p.zLocal].averageOpacity());
				return new Action(false, true);
			}

			if (p.depth < app.tree.maxDetail)
				return new Action(true, false);

			float dis = Mathf.Sqrt(disSqr);
			
			VoxelHolder original = parent.children[p.xLocal, p.yLocal, p.zLocal];
			byte newOpacity = (byte)((original.averageOpacity() * (dis - sApp.minRadius) + value.averageOpacity() * (sApp.maxRadius - dis)) / 2);
			byte newSubstance = original.averageMaterialType();
			if (newOpacity >= 2 * original.averageOpacity() ||
				(overwriteSubstance && dis < sApp.radius))
				newSubstance = value.averageMaterialType();
			if (!overwriteShape)
				newOpacity = original.averageOpacity();
			parent.children[p.xLocal, p.yLocal, p.zLocal] = new Voxel(newSubstance, newOpacity);
			return new Action(false, true);
		}

		protected class SphereApp: LocalApplication {
			public float minRadius, maxRadius;
			public float radius;
		}

	}
}
