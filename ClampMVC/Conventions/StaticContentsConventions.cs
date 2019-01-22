namespace ClampMVC.Conventions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Collection class for static content conventions
    /// </summary>
    public class StaticContentsConventions : IEnumerable<Func<ClampWebContext, string, Response>>
    {
        private readonly IEnumerable<Func<ClampWebContext, string, Response>> conventions;

        public StaticContentsConventions(IEnumerable<Func<ClampWebContext, string, Response>> conventions)
        {
            this.conventions = conventions;
        }

        public IEnumerator<Func<ClampWebContext, string, Response>> GetEnumerator()
        {
            return conventions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}