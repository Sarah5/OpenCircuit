using UnityEngine;
using System.Collections;

namespace Vox {

	public abstract class Modifier {

		public VoxelTree control;
		public Vector3 min, max;
		public bool updateMesh;

		protected uint minY;
		protected uint maxY;

		protected Modifier(VoxelTree control, bool updateMesh) {
			this.control = control;
			this.updateMesh = updateMesh;
		} 

		//protected Modifier(VoxelControlV2 control, Vector3 min, Vector3 max) {
		//	this.control = control;
		//	this.min = min;
		//	this.max = max;
		//}

		protected void apply() {
			minY = uint.MinValue;
			maxY = uint.MaxValue;
			if (control.masks != null) {
				foreach (VoxelMask mask in control.masks) {
					if (mask.active) {
						if (mask.maskAbove) {
							if (maxY > mask.yPosition)
								maxY = mask.yPosition;
						} else if (minY < mask.yPosition) {
							minY = mask.yPosition;
						}
					}
				}
			}
			maxY -= 1;
			MonoBehaviour.print("minY: " +minY);
			MonoBehaviour.print("maxY: " +maxY);
			traverse(control.getBaseUpdateInfo(), control.maxDetail);
			control.dirty = true;
		}

		protected void traverse(VoxelUpdateInfo info, byte detailLevel) {
			int factor = 1 << (detailLevel - VoxelBlock.CHILD_COUNT_POWER);
			byte xiMin = (byte)Mathf.Max(min.x / factor - info.x * VoxelBlock.CHILD_DIMENSION, 0);
			byte xiMax = (byte)Mathf.Min((max.x + 3) / factor - info.x * VoxelBlock.CHILD_DIMENSION, VoxelBlock.CHILD_DIMENSION - 1);
			byte yiMin = (byte)Mathf.Max(min.y / factor - info.y * VoxelBlock.CHILD_DIMENSION, 0);
			byte yiMax = (byte)Mathf.Min((max.y + 3) / factor - info.y * VoxelBlock.CHILD_DIMENSION, VoxelBlock.CHILD_DIMENSION - 1);
			byte ziMin = (byte)Mathf.Max(min.z / factor - info.z * VoxelBlock.CHILD_DIMENSION, 0);
			byte ziMax = (byte)Mathf.Min((max.z + 3) / factor - info.z * VoxelBlock.CHILD_DIMENSION, VoxelBlock.CHILD_DIMENSION - 1);

			VoxelBlock block = (VoxelBlock)info.blocks[1, 1, 1];

			uint scale = (uint) (1 << (VoxelBlock.CHILD_COUNT_POWER *(detailLevel -1)));
//			MonoBehaviour.print (scale);
//			MonoBehaviour.print (detailLevel);

			for (byte yi = yiMin; yi <= yiMax; ++yi) {

				if ((info.y *VoxelBlock.CHILD_DIMENSION +yi) < minY /scale ||
				    (info.y *VoxelBlock.CHILD_DIMENSION +yi) > maxY /scale +1) {
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
