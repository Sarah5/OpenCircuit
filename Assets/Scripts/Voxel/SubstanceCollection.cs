using UnityEngine;
using System.Collections;
using System;

namespace Vox {

	// NOT thread-safe
	public class SubstanceCollection: IEquatable<SubstanceCollection> {

		private SortedList substances;
		private int hashCode;
		private bool hashUpdated;

		public SubstanceCollection() {
			substances = new SortedList(3);
			hashUpdated = false;
		}
		
		public int size {
			get { return substances.Count; }
		}

		public void add(byte substance) {
			hashUpdated = false;
			if (substances.Contains(substance))
				return;
			substances.Add(substance, null);
		}

		public byte[] getSubstances() {
			byte[] subs = new byte[size];
			for(int i=0; i<substances.Count; ++i)
				subs[i] = (byte)substances.GetKey(i);
			return subs;
		}

		public byte getSubstanceRelativeIndex(byte substance) {
			return (byte) substances.IndexOfKey(substance);
		}

//		public bool Equals(Tuple<A, B> tuple) {
//
//		}

		public override bool Equals(object obj) {
			if (obj.GetType() != GetType())
				return false;
//			MonoBehaviour.print("types equal");
			return Equals ((SubstanceCollection) obj);
		}
		
		public bool Equals(SubstanceCollection other) {
			if (other.size != size)
				return false;
//			MonoBehaviour.print("sizes equal");
			for(int i=0; i<size; ++i) {
				if ((byte)substances.GetKey(i) != (byte)other.substances.GetKey(i))
					return false;
			}
//			MonoBehaviour.print("values equal");
			return true;
		}

		public override int GetHashCode() {
			if (!hashUpdated) {
				hashCode = 13;
				foreach(object ob in substances.Keys)
					hashCode = hashCode *7 +ob.GetHashCode();
				hashUpdated = true;
			}
			return hashCode;
		}

		public override string ToString ()
		{
			string val = "";
			foreach(object o in substances.Keys)
				val += o +" ";
			return val;
		}
	}
}