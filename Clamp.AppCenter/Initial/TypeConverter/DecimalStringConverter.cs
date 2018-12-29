using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.AppCenter.Initial.TypeConverter
{
    internal sealed class DecimalStringConverter : TypeStringConverter<decimal>
    {
        public override string ConvertToString(object value)
        {
            return ((decimal)value).ToString(InitialFile.CultureInfo.NumberFormat);
        }

        public override object ConvertFromString(string value, Type hint)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return decimal.Parse(value, InitialFile.CultureInfo.NumberFormat);
            return 0;

        }
    }

}
