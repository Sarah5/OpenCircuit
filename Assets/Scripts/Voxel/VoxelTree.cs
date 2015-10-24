using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace Vox {

	[ExecuteInEditMode]
	[System.Serializable]
	public class RendererDict : SerializableDictionary<VoxelIndex, VoxelRenderer> { }

	[AddComponentMenu("")]
	[ExecuteInEditMode]
	public class VoxelTree : MonoBehaviour, ISerializationCallbackReceiver {

		public const ulong FILE_FORMAT_VERSION = 1;

		// basic stats
		public float baseSize = 128;
		public byte maxDetail = 7;
		public byte isoLevel = 127;
		public float lodDetail = 1;
		public bool useLod = false;
		public GameObject trees;
		public float treeDensity = 0.02f;
		public float treeSlopeTolerance = 5;
		[HideInInspector]
		public Camera cam;
		public float curLodDetail = 10f;
		public VoxelSubstance[] voxelSubstances;
		public VoxelMask[] masks;
		public bool createColliders = true;
		public bool useStaticMeshes = true;
		public bool saveMeshes = false;


		// performance stats
//		private int treeCount = 0;

		// voxel data
		[HideInInspector]
		public VoxelBlock head;
		[HideInInspector]
		public float[] sizes;
		[System.NonSerialized]
		public RendererDict renderers = new RendererDict();
		public byte[] voxelData = new byte[0];
		[System.NonSerialized]
		public bool dirty = true;
		[System.NonSerialized]
		private Queue<VoxelJob> jobQueue = new Queue<VoxelJob>(100);
		private Vector3 localCamPosition;
		[System.NonSerialized]
		private int updateCheckJobs;
		[System.NonSerialized]
		private bool generationPaused = false;
		[System.NonSerialized]
		private bool rebakedLighting = false;



		// test values
		private int updateCounter = 0;

		public virtual void initialize() {

//			float startTime = Time.realtimeSinceStartup;

			// setup lookup tables, etc.
			sizes = new float[maxDetail + 1];
			float s = baseSize;
			for (int i = 0; i <= maxDetail; ++i) {
				sizes[i] = s;
				s /= 2;
			}
			updateCheckJobs = 0;

			// initialize voxels
			head = new VoxelBlock();
//			genData(0);

//			float endTime = Time.realtimeSinceStartup;


//			print("Voxel Gen time:                   " + (endTime - startTime));
//			print("Average Voxel Opacity:            " + head.averageOpacity());
//			print("Total Voxel Blocks:               " + VoxelHolder.blockCount);
//			print("Total Tree Count:                 " + treeCount);
//			print("Renderer Count:                   " + VoxelRenderer.rendCount);
//			print("Duplicate Triangle Count:         " + VoxelRenderer.duplicateTriangleCount);
		}

		public void Update() {
			if (Application.isPlaying) {
				if (updateCounter == 0) {
					if (useLod) {
						if (curLodDetail < lodDetail) {
							if (lodDetail - curLodDetail < 0.1f) {
								if (lodDetail - curLodDetail > -0.1f)
									curLodDetail = lodDetail;
								else
									curLodDetail -= 0.1f;
							} else
								curLodDetail += 0.1f;
						}
//						updateLocalCamPosition();
					}
					if (updateCheckJobs < 1)
						enqueueCheck(new UpdateCheckJob(head, this, 0));
				}
				updateCounter = (updateCounter + 1) % 2;
			}
			applyQueuedMeshes();
			if (generationPaused) {
				if (VoxelThread.getJobCount() < 1 && jobQueue.Count < 1) {
					//if (!rebakedLighting) {
					//	UnityEditor.Lightmapping.Bake();
					//	rebakedLighting = true;
					//} else if (!UnityEditor.Lightmapping.isRunning) {
						generationPaused = false;
						Time.timeScale = 1;
					//}
				}
			}
		}

		public void enqueueCheck(VoxelJob job) {
			VoxelThread.enqueueUpdate(job);
		}

		public void enqueueUpdate(VoxelJob job) {
			VoxelThread.enqueueUpdate(job);
		}

		public void applyQueuedMeshes() {
			lock (this) {
				while (jobQueue.Count > 0) {
					jobQueue.Dequeue().execute();
				}
			}
		}

		public float subtractSphere(Vector3 worldLocation, float radius) {
			new SphereModifier(this, worldLocation, radius, new Voxel(0, 0), true);
			return 0;
		}

		public float getLodDetail() {
			return curLodDetail;
		}

		public VoxelBlock getHead() {
			return head;
		}

//		public void updateLocalCamPosition() {
//			lock(this) {
//				cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
//			}
//			localCamPosition = transform.TransformPoint(cam.transform.position);
//		}

		public Vector3 getLocalCamPosition() {
//			lock(this) {
//				return localCamPosition;
//			}
			return Vector3.zero;
		}

		public VoxelUpdateInfo getBaseUpdateInfo() {
			return new VoxelUpdateInfo(sizes[0], head, this);
		}

		public float voxelSize() {
			return sizes[maxDetail];
		}

		public Voxel[,,] getArray(int xMin, int yMin, int zMin, int xMax, int yMax, int zMax) {
			Voxel[,,] array = new Voxel[xMax -xMin, yMax -yMin, zMax -zMin];
			head.putInArray(maxDetail, ref array, 0, 0, 0, xMin, yMin, zMin, xMax, yMax, zMax);
			return array;
		}

		internal void addUpdateCheckJob() {
			++updateCheckJobs;
		}

		internal void removeUpdateCheckJob() {
			--updateCheckJobs;
		}

		public void clearRenderers() {
			lock(renderers) {
				while(renderers.Count > 0) {
					Dictionary<VoxelIndex, VoxelRenderer>.ValueCollection.Enumerator e = renderers.Values.GetEnumerator();
					e.MoveNext();
					e.Current.clear();
				}
			}
			List<Transform> children = new List<Transform>(transform.childCount);
			foreach(Transform child in transform) {
				if ((child.hideFlags & HideFlags.HideInHierarchy) != 0)
					children.Add(child);
			}
			foreach(Transform child in children) {
				GameObject.DestroyImmediate(child.gameObject);
			}
//			foreach(MeshCollider collider in GetComponents<MeshCollider>()) {
//				if ((collider.hideFlags & HideFlags.HideInInspector) != 0)
//					GameObject.DestroyImmediate(collider);
//			}
		}

		public void generateRenderers() {
			clearRenderers();
			//updateLocalCamPosition();
			enqueueCheck(new UpdateCheckJob(head, this, 0));
		}

		public void wipe() {
			clearRenderers();
			if (head != null) {
				head = null;
			}
			dirty = true;
		}

		//public void OnGUI() {
		//	GUI.Label(new Rect(0, 200, 200, 20), "Voxel Triangle Count: " + VoxelRenderer.triangleCount);
		//	GUI.Label(new Rect(0, 220, 200, 20), "Voxel Vertex Count: " + VoxelRenderer.vertexCount);
		//	GUI.Label(new Rect(0, 240, 200, 20), "Voxel Duplicate Triangle Count: " + VoxelRenderer.duplicateTriangleCount);
		//}

		public void OnBeforeSerialize() {
			lock(this) {
				if (voxelData.Length < 1 || dirty || head == null) {
					dirty = false;
					voxelData = new byte[0];
					if (getHead() != null) {
						MemoryStream stream = new MemoryStream();
						BinaryWriter writer = new BinaryWriter(stream);
						getHead().serialize(writer);
						voxelData = stream.ToArray();
						stream.Close();
					}
				}
			}
		}

		public void OnAfterDeserialize() {
			//			clearRenderers();
			//print("Deserializing");
			lock(this) {
				if (voxelData.Length > 0) {
					MemoryStream stream = new MemoryStream(voxelData);
					BinaryReader reader = new BinaryReader(stream);
					head = (VoxelBlock)VoxelHolder.deserialize(reader);
					stream.Close();
				}

				// relink renderers
				//relinkRenderers();
				jobQueue.Enqueue(new LinkRenderersJob(this));
			}
		}

		public Dictionary<VoxelIndex, List<GameObject>> findRendererObjects() {
			Dictionary<VoxelIndex, List<GameObject>> meshes = new Dictionary<VoxelIndex, List<GameObject>>();
			foreach (Transform child in transform) {
				VoxelMeshObject meshObject = child.GetComponent<VoxelMeshObject>();
				if (meshObject == null)
					continue;
				List<GameObject> objects;
				//if (meshObject.index == null) {
				//	print("wow");
				//	continue;
				//}
				//print("Found valid child");
				meshes.TryGetValue(meshObject.index, out objects);
				if (objects == null) {
					objects = new List<GameObject>();
					meshes[meshObject.index] = objects;
				}
				objects.Add(meshObject.gameObject);
			}
			return meshes;
		}

		public void relinkRenderers() {
			relinkRenderers(findRendererObjects());
		}

		public void relinkRenderers(Dictionary<VoxelIndex, List<GameObject>> meshes) {
			lock(this) {
				//print("Start Renderers: " + renderers.Count);
				foreach (VoxelIndex index in meshes.Keys) {
					List<GameObject> objects = meshes[index];
					//print("Mesh object count: " + objects.Count);
					VoxelRenderer rend;
					renderers.TryGetValue(index, out rend);
					if (rend == null) {
						rend = new VoxelRenderer(index, this);
						renderers[index] = rend;
					//} else {
					//	print("already had renderer");
					}
					VoxelHolder block = head.get(index);
					if (block is VoxelBlock) {
						//print("linking");
						((VoxelBlock)block).renderer = rend;
					//} else {
					//	print("NOT BLOCK!");
					}
					rend.obs = objects.ToArray();
				}
				//print("End Renderers: " + renderers.Count);
			}
		}

		public bool import(string fileName) {
			clearRenderers();
			Stream stream = File.OpenRead(fileName);
			BinaryReader reader = new BinaryReader(stream);
			ulong fileFormatVersion = reader.ReadUInt64();
			if (fileFormatVersion != FILE_FORMAT_VERSION) {
				stream.Close ();
				print("Wrong voxel file format version: " +fileFormatVersion +", should be " +FILE_FORMAT_VERSION);
				return false;
			} else {
				head = (VoxelBlock)VoxelHolder.deserialize(reader);
				dirty = true;
				stream.Close();
				return true;
			}
		}

		public void export(string fileName) {
			if (getHead() != null) {
				Stream stream = File.Create(fileName);
				BinaryWriter writer = new BinaryWriter(stream);
				writer.Write(FILE_FORMAT_VERSION);
				getHead().serialize(writer);
				stream.Close();
			}
		}

		public bool generating() {
			return VoxelThread.getJobCount() > 0 || jobQueue.Count > 0;
		}

		public void pauseForGeneration() {
			generationPaused = true;
			rebakedLighting = false;
			Time.timeScale = 0;
		}

		internal void enqueueMeshApply(VoxelJob job) {
			jobQueue.Enqueue(job);
		}

		internal void enqueueJob(VoxelJob job) {
			jobQueue.Enqueue(job);
		}
	}

}
