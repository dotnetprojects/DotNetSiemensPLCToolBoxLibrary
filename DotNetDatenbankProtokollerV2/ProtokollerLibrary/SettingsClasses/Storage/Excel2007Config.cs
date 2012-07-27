using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage
{
    //[DataContract(Namespace = "")]
    public class Excel2007Config : StorageConfig
    {
        private string _filename = "c:\\tabelle.xlsx";
        [Description("Possible PlaceHolders: {yyyy}, {yy}, {MM}, {dd}, {hh}, {mm}, {ss}")]
        public string Filename
        {
            get { return this._filename; }
            set { this._filename = value; NotifyPropertyChanged("Filename"); }
        }

        public string ParseFileName()
        {
            var dt = DateTime.Now;
            var retVal = Filename;

            retVal = retVal.Replace("{dd}", dt.ToString("dd"));
            retVal = retVal.Replace("{MM}", dt.ToString("MM"));
            retVal = retVal.Replace("{yyyy}", dt.ToString("yyyy"));
            retVal = retVal.Replace("{yy}", dt.ToString("yy"));
            retVal = retVal.Replace("{hh}", dt.ToString("hh"));
            retVal = retVal.Replace("{mm}", dt.ToString("mm"));
            retVal = retVal.Replace("{ss}", dt.ToString("ss"));

            return retVal;
        }

        private bool _append;
        public bool Append
        {
            get { return _append; }
            set { _append = value; NotifyPropertyChanged("Append"); }
        }

        [XmlIgnore]
        public List<string> DatabaseFieldTypes
        {
            get { return new List<string>() { "AUTO" }; }
        }

        public override string ToString()
        {
            return "Excel-2007-File (Filename=" + Filename + ")";
        }
    }
}
