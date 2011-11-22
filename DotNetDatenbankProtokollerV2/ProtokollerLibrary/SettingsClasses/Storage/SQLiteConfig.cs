using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage
{
    //[DataContract(Namespace = "")]   
    public class SQLiteConfig : StorageConfig
    {
        private string _databasefile = "c:\\sqllitedb.db4";
        //[DataMember]
        public string DatabaseFile
        {
            get { return this._databasefile; }
            set { this._databasefile = value; NotifyPropertyChanged("DatabaseFile"); }
        }

        [XmlIgnore]
        public override List<string> DatabaseFieldTypes
        {
            get { return new List<string>() { "INTEGER", "REAL", "TEXT", "BLOB", "NUMERIC" }; }
        }

        public override string ToString()
        {
            return "SQLite-File (Filename=" + DatabaseFile + ")";
        }        
    }    
}
