using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSG.Initial.TypeConverter
{
    internal sealed class UInt16StringConverter : TypeStringConverter<ushort>
    {
        public override string ConvertToString(object value)
        {
            return ((ushort)value).ToString(InitialFile.CultureInfo.NumberFormat);
        }

        public override object ConvertFromString(string value, Type hint)
        {
            return ushort.Parse(value, InitialFile.CultureInfo.NumberFormat);
        }
    }
}
