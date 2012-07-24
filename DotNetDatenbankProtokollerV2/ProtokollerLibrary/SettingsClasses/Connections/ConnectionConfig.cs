using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections
{
    [Serializable()]
    [XmlInclude(typeof(LibNoDaveConfig))]
    [XmlInclude(typeof(TCPIPConfig))]
    [XmlInclude(typeof(DatabaseConfig))]

    //[DataContract(IsReference = true, Namespace = "")]
    //[KnownType(typeof(LibNoDaveConfig))]
    //[KnownType(typeof(TCPIPConfig))]
    public class ConnectionConfig: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
                PropertyChanged(this, new PropertyChangedEventArgs("ObjectAsString"));
            }
        }

        [Browsable(false)]
        public string ObjectAsString
        {
            get { return ToString(); }

        }

        private string _name;
        //[DataMember]
        public virtual string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged("Name"); }
        }
    }
}
