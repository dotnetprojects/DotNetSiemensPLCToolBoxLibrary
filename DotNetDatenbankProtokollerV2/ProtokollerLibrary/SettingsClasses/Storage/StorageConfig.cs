using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage
{
    //[DataContract(IsReference = true, Namespace = "")]
    //[KnownType(typeof(AccessConfig))]
    //[KnownType(typeof(CSVConfig))]
    //[KnownType(typeof(MsSQLConfig))]
    //[KnownType(typeof(MySQLConfig))]
    //[KnownType(typeof(ODBCConfig))]
    //[KnownType(typeof(PostgreSQLConfig))]
    //[KnownType(typeof(SQLiteConfig))]

    [XmlInclude(typeof(AccessConfig))]
    [XmlInclude(typeof(CSVConfig))]
    [XmlInclude(typeof(ExcelConfig))]
    [XmlInclude(typeof(MsSQLConfig))]
    [XmlInclude(typeof(MySQLConfig))]
    [XmlInclude(typeof(ODBCConfig))]
    [XmlInclude(typeof(PostgreSQLConfig))]
    [XmlInclude(typeof(SQLiteConfig))] 
    public class StorageConfig: INotifyPropertyChanged
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
            get { return ToString();  }
            
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged("Name"); }
        }

        [Browsable(false)]
        public virtual List<string> DatabaseFieldTypes
        {
            get { return new List<string>() {"Test"}; }
        }
    }
}
