using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.AppCenter.Initial.TypeConverter
{
    internal sealed class Int32StringConverter : TypeStringConverter<int>
    {
        public override string ConvertToString(object value)
        {
            return ((int)value).ToString(InitialFile.CultureInfo.NumberFormat);
        }

        public override object ConvertFromString(string value, Type hint)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return int.Parse(value, InitialFile.CultureInfo.NumberFormat);
            return 0;
        }
    }
}
