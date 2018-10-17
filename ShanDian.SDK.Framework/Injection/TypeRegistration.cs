using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.SDK.Framework.Injection
{
    public sealed class TypeRegistration
    {
        private int _hashCode;

        public Type Type { get; private set; }
        public string Name { get; private set; }

        public TypeRegistration(Type type)
            : this(type, string.Empty)
        {
        }

        public TypeRegistration(Type type, string name)
        {
            Type = type;
            Name = name;

            _hashCode = String.Concat(Type.FullName, "|", Name).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var typeRegistration = obj as TypeRegistration;

            if (typeRegistration == null)
                return false;

            if (Type != typeRegistration.Type)
                return false;

            if (String.Compare(Name, typeRegistration.Name, StringComparison.Ordinal) != 0)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
