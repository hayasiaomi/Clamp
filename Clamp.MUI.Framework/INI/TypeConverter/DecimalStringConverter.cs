using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Framework.INI.TypeConverter
{
    internal sealed class DecimalStringConverter : TypeStringConverter<decimal>
    {
        public override string ConvertToString(object value)
        {
            return ((decimal)value).ToString(INIFile.CultureInfo.NumberFormat);
        }

        public override object ConvertFromString(string value, Type hint)
        {
            return decimal.Parse(value, INIFile.CultureInfo.NumberFormat);
        }
    }

}
