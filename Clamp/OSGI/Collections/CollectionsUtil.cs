using System;
using System.Collections;

namespace Clamp.OSGI.Collections
{
	
	/// <summary>
	/// Static utility methods for collections
	/// </summary>
	public class CollectionsUtil
	{
		public static Object PutElement(IDictionary hashTable, Object key, Object newValue)
		{
			Object element = hashTable[key];
			hashTable[key] = newValue;
			return element;
		}
	}
}