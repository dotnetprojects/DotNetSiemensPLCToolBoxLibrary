/*
 This implements a high level Wrapper between libnodave.dll and applications written
 in MS .Net languages.
 
 This ConnectionLibrary was written by Jochen Kuehner
 * http://jfk-solutuions.de/
 * 
 * Thanks go to:
 * Steffen Krayer -> For his work on MC7 decoding and the Source for his Decoder
 * Zottel         -> For LibNoDave

 WPFToolboxForSiemensPLCs is free software; you can redistribute it and/or modify
 it under the terms of the GNU Library General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 WPFToolboxForSiemensPLCs is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Library General Public License
 along with Libnodave; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using DotNetSiemensPLCToolBoxLibrary.General;
using Microsoft.Win32;

namespace DotNetSiemensPLCToolBoxLibrary.Communication
{    
#if !IPHONE
    [System.ComponentModel.Editor(typeof(PLCConnectionUITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
#endif   
    [Serializable]    
    /// <summary>
    /// This Class stores the Connection Configuration to a PLC
    /// </summary>
    public class PLCConnectionConfiguration : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        
        private string _connectionName;
        public String ConnectionName
        {
            get { return _connectionName; }
            set
            {
                _connectionName = value;
                NotifyPropertyChanged("ConnectionName");
                NotifyPropertyChanged("ObjectAsString");
            }
        }

        private string _entryPoint = "S7ONLINE";
        public string EntryPoint
        {
            get { return _entryPoint; }
            set { _entryPoint = value; NotifyPropertyChanged("EntryPoint"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _cpuRack = 0;
        public int CpuRack
        {
            get { return _cpuRack; }
            set { _cpuRack = value; NotifyPropertyChanged("CpuRack"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _cpuSlot = 2;
        public int CpuSlot
        {
            get { return _cpuSlot; }
            set { _cpuSlot = value; NotifyPropertyChanged("CpuSlot"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _cpuMpi = 2;
        public int CpuMpi
        {
            get { return _cpuMpi; }
            set { _cpuMpi = value; NotifyPropertyChanged("CpuMpi"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private string _cpuIp = "192.168.1.100";
        public string CpuIP
        {
            get { return _cpuIp; }
            set { _cpuIp = value; NotifyPropertyChanged("CpuIP"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _port = 102;
        public int Port
        {
            get { return _port; }
            set { _port = value; NotifyPropertyChanged("Port"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _lokalMpi = 0;
        public int LokalMpi
        {
            get { return _lokalMpi; }
            set { _lokalMpi = value; NotifyPropertyChanged("LokalMpi"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private string _comPort = "COM1";
        public string ComPort
        {
            get { return _comPort; }
            set { _comPort = value; NotifyPropertyChanged("ComPort"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _plcConnectionType = 1;
        public int PLCConnectionType
        {
            get { return _plcConnectionType; }
            set { _plcConnectionType = value; NotifyPropertyChanged("PLCConnectionType"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _routingplcConnectionType = 1;
        public int RoutingPLCConnectionType
        {
            get { return _routingplcConnectionType; }
            set { _routingplcConnectionType = value; NotifyPropertyChanged("RoutingPLCConnectionType"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _connectionType = 122;
        public int ConnectionType
        {
            get { return _connectionType; }
            set { _connectionType = value; NotifyPropertyChanged("ConnectionType"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _busSpeed = 2;
        public int BusSpeed
        {
            get { return _busSpeed; }
            set { _busSpeed = value; NotifyPropertyChanged("BusSpeed"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private bool _netLinkReset = false;
        public bool NetLinkReset
        {
            get { return _netLinkReset; }
            set { _netLinkReset = value; NotifyPropertyChanged("NetLinkReset"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private string _comPortSpeed = "38400";
        public string ComPortSpeed
        {
            get { return _comPortSpeed; }
            set { _comPortSpeed = value; NotifyPropertyChanged("ComPortSpeed"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _comPortParity = 1;
        public int ComPortParity
        {
            get { return _comPortParity; }
            set
            {
                _comPortParity = value;
                NotifyPropertyChanged("ComPortParity");
                NotifyPropertyChanged("ObjectAsString");
            }
        }

        private bool _routing = false;
        public bool Routing
        {
            get { return _routing; }
            set { _routing = value; NotifyPropertyChanged("Routing"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private string _routingDestination;
        public string RoutingDestination
        {
            get { return _routingDestination; }
            set { _routingDestination = value; NotifyPropertyChanged("RoutingDestination"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _routingDestinationRack;
        public int RoutingDestinationRack
        {
            get { return _routingDestinationRack; }
            set { _routingDestinationRack = value; NotifyPropertyChanged("RoutingDestinationRack"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _routingDestinationSlot;
        public int RoutingDestinationSlot
        {
            get { return _routingDestinationSlot; }
            set { _routingDestinationSlot = value; NotifyPropertyChanged("RoutingDestinationSlot"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _routingSubnet1;
        public int RoutingSubnet1
        {
            get { return _routingSubnet1; }
            set { _routingSubnet1 = value; NotifyPropertyChanged("RoutingSubnet1"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _routingSubnet2;
        public int RoutingSubnet2
        {
            get { return _routingSubnet2; }
            set { _routingSubnet2 = value; NotifyPropertyChanged("RoutingSubnet2"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _timeout = 5000000;
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; NotifyPropertyChanged("Timeout"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _timeoutIpConnect = 5000;
        public int TimeoutIPConnect
        {
            get { return _timeoutIpConnect; }
            set { _timeoutIpConnect = value; NotifyPropertyChanged("TimeoutIPConnect"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private bool _initDone { get; set; }

        private LibNodaveConnectionConfigurationType _configurationType;
        public LibNodaveConnectionConfigurationType ConfigurationType
        {
            get { return _configurationType; }
            set { _configurationType = value; NotifyPropertyChanged("ConfigurationType"); NotifyPropertyChanged("ObjectAsString"); }
        }

        public string ObjectAsString
        {
            get { return ToString(); }
        }

        public static String[] GetConfigurationNames()
        {
#if !IPHONE
            if (File.Exists(ConfigurationPathAndFilename))
            {
                DictionarySerializer<String, PLCConnectionConfiguration> ConnectionsDicSer = new DictionarySerializer<string, PLCConnectionConfiguration>();
                
                Dictionary<String, PLCConnectionConfiguration> Connections = null;
                    StreamReader strm = new StreamReader(ConfigurationPathAndFilename);
                    Connections = ConnectionsDicSer.Deserialize(strm);
                    //string txt = strm.ReadToEnd();
                    strm.Close();
                    //Connections = General.SerializeToString<Dictionary<String, PLCConnectionConfiguration>>.DeSerialize(txt);                    
                    if (Connections != null)
                    {
                        string[] Names = new string[Connections.Count];
                        Connections.Keys.CopyTo(Names, 0);
                        return Names;
                    }
            }
            return new string[0];

            RegistryKey myConnectionKey =
                Registry.CurrentUser.CreateSubKey("Software\\JFKSolutions\\WPFToolboxForSiemensPLCs\\Connections");
            return myConnectionKey.GetSubKeyNames();
#else
			return null;
#endif
        }

        /// <summary>
        /// Empty Constructor for Serialization
        /// </summary>
        public PLCConnectionConfiguration()
        { }

        /// <summary>
        /// Normal Constructor of the Config Object
        /// </summary>
        /// <param name="ConnectionName"></param>
        public PLCConnectionConfiguration(String ConnectionName)
        {
            if (ConnectionName == "")
                this.ConnectionName = "tmpConnection1";
            else
                this.ConnectionName = ConnectionName;

            this.ConfigurationType = LibNodaveConnectionConfigurationType.RegistrySavedConfiguration;

            this.ReloadConfiguration();
        }

        /// <summary>
        /// Normal Constructor of the Config Object
        /// </summary>
        /// <param name="ConnectionName"></param>
        public PLCConnectionConfiguration(String ConnectionName, LibNodaveConnectionConfigurationType configurationType)
        {
            if (ConnectionName == "")
                this.ConnectionName = "tmpConnection1";
            else
                this.ConnectionName = ConnectionName;

            this.ConfigurationType = configurationType;

            this.ReloadConfiguration();
        }

        /// <summary>
        /// Normal Constructor of the Config Object
        /// </summary>
        /// <param name="ConnectionName"></param>
        public static List<PLCConnectionConfiguration> ExportConfigurations()
        {
            List<PLCConnectionConfiguration> retVal = new List<PLCConnectionConfiguration>();
            
            foreach (var myName in GetConfigurationNames())
            {
                retVal.Add(new PLCConnectionConfiguration(myName));
            }
            return retVal;
        }

        /// <summary>
        /// Normal Constructor of the Config Object
        /// </summary>
        /// <param name="ConnectionName"></param>
        public static void ImportConfigurations(List<PLCConnectionConfiguration> configs)
        {
            foreach (var myConfig in configs)
            {
                myConfig.ConfigurationType = LibNodaveConnectionConfigurationType.RegistrySavedConfiguration;
                myConfig.SaveConfiguration();
            }            
        }

        private static string ConfigurationPathAndFilename
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                    "DotNetSiemensPLCToolBoxLibrary\\Connections.config");
            }
        }

        public void ReloadConfiguration()
        {
            if (ConfigurationType == LibNodaveConnectionConfigurationType.RegistrySavedConfiguration)
            {
#if !IPHONE

                if (File.Exists(ConfigurationPathAndFilename))
                {
                    DictionarySerializer<String, PLCConnectionConfiguration> ConnectionsDicSer = new DictionarySerializer<string, PLCConnectionConfiguration>();

                    Dictionary<String, PLCConnectionConfiguration> Connections = null;
                    StreamReader strm = new StreamReader(ConfigurationPathAndFilename);
                    Connections = ConnectionsDicSer.Deserialize(strm);
                    //string txt = strm.ReadToEnd();
                    strm.Close();
                    //Connections = General.SerializeToString<Dictionary<String, PLCConnectionConfiguration>>.DeSerialize(txt);                    
                    if (Connections != null && Connections.ContainsKey(ConnectionName))
                    {
                        PLCConnectionConfiguration akConf = Connections[ConnectionName];
                        this.EntryPoint = akConf.EntryPoint;
                        this.CpuRack = akConf.CpuRack;
                        this.CpuSlot = akConf.CpuSlot;
                        this.CpuMpi = akConf.CpuMpi;
                        this.CpuIP = akConf.CpuIP;
                        this.LokalMpi = akConf.LokalMpi;
                        this.ComPort = akConf.ComPort;
                        this.ConnectionType = akConf.ConnectionType;
                        this.BusSpeed = akConf.BusSpeed;
                        this.NetLinkReset = akConf.NetLinkReset;
                        this.ComPortSpeed = akConf.ComPortSpeed;
                        this.ComPortParity = akConf.ComPortParity;
                        this.Routing = akConf.Routing;
                        this.RoutingDestinationRack = akConf.RoutingDestinationRack;
                        this.RoutingDestinationSlot = akConf.RoutingDestinationSlot;
                        this.RoutingSubnet1 = akConf.RoutingSubnet1;
                        this.RoutingSubnet2 = akConf.RoutingSubnet2;
                        this.RoutingDestination = akConf.RoutingDestination;
                        this.Port = akConf.Port;

                        this.PLCConnectionType = akConf.PLCConnectionType;
                        this.RoutingPLCConnectionType = akConf.RoutingPLCConnectionType;

                        this.Timeout = akConf.Timeout;
                        this.TimeoutIPConnect = akConf.TimeoutIPConnect;
                    }
                }
                return;


                RegistryKey myConnectionKey =
                    Registry.CurrentUser.CreateSubKey(
                        "Software\\JFKSolutions\\WPFToolboxForSiemensPLCs\\Connections\\" + ConnectionName);
                if (myConnectionKey != null)
                {
                    this.EntryPoint = (String)myConnectionKey.GetValue("EntryPoint", "S7ONLINE");
                    this.CpuRack = Convert.ToInt32(myConnectionKey.GetValue("CpuRack", "0"));
                    this.CpuSlot = Convert.ToInt32(myConnectionKey.GetValue("CpuSlot", "2"));
                    this.CpuMpi = Convert.ToInt32(myConnectionKey.GetValue("CpuMpi", "2"));
                    this.CpuIP = (String)myConnectionKey.GetValue("CpuIP", "192.168.1.1");
                    this.LokalMpi = Convert.ToInt32(myConnectionKey.GetValue("LokalMpi", "0"));
                    this.ComPort = (String)myConnectionKey.GetValue("ComPort", "");
                    this.ConnectionType = Convert.ToInt32(myConnectionKey.GetValue("ConnectionType", "1"));
                    this.BusSpeed = Convert.ToInt32(myConnectionKey.GetValue("BusSpeed", "2"));
                    this.NetLinkReset = Convert.ToBoolean(myConnectionKey.GetValue("NetLinkReset", "false"));
                    this.ComPortSpeed = (String)myConnectionKey.GetValue("ComPortSpeed", "38400");
                    this.ComPortParity = Convert.ToInt32(myConnectionKey.GetValue("ComPortParity", "1"));
                    this.Routing = Convert.ToBoolean(myConnectionKey.GetValue("Routing", "false"));
                    this.RoutingDestinationRack =
                        Convert.ToInt32(myConnectionKey.GetValue("RoutingDestinationRack", "0"));
                    this.RoutingDestinationSlot =
                        Convert.ToInt32(myConnectionKey.GetValue("RoutingDestinationSlot", "2"));
                    this.RoutingSubnet1 = Convert.ToInt32(myConnectionKey.GetValue("RoutingSubnet1", "0"));
                    this.RoutingSubnet2 = Convert.ToInt32(myConnectionKey.GetValue("RoutingSubnet2", "0"));
                    this.RoutingDestination = Convert.ToString(myConnectionKey.GetValue("RoutingDestination", "2"));
                    this.Port = Convert.ToInt32(myConnectionKey.GetValue("Port", "102"));

                    this.PLCConnectionType = Convert.ToInt32(myConnectionKey.GetValue("PLCConnectionType", "1"));
                    this.RoutingPLCConnectionType = Convert.ToInt32(myConnectionKey.GetValue("RoutingPLCConnectionType", "1"));

                    this.Timeout = Convert.ToInt32(myConnectionKey.GetValue("Timeout", "5000000"));
                    this.TimeoutIPConnect = Convert.ToInt32(myConnectionKey.GetValue("TimeoutIPConnect", "5000"));
                }
#endif
            }
            else
            {
                if (!_initDone)
                {
                    this.ConnectionType = 122;
                    this.CpuMpi = 2;
                    this.EntryPoint = "S7ONLINE";
                    this.CpuIP = "192.168.1.1";
                    this.CpuRack = 0;
                    this.CpuSlot = 2;
                    this.Port = 102;
                    this.TimeoutIPConnect = 5000;
                    this.Timeout = 5000000;
                    _initDone = true;
                }
            }
        }

        public static void DeleteConfiguration(string ConnectionName)
        {
#if !IPHONE
            try
            {
                DictionarySerializer<String, PLCConnectionConfiguration> ConnectionsDicSer = new DictionarySerializer<string, PLCConnectionConfiguration>();

                Dictionary<String, PLCConnectionConfiguration> Connections = null;
                if (File.Exists(ConfigurationPathAndFilename))
                {
                    StreamReader strm = new StreamReader(ConfigurationPathAndFilename);
                    Connections = ConnectionsDicSer.Deserialize(strm);
                    //string txt = strm.ReadToEnd();
                    strm.Close();
                    //Connections = General.SerializeToString<Dictionary<String, PLCConnectionConfiguration>>.DeSerialize(txt);                    
                }
                if (Connections == null)
                    Connections = new Dictionary<string, PLCConnectionConfiguration>();

                if (Connections.ContainsKey(ConnectionName))
                    Connections.Remove(ConnectionName);

                Directory.CreateDirectory(Path.GetDirectoryName(ConfigurationPathAndFilename));
                StreamWriter sstrm = new StreamWriter(ConfigurationPathAndFilename, false);
                ConnectionsDicSer.Serialize(Connections, sstrm);
                //sstrm.Write(stxt);
                //sstrm.Flush();
                sstrm.Close();

                return;

                Registry.CurrentUser.DeleteSubKeyTree(
                    "Software\\JFKSolutions\\WPFToolboxForSiemensPLCs\\Connections\\" + ConnectionName);
            }
            catch (Exception)
            { }
#endif
        }

        public void SaveConfiguration()
        {
            if (ConfigurationType == LibNodaveConnectionConfigurationType.RegistrySavedConfiguration)
            {
#if !IPHONE

                DictionarySerializer<String, PLCConnectionConfiguration> ConnectionsDicSer = new DictionarySerializer<string, PLCConnectionConfiguration>();
                    
                Dictionary<String, PLCConnectionConfiguration> Connections = null;
                if (File.Exists(ConfigurationPathAndFilename))
                {
                    StreamReader strm = new StreamReader(ConfigurationPathAndFilename);
                    Connections = ConnectionsDicSer.Deserialize(strm);
                    //string txt = strm.ReadToEnd();
                    strm.Close();
                    //Connections = General.SerializeToString<Dictionary<String, PLCConnectionConfiguration>>.DeSerialize(txt);                    
                }
                if (Connections == null)
                    Connections = new Dictionary<string, PLCConnectionConfiguration>();

                if (Connections.ContainsKey(ConnectionName))
                    Connections.Remove(ConnectionName);

                Connections.Add(ConnectionName, this);

                Directory.CreateDirectory(Path.GetDirectoryName(ConfigurationPathAndFilename));
                StreamWriter sstrm = new StreamWriter(ConfigurationPathAndFilename, false);
                ConnectionsDicSer.Serialize(Connections, sstrm);
                //sstrm.Write(stxt);
                //sstrm.Flush();
                sstrm.Close();

                return;

                RegistryKey myConnectionKey =
                    Registry.CurrentUser.CreateSubKey(
                        "Software\\JFKSolutions\\WPFToolboxForSiemensPLCs\\Connections\\" + ConnectionName);
                if (myConnectionKey != null)
                {
                    myConnectionKey.SetValue("EntryPoint", this.EntryPoint);
                    myConnectionKey.SetValue("CpuRack", this.CpuRack);
                    myConnectionKey.SetValue("CpuSlot", this.CpuSlot);
                    myConnectionKey.SetValue("CpuMpi", this.CpuMpi);
                    myConnectionKey.SetValue("CpuIP", this.CpuIP);
                    myConnectionKey.SetValue("LokalMpi", this.LokalMpi);
                    myConnectionKey.SetValue("ComPort", this.ComPort);
                    myConnectionKey.SetValue("ConnectionType", this.ConnectionType);
                    myConnectionKey.SetValue("BusSpeed", this.BusSpeed);
                    myConnectionKey.SetValue("NetLinkReset", this.NetLinkReset);
                    myConnectionKey.SetValue("ComPortSpeed", this.ComPortSpeed);
                    myConnectionKey.SetValue("ComPortParity", this.ComPortParity);
                    myConnectionKey.SetValue("Routing", this.Routing);
                    myConnectionKey.SetValue("RoutingDestinationRack", this.RoutingDestinationRack);
                    myConnectionKey.SetValue("RoutingDestinationSlot", this.RoutingDestinationSlot);
                    myConnectionKey.SetValue("RoutingSubnet1", this.RoutingSubnet1);
                    myConnectionKey.SetValue("RoutingSubnet2", this.RoutingSubnet2);
                    myConnectionKey.SetValue("RoutingDestination", this.RoutingDestination);
                    myConnectionKey.SetValue("Port", this.Port);
                    myConnectionKey.SetValue("PLCConnectionType", this.PLCConnectionType);
                    myConnectionKey.SetValue("RoutingPLCConnectionType", this.RoutingPLCConnectionType);
                    myConnectionKey.SetValue("Timeout", this.Timeout);
                    myConnectionKey.SetValue("TimeoutIPConnect", this.TimeoutIPConnect);
                }
#endif
            }
        }

        public override string ToString()
        {
            string retVal = "";
            switch (ConnectionType)
            {

                case 1:
                    retVal = "MPI über seriell" + " (Port: " + ComPort + ", MPI: " + CpuMpi.ToString() + ")";
                    break;
                case 2:
                    retVal = "MPI über seriell (Andrews Version)" + " (Port: " + ComPort + ", MPI: " + CpuMpi.ToString() + ")";
                    break;
                case 3:
                    retVal = "MPI über seriell (Step7 Version)" + " (Port: " + ComPort + ", MPI: " + CpuMpi.ToString() + ")";
                    break;
                case 4:
                    retVal = "MPI über seriell" + " (Port: " + ComPort + ", MPI: " + CpuMpi.ToString() + ")";
                    break;
                case 10:
                    retVal = "PPI über seriell";
                    break;
                case 20:
                    retVal = "AS 511";
                    break;
                case 50:
                    retVal = "Step7 DLL" + " (" + EntryPoint + ")";
                    break;
                case 122:
                    retVal = "ISO over TCP" + " (IP:" + CpuIP.ToString() + ",Rack:" + CpuRack.ToString() + ",Slot:" + CpuSlot.ToString() + ")";
                    break;
                case 123:
                    retVal = "ISO over TCP (CP243)" + " (IP:" + CpuIP.ToString() + ",Rack:" + CpuRack.ToString() + ",Slot:" + CpuSlot.ToString() + ")";
                    break;
                case 223:
                    retVal = "Netlink lite" + " (IP:" + CpuIP.ToString() + ")";
                    break;
                case 224:
                    retVal = "Netlink lite PPI" + " (IP:" + CpuIP.ToString() + ")";
                    break;
                case 230:
                    retVal = "Netlink PRO" + " (IP:" + CpuIP.ToString() + ")";
                    break;
            }

            if (Routing)
                if (!RoutingDestination.Contains("."))
                    retVal += " (Routing: MPI/PB:" + RoutingDestination + ",Netz:" + RoutingSubnet1.ToString("X") + "-" + RoutingSubnet2.ToString("X") + ")";
                else
                    retVal += " (Routing: IP:" + RoutingDestination + ",Rack:" + RoutingDestinationRack.ToString() + ",Slot:" + RoutingDestinationSlot.ToString() + ",Netz:" + RoutingSubnet1.ToString("X") + "-" + RoutingSubnet2.ToString("X") + ")";
            return retVal;
        }


        public void SaveConfigToFile(string filename)
        {
            string txt = General.SerializeToString<PLCConnectionConfiguration>.Serialize(this);
            StreamWriter strm = new StreamWriter(filename, false);
            strm.Write(txt);
            strm.Flush();
            strm.Close();
        }

        public static PLCConnectionConfiguration LoadConfigFromFile(string filename)
        {
            StreamReader strm = new StreamReader(filename);
            string txt = strm.ReadToEnd();
            strm.Close();
            return General.SerializeToString<PLCConnectionConfiguration>.DeSerialize(txt);              
        }
    }

    public enum LibNodaveConnectionConfigurationType
    {
        RegistrySavedConfiguration = 1,
        ObjectSavedConfiguration = 2
    }

    public enum LibNodaveConnectionTypes
    {
        // ReSharper disable InconsistentNaming
        MPI_über_Serial_Adapter = 1,
        MPI_über_Serial_Adapter_Andrews_Version_without_STX = 2,
        MPI_über_Serial_Adapter_Step_7_Version = 3,
        MPI_über_Serial_Adapter_Adrews_Version_with_STX = 4,
        PPI_über_Serial_Adapter = 10,
        AS_511 = 20,
        Use_Step7_DLL = 50,
        ISO_over_TCP = 122,
        ISO_over_TCP_CP_243 = 123,
        Netlink_lite = 223,
        Netlink_lite_PPI = 224,
        Netlink_Pro = 230,
        // ReSharper restore InconsistentNaming
    }

    public enum LibNodaveConnectionBusSpeed
    {
        // ReSharper disable InconsistentNaming
        Speed_9k = 0,
        Speed_19k = 1,
        Speed_187k = 2,
        Speed_500k = 3,
        Speed_1500k = 4,
        Speed_45k = 5,
        Speed_93k = 6
        // ReSharper restore InconsistentNaming
    }
}
