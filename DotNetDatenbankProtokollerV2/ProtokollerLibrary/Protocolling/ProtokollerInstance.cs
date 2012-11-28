using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSimaticDatabaseProtokollerLibrary.Common;
using DotNetSimaticDatabaseProtokollerLibrary.Connections;
using DotNetSimaticDatabaseProtokollerLibrary.Databases;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.CSVFile;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.Interfaces;
using DotNetSimaticDatabaseProtokollerLibrary.Databases.SQLite;
using DotNetSimaticDatabaseProtokollerLibrary.Protocolling.Trigger;
using DotNetSimaticDatabaseProtokollerLibrary.Remoting;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage;
using DotNetSimaticDatabaseProtokollerLibrary.aspx;
using DotNetSimaticDatabaseProtokollerLibrary.wcfService;

using JFKCommonLibrary.Networking;


namespace DotNetSimaticDatabaseProtokollerLibrary.Protocolling
{
    using DotNetSimaticDatabaseProtokollerLibrary.Databases.MsSQL;

    public partial class ProtokollerInstance : IDisposable
    {
        private SynchronizationContext context;

        private ProtokollerConfiguration akConfig;
        private Dictionary<ConnectionConfig, Object> ConnectionList = new Dictionary<ConnectionConfig, object>();
        private Dictionary<DatasetConfig, IDBInterface> DatabaseInterfaces = new Dictionary<DatasetConfig, IDBInterface>();

        private System.Threading.Timer synTimer;

        private List<IDisposable> myDisposables = new List<IDisposable>();
        private List<Thread> myReEstablishConnectionsThreads;

        private WebServiceHost wcfWebService;

        private AspxVirtualRoot webServer = null;

        //private Remoting.RemotingServer remotingServer;

        public event ThreadExceptionEventHandler ThreadExceptionOccured;

        public ProtokollerInstance(ProtokollerConfiguration akConfig)
        {
            this.akConfig = akConfig;           
        }

        /*public ProtokollerInstance()
        {
            ProtokollerConfiguration.Load(false);
            this.akConfig = ProtokollerConfiguration.ActualConfigInstance;            
        }*/

        public void Start(bool StartedAsService)
        {

            //Remoting Server für Benachrichtigung an Client das neue Daten da sind
            /*remotingServer = new RemotingServer();
            remotingServer.Start();*/

            if (akConfig.UseWebserver)
            {
                AspxVirtualRoot webServer = new AspxVirtualRoot(akConfig.WebserverPort);
                webServer.Configure("/", System.IO.Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "web"));
            }

            if (akConfig.UseWCFService)
            {
                var serverName = "ProtocolService";
                var addressWeb = new Uri("http://localhost:" + akConfig.WCFServicePort + "/");

                wcfWebService = new WebServiceHost(this /*.GetType()*/, new Uri[] { addressWeb });

                wcfWebService.AddServiceEndpoint(typeof(IProtocolService), new BasicHttpBinding(), serverName);
                wcfWebService.AddServiceEndpoint(typeof(IPolicyRetriever), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());

                wcfWebService.Open();
            }

            context = SynchronizationContext.Current;

            Logging.LogText("Protokoller gestartet", Logging.LogLevel.Information);
            EstablishConnections();
            OpenStoragesAndCreateTriggers(true, StartedAsService);

            synTimer = new Timer(synchronizePLCTimes, null, 0, 60000);            
        }

        public void StartTestMode()
        {
            EstablishConnections();
            OpenStoragesAndCreateTriggers(false, false);
        }

        private void ReEstablishConnectionsThreadProc(object config)
        {
            try
            {
                LibNoDaveConfig connectionConfig = config as LibNoDaveConfig;
                while (true)
                {

                    if (ConnectionList.ContainsKey(connectionConfig))
                    {
                        PLCConnection plcConn = ConnectionList[connectionConfig] as PLCConnection;
                        if (plcConn != null && !plcConn.Connected && ((LibNoDaveConfig)connectionConfig).StayConnected)
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

                    Thread.Sleep(connectionConfig.ReconnectInterval);
                }
            }
            catch (ThreadAbortException)
            {
            }
        }

        private void synchronizePLCTimes(object tmp)
        {
            foreach (ConnectionConfig connectionConfig in akConfig.Connections)
            {
                LibNoDaveConfig plcConnConf = connectionConfig as LibNoDaveConfig;
                if (plcConnConf != null)
                {
                    PLCConnection tmpConn = (PLCConnection)ConnectionList[connectionConfig];

                    if (plcConnConf.SynchronizePLCTime)
                    {
                        try
                        {
                            if (!tmpConn.Connected)
                                tmpConn.Connect();

                            tmpConn.PLCSetTime(DateTime.Now);
                            if (!plcConnConf.StayConnected)
                                tmpConn.Disconnect();
                        }
                        catch (Exception ex)
                        { }
                    }                    
                }
            }
        }

        private void EstablishConnections()
        {
                       
            foreach (ConnectionConfig connectionConfig in akConfig.Connections)
            {
                LibNoDaveConfig plcConnConf = connectionConfig as LibNoDaveConfig;
                TCPIPConfig tcpipConnConf = connectionConfig as TCPIPConfig;
                DatabaseConfig dbConnConf = connectionConfig as DatabaseConfig;


                if (plcConnConf != null)
                {
                    Logging.LogText("Connection: " + connectionConfig.Name + " is starting...", Logging.LogLevel.Information);

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
                else if (dbConnConf != null)
                {
                    var tmpConn = new DatabaseConnection(dbConnConf);
                    try
                    {
                        tmpConn.Connect();
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

            myReEstablishConnectionsThreads = new List<Thread>();
            foreach (ConnectionConfig connectionConfig in akConfig.Connections)
            {
                if (connectionConfig is LibNoDaveConfig)
                {
                    var thrd = new Thread(new ParameterizedThreadStart(ReEstablishConnectionsThreadProc)) { Name = "EstablishConnectionsThreadProc" };
                    thrd.Start(connectionConfig as LibNoDaveConfig);
                    this.myReEstablishConnectionsThreads.Add(thrd);
                }
            }
                                  

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
            ReadData.ReadDataFromDataSources(testDataset, testDataset.DatasetConfigRows, ConnectionList, false);
        }

        public void TestDataReadWrite(DatasetConfig testDataset)
        {
            DatabaseInterfaces[testDataset].Write(ReadData.ReadDataFromDataSources(testDataset, testDataset.DatasetConfigRows, ConnectionList, false));
        }

        private void OpenStoragesAndCreateTriggers(bool CreateTriggers, bool StartedAsService)
        {
            foreach (DatasetConfig datasetConfig in akConfig.Datasets)
            {
                try
                {
                    IDBInterface akDBInterface = null;

                    akDBInterface = StorageHelper.GetStorage(datasetConfig, RemotingServer.ClientComms.CallNotifyEvent);
                    akDBInterface.ThreadExceptionOccured += new ThreadExceptionEventHandler(tmpTrigger_ThreadExceptionOccured);

                    DatabaseInterfaces.Add(datasetConfig, akDBInterface);

                    Logging.LogText("DB Interface: " + datasetConfig.Name + " is starting...", Logging.LogLevel.Information);

                    akDBInterface.Initiate(datasetConfig);
                    
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
                        else if (datasetConfig.Trigger == DatasetTriggerType.Time_Trigger_With_Value_Comparison)
                        {
                            TimeTriggerWithCheckForChangesThread tmpTrigger = new TimeTriggerWithCheckForChangesThread(akDBInterface, datasetConfig, ConnectionList, StartedAsService);
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

                            if (tcpipConnConf.MultiTelegramme == 0)
                                tcpipConnConf.MultiTelegramme = 1;
                            TCPFunctionsAsync tmpConn = new TCPFunctionsAsync(null, tcpipConnConf.IPasIPAddress, tcpipConnConf.Port, !tcpipConnConf.PassiveConnection, tcpipConnConf.DontUseFixedTCPLength ? -1 : ReadData.GetCountOfBytesToRead(datasetConfig.DatasetConfigRows) * tcpipConnConf.MultiTelegramme);
                            tmpConn.AllowMultipleClients = tcpipConnConf.AcceptMultipleConnections;
                            tmpConn.UseKeepAlive = tcpipConnConf.UseTcpKeepAlive;
                            tmpConn.AsynchronousExceptionOccured += tmpTrigger_ThreadExceptionOccured;
                            tmpConn.AutoReConnect = true;
                            var conf = datasetConfig;
                            tmpConn.DataRecieved += (bytes, tcpClient) =>
                                                        {
                                                            if (tcpipConnConf.MultiTelegramme == 0)
                                                                tcpipConnConf.MultiTelegramme = 1;
                                                            for (int j = 1; j <= tcpipConnConf.MultiTelegramme; j++)
                                                            {
                                                                var ln = bytes.Length / tcpipConnConf.MultiTelegramme;
                                                                byte[] tmpArr = new byte[ln];
                                                                Array.Copy(bytes, ((j - 1) * ln), tmpArr, 0, ln);

                                                                IEnumerable<object> values = ReadData.ReadDataFromByteBuffer(conf, conf.DatasetConfigRows, tmpArr, StartedAsService);
                                                                if (values != null)
                                                                    akDBInterface.Write(values);
                                                            }
                                                        };
                            tmpConn.ConnectionEstablished += (TcpClient tcp) =>
                                                                 {
                                                                     Logging.LogText("Connection established: " + tcpipConnConf.IPasIPAddress + ", " + tcpipConnConf.Port, Logging.LogLevel.Information);
                                                                 };
                            tmpConn.ConnectionClosed += (TcpClient tcp) =>
                                                            {
                                                                Logging.LogText("Connection closed: " + tcpipConnConf.IPasIPAddress + ", " + tcpipConnConf.Port, Logging.LogLevel.Information);
                                                            };
                            Logging.LogText("Connection prepared: " + tcpipConnConf.IPasIPAddress + ", " + tcpipConnConf.Port, Logging.LogLevel.Information);
                            tmpConn.Start();
                            ConnectionList.Add(tcpipConnConf, tmpConn);
                            myDisposables.Add(tmpConn);
                        }
                }
                catch (Exception ex)
                {
                    Logging.LogText("Error in OpenStorragesAndCreateTriggers occured!", ex, Logging.LogLevel.Error);
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
            if (myReEstablishConnectionsThreads != null)
                foreach (var myReEstablishConnectionsThread in myReEstablishConnectionsThreads)
                {
                    myReEstablishConnectionsThread.Abort();
                }                

            if (webServer != null)
            {
                webServer.StopListener();
                webServer.Dispose();
                webServer = null;
            }

            if (wcfWebService != null)
            {
                wcfWebService.Close();
                wcfWebService = null;
            }

            foreach (var disposable in myDisposables)
            {
                disposable.Dispose();
            }

            foreach (object conn in ConnectionList.Values)
            {
                if (conn is PLCConnection)
                    ((PLCConnection)conn).Dispose();
                else if (conn is TCPFunctionsAsync)
                    ((TCPFunctionsAsync)conn).Dispose();
            }

            foreach (IDBInterface dbInterface in DatabaseInterfaces.Values)
            {
                dbInterface.Close();
            }

            if (synTimer != null)
                synTimer.Dispose();
        }
    }
}
