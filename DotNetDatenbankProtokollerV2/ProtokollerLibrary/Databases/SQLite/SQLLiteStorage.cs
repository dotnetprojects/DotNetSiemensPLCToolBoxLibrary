using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading;

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


namespace DotNetSimaticDatabaseProtokollerLibrary.Databases.SQLite
{
    public class SQLLiteStorage : DBBaseClass, IDBViewable, IDBViewableSQL
    {
         private Action<string> _newDataCallback;
         public SQLLiteStorage(Action<string> NewDataCallback)
        {
            _newDataCallback = NewDataCallback;
        }

        private SQLiteConfig myConfig;
        private IEnumerable<DatasetConfigRow> fieldList;
        private string dataTable;
        private string insertCommand = "";

        private DbConnection myDBConn;
        private DbCommand myCmd = new SQLiteCommand();
        private DbDataReader myReader;

        private static Dictionary<string, object> FileNameLockObjects = new Dictionary<string, object>();
        
        public override void Close()
        {
            if (myThread != null)
                myThread.Abort();
            if (myDBConn != null)
                myDBConn.Close();
        }

        private string ConnectionString
        {
            get
            {
                //return myConfig.DatabaseFile;
                return string.Format("Data Source={0};Pooling=true;FailIfMissing=false;journal_mode=WAL;synchronous=0;", myConfig.DatabaseFile);
            }
        }

        public override void Connect_To_Database(StorageConfig config)
        {
            
            myConfig = config as SQLiteConfig;
            if (myConfig == null)
                throw new Exception("Database Config is NULL");

            if (!FileNameLockObjects.ContainsKey(myConfig.DatabaseFile))
                FileNameLockObjects.Add(myConfig.DatabaseFile,new object());

            try
            {
                myDBConn = new SQLiteConnection(ConnectionString);
                myDBConn.Open();
                if (myDBConn.State != System.Data.ConnectionState.Open)
                    throw new Exception("Unable to Open Database. Storage:" + config.Name);
            }
            catch (SQLiteException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string dateFieldName;

        public override void CreateOrModify_TablesAndFields(string dataTable, DatasetConfig datasetConfig)
        {
            this.datasetConfig = datasetConfig;
            if (!string.IsNullOrEmpty(datasetConfig.DatasetTableName)) //Add the posibility to use a specific table_name (for using the table more then ones)
                this.dataTable = datasetConfig.DatasetTableName;
            else
                this.dataTable = dataTable;
            this.fieldList = datasetConfig.DatasetConfigRows;

            List<DatasetConfigRow> createFieldList;

            lock (FileNameLockObjects[myConfig.DatabaseFile])
            {
                //Look if Table exists, when not, create it!
                try
                {
                    string sql = "SELECT * FROM " + dataTable + ";";
                    myCmd.Connection = myDBConn;
                    myCmd.CommandText = sql;
                    myReader = myCmd.ExecuteReader();

                }
                catch (SQLiteException ex)
                {
                    if (ex.ResultCode == SQLiteErrorCode.Error)
                    {
                        try
                        {
                            string sql = "CREATE TABLE " + dataTable + " (id INTEGER PRIMARY KEY ASC AUTOINCREMENT); ";

                            myCmd.CommandText = sql;
                            myCmd.ExecuteNonQuery();

                            sql = "SELECT * FROM " + dataTable + ";";
                            myCmd.CommandText = sql;
                            myReader = myCmd.ExecuteReader();
                        }
                        catch (SQLiteException ex_ex)
                        {
                            throw ex_ex;
                        }
                    }
                    else
                    {
                        throw ex;
                    }
                }

                //Look for the Fields, create or alter them!
                List<String> existDBFelderliste = new List<string>();

                for (int n = 0; n < myReader.FieldCount; n++)
                {
                    existDBFelderliste.Add(myReader.GetName(n));
                }
                myReader.Close();
                myReader.Dispose();
                myCmd.Dispose();

                //Wenn Date Time Feld gesetzt...
                dateFieldName = datasetConfig.DateTimeDatabaseField;
                createFieldList = new List<DatasetConfigRow>(fieldList);
                if (!string.IsNullOrEmpty(datasetConfig.DateTimeDatabaseField))
                    createFieldList.Add(new DatasetConfigRow() {DatabaseField = dateFieldName, DatabaseFieldType = "TEXT"});


                foreach (DatasetConfigRow myFeld in createFieldList)
                {
                    foreach (string existMyFeld in existDBFelderliste)
                    {
                        if (myFeld.DatabaseField.ToLower() == existMyFeld.ToLower())
                        {
                            goto nextFeld;
                        }
                    }

                    //Feld existiert nicht -> erzeugen

                    string sql = "ALTER TABLE " + dataTable + " ADD COLUMN " + myFeld.DatabaseField + " " + myFeld.DatabaseFieldType;

                    try
                    {
                        myCmd = new SQLiteCommand();
                        myCmd.Connection = myDBConn;
                        myCmd.CommandText = sql;
                        myCmd.ExecuteNonQuery();
                        myCmd.Dispose();

                    }
                    catch (SQLiteException ex)
                    {
                        throw ex;
                    }

                    nextFeld:
                    //Irgendeine anweisung, da sonst der Sprung nicht geht...
                    {
                    }
                }
            }

            //Create Insert Command
            string wertliste = "", felderliste = "";
            foreach (DatasetConfigRow myFeld in createFieldList)
            {
                if (wertliste != "")
                {
                    wertliste += ",";
                    felderliste += ",";
                }

                felderliste += myFeld.DatabaseField;
                wertliste += "@" + myFeld.DatabaseField;
            }
            insertCommand = "INSERT INTO " + this.dataTable + "(" + felderliste + ") values(" + wertliste + ")";
        }

        protected override bool _internal_Write()
        {
            lock (FileNameLockObjects[myConfig.DatabaseFile])
            {
                myCmd = new SQLiteCommand();

                //Look if the Connection is still open..
                try
                {
                    string sql = "SELECT id FROM " + dataTable + " WHERE id = 0";
                    myCmd.Connection = myDBConn;
                    myCmd.CommandText = sql;
                    myCmd.ExecuteNonQuery();
                }

                catch (Exception)
                {
                    myDBConn.Close(); //Verbindung schließen!
                    myDBConn.Open();
                    if (myDBConn.State != System.Data.ConnectionState.Open)
                    {
                        Logging.LogText("Error ReConnecting to Database! Dataset:" + datasetConfig.Name, Logging.LogLevel.Error);
                        return false;
                    }
                }

                //Add the Fields to the Database
                myCmd.Connection = myDBConn;
                myCmd.CommandText = insertCommand;
                myCmd.Dispose();

                int tryCounter = 0;
                nomol:
                try
                {

                    using (DbTransaction dbTrans = myDBConn.BeginTransaction())
                    {
                        using (DbCommand cmd = myDBConn.CreateCommand())
                        {
                            cmd.CommandText = insertCommand;
                            for (int n = 0; n < _maxAdd; n++)
                                //foreach (IEnumerable<object> values in _intValueList)
                            {
                                cmd.Parameters.Clear();

                                IEnumerable<object> values = _intValueList[n];
                                var addDateTime = _intDateTimesList[n];

                                if (!string.IsNullOrEmpty(dateFieldName))
                                    cmd.Parameters.Add(new SQLiteParameter("@" + dateFieldName, System.Data.DbType.String) { Value = addDateTime.ToString("yyyy.MM.dd - HH:mm:ss.fff") });

                                using (IEnumerator<DatasetConfigRow> e1 = fieldList.GetEnumerator())
                                using (IEnumerator<object> e2 = values.GetEnumerator())
                                {
                                    while (e1.MoveNext() && e2.MoveNext())
                                    {
                                        //foreach (DatasetConfigRow field in fieldList)
                                        //{
                                        DatasetConfigRow field = e1.Current;
                                        Object value = e2.Current; //values[fnr++];

                                        switch (field.PLCTag.TagDataType)
                                        {
                                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.LInt:
                                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.LWord:
                                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Int:
                                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Dint:
                                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Word:
                                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Dword:
                                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Byte:
                                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.BCDByte:
                                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.BCDWord:
                                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.BCDDWord:
                                                cmd.Parameters.Add(new SQLiteParameter("@" + field.DatabaseField, System.Data.DbType.String) {Value = value.ToString()});
                                                break;
                                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Float:
                                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.LReal:
                                                cmd.Parameters.Add(new SQLiteParameter("@" + field.DatabaseField, System.Data.DbType.String) {Value = value.ToString().Replace(',', '.')});
                                                break;
                                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.DateTime:
                                                cmd.Parameters.Add(new SQLiteParameter("@" + field.DatabaseField, System.Data.DbType.String) {Value = ((DateTime) value).ToString("yyyy-MM-dd HH:mm:ss.fffffff")});
                                                break;
                                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.String:
                                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.CharArray:
                                                cmd.Parameters.Add(new SQLiteParameter("@" + field.DatabaseField, System.Data.DbType.String) {Value = (String) value});
                                                break;
                                        }
                                    }
                                }
                                cmd.ExecuteNonQuery();
                            }

                            //Ringpufferarchiv...
                            if (datasetConfig.MaxDatasets > 0)
                            {
                                string delstr = "DELETE FROM " + dataTable + " WHERE id <= (SELECT max(id) FROM " + dataTable + ") - (" + datasetConfig.MaxDatasets.ToString() + ")";
                                cmd.CommandText = delstr;
                                cmd.ExecuteNonQuery();
                            }
                        }

                        dbTrans.Commit();                        
                    }
                }
                catch (SQLiteException ex)
                {
                    if (ex.ResultCode == SQLiteErrorCode.Busy || ex.ResultCode == SQLiteErrorCode.Locked) //Locked || Busy
                    {
                        tryCounter++;
                        if (tryCounter > 20)
                            throw new Exception("SQLLite-Datenbank nach 20 Versuchen immer noch locked oder busy!!");
                        goto nomol;
                    }
                    else
                    {
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (_newDataCallback != null)
                    _newDataCallback(datasetConfig.Name);               

                return true;
            }
        }

        public override void Dispose()
        {
            if (myThread != null)
                myThread.Abort();
            if (myDBConn != null)
                myDBConn.Dispose();
            if (readDBConn != null)
                readDBConn.Dispose();
        }

        #region IDBViewable
        private DbConnection readDBConn;
        private DbCommand readCmd = new SQLiteCommand();

        private void CheckAndEstablishReadConnection()
        {
            if (readDBConn == null)
            {
                readDBConn = new SQLiteConnection(ConnectionString);
                readDBConn.Open();
            } 
        }

        public DataTable ReadData(DatasetConfig datasetConfig, string filter, long Start, int Count, DateTime? Fromdate, DateTime? ToDate)
        {
            //try
            //{
            CheckAndEstablishReadConnection();

            var dbFieldNames = "*";
            if (!string.IsNullOrEmpty(datasetConfig.DateTimeDatabaseField))
            {
                /*dbFieldNames = datasetConfig.DateTimeDatabaseField;
                foreach (var datasetConfigRow in datasetConfig.DatasetConfigRows)
                {
                    if (datasetConfigRow.DatabaseField.ToLower().Trim() != datasetConfig.DateTimeDatabaseField.ToLower().Trim())
                        dbFieldNames += ", " + datasetConfigRow.DatabaseField;
                }*/
                dbFieldNames = datasetConfig.DateTimeDatabaseField + ",*";
            }

            string where = "";
            if (!string.IsNullOrEmpty(filter))
            {
                where = " WHERE ";
                bool first = true;
                foreach (var rows in datasetConfig.DatasetConfigRows)
                {
                    if (!first) where += "OR ";
                    where += rows.DatabaseField + " LIKE '%" + filter + "%' ";
                    first = false;
                }               
            }

            readCmd.Connection = readDBConn;
            readCmd.CommandText = "SELECT " + dbFieldNames + " FROM " + datasetConfig.Name + where + " ORDER BY id DESC LIMIT " + Count.ToString() + " OFFSET " + Start.ToString();
            DbDataReader akReader = readCmd.ExecuteReader();

            DataTable myTbl = new DataTable();
            myTbl.Load(akReader);
            akReader.Close();

            return myTbl;
            //}
            //catch (Exception ex)
            //{ }
            //return null;
        }

        public DataTable ReadData(DatasetConfig datasetConfig, string sql, int Count)
        {
            //try
            {
                CheckAndEstablishReadConnection();

                readCmd.Connection = readDBConn;
                readCmd.CommandText = sql.Trim();
                if (readCmd.CommandText.EndsWith(";"))
                    readCmd.CommandText = readCmd.CommandText.Substring(0, readCmd.CommandText.Length - 1);
                if (!readCmd.CommandText.Contains("ORDER BY") && !readCmd.CommandText.Contains("LIMIT") && !readCmd.CommandText.Contains("OFFSET"))
                    readCmd.CommandText += " ORDER BY id DESC";                                
                if (!readCmd.CommandText.Contains("LIMIT") && !readCmd.CommandText.Contains("OFFSET"))
                    readCmd.CommandText += " LIMIT " + Count.ToString();                
                DbDataReader akReader = readCmd.ExecuteReader();

                DataTable myTbl = new DataTable();
                myTbl.Load(akReader);
                akReader.Close();

                return myTbl;
            }
            //catch (Exception ex)
            { }
            return null;
        }

        public Int64 ReadCount(DatasetConfig datasetConfig)
        {
            try
            {
                CheckAndEstablishReadConnection();

                readCmd.Connection = readDBConn;
                readCmd.CommandText = "SELECT COUNT(*) FROM " + datasetConfig.Name;

                return Convert.ToInt64(readCmd.ExecuteScalar());
            }
            catch (Exception ex)
            { }
            return 0;
        }

        #endregion
    }
}
