using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace Vox {

	[AddComponentMenu("Scripts/Voxel/VoxelEditor")]
	[ExecuteInEditMode]
	public class VoxelEditor : VoxelTree {

		protected static readonly Color brushGhostColor = new Color(0f, 0.8f, 1f, 0.4f);

		public byte[] heightmapSubstances;
        public Texture2D[] heightmaps;
		public float maxChange;
        public int proceduralSeed;
        public float heightPercentage;
        public bool gridEnabled;
        public bool gridUseVoxelUnits;
        public float gridSize = 1;
		public float maskDisplayAlpha = 0.3f;

        // editor data
        [System.NonSerialized]
		public int selectedMode = 0;
		[System.NonSerialized]
		public int selectedBrush = 0;
		[System.NonSerialized]
		public float sphereBrushSize = 1;
		[System.NonSerialized]
		public byte sphereBrushSubstance = 0;
		[System.NonSerialized]
		public Vector3 cubeBrushDimensions = new Vector3(1, 1, 1);
		[System.NonSerialized]
		public byte cubeBrushSubstance = 0;
		[System.NonSerialized]
		public bool drawGhostBrush = true;


		public void Awake() {
			if (Application.isPlaying && !hasRenderers()) {
				generateRenderers();
				pauseForGeneration();
			}
		}

		public void setToHeightmap() {
			int dimension = heightmaps[0].height;

			for (int index = 0; index < heightmaps.Length; ++index ) {
				float[,] map = new float[dimension, dimension];
				for (int i = 0; i < heightmaps[index].height; i++) {
					for (int j = 0; j < heightmaps[index].width; j++) {
						Color pix = heightmaps[index].GetPixel((dimension - 1) - i, j);
						map[j, i] = ((pix.r + pix.g + pix.b) / 3.0f) * dimension;
					}
				}
				head.setToHeightmap(maxDetail, 0, 0, 0, ref map, heightmapSubstances[index], this);
			}
		}

		public void setToHeight() {
			int dimension = 1 << maxDetail;
            float height = heightPercentage / 100f * dimension;
			float[,] map = new float[dimension, dimension];
			for (int i = 0; i < dimension; i++) {
				for (int j = 0; j < dimension; j++) {
					map[j, i] = height;
				}
			}
			head.setToHeightmap(maxDetail, 0, 0, 0, ref map, 0, this);
		}

		// this functions sets the values of the voxels, doing all of the procedural generation work
		// currently it just uses a "height map" system.  This is fine for initial generation, but
		// then more passes need to be done for cliffs, caves, streams, etc.
		public virtual void setToProcedural() {

			// the following generates terrain from a height map
			UnityEngine.Random.seed = proceduralSeed;
			int dimension = 1 << maxDetail;
			float acceleration = 0;
			float height = dimension * 0.6f;
			float[,] heightMap = new float[dimension, dimension];
			float[,] accelMap = new float[dimension, dimension];
			byte[,] matMap = new byte[dimension, dimension];
			for (int x = 0; x < dimension; ++x) {
				for (int z = 0; z < dimension; ++z) {
					matMap[x, z] = 0;

					// calculate the height
					if (x != 0) {
						if (z == 0) {
							height = heightMap[x - 1, z];
							acceleration = accelMap[x - 1, z];
						} else {
							height = (heightMap[x - 1, z] + heightMap[x, z - 1]) / 2;
							acceleration = (accelMap[x - 1, z] + accelMap[x, z - 1]) / 2;
						}
					}
					float edgeDistance = Mathf.Max(Mathf.Abs(dimension / 2 - x - 10), Mathf.Abs(dimension / 2 - z - 10));
					float edgeDistancePercent = 1 - edgeDistance / (dimension / 2);
					float percent;
					if (edgeDistancePercent < 0.2)
						percent = height / (dimension * 0.6f) - 0.4f;
					else
						percent = height / (dimension * 0.4f);
					float roughness = maxChange + 0.2f * (1 - edgeDistancePercent);
					acceleration += UnityEngine.Random.Range(-roughness * percent, roughness * (1 - percent));
					acceleration = Mathf.Min(Mathf.Max(acceleration, -roughness * 7), roughness * 7);
					height = Mathf.Min(Mathf.Max(height + acceleration, 0), dimension);
					heightMap[x, z] = height;
					accelMap[x, z] = acceleration;
				}
			}
			head.setToHeightmap(maxDetail, 0, 0, 0, ref heightMap, matMap, this);

			// generate trees
			//for (int x = 0; x < dimension; ++x) {
			//	for (int z = 0; z < dimension; ++z) {
			//		if (Random.Range(Mathf.Abs(accelMap[x, z]) / treeSlopeTolerance, 1) < treeDensity) {
			//			GameObject tree = (GameObject)GameObject.Instantiate(trees);
			//			tree.transform.parent = transform;
			//			tree.transform.localPosition = new Vector3(x * size, heightMap[x, z] * size - 1.5f, z * size);
			//			++treeCount;
			//		}
			//	}
			//}
		}

		// public void saveData() {
		// 	BinaryFormatter b = new BinaryFormatter();
		// 	FileStream f = File.Create(Application.persistentDataPath + "/heightmap.dat");
		// 	b.Serialize(f, heightmap);
		// 	f.Close();
		// }

		public bool hasVoxelData() {
			return getHead() != null;
		}

		public bool hasRenderers() {
			lock(renderers) {
				return renderers.Count > 0;
			}
		}

		public void OnDrawGizmosSelected() {
			if (selectedMode == 0)
				return;
			if (drawGhostBrush && selectedMode == 1) {
				Ray mouseRay = HandleUtility.GUIPointToWorldRay(UnityEngine.Event.current.mousePosition);
				Gizmos.color = brushGhostColor;
				switch(selectedBrush) {
				case 0:
					Gizmos.DrawSphere(getBrushPoint(mouseRay), sphereBrushSize);
					break;
				case 1:
					Gizmos.DrawMesh(generateRectangleMesh(cubeBrushDimensions), getBrushPoint(mouseRay));
					break;
				}
			}
			if (maskDisplayAlpha > 0) {
				Gizmos.color = new Color(1, 0, 0, maskDisplayAlpha);
				foreach(VoxelMask mask in masks) {
					if (!mask.active)
						continue;
					Gizmos.DrawMesh(generateRectangleMesh(new Vector3(baseSize, 0, baseSize)), transform.TransformPoint(baseSize /2, mask.yPosition /voxelSize(), baseSize /2));
				}
				Gizmos.color = Color.gray;
			}
		}

		public static RaycastHit getRayCollision(Ray ray) {
			RaycastHit firstHit = new RaycastHit();
			firstHit.distance = float.PositiveInfinity;
			foreach(RaycastHit hit in Physics.RaycastAll(ray)) {
				if (hit.distance < firstHit.distance) {
					firstHit = hit;
				}
			}
			return firstHit;
		}

	    public Vector3 getBrushPoint(Ray mouseLocation) {
			Vector3 point = getRayCollision(mouseLocation).point;
	        if (gridEnabled) {
	            point = transform.InverseTransformPoint(point);
	            double halfGrid = gridSize / 2.0;
	            Vector3 mod = new Vector3(point.x %gridSize, point.y %gridSize, point.z %gridSize);
				point.x += (mod.x > halfGrid) ? gridSize -mod.x: -mod.x;
				point.y += (mod.y > halfGrid) ? gridSize -mod.y: -mod.y;
	            point.z += (mod.z > halfGrid) ? gridSize -mod.z: -mod.z;
	            point = transform.TransformPoint(point);
	        }
	        return point;
	    }

		protected Mesh generateRectangleMesh(Vector3 scale) {
			Mesh mesh = new Mesh();
            scale = scale / 2;
            Vector3[] vertices = new Vector3[] {
				new Vector3(-scale.x, -scale.y, -scale.z),
				new Vector3( scale.x, -scale.y, -scale.z),
				new Vector3(-scale.x,  scale.y, -scale.z),
				new Vector3( scale.x,  scale.y, -scale.z),
				new Vector3(-scale.x, -scale.y,  scale.z),
				new Vector3( scale.x, -scale.y,  scale.z),
				new Vector3(-scale.x,  scale.y,  scale.z),
				new Vector3( scale.x,  scale.y,  scale.z),
			};
			mesh.vertices = vertices;
			mesh.normals = new Vector3[vertices.Length];
			mesh.triangles = new int[] {
				0, 1, 5,
				5, 4, 0,
				2, 7, 3,
				7, 2, 6,
				0, 3, 1,
				2, 3, 0,
				4, 5, 7,
				6, 4, 7,
				1, 3, 5,
				5, 3, 7,
				0, 4, 2,
				4, 6, 2,
			};
			return mesh;
		}

	}
}
