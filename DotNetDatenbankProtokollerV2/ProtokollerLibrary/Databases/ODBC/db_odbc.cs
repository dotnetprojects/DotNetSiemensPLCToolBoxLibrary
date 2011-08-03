using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ProtokollerDatenbankNS.DB_specific
{
    /*
    class db_odbc : IDbInterface
    {
        private string sql;

        public System.Data.Odbc.OdbcConnection myDBConn;
        private System.Data.Odbc.OdbcCommand myCmd;
        private System.Data.Odbc.OdbcDataReader myReader;

        private ProtokollDaten myProtokollDaten;
        private DBConnections myDBConnection;

        public void Close()
        {
            myDBConn.Close();
        }


        public bool Connect_To_Database(List<DbFeld> DBFelder, ProtokollDaten parProtokollDaten, DBConnections parDBConnection)
        {
            this.myProtokollDaten = parProtokollDaten;
            this.myDBConnection = parDBConnection;

            //Datenbankverbindung aufbauen
            try
            {
                myDBConn = new System.Data.Odbc.OdbcConnection(myDBConnection.ConnectionString);
                myDBConn.Open();
                if (myDBConn.State != System.Data.ConnectionState.Open)
                    throw new Exception();
            }
            catch (System.Data.SQLite.SQLiteException ex)
            {
                System.Diagnostics.EventLog.WriteEntry(ProtkollerDatenbank.MyServiceName, ProtkollerDatenbank.MyServiceName + ": Error establishing connection to the Database :  " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                return false;
            }

            //Schauen ob Tabelle schon existiert, wenn nicht erzeugen, danach existierende Felder auslesen und nichtexistente erzeugen!
            try
            {
                sql = "SELECT * FROM " + myProtokollDaten.DataBaseTable + ";";
                myCmd = new System.Data.Odbc.OdbcCommand(sql, myDBConn);
                myReader = myCmd.ExecuteReader();

            }
            catch (System.Data.SQLite.SQLiteException ex)
            {
                if (ex.ErrorCode == System.Data.SQLite.SQLiteErrorCode.Error)
                {
                    try
                    {
                        sql = "CREATE TABLE " + myProtokollDaten.DataBaseTable + " (";
                        sql += "id INTEGER PRIMARY KEY ASC AUTOINCREMENT); ";

                        myCmd = new System.Data.Odbc.OdbcCommand(sql, myDBConn);
                        myCmd.ExecuteNonQuery();

                        sql = "SELECT * FROM " + myProtokollDaten.DataBaseTable + ";";
                        myCmd = new System.Data.Odbc.OdbcCommand(sql, myDBConn);
                        myReader = myCmd.ExecuteReader();
                    }
                    catch (System.Data.SQLite.SQLiteException ex_ex)
                    {
                        System.Diagnostics.EventLog.WriteEntry(ProtkollerDatenbank.MyServiceName, ProtkollerDatenbank.MyServiceName + ": Error creating the Database Table: " + ex_ex.Message, System.Diagnostics.EventLogEntryType.Error);
                        return false;
                    }
                }
                else
                {
                    System.Diagnostics.EventLog.WriteEntry(ProtkollerDatenbank.MyServiceName, ProtkollerDatenbank.MyServiceName + ": Error accessing the table : " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                    return false;
                }
            }



            //Liste der Felder aus Reader auslesen
            List<String> existDBFelderliste = new List<string>();

            for (int n = 0; n < myReader.FieldCount; n++)
            {
                existDBFelderliste.Add(myReader.GetName(n));
            }

            myReader.Close();

            //Schauen ob Felder existieren, wenn nicht, nicht existente Felder erzeugen.
            foreach (DbFeld myFeld in DBFelder)
            {
                foreach (string existMyFeld in existDBFelderliste)
                {
                    if (myFeld.Feld.ToLower() == existMyFeld.ToLower())
                    {
                        goto nextFeld;
                    }
                }

                //Feld existiert nicht -> erzeugen

                sql = "ALTER TABLE " + myProtokollDaten.DataBaseTable + " ADD COLUMN " + myFeld.Feld + " ";

                switch (myFeld.Typ)
                {
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
                }

                try
                {
                    myCmd = new System.Data.Odbc.OdbcCommand(sql, myDBConn);
                    myCmd.ExecuteNonQuery();

                }
                catch (System.Data.SQLite.SQLiteException ex)
                {
                    System.Diagnostics.EventLog.WriteEntry(ProtkollerDatenbank.MyServiceName, ProtkollerDatenbank.MyServiceName + ": Error creating Fields in the Database Table: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                    return false;
                }

            nextFeld:
                //Irgendeine anweisung, da sonst der Sprung nicht geht...
                { }
            }

            return true;

        }

        private List<List<DbFeld>> _intList = new List<List<DbFeld>>();
        private int _maxAdd = 0;

        private Thread myThread;
        public void prepare_Write_To_Database(List<DbFeld> DBFelder)
        {
            _intList.Add(DBFelder);
            if (myThread == null)
            {
                myThread = new Thread(new ThreadStart(ThreadProc));
                myThread.Name = "DB-Connection:" + myDBConnection.ConnectionType + " für Tabelle:" +
                                myDBConnection.Database;
                myThread.Start();
            }
        }
        private void ThreadProc()
        {
            while (true)
            {
                if (_intList.Count > 0)
                {
                    int max = _intList.Count;
                    try
                    {
                        _maxAdd = _intList.Count;
                        Write_To_Database(_intList);

                        //for (int n = _maxAdd - 1; n >= 0; n--)
                        //    _intList.RemoveAt(n);
                        _intList.RemoveRange(0, _maxAdd);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.EventLog.WriteEntry(ProtkollerDatenbank.MyServiceName, ProtkollerDatenbank.MyServiceName + ": Fehler beim Schreiben in die Datenbank, warte 1 Sekunde, Fehler :" + ex.Message + "\n" + _intList.Count.ToString() + " Datensätze in der internen Liste!", System.Diagnostics.EventLogEntryType.Error);
                        Thread.Sleep(1000);
                    }

                    //for (int n = 0; n < max; n++)
                    //    Write_To_Database(_intList[n]);
                    //for (int n = max - 1; n >= 0; n--)
                    //    _intList.RemoveAt(n);
                }
                else
                    Thread.Sleep(20);
            }
        }



        public bool Write_To_Database(List<List<DbFeld>> aDBFelder)
        {
            //Prüfen ob Verbindung noch besteht!
            try
            {
                sql = "SELECT id FROM " + myProtokollDaten.DataBaseTable + " WHERE id = 0";
                myCmd = new System.Data.Odbc.OdbcCommand(sql, myDBConn);
                myCmd.ExecuteNonQuery();
            }

            catch (Exception)
            {
                System.Diagnostics.EventLog.WriteEntry(ProtkollerDatenbank.MyServiceName, ProtkollerDatenbank.MyServiceName + ": Select zur Verbindungsprüfung hatte einen fehler --> neu Verbinden?", System.Diagnostics.EventLogEntryType.Error);
                myDBConn.Close(); //Verbindung schließen!
                if (myDBConn.State != System.Data.ConnectionState.Open)
                {
                    System.Diagnostics.EventLog.WriteEntry(ProtkollerDatenbank.MyServiceName, ProtkollerDatenbank.MyServiceName + ": Select zur Verbindungsprüfung hatte einen fehler --> Ja, Ich verbinde neu...", System.Diagnostics.EventLogEntryType.Error);
                    myDBConn.Open();
                    if (myDBConn.State != System.Data.ConnectionState.Open)
                    {
                        System.Diagnostics.EventLog.WriteEntry(ProtkollerDatenbank.MyServiceName, ProtkollerDatenbank.MyServiceName + ": Select zur Verbindungsprüfung hatte einen fehler --> Verbindung kann nicht aufgebaut werden!", System.Diagnostics.EventLogEntryType.Error);
                        return false;
                    }
                }
                else
                {
                    System.Diagnostics.EventLog.WriteEntry(ProtkollerDatenbank.MyServiceName, ProtkollerDatenbank.MyServiceName + ": Select zur Verbindungsprüfung hatte einen fehler --> Unbekannter fehler", System.Diagnostics.EventLogEntryType.Error);
                    return false;
                }
            }
            //Ende Verbindungsprüfung....


            for (int n = 0; n < _maxAdd; n++)
            {
                List<DbFeld> DBFelder = aDBFelder[n];

                string wertliste = "", felderliste = "";


                foreach (DbFeld myFeld in DBFelder)
                {
                    if (wertliste != "")
                    {
                        wertliste += ",";
                        felderliste += ",";
                    }

                    felderliste += myFeld.Feld;
                    wertliste += "@" + myFeld.Feld;
                }

                sql = "INSERT INTO " + myProtokollDaten.DataBaseTable + "(" + felderliste + ") values(" + wertliste + ")";

                myCmd = new System.Data.Odbc.OdbcCommand(sql, myDBConn);

                foreach (DbFeld myFeld in DBFelder)
                {                                       
                    switch (myFeld.Typ)
                    {
                        case "int":
                        case "dint":
                        case "word":
                        case "dword":
                        case "bcdbyte":
                        case "byte":
                        case "bool":
                            myCmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter("@" + myFeld.Feld,
                                                                                        System.Data.DbType.String)
                                                     {Value = myFeld.Wert.ToString()});
                            break;
                        case "real":
                            myCmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter("@" + myFeld.Feld,
                                                                                        System.Data.DbType.String)
                                                     {Value = ((String) myFeld.Wert).Replace(',', '.')});
                            break;
                        case "datetime":
                            myCmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter("@" + myFeld.Feld,
                                                                                        System.Data.DbType.String)
                                                     {Value = ((DateTime) myFeld.Wert).ToString("dd-MM-yyyy HH:mm:ss")});
                            break;
                        case "string":
                        case "stringchar":
                            myCmd.Parameters.Add(new System.Data.SQLite.SQLiteParameter("@" + myFeld.Feld,
                                                                                        System.Data.DbType.String)
                                                     {Value = (String) myFeld.Wert});
                            break;
                    }
                }

                int tryCounter = 0;

                nomol:
                try
                {                    
                    myCmd.ExecuteNonQuery();
                }
                catch (System.Data.SQLite.SQLiteException ex)
                {
                    if (ex.ErrorCode == System.Data.SQLite.SQLiteErrorCode.Locked ||
                        ex.ErrorCode == System.Data.SQLite.SQLiteErrorCode.Busy)
                    {
                        tryCounter++;
                        if (tryCounter > 20)
                            throw new Exception("Datenbank nach 20 Versuchen immer noch locked!!");
                        goto nomol;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.EventLog.WriteEntry(ProtkollerDatenbank.MyServiceName,
                                                           ProtkollerDatenbank.MyServiceName +
                                                           ": Error writing values to the database table : " +
                                                           ex.Message, System.Diagnostics.EventLogEntryType.Error);
                    return false;
                }
                if (Settings.Default.DetailedLogging)
                    System.Diagnostics.EventLog.WriteEntry(ProtkollerDatenbank.MyServiceName,
                                                           ProtkollerDatenbank.MyServiceName +
                                                           ": Werte geschrieben. SQL:" + sql,
                                                           System.Diagnostics.EventLogEntryType.Information);
            }

            //Ringpufferarchiv...
            if (myProtokollDaten.MaxDatasets > 0)
            {
                string delstr = "DELETE FROM " + myProtokollDaten.DataBaseTable + " WHERE id <= (SELECT max(id) FROM " + myProtokollDaten.DataBaseTable + ") - (" + myProtokollDaten.MaxDatasets.ToString() + ")";
                myCmd.CommandText = delstr;
                myCmd.ExecuteNonQuery();
            }
            
            return true;
        
        }
    }
     * */
}
