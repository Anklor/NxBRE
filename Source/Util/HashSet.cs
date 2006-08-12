namespace NxBRE.Util {
	using System.Collections.Generic;
	
	/// <summary>
	/// A HashSet is collection that contains only unique items and uses their hash code to determine their unicity.
	/// </summary>
	public class HashSet<T>:ICollection<T> {
		private IDictionary<T, T> content = new Dictionary<T, T>();
		
		public int Count {
			get {
				return content.Count;
			}
		}
		
		public bool IsReadOnly {
			get {
				return false;
			}
		}
		
		public void Add(T item) {
			content.Add(item, item);
		}
		
		public void Clear() {
			content.Clear();
		}
		
		public bool Contains(T item) {
			return content.ContainsKey(item);
		}
		
		public void CopyTo(T[] array, int arrayIndex) {
			content.Keys.CopyTo(array, arrayIndex);
		}
		
		public bool Remove(T item) {
			return content.Remove(item);
		}
		
		public IEnumerator<T> GetEnumerator()	{
			return content.Keys.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()	{
			return content.Keys.GetEnumerator();
		}
	}
}
