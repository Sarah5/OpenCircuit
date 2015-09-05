using UnityEngine;
using System.Collections;

namespace Vox {

	public abstract class Modifier {

		public VoxelTree control;
//		public Vector3 min, max;
		public int minX, minY, minZ;
		public int maxX, maxY, maxZ;
		public bool updateMesh;

		protected uint maskMinY;
		protected uint maskMaxY;

		protected Modifier(VoxelTree control, bool updateMesh) {
			this.control = control;
			this.updateMesh = updateMesh;
		}
		
		protected void setMinMax(Vector3 min, Vector3 max) {
			minX = (int)(min.x +0.01f);
			minY = (int)(min.y +0.01f);
			minZ = (int)(min.z +0.01f);
			maxX = (int)(max.x +0.01f);
			maxY = (int)(max.y +0.01f);
			maxZ = (int)(max.z +0.01f);
		}

		protected void apply() {
			maskMinY = uint.MinValue;
			maskMaxY = uint.MaxValue;
			if (control.masks != null) {
				foreach (VoxelMask mask in control.masks) {
					if (mask.active) {
						if (mask.maskAbove) {
							if (maskMaxY > mask.yPosition)
								maskMaxY = mask.yPosition;
						} else if (maskMinY < mask.yPosition) {
							maskMinY = mask.yPosition;
						}
					}
				}
			}
			maskMaxY -= 1;
			traverse(control.getBaseUpdateInfo(), control.maxDetail);
			control.dirty = true;
		}

		protected void traverse(VoxelUpdateInfo info, byte detailLevel) {
			int factor = 1 << (detailLevel - VoxelBlock.CHILD_COUNT_POWER);
			byte xiMin = (byte)Mathf.Max(minX / factor - info.x * VoxelBlock.CHILD_DIMENSION, 0f);
			byte xiMax = (byte)Mathf.Min((maxX + 1) / factor - info.x * VoxelBlock.CHILD_DIMENSION, VoxelBlock.CHILD_DIMENSION - 1f);
			byte yiMin = (byte)Mathf.Max(minY / factor - info.y * VoxelBlock.CHILD_DIMENSION, 0f);
			byte yiMax = (byte)Mathf.Min((maxY + 1) / factor - info.y * VoxelBlock.CHILD_DIMENSION, VoxelBlock.CHILD_DIMENSION - 1f);
			byte ziMin = (byte)Mathf.Max(minZ / factor - info.z * VoxelBlock.CHILD_DIMENSION, 0f);
			byte ziMax = (byte)Mathf.Min((maxZ + 1) / factor - info.z * VoxelBlock.CHILD_DIMENSION, VoxelBlock.CHILD_DIMENSION - 1f);

			VoxelBlock block = (VoxelBlock)info.blocks[1, 1, 1];

			uint scale = (uint) (1 << (VoxelBlock.CHILD_COUNT_POWER *(detailLevel -1)));

			for (byte yi = yiMin; yi <= yiMax; ++yi) {

				if ((info.y *VoxelBlock.CHILD_DIMENSION +yi) < maskMinY /scale ||
				    (info.y *VoxelBlock.CHILD_DIMENSION +yi) > maskMaxY /scale +1) {
					continue;
				}
				
				for (byte xi = xiMin; xi <= xiMax; ++xi) {
					for (byte zi = ziMin; zi <= ziMax; ++zi) {
						if (detailLevel <= VoxelBlock.CHILD_COUNT_POWER) {
							block.children[xi, yi, zi] = modifyVoxel(block.children[xi, yi, zi], info.x * VoxelBlock.CHILD_DIMENSION + xi, info.y * VoxelBlock.CHILD_DIMENSION + yi, info.z * VoxelBlock.CHILD_DIMENSION + zi);
						} else {
							if (block.children[xi, yi, zi].GetType() == typeof(Voxel)) {
								block.children[xi, yi, zi] = new VoxelBlock((Voxel)block.children[xi, yi, zi]);
							}
							traverse(new VoxelUpdateInfo(info, xi, yi, zi), (byte)(detailLevel - VoxelBlock.CHILD_COUNT_POWER));
						}
					}
				}
			}

			// TODO: this should check if completely contained, not if rendersize (for distant modifications) - FIXED?
			if (updateMesh && info != null && (VoxelBlock.isRenderSize(info.size, control) || VoxelBlock.isRenderLod(info.x, info.y, info.z, control.sizes[detailLevel], control))) {
				block.updateAll(info.x, info.y, info.z, info.detailLevel, control, true);
			}
		}

		protected abstract VoxelHolder modifyVoxel(VoxelHolder original, int x, int y, int z);

	}
}
