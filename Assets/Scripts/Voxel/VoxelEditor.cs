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
		
		public bool useHeightmap;
		public Texture2D[] heightmaps;
		public byte[] heightmapSubstances;
		
		public string data_file;
		
		public Texture2D heightmap;
		public Texture2D substanceMap;

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

		public void init() {
			initialize();
			if (useHeightmap) {
//				initializeHeightmap();
				loadData();
			} else {
//				initialize();
				genData(0);
			}
		}

		public void loadData() {
			int voxels = heightmap.height;

			for (int index = 0; index < heightmaps.Length; ++index ) {
				float[,] map = new float[voxels, voxels];
				for (int i = 0; i < heightmaps[index].height; i++) {
					for (int j = 0; j < heightmaps[index].width; j++) {
						Color pix = heightmaps[index].GetPixel((heightmap.height - 1) - i, j);
						map[j, i] = ((pix.r + pix.g + pix.b) / 3.0f) * voxels;
					}
				}
				head.setToHeightmap(maxDetail, 0, 0, 0, ref map, heightmapSubstances[index], this);
			}
		}
		
//		public void initializeHeightmap() {
//			
//			head = new VoxelBlock();
//			maxDetail = (byte)Mathf.Log(heightmap.height, 2);
//			
//			sizes = new float[maxDetail + 1];
//			float s = BaseSize;
//			for (int i = 0; i <= maxDetail; ++i) {
//				sizes[i] = s;
//				s /= 2;
//			}
//			cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Camera>();
////			loadData();
//			
////			updateLocalCamPosition();
//			
////			enqueueCheck(new UpdateCheckJob(head, this, 0));
//		}
		
		public void saveData() {
			BinaryFormatter b = new BinaryFormatter();
			FileStream f = File.Create(Application.persistentDataPath + "/heightmap.dat");
			b.Serialize(f, heightmap);
			f.Close();
		}

		public bool hasVoxelData() {
			return getHead() != null;
		}

		public bool hasRenderers() {
			lock(renderers) {
				return renderers.Count > 0;
			}
		}

		public void OnDrawGizmosSelected() {
			if (drawGhostBrush && selectedMode == 1) {
				Ray mouseRay = HandleUtility.GUIPointToWorldRay(UnityEngine.Event.current.mousePosition);
				Gizmos.color = brushGhostColor;
				switch(selectedBrush) {
				case 0:
					Gizmos.DrawSphere(getRayCollision(mouseRay).point, sphereBrushSize);
					break;
				case 1:
					Gizmos.DrawMesh(generateRectangleMesh(cubeBrushDimensions /2), getRayCollision(mouseRay).point);
					break;
				}
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

		protected Mesh generateRectangleMesh(Vector3 scale) {
			Mesh mesh = new Mesh();
			Vector3[] vertices = new Vector3[] {
				new Vector3(-1, -1, -1),
				new Vector3( 1, -1, -1),
				new Vector3(-1,  1, -1),
				new Vector3( 1,  1, -1),
				new Vector3(-1, -1,  1),
				new Vector3( 1, -1,  1),
				new Vector3(-1,  1,  1),
				new Vector3( 1,  1,  1),
			};
			for(int i=0; i<vertices.Length; ++i) {
				vertices[i] = new Vector3(vertices[i].x *scale.x, vertices[i].y *scale.y, vertices[i].z *scale.z);
			}
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