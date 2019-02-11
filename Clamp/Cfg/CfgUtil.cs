using System;
using System.Collections;

namespace Clamp.Cfg
{
	
	/// <summary>
	/// Static utility methods for collections
	/// </summary>
	public class CfgUtil
	{
		public static Object PutElement(IDictionary hashTable, object key, object newValue)
		{
			Object element = hashTable[key];
			hashTable[key] = newValue;
			return element;
		}
	}
}