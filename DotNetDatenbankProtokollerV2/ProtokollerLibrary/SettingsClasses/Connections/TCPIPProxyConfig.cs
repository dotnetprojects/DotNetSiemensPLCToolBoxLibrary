using System.Net;
using System.Runtime.Serialization;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections
{
    public class TCPIPProxyConfig : ConnectionConfig
    {
        private string _incomingIp = "192.168.1.100";
        public string IncomingIP
        {
            get { return _incomingIp; }
            set { _incomingIp = value; NotifyPropertyChanged("IncomingIP"); }
        }

        public IPAddress IncomingIPasIPAddress { get { return IPAddress.Parse(_incomingIp); } }

        private int _incomingPort = 2000;
        public int IncomingPort
        {
            get { return _incomingPort; }
            set { _incomingPort = value; NotifyPropertyChanged("IncomingPort"); }
        }

        private bool _incommingPassiveConnection;
        [System.ComponentModel.Description("If not set, the Connection trys to contact the Partner, if Passive, it waits for the Partner to connect!")]
        public bool IncommingPassiveConnection
        {
            get { return _incommingPassiveConnection; }
            set { _incommingPassiveConnection = value; NotifyPropertyChanged("IncommingPassiveConnection"); }
        }

        private string _outgoingIP = "192.168.1.100";
        public string OutgoingIP
        {
            get { return _outgoingIP; }
            set { _outgoingIP = value; NotifyPropertyChanged("OutgoingIP"); }
        }
        
        public IPAddress OutgoingIPasIPAddress { get { return IPAddress.Parse(_outgoingIP); } }


        private int _outgoingPort = 2000;
        public int OutgoingPort
        {
            get { return _outgoingPort; }
            set { _outgoingPort = value; NotifyPropertyChanged("OutgoingPort"); }
        }

        private bool _outgoingPassiveConnection;
        [System.ComponentModel.Description("If not set, the Connection trys to contact the Partner, if Passive, it waits for the Partner to connect!")]
        public bool OutgoingPassiveConnection
        {
            get { return _outgoingPassiveConnection; }
            set { _outgoingPassiveConnection = value; NotifyPropertyChanged("OutgoingPassiveConnection"); }
        }

        private int _multiTelegramme;
        [System.ComponentModel.Description("When there is more than one Dataset in a TCP Telegramm, increase this to the count!")]
        public int MultiTelegramme
        {
            get { return _multiTelegramme; }
            set { _multiTelegramme = value; NotifyPropertyChanged("MultiTelegramme"); }
        }

        public override string ToString()
        {
            //return "TCP/IP-Verbindung (IP:" + IP + ", Port:" + Port.ToString() + ", Passive:" + PassiveConnection.ToString() + ")";
            return base.ToString();
        }
    }
}
