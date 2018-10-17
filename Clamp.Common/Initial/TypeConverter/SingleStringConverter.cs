using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Common.Initial.TypeConverter
{
    internal sealed class SingleStringConverter : TypeStringConverter<float>
    {
        public override string ConvertToString(object value)
        {
            return ((float)value).ToString(InitialFile.CultureInfo.NumberFormat);
        }

        public override object ConvertFromString(string value, Type hint)
        {
            return float.Parse(value, InitialFile.CultureInfo.NumberFormat);
        }
    }
}
