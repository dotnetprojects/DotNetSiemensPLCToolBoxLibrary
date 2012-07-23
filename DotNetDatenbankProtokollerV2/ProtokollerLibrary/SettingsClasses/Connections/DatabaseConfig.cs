using System.Net;
using System.Runtime.Serialization;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections
{
    public class DatabaseConfig : ConnectionConfig
    {
        public enum Databasedriver
        {
            PostgreSQL,
            MSSQL,
            MySQL,
            SQLlite,
            Firebird,

        }

        private string _connectionString;
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; NotifyPropertyChanged("ConnectionString"); }
        }

        private Databasedriver _databaseDriver;
        public Databasedriver DatabaseDriver
        {
            get { return _databaseDriver; }
            set { _databaseDriver = value; NotifyPropertyChanged("DatabaseDriver"); }
        }

        private string _abfrage = "SELECT * FROM [Database].[Table]";
        public string Abfrage
        {
            get { return _abfrage; }
            set { _abfrage = value; NotifyPropertyChanged("IP"); }
        }

        public override string ToString()
        {
            return "Datenbank-Verbindung ()";
        }
    }
}
