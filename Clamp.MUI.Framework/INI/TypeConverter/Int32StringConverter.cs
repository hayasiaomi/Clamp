using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Framework.INI.TypeConverter
{
    internal sealed class Int32StringConverter : TypeStringConverter<int>
    {
        public override string ConvertToString(object value)
        {
            return ((int)value).ToString(INIFile.CultureInfo.NumberFormat);
        }

        public override object ConvertFromString(string value, Type hint)
        {
            return int.Parse(value, INIFile.CultureInfo.NumberFormat);
        }
    }
}
