using System;

namespace Clamp.Data.Serialization
{
    /// <summary>
    /// ���л��ӿ�
    /// </summary>
	internal interface IBinaryXmlElement
    {
        void Read(BinaryXmlReader reader);

        void Write(BinaryXmlWriter writer);
    }
}
