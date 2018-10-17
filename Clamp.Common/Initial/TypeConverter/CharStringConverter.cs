using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Common.Initial.TypeConverter
{
    internal sealed class CharStringConverter : TypeStringConverter<char>
    {
        public override string ConvertToString(object value)
        {
            return value.ToString();
        }

        public override object ConvertFromString(string value, Type hint)
        {
            return char.Parse(value);
        }
    }
}
