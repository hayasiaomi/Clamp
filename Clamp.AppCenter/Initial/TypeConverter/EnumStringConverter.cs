using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.AppCenter.Initial.TypeConverter
{
    internal sealed class EnumStringConverter : TypeStringConverter<Enum>
    {
        public override string ConvertToString(object value)
        {
            return value.ToString();
        }

        public override object ConvertFromString(string value, Type hint)
        {
            int indexOfLastDot = value.LastIndexOf('.');

            if (indexOfLastDot >= 0)
                value = value.Substring(indexOfLastDot + 1, value.Length - indexOfLastDot - 1).Trim();

            return Enum.Parse(hint, value);
        }
    }
}
