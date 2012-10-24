using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.General;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets
{
    public class DatasetConfig : INotifyPropertyChanged
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

        public DatasetConfig()
        {
            _datasetConfigRows = new ObservableCollection<DatasetConfigRow>();
            _triggerTimeSpan = new TimeSpan(0, 0, 0, 0, 500);
        }

        public DatasetConfig Clone()
        {
            return SerializeToString<DatasetConfig>.DeSerialize(SerializeToString<DatasetConfig>.Serialize(this));
        }

        private bool _UseFloatIfMultiplierIsUsed = false;
        public bool UseFloatIfMultiplierIsUsed
        {
            get { return _UseFloatIfMultiplierIsUsed; }
            set
            {
                _UseFloatIfMultiplierIsUsed = value;
                NotifyPropertyChanged("UseFloatIfMultiplierIsUsed");
            }
        }

        [Browsable(false)]
        public string ObjectAsString
        {
            get { return ToString(); }
        }

        private StorageConfig _storage; 
        public StorageConfig Storage
        {
            get { return _storage; }
            set { _storage = value; NotifyPropertyChanged("Storage"); }
        }

        private int _maxDatasets = 1000000;
        [System.ComponentModel.Description("0 means unlimited. If a Value bigger than 0 is set, only this amout of Datasets will be stored, the oldest one will be removed! (Only used if supported by the Storrage!)")]
        public int MaxDatasets
        {
            get { return _maxDatasets; }
            set { _maxDatasets = value; NotifyPropertyChanged("MaxDatasets"); }
        }

        private string _dateTimeDatabaseField;
        public string DateTimeDatabaseField
        {
            get { return _dateTimeDatabaseField; }
            set { _dateTimeDatabaseField = value; NotifyPropertyChanged("DateTimeDatabaseField"); }
        }

        private string _dateTimeDatabaseFieldFormat = "yyyy.MM.dd-HH:mm:ss";

        public string DateTimeDatabaseFieldFormat
        {
            get { return _dateTimeDatabaseFieldFormat; }
            set
            {
                _dateTimeDatabaseFieldFormat = value;
                if (string.IsNullOrEmpty(value))
                    _dateTimeDatabaseFieldFormat = "yyyy.MM.dd-HH:mm:ss";
                NotifyPropertyChanged("DateTimeDatabaseFieldFormat");
            }
        }

        private ObservableCollection<DatasetConfigRow> _datasetConfigRows;
        public ObservableCollection<DatasetConfigRow> DatasetConfigRows
        {
            get { return _datasetConfigRows; }
            set { _datasetConfigRows = value; NotifyPropertyChanged("DatasetConfigRows"); }
        }

        private string _name;
        public virtual string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged("Name"); }
        }

        #region Time Trigger        
        private TimeSpan _triggerTimeSpan;
        [XmlIgnore]
        public TimeSpan TriggerTimeSpan
        {
            get { return _triggerTimeSpan; }
            set { _triggerTimeSpan = value; NotifyPropertyChanged("TriggerTimeSpan"); }
        }
        [XmlElement("TriggerTimeSpan")]
        public long TriggerTimeSpanTicks //Because Timespan could not be Serialized, this helper Property is there!
        {
            get { return _triggerTimeSpan.Ticks; }
            set { _triggerTimeSpan = new TimeSpan(value); }
        }
        #endregion

        #region Tag Handshake Trigger
        private PLCTag _triggerReadBit;
        public PLCTag TriggerReadBit
        {
            get { return _triggerReadBit; }
            set { _triggerReadBit = value; NotifyPropertyChanged("TriggerReadBit"); }
        }

        private PLCTag _triggerQuittBit;
        public PLCTag TriggerQuittBit
        {
            get { return _triggerQuittBit; }
            set { _triggerQuittBit = value; NotifyPropertyChanged("TriggerQuittBit"); }
        }

        private ConnectionConfig _triggerconnection;
        public ConnectionConfig TriggerConnection
        {
            get { return _triggerconnection; }
            set { _triggerconnection = value; NotifyPropertyChanged("TriggerConnection"); }
        }
        #endregion

        #region Quartz Trigger

        private string _cronTab;
        public string CronTab
        {
            get { return _cronTab; }
            set { _cronTab = value; NotifyPropertyChanged("CronTab"); }
        }

        #endregion

        #region Database Trigger

        private string _triggerSQL;
        public string TriggerSQL
        {
            get { return _triggerSQL; }
            set { _triggerSQL = value; NotifyPropertyChanged("TriggerSQL"); }
        }

        #endregion

        private DatasetTriggerType _trigger;
        public DatasetTriggerType Trigger
        {
            get { return _trigger; }
            set { _trigger = value; NotifyPropertyChanged("Trigger"); }
        }

        
    }
}
