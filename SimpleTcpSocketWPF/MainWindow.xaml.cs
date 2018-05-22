using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
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

using JFKCommonLibrary.Forms;
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

        private string recieveText = "";

        public MainWindow()
        {
            InitializeComponent();

            if (!String.IsNullOrEmpty(Properties.Settings.Default.LogFile))
            {
                lblLogFile.Text = this.GetTextFilename();
            }

            cbEncoding.Items.Add("DOS-437");
            cbEncoding.Items.Add("Windows-1252");
            cbEncoding.Items.Add("ASCII (keine Umlaute!)");
            cbEncoding.Items.Add("ISO-8859-1");
            cbEncoding.Items.Add("UTF7");
            cbEncoding.Items.Add("UTF8");
            cbEncoding.Items.Add("UTF32");

            cbEncoding.SelectedIndex = 0;

            DataObject.AddPastingHandler(txtTelegrammHex, OnHexPaste);
        }

        private void OnHexPaste(object sender, DataObjectPastingEventArgs e)
        {
            e.CancelCommand();

            var isText = e.SourceDataObject.GetDataPresent(System.Windows.DataFormats.Text, true);
            if (!isText) return;

            var text = e.SourceDataObject.GetData(DataFormats.Text) as string;

            string ret = "";
            string oldT = "";
            foreach (char c in text.Replace("\r","").Replace("\n",""))
            {
                if (c != ' ')
                {
                    oldT += c;
                    if (oldT.Length == 2)
                    {
                        ret += oldT + " ";
                        oldT = "";
                    }
                }
                else
                {
                    if (oldT != "")
                    {
                        ret += oldT.PadLeft(2, '0') + " ";
                        oldT = "";
                    }
                }            
            }

            if (oldT != "")
            {
                ret += oldT.PadLeft(2, '0') + " ";
                oldT = "";
            }

            txtTelegrammHex.Text = ret;

            e.Handled = true;
        }

        private Encoding getEncoding()
        {
            switch (cbEncoding.SelectedIndex)
            {
                case 0:
                    return Encoding.GetEncoding(437);
                case 1:
                    return Encoding.GetEncoding(1252);
                case 2:
                    return Encoding.ASCII;
                case 3:
                    return Encoding.GetEncoding("ISO-8859-1");
                case 4:
                    return Encoding.UTF7;
                case 5:
                    return Encoding.UTF8;
                case 6:
                    return Encoding.UTF32;
            }
            
            return Encoding.ASCII;
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
                IPAddress ip;

                if (!Properties.Settings.Default.Active)
                {
                    ip = IPAddress.Any;
                }
                else
                {
                    if (!IPAddress.TryParse(Properties.Settings.Default.IP, out ip))
                    {
                        IPAddress[] addresslist = Dns.GetHostAddresses(Properties.Settings.Default.IP);
                        foreach (var ipAddress in addresslist)
                        {
                            if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                ip = ipAddress;
                                break;
                            }
                        }
                    }
                }

                try
                {
                    if (Properties.Settings.Default.RecieveFixedLength > 0)
                    {
                        tcpFunc = new TCPFunctionsAsync(SynchronizationContext.Current, ip, Int32.Parse(Properties.Settings.Default.Port), Properties.Settings.Default.Active, Properties.Settings.Default.RecieveFixedLength);
                    }
                    else
                    {
                        tcpFunc = new TCPFunctionsAsync(SynchronizationContext.Current, ip, Int32.Parse(Properties.Settings.Default.Port), Properties.Settings.Default.Active);
                    }
                    tcpFunc.AllowMultipleClients = Properties.Settings.Default.MultipleConnections;
                    tcpFunc.DataRecieved += tcpFunc_DataRecieved;
                    tcpFunc.ConnectionEstablished += tcpFunc_ConnectionEstablished;
                    tcpFunc.ConnectionClosed += tcpFunc_ConnectionClosed;
                    tcpFunc.AutoReConnect = true;
                    cmdConnect.Background = Brushes.Orange;
                    tcpFunc.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler Creating Connection:" + ex.Message);
                }
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

        private string bytesToHexString(byte[] data)
        {
            string retVal = "";
            foreach (var b in data)
            {
                if (retVal != "") retVal += " ";
                retVal += b.ToString("X").PadLeft(2, '0');
            }

            return retVal;
        }

        private bool answer = true;

        void tcpFunc_DataRecieved(byte[] data, System.Net.Sockets.TcpClient arg2)
        {
            if (answer)
            {
                    
            }

            var tel = getEncoding().GetString(data);

            var wrt = tel;

            if (Properties.Settings.Default.LogDataAsHexBytes) 
                wrt = bytesToHexString(data);

            var len = data.Length;

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
                try
                {
                    lblLogFile.Text = this.GetTextFilename();
                    StreamWriter myFile = new StreamWriter(lblLogFile.Text, true);
                    if (Properties.Settings.Default.LogHexAndAscii && Properties.Settings.Default.LogDataAsHexBytes)
                    {
                        var akTel = tel;
                        if (Properties.Settings.Default.EscapeSpecialChars)
                            akTel = escape(akTel);
                        myFile.Write("Rec  : " + add + akTel + Environment.NewLine);
                        myFile.Write("Bytes: " + add + wrt + Environment.NewLine);
                    }
                    else
                    {
                        var akTel = wrt;
                        if (Properties.Settings.Default.EscapeSpecialChars)
                            akTel = escape(akTel, true);
                        myFile.Write("Rec  : " + add + akTel + Environment.NewLine);
                    }                    
                    myFile.Close();
                }
                catch (Exception ex)
                {
                    lblException.Text = ex.Message;
                }
            }

            if (Properties.Settings.Default.LogHexAndAscii && Properties.Settings.Default.LogDataAsHexBytes)
            {
                var akTel = tel;
                if (Properties.Settings.Default.EscapeSpecialChars)
                    akTel = escape(akTel, true);
                recieveText = add + akTel + Environment.NewLine + Environment.NewLine + recieveText;
                recieveText = add + wrt + Environment.NewLine + recieveText;
            }
            else
            {
                var akTel = wrt;
                if (Properties.Settings.Default.EscapeSpecialChars)
                    akTel = escape(akTel, true);
                recieveText = add + akTel + Environment.NewLine + recieveText;
            }   

            if (chkLoggerRefresh.IsChecked.Value)
                txtRecieve.Text = recieveText;
            
            txtRecieve2.Text = recieveText;
        }

        public string GetTextFilename()
        {
            var dt = DateTime.Now;
            var retVal = Properties.Settings.Default.LogFile;

            retVal = retVal.Replace("{dd}", dt.ToString("dd"));
            retVal = retVal.Replace("{MM}", dt.ToString("MM"));
            retVal = retVal.Replace("{yyyy}", dt.ToString("yyyy"));
            retVal = retVal.Replace("{yy}", dt.ToString("yy"));
            retVal = retVal.Replace("{hh}", dt.ToString("hh"));
            retVal = retVal.Replace("{HH}", dt.ToString("HH"));
            retVal = retVal.Replace("{mm}", dt.ToString("mm"));
            retVal = retVal.Replace("{ss}", dt.ToString("ss"));

            string path = System.IO.Path.Combine(Environment.CurrentDirectory, retVal);
            retVal = System.IO.Path.GetFullPath(path);

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
        //private static Regex _regex2 = new Regex(@"\\(?<Value>[0-9]{4})", RegexOptions.Compiled);

        private string unescape(string text)
        {
            text = _regex.Replace(text, m => ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString());
            return text.Replace("\\f", "\f").Replace("\\v", "\v").Replace("\\b", "\b").Replace("\\0", "\0").Replace("\\t", "\t").Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\\\", "\\");           
        }

        private string escape(string text, bool forAvalonEdit =false)
        {
            text = text.Replace("\\", "\\\\");
            text = text.Replace("\r", "\\r").Replace("\n", "\\n");
            text = text.Replace("\u00ff", "\\u00ff");
            if (!forAvalonEdit)
            {
                text =
                    text
                        .Replace("\f", "\\f")
                        .Replace("\v", "\\v")
                        .Replace("\b", "\\b")
                        .Replace("\0", "\\0")
                        .Replace("\t", "\\t");
                for (int n = 0; n < 20; n++)
                {
                    text = text.Replace("" + (char) n + "", "\\u" + n.ToString("X").PadLeft(4, '0'));
                }
                text = text.Replace("\u007f", "\\u007f");                
            }
            return text;
        }

        private void SendTel(string tel)
        {
            if (Properties.Settings.Default.EscapeSpecialChars) 
                tel = unescape(tel);

            sendedTele.Add(tel);

            var wrt = tel;

            var sendbytes = getEncoding().GetBytes(tel);

            if (Properties.Settings.Default.LogDataAsHexBytes)
                wrt = bytesToHexString(sendbytes);

            string add = "";

            if (Properties.Settings.Default.ShowDate)
            {
                add += DateTime.Now.ToString(Properties.Settings.Default.DateTimeFormat);
            }
            if (Properties.Settings.Default.ShowRecievedLen)
            {
                if (!string.IsNullOrEmpty(add)) add += " - ";
                add += sendbytes.Length.ToString().PadLeft(4, '0') + " Bytes";
            }
            if (!string.IsNullOrEmpty(add)) add += ": ";


            if (Properties.Settings.Default.LogHexAndAscii && Properties.Settings.Default.LogDataAsHexBytes)
            {
                var akTel = tel;
                if (Properties.Settings.Default.EscapeSpecialChars)
                    akTel = escape(akTel);
                txtSended.Text = add + wrt + Environment.NewLine + Environment.NewLine + txtSended.Text;
                txtSended.Text = add + akTel + Environment.NewLine + txtSended.Text;                
            }
            else
            {
                var akTel = wrt;
                if (Properties.Settings.Default.EscapeSpecialChars)
                    akTel = escape(akTel, true);
                txtSended.Text = add + akTel + Environment.NewLine + txtSended.Text;
            }   
            
            if (tcpFunc != null)
                tcpFunc.SendData(sendbytes);

            if (!String.IsNullOrEmpty(Properties.Settings.Default.LogFile))
            {
                try
                {
                    lblLogFile.Text = this.GetTextFilename();
                    StreamWriter myFile = new StreamWriter(lblLogFile.Text, true);
                    if (Properties.Settings.Default.LogHexAndAscii && Properties.Settings.Default.LogDataAsHexBytes)
                    {
                        var akTel = tel;
                        if (Properties.Settings.Default.EscapeSpecialChars)
                            akTel = escape(akTel);
                        myFile.Write("Send : " + add + akTel + Environment.NewLine);
                        myFile.Write("Bytes: " + add + wrt + Environment.NewLine);
                    }
                    else
                    {
                        var akTel = tel;
                        if (Properties.Settings.Default.EscapeSpecialChars)
                            akTel = escape(akTel);
                        myFile.Write("Send : " + add + akTel + Environment.NewLine);
                    }
                    myFile.Close();
                }
                catch (Exception ex)
                {
                    lblException.Text = ex.Message;
                }
            }
        }

        private void txtTelegramm_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.Changes.First().AddedLength == 1)
            {
                blLen.Content = "0";
                if (txtTelegramm.Text != null)
                {
                    blLen.Content = txtTelegramm.Text.Length.ToString();

                    string txt = "";

                    foreach (byte b in getEncoding().GetBytes(txtTelegramm.Text))
                    {
                        txt += b.ToString("X").PadLeft(2, '0') + " ";
                    }

                    if (Properties.Settings.Default.EscapeSpecialChars)
                    {
                        try
                        {
                            var t2 = unescape(txtTelegramm.Text);
                            blLen.Content = t2.Length.ToString();

                            txt = "";
                            foreach (byte b in getEncoding().GetBytes(t2))
                            {
                                txt += b.ToString("X").PadLeft(2, '0') + " ";
                            }

                        }
                        catch (Exception)
                        { }
                    }
                    txtTelegrammHex.Text = txt;
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

        private void txtTelegramm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.cmdSend_Click(sender, null);
            }           
        }

        private void chkLoggerRefresh_Checked(object sender, RoutedEventArgs e)
        {
            txtRecieve.Text = recieveText;            
        }

        private void txtTelegrammHex_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!"0123456789ABCDEFabcdef".Contains(e.Text))
            {
                e.Handled = true;
            }            
        }

        private void txtTelegrammHex_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if ((txtTelegrammHex.CaretIndex + 1) % 3 != 0)
                {
                    e.Handled = true;
                }                
            }            
        }

        private void txtTelegrammHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            var idx = txtTelegrammHex.CaretIndex;

            if (keywasPressed)
            {

                List<byte> bytes = new List<byte>();

                keywasPressed = false;

                if (idx == txtTelegrammHex.Text.Length && e.Changes.First().RemovedLength == 0)
                {
                    if ((idx + 1)%3 == 0)
                    {
                        txtTelegrammHex.Text += " ";
                        txtTelegrammHex.CaretIndex = idx + 1;
                    }
                }

                string txt = "";
                string parseTl = txtTelegrammHex.Text.Replace(" ", "");

                try
                {
                    for (int n = 0; n < parseTl.Length; n += 2)
                    {
                        if (parseTl.Length > n + 1)
                        {
                            var wrt =
                                (int.Parse("" + parseTl[n] + parseTl[n + 1], System.Globalization.NumberStyles.HexNumber));
                            bytes.Add((byte) wrt);
                            //txt = txt + (char)wrt;
                        }
                        else if (parseTl.Length > n)
                        {
                            var wrt = (int.Parse("" + parseTl[n], System.Globalization.NumberStyles.HexNumber));
                            bytes.Add((byte) wrt);
                            //txt = txt + (char)wrt;
                        }
                    }
                }
                catch (Exception)
                {
                }

                txt = getEncoding().GetString(bytes.ToArray());

                if (Properties.Settings.Default.EscapeSpecialChars)
                {
                    txt = this.escape(txt);
                }
                txtTelegramm.Text = txt;

                blLen.Content = txtTelegramm.Text.Length.ToString();

                if (Properties.Settings.Default.EscapeSpecialChars)
                {
                    try
                    {
                        var t2 = unescape(txtTelegramm.Text);
                        blLen.Content = t2.Length.ToString();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            if (txtTelegrammHex.Text != txtTelegrammHex.Text.ToUpper())
            {
                txtTelegrammHex.Text = txtTelegrammHex.Text.ToUpper();
                txtTelegrammHex.CaretIndex = idx;
            }
        }

        private bool keywasPressed = false;

        private void txtTelegrammHex_KeyDown(object sender, KeyEventArgs e)
        {
            keywasPressed = true;
        }

        private void cmdSend_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                var cnt = InputBox.Show("Multi Send", "Anzahl der zu Sendenden Telegramme:", "100");
                var anz = Convert.ToInt32(cnt);
                for (int n = 0; n < anz; n++)
                {
                    this.cmdSend_Click(sender, null);
                }
            }
        }

        private void cmdClear_Click(object sender, RoutedEventArgs e)
        {
            txtRecieve.Text = "";
            txtRecieve2.Text = "";
            txtSended.Text = "";
            recieveText = "";
            sendedTele.Clear();
        }
    }
}
