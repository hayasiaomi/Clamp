using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Framework.INI.TypeConverter
{
    internal sealed class Int16StringConverter : TypeStringConverter<short>
    {
        public override string ConvertToString(object value)
        {
            return ((short)value).ToString(INIFile.CultureInfo.NumberFormat);
        }

        public override object ConvertFromString(string value, Type hint)
        {
            return short.Parse(value, INIFile.CultureInfo.NumberFormat);
        }
    }
}
