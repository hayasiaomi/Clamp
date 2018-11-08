using System;

namespace Clamp.OSGI.Framework.Data.Serialization
{
	internal interface IBinaryXmlElement
	{
		void Read (BinaryXmlReader reader);

		void Write (BinaryXmlWriter writer);
	}
}
