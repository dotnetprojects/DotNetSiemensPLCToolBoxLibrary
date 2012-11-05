using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using DotNetSimaticDatabaseProtokollerLibrary.Common;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage;
using LumenWorks.Framework.IO.Csv;

namespace DotNetSimaticDatabaseProtokollerLibrary.Databases.CSVFile
{
    public class CSVStorage : DBBaseClass, IDBViewable
    {
        private Action<string> _newDataCallback;
        public CSVStorage(Action<string> NewDataCallback)
        {
            _newDataCallback = NewDataCallback;
        }

        private CSVConfig myConfig;
        private IEnumerable<DatasetConfigRow> fieldList;
        private string dataTable;
        
        private System.IO.StreamWriter writer = null;

        public override void Close()
        {
            if (myThread != null)
                myThread.Abort();
            if (writer != null)
                writer.Close();
        }

        public override void Connect_To_Database(StorageConfig config)
        {
            myConfig = config as CSVConfig;
            if (myConfig == null)
                throw new Exception("Database Config is NULL");

            if (!string.IsNullOrEmpty(myConfig.NetworkUserName))
            {
                try
                {                    
                    networkShare = new NetworkShare(Path.GetDirectoryName(myConfig.Textfile), myConfig.NetworkUserName, myConfig.NetworkPassword);
                }
                catch(Exception ex)
                {
                    if (!RaiseThreadExceptionOccured(this, ex)) 
                        Logging.LogText("Exception: ", ex, Logging.LogLevel.Error);
                }
            }
        }

        private NetworkShare networkShare;

        protected override void CreateOrModify_TablesAndFields(string dataTable, DatasetConfig datasetConfig)
        {
            this.dataTable = dataTable;
            this.datasetConfig = datasetConfig;
            
            this.fieldList = datasetConfig.DatasetConfigRows;


            writeHeader(myConfig.ParseTextFilname());
            
        }

        private void writeHeader(string filename)
        {
            string zeile = "";
            
            if (!string.IsNullOrEmpty(datasetConfig.DateTimeDatabaseField))
            {
                if (myConfig.UseQuotes)
                    zeile += "\"" + datasetConfig.DateTimeDatabaseField + "\"";
                else
                    zeile += datasetConfig.DateTimeDatabaseField;
            }

            foreach (DatasetConfigRow myFeld in fieldList)
            {
                if (zeile != "")
                    zeile += myConfig.Seperator;
                if (myConfig.UseQuotes)
                    zeile += "\"" + myFeld.DatabaseField + "\"";
                else
                    zeile += myFeld.DatabaseField;
            }

            if (!System.IO.File.Exists(filename) || !myConfig.Append)
            {
                writer = new System.IO.StreamWriter(filename);
                writer.WriteLine(zeile);
                writer.Close();
            }

            DeleteOldFiles();
        }

        private void DeleteOldFiles()
        {
            if (string.IsNullOrEmpty(myConfig.DeletionSearchPattern) || myConfig.DeleteCSVsOlderThen.Ticks == 0)
                return;
            
            var dir = Path.GetDirectoryName(myConfig.ParseTextFilname());

            dir += dir.EndsWith("\\") ? "" : "\\";

            var files = Directory.GetFiles(dir, myConfig.DeletionSearchPattern);
            foreach (var file in files)
            {
                var info = new FileInfo(file);
                if (info.LastWriteTimeUtc < DateTime.UtcNow.Subtract(myConfig.DeleteCSVsOlderThen))
                {
                    info.Delete();
                }
            }
        }

        protected override bool _internal_Write()
        {
            var fnm = myConfig.ParseTextFilname();
            writeHeader(fnm);

            string zeilen = "";
            for (int n = 0; n < _maxAdd; n++)
            {
                string zeile = "";

                IEnumerable<object> values = _intValueList[n];
                var addDateTime = _intDateTimesList[n];

                if (!string.IsNullOrEmpty(datasetConfig.DateTimeDatabaseField))
                {
                    string akV = "";
                    akV = addDateTime.ToString(datasetConfig.DateTimeDatabaseFieldFormat);
                    if (myConfig.UseQuotes)
                        zeile += "\"" + akV + "\"";
                    else
                        zeile += akV;
                }

                foreach (object akValue in values)
                {
                    if (zeile != "")
                        zeile += myConfig.Seperator;
                    string akV = "";
                    if (akValue != null)
                    {
                        if (akValue is float)
                        {
                            var fl = (float) akValue;
                            akV = fl.ToString();
                            if (myConfig.SwitchPunctation)
                                akV = akV.Replace('.', '!').Replace(',', '.').Replace('!', ',');
                        }
                        else
                            akV = akValue.ToString();
                    }
                    if (myConfig.UseQuotes)
                        zeile += "\"" + akV + "\"";
                    else
                        zeile += akV;
                }

                zeilen += zeile + Environment.NewLine;
            }

            writer = new System.IO.StreamWriter(fnm, true);
            writer.Write(zeilen);
            writer.Close();

            return true;
        }

        public override void Dispose()
        {
            if (myThread != null)
                myThread.Abort();
            if (networkShare != null)
                networkShare.Dispose();
        }

        #region IDBViewable

        public DataTable ReadData(DatasetConfig datasetConfig, string filter, long Start, int Count, DateTime? Fromdate, DateTime? ToDate)
        {
            try
            {
                using (CsvReader csv = new CsvReader(new StreamReader(myConfig.ParseTextFilname()), true, myConfig.Seperator))
                {
                    int fieldCount = csv.FieldCount;
                    string[] headers = csv.GetFieldHeaders();

                    DataTable tbl = new DataTable();

                    int n = 0;
                    long ende = Start + Count;

                    for (int i = 0; i < fieldCount; i++) tbl.Columns.Add(headers[i]);

                    while (csv.ReadNextRecord())
                    {
                        if (n >= Start && n < ende)
                        {
                            string[] values = new string[fieldCount];
                            for (int i = 0; i < fieldCount; i++)
                            {
                                values[i] = csv[i];
                                if (!string.IsNullOrEmpty(filter))
                                {
                                    if (csv[i].ToLower().Contains(filter.ToLower()))
                                    {
                                        goto brk;
                                    }
                                }
                            }
                            tbl.Rows.Add(values);
                        }
                        brk:
                        if (n > ende) return tbl;
                        n++;
                    }
                    return tbl;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public Int64 ReadCount(DatasetConfig datasetConfig)
        {
            using (CsvReader csv = new CsvReader(new StreamReader(myConfig.ParseTextFilname()), true, myConfig.Seperator))
            {
                int n = 0;

                while (csv.ReadNextRecord())
                {
                    n++;
                }
                return n;
            }
        }
        #endregion
    }
}
