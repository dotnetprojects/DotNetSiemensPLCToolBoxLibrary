using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage
{
    //[DataContract(Namespace = "")]   
    public class ODBCConfig : StorageConfig
    {
        private string _connectionstring = "Data Source=myServerAddress;Initial Catalog=myDataBase;User Id=myUsername;Password=myPassword;";
        //[DataMember]
        public string ConnectionString
        {
            get { return _connectionstring; }
            set { _connectionstring = value; NotifyPropertyChanged("ConnectionString"); }
        }

        private string _database = "myDataBase";
        //[DataMember]
        public string Database
        {
            get { return _database; }
            set { _database = value; NotifyPropertyChanged("Database"); }
        }

        [XmlIgnore]
        public override List<string> DatabaseFieldTypes
        {
            get { return new List<string>() { "CHAR", "VARCHAR", "LONG VARCHAR", "WCHAR", "VARWCHAR", "LONGWVARCHAR", "DECIMAL", "NUMERIC", "SMALLINT", "INTEGER", "REAL", "FLOAT", "DOUBLEPRECISION", "BIT", "TINYINT", "BIGINT", "BINARY", "VARBINARY", "LONG VARBINARY", "DATE", "TIME", "TIMESTAMP", "INTERVAL MONTH", "INTERVAL YEAR", "INTERVAL DAY", "INTERVAL HOUR", "INTERVAL MINUTE", "INTERVAL SECOND", "GUID" }; }
        }

        public override string ToString()
        {
            return "ODBC-Verbindung (ConnectionString=" + ConnectionString + ")";
        }
    }
}
