using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Networking
{
    public class TCPFunctionsAsync : IDisposable
    {
        private bool disposed = false;
        public void Dispose()
        {
            disposed = true;

            Stop();
        }

        /// <summary>
        /// Status of the socket client
        /// </summary>
        public enum Status
        {
            /// <summary>client is not connected with the server</summary>
            DISCONNECTED = 0,
            /// <summary>the client try to connect the server</summary>
            LISTENING = 1,
            /// <summary>the client try to connect the server</summary>
            CONNECTING = 2,
            /// <summary>the client is connected</summary>
            CONNECTED = 3,
        }

        #region Properties & Events

        public Status State = Status.DISCONNECTED;

        public string StateString
        {
            get
            {
                string retVal = State.ToString();
                if (tcpClient != null)
                    retVal = "Local-Port: " + tcpClient.Client.LocalEndPoint + ": " + retVal;
                return retVal;
            }
        }

        public string Name { get; set; }

        public string LastErrorMessage { get; set; }

        public string LastMessage { get; set; }

        private bool _autoReConnect = true;
        public bool AutoReConnect
        {
            get { return _autoReConnect; }
            set { _autoReConnect = value; }
        }

        /// <summary>
        /// On a Socket Server, allow multiple Clients 
        /// </summary>
        public bool AllowMultipleClients { get; set; }


        public event Action<TcpClient> ConnectionEstablished;
        public event Action<TcpClient> ConnectionClosed;
        public event Action<byte[], TcpClient> DataRecieved;

        public event ThreadExceptionEventHandler AsynchronousExceptionOccured;

        #endregion

        #region Private Fields

        private TcpClient tcpClient;
        private List<TcpClient> tcpClientsFromListener = new List<TcpClient>();
        private TcpListener tcpListener;

        private IPAddress local_ip;
        private int connection_port;
        private bool connection_active;
        private SynchronizationContext context;

        public bool UseKeepAlive { get; set; }

        public bool Started { get { return State != Status.DISCONNECTED; } }
        private int fixedLength = -1;

        private Dictionary<TcpClient, byte[]> readBytesPerCennection = new Dictionary<TcpClient, byte[]>();
        private Dictionary<TcpClient, int> readBytesCountPerCennection = new Dictionary<TcpClient, int>();
        //private Byte[] readBytes;
        //private int readPos = 0;

        private object lockObject = new object();

        #endregion

        #region Konstruktoren

        public TCPFunctionsAsync(SynchronizationContext context, IPAddress IP, int connection_port, bool connection_active)
        {
            this.context = context;

            this.connection_port = connection_port;
            this.connection_active = connection_active;

            local_ip = IP; // Dns.GetHostByName(Dns.GetHostName()).AddressList[0];
        }

        public TCPFunctionsAsync(SynchronizationContext context, IPAddress IP, int connection_port, bool connection_active, int FixedLength)
            : this(context, IP, connection_port, connection_active)
        {
            fixedLength = FixedLength;
        }

        public TCPFunctionsAsync(SynchronizationContext context, IPEndPoint endPoint, bool connection_active, int FixedLength)
            : this(context, endPoint.Address, endPoint.Port, connection_active, FixedLength)
        { }

        public TCPFunctionsAsync(SynchronizationContext context, IPEndPoint endPoint, bool connection_active)
            : this(context, endPoint.Address, endPoint.Port, connection_active)
        { }

        #endregion

        public void StartAsync()
        {
            Start();
        }

        public void Start()
        {
            if (!this.connection_active)
            {
                this.State = Status.LISTENING;

                //if (tcpListener != null)
                //    tcpListener.Stop();

                if (local_ip == null)
                    local_ip = IPAddress.Any;

                if (tcpListener == null)
                {
                    tcpListener = new TcpListener(local_ip, connection_port);
                    tcpListener.Start();
                }
                tcpListener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), tcpListener);
            }
            else
            {
                lock (lockObject)
                {
                    if (this.State != Status.CONNECTING)
                    {
                        if (tcpClient != null)
                        {
                            tcpClient.Close();
                            tcpClient = null;
                        }

                        this.State = Status.CONNECTING;

                        var tcpc = new TcpClient();
                        tcpc.BeginConnect(local_ip, connection_port, new AsyncCallback(DoBeginnConnectCallback), tcpc);
                    }
                }
            }
        }

        public void Stop()
        {
            if (tcpListener != null)
                tcpListener.Stop();

            tcpListener = null;
            
            StopInternal();
        }

        private void StopInternal()
        {
            if (tcpClient != null)
                tcpClient.Close();


            foreach (var client in tcpClientsFromListener)
            {
                client.Close();
            }
            tcpClientsFromListener.Clear();

            tcpClient = null;
            
            this.State = Status.DISCONNECTED;
        }

        public void DoBeginnConnectCallback(IAsyncResult ar)
        {
            TcpClient tcpc = null;
            try
            {
                tcpc = (TcpClient)ar.AsyncState;
                tcpc.EndConnect(ar);

                this.State = Status.CONNECTED;

                if (tcpClient == null)
                    tcpClient = tcpc;
                else
                {
                    throw new Exception("Error: TCP Client already assigned");
                }

                if (UseKeepAlive)
                    tcpc.Client.SetKeepAlive(50, 100);

                if (ConnectionEstablished != null)
                    if (context == null)
                        ConnectionEstablished(tcpc);
                    else
                        context.Post(delegate { ConnectionEstablished(tcpc); }, null);

                beginRead(tcpc);
            }
            catch (Exception ex)
            {
                string sMsg = DateTime.Now.ToString() + " - " + "TCPSocketClientAndServer.DoBeginConnectCallback(IAsyncResult) - error: " + ex.Message;
                this.LastErrorMessage = sMsg;

                if (ex is SocketException)// && (((SocketException)ex).ErrorCode == 10060 || ((SocketException)ex).ErrorCode == 10061 || ((SocketException)ex).ErrorCode == 10065))
                {
                    if (tcpc != null)
                        tcpc.Close();

                    if (ConnectionClosed != null && this.State == Status.CONNECTED)
                        if (context == null)
                            ConnectionClosed(tcpClient);
                        else
                            context.Post(delegate { ConnectionClosed(tcpClient); }, null);

                    StopInternal();

                    if (!AllowMultipleClients || connection_active)
                        if (AutoReConnect) Start();
                }
                else
                {
                    if (AsynchronousExceptionOccured != null)
                        AsynchronousExceptionOccured(this, new ThreadExceptionEventArgs(ex));
                }
            }
        }

        public void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            try
            {
                this.State = Status.CONNECTED;
                TcpListener listener = (TcpListener)ar.AsyncState;
                var akTcpClient = listener.EndAcceptTcpClient(ar);

                if (UseKeepAlive)
                    akTcpClient.Client.SetKeepAlive(50, 100);

                tcpClientsFromListener.Add(akTcpClient);

                if (!AllowMultipleClients)
                    tcpClient = akTcpClient;

                if (ConnectionEstablished != null)
                    if (context == null)
                        ConnectionEstablished(tcpClient);
                    else
                        context.Post(delegate { ConnectionEstablished(tcpClient); }, null);

                if (AllowMultipleClients)
                    tcpListener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);

                beginRead(akTcpClient);
            }
            catch (Exception ex)
            {
                if (AsynchronousExceptionOccured != null)
                    AsynchronousExceptionOccured(this, new ThreadExceptionEventArgs(ex));

                string sMsg = DateTime.Now.ToString() + " - " + "TCPSocketClientAndServer.DoAcceptTcpClientCallback(IAsyncResult) - error: " + ex.Message;
                this.LastErrorMessage = sMsg;
            }
        }

        private void beginRead(TcpClient akTcpClient)
        {
            if (!disposed && akTcpClient.Connected == false)
            {
                if (ConnectionClosed != null && this.State == Status.CONNECTED)
                    if (context == null)
                        ConnectionClosed(tcpClient);
                    else
                        context.Post(delegate { ConnectionClosed(tcpClient); }, null);

                //if (this.State != Status.CONNECTING)
                StopInternal();

                if (!AllowMultipleClients || connection_active)
                    if (AutoReConnect && this.State != Status.CONNECTING) Start();
                return;
            }

            if (!disposed)
            {
                try
                {
                    if (readBytesPerCennection.ContainsKey(akTcpClient))
                    {
                        readBytesPerCennection.Remove(akTcpClient);
                        readBytesCountPerCennection.Remove(akTcpClient);
                    }
                    byte[] readBytes;

                    if (fixedLength <= 0)
                        readBytes = new Byte[65536];
                    else
                        readBytes = new Byte[fixedLength];

                    readBytesPerCennection.Add(akTcpClient, readBytes);
                    readBytesCountPerCennection.Add(akTcpClient, 0);

                    akTcpClient.Client.BeginReceive(readBytes, 0, readBytes.Length, SocketFlags.None, new AsyncCallback(DoReadCallback), akTcpClient);
                }
                catch (Exception ex)
                {

                    string sMsg = DateTime.Now.ToString() + " - " + "TCPSocketClientAndServer.beginRead(TcpClient) - error: " + ex.Message;
                    this.LastErrorMessage = sMsg;

                    if (ex is SocketException) // && ((SocketException)ex).ErrorCode == 10060)
                    {
                        if (ConnectionClosed != null && this.State == Status.CONNECTED)
                            if (context == null)
                                ConnectionClosed(tcpClient);
                            else
                                context.Post(delegate { ConnectionClosed(tcpClient); }, null);

                        //if (this.State != Status.CONNECTING)
                        StopInternal();

                        if (!AllowMultipleClients || connection_active)
                            if (AutoReConnect && this.State != Status.CONNECTING) Start();
                    }
                    else
                    {
                        if (AsynchronousExceptionOccured != null)
                            AsynchronousExceptionOccured(this, new ThreadExceptionEventArgs(ex));
                    }
                }
            }
        }

        void DoReadCallback(IAsyncResult ar)
        {
            TcpClient akTcpClient = (TcpClient)ar.AsyncState;

            if (!disposed && (akTcpClient.Client == null || akTcpClient.Connected == false))
            {
                if (ConnectionClosed != null && this.State == Status.CONNECTED)
                    if (context == null)
                        ConnectionClosed(tcpClient);
                    else
                        context.Post(delegate { ConnectionClosed(tcpClient); }, null);

                //if (this.State != Status.CONNECTING)
                StopInternal();

                if (!AllowMultipleClients || connection_active)
                    if (AutoReConnect && this.State != Status.CONNECTING) Start();
                return;
            }

            if (!disposed)
                try
                {


                    var readBytes = readBytesPerCennection[akTcpClient];
                    var readPos = readBytesCountPerCennection[akTcpClient];

                    int cnt = akTcpClient.Client.EndReceive(ar);
                    if (cnt > 0 && fixedLength <= 0)
                    {
                        byte[] rec = new byte[cnt];
                        Array.Copy(readBytes, 0, rec, 0, cnt);
                        LastMessage = DateTime.Now.ToString() + " - Recieved " + readBytes.Length + " Bytes";
                        if (context == null)
                            DataRecieved(rec, akTcpClient);
                        else
                            context.Post(delegate { DataRecieved(rec, akTcpClient); }, null);
                        beginRead(akTcpClient);
                    }
                    else if (cnt == 0)
                    {
                        if (ConnectionClosed != null && this.State == Status.CONNECTED)
                            if (context == null)
                                ConnectionClosed(tcpClient);
                            else
                                context.Post(delegate { ConnectionClosed(tcpClient); }, null);

                        //if (this.State != Status.CONNECTING)
                        StopInternal();

                        if (!AllowMultipleClients || connection_active)
                            if (AutoReConnect && this.State != Status.CONNECTING) Start();
                    }
                    else
                    {
                        readPos += cnt;
                        readBytesCountPerCennection[akTcpClient] = readPos;
                        if (readPos < fixedLength)
                            akTcpClient.Client.BeginReceive(readBytes, readPos, readBytes.Length - readPos, SocketFlags.None, new AsyncCallback(DoReadCallback), akTcpClient);
                        else
                        {
                            LastMessage = DateTime.Now.ToString() + " - Recieved " + readBytes.Length + " Bytes";
                            if (context == null)
                                DataRecieved(readBytes, akTcpClient);
                            else
                                context.Post(delegate { DataRecieved(readBytes, akTcpClient); }, null);
                            beginRead(akTcpClient);
                        }
                    }

                }
                catch (Exception ex)
                {

                    string sMsg = DateTime.Now.ToString() + " - " + "TCPSocketClientAndServer.DoReadCallback(IAsyncResult) - error: " + ex.Message;
                    this.LastErrorMessage = sMsg;

                    if (ex is SocketException) // && ((SocketException)ex).ErrorCode == 10060)
                    {
                        if (ConnectionClosed != null && this.State == Status.CONNECTED)
                            if (context == null)
                                ConnectionClosed(tcpClient);
                            else
                                context.Post(delegate { ConnectionClosed(tcpClient); }, null);

                        //if (this.State != Status.CONNECTING)
                        StopInternal();

                        if (!AllowMultipleClients || connection_active)
                            if (AutoReConnect && this.State != Status.CONNECTING) Start();
                    }
                    else
                    {
                        if (AsynchronousExceptionOccured != null)
                            AsynchronousExceptionOccured(this, new ThreadExceptionEventArgs(ex));
                    }
                }
        }

        public bool SendStringData(string telegramm)
        {
            return SendData(telegramm);
        }

        public bool SendData(string telegramm)
        {
            return SendData(Encoding.ASCII.GetBytes(telegramm));
        }

        public bool SendData(string telegramm, TcpClient client)
        {
            return SendData(Encoding.ASCII.GetBytes(telegramm), client);
        }

        public bool SendData(byte[] telegramm)
        {
            if (AllowMultipleClients && !connection_active)
            {
                foreach (var clnt in tcpClientsFromListener)
                {
                    SendData(telegramm, clnt);
                }
                return true;
            }
            else
                return SendData(telegramm, tcpClient);
        }

        public bool SendData(byte[] telegramm, TcpClient client)
        {
            try
            {
                if (tcpClient == null)
                    throw new Exception("Send not possible, not connected");
                NetworkStream stream = client.GetStream();
                stream.Write(telegramm, 0, telegramm.Length);
                return true;
            }
            catch (Exception ex)
            {
                string sMsg = DateTime.Now.ToString() + " - " + "TCPSocketClientAndServer.SendData(byte[]) - error: " + ex.Message;
                this.LastErrorMessage = sMsg;

                if (ex is SocketException || ex is InvalidOperationException) // && ((SocketException)ex).ErrorCode == 10060)
                {
                    if (ConnectionClosed != null && this.State == Status.CONNECTED)
                        if (context == null)
                            ConnectionClosed(tcpClient);
                        else
                            context.Post(delegate { ConnectionClosed(tcpClient); }, null);

                    if (this.State != Status.CONNECTING)
                        StopInternal();

                    if (!AllowMultipleClients || connection_active)
                        if (AutoReConnect && this.State != Status.CONNECTING) Start();
                }


            }
            return false;
        }
    }
}