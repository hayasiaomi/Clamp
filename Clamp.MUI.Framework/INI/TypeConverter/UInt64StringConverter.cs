using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Framework.INI.TypeConverter
{
    internal sealed class UInt64StringConverter : TypeStringConverter<ulong>
    {
        public override string ConvertToString(object value)
        {
            return ((ulong)value).ToString(INIFile.CultureInfo.NumberFormat);
        }

        public override object ConvertFromString(string value, Type hint)
        {
            return ulong.Parse(value, INIFile.CultureInfo.NumberFormat);
        }
    }
}
