using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.Common.Initial.TypeConverter
{
    internal sealed class DoubleStringConverter : TypeStringConverter<double>
    {
        public override string ConvertToString(object value)
        {
            return ((double)value).ToString(InitialFile.CultureInfo.NumberFormat);
        }

        public override object ConvertFromString(string value, Type hint)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return double.Parse(value, InitialFile.CultureInfo.NumberFormat);
            return 0.0d;
        }
    }
}
