using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage
{
    public class MultiStorageConfig : StorageConfig
    {
        private List<string> _storageList = new List<string>();
        public List<string> StorageList
        {
            get { return _storageList; }
            set { _storageList = value; NotifyPropertyChanged("StorageList"); }
        }

        public override string ToString()
        {
            return "MultiStorage";
        }
    }
}
