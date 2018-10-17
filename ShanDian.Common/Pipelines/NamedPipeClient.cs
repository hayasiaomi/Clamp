using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using ShanDian.Common.Commands;
using ShanDian.Common.Pipelines.IO;
using ShanDian.Common.Pipelines.Threading;

namespace ShanDian.Common.Pipelines
{
    /// <summary>
    /// 管道客户端
    /// </summary>
    /// <typeparam name="TReadWrite"></typeparam>
    public class NamedPipeClient<TReadWrite> : NamedPipeClient<TReadWrite, TReadWrite> where TReadWrite : class
    {
        public NamedPipeClient(string pipeName, string serverName = ".") : base(pipeName, serverName)
        {

        }
    }


    public class NamedPipeClient<TRead, TWrite> where TRead : class where TWrite : class
    {
        private readonly string _pipeName;
        private NamedPipeConnection<TRead, TWrite> _connection;
        private readonly AutoResetEvent _connected = new AutoResetEvent(false);
        private readonly AutoResetEvent _disconnected = new AutoResetEvent(false);
        private volatile bool _closedExplicitly;
        private string _serverName;
        private bool _notifiedSucceeded;

        public bool AutoReconnect { get; set; }
        public event ConnectionEventHandler<TRead, TWrite> Disconnected;
        public event PipeExceptionEventHandler Error;

        public string Possessor { set; get; }

        public NamedPipeClient(string pipeName, string serverName)
        {
            _pipeName = pipeName;
            _serverName = serverName;
            AutoReconnect = true;
        }

        public void Start()
        {
            _closedExplicitly = false;
            var worker = new Worker();
            worker.Error += OnError;
            worker.DoWork(ListenSync);
        }

        public void Stop()
        {
            _closedExplicitly = true;
            if (_connection != null)
                _connection.Close();
        }


        public void WaitForConnection(TimeSpan timeout)
        {
            _connected.WaitOne(timeout);
        }


        public void WaitForDisconnection(TimeSpan timeout)
        {
            _disconnected.WaitOne(timeout);
        }


        #region Private methods

        private void ListenSync()
        {
            var handshake = PipeClientFactory.Connect<string, string>(_pipeName, _serverName);

            var dataPipeName = handshake.ReadObject();
            handshake.WriteObject(this.Possessor);
            handshake.WaitForPipeDrain();

            handshake.Close();

            var dataPipe = PipeClientFactory.CreateAndConnectPipe(dataPipeName, _serverName);

            _connection = ConnectionFactory.CreateConnection<TRead, TWrite>(this.Possessor, dataPipe);

            var readWorker = new Worker();
            readWorker.Succeeded += OnSucceeded;
            readWorker.Error += OnError;
            readWorker.DoWork(ReadPipe);

            _connected.Set();
        }


        private void OnSucceeded()
        {
            if (_notifiedSucceeded)
                return;

            _notifiedSucceeded = true;

            if (Disconnected != null)
                Disconnected(this._connection);

            _disconnected.Set();

            // Reconnect
            if (AutoReconnect && !_closedExplicitly)
                Start();
        }

        private void ReadPipe()
        {
            while (this._connection.IsConnected && this._connection.CanRead)
            {
                try
                {
                    var obj = this._connection.ReadMessage();

                    this.OnReceiveMessage(this._connection, obj);
                }
                catch(Exception ex)
                {
                   
                }
            }
        }

        protected virtual void OnReceiveMessage(NamedPipeConnection<TRead, TWrite> connection, TRead message)
        {

        }

        private void OnError(Exception exception)
        {
            if (Error != null)
                Error(exception);
        }

        #endregion
    }


}
