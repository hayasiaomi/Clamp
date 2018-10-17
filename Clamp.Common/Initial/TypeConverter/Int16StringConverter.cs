using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Common.Initial.TypeConverter
{
    internal sealed class Int16StringConverter : TypeStringConverter<short>
    {
        public override string ConvertToString(object value)
        {
            return ((short)value).ToString(InitialFile.CultureInfo.NumberFormat);
        }

        public override object ConvertFromString(string value, Type hint)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return short.Parse(value, InitialFile.CultureInfo.NumberFormat);
            return 0;
        }
    }
}
