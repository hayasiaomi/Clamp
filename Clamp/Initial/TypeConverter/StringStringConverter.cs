using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSG.Initial.TypeConverter
{
    internal sealed class StringStringConverter : TypeStringConverter<string>
    {
        public override string ConvertToString(object value)
        {
            return value.ToString().Trim();
        }

        public override object ConvertFromString(string value, Type hint)
        {
            return value;
        }
    }
}
