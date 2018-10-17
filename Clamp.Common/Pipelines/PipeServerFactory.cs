using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;

namespace Clamp.Common.Pipelines
{
    /// <summary>
    /// 服务类的工厂类
    /// </summary>
    static class PipeServerFactory
    {
        public static NamedPipeServerStream CreateAndConnectPipe(string pipeName, PipeSecurity pipeSecurity)
        {
            var pipe = CreatePipe(pipeName, pipeSecurity);
            pipe.WaitForConnection();
            return pipe;
        }

        public static NamedPipeServerStream CreatePipe(string pipeName, PipeSecurity pipeSecurity)
        {
            return new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous | PipeOptions.WriteThrough, 0, 0, pipeSecurity);
        }
    }
}
