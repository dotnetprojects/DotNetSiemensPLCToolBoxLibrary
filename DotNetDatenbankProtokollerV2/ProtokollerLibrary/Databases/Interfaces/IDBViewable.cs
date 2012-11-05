using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;

namespace DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces
{
    public interface IDBViewable
    {        
        DataTable ReadData(DatasetConfig datasetConfig, string filter, long Start, int Count, DateTime? FromDate, DateTime? ToDate);

        Int64 ReadCount(DatasetConfig datasetConfig);
    }
}
