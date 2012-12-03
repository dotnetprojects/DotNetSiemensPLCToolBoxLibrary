using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using DotNetSimaticDatabaseProtokollerLibrary.Common;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage;

namespace DotNetSimaticDatabaseProtokollerLibrary.Databases.MsSQL
{
    public class MsSQLStorage : DBBaseClass, IDBViewable, IDBViewableSQL
    {
        private Action<string> _newDataCallback;
        public MsSQLStorage(Action<string> NewDataCallback)
        {
            _newDataCallback = NewDataCallback;
        }

        private MsSQLConfig myConfig;
        private IEnumerable<DatasetConfigRow> fieldList;
        private string dataTable;
        private string insertCommand = "";

        private DbConnection myDBConn;
        private DbCommand myCmd = new SqlCommand();
        private DbDataReader myReader;
        
        private string dateFieldName;

        private DatasetConfig datasetConfig;

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
                if (!string.IsNullOrEmpty(myConfig.ExtendedConnectionString)) 
                    return myConfig.ExtendedConnectionString;
                return string.Format("Data Source={0},{1};Network Library=DBMSSOCN;Initial Catalog={2};User ID={3};Password={4};", myConfig.Server, myConfig.Port, myConfig.Database,  myConfig.Username, myConfig.Password);
            }
        }

        public override void Connect_To_Database(StorageConfig config)
        {

            myConfig = config as MsSQLConfig;
            if (myConfig == null)
                throw new Exception("Database Config is NULL");

            try
            {
                myDBConn = new SqlConnection(ConnectionString);
                myDBConn.Open();
                if (myDBConn.State != System.Data.ConnectionState.Open)
                    throw new Exception("Unable to Open Database. Storage:" + config.Name);
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
        protected override void CreateOrModify_TablesAndFields(string dataTable, DatasetConfig datasetConfig)
        {
            this.datasetConfig = datasetConfig;
            this.dataTable = dataTable;
            this.fieldList = datasetConfig.DatasetConfigRows;

            List<DatasetConfigRow> createFieldList;

            //Look if Table exists, when not, create it!
            try
            {
                string sql = "SELECT * FROM " + dataTable + ";";
                myCmd.Connection = myDBConn;
                myCmd.CommandText = sql;
                myReader = myCmd.ExecuteReader();

            }
            catch (SqlException ex)
            {
                if (ex.Number == 208)
                {
                    try
                    {
                        string sql = "CREATE TABLE " + dataTable + " (id int IDENTITY(1,1)PRIMARY KEY CLUSTERED); ";

                        myCmd.CommandText = sql;
                        myCmd.ExecuteNonQuery();

                        sql = "SELECT * FROM " + dataTable + ";";
                        myCmd.CommandText = sql;
                        myReader = myCmd.ExecuteReader();
                    }
                    catch (SqlException ex_ex)
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


            //Wenn Date Time Feld gesetzt...
            dateFieldName = datasetConfig.DateTimeDatabaseField;
            createFieldList = new List<DatasetConfigRow>(fieldList);
            if (!string.IsNullOrEmpty(datasetConfig.DateTimeDatabaseField))
                createFieldList.Add(new DatasetConfigRow() {DatabaseField = dateFieldName, DatabaseFieldType = "datetime"});


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

                string sql = "ALTER TABLE " + dataTable + " ADD " + myFeld.DatabaseField + " " + myFeld.DatabaseFieldType;

                try
                {
                    myCmd.Connection = myDBConn;
                    myCmd.CommandText = sql;
                    myCmd.ExecuteNonQuery();

                }
                catch (SqlException ex)
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
            insertCommand = "INSERT INTO " + dataTable + "(" + felderliste + ") values(" + wertliste + ")";
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
                myDBConn.Close(); //Verbindung schließen!
                myDBConn.Open();
                if (myDBConn.State != System.Data.ConnectionState.Open)
                {
                    Logging.LogText("Error ReConnecting to Database! Dataset:" + datasetConfig.Name, Logging.LogLevel.Error);
                    return false;
                }
            }

            try
            {
                DbTransaction dbTrans = null;

                if (myConfig.CombineMultipleInsertsInATransaction)
                    dbTrans = myDBConn.BeginTransaction();
                {
                    using (DbCommand cmd = myDBConn.CreateCommand())
                    {
                        cmd.CommandText = insertCommand;

                        if (myConfig.CombineMultipleInsertsInATransaction)
                            cmd.Transaction = dbTrans;
                        
                        for (int n = 0; n < _maxAdd; n++)                            
                        {
                            cmd.Parameters.Clear();

                            IEnumerable<object> values = _intValueList[n];
                            var addDateTime = _intDateTimesList[n];

                            if (!string.IsNullOrEmpty(dateFieldName))
                                cmd.Parameters.Add(new SqlParameter("@" + dateFieldName, System.Data.DbType.String) { Value = addDateTime });

                            using (IEnumerator<DatasetConfigRow> e1 = fieldList.GetEnumerator())
                            using (IEnumerator<object> e2 = values.GetEnumerator())
                            {
                                while (e1.MoveNext() && e2.MoveNext())
                                {
                                    DatasetConfigRow field = e1.Current;
                                    Object value = e2.Current;
                                    if (field.DatabaseFieldType == "text" || field.DatabaseFieldType == "varchar" || field.DatabaseFieldType == "ntext" || field.DatabaseFieldType == "nvarchar" || field.DatabaseFieldType == "char" || field.DatabaseFieldType == "nchar")
                                        cmd.Parameters.Add(new SqlParameter() { ParameterName = "@" + field.DatabaseField, Value = value.ToString() });
                                    else
                                    {
                                        if (value is System.Single && field.DatabaseFieldType == "float")
                                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@" + field.DatabaseField, Value = Convert.ToDouble(value) });
                                        else
                                            cmd.Parameters.Add(new SqlParameter() { ParameterName = "@" + field.DatabaseField, Value = value });
                                    }
                                }
                            }
                            cmd.ExecuteNonQuery();
                        }

                       
                    }

                    if (myConfig.CombineMultipleInsertsInATransaction && dbTrans != null)
                        dbTrans.Commit();
                }


                //Ringpufferarchiv...
                if (datasetConfig.MaxDatasets > 0)
                {
                    using (DbCommand cmd = myDBConn.CreateCommand())
                    {
                        string delstr = "DELETE FROM " + dataTable + " WHERE id <= (SELECT max(id) FROM " + dataTable + ") - (" + datasetConfig.MaxDatasets.ToString() + ")";
                        cmd.CommandText = delstr;
                        cmd.ExecuteNonQuery();
                    }
                }


                if (dbTrans != null)
                {
                    dbTrans.Dispose();
                }
            }            
            catch (Exception ex)
            {
                throw;
            }

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
        private DbCommand readCmd = new SqlCommand();

        private void CheckAndEstablishReadConnection()
        {
            if (readDBConn == null)
            {
                readDBConn = new SqlConnection(ConnectionString);
                readDBConn.Open();
            }
        }

        public DataTable ReadData(DatasetConfig datasetConfig, string filter, long Start, int Count, DateTime? FromDate, DateTime? ToDate)
        {
            //try
            //{
            CheckAndEstablishReadConnection();

            readCmd.Connection = readDBConn;

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

            if (FromDate!=null)
            {
                if (string.IsNullOrEmpty(where)) where = " WHERE ";
                else where += " AND ";

                where += datasetConfig.DateTimeDatabaseField;
                where += " >= '" + FromDate.Value.ToString("yyyyMMdd HH:mm:ss.mmm") + "'";
            }

            if (ToDate != null)
            {
                if (string.IsNullOrEmpty(where)) where = " WHERE ";
                else where += " AND ";

                where += datasetConfig.DateTimeDatabaseField;
                where += " <= '" + ToDate.Value.ToString("yyyyMMdd HH:mm:ss.mmm") + "'";
            }

            readCmd.CommandText = "WITH " + datasetConfig.Name + "_withRowNumber AS ( SELECT *, ROW_NUMBER() OVER (ORDER BY id DESC) AS 'RowNumber' FROM " + datasetConfig.Name + where + " ) SELECT * FROM " + datasetConfig.Name + "_withRowNumber WHERE RowNumber BETWEEN " + Start.ToString() + " AND " + (Start + Count).ToString() + ";";
            //readCmd.CommandText = "SELECT *, ROW_NUMBER() OVER(ORDER BY id DESC) AS [RowNum] FROM " + datasetConfig.Name + " WHERE RowNum BETWEEN " + Start.ToString() + " AND " + (Start + Count).ToString();
            //readCmd.CommandText = "SELECT * FROM " + datasetConfig.Name + " ORDER BY id DESC LIMIT " + Count.ToString() + " OFFSET " + Start.ToString();
            DbDataReader akReader = readCmd.ExecuteReader();

            DataTable myTbl = new DataTable();
            myTbl.Load(akReader);
            akReader.Close();

            return myTbl;  
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
                //if (!readCmd.CommandText.Contains("ORDER BY"))
                //    readCmd.CommandText += " ORDER BY id DESC";
                //if (!readCmd.CommandText.Contains("LIMIT") && !readCmd.CommandText.Contains("OFFSET"))
                //    readCmd.CommandText += " LIMIT " + Count.ToString();
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
