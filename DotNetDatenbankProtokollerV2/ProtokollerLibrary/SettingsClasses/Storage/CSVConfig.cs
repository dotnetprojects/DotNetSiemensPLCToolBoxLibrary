using System.Collections.Generic;
using System.ComponentModel;
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

        private bool _switchPunctation;
        [Description("Switches Comma and Point")]         
        public bool SwitchPunctation
        {
            get { return _switchPunctation; }
            set { _switchPunctation = value; NotifyPropertyChanged("SwitchPunctation"); }
        }

        private bool _append = true;
        public bool Append
        {
            get { return _append; }
            set { _append = value; NotifyPropertyChanged("Append"); }
        }

        private string _networkusername = "";
        [Description("When this Username is not Empty, the Protokoller will try to Logon with this name on the Share with wich the Filename starts (for this you need to specify the Filename as a UNC Path like \\\\ServerName\\ShareName\\FileName.csv)"),Category("Network Share")] 
        public string NetworkUserName
        {
            get { return _networkusername; }
            set { _networkusername = value; NotifyPropertyChanged("NetworkUserName"); }
        }
        private string _networkpassword = "";
        [Description("Password for the Network Share"), Category("Network Share")] 
        public string NetworkPassword
        {
            get { return _networkpassword; }
            set { _networkpassword = value; NotifyPropertyChanged("NetworkPassword"); }
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
