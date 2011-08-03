using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library
{
    internal class TCPConnection : IDisposable
    {
        public void Dispose()
        {
            foreach (var thread in Threads)
            {
                thread.Abort();
            }
            Threads = null;

            if (tcpClient != null)
                tcpClient.Close();
        }

        private TcpClient tcpClient;

        private Thread ReceiveDataSendClientThread;

        private IPAddress plc_ip;

        private int connection_port;
        private SynchronizationContext context;

        public delegate void TegramRecievedEventHandler(byte[] telegramm);
        public event TegramRecievedEventHandler TelegrammRecievedSend;

        public delegate void ConnectionEstablishedEventHandler(int Number);
        public event ConnectionEstablishedEventHandler ConnectionEstablished;

        public delegate void ConnectionClosedEventHandler(int Number);
        public event ConnectionClosedEventHandler ConnectionClosed;

        private List<Thread> Threads = new List<Thread>();

        public TCPConnection(SynchronizationContext context, IPAddress PlcIP, int send_connection_port)
        {
            this.plc_ip = PlcIP;
            this.context = context;
            if (context == null)
                this.context = new SynchronizationContext();
            this.connection_port = send_connection_port;

        }

        public void Connect()
        {
            connectClient();
        }

        private void connectClient()
        {
            Thread send_wait_for_conn_thread = new Thread(new ThreadStart(initClient));
            send_wait_for_conn_thread.Name = "send_wait_for_conn_thread";
            send_wait_for_conn_thread.Start();
            Threads.Add(send_wait_for_conn_thread);

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

        public void initClient()
        {
            bool closed = false;
            while (true)
            {
                try
                {
                    if (ConnectionClosed != null && closed)
                    {
                        closed = false;
                        context.Post(delegate { ConnectionClosed(1); }, null);
                    }

                    if (tcpClient == null || tcpClient.Connected == false)
                    {
                        if (tcpClient == null)
                            tcpClient.Close();

                        tcpClient = new TcpClient(plc_ip.ToString(), connection_port);
                        closed = true;

                        if (ConnectionEstablished != null)
                            context.Post(delegate { ConnectionEstablished(1); }, null);

                        //Thread for Recieving Data
                        if (ReceiveDataSendClientThread != null)
                        {
                            ReceiveDataSendClientThread.Abort();
                            Threads.Remove(ReceiveDataSendClientThread);
                        }
                        ReceiveDataSendClientThread = new Thread(ReceiveData);
                        ReceiveDataSendClientThread.Name = "ReceiveDataThread";
                        ReceiveDataSendClientThread.Start();
                        Threads.Add(ReceiveDataSendClientThread);
                        //End Thread

                    }
                    Thread.Sleep(1000);
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch
                {
                    Thread.Sleep(5000);
                }
            }
        }

        public void ReceiveData()
        {
            Byte[] bytes = new Byte[4];
            NetworkStream stream = null;
            try
            {
                while (true)
                {
                    try
                    {
                        if (stream == null)
                            stream = tcpClient.GetStream();
                        int len = stream.Read(bytes, 0, 4);

                        if (len > 3)
                        {
                            int size = bytes[3] + 0x100 * bytes[2];
                            Byte[] gesbytes = new Byte[size];
                            len = stream.Read(gesbytes, 4, size);
                            Array.Copy(bytes, 0, gesbytes, 0, 4);

                            if (TelegrammRecievedSend != null && len > 0)
                                context.Post(delegate { TelegrammRecievedSend(gesbytes); }, null);
                        }

                        //stream.Dispose();
                    }
                    catch (Exception)
                    {
                        if (stream != null)
                        {
                            stream.Dispose();
                            stream = null;
                        }
                    }
                }
            }
            catch (ThreadAbortException) //Kill this Thread when a Exception occurs!
            {
                if (stream != null)
                {
                    stream.Dispose();
                    stream = null;
                }
            }
        }
    }
}
