using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSimaticDatabaseProtokollerLibrary.Common;
using DotNetSimaticDatabaseProtokollerLibrary.Connections;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;

namespace DotNetSimaticDatabaseProtokollerLibrary.Protocolling
{
    static class ReadData
    {       
        public static int GetCountOfBytesToRead(IEnumerable<DatasetConfigRow> datasetConfigRows)
        {
            int cntBytes = 0;

            foreach (var itm in datasetConfigRows)
                cntBytes += itm.PLCTag.ReadByteSize;

            return cntBytes;
        }

        public static IEnumerable<object> ReadDataFromByteBuffer(DatasetConfig datasetConfig, IEnumerable<DatasetConfigRow> datasetConfigRows, byte[] bytes, bool StartedAsService)
        {
            int pos = 0;

            foreach (var itm in datasetConfigRows)
            {
                if (pos < bytes.Length)
                {
                    itm.PLCTag.ParseValueFromByteArray(bytes, pos);
                    pos += itm.PLCTag.ReadByteSize;
                }
            }

            List<object> retVal = new List<object>();
            foreach (var datasetConfigRow in datasetConfigRows)
            {
                retVal.Add(datasetConfigRow.Value(datasetConfig.UseFloatIfMultiplierIsUsed));
            }
            
            return retVal;
        }

        public static IEnumerable<object> ReadDataFromDataSources(DatasetConfig datasetConfig, IEnumerable<DatasetConfigRow> datasetConfigRows, Dictionary<ConnectionConfig, Object> activConnections, bool StartedAsService)
        {
            var usedConnections = from n in datasetConfigRows
                                  group n by n.Connection into g       //Es wird in das Object g hineingruppiert.
                                  select new { Connection = g.Key };     //n.Connection ist dann g.Key, g ist eine ganze Zeile

            foreach (var usedConnection in usedConnections)
            {
                var tags = from n in datasetConfigRows
                           where n.Connection == usedConnection.Connection
                           select n.PLCTag;
                if (usedConnection.Connection.GetType() == typeof(DatabaseConfig))
                {
                    //Read Data from a Database
                    var dbConn = (DatabaseConnection)activConnections[usedConnection.Connection];

                    var dta = dbConn.ReadData();

                    try
                    {                     
                        foreach (var plcTag in tags)
                        {
                            plcTag.Value = dta[plcTag.ValueName];
                        }
                    }
                    finally
                    {
                        dta.Close();                  
                    }
                }
                else if (usedConnection.Connection.GetType() == typeof(LibNoDaveConfig))
                {                   
                    PLCConnection plcConn = (PLCConnection)activConnections[usedConnection.Connection];
                    
                    if (!plcConn.Connected)
                        plcConn.Connect();

                    if (plcConn.Connected)
                    {
                        try
                        {
                            plcConn.ReadValues(tags);
                        }
                        catch(Exception ex)
                        {
                            if (StartedAsService)
                            {
                                Logging.LogText("Error: Exception during ReadData, maybe Connection interupted?", ex, Logging.LogLevel.Error);
                                return null;
                            }
                            
                            throw;
                        }
                        foreach (var plcTag in tags)
                        {
                            if (plcTag.ItemDoesNotExist)
                                if (StartedAsService)
                                    Logging.LogText("Tag does not Exist! " + plcConn.Configuration.ConnectionName + ": " + plcTag.S7FormatAddress, Logging.LogLevel.Error);
                                else
                                    throw new Exception("Tag does not Exist! " + plcConn.Configuration.ConnectionName + ": " + plcTag.S7FormatAddress);
                        }

                        if (usedConnection.Connection is LibNoDaveConfig)
                            if (!((LibNoDaveConfig)usedConnection.Connection).StayConnected)
                                plcConn.Disconnect();
                    
                    }
                    else
                    {
                        Logging.LogText("Error: Read Data returned \"null\" maybe a Connection is offline?", Logging.LogLevel.Error);
                        return null;
                    }
                }
            }

            List<object> retVal = new List<object>();
            foreach (var datasetConfigRow in datasetConfigRows)
            {
                retVal.Add(datasetConfigRow.Value(datasetConfig.UseFloatIfMultiplierIsUsed));
            }

            return retVal;            
        }
    }
}
