using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.Common.Initial.TypeConverter
{
    internal sealed class BoolStringConverter : TypeStringConverter<bool>
    {
        public override string ConvertToString(object value)
        {
            return value.ToString();
        }

        public override object ConvertFromString(string value, Type hint)
        {
            switch (value.ToLowerInvariant())
            {
                case "false":
                case "off":
                case "no":
                case "n":
                case "0":
                    return false;
                case "true":
                case "on":
                case "yes":
                case "y":
                case "1":
                    return true;
            }

            throw InitialPropertyCastException.Create(value, hint, null);
        }
    }
}
