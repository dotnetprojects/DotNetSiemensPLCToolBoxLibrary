using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace DotNetSimaticDatabaseProtokollerLibrary.Protocolling
{
    public class TCPFunctionsAsync : IDisposable
    {
        public void Dispose()
        {
            if (tcpClient != null)
                tcpClient.Close();
            if (tcpListener!=null)
                tcpListener.Stop();
        }

        public event ThreadExceptionEventHandler AsynchronousExceptionOccured;

        private TcpClient tcpClient;
        private TcpListener tcpListener;

        private IPAddress local_ip;
        private int connection_port;
        private bool connection_active;
        private SynchronizationContext context;

        private int fixedLength = -1;

        public delegate void TegramRecievedEventHandler(byte[] telegramm);
        public event TegramRecievedEventHandler DataRecieved;

        public event Action ConnectionEstablished;        
        public event Action ConnectionClosed;

        public TCPFunctionsAsync(SynchronizationContext context, IPAddress IP, int connection_port, bool connection_active)
        {            
            this.context = context;
            if (context == null)
                context = new SynchronizationContext();

            this.connection_port = connection_port;
            this.connection_active = connection_active;

            local_ip = IP; // Dns.GetHostByName(Dns.GetHostName()).AddressList[0];
        }

        public TCPFunctionsAsync(SynchronizationContext context, IPAddress IP, int connection_port, bool connection_active, int FixedLength)
            : this(context, IP, connection_port, connection_active)
        {
            fixedLength = FixedLength;
        }

        public void Connect()
        {
            if (!this.connection_active)
            {
                tcpListener = new TcpListener(local_ip, connection_port);
                tcpListener.Start();
                tcpListener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), tcpListener);
            }
            else
            {
                tcpClient = new TcpClient();
                tcpClient.BeginConnect(local_ip, connection_port, new AsyncCallback(DoBeginnConnectCallback), tcpClient);
            }
        }

        public void DoBeginnConnectCallback(IAsyncResult ar)
        {
            try
            {
                var tcpc = (TcpClient)ar.AsyncState;
                tcpc.EndConnect(ar);

                if (ConnectionEstablished != null)
                    context.Post(delegate { ConnectionEstablished(); }, null);                   

                beginRead();
            }
            catch (Exception ex)
            {
                if (AsynchronousExceptionOccured != null)
                    AsynchronousExceptionOccured(this, new ThreadExceptionEventArgs(ex));
            }
        }

        public void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            try
            {
                TcpListener listener = (TcpListener) ar.AsyncState;
                tcpClient = listener.EndAcceptTcpClient(ar);

                if (ConnectionEstablished != null)
                    context.Post(delegate { ConnectionEstablished(); }, null);                   

                //Multiple Clients may Connect???
                //tcpListener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);

                beginRead();
            }
            catch(Exception ex)
            {
                if (AsynchronousExceptionOccured != null)
                    AsynchronousExceptionOccured(this, new ThreadExceptionEventArgs(ex));
            }
        }


        private Byte[] readBytes;
        private int readPos = 0;

        private void beginRead()
        {
            try
            {
                if (fixedLength<0)
                    readBytes = new Byte[65536];
                else
                    readBytes = new Byte[fixedLength];

                tcpClient.Client.BeginReceive(readBytes, 0, readBytes.Length, SocketFlags.None, new AsyncCallback(DoReadCallback), tcpClient);
            }
            catch (Exception ex)
            {
                if (AsynchronousExceptionOccured != null)
                    AsynchronousExceptionOccured(this, new ThreadExceptionEventArgs(ex));
            }
        }

        void DoReadCallback(IAsyncResult ar)
        {
            try
            {
                TcpClient tcpClient = (TcpClient) ar.AsyncState;

                int cnt = tcpClient.Client.EndReceive(ar);

                if (cnt > 0 && fixedLength<0)
                {
                    byte[] rec = new byte[cnt];
                    Array.Copy(readBytes, 0, rec, 0, cnt);
                    context.Post(delegate { DataRecieved(rec); }, null);
                    beginRead();
                }
                else
                {
                    readPos += cnt;
                    if (readPos < fixedLength)
                        tcpClient.Client.BeginReceive(readBytes, readPos, readBytes.Length - readPos, SocketFlags.None, new AsyncCallback(DoReadCallback), tcpClient);
                    else
                    {
                        context.Post(delegate { DataRecieved(readBytes); }, null);
                        beginRead();
                    }
                }
                
            }
            catch (Exception ex)
            {
                if (AsynchronousExceptionOccured != null)
                    AsynchronousExceptionOccured(this, new ThreadExceptionEventArgs(ex));
            }
        }

        public void SendData(string telegramm)
        {
            SendData(Encoding.ASCII.GetBytes(telegramm));
        }

        public void SendData(byte[] telegramm)
        {
            if (tcpClient == null)
                throw new Exception("Send not possible, not connected");
            NetworkStream stream = tcpClient.GetStream();
            stream.Write(telegramm, 0, telegramm.Length);
        }
    }
}