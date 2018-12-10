using Clamp.MUI.Framework.Properties;
using System;

namespace Clamp.MUI.Framework.INI
{
    [Serializable]
    public sealed class SectionValueCastException : Exception
    {
        private SectionValueCastException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        internal static SectionValueCastException Create(string stringValue, Type dstType, Exception innerException)
        {
            string msg = string.Format(StringResources.INI_Exception_SectionItemValueCast, stringValue, dstType.FullName);
            return new SectionValueCastException(msg, innerException);
        }
    }
}