using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using JFKCommonLibrary.Networking;

namespace SimpleTcpSocket
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private TCPFunctionsAsync tcpFunc;

        private void cmdConnect_Click(object sender, EventArgs e)
        {
            if (tcpFunc != null)
            {
                tcpFunc.AutoReConnect = false;
                tcpFunc.Dispose();
                tcpFunc = null;
                cmdConnect.BackColor = Color.FromArgb(224, 224, 224);
            }
            else
            {
                tcpFunc = new TCPFunctionsAsync(SynchronizationContext.Current, IPAddress.Parse(txtIP.Text), Int32.Parse(txtPort.Text), chkActive.Checked);
                tcpFunc.DataRecieved += tcpFunc_DataRecieved;
                tcpFunc.ConnectionEstablished += tcpFunc_ConnectionEstablished;
                tcpFunc.ConnectionClosed += tcpFunc_ConnectionClosed;
                tcpFunc.AutoReConnect = true;
                cmdConnect.BackColor = Color.Orange;
                tcpFunc.Start();
            }
        }

        void tcpFunc_ConnectionEstablished(System.Net.Sockets.TcpClient obj)
        {
            cmdConnect.BackColor = Color.LightGreen;
        }

        void tcpFunc_ConnectionClosed(System.Net.Sockets.TcpClient obj)
        {
            cmdConnect.BackColor = Color.Red;
        }

        void tcpFunc_DataRecieved(byte[] arg1, System.Net.Sockets.TcpClient arg2)
        {
            var wrt = Encoding.ASCII.GetString(arg1);

            for (byte n = 0; n < 32; n++)
            {
                wrt = wrt.Replace(Encoding.ASCII.GetString(new byte[] { n }), "{0x" + n.ToString("X").PadLeft(2, '0') + "}");
            }

            string add = "";

            if (chkShowDate.Checked)
            {
                add += DateTime.Now.ToString();
            }
            if (chkShowRecievedLen.Checked)
            {
                if (!string.IsNullOrEmpty(add)) add += " - ";
                add += wrt.Length.ToString().PadLeft(4, '0') + " Bytes";
            }
            if (!string.IsNullOrEmpty(add)) add += ": ";

            txtRecieve.Text = add + wrt + Environment.NewLine + txtRecieve.Text;
            txtRecieve2.Text = txtRecieve.Text;
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void cmdSend_Click(object sender, EventArgs e)
        {
            var wrt = txtTelegramm.Text;

            string add = "";

            if (chkShowDate.Checked) 
            {
                add += DateTime.Now.ToString();
            }
            if (chkShowRecievedLen.Checked)
            {
                if (!string.IsNullOrEmpty(add)) add += " - ";
                add += wrt.Length.ToString().PadLeft(4, '0') + " Bytes";
            }
            if (!string.IsNullOrEmpty(add)) add += ": ";

            txtSended.Text = add + wrt + Environment.NewLine + txtSended.Text;
            if (tcpFunc != null) 
                tcpFunc.SendStringData(txtTelegramm.Text);
        }

        private void txtTelegramm_TextChanged(object sender, EventArgs e)
        {
            blLen.Text = "0";
            if (txtTelegramm.Text != null) blLen.Text = txtTelegramm.Text.Length.ToString();
        }

        private void mnuAddSpecialChar_Click(object sender, EventArgs e)
        {
            var dlg = new AddSpecialChar(txtTelegramm);
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text = this.Text;
        }
    }
}
