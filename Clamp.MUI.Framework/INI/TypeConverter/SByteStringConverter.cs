﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Framework.INI.TypeConverter
{
    internal sealed class SByteStringConverter : TypeStringConverter<sbyte>
    {
        public override string ConvertToString(object value)
        {
            return ((sbyte)value).ToString(INIFile.CultureInfo.NumberFormat);
        }

        public override object ConvertFromString(string value, Type hint)
        {
            return sbyte.Parse(value, INIFile.CultureInfo.NumberFormat);
        }
    }
}