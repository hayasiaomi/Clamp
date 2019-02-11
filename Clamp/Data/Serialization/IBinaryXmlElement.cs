using System;

namespace Clamp.Data.Serialization
{
    /// <summary>
    /// 序列化接口
    /// </summary>
	internal interface IBinaryXmlElement
    {
        void Read(BinaryXmlReader reader);

        void Write(BinaryXmlWriter writer);
    }
}
