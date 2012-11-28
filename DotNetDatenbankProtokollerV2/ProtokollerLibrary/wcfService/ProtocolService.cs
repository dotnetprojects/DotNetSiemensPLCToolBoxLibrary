using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

using DotNetSimaticDatabaseProtokollerLibrary.Databases;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;
using DotNetSimaticDatabaseProtokollerLibrary.wcfService;

namespace DotNetSimaticDatabaseProtokollerLibrary.Protocolling
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public partial class ProtokollerInstance : IProtocolService
    {
        public IEnumerable<string> GetStorageNames()
        {
            return ProtokollerConfiguration.ActualConfigInstance.Datasets.Select(s => s.Name);
        }

        public IEnumerable<ProtocolRow> GetProtocolData(string storageName, int startRow, int maxRows, string filter, DateTime? FromDate, DateTime? ToDate)
        {
            var datasetConfig = ProtokollerConfiguration.ActualConfigInstance.Datasets.FirstOrDefault(s => s.Name == storageName);

            IDBInterface dbInterface = StorageHelper.GetStorage(datasetConfig, null);
            dbInterface.Connect_To_Database(datasetConfig.Storage);
            IDBViewable dbViewable = dbInterface as IDBViewable;
            List<ProtocolRow> list = new List<ProtocolRow>();
            DataTable table = dbViewable.ReadData(datasetConfig, filter, startRow, maxRows, FromDate, ToDate);
            foreach (DataRow row in table.Rows)
            {
                ProtocolRow storageLine = new ProtocolRow();
                var date = row["datetime"];
                if (date is DateTime)
                    storageLine.Timestamp = (DateTime)date;
                else
                    storageLine.Timestamp = DateTime.ParseExact(row["datetime"].ToString(), "yyyy.MM.dd - HH:mm:ss.fff", null);

                storageLine.Telegram = Convert.ToString(row["data"]);
                list.Add(storageLine);
            }

            return list;
        }
    }
}
