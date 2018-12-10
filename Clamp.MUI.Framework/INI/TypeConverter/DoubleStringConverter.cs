using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Framework.INI.TypeConverter
{
    internal sealed class DoubleStringConverter : TypeStringConverter<double>
    {
        public override string ConvertToString(object value)
        {
            return ((double)value).ToString(INIFile.CultureInfo.NumberFormat);
        }

        public override object ConvertFromString(string value, Type hint)
        {
            return double.Parse(value, INIFile.CultureInfo.NumberFormat);
        }
    }
}
