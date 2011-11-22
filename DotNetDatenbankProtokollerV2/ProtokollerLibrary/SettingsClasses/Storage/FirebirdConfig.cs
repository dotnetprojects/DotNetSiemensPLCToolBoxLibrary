using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage
{    
    public class FirebirdConfig : StorageConfig
    {
        private string _databasefile = "c:\\sqllitedb.db4";        
        public string DatabaseFile
        {
            get { return this._databasefile; }
            set { this._databasefile = value; NotifyPropertyChanged("DatabaseFile"); }
        }

        [XmlIgnore]
        public override List<string> DatabaseFieldTypes
        {
            get { return new List<string>() {"INT64", "CHAR", "TIMESTAMP", "DECIMAL", "FLOAT", "BLOB", "INTEGER", "NUMERIC", "DOUBLE", "SMALLINT", "VARCHAR"}; }
        }

        public override string ToString()
        {
            return "Firebird (Filename=" + DatabaseFile + ")";
        }        
    }    
}
