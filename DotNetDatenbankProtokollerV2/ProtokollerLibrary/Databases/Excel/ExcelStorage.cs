using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using DotNetSimaticDatabaseProtokollerLibrary.Common;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage;
using ExcelLibrary.SpreadSheet;
using LumenWorks.Framework.IO.Csv;
using QiHe.CodeLib;

namespace DotNetSimaticDatabaseProtokollerLibrary.Databases.Excel
{
    public class ExcelStorage : DBBaseClass, IDBViewable
    {
        private Action<string> _newDataCallback;
        public ExcelStorage(Action<string> NewDataCallback)
        {
            _newDataCallback = NewDataCallback;
        }

        private ExcelConfig myConfig;
        //private IEnumerable<DatasetConfigRow> fieldList;
        private string TableName;
        
        public override void Close()
        {
            if (myThread != null)
                myThread.Abort();            
        }

        public override void Connect_To_Database(StorageConfig config)
        {                        
            myConfig = config as ExcelConfig;            
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
            Workbook workbook;
            if (File.Exists(filename))
                workbook = Workbook.Load(filename);
            else
                workbook = new Workbook();

            Worksheet akWorksheet = null;
            foreach (Worksheet worksheet in workbook.Worksheets)
            {
                if (worksheet.Name == TableName)
                    akWorksheet = worksheet;
            }
            if (akWorksheet == null)
            {
                akWorksheet = new Worksheet(TableName);
                workbook.Worksheets.Add(akWorksheet);
            }

            int n = 0;
            foreach (DatasetConfigRow myFeld in datasetConfig.DatasetConfigRows)
            {
                akWorksheet.Cells[0, n] = new Cell(myFeld.DatabaseField);
                n++;
            }

            workbook.Save(filename);
        }

        protected override bool _internal_Write()
        {
            var filename = myConfig.ParseFileName();

            Workbook workbook;
            if (File.Exists(filename))
                workbook = Workbook.Load(filename);
            else
                workbook = new Workbook();

            Worksheet akWorksheet = null;
            foreach (Worksheet worksheet in workbook.Worksheets)
            {
                if (worksheet.Name == TableName)
                    akWorksheet = worksheet;
            }
            if (akWorksheet == null)
            {
                akWorksheet = new Worksheet(TableName);
                workbook.Worksheets.Add(akWorksheet);
            }

            int zeile = akWorksheet.Cells.Rows.Count;
            //for (; zeile < akWorksheet.Cells.Rows.Count; zeile++)
            //{
            //    if (akWorksheet.Cells[0, zeile] == null)
            //        break;
            //}



            string zeilen = "";
            for (int n = 0; n < _maxAdd; n++)
            {

                IEnumerable<object> values = _intValueList[n];

                int spalte = 0;
                foreach (object akValue in values)
                {
                    if (akValue is DateTime)
                    {

                        if (datasetConfig.DatasetConfigRows[spalte].PLCTag.LibNoDaveDataType ==
                            DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Date)
                            akWorksheet.Cells[zeile, spalte] = new Cell(akValue, @"YYYY\-MM\-DD");
                        else if (datasetConfig.DatasetConfigRows[spalte].PLCTag.LibNoDaveDataType ==
                                 DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.TimeOfDay)
                            akWorksheet.Cells[zeile, spalte] = new Cell(akValue, @"hh:mm:ss");
                        else
                            akWorksheet.Cells[zeile, spalte] = new Cell(akValue, @"YYYY\-MM\-DD·hh:mm:ss");
                    }
                    else if (akValue is Int16 || akValue is Int32 || akValue is Int64 || akValue is UInt16 ||
                             akValue is UInt32 || akValue is UInt64 || akValue is Byte || akValue is SByte)
                        akWorksheet.Cells[zeile, spalte] = new Cell(akValue);
                    else if (akValue is Single)
                        akWorksheet.Cells[zeile, spalte] = new Cell(((Single) akValue).ToString(), @"0,00E+00");
                    else
                        akWorksheet.Cells[zeile, spalte] = new Cell(akValue.ToString());

                    spalte++;
                }

                zeile++;
            }

            workbook.Save(filename);

            return true;
        }

        public override void Dispose()
        {
            if (myThread != null)
                myThread.Abort();
        }

        
        #region IDBViewable

        public DataTable ReadData(DatasetConfig datasetConfig, string filter, long Start, int Count, DateTime? Fromdate, DateTime? ToDate)
        {
            try
            {
                Workbook workbook = Workbook.Load(myConfig.ParseFileName());

                Worksheet akWorksheet = null;
                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    if (worksheet.Name == datasetConfig.Name)
                        akWorksheet = worksheet;
                }

                if (akWorksheet != null)
                {
                    int fieldCount = akWorksheet.Cells.LastColIndex;


                    DataTable tbl = new DataTable();
                    int n = 0;
                    
                    for (int i = 0; i <= fieldCount; i++)
                        tbl.Columns.Add(akWorksheet.Cells[0, i].StringValue);


                    for (int j =(int) Start+1; j < Start + Count; j++)
                    {
                        if (j < akWorksheet.Cells.Rows.Count)
                        {
                            string[] values = new string[fieldCount+1];
                            for (int i = 0; i <= fieldCount; i++)
                                values[i] = akWorksheet.Cells[j, i].StringValue;
                            tbl.Rows.Add(values);
                        }
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
            if (File.Exists(myConfig.ParseFileName()))
            {
                Workbook workbook = Workbook.Load(myConfig.ParseFileName());

                Worksheet akWorksheet = null;
                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    if (worksheet.Name == datasetConfig.Name)
                        akWorksheet = worksheet;
                }
                if (akWorksheet != null)
                {
                    return akWorksheet.Cells.Rows.Count;
                }

            }
            return 0;
        }
        #endregion         
    }
}
