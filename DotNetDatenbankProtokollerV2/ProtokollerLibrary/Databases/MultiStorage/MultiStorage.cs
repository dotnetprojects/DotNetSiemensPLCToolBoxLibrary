using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Documents;

using DotNetSimaticDatabaseProtokollerLibrary.Common;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;
using DotNetSimaticDatabaseProtokollerLibrary.Remoting;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage;

using LumenWorks.Framework.IO.Csv;

namespace DotNetSimaticDatabaseProtokollerLibrary.Databases.MultiStorage
{
    public class MultiStorage : DBBaseClass, IDBViewable
    {
        private ProtokollerConfiguration _protokollerConfiguration;

        private Dictionary<DBBaseClass, StorageConfig> storrageConfigs = new Dictionary<DBBaseClass, StorageConfig>();

        private Action<string> _newDataCallback;
        public MultiStorage(ProtokollerConfiguration protokollerConfiguration, StorageConfig config, Action<string> NewDataCallback)
        {
            this._protokollerConfiguration = protokollerConfiguration;
            this._newDataCallback = NewDataCallback;

            storages = new List<DBBaseClass>();
            this.myConfig = config as MultiStorageConfig;

            foreach (string s in myConfig.StorageList)
            {
                var storCfg = _protokollerConfiguration.Storages.First(x => x.Name == s);
                var stor = (DBBaseClass)StorageHelper.GetStorage(null, storCfg, RemotingServer.ClientComms.CallNotifyEvent);
                storages.Add(stor);

                storrageConfigs.Add(stor, storCfg);

            }   
        }

        
        private MultiStorageConfig myConfig;
        private IEnumerable<DatasetConfigRow> fieldList;
        private string dataTable;

        private List<DBBaseClass> storages;

        public override void Close()
        {
            foreach (var s in storages)
            {
                s.Close();
            } 
        }

        public override void Initiate(DatasetConfig dsConfig)
        {
            foreach (var s in storages)
            {
                dsConfig.Storage = storrageConfigs[s];
                s.Initiate(dsConfig);
            }

            dsConfig.Storage = myConfig;
        }

        protected override bool _internal_Write()
        {
            return true;
        }

        public override void Connect_To_Database(StorageConfig config)
        {
            foreach (var s in storages)
            {
                s.Connect_To_Database(storrageConfigs[s]);
            }                    
        }

        public override void CreateOrModify_TablesAndFields(string dataTable, DatasetConfig datasetConfig)
        {
            foreach (var s in storages)
            {
                s.CreateOrModify_TablesAndFields(dataTable, datasetConfig);
            }                        
        }

        public override void Write(IEnumerable<object> values)
        {
            foreach (var s in storages)
            {
                s.Write(values);
            }             
        }

        public override void Dispose()
        {
            foreach (var s in storages)
            {
                s.Dispose();
            }       
        }

        public DataTable ReadData(DatasetConfig datasetConfig, string filter, long Start, int Count, DateTime? FromDate, DateTime? ToDate)
        {
            var s = (IDBViewable)storages.First();
            return s.ReadData(datasetConfig, filter, Start, Count, FromDate, ToDate);
        }

        public long ReadCount(DatasetConfig datasetConfig)
        {
            var s = (IDBViewable)storages.First();
            return s.ReadCount(datasetConfig);
        }
    }
}
