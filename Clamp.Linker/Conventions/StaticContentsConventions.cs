namespace Clamp.Linker.Conventions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Collection class for static content conventions
    /// </summary>
    public class StaticContentsConventions : IEnumerable<Func<LinkerContext, string, Response>>
    {
        private readonly IEnumerable<Func<LinkerContext, string, Response>> conventions;

        public StaticContentsConventions(IEnumerable<Func<LinkerContext, string, Response>> conventions)
        {
            this.conventions = conventions;
        }

        public IEnumerator<Func<LinkerContext, string, Response>> GetEnumerator()
        {
            return conventions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}