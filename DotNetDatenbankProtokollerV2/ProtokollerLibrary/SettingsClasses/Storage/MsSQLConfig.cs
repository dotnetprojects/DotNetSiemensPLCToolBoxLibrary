using System.Runtime.Serialization;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage
{
    //[DataContract(Namespace = "")]
    public class MsSQLConfig : StorageConfig
    {
        private string _server = "localhost";
        //[DataMember]        
        public string Server
        {
            get { return _server; }
            set { _server = value; NotifyPropertyChanged("Server"); }
        }

        private int _port = 1433;
        //[DataMember]
        public int Port
        {
            get { return _port; }
            set { _port = value; NotifyPropertyChanged("Port"); }
        }

        private string _username;
        //[DataMember]
        public string Username
        {
            get { return _username; }
            set { _username = value; NotifyPropertyChanged("Username"); }
        }

        private string _password;
        //[DataMember]
        public string Password
        {
            get { return _password; }
            set { _password = value; NotifyPropertyChanged("Password"); }
        }

        private string _database;
        //[DataMember]
        public string Database
        {
            get { return _database; }
            set { _database = value; NotifyPropertyChanged("Database"); }
        }

        public override string ToString()
        {
            return "MsSQL-Server (Server=" + Server + ", Port=" + Port + ", Username=" + Username + ")";
        }
    }
}
