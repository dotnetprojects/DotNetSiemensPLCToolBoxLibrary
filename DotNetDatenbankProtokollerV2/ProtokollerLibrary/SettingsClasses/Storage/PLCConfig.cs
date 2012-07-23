using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using DotNetSiemensPLCToolBoxLibrary.Communication;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage
{
    //[DataContract(Namespace = "")]   
    public class PLCConfig : StorageConfig
    {
        private PLCConnectionConfiguration _configuration;
        public PLCConnectionConfiguration Configuration
        {
            get { return _configuration; }
            set { _configuration = value; NotifyPropertyChanged("Configuration"); }
        }

        [XmlIgnore]
        public override List<string> DatabaseFieldTypes
        {
            get { return new List<string>() { "default" }; }
        }

        public override string ToString()
        {
            return "PLC (" + Configuration.ToString() + ")";
        }        
    }    
}
