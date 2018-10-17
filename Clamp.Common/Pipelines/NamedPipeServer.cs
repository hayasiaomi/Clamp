using Clamp.Common.Pipelines.IO;
using Clamp.Common.Pipelines.Threading;
using System;
using System.Collections.Generic;
using System.IO.Pipes;

namespace Clamp.Common.Pipelines
{
    /// <summary>
    /// 管道的通信服务类
    /// </summary>
    /// <typeparam name="TReadWrite"></typeparam>
    public class NamedPipeServer<TReadWrite> : NamedPipeServer<TReadWrite, TReadWrite> where TReadWrite : class
    {
        public NamedPipeServer(string pipeName) : base(pipeName, null)
        {
        }

        public NamedPipeServer(string pipeName, PipeSecurity pipeSecurity) : base(pipeName, pipeSecurity)
        {
        }
    }

    /// <summary>
    /// 管道通信服务类的基类
    /// </summary>
    /// <typeparam name="TRead"></typeparam>
    /// <typeparam name="TWrite"></typeparam>
    public class NamedPipeServer<TRead, TWrite> where TRead : class where TWrite : class
    {
        private readonly string _pipeName;
        private readonly PipeSecurity _pipeSecurity;
        private readonly Dictionary<string, NamedPipeConnection<TRead, TWrite>> connections = new Dictionary<string, NamedPipeConnection<TRead, TWrite>>();
        private int _nextPipeId;
        private volatile bool _shouldKeepRunning;
        private volatile bool _isRunning;

        public bool IsRunning { get { return this._isRunning; } }

        public event PipeExceptionEventHandler Error;

        public NamedPipeServer(string pipeName, PipeSecurity pipeSecurity)
        {
            _pipeName = pipeName;
            _pipeSecurity = pipeSecurity;
        }

        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            _shouldKeepRunning = true;
            var worker = new Worker();
            worker.Error += OnError;
            worker.DoWork(ListenSync);
        }


        /// <summary>
        /// Closes all open client connections and stops listening for new ones.
        /// </summary>
        public void Stop()
        {
            _shouldKeepRunning = false;

            lock (connections)
            {
                foreach (var client in connections.Values)
                {
                    client.Close();
                }
            }

            // If background thread is still listening for a client to connect,
            // initiate a dummy connection that will allow the thread to exit.
            //dummy connection will use the local server name.
            var dummyClient = new NamedPipeClient<TRead, TWrite>(_pipeName, ".");
            dummyClient.Start();
            dummyClient.WaitForConnection(TimeSpan.FromSeconds(2));
            dummyClient.Stop();
            dummyClient.WaitForDisconnection(TimeSpan.FromSeconds(2));
        }

        protected NamedPipeConnection<TRead, TWrite> GetConnection(string name)
        {
            lock (connections)
            {
                if (this.connections.ContainsKey(name))
                    return this.connections[name];
                return null;
            }
        }

        public PipeCommander GetCommanderWithResult(string name)
        {
            var conn = GetConnection(name);
            if (conn == null)
                return null;
            return new PipeCommander(conn);
        }

        /// <summary>
        /// 内部类,用于封装发送和获取响应的方法
        /// </summary>
        public class PipeCommander
        {
            private NamedPipeConnection<TRead, TWrite> Conn;
            public bool IsConnected
            {
                get => Conn.IsConnected; 
            }

            public PipeCommander(NamedPipeConnection<TRead, TWrite> conn)
            {
                this.Conn = conn;
            }

            public TRead GetResponse(TWrite message)
            {
                lock (Conn)
                {
                    Conn.PushMessage(message);
                    return Conn.ReadMessage();
                }
            }
        }

        #region Private methods

        private void ListenSync()
        {
            _isRunning = true;
            while (_shouldKeepRunning)
            {
                WaitForConnection(_pipeName, _pipeSecurity);
            }
            _isRunning = false;
        }

        private void WaitForConnection(string pipeName, PipeSecurity pipeSecurity)
        {
            NamedPipeServerStream handshakePipe = null;
            NamedPipeServerStream dataPipe = null;
            NamedPipeConnection<TRead, TWrite> connection = null;

            var connectionPipeName = GetNextConnectionPipeName(pipeName);

            try
            {
                //用于建立连接用的。
                handshakePipe = PipeServerFactory.CreateAndConnectPipe(pipeName, pipeSecurity);
                var handshakeWrapper = new PipeStreamWrapper<string, string>(handshakePipe);
                handshakeWrapper.WriteObject(connectionPipeName);
                handshakeWrapper.WaitForPipeDrain();
                var possessor = handshakeWrapper.ReadObject();

                handshakeWrapper.Close();

                // 用于请求数据用的
                dataPipe = PipeServerFactory.CreatePipe(connectionPipeName, pipeSecurity);
                dataPipe.WaitForConnection();

                // Add the client's connection to the list of connections
                connection = ConnectionFactory.CreateConnection<TRead, TWrite>(possessor, dataPipe);

                lock (connections)
                {
                    connections.Add(possessor, connection);
                }

            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Named pipe is broken or disconnected: {0}", e);

                Cleanup(handshakePipe);
                Cleanup(dataPipe);

                ClientOnDisconnected(connection);
            }
        }

        private void ClientOnDisconnected(NamedPipeConnection<TRead, TWrite> connection)
        {
            if (connection == null)
                return;

            lock (connections)
            {
                connections.Remove(connection.Name);
            }
        }

        /// <summary>
        ///     Invoked on the UI thread.
        /// </summary>
        /// <param name="exception"></param>
        private void OnError(Exception exception)
        {
            if (Error != null)
                Error(exception);
        }

        private string GetNextConnectionPipeName(string pipeName)
        {
            return string.Format("{0}_{1}", pipeName, ++_nextPipeId);
        }

        private static void Cleanup(NamedPipeServerStream pipe)
        {
            if (pipe == null) return;
            using (var x = pipe)
            {
                x.Close();
            }
        }

        #endregion
    }
}
