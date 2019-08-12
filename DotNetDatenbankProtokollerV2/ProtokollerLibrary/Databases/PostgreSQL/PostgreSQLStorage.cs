using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading;
using DotNetSimaticDatabaseProtokollerLibrary.Common;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage;
using Npgsql;
using NpgsqlTypes;

namespace DotNetSimaticDatabaseProtokollerLibrary.Databases.PostgreSQL
{
    public class PostgreSQLStorage : DBBaseClass, IDBViewable, IDBViewableSQL
    {
        private Action<string> _newDataCallback;
        public PostgreSQLStorage(Action<string> NewDataCallback)
        {
            _newDataCallback = NewDataCallback;
        }


        private PostgreSQLConfig myConfig;
        private IEnumerable<DatasetConfigRow> fieldList;
        private string dataTable;
        private string insertCommand = "";

        private DbConnection myDBConn;
        private DbCommand myCmd = new NpgsqlCommand();
        private DbDataReader myReader;
        
        
        public override void Close()
        {
            if (myThread != null)
                myThread.Abort();
            if (myDBConn != null)
                myDBConn.Close();
        }

        private string ConnectionString
        {
            get { return string.Format("server=" + myConfig.Server + ";user id=" + myConfig.Username + "; password=" + myConfig.Password + "; port=" + myConfig.Port + ";"); }
        }

        public override void Connect_To_Database(StorageConfig config)
        {
            myConfig = config as PostgreSQLConfig;
            if (myConfig == null)
                throw new Exception("Database Config is NULL");
            try
            {
                myDBConn = new Npgsql.NpgsqlConnection(ConnectionString);
                myDBConn.Open();
                if (myDBConn.State != System.Data.ConnectionState.Open)
                    throw new Exception("Unable to Open Database. Storage:" + config.Name);                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void CreateOrModify_TablesAndFields(string dataTable, DatasetConfig datasetConfig)
        {
            if (!string.IsNullOrEmpty(datasetConfig.DatasetTableName)) //Add the posibility to use a specific table_name (for using the table more then ones)
                this.dataTable = datasetConfig.DatasetTableName;
            else
                this.dataTable = dataTable;
            this.datasetConfig = datasetConfig;
            this.fieldList = datasetConfig.DatasetConfigRows;

            string sql = "";
            try
            {
                sql = "CREATE DATABASE " + myConfig.Database + ";";
                myCmd.Connection = myDBConn;
                myCmd.CommandText = sql;
                myCmd.ExecuteNonQuery();
            }
            catch (Npgsql.PostgresException ex)
            {
                if (ex.Code != "42P04")
                {
                    Logging.LogText("Database could not be created. Storage: " + myConfig.Name, ex, Logging.LogLevel.Error);
                    throw ex;
                }
            }

            myDBConn.ChangeDatabase(myConfig.Database);

            //Look if Table exists, when not, create it!
            try
            {
                sql = "SELECT * FROM " + dataTable + ";";
                myCmd.Connection = myDBConn;
                myCmd.CommandText = sql;
                myReader = myCmd.ExecuteReader();

            }
            catch (Npgsql.PostgresException ex)
            {
                if (ex.Code == "42P01")
                {
                    try
                    {
                        sql = "CREATE TABLE " + dataTable + " (";
                        sql += "\"id\" bigserial NOT NULL, ";
                        sql += "CONSTRAINT " + dataTable + "_data_pkey PRIMARY KEY (id))";

                        myCmd.CommandText = sql;
                        myCmd.ExecuteNonQuery();

                        sql = "SELECT * FROM " + dataTable + ";";                        
                        myCmd.CommandText = sql;
                        myReader = myCmd.ExecuteReader();
                    }
                    catch (Exception ex_ex)
                    {
                        Logging.LogText("Database-table could not be created. Storage: " + myConfig.Name + ", Table: " + dataTable, ex, Logging.LogLevel.Error);
                        throw ex_ex;
                    }
                }
                else
                {
                    Logging.LogText("Error accessing Table. Storage: " + myConfig.Name, ex, Logging.LogLevel.Error);
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

            foreach (DatasetConfigRow myFeld in fieldList)
            {
                foreach (string existMyFeld in existDBFelderliste)
                {
                    if (myFeld.DatabaseField.ToLower() == existMyFeld.ToLower())
                    {
                        goto nextFeld;
                    }
                }

                //Feld existiert nicht -> erzeugen
                string dbfieldtype = myFeld.DatabaseFieldType;

                switch (dbfieldtype)
                {
                    case "bigint":
                        dbfieldtype = "bigint NOT NULL default 0";
                        break;
                    case "real":
                        dbfieldtype = "real NOT NULL default 0";
                        break;
                    case "varchar":
                        dbfieldtype = "character varying(" + myFeld.DatabaseFieldSize + ") NOT NULL DEFAULT ''::character varying";
                        break;
                }

                sql = "ALTER TABLE " + dataTable + " ADD COLUMN " + myFeld.DatabaseField + " " + dbfieldtype;

                try
                {
                    myCmd.Connection = myDBConn;
                    myCmd.CommandText = sql;
                    myCmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                nextFeld:
                //Irgendeine anweisung, da sonst der Sprung nicht geht...
                {
                }
            }


            //Create Insert Command
            string wertliste = "", felderliste = "";
            foreach (DatasetConfigRow myFeld in fieldList)
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
            //Look if the Connection is still open..
            try
            {
                string sql = "SELECT id FROM " + dataTable + " WHERE id = 0";
                myCmd.Connection = myDBConn;
                myCmd.CommandText = sql;
                myCmd.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                Logging.LogTextToLog4Net("Exception in SQL:" + myCmd.CommandText, Logging.LogLevel.Error, ex);

                myDBConn.Close(); //Verbindung schließen!
                myDBConn.Open();
                if (myDBConn.State != System.Data.ConnectionState.Open)
                {
                    Logging.LogText("Error ReConnecting to Database! Dataset:" + datasetConfig.Name, Logging.LogLevel.Error);
                    return false;
                }
                myDBConn.ChangeDatabase(myConfig.Database);
            }

            //Add the Fields to the Database
            myCmd.Connection = myDBConn;
            myCmd.CommandText = insertCommand;                                

            
            for (int n = 0; n < _maxAdd; n++)
            //foreach (IEnumerable<object> values in _intValueList)
            {
                myCmd.Parameters.Clear();

                IEnumerable<object>  values = _intValueList[n];


                using (IEnumerator<DatasetConfigRow> e1 = fieldList.GetEnumerator())
                using (IEnumerator<object> e2 = values.GetEnumerator())
                {
                    while (e1.MoveNext() && e2.MoveNext())
                    {
                        //foreach (DatasetConfigRow field in fieldList)
                        //{
                        DatasetConfigRow field = e1.Current;
                        Object value = e2.Current; //values[fnr++];

                        if (value != null)
                        {
                            if (value is string)
                            {
                                value = ((string) value).Replace("\0", "");
                            }
                            var par = new NpgsqlParameter("@" + field.DatabaseField, value);
                            if (value is string)
                            {
                                par.Size = ((string) value).Length;                            
                            }
                            myCmd.Parameters.Add(par);
                        }
                        else
                        {
                            var par = new NpgsqlParameter("@" + field.DatabaseField, "");
                            par.Size = 0;
                            
                            myCmd.Parameters.Add(par);
                        }

                        //switch (field.DatabaseFieldType)
                        //{
                        //    case "int8":
                        //        par.NpgsqlDbType = NpgsqlDbType.Bigint;
                        //        //par.Size = 1;
                        //        break;
                        //    case "timestamp":
                        //        par.NpgsqlDbType = NpgsqlDbType.Timestamp;
                        //        //par.Size = 1;
                        //        break;
                        //    case "varchar":
                        //        par.NpgsqlDbType = NpgsqlDbType.Varchar;
                        //        par.Size = value != null ? value.ToString().Length : 0;
                        //        break;
                        //}


                        //Logging.LogText(string.Format("KeyValue Key:{0} Value:{1}", field.DatabaseField, value ?? "null"), Logging.LogLevel.Information);                   
                    }
                }
               
                int tryCounter = 0;

                nomol:
                try
                {
                    myCmd.ExecuteNonQuery();                   
                }
                catch (NpgsqlException ex)
                {
                    Logging.LogTextToLog4Net("Exception in SQL:" + myCmd.CommandText, Logging.LogLevel.Error, ex);

                    var sql = myCmd.CommandText;
                    foreach (NpgsqlParameter parameter in myCmd.Parameters.Cast<NpgsqlParameter>().OrderByDescending(x=>x.ParameterName))
                    {
                        sql = sql.Replace(parameter.ParameterName,
                            parameter.Value is string || parameter.Value is DateTime ? "'" + parameter.Value + "'" : parameter.Value.ToString());
                    }
                    
                    Logging.LogTextToLog4Net("Exception in SQL(filled):" + sql, Logging.LogLevel.Error);
                    
                    //if (ex.ErrorCode == "08P01")
                    //{
                    //    myCmd = new NpgsqlCommand();
                    //    myCmd.Connection = myDBConn;
                    //    myCmd.CommandText = ex.ErrorSql;
                    //    myCmd.Parameters.Clear();
                    //    myCmd.ExecuteNonQuery();
                    //}
                    //else
                    //{

                    //    Logging.LogText(
                    //        "Exception (ColumnName:" + (ex.ColumnName ?? "") + ", SQL:" + (ex.ErrorSql ?? "") +
                    //        ", Detail:" +
                    //        (ex.Detail ?? "") + ": ",
                    //        Logging.LogLevel.Error);

                    //    //using (StreamWriter outfile = new StreamWriter("c:\\error.txt", true))
                    //    //{
                    //    //    outfile.WriteLine("Exception (ColumnName:" + (ex.ColumnName ?? "") + ", Detail:" +
                    //    //                      (ex.Detail ?? "") + ", SQL:" + (ex.ErrorSql ?? "") + ": ");
                    //    //}

                    //    throw ex;
                    //}
                    throw;

                }
                catch (Exception ex)
                {
                    Logging.LogText("Exception: ", ex, Logging.LogLevel.Error);
                    throw ex;
                }
            }

            /*
            //Ringpufferarchiv...
            if (myProtokollDaten.MaxDatasets > 0)
            {
                string delstr = "DELETE FROM " + myProtokollDaten.DataBaseTable + " WHERE id <= (SELECT max(id) FROM " + myProtokollDaten.DataBaseTable + ") - (" + myProtokollDaten.MaxDatasets.ToString() + ")";
                myCmd.CommandText = delstr;
                myCmd.ExecuteNonQuery();
            }
            */
            return true;
        
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
        private DbCommand readCmd = new NpgsqlCommand();

        private void CheckAndEstablishReadConnection()
        {
            if (readDBConn == null)
            {
                readDBConn = new NpgsqlConnection(ConnectionString);
                readDBConn.Open();
                readDBConn.ChangeDatabase(myConfig.Database);
            } 
        }
        public DataTable ReadData(DatasetConfig datasetConfig, string filter, long Start, int Count, DateTime? Fromdate, DateTime? ToDate)
        {
            try
            {
                CheckAndEstablishReadConnection();

                readCmd.Connection = readDBConn;
                readCmd.CommandText = "SELECT * FROM " + datasetConfig.Name + " LIMIT " + Count.ToString() + " OFFSET " + Start.ToString();
                DbDataReader akReader = readCmd.ExecuteReader();

                DataTable myTbl = new DataTable();
                myTbl.Load(akReader);
                akReader.Close();

                return myTbl;
            }
            catch (Exception ex)
            { }
            return null;
        }

        public DataTable ReadData(DatasetConfig datasetConfig, string sql, int Count)
        {
            try
            {
                CheckAndEstablishReadConnection();

                readCmd.Connection = readDBConn;
                readCmd.CommandText = sql.Trim();
                if (readCmd.CommandText.EndsWith(";"))
                    readCmd.CommandText = readCmd.CommandText.Substring(0, readCmd.CommandText.Length - 1);
                if (!readCmd.CommandText.Contains("LIMIT") && !readCmd.CommandText.Contains("OFFSET"))
                    readCmd.CommandText += " LIMIT " + Count.ToString();
                DbDataReader akReader = readCmd.ExecuteReader();

                DataTable myTbl = new DataTable();
                myTbl.Load(akReader);
                akReader.Close();

                return myTbl;
            }
            catch (Exception ex)
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
