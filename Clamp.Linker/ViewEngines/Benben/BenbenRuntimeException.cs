using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clamp.Linker.ViewEngines.Benben
{
    public class BenbenRuntimeException : BenbenException
    {
        public BenbenRuntimeException(string message)
            : base(message)
        {
        }

        public BenbenRuntimeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
