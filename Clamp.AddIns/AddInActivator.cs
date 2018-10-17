using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.AddIns
{
    public abstract class AddInActivator : IAddInActivator
    {
        public virtual void Start(AddInContext addInContext)
        {

        }

        public virtual void Stop(AddInContext addInContext)
        {

        }
    }
}
