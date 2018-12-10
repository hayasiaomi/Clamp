using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Framework.INI.TypeConverter
{
    internal sealed class FallbackStringConverter : ITypeStringConverter
    {
        public string ConvertToString(object value)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(value);

                return converter.ConvertToString(null, INIFile.CultureInfo, value);
            }
            catch (Exception ex)
            {
                throw SectionValueCastException.Create(value.ToString(), value.GetType(), ex);
            }
        }

        public object ConvertFromString(string value, Type hint)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(hint);

                return converter.ConvertFrom(null, INIFile.CultureInfo, value);
            }
            catch (Exception ex)
            {
                throw SectionValueCastException.Create(value, hint, ex);
            }
        }

        public Type ConvertibleType
        {
            get { return null; }
        }
    }
}
