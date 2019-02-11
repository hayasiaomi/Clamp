using System;
using System.Collections;

namespace Clamp.Cfg
{

	/// <summary>
	/// A keyed list with a fixed maximum size which removes
	/// the least recently used entry if an entry is added when full.
	/// </summary>
	[Serializable]
	public class LRUMap : ICollection, IDictionary, IEnumerable
	{
		private Hashtable objectTable = new Hashtable();
		private ArrayList objectList = new ArrayList();

		/// <summary>
		/// Default maximum size 
		/// </summary>
		protected internal const int DEFAULT_MAX_SIZE = 100;

		/// <summary>
		/// Maximum size 
		/// </summary>
		[NonSerialized] private int maxSize;

		public LRUMap() : this(DEFAULT_MAX_SIZE)
		{
		}

		public LRUMap(Int32 maxSize)
		{
			this.maxSize = maxSize;
		}

		public virtual void Add(object key, object value)
		{
			if (objectList.Count == maxSize)
			{
				RemoveLRU();
			}

			objectTable.Add(key, value);
			objectList.Insert(0, new DictionaryEntry(key, value));
		}

		public virtual void Clear()
		{
			objectTable.Clear();
			objectList.Clear();
		}

		public virtual bool Contains(object key)
		{
			return objectTable.Contains(key);
		}

		public virtual void CopyTo(Array array, int idx)
		{
			objectTable.CopyTo(array, idx);
		}

		public virtual void Remove(object key)
		{
			objectTable.Remove(key);
			objectList.RemoveAt(IndexOf(key));
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new KeyedListEnumerator(objectList);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new KeyedListEnumerator(objectList);
		}

		public virtual int Count
		{
			get { return objectList.Count; }
		}

		public virtual bool IsFixedSize
		{
			get { return true; }
		}

		public virtual bool IsReadOnly
		{
			get { return false; }
		}

		public virtual bool IsSynchronized
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the maximum size of the map (the bound).
		/// </summary>
		public Int32 MaxSize
		{
			get { return maxSize; }
		}


		public virtual object this[object key]
		{
			get
			{
				MoveToMRU(key);
				return objectTable[key];
			}
			set
			{
				if (objectTable.Contains(key))
				{
					Remove(key);
				}
				Add(key, value);
			}
		}

		public virtual ICollection Keys
		{
			get
			{
				ArrayList retList = new ArrayList();
				for(int i = 0; i < objectList.Count; i++)
				{
					retList.Add(((DictionaryEntry) objectList[i]).Key);
				}
				return retList;
			}
		}

		public virtual ICollection Values
		{
			get
			{
				ArrayList retList = new ArrayList();
				for(int i = 0; i < objectList.Count; i++)
				{
					retList.Add(((DictionaryEntry) objectList[i]).Value);
				}
				return retList;
			}
		}

		public virtual object SyncRoot
		{
			get { return this; }
		}

		public void AddAll(IDictionary dictionary)
		{
			foreach(DictionaryEntry entry in dictionary)
			{
				Add(entry.Key, entry.Value);
			}
		}

		private int IndexOf(object key)
		{
			for(int i = 0; i < objectList.Count; i++)
			{
				if (((DictionaryEntry) objectList[i]).Key.Equals(key))
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Remove the least recently used entry (the last one in the list)
		/// </summary>
		private void RemoveLRU()
		{
			Object key = ((DictionaryEntry) objectList[objectList.Count - 1]).Key;
			objectTable.Remove(key);
			objectList.RemoveAt(objectList.Count - 1);
		}

		private void MoveToMRU(Object key)
		{
			Int32 i = IndexOf(key);

			// only move if found and not already first
			if (i > 0)
			{
				Object value = objectList[i];
				objectList.RemoveAt(i);
				objectList.Insert(0, value);
			}
		}

		// Returns a thread-safe wrapper for a LRUMap.
		//
		public static LRUMap Synchronized(LRUMap table)
		{
			if (table == null)
			{
				throw new ArgumentNullException("table");
			}
			return new SyncLRUMap(table);
		}

		// Synchronized wrapper for LRUMap
		[Serializable()]
		private class SyncLRUMap : LRUMap, IDictionary, IEnumerable
		{
			protected LRUMap _table;

			internal SyncLRUMap(LRUMap table)
			{
				_table = table;
			}

			public override int Count
			{
				get { return _table.Count; }
			}

			public override bool IsReadOnly
			{
				get { return _table.IsReadOnly; }
			}

			public override bool IsFixedSize
			{
				get { return _table.IsFixedSize; }
			}

			public override bool IsSynchronized
			{
				get { return true; }
			}

			public override Object this[Object key]
			{
				get
				{
					lock (_table.SyncRoot)
					{
						return _table[key];
					}
				}
				set
				{
					lock (_table.SyncRoot)
					{
						_table[key] = value;
					}
				}
			}

			public override Object SyncRoot
			{
				get { return _table.SyncRoot; }
			}

			public override void Add(Object key, Object value)
			{
				lock(_table.SyncRoot)
				{
					_table.Add(key, value);
				}
			}

			public override void Clear()
			{
				lock(_table.SyncRoot)
				{
					_table.Clear();
				}
			}

			public override bool Contains(Object key)
			{
				return _table.Contains(key);
			}


			public override void CopyTo(Array array, int arrayIndex)
			{
				_table.CopyTo(array, arrayIndex);
			}



			IDictionaryEnumerator IDictionary.GetEnumerator()
			{
				return ((IDictionary) _table).GetEnumerator();
			}


			public override ICollection Keys
			{
				get
				{
					lock(_table.SyncRoot)
					{
						return _table.Keys;
					}
				}
			}

			public override ICollection Values
			{
				get
				{
					lock(_table.SyncRoot)
					{
						return _table.Values;
					}
				}
			}

			public override void Remove(Object key)
			{
				lock(_table.SyncRoot)
				{
					_table.Remove(key);
				}
			}

		}
	}
}