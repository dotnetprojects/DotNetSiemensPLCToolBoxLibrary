using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets
{
    //[DataContract()]
    public class DatasetConfigRow : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));                
            }
        }
        private string _databaseField;
        public string DatabaseField
        {
            get { return _databaseField; }
            set { _databaseField = value; NotifyPropertyChanged("DatabaseField"); }
        }

        private string _databaseFieldType;
        public string DatabaseFieldType
        {
            get { return _databaseFieldType; }
            set { _databaseFieldType = value; NotifyPropertyChanged("DatabaseFieldType"); }
        }

        private int _databaseFieldSize;
        public int DatabaseFieldSize
        {
            get { return _databaseFieldSize; }
            set { _databaseFieldSize = value; NotifyPropertyChanged("DatabaseFieldSize"); }
        }

        //Hier gibts auch Interne Felder wie DateTime und Index!
        private ConnectionConfig _connection;
        public ConnectionConfig Connection
        {
            get { return _connection; }
            set { _connection = value; NotifyPropertyChanged("Connection"); }
        }

        private PLCTag _plcTag=new PLCTag();
        public PLCTag PLCTag
        {
            get { return _plcTag; }
            set { _plcTag = value; NotifyPropertyChanged("PLCTag"); }
        }

        //This is not needed when using a TCP/IP Connection, but we need the Type and Length! So we ignor the Address when using this!
    }   
}
