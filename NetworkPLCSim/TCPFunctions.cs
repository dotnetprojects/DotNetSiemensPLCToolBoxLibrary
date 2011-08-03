using System;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace Kopplungstester
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

            if (Send_Client != null)
                Send_Client.Close();

            if (Recieve_Client != null)
                Recieve_Client.Close();
        }


        private TcpClient Send_Client;
        private TcpClient Recieve_Client;

        private Thread ReceiveDataSendClientThread;
        private Thread ReceiveDataRecieveClientThread;

        private IPAddress plc_ip;
        private IPAddress local_ip;
        private bool onlyOneConnection;
        private int send_connection_port;
        private bool send_connection_active;
        private int recieve_connection_port;
        private bool recieve_connection_active;
        private SynchronizationContext context;

        private int send_tele_len;
        private int recieve_tele_len;

        public delegate void TegramRecievedEventHandler(byte[] telegramm);
        public event TegramRecievedEventHandler TelegrammRecievedSend;
        public event TegramRecievedEventHandler TelegrammRecievedRecieve;

        public delegate void ConnectionEstablishedEventHandler(int Number);
        public event ConnectionEstablishedEventHandler ConnectionEstablished;

        public delegate void ConnectionClosedEventHandler(int Number);
        public event ConnectionClosedEventHandler ConnectionClosed;

        private List<Thread> Threads = new List<Thread>();               

        public TCPFunctions(SynchronizationContext context, IPAddress PlcIP, bool onlyOneConnection, int send_connection_port, bool send_connection_active, int send_tele_len, int recieve_connection_port, bool recieve_connection_active, int recieve_tele_len)
        {
            this.send_tele_len = send_tele_len;
            this.recieve_tele_len = recieve_tele_len;

            this.plc_ip = PlcIP;
            this.context = context;
            this.onlyOneConnection = onlyOneConnection;
            this.send_connection_port = send_connection_port;
            this.send_connection_active = send_connection_active;
            this.recieve_connection_port = recieve_connection_port;
            this.recieve_connection_active = recieve_connection_active;

            local_ip = Dns.GetHostByName(Dns.GetHostName()).AddressList[0];
        }

        public void Connect()
        {
            ConnectSend();

            ConnectRecieve();
        }

        private void ConnectSend()
        {
            if (!this.send_connection_active)
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

        private void ConnectRecieve()
        {
            if (!this.recieve_connection_active && !onlyOneConnection)
            {
                Thread recieve_wait_for_conn_thread = new Thread(new ThreadStart(InitRecieveClientPassive));
                recieve_wait_for_conn_thread.Name = "recieve_wait_for_conn_thread";
                recieve_wait_for_conn_thread.Start();
                Threads.Add(recieve_wait_for_conn_thread);
            }
            else if (!onlyOneConnection)
            {
                Thread send_wait_for_conn_thread = new Thread(new ThreadStart(InitRecieveClientActive));
                send_wait_for_conn_thread.Name = "recieve_wait_for_conn_thread";
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
            if (Send_Client == null)
                throw new Exception("Send not possible, not connected");
            NetworkStream stream = Send_Client.GetStream();
            stream.Write(telegramm, 0, telegramm.Length);
        }

        public void SendQuittData(string telegramm)
        {
            SendQuittData(Encoding.ASCII.GetBytes(telegramm));
        }

        public void SendQuittData(byte[] telegramm)
        {
            if (Recieve_Client == null)
                throw new Exception("Send not possible, not connected");
            NetworkStream stream = Recieve_Client.GetStream();
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

               

                if (Send_Client == null || Send_Client.Connected == false)
                {
                    if (Send_Client!=null)
                        Send_Client.Close();

                    TcpListener server = new TcpListener(local_ip, send_connection_port);
                    server.Start();
                    
                    Send_Client = null;
                    if (onlyOneConnection)
                        Recieve_Client = null;
                    Send_Client = server.AcceptTcpClient();

                    SocketExtensions.SetKeepAlive(Send_Client.Client, 100, 20);                    

                    SendClosed = true;
                    
                    if (ConnectionEstablished != null)
                        context.Post(delegate { ConnectionEstablished(1); }, null);

                    //Thread for Recieving Data
                    if (ReceiveDataSendClientThread != null)
                    {
                        ReceiveDataSendClientThread.Abort();
                        Threads.Remove(ReceiveDataSendClientThread);
                    }
                    ReceiveDataSendClientThread = new Thread(ReceiveDataSendClient);
                    ReceiveDataSendClientThread.Name = "PReceiveDataSendClientThread";
                    ReceiveDataSendClientThread.Start();
                    Threads.Add(ReceiveDataSendClientThread);
                    //End Thread

                    if (onlyOneConnection)
                        Recieve_Client = Send_Client;
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
                    if (Send_Client == null || Send_Client.Connected == false)
                    {
                        if (SendClosed)
                        {
                            SendClosed = false;
                            context.Post(delegate { ConnectionClosed(1); }, null);
                        }

                        Send_Client = null;
                        if (onlyOneConnection)
                            Recieve_Client = null;
                        
                        Send_Client = new TcpClient(plc_ip.ToString(), send_connection_port);
                        SocketExtensions.SetKeepAlive(Send_Client.Client, 100, 20);                        

                        SendClosed = true;

                        if (ConnectionEstablished != null)
                            context.Post(delegate { ConnectionEstablished(1); }, null);

                        //Thread for Recieving Data
                        if (ReceiveDataSendClientThread != null)
                        {
                            ReceiveDataSendClientThread.Abort();
                            Threads.Remove(ReceiveDataSendClientThread);
                        }
                        ReceiveDataSendClientThread = new Thread(ReceiveDataSendClient);
                        ReceiveDataSendClientThread.Name = "AReceiveDataSendClientThread";
                        ReceiveDataSendClientThread.Start();
                        Threads.Add(ReceiveDataSendClientThread);
                        //End Thread

                        if (onlyOneConnection)
                            Recieve_Client = Send_Client;
                    }
                    Thread.Sleep(1000);
                }
                catch (ThreadAbortException ex)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(5000);
                }
            }
        }

        public void ReceiveDataSendClient()
        {
            Byte[] bytes = new Byte[send_tele_len];
            NetworkStream stream = null;
            try
            {
                while (true)
                {
                    try
                    {
                        if (stream == null)
                            stream = Send_Client.GetStream();
                        int len = stream.Read(bytes, 0, send_tele_len);

                        if (TelegrammRecievedSend != null && len > 0)
                            context.Post(delegate { TelegrammRecievedSend(bytes); }, null);
                        
                        //stream.Dispose();
                    }
                    catch (Exception ex)
                    {
                        if (stream!=null)
                        {
                            stream.Dispose();
                            stream = null;
                        }
                    }
                }
            }
            catch (ThreadAbortException ex) //Kill this Thread when a Exception occurs!
            {
                if (stream != null)
                {
                    stream.Dispose();
                    stream = null;
                }
            }
        }

        public void InitRecieveClientPassive()
        {
            bool RecieveClosed = false;
            while (true)
            {
                if (RecieveClosed)
                {
                    RecieveClosed = false;
                    context.Post(delegate { ConnectionClosed(1); }, null);
                }

               

                if (Recieve_Client == null || Recieve_Client.Connected == false)
                {
                    if (Recieve_Client != null)
                        Recieve_Client.Close();

                    TcpListener server = new TcpListener(local_ip, recieve_connection_port);
                    server.Start();

                    Recieve_Client = null;
                    Recieve_Client = server.AcceptTcpClient();
                    SocketExtensions.SetKeepAlive(Recieve_Client.Client, 100, 20);

                    RecieveClosed = true;

                    if (ConnectionEstablished != null)
                        context.Post(delegate { ConnectionEstablished(2); }, null);

                    //Thread for Recieving Data
                    if (ReceiveDataRecieveClientThread != null)
                    {
                        ReceiveDataRecieveClientThread.Abort();
                        Threads.Remove(ReceiveDataRecieveClientThread);
                    }
                    ReceiveDataRecieveClientThread = new Thread(ReceiveDataRecieveClient);
                    ReceiveDataRecieveClientThread.Name = "PReceiveDataRecieveClientThread";
                    ReceiveDataRecieveClientThread.Start();
                    Threads.Add(ReceiveDataRecieveClientThread);
                    //End Thread
                }                
            }
        }

        public void InitRecieveClientActive()
        {
            bool RecieveClosed = false;
            while (true)
            {
                try
                {
                    if (Recieve_Client == null || Recieve_Client.Connected == false)
                    {
                        if (RecieveClosed)
                        {
                            RecieveClosed = false;
                            context.Post(delegate { ConnectionClosed(2); }, null);
                        }

                        Recieve_Client = null;
                        Recieve_Client = new TcpClient(plc_ip.ToString(), recieve_connection_port);
                        SocketExtensions.SetKeepAlive(Recieve_Client.Client, 100, 20);

                        RecieveClosed = true;

                        if (ConnectionEstablished != null)
                            context.Post(delegate { ConnectionEstablished(2); }, null);

                        //Thread for Recieving Data
                        if (ReceiveDataRecieveClientThread != null)
                        {
                            ReceiveDataRecieveClientThread.Abort();
                            Threads.Remove(ReceiveDataRecieveClientThread);
                        }
                        ReceiveDataRecieveClientThread = new Thread(ReceiveDataRecieveClient);
                        ReceiveDataRecieveClientThread.Name = "AReceiveDataRecieveClient";
                        ReceiveDataRecieveClientThread.Start();
                        Threads.Add(ReceiveDataRecieveClientThread);
                        //End Thread                       
                    }
                    Thread.Sleep(1000);
                }
                catch (ThreadAbortException ex)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(5000);
                }
            }
        }

        public void ReceiveDataRecieveClient()
        {
            Byte[] bytes = new Byte[recieve_tele_len];
            NetworkStream stream = null;
            try
            {
                while (true)
                {
                    try
                    {
                        if (stream == null)
                            stream = Recieve_Client.GetStream();
                        int len = stream.Read(bytes, 0, recieve_tele_len);

                        if (TelegrammRecievedRecieve != null && len>0)
                                context.Post(delegate { TelegrammRecievedRecieve(bytes); }, null);                            
                        //stream.Dispose();
                        //stream = null;
                    }
                    catch(Exception ex)
                    {
                        if (stream != null)
                        {
                            stream.Dispose();
                            stream = null;
                        }
                    }
                }
            }
            catch (ThreadAbortException ex) //Kill this Thread when a Exception occurs!
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