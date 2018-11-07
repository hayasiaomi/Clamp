using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSG.Initial.TypeConverter
{
    internal sealed class UInt32StringConverter : TypeStringConverter<uint>
    {
        public override string ConvertToString(object value)
        {
            return ((uint)value).ToString(InitialFile.CultureInfo.NumberFormat);
        }

        public override object ConvertFromString(string value, Type hint)
        {
            return uint.Parse(value, InitialFile.CultureInfo.NumberFormat);
        }
    }
}
