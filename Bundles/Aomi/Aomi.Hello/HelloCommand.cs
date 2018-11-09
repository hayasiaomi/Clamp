using Aomi.Main;
using Clamp.OSGI.Framework.Data.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aomi.Hello
{
    [Extension]
    public class HelloCommand : ICommand
    {
        public void Run()
        {
            Console.WriteLine("It is Hello");
        }
    }
}
