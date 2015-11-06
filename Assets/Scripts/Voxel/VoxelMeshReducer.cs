using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Vox {
	public class VoxelMeshReducer {

		public static void reduce(Vector3[] vertices, int[] triangles, float maxMergeCost) {

			/////////////////////////////////////////////////////
			//  calculate merge costs and assemble merge list  //
			/////////////////////////////////////////////////////
			HashSet<MergeOperation> mergeSet = new HashSet<MergeOperation>();
			Vector3[] triangleNormals = new Vector3[triangles.Length /3];
			List<int>[] vertexNeighbors = new List<int>[vertices.Length];
			List<int>[] vertexTriangles = new List<int>[vertices.Length];

			// initialize vertexNeighbors and vertexTriangles
			for (int i = 0; i<vertices.Length; ++i) {
				vertexNeighbors[i] = new List<int>(6);
				vertexTriangles[i] = new List<int>(6);
			}

			// populate triangleNormals, vertexNeighbors, and vertexTriangles
			for (int i=0; i<triangles.Length; i+=3) {
				int[] verts = new int[] {triangles[i], triangles[i+1], triangles[i+2]};
				Vector3 surfNorm = Vector3.Cross(vertices[verts[1]] - vertices[verts[0]], vertices[verts[2]] - vertices[verts[0]]).normalized;
				triangleNormals[i /3] = surfNorm;
				for (int j=0; j<3; ++j) {
					addNeighbors(vertexNeighbors[verts[i]], verts, i);
					vertexTriangles[verts[i]].Add(i /3);
				}
			}

			// populate merge list
			for(int i=0; i<vertices.Length; ++i) {
				List<int> neighboringTriangles = vertexNeighbors[i];
				float minMergeCost = float.PositiveInfinity;
				int mergetarget = i;

				// iterate through neighbors
				foreach(int neighbor in vertexNeighbors[i]) {
					float mergeCost = calculateMergeCost(i, neighbor, neighboringTriangles, triangles, triangleNormals);
					if (mergeCost < minMergeCost) {
						minMergeCost = mergeCost;
						mergetarget = neighbor;
					}
				}

				// add to merge list if cost is low enough
				if (minMergeCost <= maxMergeCost)
					mergeSet.Add(new MergeOperation(i, mergetarget, minMergeCost));
			}



			////////////////////////////////////
			//  perform merges in merge list  //
			////////////////////////////////////

			// construct new vertex array

		}

		protected static void addNeighbors(List<int> vertexNeighbors, int[] verticesToAdd, int selfIndexInVerticesToAdd) {
			for(int i=0; i<verticesToAdd.Length; ++i)
				if (i != selfIndexInVerticesToAdd &&
					!vertexNeighbors.Contains(verticesToAdd[i]))
						vertexNeighbors.Add(verticesToAdd[i]);
		}

		protected static float calculateMergeCost(int victom, int targetNeighbor, List<int> neighboringTriangles, int[] triangles, Vector3[] triangleNormals) {
			List<int> sharedTriangles = new List<int>(2);
			List<int> unsharededTriangles = new List<int>(4);

			// populate shared and unshared triangle lists
			foreach (int triangleIndex in neighboringTriangles) {
				int[] verts = new int[] { triangles[triangleIndex *3], triangles[triangleIndex *3 +1], triangles[triangleIndex *3 +2] };
				if (System.Array.IndexOf(verts, targetNeighbor) < 0)
					unsharededTriangles.Add(triangleIndex);
				else
					sharedTriangles.Add(triangleIndex);
			}

			// calculate total cost
			float totalCost = 0;
			foreach(int unsharedTriangle in unsharededTriangles) {
				float cost = float.PositiveInfinity;
				foreach(int sharedTriangle in sharedTriangles)
					cost = Mathf.Min(cost, 1 -Vector3.Dot(triangleNormals[sharedTriangle], triangleNormals[unsharedTriangle]));
				totalCost += cost;
			}
			return totalCost;
		}

		protected struct MergeOperation {
			int victomVertex;
			int targetVertex;
			float mergeCost;

			public MergeOperation(int victomVertex, int targetVertex, float mergeCost) {
				this.victomVertex = victomVertex;
				this.targetVertex = targetVertex;
				this.mergeCost = mergeCost;
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
	}
}