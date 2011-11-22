using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace DotNetSimaticDatabaseProtokollerLibrary.TCPIP
{
    public class TCPFunctionsAsync : IDisposable
    {
        private bool disposed = false;
        public void Dispose()
        {
            disposed = true;

            if (tcpClient != null)
                tcpClient.Close();
            if (tcpListener != null)
                tcpListener.Stop();
        }

        /// <summary>
        /// Status of the socket client
        /// </summary>
        public enum Status
        {
            /// <summary>client is not connected with the server</summary>
            STOPPED = 0,
            /// <summary>the client try to connect the server</summary>
            LISTENING = 1,
            /// <summary>the client try to connect the server</summary>
            CONNECTING = 2,
            /// <summary>the client is connected</summary>
            CONNECTED = 3,
        }

        #region Properties & Events

        public Status State = Status.STOPPED;

        public string Name { get; set; }

        public string LastErrorMessage { get; set; }

        public bool AutoReConnect { get; set; }

        /// <summary>
        /// On a Socket Server, allow multiple Clients 
        /// </summary>
        public bool AllowMultipleClients { get; set; }


        public event Action<TcpClient> ConnectionEstablished;
        public event Action<TcpClient> ConnectionClosed;
        public event Action<byte[]> DataRecieved;

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

        private int fixedLength = -1;

        private Dictionary<TcpClient, byte[]> readBytesPerCennection = new Dictionary<TcpClient, byte[]>();
        private Dictionary<TcpClient, int> readBytesCountPerCennection = new Dictionary<TcpClient, int>();
        //private Byte[] readBytes;
        //private int readPos = 0;

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

        public void Connect()
        {
            if (!this.connection_active)
            {
                this.State = Status.LISTENING;
                tcpListener = new TcpListener(local_ip, connection_port);
                tcpListener.Start();
                tcpListener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), tcpListener);
            }
            else
            {
                this.State = Status.CONNECTING;
                tcpClient = new TcpClient();
                tcpClient.BeginConnect(local_ip, connection_port, new AsyncCallback(DoBeginnConnectCallback), tcpClient);                
            }
        }

        public void Disconnect()
        { }

        public void DoBeginnConnectCallback(IAsyncResult ar)
        {
            try
            {
                this.State = Status.CONNECTED;
                var tcpc = (TcpClient)ar.AsyncState;
                tcpc.EndConnect(ar);

                if (ConnectionEstablished != null)
                    if (context == null)
                        ConnectionEstablished(tcpc);
                    else
                        context.Post(delegate { ConnectionEstablished(tcpc); }, null);

                beginRead(tcpc);
            }
            catch (Exception ex)
            {
                if (AsynchronousExceptionOccured != null)
                    AsynchronousExceptionOccured(this, new ThreadExceptionEventArgs(ex));

                string sMsg = "TCPSocketClientAndServer.DoBeginConnectCallback(IAsyncResult) - error: " + ex.Message;
                this.LastErrorMessage = sMsg;
            }
        }

        public void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            try
            {
                this.State = Status.CONNECTED;
                TcpListener listener = (TcpListener)ar.AsyncState;
                var akTcpClient = listener.EndAcceptTcpClient(ar);

                tcpClientsFromListener.Add(akTcpClient);

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

                string sMsg = "TCPSocketClientAndServer.DoAcceptTcpClientCallback(IAsyncResult) - error: " + ex.Message;
                this.LastErrorMessage = sMsg;
            }
        }

        private void beginRead(TcpClient akTcpClient)
        {
            try
            {
                if (readBytesPerCennection.ContainsKey(akTcpClient))
                {
                    readBytesPerCennection.Remove(akTcpClient);
                    readBytesCountPerCennection.Remove(akTcpClient);
                }
                byte[] readBytes;

                if (fixedLength < 0)
                    readBytes = new Byte[65536];
                else
                    readBytes = new Byte[fixedLength];

                readBytesPerCennection.Add(akTcpClient, readBytes);
                readBytesCountPerCennection.Add(akTcpClient, 0);

                akTcpClient.Client.BeginReceive(readBytes, 0, readBytes.Length, SocketFlags.None, new AsyncCallback(DoReadCallback), akTcpClient);
            }
            catch (Exception ex)
            {
                if (AsynchronousExceptionOccured != null)
                    AsynchronousExceptionOccured(this, new ThreadExceptionEventArgs(ex));

                string sMsg = "TCPSocketClientAndServer.beginRead(TcpClient) - error: " + ex.Message;
                this.LastErrorMessage = sMsg;
            }
        }

        void DoReadCallback(IAsyncResult ar)
        {
            if (!disposed)
                try
                {
                    TcpClient akTcpClient = (TcpClient) ar.AsyncState;

                    var readBytes = readBytesPerCennection[akTcpClient];
                    var readPos = readBytesCountPerCennection[akTcpClient];
                    
                    int cnt = akTcpClient.Client.EndReceive(ar);
                    if (cnt > 0 && fixedLength < 0)
                    {
                        byte[] rec = new byte[cnt];
                        Array.Copy(readBytes, 0, rec, 0, cnt);
                        if (context == null)
                            DataRecieved(rec);
                        else
                            context.Post(delegate { DataRecieved(rec); }, null);
                        beginRead(akTcpClient);
                    }
                    else
                    {
                        readPos += cnt;
                        readBytesCountPerCennection[akTcpClient] = readPos;
                        if (readPos < fixedLength)
                            akTcpClient.Client.BeginReceive(readBytes, readPos, readBytes.Length - readPos, SocketFlags.None, new AsyncCallback(DoReadCallback), akTcpClient);
                        else
                        {
                            if (context == null)
                                DataRecieved(readBytes);
                            else
                                context.Post(delegate { DataRecieved(readBytes); }, null);
                            beginRead(akTcpClient);
                        }
                    }

                }
                catch (Exception ex)
                {
                    if (AsynchronousExceptionOccured != null)
                        AsynchronousExceptionOccured(this, new ThreadExceptionEventArgs(ex));

                    string sMsg = "TCPSocketClientAndServer.DoReadCallback(IAsyncResult) - error: " + ex.Message;
                    this.LastErrorMessage = sMsg;
                }
        }

        public bool SendData(string telegramm)
        {
            return SendData(Encoding.ASCII.GetBytes(telegramm));
        }

        public bool SendData(byte[] telegramm)
        {
            try
            {
                if (tcpClient == null)
                    throw new Exception("Send not possible, not connected");
                NetworkStream stream = tcpClient.GetStream();
                stream.Write(telegramm, 0, telegramm.Length);
                return true;
            }
            catch (Exception ex)
            {
                string sMsg = "TCPSocketClientAndServer.SendData(byte[]) - error: " + ex.Message;
                this.LastErrorMessage = sMsg;
            }
            return false;
        }
    }
}