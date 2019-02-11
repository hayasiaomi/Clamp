using System;

namespace Clamp.OSGI.Collections
{
	public interface IOrderedDictionary
	{
		void Insert(Int32 index, Object key, Object value);
		void RemoveAt(Int32 index);
	}
}