using UnityEngine;
using System.Collections;
using System;

namespace Vox {

	public class SphereModifier : Mutator {

		public Voxel value;
		public bool overwriteSubstance = true;
		public bool overwriteShape = true;
		public float worldRadius;
		public Vector3 worldPosition;

		public SphereModifier(Vector3 worldPosition, float worldRadius, Voxel value) {
			this.value = value;
			this.worldPosition = worldPosition;
			this.worldRadius = worldRadius;
		}

		protected override Application setup(Tree target) {
			//SphereApp app = new SphereApp();
			//Vector3 radiusCube = new Vector3(worldRadius, worldRadius, worldRadius) / target.voxelSize();
			//Vector3 min = target.transform.InverseTransformPoint(worldPosition) / target.voxelSize() - radiusCube - Vector3.one * 0.5f;
			//Vector3 max = min + radiusCube * 2;
			//center = (min + max) / 2;
			//radius = center.x - min.x;
			//minDis = (radius - 1);
			//maxDis = (radius + 1);
			////setMinMax(min, max);
			//app.min;

			Vector3 radiusCube = new Vector3(worldRadius, worldRadius, worldRadius) / target.voxelSize();
			Vector3 center = target.transform.InverseTransformPoint(worldPosition) / target.voxelSize();// - Vector3.one * 0.5f;
			Vector3 exactMin = center - radiusCube;
			Vector3 exactMax = center + radiusCube;
			SphereApp app = new SphereApp();
			app.tree = target;
			//app.min = new Index(target.maxDetail, (uint)exactMin.x, (uint)exactMin.y, (uint)exactMin.z);
			//app.max = new Index(target.maxDetail, (uint)exactMax.x, (uint)exactMax.y, (uint)exactMax.z);
			//app.minRadius = radius / target.voxelSize() - 1;
			//app.minRadSqr = app.minRadius* app.minRadius;
			//app.maxRadius = radius / target.voxelSize() + 1;
			//app.maxRadSqr = app.maxRadius* app.maxRadius;
			app.center = center;
			app.radius = radiusCube.x;
			MonoBehaviour.print("Simple radius: " + radiusCube);
			MonoBehaviour.print("center: " +app.center);
			return app;
		}

		//public override Vector3 getRadiusCube() {
		//	return new Vector3(radius, radius, radius);
		//}

		protected override Action mutate(Application app, Index p, VoxelBlock parent) {
			SphereApp sApp = (SphereApp)app;
			float voxelSize = (1 << (app.tree.maxDetail -p.depth));
			float disSqr = (sApp.center - new Vector3(p.x +0.5f, p.y +0.5f, p.z +0.5f) * voxelSize).sqrMagnitude;
			float maxRadius = sApp.radius + voxelSize * 0.75f;
			float maxRadSqr = maxRadius * maxRadius;
			//MonoBehaviour.print("workin' it. " +voxelSize *0.75f);
			//MonoBehaviour.print("Simple radius: " +sApp.radius);
			//MonoBehaviour.print("sqr Dist: " +disSqr +", Sqr Rad: " +maxRadSqr);
			if (disSqr > maxRadSqr)
				return new Action(false, false);
			float minRadius = Mathf.Max(0, sApp.radius - voxelSize * 0.75f);
			float minRadSqr = minRadius * minRadius;
			//MonoBehaviour.print("Min Sqr Rad: " + minRadSqr);
			if (disSqr < minRadSqr) {
				parent.children[p.xLocal, p.yLocal, p.zLocal] = value;
				return new Action(false, true);
			}
			//MonoBehaviour.print("should traverse.");

			if (p.depth < app.tree.maxDetail)
				return new Action(true, false);
			//MonoBehaviour.print("didn't traverse.");

			float dis = Mathf.Sqrt(disSqr);
			byte newOpacity = (byte)((parent.children[p.xLocal, p.yLocal, p.zLocal].averageOpacity()
				* (dis - minRadius) + value.averageOpacity() * (maxRadius - dis)) / 2);
			if ((dis - minRadius) > 0.5f)
				parent.children[p.xLocal, p.yLocal, p.zLocal] = new Voxel(value.matType, newOpacity);
			else
				parent.children[p.xLocal, p.yLocal, p.zLocal] = new Voxel(parent.children[p.xLocal, p.yLocal, p.zLocal].averageMaterialType(), newOpacity);
			return new Action(false, true);
		}

		//protected override Action mutate(VoxelHolder original, int x, int y, int z) {
		//	float dis = (center - new Vector3(x, y, z)).magnitude;
		//	if (dis > maxDis)
		//		return original;
		//	if (dis < minDis)
		//		return new Voxel(value.averageMaterialType(), overwriteShape? value.averageOpacity(): original.averageOpacity());
		//	byte newOpacity = (byte)((original.averageOpacity() * (dis - minDis) + value.averageOpacity() * (maxDis - dis)) / 2);
		//	byte newSubstance = value.averageMaterialType();
		//	if (newOpacity >= 2 *original.averageOpacity() ||
		//	    (overwriteSubstance && dis < radius))
		//		newSubstance = value.averageMaterialType();
		//	if (!overwriteShape)
		//		newOpacity = original.averageOpacity();
		//	return new Voxel(newSubstance, newOpacity);
		//}

		protected class SphereApp: Application {
			//public float minRadius, minRadSqr, maxRadius, maxRadSqr;
			public Vector3 center;
			public float radius;
		}

	}
}
