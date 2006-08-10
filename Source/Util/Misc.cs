namespace NxBRE.Util
{
	using System;
	using System.Collections;
	
	/// <summary>Misc NxBRE utilities.</summary>
	/// <author>David Dossot</author>
	public abstract class Misc {
		private Misc() {}
		
		/// <summary>
		/// An empty read-only IDictionary.
		/// </summary>
		public static readonly IDictionary EMPTY_DICTIONARY = new ReadOnlyHashtable();
		private class ReadOnlyHashtable : Hashtable {
			public override void Add(object key, object value) { throw new NotImplementedException(); }
			public override bool IsReadOnly {get {return true;} }
		}
		
		/// <summary>
		/// An empty read-only IList.
		/// </summary>
		public static readonly IList EMPTY_LIST = new ReadOnlyArrayList();
		private class ReadOnlyArrayList : ArrayList {
			public override int Add(object key) { throw new NotImplementedException(); }
			public override bool IsReadOnly {get {return true;} }
		}

		///<summary>
		/// Determines if two ArrayList are intersecting, i.e. have an object in common.
		///</summary>
		/// <param name="collectionA">One of the two collections to evaluate.</param>
		/// <param name="collectionB">The other of the two collections to evaluate.</param>
		/// <returns>True if at least one object is found in both collections, otherwise false.</returns>
		/// <remarks>
		/// For performance reasons, it iterates on the smallest collection and use contains on the other.
		/// </remarks>
		public static bool AreIntersecting(ArrayList collectionA, ArrayList collectionB) {
			if (collectionA.Count > collectionB.Count) {
				foreach(object o in collectionB)
					if (collectionA.Contains(o))
						return true;
			}
			else {
				foreach(object o in collectionA)
					if (collectionB.Contains(o))
						return true;
			}
			return false;
		}

		/// <summary>
		/// Outputs the content of an ArrayList in a string.
		/// </summary>
		/// <param name="objects">The ArrayList to output.</param>
		/// <returns>The content of the ArrayList in a string.</returns>
		[Obsolete("This method has been deprecated. Please use Misc.IListToString instead.")]
		public static string ArrayListToString(ArrayList objects) {
			return ArrayListToString(objects, String.Empty);	
		}

		/// <summary>
		/// Outputs the content of an ArrayList in a string.
		/// </summary>
		/// <param name="objects">The ArrayList to output.</param>
		/// <param name="margin">A left margin string to place before each value.</param>
		/// <returns>The content of the ArrayList in a string.</returns>
		[Obsolete("This method has been deprecated. Please use Misc.IListToString instead.")]
		public static string ArrayListToString(ArrayList objects, string margin) {
			return IListToString(objects, margin);
		}

		/// <summary>
		/// Outputs the content of an IList in a string.
		/// </summary>
		/// <param name="objects">The IList to output.</param>
		/// <returns>The content of the IList in a string.</returns>
		public static string IListToString(IList objects) {
			return IListToString(objects, String.Empty);	
		}

		/// <summary>
		/// Outputs the content of an IList in a string.
		/// </summary>
		/// <param name="objects">The IList to output.</param>
		/// <param name="margin">A left margin string to place before each value.</param>
		/// <returns>The content of the IList in a string.</returns>
		public static string IListToString(IList objects, string margin) {
			string result = "(";
			
			if (objects != null) {
				bool first = true;
				foreach (object o in objects) {
					string stringContent;
					
					if (o is IDictionary) stringContent = IDictionaryToString((IDictionary)o);
					else if (o is IList) stringContent = IListToString((IList)o);
					else stringContent = o.ToString();
					
					if (margin != String.Empty) result = result + margin + stringContent + "\n";
					else result = result + (first?String.Empty:",") + stringContent;
					
					first = false;
				}
			}
				
			return result + ")";	
		}
		
		/// <summary>
		/// Outputs the content of an IDictionary in a string.
		/// </summary>
		/// <param name="map">The IDictionary to output.</param>
		/// <returns>The content of the IDictionary in a string.</returns>
		public static string IDictionaryToString(IDictionary map) {
			string result = "[";

			if (map != null) {
				bool first = true;
				foreach(object key in map.Keys) {
					object content = map[key];
					string stringContent;
					
					if (content is IDictionary) stringContent = IDictionaryToString((IDictionary)content);
					else if (content is IList) stringContent = IListToString((IList)content);
					else stringContent = content.ToString();
					
					result = result + (first?String.Empty:";") + key + "=" + stringContent;
					
					first = false;
				}
			}
				
			return result + "]";
		}
		
		/// <summary>
		/// Deep clones an IDictionary.
		/// If a value element is an IDictionary or IList, DeepClone will be called recursively.
		/// </summary>
		/// <param name="source">The source collection.</param>
		/// <param name="cloneContent">If true, the content of the collections will be cloned if it supports ICloneable ; else the same object will be used</param>
		/// <returns>A clone of the source collection.</returns>
		public static IDictionary DeepClone(IDictionary source, bool cloneContent) {
			IDictionary result = (IDictionary)Activator.CreateInstance(source.GetType());
			
			foreach(object key in source.Keys) {
				object val = source[key];
				if (val is IDictionary) result.Add(key, DeepClone((IDictionary)val, cloneContent));
				else if (val is IList) result.Add(key, DeepClone((IList)val, cloneContent));
				else if ((cloneContent) && (val is ICloneable)) result.Add(key, ((ICloneable)val).Clone());
				else result.Add(key, val);
			}
			
			return result;
		}
		
		/// <summary>
		/// Deep clones an IList.
		/// If an element is an IDictionary or IList, DeepClone will be called recursively.
		/// </summary>
		/// <param name="source">The source collection.</param>
		/// <param name="cloneContent">If true, the content of the collections will be cloned if it supports ICloneable ; else the same object will be used</param>
		/// <returns>A clone of the source collection.</returns>
		public static IList DeepClone(IList source, bool cloneContent) {
			IList result = (IList)Activator.CreateInstance(source.GetType());
			
			foreach(object val in source) {
				if (val is IDictionary) result.Add(DeepClone((IDictionary)val, cloneContent));
				else if (val is IList) result.Add(DeepClone((IList)val, cloneContent));
				else if ((cloneContent) && (val is ICloneable)) result.Add(((ICloneable)val).Clone());
				else result.Add(val);
			}
			
			return result;
		}
		

		
		/// <summary>
		/// Returns either the string value if o is a string, else a string representation of its hashcode.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static string GetStringHashcode(object o) {
			return (o is string)?(string)o:o.GetHashCode().ToString();
		}
		
	}
}
