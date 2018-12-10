using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Framework.INI.TypeConverter
{
    internal sealed class Int64StringConverter : TypeStringConverter<long>
    {
        public override string ConvertToString(object value)
        {
            return ((long)value).ToString(INIFile.CultureInfo.NumberFormat);
        }

        public override object ConvertFromString(string value, Type hint)
        {
            return long.Parse(value, INIFile.CultureInfo.NumberFormat);
        }
    }
}
