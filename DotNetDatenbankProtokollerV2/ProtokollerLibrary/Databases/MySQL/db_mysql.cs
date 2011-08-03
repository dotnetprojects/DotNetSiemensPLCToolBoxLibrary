using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ProtokollerDatenbankNS.DB_specific
{
    /*
    class db_mysql : IDbInterface
    {
        private string sql;

        private MySql.Data.MySqlClient.MySqlConnection myDBConn;
        private MySql.Data.MySqlClient.MySqlCommand myCmd;
        private MySql.Data.MySqlClient.MySqlDataReader myReader;

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
                myDBConn = new MySql.Data.MySqlClient.MySqlConnection("server=" + myDBConnection.Server + ";user id=" + myDBConnection.User + "; password=" + myDBConnection.Password + ";");
                myDBConn.Open();
                if (myDBConn.State != System.Data.ConnectionState.Open)
                    throw new Exception();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                System.Diagnostics.EventLog.WriteEntry(ProtkollerDatenbank.MyServiceName, ProtkollerDatenbank.MyServiceName + ": Error establishing connection to the Database :  " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                return false;
            }

            //Schauen ob Datenbank schon existiert, wenn nicht, erzeugen.
            try
            {
                sql = "CREATE DATABASE " + myDBConnection.Database + ";";
                myCmd = new MySql.Data.MySqlClient.MySqlCommand(sql, myDBConn);
                myCmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                if (ex.Number != 1007)
                {
                    System.Diagnostics.EventLog.WriteEntry(ProtkollerDatenbank.MyServiceName, ProtkollerDatenbank.MyServiceName + ": Error creating the Database : " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                    return false;
                }
            }

            myDBConn.ChangeDatabase(myDBConnection.Database);

            //Schauen ob Tabelle schon existiert, wenn nicht erzeugen, danach existierende Felder auslesen und nichtexistente erzeugen!
            try
            {
                sql = "SELECT * FROM " + myProtokollDaten.DataBaseTable + ";";
                myCmd = new MySql.Data.MySqlClient.MySqlCommand(sql, myDBConn);
                myReader = myCmd.ExecuteReader();

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                if (ex.Number == 1146)
                {
                    try
                    {
                        sql = "CREATE TABLE " + myProtokollDaten.DataBaseTable + " (";
                        sql += "id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY); ";
                        
                        myCmd = new MySql.Data.MySqlClient.MySqlCommand(sql, myDBConn);
                        myCmd.ExecuteNonQuery();

                        sql = "SELECT * FROM " + myProtokollDaten.DataBaseTable + ";";
                        myCmd = new MySql.Data.MySqlClient.MySqlCommand(sql, myDBConn);
                        myReader = myCmd.ExecuteReader();
                    }
                    catch (MySql.Data.MySqlClient.MySqlException ex_ex)
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
                    case "bool": sql += "BIGINT NOT NULL DEFAULT 0";
                        break;
                    case "real": sql += "REAL NOT NULL DEFAULT 0";
                        break;
                    case "datetime": sql += "TIMESTAMP NOT NULL";
                        break;
                    case "string":
                    case "stringchar": sql += "VARCHAR(" + myFeld.Length + ") NOT NULL DEFAULT ''";
                        break;
                }

                try
                {
                    myCmd = new MySql.Data.MySqlClient.MySqlCommand(sql, myDBConn);
                    myCmd.ExecuteNonQuery();

                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
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
                    for (int n = 0; n < max; n++)
                        Write_To_Database(_intList[n]);
                    for (int n = max - 1; n >= 0; n--)
                        _intList.RemoveAt(n);
                }
                else
                    Thread.Sleep(20);
            }
        }



        public bool Write_To_Database(List<DbFeld> DBFelder)
        {
            //Prüfen ob Verbindung noch besteht!
            try
            {
                sql = "SELECT id FROM " + myProtokollDaten.DataBaseTable + " WHERE id = 0";
                myCmd = new MySql.Data.MySqlClient.MySqlCommand(sql, myDBConn);
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

            string wertliste = "", felderliste = "";

            foreach (DbFeld myFeld in DBFelder)
            {
                if (wertliste != "")
                {
                    wertliste += ",";
                    felderliste += ",";
                }

                felderliste += myFeld.Feld;

                switch (myFeld.Typ)
                {
                    case "int":
                    case "dint":
                    case "word":
                    case "dword":
                    case "bcdbyte":
                    case "byte":
                    case "bool": wertliste += "'" + (String)myFeld.Wert + "'";
                        break;
                    case "real": wertliste += "'" + ((String)myFeld.Wert).Replace(',', '.') + "'";
                        break;
                    case "datetime": wertliste += "'" + ((DateTime)myFeld.Wert).ToString("dd-MM-yyyy HH:mm:ss") + "'";
                        break;
                    case "string":
                    case "stringchar": wertliste += "'" + (String)myFeld.Wert + "'";
                        break;
                }
            }

            sql = "INSERT INTO " + myProtokollDaten.DataBaseTable + "(" + felderliste + ") values(" + wertliste + ")";

            try
            {
                myCmd = new MySql.Data.MySqlClient.MySqlCommand(sql, myDBConn);
                myCmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry(ProtkollerDatenbank.MyServiceName, ProtkollerDatenbank.MyServiceName + ": Error writing values to the database table : " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                return false;
            }

            myReader.Close();

            return true;
        }
    }
     * */
}
