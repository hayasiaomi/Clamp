using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSG.Initial.TypeConverter
{
    public abstract class TypeStringConverter<T> : ITypeStringConverter
    {
        public abstract string ConvertToString(object value);

        public abstract object ConvertFromString(string value, Type hint);

        public Type ConvertibleType
        {
            get { return typeof(T); }
        }
    }
}
