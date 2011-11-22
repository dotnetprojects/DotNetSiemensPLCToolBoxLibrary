using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage
{
    //[DataContract(Namespace = "")]
    public class ExcelConfig : StorageConfig
    {
        private string _filename = "c:\\tabelle.xls";
        public string Filename
        {
            get { return this._filename; }
            set { this._filename = value; NotifyPropertyChanged("Filename"); }
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
            return "Excel-File (Filename=" + Filename + ")";
        }
    }
}
