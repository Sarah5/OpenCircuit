using UnityEngine;

namespace Vox {

	//public class LineModifier : Modifier {

	//	public Modifier modifier;

	//	protected Vector3 center;
	//	protected float radius;
	//	protected float minDis;
	//	protected float maxDis;

	//	public LineModifier(VoxelTree control, Vector3 worldStartPos, Vector3 worldEndPos, Modifier modifier, bool updateMesh)
	//		: base(control, updateMesh) {
	//		this.modifier = modifier;
	//		Vector3 radiusCube = new Vector3(worldRadius, worldRadius, worldRadius) / control.voxelSize();
	//		Vector3 min = control.transform.InverseTransformPoint(worldPosition) / control.voxelSize() - radiusCube - Vector3.one * 0.5f;
	//		Vector3 max = min + radiusCube * 2;
	//		center = (min + max) / 2;
	//		radius = center.x - min.x;
	//		minDis = (radius - 1);
	//		maxDis = (radius + 1);
	//		setMinMax(min, max);
	//	}


	//	protected override VoxelHolder modifyVoxel(VoxelHolder original, int x, int y, int z) {
	//		float dis = (center - new Vector3(x, y, z)).magnitude;
	//		if (dis > maxDis)
	//			return original;
	//		if (dis < minDis)
	//			return new Voxel(value.averageMaterialType(), overwriteShape ? value.averageOpacity() : original.averageOpacity());
	//		byte newOpacity = (byte)((original.averageOpacity() * (dis - minDis) + value.averageOpacity() * (maxDis - dis)) / 2);
	//		byte newSubstance = value.averageMaterialType();
	//		if (newOpacity >= 2 * original.averageOpacity() ||
	//			(overwriteSubstance && dis < radius))
	//			newSubstance = value.averageMaterialType();
	//		if (!overwriteShape)
	//			newOpacity = original.averageOpacity();
	//		return new Voxel(newSubstance, newOpacity);
	//	}

	//}
}
