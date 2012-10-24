using System.Runtime.Serialization;
using DotNetSiemensPLCToolBoxLibrary.Communication;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections
{
    //[DataContract(Namespace = "")]
    public class LibNoDaveConfig : ConnectionConfig
    {
        private PLCConnectionConfiguration _configuration;
        //[DataMember]
        public PLCConnectionConfiguration Configuration
        {
            get { return _configuration; }
            set { _configuration = value; NotifyPropertyChanged("Configuration"); }
        }

        public bool SynchronizePLCTime { get; set; }

        public override string Name
        {
            get
            {
                if (Configuration != null)
                    return Configuration.ConnectionName;
                return null;
            }
            set
            {
                if (Configuration != null)
                {
                    Configuration.ConnectionName = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public int _ReconnectInterval = 500;
        public int ReconnectInterval
        {
            get { return _ReconnectInterval; }
            set { _ReconnectInterval = value; }
        }

        private bool _stayConnected = true;
        [System.ComponentModel.Description("If set to false, after every Read Request, the Connection will be closed!")]        
        public bool StayConnected
        {
            get { return _stayConnected; }
            set { _stayConnected = value; }
        }

        public override string ToString()
        {
            return Configuration.ToString();
        }
    }
}
