using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotNetSiemensPLCToolBoxLibrary.Communication.Library;
using DotNetSiemensPLCToolBoxLibrary.Communication.Library.Interfaces;
using DotNetSiemensPLCToolBoxLibrary.Communication.Library.Pdus;
using Kopplungstester;
using System.Threading;
using System.Net.Sockets;

namespace NetworkPLCSim
{
    public partial class NetworkPLCSim : Form
    {
        private TCPServerTPKT myTCPServer;
        private S7OnlineInterface myS7OnlineInterface;
        private Connection myConnection;

        public NetworkPLCSim()
        {
            InitializeComponent();
        }
        
        private void cmdStart_Click(object sender, EventArgs e)
        {
            myS7OnlineInterface = new S7OnlineInterface("S7ONLINE");

            myConnection = myS7OnlineInterface.ConnectPlc(new ConnectionConfig(2, 0, 2));
            myConnection.PDURecieved += new Connection.PDURecievedDelegate(myConnection_PDURecieved);

            myTCPServer = new TCPServerTPKT(SynchronizationContext.Current, 102);
            myTCPServer.ConnectionEstablished += new TCPServerTPKT.ConnectionEstablishedEventHandler(MyTcpServerConnectionEstablished);
            myTCPServer.ConnectionClosed += new TCPServerTPKT.ConnectionClosedEventHandler(MyTcpServerConnectionClosed);
            myTCPServer.TelegrammRecieved += new TCPServerTPKT.TelegramRecievedEventHandler(myTCPServer_TelegrammRecieved);
            myTCPServer.Start();
        }

        void myConnection_PDURecieved(Pdu pdu)
        {
            byte[] pdu_bt = pdu.ToBytes();
            byte[] data = new byte[pdu_bt.Length + 3]; //+3 Bytes (COTP)
            Array.Copy(pdu_bt, 0, data, 3, pdu_bt.Length);

            //COTP
            data[0] = 2;
            data[1] = 0xf0;
            data[2] = 0x80;

            listBox1.Invoke(
                (MethodInvoker) delegate() { listBox1.Items.Add("Send (TCP): " + ByteExtensions.ToHexString(data)); });
            
            
            myTCPServer.SendData(data);
        }

        void myTCPServer_TelegrammRecieved(byte[] telegramm, TcpClient tcpClient)
        {

            //Dann kommt COPT
            //Dann S7 PDU
            byte[] coptPart = new byte[telegramm[0] + 1];
            Array.Copy(telegramm, 0, coptPart, 0, coptPart.Length);

            listBox1.Items.Add("Recieved (TCP): " + ByteExtensions.ToHexString(telegramm));
            try
            {
                if (coptPart[1] == 0xe0 /* 0xe0 = CR = Connection Request */)
                {
                    // Auf Anfrage mit CC Connect Confirm antworten
                    byte[] cc = {
                                    // *** fixed part ***
                                    0x11, // Length 17
                                    0x0D, // 0xD0 = CC Connect Confirm
                                    0x00, 0x01, // 2, 3: Dest.Reference
                                    0x00, 0x01, // 4, 5: Source Reference
                                    0x00, // 6 :Class Option
                                    // *** variable part ***
                                    0xC0, // 7: Param. Code: tdpu-size
                                    0x01, // 8: Param. length 1
                                    0x09, // 9: TPDU size
                                    0xC1, // 10: Param. Code:scr-tsap
                                    0x02, // 11: Param. length 2
                                    0x01, // 12:
                                    0x00, // 13:
                                    0xC2, // 14: Param. Code: dst-tsap
                                    0x02, // 15: Param. length 2
                                    0x03, // 16:
                                    0x02 // 17:
                                };
                    cc[2] = coptPart[4];
                    cc[3] = coptPart[5];
                    listBox1.Items.Add("Send (TCP): " + ByteExtensions.ToHexString(cc));
            
                    myTCPServer.SendData(cc);

                }
                else if (coptPart[1] == 0xf0 /* 0xf0 = DT = Data */)
                {
                    byte[] pduBytes = new byte[telegramm.Length - coptPart.Length];
                    Array.Copy(telegramm, coptPart.Length, pduBytes, 0, pduBytes.Length);
                    Pdu recPdu = new Pdu(pduBytes);

                    if (recPdu.Param[0] == 0xf0)
                    {
                        //Negotiate PDU Length request 
                    }
                    else
                        myConnection.SendPdu(recPdu);
                }
            }
            catch (Exception ex)
            {
                listBox1.Items.Add(ex.Message);
            }

        }

        void MyTcpServerConnectionClosed(TcpClient client)
        {
            listBox1.Items.Add("Closed: " + client.ToString());            
        }

        void MyTcpServerConnectionEstablished(TcpClient client)
        {
            listBox1.Items.Add("Connected: " + client.Client.RemoteEndPoint.ToString());            
        }

        private void NetworkPLCSim_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (myTCPServer != null)
                myTCPServer.Dispose();
            if (myConnection != null)
                myConnection.Dispose();
            if (myS7OnlineInterface != null)
                myS7OnlineInterface.Dispose();
        }
    }
}
