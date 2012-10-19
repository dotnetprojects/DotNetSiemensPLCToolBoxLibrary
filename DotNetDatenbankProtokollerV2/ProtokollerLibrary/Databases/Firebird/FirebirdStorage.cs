using System;
using System.Collections.Generic;
using System.Threading;
using DotNetSimaticDatabaseProtokollerLibrary.Common;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage;
using FirebirdSql.Data.FirebirdClient;

namespace DotNetSimaticDatabaseProtokollerLibrary.Databases.Firebird
{
    class FirebirdStorage : DBBaseClass
    {
        private Action<string> _newDataCallback;
        public FirebirdStorage(Action<string> NewDataCallback)
        {
            _newDataCallback = NewDataCallback;
        }


        private SQLiteConfig myConfig;
        private IEnumerable<DatasetConfigRow> fieldList;
        private string dataTable;
        private string insertCommand = "";

        private FirebirdSql.Data.FirebirdClient.FbConnection myDBConn;
        private FirebirdSql.Data.FirebirdClient.FbCommand myCmd;
        private FirebirdSql.Data.FirebirdClient.FbDataReader myReader;
        
        public override void Close()
        {
            if (myThread != null)
                myThread.Abort();
            if (myDBConn != null)
                myDBConn.Close();
        }

        public override void Connect_To_Database(StorageConfig config)
        {
            myConfig = config as SQLiteConfig;
            if (myConfig == null)
                throw new Exception("Database Config is NULL");
            try
            {
                string connString = string.Format("Data Source={0};Pooling=true;FailIfMissing=false", myConfig.DatabaseFile);
                myDBConn = new FbConnection(connString);
                myDBConn.Open();
                if (myDBConn.State != System.Data.ConnectionState.Open)
                    throw new Exception();
            }
            catch (FbException ex)
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
            this.dataTable = dataTable;
            this.datasetConfig = datasetConfig;
            this.fieldList = datasetConfig.DatasetConfigRows;

            //Look if Table exists, when not, create it!
            try
            {
                string sql = "SELECT * FROM " + dataTable + ";";
                myCmd = new FbCommand(sql, myDBConn);
                myReader = myCmd.ExecuteReader();

            }
            catch (FbException ex)
            {
                /*if (ex.ErrorCode == 0)
                {
                    try
                    {
                        string sql = "CREATE TABLE " + dataTable + " (id INTEGER PRIMARY KEY ASC AUTOINCREMENT); ";

                        myCmd = new FbCommand(sql, myDBConn);
                        myCmd.ExecuteNonQuery();

                        sql = "SELECT * FROM " + dataTable + ";";
                        myCmd = new FbCommand(sql, myDBConn);
                        myReader = myCmd.ExecuteReader();
                    }
                    catch (System.Data.SQLite.SQLiteException ex_ex)
                    {
                        throw ex_ex;
                    }
                }
                else
                {
                    throw ex;
                }*/
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

                string sql = "ALTER TABLE " + dataTable + " ADD COLUMN " + myFeld.DatabaseField + " " + myFeld.DatabaseFieldType;

                try
                {
                    myCmd = new FbCommand(sql, myDBConn);
                    myCmd.ExecuteNonQuery();

                }
                catch (FbException ex)
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
            insertCommand = "INSERT INTO " + dataTable + "(" + felderliste + ") values(" + wertliste + ")";
        }


        protected override bool _internal_Write()
        {
            //Look if the Connection is still open..
            try
            {
                string sql = "SELECT id FROM " + dataTable + " WHERE id = 0";
                myCmd = new FbCommand(sql, myDBConn);
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
                myDBConn.ChangeDatabase(myConfig.DatabaseFile);
            }

            //Add the Fields to the Database
            myCmd = new FbCommand(insertCommand, myDBConn);

            
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

                        switch (field.PLCTag.LibNoDaveDataType)
                        {
                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Int:
                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Dint:
                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Word:
                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Dword:
                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Byte:
                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.BCDByte:
                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.BCDWord:
                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.BCDDWord:
                                myCmd.Parameters.Add(new FbParameter("@" + field.DatabaseField, System.Data.DbType.String) {Value = value.ToString()});
                                break;
                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Float:
                                myCmd.Parameters.Add(new FbParameter("@" + field.DatabaseField, System.Data.DbType.String) {Value = value.ToString().Replace(',', '.')});
                                break;
                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.DateTime:
                                myCmd.Parameters.Add(new FbParameter("@" + field.DatabaseField, System.Data.DbType.String) {Value = ((DateTime) value).ToString("dd-MM-yyyy HH:mm:ss")});
                                break;
                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.String:
                            case DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.CharArray:
                                myCmd.Parameters.Add(new FbParameter("@" + field.DatabaseField, System.Data.DbType.String) {Value = (String) value});
                                break;
                        }
                    }
                }
               
                int tryCounter = 0;

                nomol:
                try
                {
                    myCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
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
        }
    }
}
