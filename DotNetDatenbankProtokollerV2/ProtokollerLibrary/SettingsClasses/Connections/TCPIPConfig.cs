using System.Net;
using System.Runtime.Serialization;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections
{
    public class TCPIPConfig : ConnectionConfig
    {
        private string _ip = "192.168.1.100";
        public string IP
        {
            get { return _ip; }
            set { _ip = value; NotifyPropertyChanged("IP"); }
        }

        [System.ComponentModel.Browsable(false)]
        public IPAddress IPasIPAddress { get { return IPAddress.Parse(_ip); } }


        private int _port = 2000;
        public int Port
        {
            get { return _port; }
            set { _port = value; NotifyPropertyChanged("Port"); }
        }

        private bool _useTcpKeepAlive;
        public bool UseTcpKeepAlive
        {
            get { return _useTcpKeepAlive; }
            set { _useTcpKeepAlive = value; NotifyPropertyChanged("UseTcpKeepAlive"); }
        }

        private bool _passiveConnection;
        [System.ComponentModel.Description("If not set, the Connection trys to contact the Partner, if Passive, it waits for the Partner to connect!")]
        public bool PassiveConnection
        {
            get { return _passiveConnection; }
            set { _passiveConnection = value; NotifyPropertyChanged("PassiveConnection"); }
        }

        private int _multiTelegramme;
        [System.ComponentModel.Description("When there is more than one Dataset in a TCP Telegramm, increase this to the count!")]
        public int MultiTelegramme
        {
            get { return _multiTelegramme; }
            set { _multiTelegramme = value; NotifyPropertyChanged("MultiTelegramme"); }
        }

        private bool _acceptMultipleConnections;
        [System.ComponentModel.Description("If set, a passive Connection can Accept more than one Client!")]
        public bool AcceptMultipleConnections
        {
            get { return _acceptMultipleConnections; }
            set { _acceptMultipleConnections = value; NotifyPropertyChanged("AcceptMultipleConnections"); }
        }

        public override string ToString()
        {
            return "TCP/IP-Verbindung (IP:" + IP + ", Port:" + Port.ToString() + ", Passive:" + PassiveConnection.ToString() + ")";
        }
    }
}
