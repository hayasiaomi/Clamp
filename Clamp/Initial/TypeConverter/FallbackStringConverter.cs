using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Clamp.OSG.Initial.TypeConverter
{
    internal sealed class FallbackStringConverter : ITypeStringConverter
    {
        public string ConvertToString(object value)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(value);

                return converter.ConvertToString(null, InitialFile.CultureInfo, value);
            }
            catch (Exception ex)
            {
                throw InitialPropertyCastException.Create(value.ToString(), value.GetType(), ex);
            }
        }

        public object ConvertFromString(string value, Type hint)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(hint);

                return converter.ConvertFrom(null, InitialFile.CultureInfo, value);
            }
            catch (Exception ex)
            {
                throw InitialPropertyCastException.Create(value, hint, ex);
            }
        }

        public Type ConvertibleType
        {
            get { return null; }
        }
    }
}
