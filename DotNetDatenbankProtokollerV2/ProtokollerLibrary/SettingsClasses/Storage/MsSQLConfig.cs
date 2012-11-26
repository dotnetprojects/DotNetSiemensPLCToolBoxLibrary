using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage
{
    //[DataContract(Namespace = "")]
    public class MsSQLConfig : StorageConfig
    {
        private string _server = "localhost";
        public string Server
        {
            get { return _server; }
            set { _server = value; NotifyPropertyChanged("Server"); }
        }

        private bool _combineMultipleInsertsInATransaction;
        public bool CombineMultipleInsertsInATransaction
        {
            get { return _combineMultipleInsertsInATransaction; }
            set { _combineMultipleInsertsInATransaction = value; NotifyPropertyChanged("CombineMultipleInsertsInATransaction"); }
        }

        private int _port = 1433;
        public int Port
        {
            get { return _port; }
            set { _port = value; NotifyPropertyChanged("Port"); }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; NotifyPropertyChanged("Username"); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; NotifyPropertyChanged("Password"); }
        }

        private string _database;
        public string Database
        {
            get { return _database; }
            set { _database = value; NotifyPropertyChanged("Database"); }
        }

        private string _extendedConnectionString;
        [Description("When this String is set, Username, Password and Port are not used!!")]
        public string ExtendedConnectionString
        {
            get { return _extendedConnectionString; }
            set { _extendedConnectionString = value; NotifyPropertyChanged("ExtendedConnectionString"); }
        }

        [XmlIgnore]
        public override List<string> DatabaseFieldTypes
        {
            get { return new List<string>() { "bigint", "numeric", "bit", "smallint", "decimal", "smallmoney", "int", "tinyint", "money", "float", "date", "datetime", "time", "char", "varchar", "text", "nchar", "nvarchar", "ntext" }; }
        }

        public override string ToString()
        {
            return "MsSQL-Server (Server=" + Server + ", Port=" + Port + ", Username=" + Username + ")";
        }
    }
}
