using UnityEngine;
using System.Collections;

namespace Vox {

	[ExecuteInEditMode]
	[System.Serializable]
	public class VoxelIndex {

		public int x, y, z;
		public byte depth;

		public VoxelIndex() {
			this.x = 0;
			this.y = 0;
			this.z = 0;
			this.depth = 0;
		}

		public VoxelIndex(int x, int y, int z, byte depth) {
			this.x = x;
			this.y = y;
			this.z = z;
			this.depth = depth;
		}


		public override bool Equals(object ob) {
			if (ob == null || GetType () != ob.GetType())
				return false;
			return this == (VoxelIndex)ob;
		}

		public static bool operator==(VoxelIndex v1, VoxelIndex v2) {
			if (System.Object.ReferenceEquals(v1, v2))
				return true;
			if (((object)v1 == null) ^ ((object)v2 == null))
				return false;
			return (v1.x == v2.x && v1.y == v2.y && v1.z == v2.z && v1.depth == v2.depth);
		}
		
		public static bool operator!=(VoxelIndex v1, VoxelIndex v2) {
			return !(v1 == v2);
		}

		public override int GetHashCode() {
			long h = x ^ y ^ z ^ depth;
			h = (h^0xdeadbeef) + (h<<4);
			h = h ^ (h>>10);
			h = h + (h<<7);
			return (int)(h ^ (h>>13));
		}
	}
}