using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.AppCenter.Initial.TypeConverter
{
    internal sealed class UInt64StringConverter : TypeStringConverter<ulong>
    {
        public override string ConvertToString(object value)
        {
            return ((ulong)value).ToString(InitialFile.CultureInfo.NumberFormat);
        }

        public override object ConvertFromString(string value, Type hint)
        {
            return ulong.Parse(value, InitialFile.CultureInfo.NumberFormat);
        }
    }
}
