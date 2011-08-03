using System.Runtime.Serialization;

namespace DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage
{    
    //[DataContract(Namespace = "")]
    public class AccessConfig : StorageConfig
    {
        private string _databasefile = "c:\\database.mdx";
        public string DatabaseFile
        {
            get { return this._databasefile; }
            set { this._databasefile = value; NotifyPropertyChanged("DatabaseFile"); }
        }

        public override string ToString()
        {
            return "AccesFile-File (Filename=" + DatabaseFile + ")";
        }
    }
}
