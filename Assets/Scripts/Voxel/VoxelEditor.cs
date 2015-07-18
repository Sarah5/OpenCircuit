using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace Vox {

	[AddComponentMenu("Scripts/Voxel/VoxelEditor")]
	public class VoxelEditor : VoxelTree {

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


		public void Awake() {
			if (Application.isPlaying && !hasRenderers()) {
				init();
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
	}
}