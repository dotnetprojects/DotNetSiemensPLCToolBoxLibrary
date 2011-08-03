using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage
{
    public class CSVConfig : StorageConfig
    {
        private string _textfile = "c:\\csvfile.csv";
        public string Textfile
        {
            get { return this._textfile; }
            set { this._textfile = value; NotifyPropertyChanged("Textfile"); }
        }

        private char _seperator = ';';
        public char Seperator
        {
            get { return _seperator; }
            set { _seperator = value; NotifyPropertyChanged("Seperator"); }
        }

        private bool _useQuotes;
        public bool UseQuotes
        {
            get { return _useQuotes; }
            set { _useQuotes = value; NotifyPropertyChanged("UseQuotes"); }
        }

        private bool _append = true;
        public bool Append
        {
            get { return _append; }
            set { _append = value; NotifyPropertyChanged("Append"); }
        }

        public List<string> DatabaseFieldTypes
        {
            get { return new List<string>() { "TEXT" }; }
        }

        public override string ToString()
        {
            return "CSV-File (Filename=" + Textfile + ", Seperator=" + Seperator + ", UseQuotes=" + UseQuotes.ToString() + ")";
        }
    }
}
