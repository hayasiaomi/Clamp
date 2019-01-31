using System;
using System.IO;

namespace Clamp.Linker.ViewEngines.Benben
{
    public static class BenbenExtensions
    {
        public static void WriteSafeString(this TextWriter writer, string value)
        {
            writer.Write(new SafeString(value));
        }

        public static void WriteSafeString(this TextWriter writer, object value)
        {
            writer.WriteSafeString(value.ToString());
        }

        private class SafeString : ISafeString
        {
            private readonly string _value;

            public SafeString(string value)
            {
                _value = value;
            }

            public override string ToString()
            {
                return _value;
            }
        }
    }
}

