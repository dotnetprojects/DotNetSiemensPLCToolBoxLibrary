using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSimaticDatabaseProtokollerLibrary.Common;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage;
using JFKCommonLibrary.Serialization;
using Microsoft.Win32;

namespace DotNetSimaticDatabaseProtokollerLibrary
{
    [Serializable()]
    [DataContract(Namespace = "")]
    public class ProtokollerConfiguration
    {
        //SaveData
        [DataMember]
        public ObservableCollection<ConnectionConfig> Connections { get; set; }
        [DataMember]        
        public ObservableCollection<StorageConfig> Storages { get; set; }
        [DataMember]        
        public ObservableCollection<DatasetConfig> Datasets { get; set; }

        #region WCF Service
        private bool useWCFService = false;
        [DataMember]
        public bool UseWCFService
        {
            get
            {
                return this.useWCFService;
            }
            set
            {
                this.useWCFService = value;
            }
        }

        private int wCFServicePort = 9998;
        [DataMember]
        public int WCFServicePort
        {
            get { return wCFServicePort; }
            set { wCFServicePort = value; }
        }

        #endregion

        #region Webserver

        private bool useWebserver = false;
        [DataMember]
        public bool UseWebserver
        {
            get
            {
                return this.useWebserver;
            }
            set
            {
                this.useWebserver = value;
            }
        }

        private int _webserverPort = 80;
        [DataMember]
        public int WebserverPort
        {
            get { return _webserverPort; }
            set { _webserverPort = value; }
        }

        [DataMember]
        public string  WebserverPath { get; set; }
        #endregion

        #region LoggingConfig
        private bool useMailInform;

        [DataMember]
        public bool UseMailInform
        {
            get { return useMailInform; }
            set { useMailInform = value; }
        }

        private string _SmtpServer;

        [DataMember]
        public string SmtpServer
        {
            get { return _SmtpServer; }
            set { _SmtpServer = value; }
        }

        private int _SmtpPort = 25;

        [DataMember]
        public int SmtpPort
        {
            get { return _SmtpPort; }
            set { _SmtpPort = value; }
        }

        public bool useCredentials;

        [DataMember]
        public bool UseCredentials
        {
            get { return this.useCredentials; }
            set { this.useCredentials = value; }
        }

        public string _SmtpUsername;
        public string _SmtpPassword;

        [DataMember]
        public string SmtpUsername
        {
            get { return _SmtpUsername; }
            set { _SmtpUsername = value; }
        }

        [DataMember]
        public string SmtpPassword
        {
            get { return _SmtpPassword; }
            set { _SmtpPassword = value; }
        }

        public string _Sender;
        public string _Recipient;
        public string _Subject;
        public int _SendInterval = 60000;

        [DataMember]
        public string Sender
        {
            get { return _Sender; }
            set { _Sender = value; }
        }

        [DataMember]
        public string Recipient
        {
            get { return _Recipient; }
            set { _Recipient = value; }
        }

        [DataMember]
        public string Subject
        {
            get { return _Subject; }
            set { _Subject = value; }
        }

        [DataMember]
        public int SendInterval
        {
            get { return _SendInterval; }
            set { _SendInterval = value; }
        }

        private Common.LogErrorLevel _errorLevel;

        [DataMember]
        public Common.LogErrorLevel ErrorLevel
        {
            get { return _errorLevel; }
            set { _errorLevel = value; }
        }

        #endregion

        /// <summary>
        /// Config has Changed, and was not Saved!
        /// </summary>
        public bool isDirty { get; set; }

        public static ProtokollerConfiguration ActualConfigInstance = new ProtokollerConfiguration();

        public static void Save()
        {
            foreach (var ds in ProtokollerConfiguration.ActualConfigInstance.Datasets)
            {
                foreach (var rw in ds.DatasetConfigRows)
                {
                    if (rw.PLCTag != null)
                        rw.PLCTag.ClearValue();
                }
            }

            string conf = SerializeToString<ProtokollerConfiguration>.Serialize(ProtokollerConfiguration.ActualConfigInstance);
            RegistryKey regKey = Registry.LocalMachine;
            regKey = regKey.CreateSubKey("SYSTEM\\CurrentControlSet\\services\\" + StaticServiceConfig.MyServiceName + "\\Parameters");
            if (regKey != null)
            {
                regKey.SetValue("XMLConfig", conf);
                ProtokollerConfiguration.ActualConfigInstance.isDirty = false;
            }
            else
                MessageBox.Show("Error writing Config to the Registry, maybe you need to Login as Administrator?");
        }

        public static void SaveToFile(string filename)
        {
            StreamWriter fstrm = new StreamWriter(filename, false);
           
            string conf = SerializeToString<ProtokollerConfiguration>.Serialize(ProtokollerConfiguration.ActualConfigInstance);
            fstrm.Write(conf);
            fstrm.Close();
        }

        private static ProtokollerConfiguration DeSerialize(string txt)
        {
            ProtokollerConfiguration retVal = SerializeToString<ProtokollerConfiguration>.DeSerialize(txt);

            ReReferenceProtokollerConfiguration(retVal);

            return retVal;
        }

        public static void ReReferenceProtokollerConfiguration(ProtokollerConfiguration cfg)
        {
            //Recreate the References, because XMLSerializer creates copies all the time!
            foreach (DatasetConfig datasetConfig in cfg.Datasets)
            {
                if (datasetConfig.Storage != null)
                    datasetConfig.Storage = cfg.Storages.Where(c => c.Name == datasetConfig.Storage.Name).FirstOrDefault();
                if (datasetConfig.TriggerConnection != null)
                    datasetConfig.TriggerConnection = cfg.Connections.Where(c => c.Name == datasetConfig.TriggerConnection.Name).FirstOrDefault();

                foreach (DatasetConfigRow datasetConfigRow in datasetConfig.DatasetConfigRows)
                {
                    if (datasetConfigRow.Connection != null)
                    {
                        datasetConfigRow.Connection = cfg.Connections.Where(c => c.Name == datasetConfigRow.Connection.Name).FirstOrDefault();
                    }
                }
            }            
        }

        public static void LoadFromFile(string filename)
        {
            StreamReader fstrm = new StreamReader(filename);
            ProtokollerConfiguration.ActualConfigInstance = DeSerialize((string) fstrm.ReadToEnd());            
            fstrm.Close();
        }

        public static void Load(bool doError)
        {
            RegistryKey regKey = Registry.LocalMachine;
            regKey = regKey.CreateSubKey("SYSTEM\\CurrentControlSet\\services\\" + StaticServiceConfig.MyServiceName + "\\Parameters");
            if (regKey != null)
            {
                object conf = regKey.GetValue("XMLConfig");
                if (conf!=null && (string)conf!="")
                {
                    ProtokollerConfiguration.ActualConfigInstance = DeSerialize((string)conf);
                }
                else
                {
                    if (doError)
                        MessageBox.Show("Error reading Config from the Registry, maybe you need to Login as Administrator, or no config has yet been created!");
                }
            }
        }
     
        public static void Clear()
        {
           ActualConfigInstance = new ProtokollerConfiguration();
        }

        public ProtokollerConfiguration()
        {
            Storages = new ObservableCollection<SettingsClasses.Storage.StorageConfig>();
            Connections = new ObservableCollection<ConnectionConfig>();
            Datasets = new ObservableCollection<DatasetConfig>();
        }

        public string CheckConfiguration(bool TestConnections)
        {
            string error = "";

            //Try to Connect to the PLCs
            if (TestConnections)
                foreach (ConnectionConfig connectionConfig in Connections)
                {
                    LibNoDaveConfig lConn = connectionConfig as LibNoDaveConfig;
                    if (lConn != null)
                    {
                        using (PLCConnection myConn = new PLCConnection(lConn.Configuration))
                        {
                            try
                            {
                                myConn.Connect();
                            }
                            catch (Exception ex)
                            {
                                error += "Error: Error connecting \"" + lConn.Name + "\" : " + ex.Message + Environment.NewLine;
                            }
                        }
                    }
                }

            foreach (DatasetConfig datasetConfig in Datasets)
            {
                if (datasetConfig.Storage == null)
                    error += "Error: Dataset \"" + datasetConfig.Name + " - Storage is not set!" + Environment.NewLine;

                foreach (DatasetConfigRow datasetConfigRow in datasetConfig.DatasetConfigRows)
                {
                    if (datasetConfigRow.Connection == null && datasetConfig.Trigger != DatasetTriggerType.Triggered_By_Incoming_Data_On_A_TCPIP_Connection)
                        error += "Error: Dataset \"" + datasetConfig.Name + "\" Row \"" + datasetConfigRow.DatabaseField + "\" - Connection not Set!" + Environment.NewLine;
                }

                if (datasetConfig.Trigger==DatasetTriggerType.Tags_Handshake_Trigger && datasetConfig.TriggerConnection==null)
                    error += "Error: Dataset \"" + datasetConfig.Name + "\" Trigger Connection not set!" + Environment.NewLine;


                /*
                foreach (DatasetConfigRow datasetConfigRow in datasetConfig.DatasetConfigRows)
                {
                    //Look if PLC-Connection was selected
                    if (datasetConfigRow.Connection == null && datasetConfig.Trigger != DatasetTriggerType.Triggered_By_Incoming_Data_On_A_TCPIP_Connection) error += "Error: Dataset \"" + datasetConfig.Name + "\" Row \"" + datasetConfigRow.DatabaseField + "\" - Connection not Set!" + Environment.NewLine;
                    //Look if DatabaseFieldType was selected
                    if (datasetConfigRow.DatabaseFieldType == "") error += "Error: Dataset \"" + datasetConfig.Name + "\" Row \"" + datasetConfigRow.DatabaseField + "\" - DatabaseFieldType not Set!" + Environment.NewLine;
                    ////Look if PLC-ValueType was selected
                    //if (datasetConfigRow.PLCTag.LibNoDaveDataType == null)
                    //    error += "Error: Dataset \"" + datasetConfig.Name + "\" Row \"" + datasetConfigRow.DatabaseField + "\" - PLC-ValueType not Set!" + Environment.NewLine;

                    PLCConnection conn = null as PLCConnection;
                    if (datasetConfigRow.Connection != null)
                    {
                        try
                        {
                            conn = ConnectionList[datasetConfigRow.Connection] as PLCConnection;
                        }
                        catch
                        {
                            conn = null;
                        }
                    }
                    else conn = null;
                    if (conn != null)
                    {
                        try
                        {
                            conn.ReadValue(datasetConfigRow.PLCTag);
                            if (datasetConfigRow.PLCTag.ItemDoesNotExist) error += "Error: Dataset \"" + datasetConfig.Name + "\" Row \"" + datasetConfigRow.DatabaseField + "\" - Error Reading Value on Address " + datasetConfigRow.PLCTag.S7FormatAddress + " !" + Environment.NewLine;
                        }
                        catch (Exception ex)
                        {
                            error += "Error: Dataset \"" + datasetConfig.Name + "\" Row \"" + datasetConfigRow.DatabaseField + "\" - Error Reading Value on Address " + datasetConfigRow.PLCTag.S7FormatAddress + " !" + Environment.NewLine;
                        }
                    }                    
                }
                */

            }
            //Look if a Dataset contains more than one TCP-IP Connection, this is not possible, because
            //when a TCP/IP Connection is used, the Trigger needs to be the incoming Data from this Connection!

            //Look if Trigger on a Dataset with TCP/IP Connection is Incoming Data, and that this trigger is not used on a Connection without TCP/IP

            //Look if TCPIP Connection is only used in one Dataset, because we need the length for each Connection!

            //Look if the Database Field Type is in the Field Types List
            
            //Look if Connection Name exists only once

            //Look if Storrage Name exists only once

            //Look if Field Name Exists only Once

            if (error == "")
                return null;
            return error;
        }
    }   
}

