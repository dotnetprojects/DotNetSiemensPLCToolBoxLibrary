using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSimaticDatabaseProtokollerLibrary.Common;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;

namespace DotNetSimaticDatabaseProtokollerLibrary.Protocolling
{
    static class ReadData
    {               
        public static IEnumerable<object> ReadDataFromPLCs(IEnumerable<DatasetConfigRow> datasetConfigRows, Dictionary<ConnectionConfig, Object> activConnections)
        {
            var usedConnections = from n in datasetConfigRows
                                  group n by n.Connection into g       //Es wird in das Object g hineingruppiert.
                                  select new { Connection = g.Key };     //n.Connection ist dann g.Key, g ist eine ganze Zeile

            foreach (var usedConnection in usedConnections)
            {
                var tags = from n in datasetConfigRows
                           where n.Connection == usedConnection.Connection
                           select n.PLCTag;
                if (usedConnection.Connection.GetType() == typeof(LibNoDaveConfig))
                {                   
                    PLCConnection plcConn = (PLCConnection)activConnections[usedConnection.Connection];
                    if (plcConn.Connected)
                    {
                        plcConn.ReadValues(tags);
                        foreach (var plcTag in tags)
                        {
                            if (plcTag.ItemDoesNotExist)
                                throw new Exception("Tag does not Exist! " + plcConn.Configuration.ConnectionName + ": " + plcTag.S7FormatAddress);
                        }
                    }
                    else
                    {
                        Logging.LogText("Error: Read Data returned \"null\" maybe a Connection is offline?", Logging.LogLevel.Error);
                        return null;
                    }
                }
            }

            var values = from n in datasetConfigRows
                         select n.PLCTag.Value;

            return values;
        }
    }
}
