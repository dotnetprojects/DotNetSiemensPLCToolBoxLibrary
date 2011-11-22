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
                foreach (DatasetConfigRow datasetConfigRow in datasetConfig.DatasetConfigRows)
                {
                    if (datasetConfigRow.Connection == null && datasetConfig.Trigger != DatasetTriggerType.Triggered_By_Incoming_Data_On_A_TCPIP_Connection)
                        error += "Error: Dataset \"" + datasetConfig.Name + "\" Row \"" + datasetConfigRow.DatabaseField + "\" - Connection not Set!" + Environment.NewLine;
                }

                if (datasetConfig.Trigger==DatasetTriggerType.Tags_Handshake_Trigger && datasetConfig.TriggerConnection==null)
                    error += "Error: Dataset \"" + datasetConfig.Name + "\" Trigger Connection not set!" + Environment.NewLine;

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

