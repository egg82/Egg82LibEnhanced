using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace Egg82LibEnhanced.Utils {

	/// <summary>
	/// A dictionary object that allows rapid hash lookups using keys, but also
	/// maintains the key insertion order so that values can be retrieved by
	/// key index.
	/// </summary>
	/// <remarks>
	/// Similar to the way a DataColumn is indexed by column position and by column name, this
	/// advanced dictionary construct allows for a very natural and robust handling of indexed
	/// structured data.
	/// </remarks>
	public class LinkedDictionary<TKey, TValue> : IOrderedDictionary<TKey, TValue> {

		#region Fields/Properties

		private KeyedCollection2<TKey, KeyValuePair<TKey, TValue>> _keyedCollection;

		/// <summary>
		/// Gets or sets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key associated with the value to get or set.</param>
		public TValue this[TKey key] {
			get {
				return GetValue(key);
			}
			set {
				SetValue(key, value);
			}
		}

		/// <summary>
		/// Gets or sets the value at the specified index.
		/// </summary>
		/// <param name="index">The index of the value to get or set.</param>
		public TValue this[int index] {
			get {
				return GetItem(index).Value;
			}
			set {
				SetItem(index, value);
			}
		}

		
		/// <summary>
		/// Gets the number of items in the dictionary
		/// </summary>
		public int Count {
			get { return _keyedCollection.Count; }
		}

		/// <summary>
		/// Gets all the keys in the ordered dictionary in their proper order.
		/// </summary>
		public ICollection<TKey> Keys {
			get {
				return _keyedCollection.Select(x => x.Key).ToList();
			}
		}

		/// <summary>
		/// Gets all the values in the ordered dictionary in their proper order.
		/// </summary>
		public ICollection<TValue> Values {
			get {
				return _keyedCollection.Select(x => x.Value).ToList();
			}
		}

		/// <summary>
		/// Gets the key comparer for this dictionary
		/// </summary>
		public IEqualityComparer<TKey> Comparer {
			get;
			private set;
		}

		#endregion
		
		#region Constructors

		public LinkedDictionary() {
			Initialize();
		}

		public LinkedDictionary(IEqualityComparer<TKey> comparer) {
			Initialize(comparer);
		}

		public LinkedDictionary(IOrderedDictionary<TKey, TValue> dictionary) {
			if (dictionary == null) {
				throw new ArgumentNullException("dictionary");
			}

			Initialize();
			foreach (KeyValuePair<TKey, TValue> pair in dictionary) {
				_keyedCollection.Add(pair);
			}
		}

		public LinkedDictionary(IOrderedDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) {
			if (dictionary == null) {
				throw new ArgumentNullException("dictionary");
			}

			Initialize(comparer);
			foreach (KeyValuePair<TKey, TValue> pair in dictionary) {
				_keyedCollection.Add(pair);
			}
		}

		public LinkedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> items) {
			if (items == null) {
				throw new ArgumentNullException("items");
			}

			Initialize();
			foreach (KeyValuePair<TKey, TValue> pair in items) {
				_keyedCollection.Add(pair);
			}
		}

		public LinkedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> items, IEqualityComparer<TKey> comparer) {
			if (items == null) {
				throw new ArgumentNullException("items");
			}

			Initialize(comparer);
			foreach (KeyValuePair<TKey, TValue> pair in items) {
				_keyedCollection.Add(pair);
			}
		}

		#endregion
		
		#region Methods

		private void Initialize(IEqualityComparer<TKey> comparer = null) {
			this.Comparer = comparer;
			if (comparer != null) {
				_keyedCollection = new KeyedCollection2<TKey, KeyValuePair<TKey, TValue>>(x => x.Key, comparer);
			}
			else {
				_keyedCollection = new KeyedCollection2<TKey, KeyValuePair<TKey, TValue>>(x => x.Key);
			}
		}

		/// <summary>
		/// Adds the specified key and value to the dictionary.
		/// </summary>
		/// <param name="key">The key of the element to add.</param>
		/// <param name="value">The value of the element to add.  The value can be null for reference types.</param>
		public void Add(TKey key, TValue value) {
			_keyedCollection.Add(new KeyValuePair<TKey, TValue>(key, value));
		}

		/// <summary>
		/// Removes all keys and values from this object.
		/// </summary>
		public void Clear() {
			_keyedCollection.Clear();
		}

		/// <summary>
		/// Inserts a new key-value pair at the index specified.
		/// </summary>
		/// <param name="index">The insertion index.  This value must be between 0 and the count of items in this object.</param>
		/// <param name="key">A unique key for the element to add</param>
		/// <param name="value">The value of the element to add.  Can be null for reference types.</param>
		public void Insert(int index, TKey key, TValue value) {
			_keyedCollection.Insert(index, new KeyValuePair<TKey, TValue>(key, value));
		}

		/// <summary>
		/// Gets the index of the key specified.
		/// </summary>
		/// <param name="key">The key whose index will be located</param>
		/// <returns>Returns the index of the key specified if found.  Returns -1 if the key could not be located.</returns>
		public int IndexOf(TKey key) {
			if (_keyedCollection.Contains(key)) {
				return _keyedCollection.IndexOf(_keyedCollection[key]);
			}
			else {
				return -1;
			}
		}

		/// <summary>
		/// Determines whether this object contains the specified value.
		/// </summary>
		/// <param name="value">The value to locate in this object.</param>
		/// <returns>True if the value is found.  False otherwise.</returns>
		public bool ContainsValue(TValue value) {
			return this.Values.Contains(value);
		}

		/// <summary>
		/// Determines whether this object contains the specified value.
		/// </summary>
		/// <param name="value">The value to locate in this object.</param>
		/// <param name="comparer">The equality comparer used to locate the specified value in this object.</param>
		/// <returns>True if the value is found.  False otherwise.</returns>
		public bool ContainsValue(TValue value, IEqualityComparer<TValue> comparer) {
			return this.Values.Contains(value, comparer);
		}

		/// <summary>
		/// Determines whether this object contains the specified key.
		/// </summary>
		/// <param name="key">The key to locate in this object.</param>
		/// <returns>True if the key is found.  False otherwise.</returns>
		public bool ContainsKey(TKey key) {
			return _keyedCollection.Contains(key);
		}

		/// <summary>
		/// Returns the KeyValuePair at the index specified.
		/// </summary>
		/// <param name="index">The index of the KeyValuePair desired</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when the index specified does not refer to a KeyValuePair in this object
		/// </exception>
		public KeyValuePair<TKey, TValue> GetItem(int index) {
			if (index < 0 || index >= _keyedCollection.Count) {
				throw new ArgumentException("The index was outside the bounds of the dictionary: " + index);
			}
			return _keyedCollection[index];
		}

		/// <summary>
		/// Sets the value at the index specified.
		/// </summary>
		/// <param name="index">The index of the value desired</param>
		/// <param name="value">The value to set</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when the index specified does not refer to a KeyValuePair in this object
		/// </exception>
		public void SetItem(int index, TValue value) {
			if (index < 0 || index >= _keyedCollection.Count) {
				throw new ArgumentException("The index is outside the bounds of the dictionary: " + index);
			}
			var kvp = new KeyValuePair<TKey, TValue>(_keyedCollection[index].Key, value);
			_keyedCollection[index] = kvp;
		}

		/// <summary>
		/// Returns an enumerator that iterates through all the KeyValuePairs in this object.
		/// </summary>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			return _keyedCollection.GetEnumerator();
		}

		/// <summary>
		/// Removes the key-value pair for the specified key.
		/// </summary>
		/// <param name="key">The key to remove from the dictionary.</param>
		/// <returns>True if the item specified existed and the removal was successful.  False otherwise.</returns>
		public bool Remove(TKey key) {
			return _keyedCollection.Remove(key);
		}

		/// <summary>
		/// Removes the key-value pair at the specified index.
		/// </summary>
		/// <param name="index">The index of the key-value pair to remove from the dictionary.</param>
		public void RemoveAt(int index) {
			if (index < 0 || index >= _keyedCollection.Count) {
				throw new ArgumentException("The index was outside the bounds of the dictionary: " + index);
			}
			_keyedCollection.RemoveAt(index);
		}

		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key associated with the value to get.</param>
		public TValue GetValue(TKey key) {
			if (_keyedCollection.Contains(key) == false) {
				throw new ArgumentException("The given key is not present in the dictionary: " + key);
			}
			var kvp = _keyedCollection[key];
			return kvp.Value;
		}

		/// <summary>
		/// Sets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key associated with the value to set.</param>
		/// <param name="value">The the value to set.</param>
		public void SetValue(TKey key, TValue value) {
			var kvp = new KeyValuePair<TKey, TValue>(key, value);
			var idx = IndexOf(key);
			if (idx > -1) {
				_keyedCollection[idx] = kvp;
			}
			else {
				_keyedCollection.Add(kvp);
			}
		}

		/// <summary>
		/// Tries to get the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key of the desired element.</param>
		/// <param name="value">
		/// When this method returns, contains the value associated with the specified key if
		/// that key was found.  Otherwise it will contain the default value for parameter's type.
		/// This parameter should be provided uninitialized.
		/// </param>
		/// <returns>True if the value was found.  False otherwise.</returns>
		/// <remarks></remarks>
		public bool TryGetValue(TKey key, out TValue value) {
			if (_keyedCollection.Contains(key)) {
				value = _keyedCollection[key].Value;
				return true;
			}
			else {
				value = default(TValue);
				return false;
			}
		}

		#endregion

		#region Sorting
		public void SortKeys() {
			_keyedCollection.SortByKeys();
		}

		public void SortKeys(IComparer<TKey> comparer) {
			_keyedCollection.SortByKeys(comparer);
		}

		public void SortKeys(Comparison<TKey> comparison) {
			_keyedCollection.SortByKeys(comparison);
		}

		public void SortValues() {
			var comparer = Comparer<TValue>.Default;
			SortValues(comparer);
		}

		public void SortValues(IComparer<TValue> comparer) {
			_keyedCollection.Sort((x,y) => comparer.Compare(x.Value, y.Value));
		}

		public void SortValues(Comparison<TValue> comparison) {
			_keyedCollection.Sort((x, y) => comparison(x.Value, y.Value));
		}
		#endregion

		#region IDictionary<TKey, TValue>

		void IDictionary<TKey, TValue>.Add(TKey key, TValue value) {
			Add(key, value);
		}

		bool IDictionary<TKey, TValue>.ContainsKey(TKey key) {
			return ContainsKey(key);
		}

		ICollection<TKey> IDictionary<TKey, TValue>.Keys {
			get { return Keys; }
		}

		bool IDictionary<TKey, TValue>.Remove(TKey key) {
			return Remove(key);
		}

		bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value) {
			return TryGetValue(key, out value);
		}

		ICollection<TValue> IDictionary<TKey, TValue>.Values {
			get { return Values; }
		}

		TValue IDictionary<TKey, TValue>.this[TKey key] {
			get {
				return this[key];
			}
			set {
				this[key] = value;
			}
		}

		#endregion

		#region ICollection<KeyValuePair<TKey, TValue>>

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) {
			_keyedCollection.Add(item);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Clear() {
			_keyedCollection.Clear();
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) {
			return _keyedCollection.Contains(item);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			_keyedCollection.CopyTo(array, arrayIndex);
		}

		int ICollection<KeyValuePair<TKey, TValue>>.Count {
			get { return _keyedCollection.Count; }
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly {
			get { return false; }
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
			return _keyedCollection.Remove(item);
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey, TValue>>

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
			return GetEnumerator();
		}

		#endregion

		#region IEnumerable

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		#endregion

		#region IOrderedDictionary

		IDictionaryEnumerator IOrderedDictionary.GetEnumerator() {
			return new DictionaryEnumerator<TKey, TValue>(this);
		}

		void IOrderedDictionary.Insert(int index, object key, object value) {
			Insert(index, (TKey)key, (TValue)value);
		}

		void IOrderedDictionary.RemoveAt(int index) {
			RemoveAt(index);
		}

		object IOrderedDictionary.this[int index] {
			get {
				return this[index];
			}
			set {
				this[index] = (TValue)value;
			}
		}

		#endregion

		#region IDictionary

		void IDictionary.Add(object key, object value) {
			Add((TKey)key, (TValue)value);
		}

		void IDictionary.Clear() {
			Clear();
		}

		bool IDictionary.Contains(object key) {
			return _keyedCollection.Contains((TKey)key);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator() {
			return new DictionaryEnumerator<TKey, TValue>(this);
		}

		bool IDictionary.IsFixedSize {
			get { return false; }
		}

		bool IDictionary.IsReadOnly {
			get { return false; }
		}

		ICollection IDictionary.Keys {
			get { return (ICollection)this.Keys; }
		}

		void IDictionary.Remove(object key) {
			Remove((TKey)key);
		}

		ICollection IDictionary.Values {
			get { return (ICollection)this.Values; }
		}

		object IDictionary.this[object key] {
			get {
				return this[(TKey)key];
			}
			set {
				this[(TKey)key] = (TValue)value;
			}
		}

		#endregion

		#region ICollection

		void ICollection.CopyTo(Array array, int index) {
			((ICollection)_keyedCollection).CopyTo(array, index);
		}

		int ICollection.Count {
			get { return ((ICollection)_keyedCollection).Count; }
		}

		bool ICollection.IsSynchronized {
			get { return ((ICollection)_keyedCollection).IsSynchronized; }
		}

		object ICollection.SyncRoot {
			get { return ((ICollection)_keyedCollection).SyncRoot; }
		}

		#endregion
	}

	public class KeyedCollection2<TKey, TItem> : KeyedCollection<TKey, TItem> {
		private const string DelegateNullExceptionMessage = "Delegate passed cannot be null";
		private Func<TItem, TKey> _getKeyForItemDelegate;

		public KeyedCollection2(Func<TItem, TKey> getKeyForItemDelegate)
			: base() {
			if (getKeyForItemDelegate == null)
				throw new ArgumentNullException(DelegateNullExceptionMessage);
			_getKeyForItemDelegate = getKeyForItemDelegate;
		}

		public KeyedCollection2(Func<TItem, TKey> getKeyForItemDelegate, IEqualityComparer<TKey> comparer)
			: base(comparer) {
			if (getKeyForItemDelegate == null)
				throw new ArgumentNullException(DelegateNullExceptionMessage);
			_getKeyForItemDelegate = getKeyForItemDelegate;
		}

		protected override TKey GetKeyForItem(TItem item) {
			return _getKeyForItemDelegate(item);
		}

		public void SortByKeys() {
			var comparer = Comparer<TKey>.Default;
			SortByKeys(comparer);
		}

		public void SortByKeys(IComparer<TKey> keyComparer) {
			var comparer = new Comparer2<TItem>((x, y) => keyComparer.Compare(GetKeyForItem(x), GetKeyForItem(y)));
			Sort(comparer);
		}

		public void SortByKeys(Comparison<TKey> keyComparison) {
			var comparer = new Comparer2<TItem>((x, y) => keyComparison(GetKeyForItem(x), GetKeyForItem(y)));
			Sort(comparer);
		}

		public void Sort() {
			var comparer = Comparer<TItem>.Default;
			Sort(comparer);
		}

		public void Sort(Comparison<TItem> comparison) {
			var newComparer = new Comparer2<TItem>((x, y) => comparison(x, y));
			Sort(newComparer);
		}

		public void Sort(IComparer<TItem> comparer) {
			List<TItem> list = base.Items as List<TItem>;
			if (list != null) {
				list.Sort(comparer);
			}
		}
	}

	public class Comparer2<T> : Comparer<T> {
		//private readonly Func<T, T, int> _compareFunction;
		private readonly Comparison<T> _compareFunction;

		#region Constructors

		public Comparer2(Comparison<T> comparison) {
			if (comparison == null)
				throw new ArgumentNullException("comparison");
			_compareFunction = comparison;
		}

		#endregion

		public override int Compare(T arg1, T arg2) {
			return _compareFunction(arg1, arg2);
		}
	}

	public class DictionaryEnumerator<TKey, TValue> : IDictionaryEnumerator, IDisposable {
		readonly IEnumerator<KeyValuePair<TKey, TValue>> impl;
		public void Dispose() { impl.Dispose(); }
		public DictionaryEnumerator(IDictionary<TKey, TValue> value) {
			this.impl = value.GetEnumerator();
		}
		public void Reset() { impl.Reset(); }
		public bool MoveNext() { return impl.MoveNext(); }
		public DictionaryEntry Entry {
			get {
				var pair = impl.Current;
				return new DictionaryEntry(pair.Key, pair.Value);
			}
		}
		public object Key { get { return impl.Current.Key; } }
		public object Value { get { return impl.Current.Value; } }
		public object Current { get { return Entry; } }
	}

	public interface IOrderedDictionary<K, V> : IDictionary, IOrderedDictionary, IDictionary<K, V>, IEnumerable<KeyValuePair<K, V>> {

	}
}