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
    public class Excel2007Storage : DBBaseClass
    {
        private Action<string> _newDataCallback;
        public Excel2007Storage(Action<string> NewDataCallback)
        {
            _newDataCallback = NewDataCallback;
        }

        private Excel2007Config myConfig;
        //private IEnumerable<DatasetConfigRow> fieldList;
        private string TableName;
        

        public override void Close()
        {
            if (myThread != null)
                myThread.Abort();            
        }

        public override void Connect_To_Database(StorageConfig config)
        {                        
            myConfig = config as Excel2007Config;            
            if (myConfig == null)
                throw new Exception("Database Config is NULL");
        }

        protected override void CreateOrModify_TablesAndFields(string dataTable, DatasetConfig datasetConfig)
        {            
            TableName = datasetConfig.Name;
        
            writeHeader(myConfig.ParseFileName());
        }

        private void writeHeader(string filename)
        {
            ExcelPackage ep;

            ep = new ExcelPackage(new FileInfo(filename));
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

        private int zeile = 0;

        protected override bool _internal_Write()
        {
            var fileName = myConfig.ParseFileName();

            ExcelPackage ep;
            ep = new ExcelPackage(new FileInfo(fileName));
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
            
            string zeilen = "";
            for (int n = 0; n < _maxAdd; n++)
            {

                IEnumerable<object> values = _intValueList[n];

                int spalte = 1;
                foreach (object akValue in values)
                {
                    if (akValue is DateTime)
                    {

                        if (datasetConfig.DatasetConfigRows[spalte].PLCTag.LibNoDaveDataType ==
                            DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Date)
                            akWorksheet.Cells[zeile, spalte].Value = akValue;
                        else if (datasetConfig.DatasetConfigRows[spalte].PLCTag.LibNoDaveDataType ==
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

        public override void Dispose()
        {
            if (myThread != null)
                myThread.Abort();
        }
               
    }
}
