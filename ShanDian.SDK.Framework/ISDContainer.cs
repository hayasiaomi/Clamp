using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.SDK.Framework.Injection
{
    public interface ISDContainer
    {
        object GetService(Type sType);

        object GetService(Type sType, string name);

        void AddService(Type sType);
        void AddService(Type sType, string name);

        void AddService(Type sType, object sInstance);

        void AddService(Type sType, object sInstance, string name);

        void RemoveService(Type sType);

        void RemoveService(Type sType, string name);
    }
}
