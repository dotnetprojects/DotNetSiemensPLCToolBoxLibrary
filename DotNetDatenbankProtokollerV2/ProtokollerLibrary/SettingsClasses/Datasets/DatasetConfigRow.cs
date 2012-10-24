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
            set
            {
                _databaseField = value;
                NotifyPropertyChanged("DatabaseField");
            }
        }

        private string _databaseFieldType;

        public string DatabaseFieldType
        {
            get { return _databaseFieldType; }
            set
            {
                _databaseFieldType = value;
                NotifyPropertyChanged("DatabaseFieldType");
            }
        }

        private int _databaseFieldSize;

        public int DatabaseFieldSize
        {
            get { return _databaseFieldSize; }
            set
            {
                _databaseFieldSize = value;
                NotifyPropertyChanged("DatabaseFieldSize");
            }
        }

        //Hier gibts auch Interne Felder wie DateTime und Index!
        private ConnectionConfig _connection;

        public ConnectionConfig Connection
        {
            get { return _connection; }
            set
            {
                _connection = value;
                NotifyPropertyChanged("Connection");
            }
        }

        //This is not needed when using a TCP/IP Connection, but we need the Type and Length! So we ignor the Address when using this!
        private PLCTag _plcTag = new PLCTag();

        public PLCTag PLCTag
        {
            get { return _plcTag; }
            set
            {
                _plcTag = value;
                NotifyPropertyChanged("PLCTag");
            }
        }

        private double _multiplier = 0;

        public double Multiplier
        {
            get { return _multiplier; }
            set
            {
                _multiplier = value;
                NotifyPropertyChanged("Multiplier");
            }
        }

        private double _precision = 0;
        [Description("This is used for Precision (When this is used, Databasefieldtype has to be a Textfield, because Value is Converted to Text)")]
        public double Precision
        {
            get { return _precision; }
            set
            {
                _precision = value;
                NotifyPropertyChanged("Precision");
            }
        }

        private string _stringSubFields = "";
        [Description("List of Fixed Length Fields in the String (f.E: TYPE|4|NR|2...)")]
        public string StringSubFields
        {
            get { return _stringSubFields; }
            set
            {
                _stringSubFields = value;
                NotifyPropertyChanged("StringSubFields");
            }
        }

        public object Value(bool UseFloatIfMultiplierIsUsed)
        {
            if ((Multiplier != 0 || Precision != 0) && (PLCTag.Value is double || PLCTag.Value is float || PLCTag.Value is byte || PLCTag.Value is sbyte || PLCTag.Value is Int16 || PLCTag.Value is UInt16 || PLCTag.Value is int || PLCTag.Value is uint || PLCTag.Value is byte || PLCTag.Value is sbyte || PLCTag.Value is Int64 || PLCTag.Value is UInt64))
            {
                string sFormat = "";
                if (Precision != 0)
                {
                    sFormat = "F" + Precision.ToString();
                }

                try
                {
                    if (Multiplier != 0)
                    {
                        if (UseFloatIfMultiplierIsUsed) return (Convert.ToDouble(PLCTag.Value) * Multiplier);
                        else return (Convert.ToDouble(PLCTag.Value) * Multiplier).ToString(sFormat);
                    }
                    else return (Convert.ToDouble(PLCTag.Value)).ToString(sFormat);
                }
                catch (Exception)
                {
                }
            }
            return PLCTag.Value;
        }
    }
}
