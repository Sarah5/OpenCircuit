using UnityEngine;
using System.Collections;

namespace Vox {

	[System.Serializable]
	public class VoxelSubstance {

		public string name;
		public Material renderMaterial;
		public Material blendMaterial;
		public PhysicMaterial physicsMaterial;

		// physics
//		public float strength = 10;
//		public float pliability = 10;
//		public float conductivity = 1;

		public VoxelSubstance() {
//			this.strength = 10;
//			this.pliability = 10;
//			this.conductivity = 1;
		}

		public VoxelSubstance(
			string name,
			Material renderMaterial,
			Material blendMaterial,
			PhysicMaterial physicsMaterial
//			float strength, float pliability, float conductivity
			) {
			this.name = name;
			this.renderMaterial = renderMaterial;
			this.blendMaterial = blendMaterial;
			this.physicsMaterial = physicsMaterial;
//			this.strength = strength;
//			this.pliability = pliability;
//			this.conductivity = conductivity;
		}
	}
}