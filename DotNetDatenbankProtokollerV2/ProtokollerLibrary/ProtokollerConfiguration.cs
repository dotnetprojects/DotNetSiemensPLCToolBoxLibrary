using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSimaticDatabaseProtokollerLibrary.Common;
using DotNetSimaticDatabaseProtokollerLibrary.Protocolling;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Storage;
using JFKCommonLibrary.Serialization;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Xml.Serialization;
using System.Windows.Forms;

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

            string conf = JsonNetSerialize(ProtokollerConfiguration.ActualConfigInstance);  //SerializeToString<ProtokollerConfiguration>.Serialize(ProtokollerConfiguration.ActualConfigInstance);

            /*
            RegistryKey regKey = Registry.LocalMachine;
            regKey = regKey.CreateSubKey("SYSTEM\\CurrentControlSet\\services\\" + StaticServiceConfig.MyServiceName + "\\Parameters");
            if (regKey != null)
            {
                regKey.SetValue("XMLConfig", conf,RegistryValueKind.String);
                ProtokollerConfiguration.ActualConfigInstance.isDirty = false;
            }
            else
                MessageBox.Show("Error writing Config to the Registry, maybe you need to Login as Administrator?");
            */
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigFileName()));
            StreamWriter sstrm = new StreamWriter(ConfigFileName(), false);
            sstrm.Write(conf);
            sstrm.Close();            
        }

        private static string ConfigFileName()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), StaticServiceConfig.MyServiceName + "\\XMLConfig.config");
        }


        private static Newtonsoft.Json.JsonSerializerSettings jsonNetSettings = new Newtonsoft.Json.JsonSerializerSettings() { ContractResolver = new MyJsonContractResolver(), TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto, PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.All, };
        public static string JsonNetSerialize<T>(T obj, bool beautifyOutput = true)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, typeof(T), beautifyOutput ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None, jsonNetSettings);
        }
        public static T JsonNetDeSerialize<T>(string text)
        {
            if (string.IsNullOrEmpty(text))
                return default(T);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text, jsonNetSettings);
        }

        public class MyJsonContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);
                if (Attribute.IsDefined(member, typeof(XmlIgnoreAttribute), true))
                {
                    property.Ignored = true;
                }
                return property;
            }
        }


        public static void SaveToFile(string filename)
        {
            StreamWriter fstrm = new StreamWriter(filename, false);

            string conf = JsonNetSerialize(ProtokollerConfiguration.ActualConfigInstance); // SerializeToString<ProtokollerConfiguration>.Serialize(ProtokollerConfiguration.ActualConfigInstance);
            fstrm.Write(conf);
            fstrm.Close();
        }

        private static ProtokollerConfiguration DeSerialize(string txt)
        {
            ProtokollerConfiguration retVal = null;
            try
            {
                retVal = JsonNetDeSerialize<ProtokollerConfiguration>(txt);
            }
            catch (Exception)
            { }

            if (retVal == null)
            {
                try
                {
                    retVal = SerializeToString<ProtokollerConfiguration>.DeSerialize(txt);
                }
                catch (Exception)
                { }
            }

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

        public static void ImportFromRegistry()
        {
            RegistryKey regKey = Registry.LocalMachine;
            regKey = regKey.CreateSubKey("SYSTEM\\CurrentControlSet\\services\\" + StaticServiceConfig.MyServiceName + "\\Parameters");
            if (regKey != null)
            {
                object conf = regKey.GetValue("XMLConfig");
                if (conf != null && (string)conf != "")
                {
                    ProtokollerConfiguration.ActualConfigInstance = DeSerialize((string)conf);
                }
                else
                {
                    MessageBox.Show("Error reading Config from the Registry, maybe you need to Login as Administrator, or no config has yet been created!");
                }
            }
        }

        public static void Load()
        {
            if (File.Exists(ConfigFileName()))
            {
                StreamReader sstrm = new StreamReader(ConfigFileName(), false);
                var conf = sstrm.ReadToEnd();
                sstrm.Close();
                ProtokollerConfiguration.ActualConfigInstance = DeSerialize(conf);                                
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

            var ConnectionList = new Dictionary<ConnectionConfig, PLCConnection>();

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
                                ConnectionList.Add(connectionConfig, myConn);
                            }
                            catch (Exception ex)
                            {
                                error += "Error: Error connecting \"" + lConn.Name + "\" : " + ex.Message + Environment.NewLine;
                            }
                        }
                    }
                }

            var TCPIPKeys = new Dictionary<string, int>();

            foreach (DatasetConfig datasetConfig in Datasets)
            {
                var DatasetConnectionKeys = new Dictionary<string, object>();

                //Look if TCPIP Connection is only used in one Dataset, because we need the length for each Connection!
                try
                {
                    if (datasetConfig.DatasetConfigRows.Count > 0)
                    {
                        if (datasetConfig.DatasetConfigRows[0].Connection != null)
                        {
                            var tcp = datasetConfig.DatasetConfigRows[0].Connection as TCPIPConfig;
                            var byteCount = ReadData.GetCountOfBytesToRead(datasetConfig.DatasetConfigRows);
                            if (tcp != null && !tcp.DontUseFixedTCPLength)
                            {
                                if (!TCPIPKeys.ContainsKey(datasetConfig.DatasetConfigRows[0].Connection.Name))
                                {
                                    TCPIPKeys.Add(datasetConfig.DatasetConfigRows[0].Connection.Name, byteCount);
                                }

                                if (TCPIPKeys[datasetConfig.DatasetConfigRows[0].Connection.Name] != byteCount)
                                {
                                    error += "Error: Dataset \"" + datasetConfig.Name + "\" - The same TCP/IP Connection is used in more than one Dataset with differnet bytes sizes, but fixed Length should be used!" + Environment.NewLine;
                                }
                            }
                        }
                    }
                    else
                    {
                        error += "Error: Dataset \"" + datasetConfig.Name + "\" - No DatasetConfigRow is set!" + Environment.NewLine;
                    }
                }
                catch
                {
                    error += "Error: Dataset \"" + datasetConfig.Name + "\" - The same TCP/IP Connection is used in more than one Dataset!" + Environment.NewLine;
                }

                //Look if Trigger on a Dataset with TCP/IP Connection is Incoming Data, and that this trigger is not used on a Connection without TCP/IP
                try
                {
                    if (!(datasetConfig.TriggerConnection is TCPIPConfig) && (datasetConfig.Trigger == DatasetTriggerType.Triggered_By_Incoming_Data_On_A_TCPIP_Connection))
                        error += "Error: Dataset \"" + datasetConfig.Name + "\" - The selected Connection for incoming Trigger is no TCP/IP Connection !" + Environment.NewLine;
                }
                catch { }

                //Look if Trigger Connection was selected (Handshake Trigger)
                if (datasetConfig.Trigger == DatasetTriggerType.Tags_Handshake_Trigger && datasetConfig.TriggerConnection == null)
                    error += "Error: Dataset \"" + datasetConfig.Name + "\" Trigger Connection not set!" + Environment.NewLine;

                //Look if Trigger Connection was selected (TCPIP Trigger)
                if (datasetConfig.Trigger == DatasetTriggerType.Triggered_By_Incoming_Data_On_A_TCPIP_Connection && datasetConfig.TriggerConnection == null)
                    error += "Error: Dataset \"" + datasetConfig.Name + "\" Trigger Connection not set!" + Environment.NewLine;

                if (datasetConfig.Storage == null)
                    error += "Error: Dataset \"" + datasetConfig.Name + " - Storage is not set!" + Environment.NewLine;

                foreach (DatasetConfigRow datasetConfigRow in datasetConfig.DatasetConfigRows)
                {
                    if (datasetConfigRow.Connection == null && datasetConfig.Trigger != DatasetTriggerType.Triggered_By_Incoming_Data_On_A_TCPIP_Connection)
                        error += "Error: Dataset \"" + datasetConfig.Name + "\" Row \"" + datasetConfigRow.DatabaseField + "\" - Connection not Set!" + Environment.NewLine;
                }

                if (datasetConfig.Trigger==DatasetTriggerType.Tags_Handshake_Trigger && datasetConfig.TriggerConnection==null)
                    error += "Error: Dataset \"" + datasetConfig.Name + "\" Trigger Connection not set!" + Environment.NewLine;


                
                foreach (DatasetConfigRow datasetConfigRow in datasetConfig.DatasetConfigRows)
                {
                    //Look if PLC-Connection was selected
                    if (datasetConfigRow.Connection == null && datasetConfig.Trigger != DatasetTriggerType.Triggered_By_Incoming_Data_On_A_TCPIP_Connection) error += "Error: Dataset \"" + datasetConfig.Name + "\" Row \"" + datasetConfigRow.DatabaseField + "\" - Connection not Set!" + Environment.NewLine;
                    //Look if DatabaseFieldType was selected
                    if (datasetConfigRow.DatabaseFieldType == "") error += "Error: Dataset \"" + datasetConfig.Name + "\" Row \"" + datasetConfigRow.DatabaseField + "\" - DatabaseFieldType not Set!" + Environment.NewLine;
                    ////Look if PLC-ValueType was selected
                    if (datasetConfigRow.PLCTag.TagDataType == null)
                        error += "Error: Dataset \"" + datasetConfig.Name + "\" Row \"" + datasetConfigRow.DatabaseField + "\" - PLC-ValueType not Set!" + Environment.NewLine;

                    if (TestConnections && datasetConfig.Trigger != DatasetTriggerType.Triggered_By_Incoming_Data_On_A_TCPIP_Connection)
                    {
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
                }                
            }

            //Look if Connection Name exists only once
            var ConnectionNames = new List<string>();
            var ConnectionKeys = new Dictionary<string, object>();
            foreach (ConnectionConfig item in Connections)
            {
                if (ConnectionKeys.ContainsKey(item.Name))
                    error += "Error: Connection name \"" + item.Name + "\" - exist more than once!" + Environment.NewLine;
                else
                    ConnectionKeys.Add(item.Name, null);
            }

            //Look if Storrage Name exists only once
            var StorageNames = new List<string>();
            var StoragesKeys = new Dictionary<string, object>();
            foreach (StorageConfig item in Storages)
            {
                if (StoragesKeys.ContainsKey(item.Name))
                    error += "Error: Storage name \"" + item.Name + "\" - exist more than once!" + Environment.NewLine;
                else
                    StoragesKeys.Add(item.Name, null);
            }

           
            //Look if the Database Field Type is in the Field Types List
            
            //Look if Field Name Exists only Once -> This is possible in excel, but not in databases

            if (error == "")
                return null;
            return error;
        }
    }   
}

