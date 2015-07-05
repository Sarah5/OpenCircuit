using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

namespace Vox {

	[AddComponentMenu("Scripts/Voxel/VoxelTerrain")]
	[ExecuteInEditMode]
	public class VoxelTerrain : VoxelTree {

		public string data_file;

		public Texture2D heightmap;
		public Texture2D materialMap;

		void Awake() {
			if (Application.isPlaying)
				initializeHeightmap();
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

		public virtual void loadData() {
			int voxels = heightmap.height;

			float[,] map = new float[voxels, voxels];
			for (int i = 0; i < heightmap.height; i++) {
				for (int j = 0; j < heightmap.width; j++) {
					Color pix = heightmap.GetPixel((heightmap.height - 1) - i, j);
					map[j, i] = ((pix.r + pix.g + pix.b) / 3.0f) * voxels;
				}
			}
			if (materialMap == null)
				head.setToHeightmap(maxDetail, 0, 0, 0, ref map, 0, this);
			else {
				byte[,] matMap = new byte[voxels, voxels];
				for (int i = 0; i < materialMap.height; i++) {
					for (int j = 0; j < materialMap.width; j++) {
						Color pix = materialMap.GetPixel((materialMap.height - 1) - i, j);
						matMap[j, i] = (byte)(((pix.r + pix.g + pix.b) / 3.0f) * voxelMaterials.Length);
					}
				}
				head.setToHeightmap(maxDetail, 0, 0, 0, ref map, matMap, this);
			}
		}

		public void saveData() {
			BinaryFormatter b = new BinaryFormatter();
			FileStream f = File.Create(Application.persistentDataPath + "/heightmap.dat");
			b.Serialize(f, heightmap);
			f.Close();
		}
	}

}