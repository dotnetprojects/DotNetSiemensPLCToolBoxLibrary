using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace DotNetSimaticDatabaseProtokollerLibrary.Protocolling
{
    public class TCPFunctions : IDisposable
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

        private Thread ReceiveDataThread;

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

        private List<Thread> Threads = new List<Thread>();

        public TCPFunctions(SynchronizationContext context, IPAddress PlcIP, int connection_port, bool connection_active, int tele_length)
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
            ConnectSend();
        }

        private void ConnectSend()
        {
            if (!this.connection_active)
            {
                Thread send_wait_for_conn_thread = new Thread(new ThreadStart(InitSendClientPassive));
                send_wait_for_conn_thread.Name = "send_wait_for_conn_thread";
                send_wait_for_conn_thread.Start();
                Threads.Add(send_wait_for_conn_thread);
            }
            else
            {
                Thread send_wait_for_conn_thread = new Thread(new ThreadStart(InitSendClientActive));
                send_wait_for_conn_thread.Name = "send_wait_for_conn_thread";
                send_wait_for_conn_thread.Start();
                Threads.Add(send_wait_for_conn_thread);
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

        public void InitSendClientPassive()
        {
            bool SendClosed = false;
            while (true)
            {                
                if (ConnectionClosed != null && SendClosed)
                {
                    SendClosed = false;
                    context.Post(delegate { ConnectionClosed(1); }, null);
                }

                if (tcpClient == null || tcpClient.Connected == false)
                {
                    if (tcpClient!=null)
                        tcpClient.Close();

                    TcpListener server = new TcpListener(local_ip, connection_port);
                    server.Start();

                    tcpClient = null;
                    tcpClient = server.AcceptTcpClient();

                    SendClosed = true;
                    
                    if (ConnectionEstablished != null)
                        context.Post(delegate { ConnectionEstablished(1); }, null);

                    //Thread for Recieving Data
                    if (ReceiveDataThread != null)
                    {
                        ReceiveDataThread.Abort();
                        Threads.Remove(ReceiveDataThread);
                    }
                    ReceiveDataThread = new Thread(ReceiveDataSendClient);
                    ReceiveDataThread.Name = "PReceiveDataThread";
                    ReceiveDataThread.Start();
                    Threads.Add(ReceiveDataThread);
                    //End Thread
                }              
            }
        }

        public void InitSendClientActive()
        {
            bool SendClosed = false;
            while (true)
            {
                try
                {                    
                    if (tcpClient == null || tcpClient.Connected == false)
                    {
                        if (SendClosed)
                        {
                            SendClosed = false;
                            context.Post(delegate { ConnectionClosed(1); }, null);
                        }

                        tcpClient = null;
                        
                        tcpClient = new TcpClient(plc_ip.ToString(), connection_port);

                        SendClosed = true;

                        if (ConnectionEstablished != null)
                            context.Post(delegate { ConnectionEstablished(1); }, null);

                        //Thread for Recieving Data
                        if (ReceiveDataThread != null)
                        {
                            ReceiveDataThread.Abort();
                            Threads.Remove(ReceiveDataThread);
                        }
                        ReceiveDataThread = new Thread(ReceiveDataSendClient);
                        ReceiveDataThread.Name = "AReceiveDataThread";
                        ReceiveDataThread.Start();
                        Threads.Add(ReceiveDataThread);
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

        public void ReceiveDataSendClient()
        {
            Byte[] bytes = new Byte[tele_length];
            
            NetworkStream stream = null;

            try
            {
                while (true)
                {
                    try
                    {
                        if (stream == null)
                            stream = tcpClient.GetStream();
                        int len = stream.Read(bytes, 0, tele_length);

                        if (TelegrammRecievedSend != null && len > 0)
                            context.Post(delegate { TelegrammRecievedSend(bytes); }, null);
                    }
                    catch (Exception)
                    {
                        if (stream!=null)
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