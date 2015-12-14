using UnityEngine;
using System.Collections;
using System.IO;
using System;

namespace Vox {

	[System.Serializable]
	public class Voxel : VoxelHolder {

		public static readonly Voxel empty = new Voxel(0, 0);

		public readonly byte opacity;
		public readonly byte matType;

		public Voxel(byte materialType, byte opacity) {
			this.matType = materialType;
			this.opacity = opacity;
		}

		public Voxel(Voxel other) {
			this.matType = other.matType;
			this.opacity = other.opacity;
		}

		public Voxel(BinaryReader reader) {
			this.matType = reader.ReadByte();
			this.opacity = reader.ReadByte();
		}

		public override void serialize(BinaryWriter writer) {
			writer.Write(VoxelHolder.VOXEL_SERIAL_ID);
			writer.Write(matType);
			writer.Write(opacity);
		}

		public override byte detail() {
			return 0;
		}

		public override byte averageOpacity() {
			return opacity;
		}

		public override byte averageMaterialType() {
			return matType;
		}

		public override VoxelHolder get(byte detailLevel, int x, int y, int z) {
			return this;
		}
		
		public override VoxelHolder get(VoxelIndex i) {
			return this;
		}

		public override VoxelRenderer getRenderer(byte detailLevel, int x, int y, int z) {
			return null;
		}

		public override void putInArray(byte level, ref Voxel[,,] array, int x, int y, int z, int xMin, int yMin, int zMin, int xMax, int yMax, int zMax) {
			int size = 1 << (VoxelBlock.CHILD_COUNT_POWER *level);
			int xStart = Mathf.Max(x, xMin);
			int xEnd = Mathf.Min(x +size, xMax);
			int yStart = Mathf.Max(y, yMin);
			int yEnd = Mathf.Min(y +size, yMax);
			int zStart = Mathf.Max(z, zMin);
			int zEnd = Mathf.Min(z +size, zMax);
			for(int xi=xStart; xi<xEnd; ++xi) {
				for(int yi=yStart; yi<yEnd; ++yi) {
					for(int zi=zStart; zi<zEnd; ++zi) {
						array[xi -xMin, yi -yMin, zi -zMin] = this;
					}
				}
			}
		}

		public override int canSimplify(out Voxel simplification) {
			simplification = this;
			return 0;
		}

		public static VoxelHolder setSphere(VoxelHolder original, int x, int y, int z, Vector3 min, Vector3 max, VoxelHolder val) {
			Vector3 center = (min + max) / 2;
			float radius = center.x - min.x;
			float minDis = (radius - 1);
			float maxDis = (radius + 1);
			float dis = (center - new Vector3(x, y, z)).magnitude;
			if (dis > maxDis)
				return original;
			if (dis < minDis)
				return val;
			byte newOpacity = (byte)((original.averageOpacity() * (dis - minDis) + val.averageOpacity() * (maxDis - dis)) /2);
			if ((dis - minDis) > 0.5f)
				return new Voxel(val.averageMaterialType(), newOpacity);
			return new Voxel(original.averageMaterialType(), newOpacity);
		}

		public override Voxel toVoxel() {
			return this;
		}

	}

}