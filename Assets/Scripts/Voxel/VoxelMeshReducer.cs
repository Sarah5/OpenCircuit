using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Vox {
	public class VoxelMeshReducer {

		public static HashSet<int> reduce(ref Vector3[] vertices, ref int[] triangleVertices, float maxMergeCost) {
			
			HashSet<int> verticesMerged = new HashSet<int>();

			// TODO: change this to be a smarter exit condition
			for (int k = 0; k<20; ++k) {
				/////////////////////////////////////////////////////
				//  calculate merge costs and assemble merge list  //
				/////////////////////////////////////////////////////
				SortedList<MergeOperation, int> mergeSet = new SortedList<MergeOperation, int>(new MergeOperationComp());
				Vector3[] triangleNormals = new Vector3[triangleVertices.Length /3];
				List<int>[] vertexNeighbors = new List<int>[vertices.Length];
				List<int>[] vertexTriangles = new List<int>[vertices.Length];

				// initialize vertexNeighbors and vertexTriangles
				for (int i = 0; i<vertices.Length; ++i) {
					vertexNeighbors[i] = new List<int>(6);
					vertexTriangles[i] = new List<int>(6);
				}

				// populate triangleNormals, vertexNeighbors, and vertexTriangles
				for (int i = 0; i<triangleVertices.Length; i+=3) {
					int[] verts = new int[] { triangleVertices[i], triangleVertices[i+1], triangleVertices[i+2] };
					Vector3 surfNorm = Vector3.Cross(vertices[verts[1]] - vertices[verts[0]], vertices[verts[2]] - vertices[verts[0]]).normalized;
					triangleNormals[i /3] = surfNorm;
					for (int j = 0; j<3; ++j) {
						addNeighbors(ref vertexNeighbors[verts[j]], verts, j);
						vertexTriangles[verts[j]].Add(i /3);
					}
				}

				// populate merge list
				for (int i = 0; i<vertices.Length; ++i) {
					List<int> neighboringTriangles = vertexTriangles[i];
					if (neighboringTriangles.Count < 1)
						continue;

					// the following makes sure we don't change vertices on the edges of a mesh
					if (neighboringTriangles.Count != vertexNeighbors[i].Count)
						continue;

					float minMergeCost = float.PositiveInfinity;
					int mergetarget = i;

					// iterate through neighbors
					foreach (int neighbor in vertexNeighbors[i]) {
						float mergeCost = calculateMergeCost(i, neighbor, neighboringTriangles, triangleVertices, triangleNormals, vertices);
						if (mergeCost < minMergeCost) {
							minMergeCost = mergeCost;
							mergetarget = neighbor;
						}
					}

					// add to merge list if cost is low enough
					if (minMergeCost <= maxMergeCost)
						mergeSet.Add(new MergeOperation(i, mergetarget, minMergeCost), 0);
				}



				////////////////////////////////////
				//  perform merges in merge list  //
				////////////////////////////////////

				// check if there are no merges left to do
				if (mergeSet.Count < 1)
					break;

				HashSet<int> verticesNeighboringMerge = new HashSet<int>();
				HashSet<int> trianglesDropped = new HashSet<int>();
				foreach (MergeOperation op in mergeSet.Keys) {
					if (verticesNeighboringMerge.Contains(op.victomVertex))
						continue;
					//MonoBehaviour.print("Merging " + vertices[op.victomVertex] + " into " + vertices[op.targetVertex]);
					trianglesDropped.UnionWith(op.perform(vertexTriangles[op.victomVertex], ref triangleVertices));
					verticesNeighboringMerge.UnionWith(vertexNeighbors[op.victomVertex]);
					verticesMerged.Add(op.victomVertex);
					//break;
				}

				// cleanup triangle list
				int[] reducedTriangleVertices = new int[triangleVertices.Length - trianglesDropped.Count *3];
				int a = 0;
				for(int i=0; i<triangleVertices.Length; i+=3) {
					if (trianglesDropped.Contains(i /3))
						continue;
					reducedTriangleVertices[a] = triangleVertices[i];
					reducedTriangleVertices[a +1] = triangleVertices[i +1];
					reducedTriangleVertices[a +2] = triangleVertices[i +2];
					a += 3;
				}
				triangleVertices = reducedTriangleVertices;
			}

			// cleanup vertex list
			for(int i=vertices.Length-1; i>=0; --i) {
				//MonoBehaviour.print("Removed " +vertices[i]);
				if (verticesMerged.Contains(i)) {
					for (int k = 0; k < triangleVertices.Length; ++k) {
						if (triangleVertices[k] > i)
							--triangleVertices[k];
					}
				}
			}
			vertices = removeEntries(vertices, verticesMerged);
			return verticesMerged;
		}

		public static T[] removeEntries<T>(T[] array, HashSet<int> entriesToRemove) {
			T[] reducedArray = new T[array.Length - entriesToRemove.Count];
			int j = 0;
			for (int i = 0; i < array.Length; ++i) {
				if (entriesToRemove.Contains(i)) {
					continue;
				}
				reducedArray[j] = array[i];
				++j;
			}
			return reducedArray;
		}

		protected static void addNeighbors(ref List<int> vertexNeighbors, int[] verticesToAdd, int selfIndexInVerticesToAdd) {
			for(int i=0; i<verticesToAdd.Length; ++i)
				if (i != selfIndexInVerticesToAdd &&
					!vertexNeighbors.Contains(verticesToAdd[i]))
						vertexNeighbors.Add(verticesToAdd[i]);
		}

		protected static float calculateMergeCost(int victom, int targetNeighbor, List<int> neighboringTriangles, int[] triangleVertices, Vector3[] triangleNormals, Vector3[] vertices) {
			List<int> sharedTriangles = new List<int>(2);
			List<int> unsharedTriangles = new List<int>(4);

			// populate shared and unshared triangle lists
			foreach (int triangleIndex in neighboringTriangles) {
				int[] verts = new int[] { triangleVertices[triangleIndex *3], triangleVertices[triangleIndex *3 +1], triangleVertices[triangleIndex *3 +2] };
				if (System.Array.IndexOf(verts, targetNeighbor) < 0)
					unsharedTriangles.Add(triangleIndex);
				else
					sharedTriangles.Add(triangleIndex);
			}

			// calculate total cost
			float totalCost = 0;
			foreach(int unsharedTriangle in unsharedTriangles) {
				float cost = float.PositiveInfinity;
				foreach(int sharedTriangle in sharedTriangles)
					cost = Mathf.Min(cost, 1 -Vector3.Dot(triangleNormals[sharedTriangle], triangleNormals[unsharedTriangle]));
				totalCost += cost *cost;
			}

			// calculate cost for distance
			float distanceSquared = (vertices[victom] - vertices[targetNeighbor]).sqrMagnitude;
			if (totalCost > distanceSquared)
				totalCost = distanceSquared;

			return totalCost;
		}

		protected struct MergeOperation {
			public int victomVertex;
			public int targetVertex;
			public float mergeCost;

			public MergeOperation(int victomVertex, int targetVertex, float mergeCost) {
				this.victomVertex = victomVertex;
				this.targetVertex = targetVertex;
				this.mergeCost = mergeCost;
			}

			public List<int> perform(List<int> victomTriangles, ref int[] triangleVertices) {
				List<int> droppedTriangles = new List<int>(2);
				foreach (int triangleIndex in victomTriangles) {
					int[] verts = new int[] { triangleVertices[triangleIndex *3], triangleVertices[triangleIndex *3 +1], triangleVertices[triangleIndex *3 +2] };
					if (System.Array.IndexOf(verts, targetVertex) < 0) {
						triangleVertices[triangleIndex *3 +System.Array.IndexOf(verts, victomVertex)] = targetVertex;
					} else {
						droppedTriangles.Add(triangleIndex);
					}
				}
				return droppedTriangles;
			}

			public override bool Equals(object ob) {
				if (ob == null || GetType() != ob.GetType())
					return false;
				return this == (MergeOperation)ob;
			}

			public static bool operator==(MergeOperation m1, MergeOperation m2) {
				if (System.Object.ReferenceEquals(m1, m2))
					return true;
				if (((object)m1 == null) ^ ((object)m2 == null))
					return false;
				return (m1.victomVertex == m2.victomVertex && m1.targetVertex == m2.targetVertex);
			}

			public static bool operator!=(MergeOperation m1, MergeOperation m2) {
				return !(m1 == m2);
			}

			public override int GetHashCode() {
				long h = victomVertex ^ targetVertex;
				h = (h^0xdeadbeef) + (h<<4);
				h = h ^ (h>>10);
				h = h + (h<<7);
				return (int)(h ^ (h>>13));
			}
		}

		protected class MergeOperationComp: IComparer<MergeOperation> {
			public int Compare(MergeOperation a, MergeOperation b) {
				int comparison = a.mergeCost.CompareTo(b.mergeCost);
				return comparison == 0 ? a.victomVertex.CompareTo(b.victomVertex) : comparison;
			}
		}
	}
}