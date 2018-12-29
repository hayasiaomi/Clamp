using System;

namespace Clamp.AppCenter.Initial
{
    [Serializable]
    public sealed class InitialPropertyCastException : Exception
    {
        private InitialPropertyCastException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        internal static InitialPropertyCastException Create(string stringValue, Type dstType, Exception innerException)
        {
            return new InitialPropertyCastException(string.Format("无法将值{0}转化{1}类型", stringValue, dstType.FullName), innerException);
        }
    }
}