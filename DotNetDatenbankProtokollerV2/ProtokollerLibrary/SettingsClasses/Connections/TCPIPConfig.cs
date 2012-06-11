using System.Net;
using System.Runtime.Serialization;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections
{
    public class TCPIPConfig : ConnectionConfig
    {
        private string _ip = "192.168.1.100";
        [System.ComponentModel.Description("On a Active Connection this is the IP of the Partner, on a Passive Connection, this has to be the IP of the used Ethernet Adaptor!")]
        public string IP
        {
            get { return _ip; }
            set { _ip = value; NotifyPropertyChanged("IP"); }
        }

        [System.ComponentModel.Browsable(false)]
        public IPAddress IPasIPAddress
        {
            get
            {
                if (string.IsNullOrEmpty(_ip) && PassiveConnection)
                    return IPAddress.Any;
                return IPAddress.Parse(_ip);
            }
        }


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

        private bool _dontUseFixedTcpLength = false;
        [System.ComponentModel.Description("If this is set, we dont wait to recieve e telegramm with the full Length, but then also only the fields are filled, wich can be filled!")]
        public bool DontUseFixedTCPLength
        {
            get { return _dontUseFixedTcpLength; }
            set { _dontUseFixedTcpLength = value; }
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
