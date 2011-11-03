using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSimaticDatabaseProtokollerLibrary.Common;
using DotNetSimaticDatabaseProtokollerLibrary.Databases;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.CSVFile;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.SQLite;
using DotNetSimaticDatabaseProtokollerLibrary.Protocolling.Trigger;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage;

namespace DotNetSimaticDatabaseProtokollerLibrary.Protocolling
{
    public class ProtokollerInstance : IDisposable
    {
        private SynchronizationContext context;

        private ProtokollerConfiguration akConfig;
        private Dictionary<ConnectionConfig, Object> ConnectionList = new Dictionary<ConnectionConfig, object>();
        private Dictionary<DatasetConfig, IDBInterface> DatabaseInterfaces = new Dictionary<DatasetConfig, IDBInterface>();

        private List<IDisposable> myDisposables = new List<IDisposable>();
        private Thread myReEstablishConnectionsThread;

        public event ThreadExceptionEventHandler ThreadExceptionOccured;

        public ProtokollerInstance(ProtokollerConfiguration akConfig)
        {
            this.akConfig = akConfig;           
        }

        public void Start(bool StartedAsService)
        {
            context = SynchronizationContext.Current;

            Logging.LogText("Protokoller gestartet", Logging.LogLevel.Information);
            EstablishConnections();
            OpenStoragesAndCreateTriggers(true, StartedAsService);            
        }

        public void StartTestMode()
        {
            EstablishConnections();
            OpenStoragesAndCreateTriggers(false, false);
        }

        private void ReEstablishConnectionsThreadProc()
        {
            try
            {
                while (true)
                {
                    foreach (ConnectionConfig connectionConfig in akConfig.Connections)
                    {
                        if (ConnectionList.ContainsKey(connectionConfig))
                        {
                            PLCConnection plcConn = ConnectionList[connectionConfig] as PLCConnection;
                            TCPFunctions tcpipFunc = ConnectionList[connectionConfig] as TCPFunctions;
                            if (plcConn != null && !plcConn.Connected && ((LibNoDaveConfig) connectionConfig).StayConnected)
                            {
                                try
                                {
                                    plcConn.Connect();
                                    Logging.LogText("Connection: " + connectionConfig.Name + " connected", Logging.LogLevel.Information);
                                }
                                catch (ThreadAbortException ex)
                                {
                                    throw ex;
                                }
                                catch (Exception ex)
                                {
                                    Logging.LogText("Connection: " + connectionConfig.Name, ex, Logging.LogLevel.Warning);
                                }
                            }
                        }
                    }
                    Thread.Sleep(500);
                }
            }
            catch (ThreadAbortException)
            {
            }
        }

        private void EstablishConnections()
        {
                       
            foreach (ConnectionConfig connectionConfig in akConfig.Connections)
            {
                LibNoDaveConfig plcConnConf = connectionConfig as LibNoDaveConfig;
                TCPIPConfig tcpipConnConf = connectionConfig as TCPIPConfig;

                if (plcConnConf != null)
                {
                    PLCConnection tmpConn = new PLCConnection(plcConnConf.Configuration);
                    try
                    {
                        tmpConn.Connect();
                        if (!plcConnConf.StayConnected)
                            tmpConn.Disconnect();
                    }
                    catch (Exception ex)
                    {
                        Logging.LogText("Connection: " + connectionConfig.Name, ex, Logging.LogLevel.Warning);
                    }

                    ConnectionList.Add(connectionConfig, tmpConn);
                }
                else if (tcpipConnConf != null)
                {
                    //todo: legth of tcp conn
                    //TCPFunctionsAsync tmpConn = new TCPFunctionsAsync(new SynchronizationContext(), tcpipConnConf.IPasIPAddres, tcpipConnConf.Port, !tcpipConnConf.PassiveConnection, 0);
                    
                    //tmpConn.Connect();
                    
                    //ConnectionList.Add(connectionConfig, tmpConn);
                }
            }            

            myReEstablishConnectionsThread = new Thread(new ThreadStart(ReEstablishConnectionsThreadProc)) { Name = "EstablishConnectionsThreadProc" };
            myReEstablishConnectionsThread.Start();                        

        }

        public void TestTriggers(DatasetConfig testDataset)
        {
            if (testDataset.Trigger == DatasetTriggerType.Tags_Handshake_Trigger)
            {
                EstablishConnections();

                PLCConnection conn = ConnectionList[testDataset.TriggerConnection] as PLCConnection;
                if (conn != null)
                {
                    conn.ReadValue(testDataset.TriggerReadBit);
                    conn.ReadValue(testDataset.TriggerQuittBit);
                }
            }
            
        }


        public void TestDataRead(DatasetConfig testDataset)
        {
            ReadData.ReadDataFromPLCs(testDataset.DatasetConfigRows, ConnectionList, false);
        }

        public void TestDataReadWrite(DatasetConfig testDataset)
        {
            DatabaseInterfaces[testDataset].Write(ReadData.ReadDataFromPLCs(testDataset.DatasetConfigRows, ConnectionList, false));
        }

        private void OpenStoragesAndCreateTriggers(bool CreateTriggers, bool StartedAsService)
        {
            foreach (DatasetConfig datasetConfig in akConfig.Datasets)
            {
                IDBInterface akDBInterface = null;

                akDBInterface = StorageHelper.GetStorage(datasetConfig);
                akDBInterface.ThreadExceptionOccured += new ThreadExceptionEventHandler(tmpTrigger_ThreadExceptionOccured);
                
                DatabaseInterfaces.Add(datasetConfig, akDBInterface);

                akDBInterface.Connect_To_Database(datasetConfig.Storage);
                akDBInterface.CreateOrModify_TablesAndFields(datasetConfig.Name, datasetConfig);

                if (CreateTriggers)
                    if (datasetConfig.Trigger == DatasetTriggerType.Tags_Handshake_Trigger)
                    {
                        PLCTagTriggerThread tmpTrigger = new PLCTagTriggerThread(akDBInterface, datasetConfig, ConnectionList, StartedAsService);
                        tmpTrigger.StartTrigger();
                        tmpTrigger.ThreadExceptionOccured += tmpTrigger_ThreadExceptionOccured;
                        myDisposables.Add(tmpTrigger);
                    }
                    else if (datasetConfig.Trigger == DatasetTriggerType.Time_Trigger)
                    {
                        TimeTriggerThread tmpTrigger = new TimeTriggerThread(akDBInterface, datasetConfig, ConnectionList, StartedAsService);
                        tmpTrigger.StartTrigger();
                        tmpTrigger.ThreadExceptionOccured += tmpTrigger_ThreadExceptionOccured;
                        myDisposables.Add(tmpTrigger);
                    }
                    else if (datasetConfig.Trigger == DatasetTriggerType.Quartz_Trigger)
                    {
                        QuartzTriggerThread tmpTrigger = new QuartzTriggerThread(akDBInterface, datasetConfig, ConnectionList, StartedAsService);
                        tmpTrigger.StartTrigger();
                        tmpTrigger.ThreadExceptionOccured += tmpTrigger_ThreadExceptionOccured;
                        myDisposables.Add(tmpTrigger);
                    }
                    else if (datasetConfig.Trigger == DatasetTriggerType.Triggered_By_Incoming_Data_On_A_TCPIP_Connection)
                    {
                        TCPIPConfig tcpipConnConf = datasetConfig.TriggerConnection as TCPIPConfig;

                        tcpipConnConf.MultiTelegramme = tcpipConnConf.MultiTelegramme <= 0 ? 1 : tcpipConnConf.MultiTelegramme;
                        
                        TCPFunctionsAsync tmpConn = new TCPFunctionsAsync(new SynchronizationContext(), tcpipConnConf.IPasIPAddress, tcpipConnConf.Port, !tcpipConnConf.PassiveConnection, ReadData.GetCountOfBytesToRead(datasetConfig.DatasetConfigRows)*tcpipConnConf.MultiTelegramme);
                        tmpConn.AsynchronousExceptionOccured += tmpTrigger_ThreadExceptionOccured;
                        tmpConn.DataRecieved += (bytes) =>
                                                             {
                                                                 IEnumerable<object> values = ReadData.ReadDataFromByteBuffer(datasetConfig.DatasetConfigRows, bytes, StartedAsService);
                                                                 if (values != null)
                                                                     akDBInterface.Write(values);
                                                             };
                        
                        tmpConn.Connect();
                        ConnectionList.Add(tcpipConnConf, tmpConn);
                    }
            }
        }

        void tmpTrigger_ThreadExceptionOccured(object sender, ThreadExceptionEventArgs e)
        {
            if (ThreadExceptionOccured != null)
            {
                context.Send(delegate
                                 {
                                     ThreadExceptionOccured.Invoke(sender, e);
                                 }, null);
                Dispose();
            }
            else
            {
                Logging.LogText("Exception occured! ", e.Exception, Logging.LogLevel.Error);
                //Dispose(); //Möglicherweise ein Restart hier rein??
            }
            
        }        

        public void Dispose()
        {
            Logging.LogText("Protokoller gestopt", Logging.LogLevel.Information);
            if (myReEstablishConnectionsThread != null)
                myReEstablishConnectionsThread.Abort();

            foreach (var disposable in myDisposables)
            {
                disposable.Dispose();
            }

            foreach (object conn in ConnectionList.Values)
            {
                if (conn is PLCConnection)
                    ((PLCConnection)conn).Dispose();
                else if (conn is TCPFunctions)
                    ((TCPFunctions)conn).Dispose();
            }

            foreach (IDBInterface dbInterface in DatabaseInterfaces.Values)
            {
                dbInterface.Close();
            }
        }
    }
}
