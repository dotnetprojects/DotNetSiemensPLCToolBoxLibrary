using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using JFKCommonLibrary.Networking;

namespace SimpleTcpSocketWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TCPFunctionsAsync tcpFunc;

        private List<string> sendedTele = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void cmdConnect_Click(object sender, RoutedEventArgs e)
        {
            if (tcpFunc != null)
            {
                tcpFunc.AutoReConnect = false;
                tcpFunc.Dispose();
                tcpFunc = null;
                cmdConnect.Background = null;
            }
            else
            {
                tcpFunc = new TCPFunctionsAsync(SynchronizationContext.Current, IPAddress.Parse(Properties.Settings.Default.IP), Int32.Parse(Properties.Settings.Default.Port), Properties.Settings.Default.Active);
                tcpFunc.DataRecieved += tcpFunc_DataRecieved;
                tcpFunc.ConnectionEstablished += tcpFunc_ConnectionEstablished;
                tcpFunc.ConnectionClosed += tcpFunc_ConnectionClosed;
                tcpFunc.AutoReConnect = true;
                cmdConnect.Background = Brushes.Orange;
                tcpFunc.Start();
            }
        }


        void tcpFunc_ConnectionEstablished(System.Net.Sockets.TcpClient obj)
        {
            cmdConnect.Background = Brushes.LightGreen;
        }

        void tcpFunc_ConnectionClosed(System.Net.Sockets.TcpClient obj)
        {
            cmdConnect.Background = Brushes.Red;
        }

        void tcpFunc_DataRecieved(byte[] data, System.Net.Sockets.TcpClient arg2)
        {
            var wrt = Encoding.ASCII.GetString(data);

            var len = wrt.Length;

            string add = "";

            if (Properties.Settings.Default.ShowDate)
            {
                add += DateTime.Now.ToString(Properties.Settings.Default.DateTimeFormat);
            }
            if (Properties.Settings.Default.ShowRecievedLen)
            {
                if (!string.IsNullOrEmpty(add)) add += " - ";
                add += len.ToString().PadLeft(4, '0') + " Bytes";
            }
            if (!string.IsNullOrEmpty(add)) add += ": ";

            if (!String.IsNullOrEmpty(Properties.Settings.Default.LogFile))
            {
                StreamWriter myFile = new StreamWriter(GetTextFilname(), true);
                myFile.Write("Rec : " + add + wrt + Environment.NewLine);
                myFile.Close();
            }

            txtRecieve.Text = add + wrt + Environment.NewLine + txtRecieve.Text;
            txtRecieve2.Text = txtRecieve.Text;
        }

        public string GetTextFilname()
        {
            var dt = DateTime.Now;
            var retVal = Properties.Settings.Default.LogFile;

            retVal = retVal.Replace("{dd}", dt.ToString("dd"));
            retVal = retVal.Replace("{MM}", dt.ToString("MM"));
            retVal = retVal.Replace("{yyyy}", dt.ToString("yyyy"));
            retVal = retVal.Replace("{yy}", dt.ToString("yy"));
            retVal = retVal.Replace("{hh}", dt.ToString("hh"));
            retVal = retVal.Replace("{mm}", dt.ToString("mm"));
            retVal = retVal.Replace("{ss}", dt.ToString("ss"));

            return retVal;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void cmdSend_Click(object sender, RoutedEventArgs e)
        {
            this.SendTel(txtTelegramm.Text);
        }

        private static Regex _regex = new Regex(@"\\u(?<Value>[a-zA-Z0-9]{4})", RegexOptions.Compiled);
        private string unescape(string text)
        {
            text = _regex.Replace(text, m => ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString());
            return text.Replace("\\f", "\f").Replace("\\v", "\v").Replace("\\b", "\b").Replace("\\0", "\0").Replace("\\t", "\t").Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\\\", "\\");           
        }

        private void SendTel(string tel)
        {
            if (Properties.Settings.Default.EscapeSpecialChars) 
                tel = unescape(tel);

            sendedTele.Add(tel);

            var wrt = tel;

            string add = "";

            if (Properties.Settings.Default.ShowDate)
            {
                add += DateTime.Now.ToString(Properties.Settings.Default.DateTimeFormat);
            }
            if (Properties.Settings.Default.ShowRecievedLen)
            {
                if (!string.IsNullOrEmpty(add)) add += " - ";
                add += wrt.Length.ToString().PadLeft(4, '0') + " Bytes";
            }
            if (!string.IsNullOrEmpty(add)) add += ": ";

            txtSended.Text = add + wrt + Environment.NewLine + txtSended.Text;
            if (tcpFunc != null)
                tcpFunc.SendStringData(tel);

            if (!String.IsNullOrEmpty(Properties.Settings.Default.LogFile))
            {
                StreamWriter myFile = new StreamWriter(GetTextFilname(), true);
                myFile.Write("Send: " + add + wrt + Environment.NewLine);
                myFile.Close();
            }
        }

        private void txtTelegramm_TextChanged(object sender, TextChangedEventArgs e)
        {
            blLen.Content = "0";
            if (txtTelegramm.Text != null)
            {
                blLen.Content = txtTelegramm.Text.Length.ToString();

                if (Properties.Settings.Default.EscapeSpecialChars)
                {
                    try
                    {
                        var t2 = unescape(txtTelegramm.Text);
                        blLen.Content = t2.Length.ToString();
                    }
                    catch (Exception)
                    { }
                }

            }
        }

        private AddSpecialChar addSpecialChar;

        private void txtTelegramm_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            {
                var pos = txtSended.SelectionStart;
                var input = txtSended.Text;
                var lineNumber = input.Substring(0, pos).Count(c => c == '\n');

                if (sendedTele.Count > lineNumber)
                {
                    this.SendTel(sendedTele[sendedTele.Count - lineNumber - 1]);
                }
            }
        }

        private void txtTelegramm_MouseUp(object sender, MouseButtonEventArgs e)
        {

            if (e.ChangedButton == MouseButton.Middle)
            {
                if (addSpecialChar != null)
                {
                    addSpecialChar.Close();
                }

                addSpecialChar = new AddSpecialChar(txtTelegramm);

                addSpecialChar.Owner = this;
                addSpecialChar.Show();

            }
        }

    }
}
