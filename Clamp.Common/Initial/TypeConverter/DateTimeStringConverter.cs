using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Common.Initial.TypeConverter
{
    internal sealed class DateTimeStringConverter : TypeStringConverter<DateTime>
    {
        public override string ConvertToString(object value)
        {
            return ((DateTime)value).ToString(InitialFile.CultureInfo.DateTimeFormat);
        }

        public override object ConvertFromString(string value, Type hint)
        {
            return DateTime.Parse(value, InitialFile.CultureInfo.DateTimeFormat);
        }
    }
}
