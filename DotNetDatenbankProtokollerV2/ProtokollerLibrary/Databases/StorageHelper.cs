using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.CSVFile;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Excel;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.PostgreSQL;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.SQLite;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage;

namespace DotNetSimaticDatabaseProtokollerLibrary.Databases
{
    public static class StorageHelper        
    {
        public static IDBInterface GetStorage(DatasetConfig cfg)
        {
            if (cfg.Storage is SQLiteConfig)
                return new SQLLiteStorage();
            else if (cfg.Storage is CSVConfig)
                return new CSVStorage();
            else if (cfg.Storage is ExcelConfig)
                return new ExcelStorage();
            else if (cfg.Storage is PostgreSQLConfig)
                return new PostgreSQLStorage();
            return null;
        }
    }
}
