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

        public override string ToString()
        {
            return Configuration.ToString();
        }
    }
}
