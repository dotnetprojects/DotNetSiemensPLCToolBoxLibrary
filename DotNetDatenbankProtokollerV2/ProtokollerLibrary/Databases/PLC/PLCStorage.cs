using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSimaticDatabaseProtokollerLibrary.Common;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage;

/*
Feldzuordnung bei SQLLite
                    case "int":
                    case "dint":
                    case "word":
                    case "dword":
                    case "bcdbyte":
                    case "byte":
                    case "bool": sql += "INTEGER";
                        break;
                    case "real": sql += "REAL";
                        break;
                    case "datetime": sql += "TEXT";
                        break;
                    case "string":
                    case "stringchar": sql += "TEXT";
                        break;
*/


namespace DotNetSimaticDatabaseProtokollerLibrary.Databases.PLC
{
    public class PLCStorage : DBBaseClass
    {
         private Action<string> _newDataCallback;
         public PLCStorage(Action<string> NewDataCallback)
        {
            _newDataCallback = NewDataCallback;
        }

        private PLCConfig myConfig;
        private IEnumerable<DatasetConfigRow> fieldList;

        private string dataTable;
        private PLCConnection _plcConnection = null;
        
        public override void Close()
        {
            if (myThread != null)
                myThread.Abort();
            if (_plcConnection != null)
                _plcConnection.Disconnect();
        }

        public override void Connect_To_Database(StorageConfig config)
        {
            myConfig = config as PLCConfig;

            _plcConnection = new PLCConnection(myConfig.Configuration);
            _plcConnection.AutoConnect = true;
            _plcConnection.Connect();
            Logging.LogTextToLog4Net("Connect_To_Database() => \"" + _plcConnection.Name + "\" => Connect...");
        }

        private string dateFieldName;

        public override void CreateOrModify_TablesAndFields(string dataTable, DatasetConfig datasetConfig)
        {
            this.datasetConfig = datasetConfig;
            this.dataTable = dataTable;
            this.fieldList = datasetConfig.DatasetConfigRows;
        }

        protected override bool _internal_Write()
        {
            try
            {
                for (int n = 0; n < _maxAdd; n++)
                {
                    IEnumerable<object> values = _intValueList[n];
                    var addDateTime = _intDateTimesList[n];

                    List<PLCTag> writeList = new List<PLCTag>();
                    using (IEnumerator<DatasetConfigRow> e1 = fieldList.GetEnumerator())
                    using (IEnumerator<object> e2 = values.GetEnumerator())
                    {
                        while (e1.MoveNext() && e2.MoveNext())
                        {
                            DatasetConfigRow field = e1.Current;
                            Object value = e2.Current;

                            field.PLCTag.Value = value;
                            writeList.Add(field.PLCTag);
                        }
                    }
                    _plcConnection.WriteValues(writeList, true);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            if (_newDataCallback != null)
                _newDataCallback(datasetConfig.Name);

            return true;
        }

        public override void Dispose()
        {
            if (myThread != null)
                myThread.Abort();
            if (_plcConnection != null)
                _plcConnection.Dispose();
        }
    }
}
