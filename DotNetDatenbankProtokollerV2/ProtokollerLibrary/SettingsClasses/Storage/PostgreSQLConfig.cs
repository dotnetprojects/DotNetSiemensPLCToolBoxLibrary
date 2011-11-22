using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage
{
    public class PostgreSQLConfig : StorageConfig
    {
        private string _server = "localhost";
        public string Server
        {
            get { return _server; }
            set { _server = value; NotifyPropertyChanged("Server"); }
        }

        private int _port = 5432;
        public int Port
        {
            get { return _port; }
            set { _port = value; NotifyPropertyChanged("Port"); }
        }

        private string _username = "postgres";
        public string Username
        {
            get { return _username; }
            set { _username = value; NotifyPropertyChanged("Username"); }
        }

        private string _password = "postgres";
        public string Password
        {
            get { return _password; }
            set { _password = value; NotifyPropertyChanged("Password"); }
        }

        private string _database = "daten";
        public string Database
        {
            get { return _database; }
            set { _database = value; NotifyPropertyChanged("Database"); }
        }

        [XmlIgnore]
        public override List<string> DatabaseFieldTypes
        {
            get { return new List<string>() { "int8", "serial8", "bit", "boolean", "box", "bytea", "varchar", "varbit", "char", "cidr", "circle", "date", "float8", "inet", "int4", "interval", "line", "lseg", "macaddr", "money", "decimal", "path", "point", "polygon", "float4", "int2", "serial4", "text", "time", "timetz", "timestamp", "timestamptz" }; }
        }

        public override string ToString()
        {
            return "PostgreSQL-Server (Server=" + Server + ", Port=" + Port + ", Username=" + Username + ")";
        }
    }
}
