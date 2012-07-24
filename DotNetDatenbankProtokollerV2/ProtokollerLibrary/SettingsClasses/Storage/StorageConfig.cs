using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage
{
    [XmlInclude(typeof(AccessConfig))]
    [XmlInclude(typeof(CSVConfig))]
    [XmlInclude(typeof(ExcelConfig))]
    [XmlInclude(typeof(MsSQLConfig))]
    [XmlInclude(typeof(MySQLConfig))]
    [XmlInclude(typeof(ODBCConfig))]
    [XmlInclude(typeof(PostgreSQLConfig))]
    [XmlInclude(typeof(SQLiteConfig))]
    [XmlInclude(typeof(Excel2007Config))]
    [XmlInclude(typeof(PLCConfig))] 
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
        [XmlIgnore]
        public virtual List<string> DatabaseFieldTypes
        {
            get { return new List<string>() {"Test"}; }
        }

        public virtual string GetDefaultDatabaseFieldTypeForLibNoDaveTag(PLCTag tag)
        {
            switch (tag.LibNoDaveDataType.ToString().ToLower().Trim())
            {
                case "float":
                    return "float";
                    break;

                case "int":
                    return "int";
                    break;

                case "word":
                    return "word";
                    break;

                case "date":
                    return "date";
                    break;

                case "datetime":
                    return "datetime";
                    break;

                case "time":
                    return "time";
                    break;

                case "dint":
                    return "bigint";
                    break;               
            }
            return "";
        }
    }
}
