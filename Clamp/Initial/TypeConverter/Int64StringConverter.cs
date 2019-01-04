using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSG.Initial.TypeConverter
{
    internal sealed class Int64StringConverter : TypeStringConverter<long>
    {
        public override string ConvertToString(object value)
        {
            return ((long)value).ToString(InitialFile.CultureInfo.NumberFormat);
        }

        public override object ConvertFromString(string value, Type hint)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return long.Parse(value, InitialFile.CultureInfo.NumberFormat);
            return 0L;
        }
    }
}
