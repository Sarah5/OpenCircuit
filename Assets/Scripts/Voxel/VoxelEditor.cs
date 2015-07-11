using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace Vox {

	[AddComponentMenu("Scripts/Voxel/VoxelEditor")]
	public class VoxelEditor : VoxelTree {

		public bool useHeightmap;
		public Texture2D[] heightmaps;
		public byte[] heightmapMaterials;
		
		public string data_file;
		
		public Texture2D heightmap;
		public Texture2D materialMap;

		// editor data
		public int selectedMode = 0;
		public int selectedBrush = 0;
		public float sphereBrushSize = 1;
		public byte sphereBrushMaterial = 0;
		public Vector3 cubeBrushDimensions = new Vector3(1, 1, 1);
		public byte cubeBrushMaterial = 0;


		public void Awake() {
			if (Application.isPlaying) {
				init();
			}
		}

		public void init() {
			if (useHeightmap)
				initializeHeightmap();
			else
				initialize();
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
				head.setToHeightmap(maxDetail, 0, 0, 0, ref map, heightmapMaterials[index], this);
			}
		}
		
		public void initializeHeightmap() {
			
			head = new VoxelBlock();
			maxDetail = (byte)Mathf.Log(heightmap.height, 2);
			
			sizes = new float[maxDetail + 1];
			float s = BaseSize;
			for (int i = 0; i <= maxDetail; ++i) {
				sizes[i] = s;
				s /= 2;
			}
			cam = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Camera>();
			loadData();
			
			applyRenderers();
			
			updateLocalCamPosition();
			
			enqueueCheck(new UpdateCheckJob(head, this, 0));
		}
		
		public void saveData() {
			BinaryFormatter b = new BinaryFormatter();
			FileStream f = File.Create(Application.persistentDataPath + "/heightmap.dat");
			b.Serialize(f, heightmap);
			f.Close();
		}

		public bool hasGeneratedData() {
			return getHead() != null || chunks.Count > 0;
		}
	}
}