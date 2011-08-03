using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using DotNetSiemensPLCToolBoxLibrary.Communication;
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

        private int _maxDatasets;
        [System.ComponentModel.Description("0 means unlimeted. If a Value bigger than 0 is set, only this amout of Datasets will be stored, the oldest one will be removed!")]
        public int MaxDatasets
        {
            get { return _maxDatasets; }
            set { _maxDatasets = value; NotifyPropertyChanged("MaxDatasets"); }
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

        private DatasetTriggerType _trigger;
        public DatasetTriggerType Trigger
        {
            get { return _trigger; }
            set { _trigger = value; NotifyPropertyChanged("Trigger"); }
        }

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
    }
}
