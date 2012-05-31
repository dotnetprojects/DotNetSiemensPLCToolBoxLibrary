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
    public class TCPFunctions    
    {
        public class QuittConfig
        {
            [Serializable()]
            public class QuittText
            {
                //This is neccessary, that the spaces stay
                [XmlAttribute("xml:space")]
                public String SpacePreserve = "preserve";

                public QuittText()
                { }

                public QuittText(int Position, byte[] Value)
                {
                    this.Position = Position;
                    this.Value = Encoding.ASCII.GetString(Value);
                }

                public QuittText(int Position, string Value)
                {
                    this.Position = Position;
                    this.Value = Value;
                }

                [Description("The Offset at wich the Value should be Placed")]
                public int Position { get; set; }
                public string Value { get; set; }

                public override string ToString()
                {
                    return "[" + Position.ToString() + "] " + Value;
                }
            }
            public List<QuittText> QuittReplacmentBytes = new List<QuittText>();

            public bool AutomaticQuitt;

            public int SequenceNumberPosition;
            public int LengthSequenceNumber;
        }

        /*public void Dispose()
        {
            if (serverRec != null)
                serverRec.Stop();
            if (serverSend != null)
                serverSend.Stop();

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

        //Quitt Config:
        public class QuittConfig
        {
            [Serializable()]
            public class QuittText
            {
                //This is neccessary, that the spaces stay
                [XmlAttribute("xml:space")]
                public String SpacePreserve = "preserve";

                public QuittText()
                { }

                public QuittText(int Position, byte[] Value)
                {
                    this.Position = Position;
                    this.Value = Encoding.ASCII.GetString(Value);
                }

                public QuittText(int Position, string Value)
                {
                    this.Position = Position;
                    this.Value = Value;
                }

                [Description("The Offset at wich the Value should be Placed")]
                public int Position { get; set; }
                public string Value { get; set; }

                public override string ToString()
                {
                    return "[" + Position.ToString() + "] " + Value;
                }
            }
            public List<QuittText> QuittReplacmentBytes = new List<QuittText>();

            public bool AutomaticQuitt;

            public int SequenceNumberPosition;
            public int LengthSequenceNumber;
        }

        public QuittConfig QuittConfiguration;
        private byte[] oldSequenceNumber;
        //End Quitt Config


        public TCPFunctions(SynchronizationContext context, IPAddress PlcIP, bool onlyOneConnection, int send_connection_port, bool send_connection_active, int send_tele_len, int recieve_connection_port, bool recieve_connection_active, int recieve_tele_len, QuittConfig QuittConfiguation)
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

            this.QuittConfiguration = QuittConfiguation;
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
                Thread recieve_wait_for_conn_thread = new Thread(new ThreadStart(InitRecieveClientActive));
                recieve_wait_for_conn_thread.Name = "recieve_wait_for_conn_thread";
                recieve_wait_for_conn_thread.Start();
                Threads.Add(recieve_wait_for_conn_thread);
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

        private TcpListener serverSend; 
        public void InitSendClientPassive()
        {
            try
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
                        if (Send_Client != null)
                            Send_Client.Close();

                        serverSend = new TcpListener(local_ip, send_connection_port);
                        serverSend.Start();

                        Send_Client = null;
                        if (onlyOneConnection)
                            Recieve_Client = null;
                        Send_Client = serverSend.AcceptTcpClient();
                        Send_Client.Client.SetKeepAlive(100, 20);

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
            catch (Exception  ex)
            { }
        }

        public void InitSendClientActive()
        {
            try
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
                            Send_Client.Client.SetKeepAlive(100, 20);

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
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(5000);
                    }
                }
            }
            catch (ThreadAbortException ex)
            { }
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
                    catch (ThreadAbortException ex)
                    {
                        throw;
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
            catch (ThreadAbortException ex) //Kill this Thread when a Exception occurs!
            {
                if (stream != null)
                {
                    stream.Dispose();
                    stream = null;
                }
            }
        }

        private TcpListener serverRec;
        public void InitRecieveClientPassive()
        {
            try
            {
                bool RecieveClosed = false;
                while (true)
                {
                    if (RecieveClosed)
                    {
                        RecieveClosed = false;
                        context.Post(delegate { ConnectionClosed(2); }, null);
                    }



                    if (Recieve_Client == null || Recieve_Client.Connected == false)
                    {
                        if (Recieve_Client != null)
                            Recieve_Client.Close();

                        serverRec = new TcpListener(local_ip, recieve_connection_port);
                        serverRec.Start();
                        
                        Recieve_Client = null;
                        Recieve_Client = serverRec.AcceptTcpClient();
                        Recieve_Client.Client.SetKeepAlive(100, 20);

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
            catch (Exception ex) //Kill this Thread when a Exception occurs!
            { }
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
                        Recieve_Client.Client.SetKeepAlive(100, 20);

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
                            if (QuittConfiguration == null || !QuittConfiguration.AutomaticQuitt)
                                context.Post(delegate { TelegrammRecievedRecieve(bytes); }, null);
                            else
                            {
                                //Automaticly send a quitt Telegramm
                                byte[] quittTel = new byte[recieve_tele_len];
                                Array.Copy(bytes, quittTel, recieve_tele_len);

                                foreach (var quittReplacmentByte in QuittConfiguration.QuittReplacmentBytes)
                                {
                                    Array.Copy(Encoding.ASCII.GetBytes(quittReplacmentByte.Value), 0, quittTel, quittReplacmentByte.Position, quittReplacmentByte.Value.Length);
                                }
                                stream.Write(quittTel, 0, quittTel.Length);

                                byte[] sequNr = new byte[QuittConfiguration.LengthSequenceNumber];
                                Array.Copy(bytes, QuittConfiguration.SequenceNumberPosition, sequNr, 0, QuittConfiguration.LengthSequenceNumber);

                                if (sequNr != oldSequenceNumber)
                                    context.Post(delegate { TelegrammRecievedRecieve(bytes); }, null);
                                oldSequenceNumber = sequNr;
                            }
                        //stream.Dispose();
                        //stream = null;
                    }
                    catch (ThreadAbortException ex)
                    {
                        throw;
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
        }*/
    }
}