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
    public class TCPServerTPKT : IDisposable
    {
        public void Dispose()
        {
            foreach (var thread in Threads)
            {
                thread.Abort();
            }
            Threads = null;

            if (_tcpClient != null)
                _tcpClient.Close();
        }


        private TcpClient _tcpClient;

        private Thread _receiveDataThread;

        private IPAddress local_ip;
        private int _port;
        private SynchronizationContext context;

        public delegate void TelegramRecievedEventHandler(byte[] telegramm, TcpClient tcpClient);
        public event TelegramRecievedEventHandler TelegrammRecieved;

        public delegate void ConnectionEstablishedEventHandler(TcpClient tcpClient);
        public event ConnectionEstablishedEventHandler ConnectionEstablished;

        public delegate void ConnectionClosedEventHandler(TcpClient tcpClient);
        public event ConnectionClosedEventHandler ConnectionClosed;

        private List<Thread> Threads = new List<Thread>();

        public TCPServerTPKT(SynchronizationContext context, int port)
        {
            this.context = context;
            this._port = port;

            local_ip = Dns.GetHostByName(Dns.GetHostName()).AddressList[0];
        }

        public void Start()
        {
            ConnectSend();
        }

        private void ConnectSend()
        {
                Thread send_wait_for_conn_thread = new Thread(new ThreadStart(InitSendClientPassive));
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

            byte[] data = new byte[telegramm.Length + 4]; //+4 Bytes (TPKT)
            Array.Copy(telegramm, 0, data, 4, telegramm.Length);

            //TPKT
            data[0] = 3;
            data[1] = 0;
            data[2] = BitConverter.GetBytes(data.Length)[1];
            data[3] = BitConverter.GetBytes(data.Length)[0];
            
            if (_tcpClient == null)
                throw new Exception("Send not possible, not connected");
            NetworkStream stream = _tcpClient.GetStream();
            stream.Write(data, 0, data.Length);
        }

        public void InitSendClientPassive()
        {
            TcpListener server = null;
            TcpClient oldTcpClient = null;
            bool SendClosed = false;
            while (true)
            {
                if (ConnectionClosed != null && SendClosed && (_tcpClient == null || _tcpClient.Connected == false))
                {
                    oldTcpClient = _tcpClient;
                    SendClosed = false;
                    context.Post(delegate { ConnectionClosed(oldTcpClient); }, null);
                }

                if (!SendClosed && (_tcpClient == null || _tcpClient.Connected == false))
                {
                    if (_tcpClient != null)
                        _tcpClient.Close();

                    if (server == null)
                    {
                        server = new TcpListener(/*local_ip,*/ _port);
                        server.Start();
                    }

                    _tcpClient = null;
                    _tcpClient = server.AcceptTcpClient();
                    SocketExtensions.SetKeepAlive(_tcpClient.Client, 100, 20);
                    SendClosed = true;

                    if (ConnectionEstablished != null)
                        context.Post(delegate { ConnectionEstablished(_tcpClient); }, null);

                    //Thread for Recieving Data
                    if (_receiveDataThread != null)
                    {
                        _receiveDataThread.Abort();
                        Threads.Remove(_receiveDataThread);
                    }
                    _receiveDataThread = new Thread(ReceiveDataSendClient);
                    _receiveDataThread.Name = "PReceiveDataSendClientThread";
                    _receiveDataThread.Start();
                    Threads.Add(_receiveDataThread);
                    //End Thread                    
                }
                else
                    Thread.Sleep(100);
            }
        }

        public void ReceiveDataSendClient()
        {
            Byte[] bytes;
            NetworkStream stream = null;
            int len;
            try
            {
                while (true)
                {
                    if (stream == null)
                        stream = _tcpClient.GetStream();
                    
                    try
                    {
                        //Read Telegramm

                        //Read TPKT Header
                        bytes = new Byte[4];
                        len = stream.Read(bytes, 0, bytes.Length);
                        if (len != bytes.Length || bytes[0] != 3 /* TelegrammVersion == 3 */) 
                            throw new Exception();
                                                
                        //Read ISO 8073 COPT
                        bytes = new Byte[bytes[2] * 0x100 + bytes[3] - 4]; 
                        len = stream.Read(bytes, 0, bytes.Length);
                        if (len != bytes.Length)
                            throw new Exception();

                        if (TelegrammRecieved != null && len > 0)
                            context.Post(delegate { TelegrammRecieved(bytes, _tcpClient); }, null);

                        //stream.Dispose();
                    }
                    catch (Exception ex)
                    {
                        if (stream != null)
                        {
                            stream.Dispose();
                            stream = null;
                        }
                    }
                }
            }
            catch (Exception ex) //Kill this Thread when a Exception occurs!
            {
                if (stream != null)
                {
                    stream.Dispose();
                    stream = null;
                }

                if (_tcpClient != null)
                    _tcpClient.Close();
            }
        }
    }
}