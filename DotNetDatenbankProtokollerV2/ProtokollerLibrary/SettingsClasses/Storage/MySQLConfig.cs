using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage
{
    //[DataContract(Namespace = "")]  
    public class MySQLConfig : StorageConfig
    {
        private string _server = "localhost";
        //[DataMember]
        public string Server
        {
            get { return _server; }
            set { _server = value; NotifyPropertyChanged("Server"); }
        }

        private int _port = 3306;
        //[DataMember]
        public int Port
        {
            get { return _port; }
            set { _port = value; NotifyPropertyChanged("Port"); }
        }

        private string _username = "mysql";
        //[DataMember]
        public string Username
        {
            get { return _username; }
            set { _username = value; NotifyPropertyChanged("Username"); }
        }

        private string _password = "mysql";
        //[DataMember]
        public string Password
        {
            get { return _password; }
            set { _password = value; NotifyPropertyChanged("Password"); }
        }

        private string _database = "daten";
        //[DataMember]
        public string Database
        {
            get { return _database; }
            set { _database = value; NotifyPropertyChanged("Database"); }
        }

        [XmlIgnore]
        public override List<string> DatabaseFieldTypes
        {
            get { return new List<string>() { "bigint", "real", "datetime", "varchar" }; }
        }

        public override string ToString()
        {
            return "MySQL-Server (Server=" + Server + ", Port=" + Port + ", Username=" + Username + ")";
        }
    }
}
