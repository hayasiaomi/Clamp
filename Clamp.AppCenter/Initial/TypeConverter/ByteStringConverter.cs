using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.AppCenter.Initial.TypeConverter
{
    internal sealed class ByteStringConverter : TypeStringConverter<byte>
    {
        public override string ConvertToString(object value)
        {
            return value.ToString();
        }

        public override object ConvertFromString(string value, Type hint)
        {
            return sbyte.Parse(value, InitialFile.CultureInfo.NumberFormat);
        }
    }

}
