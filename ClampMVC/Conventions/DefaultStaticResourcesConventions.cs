using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Linker.Conventions
{
    /// <summary>
    /// 静态内部资源
    /// </summary>
    public class DefaultStaticResourcesConventions : IConvention
    {
        /// <summary>
        /// 初始完
        /// </summary>
        /// <param name="conventions">Convention object instance.</param>
        public void Initialise(LinkerConventions conventions)
        {
            conventions.StaticContentsConventions = new List<Func<LinkerContext, string, Response>>
            {
               StaticContentConventionBuilder.AddResources("js","css","less","scss","map","ttf","eot","svg","woff","otf")
            };
        }

        /// <summary>
        /// 检证有效
        /// </summary>
        /// <param name="conventions"></param>
        /// <returns></returns>
        public Tuple<bool, string> Validate(LinkerConventions conventions)
        {
            if (conventions.StaticContentsConventions == null)
            {
                return Tuple.Create(false, "The static contents conventions cannot be null.");
            }

            return (conventions.StaticContentsConventions.Count > 0) ? Tuple.Create(true, string.Empty) : Tuple.Create(false, "The static contents conventions cannot be empty.");
        }


    }
}
