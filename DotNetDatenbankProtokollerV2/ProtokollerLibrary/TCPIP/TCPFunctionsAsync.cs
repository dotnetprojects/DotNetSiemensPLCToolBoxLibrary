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

        private IPAddress plc_ip;
        private IPAddress local_ip;
        private int connection_port;
        private bool connection_active;
        private SynchronizationContext context;

        private int tele_length;

        public delegate void TegramRecievedEventHandler(byte[] telegramm);
        public event TegramRecievedEventHandler TelegrammRecievedSend;

        public delegate void ConnectionEstablishedEventHandler(int Number);
        public event ConnectionEstablishedEventHandler ConnectionEstablished;

        public delegate void ConnectionClosedEventHandler(int Number);
        public event ConnectionClosedEventHandler ConnectionClosed;

        public TCPFunctionsAsync(SynchronizationContext context, IPAddress PlcIP, int connection_port, bool connection_active, int tele_length)
        {
            this.tele_length = tele_length;

            this.plc_ip = PlcIP;
            this.context = context;
            this.connection_port = connection_port;
            this.connection_active = connection_active;

            local_ip = Dns.GetHostByName(Dns.GetHostName()).AddressList[0];
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
                tcpClient = new TcpClient(plc_ip.ToString(), connection_port);                
                beginRead();
            }
        }

        public void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            try
            {
                TcpListener listener = (TcpListener) ar.AsyncState;
                tcpClient = listener.EndAcceptTcpClient(ar);

                beginRead();
            }
            catch(Exception ex)
            {
                if (AsynchronousExceptionOccured != null)
                    AsynchronousExceptionOccured(this, new ThreadExceptionEventArgs(ex));
            }
        }


        private Byte[] readBytes;
        private int readBytesCount;

        private void beginRead()
        {
            try
            {
                readBytes = new Byte[tele_length];

                readBytesCount = 0;
                tcpClient.Client.BeginReceive(readBytes, readBytesCount, tele_length - readBytesCount, SocketFlags.None, new AsyncCallback(DoReadCallback), tcpClient);
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

                readBytesCount += cnt;
                if (readBytesCount < tele_length)
                    tcpClient.Client.BeginReceive(readBytes, readBytesCount, tele_length - readBytesCount, SocketFlags.None, new AsyncCallback(DoReadCallback), tcpClient);
                else
                {
                    context.Post(delegate { TelegrammRecievedSend(readBytes); }, null);
                    beginRead();
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