using System;
using System.Collections.Concurrent;
using System.ComponentModel.Design;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShanDian.SDK.Framework.Injection;

namespace ShanDian.SDK.Framework
{
    /// <summary>
    /// SD的服务容器
    /// </summary>
    internal sealed class SDContainer : Container, ISDContainer
    {

        public object GetService(Type sType, string name)
        {
            return this.Resolve(sType, name);
        }

        public object GetService(Type sType)
        {
            return this.Resolve(sType);
        }

        public void AddService(Type sType)
        {
            this.Register(sType);
        }

        public void AddService(Type sType, string name)
        {
            this.Register(sType, name);
        }

        public void AddService(Type sType, object sInstance)
        {
            this.Register(sType, sInstance);
        }

        public void AddService(Type sType, object sInstance, string name)
        {
            this.Register(sType, sInstance, name);
        }

        public void RemoveService(Type sType)
        {
            this.Unregister(sType);
        }

        public void RemoveService(Type sType, string name)
        {
            this.Unregister(sType, name);
        }
    }
}
