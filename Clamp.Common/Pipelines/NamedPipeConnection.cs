using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Clamp.Common.Pipelines.IO;
using Clamp.Common.Pipelines.Threading;
using System.Collections.Concurrent;

namespace Clamp.Common.Pipelines
{

    public class NamedPipeConnection<TRead, TWrite> where TRead : class where TWrite : class
    {
        private readonly PipeStreamWrapper<TRead, TWrite> _streamWrapper;
        private readonly AutoResetEvent _writeSignal = new AutoResetEvent(false);


        public readonly int Id;

        public string Name { get; }

        public bool CanRead { get { return this._streamWrapper.CanRead; } }

        public bool IsConnected { get { return _streamWrapper.IsConnected; } }

        internal NamedPipeConnection(int id, string name, PipeStream serverStream)
        {
            Id = id;
            Name = name;
            _streamWrapper = new PipeStreamWrapper<TRead, TWrite>(serverStream);
        }

        public void PushMessage(TWrite message)
        {
            _streamWrapper.WriteObject(message);
            _streamWrapper.WaitForPipeDrain();
        }

        public TRead ReadMessage()
        {
            return _streamWrapper.ReadObject();
        }

        public void Close()
        {
            CloseImpl();
        }

        private void CloseImpl()
        {
            _streamWrapper.Close();
            _writeSignal.Set();
        }
    }

    static class ConnectionFactory
    {
        private static int _lastId;

        public static NamedPipeConnection<TRead, TWrite> CreateConnection<TRead, TWrite>(string name, PipeStream pipeStream) where TRead : class where TWrite : class
        {
            return new NamedPipeConnection<TRead, TWrite>(++_lastId, name, pipeStream);
        }
    }

    public delegate void ConnectionEventHandler<TRead, TWrite>(NamedPipeConnection<TRead, TWrite> connection) where TRead : class where TWrite : class;

    public delegate void ConnectionMessageEventHandler<TRead, TWrite>(NamedPipeConnection<TRead, TWrite> connection, TRead message) where TRead : class where TWrite : class;

    public delegate void ConnectionExceptionEventHandler<TRead, TWrite>(NamedPipeConnection<TRead, TWrite> connection, Exception exception) where TRead : class where TWrite : class;
}
