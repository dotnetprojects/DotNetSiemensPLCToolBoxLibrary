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
    public class CSVStorage : IDBInterface, IDBViewable
    {
        private CSVConfig myConfig;
        private IEnumerable<DatasetConfigRow> fieldList;
        private string dataTable;
        private string insertCommand = "";

        private System.IO.StreamWriter writer = null;

        public void Close()
        {
            if (myThread != null)
                myThread.Abort();
            if (writer != null)
                writer.Close();
        }

        public event ThreadExceptionEventHandler ThreadExceptionOccured;

        public void Connect_To_Database(StorageConfig config)
        {
            myConfig = config as CSVConfig;
            if (myConfig == null)
                throw new Exception("Database Config is NULL");
        }

        public void CreateOrModify_TablesAndFields(string dataTable, DatasetConfig datasetConfig)
        {
            this.dataTable = dataTable;
            this.fieldList = datasetConfig.DatasetConfigRows;

            string zeile = "";
            foreach (DatasetConfigRow myFeld in fieldList)
            {
                if (zeile != "")
                    zeile += myConfig.Seperator;
                if (myConfig.UseQuotes)
                    zeile += "\"" + myFeld.DatabaseField + "\"";
                else
                    zeile += myFeld.DatabaseField;
            }

            if (!System.IO.File.Exists(myConfig.Textfile) || !myConfig.Append)
            {
                writer = new System.IO.StreamWriter(myConfig.Textfile);
                writer.WriteLine(zeile);
                writer.Close();
            }
        }

        private Thread myThread;

        private List<IEnumerable<object>> _intValueList = new List<IEnumerable<Object>>();
        private int _maxAdd = 0;

        /// <summary>
        /// The write is added to a List and then put into an extra Thread, so that the PLC gets it's quitt imidiatly
        /// </summary>
        /// <param name="values"></param>
        public void Write(IEnumerable<object> values)
        {
            lock (_intValueList)
                _intValueList.Add(values);

            if (myThread == null)
            {
                myThread = new Thread(new ThreadStart(ThreadProc));
                myThread.Name = "Thread from Storage: " + myConfig.Name + " for Table: " + dataTable;
                myThread.Start();
            }
        }

        private void ThreadProc()
        {
            try
            {
                while (true)
                {
                    if (_intValueList.Count > 0)
                    {
                        lock (_intValueList)
                            _maxAdd = _intValueList.Count;

                        try
                        {
                            _internal_Write();
                        }
                        catch (ThreadAbortException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            if (ThreadExceptionOccured != null)
                                ThreadExceptionOccured.Invoke(this, new ThreadExceptionEventArgs(ex));
                            else
                                Logging.LogText(ex.Message, Logging.LogLevel.Error);
                        }

                        _intValueList.RemoveRange(0, _maxAdd);
                    }
                    else
                        Thread.Sleep(20);
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
        }



        public bool _internal_Write()
        {
            string zeilen = "";
            for (int n = 0; n < _maxAdd; n++)
            {
                string zeile = "";

                IEnumerable<object> values = _intValueList[n];

                foreach (object akValue in values)
                {
                    if (zeile != "")
                        zeile += myConfig.Seperator;
                    string akV = "";
                    if (akValue != null)
                        akV = akValue.ToString();
                    if (myConfig.UseQuotes)
                        zeile += "\"" + akV + "\"";
                    else
                        zeile += akV;
                }

                zeilen += zeile + Environment.NewLine;
            }

            writer = new System.IO.StreamWriter(myConfig.Textfile, true);
            writer.Write(zeilen);
            writer.Close();

            return true;
        }

        public void Dispose()
        {
            if (myThread != null)
                myThread.Abort();
        }

        #region IDBViewable

        public DataTable ReadData(DatasetConfig datasetConfig, long Start, int Count)
        {
            try
            {
                using (CsvReader csv = new CsvReader(new StreamReader(myConfig.Textfile), true, myConfig.Seperator))
                {
                    int fieldCount = csv.FieldCount;
                    string[] headers = csv.GetFieldHeaders();

                    DataTable tbl = new DataTable();
                    
                    int n = 0;
                    long ende = Start + Count;

                    for (int i = 0; i < fieldCount; i++)
                        tbl.Columns.Add(headers[i]);

                    while (csv.ReadNextRecord())
                    {
                        if (n >= Start && n < ende)
                        {
                            string[] values = new string[fieldCount];
                            for (int i = 0; i < fieldCount; i++)
                                values[i] = csv[i];
                            tbl.Rows.Add(values);
                        }
                        if (n > ende)
                            return tbl;
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
            using (CsvReader csv = new CsvReader(new StreamReader(myConfig.Textfile), true, myConfig.Seperator))
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
