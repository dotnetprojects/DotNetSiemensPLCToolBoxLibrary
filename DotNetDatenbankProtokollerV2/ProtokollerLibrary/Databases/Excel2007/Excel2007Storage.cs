using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using DotNetSimaticDatabaseProtokollerLibrary.Common;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage;
using OfficeOpenXml;

namespace DotNetSimaticDatabaseProtokollerLibrary.Databases.Excel
{
    public class Excel2007Storage : IDBInterface
    {
        private Action<string> _newDataCallback;
        public Excel2007Storage(Action<string> NewDataCallback)
        {
            _newDataCallback = NewDataCallback;
        }

        private Excel2007Config myConfig;
        //private IEnumerable<DatasetConfigRow> fieldList;
        private string TableName;
        private string FileName;
        

        //private System.IO.StreamWriter writer = null;

        public void Close()
        {
            if (myThread != null)
                myThread.Abort();            
        }

        public event ThreadExceptionEventHandler ThreadExceptionOccured;

        public void Connect_To_Database(StorageConfig config)
        {                        
            myConfig = config as Excel2007Config;            
            if (myConfig == null)
                throw new Exception("Database Config is NULL");

            FileName = myConfig.Filename;
        }

        private DatasetConfig _datasetConfig;


        public void CreateOrModify_TablesAndFields(string dataTable, DatasetConfig datasetConfig)
        {            
            TableName = datasetConfig.Name;
            _datasetConfig = datasetConfig;

            ExcelPackage ep;
            /*
            if (File.Exists(FileName))
                
                workbook = Workbook.Load(FileName);
            else
                workbook = new Workbook();*/

            ep = new ExcelPackage(new FileInfo(FileName));
            var workbook = ep.Workbook;
            
            ExcelWorksheet akWorksheet = null;
            foreach (ExcelWorksheet worksheet in workbook.Worksheets)
            {
                if (worksheet.Name == TableName)
                    akWorksheet = worksheet;
            }
            if (akWorksheet == null)
            {
                akWorksheet = workbook.Worksheets.Add(TableName);
            }

            int n = 1;
            
            foreach (DatasetConfigRow myFeld in datasetConfig.DatasetConfigRows)
            {
               
                akWorksheet.Cells[1, n].Value = myFeld.DatabaseField;
                n++;
            }

            ep.Save();
            ep.Dispose();
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
                myThread.Name = "Thread from Storage: " + myConfig.Name + " for Table: " + TableName;
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
                                Logging.LogText("Exception: ", ex, Logging.LogLevel.Error);
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


        private int zeile = 0;
            
        public bool _internal_Write()
        {

            ExcelPackage ep;
            ep = new ExcelPackage(new FileInfo(FileName));
            var workbook = ep.Workbook;


            ExcelWorksheet akWorksheet = null;
            foreach (ExcelWorksheet worksheet in workbook.Worksheets)
            {
                if (worksheet.Name == TableName)
                    akWorksheet = worksheet;
            }
            if (akWorksheet == null)
            {
                akWorksheet = workbook.Worksheets.Add(TableName);
            }


            if (zeile <= 0)
            {
                while (true)
                {
                    zeile++;
                    try
                    {
                        if (string.IsNullOrEmpty(akWorksheet.Cells[zeile, 1].Value.ToString()))
                            break;
                    }
                    catch (Exception ex)
                    {
                        break;
                    }
                }
            }
            else
                zeile++;
            //for (; zeile < akWorksheet.Cells.Rows.Count; zeile++)
            //{
            //    if (akWorksheet.Cells[0, zeile] == null)
            //        break;
            //}



            string zeilen = "";
            for (int n = 0; n < _maxAdd; n++)
            {

                IEnumerable<object> values = _intValueList[n];

                int spalte = 1;
                foreach (object akValue in values)
                {
                    if (akValue is DateTime)
                    {

                        if (_datasetConfig.DatasetConfigRows[spalte].PLCTag.LibNoDaveDataType ==
                            DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Date)
                            akWorksheet.Cells[zeile, spalte].Value = akValue;
                        else if (_datasetConfig.DatasetConfigRows[spalte].PLCTag.LibNoDaveDataType ==
                                 DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.TimeOfDay)
                            akWorksheet.Cells[zeile, spalte].Value = akValue;
                        else
                            akWorksheet.Cells[zeile, spalte].Value = akValue;
                    }
                    else if (akValue is Int16 || akValue is Int32 || akValue is Int64 || akValue is UInt16 ||
                             akValue is UInt32 || akValue is UInt64 || akValue is Byte || akValue is SByte)
                        akWorksheet.Cells[zeile, spalte].Value = akValue;
                    else if (akValue is Single)
                        akWorksheet.Cells[zeile, spalte].Value = akValue;
                    else
                        akWorksheet.Cells[zeile, spalte].Value = akValue;

                    spalte++;
                }

                zeile++;
            }

            ep.Save();
            return true;
        }

        public void Dispose()
        {
            if (myThread != null)
                myThread.Abort();
        }
               
    }
}
