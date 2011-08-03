using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;

namespace DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces
{
    public interface IDBViewableSQL
    {        
        DataTable ReadData(DatasetConfig datasetConfig, string sql, int Count);
    }
}
