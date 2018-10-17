namespace ShanDian.Webwork.Conventions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Collection class for static content conventions
    /// </summary>
    public class StaticContentsConventions : IEnumerable<Func<WebworkContext, string, Response>>
    {
        private readonly IEnumerable<Func<WebworkContext, string, Response>> conventions;

        public StaticContentsConventions(IEnumerable<Func<WebworkContext, string, Response>> conventions)
        {
            this.conventions = conventions;
        }

        public IEnumerator<Func<WebworkContext, string, Response>> GetEnumerator()
        {
            return conventions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}