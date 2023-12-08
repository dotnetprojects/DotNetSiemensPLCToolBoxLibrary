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

using DotNetSiemensPLCToolBoxLibrary.General;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
    [System.ComponentModel.Editor("DotNetSiemensPLCToolBoxLibrary.Communication.PLCConnectionUITypeEditor", "System.Drawing.Design.UITypeEditor")]
    [Serializable]
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

        #region Properties

        private string _connectionName;

        /// <summary>
        /// Name of the connection. This value can be freely defined by the user
        /// </summary>
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

        /// <summary>
        /// the entry point of the Simatic NEt configuration to be used.
        /// Please refer to the Simatic Net documentation for details.
        /// by default is S7ONLINE
        /// </summary>
        public string EntryPoint
        {
            get { return _entryPoint; }
            set { _entryPoint = value; NotifyPropertyChanged("EntryPoint"); NotifyPropertyChanged("ObjectAsString"); }
        }

        //Only possible on 400er CPUs
        private bool _useShortDataBlockRequest = false;

        public bool UseShortDataBlockRequest
        {
            get { return _useShortDataBlockRequest; }
            set { _useShortDataBlockRequest = value; NotifyPropertyChanged("UseShortDataBlockRequest"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _cpuRack = 0;

        /// <summary>
        /// The Rack where the destination CPU is inserted. This value should be 0 in almost all cases.
        /// Only in Multi CPU configurations this needs to be adjusted.
        /// </summary>
        public int CpuRack
        {
            get { return _cpuRack; }
            set { _cpuRack = value; NotifyPropertyChanged("CpuRack"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _cpuSlot = 2;

        /// <summary>
        /// The slot where the destination CPU is inserted in the selected Rack.
        /// This value depends on the particular hardware configuration of the CPU.
        /// Usually it is 2 for S7-300 CPUs and 3 for S7-400 CPUs
        /// On Redundant H-CPU configurations this value must be set accordingly, in order for the comunication to work properly
        /// </summary>
        public int CpuSlot
        {
            get { return _cpuSlot; }
            set { _cpuSlot = value; NotifyPropertyChanged("CpuSlot"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _cpuMpi = 2;

        /// <summary>
        /// The MPI address of the remote CPU. this depends on the particular hardware configuration and is usually
        /// set to 2 by default.
        /// Only relevant for MPI connections
        /// </summary>
        public int CpuMpi
        {
            get { return _cpuMpi; }
            set { _cpuMpi = value; NotifyPropertyChanged("CpuMpi"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private string _cpuIp = "192.168.1.100";

        /// <summary>
        /// The IP-Adress or Hostname of the CPU's ethernet connection.
        /// Only relevant for ISO over TCP connections.
        /// </summary>
        public string CpuIP
        {
            get { return _cpuIp; }
            set { _cpuIp = value; NotifyPropertyChanged("CpuIP"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _port = 102;

        /// <summary>
        /// The TCP Port to be used for connecting the CPU via ISO over TCP.
        /// This value usually should set to the default 102 (ISO Transport Service Access Point (TSAP) Class 0 protocol)
        /// </summary>
        public int Port
        {
            get { return _port; }
            set { _port = value; NotifyPropertyChanged("Port"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _writePort = 30501;

        public int WritePort
        {
            get { return _writePort; }
            set { _writePort = value; NotifyPropertyChanged("WritePort"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private int _lokalMpi = 0;

        /// <summary>
        /// The local MPI Address to be used by the MPI adapter to comunicate with the CPU.
        /// By convention the address 0 is reserved for Programing devices.
        /// </summary>
        public int LokalMpi
        {
            get { return _lokalMpi; }
            set { _lokalMpi = value; NotifyPropertyChanged("LokalMpi"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private string _comPort = "COM1";

        /// <summary>
        /// The Com port name where the MPI adapter is connected to.
        /// if an RS232 to USB adapter is used (such as the Siamtic USB-MPI adapter)
        /// the Virtual com port name of the adapter must be chosen.
        /// </summary>
        public string ComPort
        {
            get { return _comPort; }
            set { _comPort = value; NotifyPropertyChanged("ComPort"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private LibNodaveConnectionResource _plcConnectionType = LibNodaveConnectionResource.PG;

        /// <summary>
        /// Defines the Connection resource to be used in the PLC
        /// </summary>
        public LibNodaveConnectionResource PLCConnectionType
        {
            get { return _plcConnectionType; }
            set { _plcConnectionType = value; NotifyPropertyChanged("PLCConnectionType"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private LibNodaveConnectionResource _routingplcConnectionType = LibNodaveConnectionResource.PG;

        public LibNodaveConnectionResource RoutingPLCConnectionType
        {
            get { return _routingplcConnectionType; }
            set { _routingplcConnectionType = value; NotifyPropertyChanged("RoutingPLCConnectionType"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private LibNodaveConnectionTypes _connectionType = LibNodaveConnectionTypes.ISO_over_TCP;

        /// <summary>
        /// Defines how the connection to the PLC should be established
        /// </summary>
        public LibNodaveConnectionTypes ConnectionType
        {
            get { return _connectionType; }
            set { _connectionType = value; NotifyPropertyChanged("ConnectionType"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private LibNodaveConnectionBusSpeed _busSpeed = LibNodaveConnectionBusSpeed.Speed_187k;

        /// <summary>
        /// defines the Bus speed for the connected MPI adapter
        /// The default MPI bus speed is 19200 bit / second
        /// </summary>
        public LibNodaveConnectionBusSpeed BusSpeed
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

        /// <summary>
        /// defines the Bus speed for the connected MPI adapter
        /// The default MPI bus speed is 19200 bit / second
        /// </summary>
        public string ComPortSpeed
        {
            get { return _comPortSpeed; }
            set { _comPortSpeed = value; NotifyPropertyChanged("ComPortSpeed"); NotifyPropertyChanged("ObjectAsString"); }
        }

        private LibNodaveConnectionBusParity _comPortParity = LibNodaveConnectionBusParity.even;

        /// <summary>
        /// Com port parity to use for MPI connection. By default MPI uses Even parity
        /// </summary>
        public LibNodaveConnectionBusParity ComPortParity
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

        private TimeSpan _timeout = TimeSpan.FromMilliseconds(5000);

        /// <summary>
        /// The timeout to wait for responses from the PLC
        /// in microseconds
        /// </summary>
        [XmlIgnore]
        public TimeSpan Timeout
        {
            get { return _timeout; }
            set { _timeout = value; NotifyPropertyChanged("Timeout"); NotifyPropertyChanged("ObjectAsString"); }
        }

        /// <summary>
        /// Used for serialization only.
        /// </summary>
        /// <remarks>The Problem is that the XML Serializer can not serialize Timespans. So serialize an "hidden" integer instead</remarks>
        [XmlElement(ElementName = "Timeout"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int TimeoutMicroseconds
        {
            get { return (int)Timeout.TotalMilliseconds * 1000; }
            set { Timeout = TimeSpan.FromMilliseconds(value / 1000); }
        }

        private TimeSpan _timeoutIpConnect = TimeSpan.FromMilliseconds(5000);

        /// <summary>
        /// The default Timeout to be used for ISO over TCP conection
        /// In miliseconds
        /// </summary>
        [XmlIgnore]
        public TimeSpan TimeoutIPConnect
        {
            get { return _timeoutIpConnect; }
            set { _timeoutIpConnect = value; NotifyPropertyChanged("TimeoutIPConnect"); NotifyPropertyChanged("ObjectAsString"); }
        }

        /// <summary>
        /// Used for serialization only.
        /// </summary>
        /// <remarks>The Problem is that the XML Serializer can not serialize Timespans. So serialize an "hidden" integer instead</remarks>
        [XmlElement(ElementName = "TimeoutIPConnect"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int TimeoutIPConnectMiliseconds
        {
            get { return (int)TimeoutIPConnect.TotalMilliseconds; }
            set { _timeoutIpConnect = TimeSpan.FromMilliseconds(value); }
        }

        private bool _initDone { get; set; }

        private LibNodaveConnectionConfigurationType _configurationType;

        public LibNodaveConnectionConfigurationType ConfigurationType
        {
            get { return _configurationType; }
            set { _configurationType = value; NotifyPropertyChanged("ConfigurationType"); NotifyPropertyChanged("ObjectAsString"); }
        }

        #endregion Properties

        public string ObjectAsString
        {
            get { return ToString(); }
        }

        public static String[] GetConfigurationNames()
        {
#if !IPHONE
            if (File.Exists(ConfigurationPathAndFilename))
            {
                Dictionary<String, PLCConnectionConfiguration> Connections = null;
                using (TextReader strm = new StreamReader(ConfigurationPathAndFilename))
                {
                    Connections = LoadConfigFile(strm);
                    strm.Close();
                }

                //Connections = General.SerializeToString<Dictionary<String, PLCConnectionConfiguration>>.DeSerialize(txt);
                if (Connections != null)
                {
                    string[] Names = new string[Connections.Count];
                    Connections.Keys.CopyTo(Names, 0);
                    return Names;
                }
            }
            return new string[0];

            //RegistryKey myConnectionKey =
            //    Registry.CurrentUser.CreateSubKey("Software\\JFKSolutions\\WPFToolboxForSiemensPLCs\\Connections");
            //return myConnectionKey.GetSubKeyNames();
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
                    Dictionary<String, PLCConnectionConfiguration> Connections = null;
                    using (TextReader strm = new StreamReader(ConfigurationPathAndFilename))
                    {
                        Connections = LoadConfigFile(strm);
                        strm.Close();
                    }

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
                        this.WritePort = akConf.WritePort;
                        this.UseShortDataBlockRequest = akConf.UseShortDataBlockRequest;

                        this.PLCConnectionType = akConf.PLCConnectionType;
                        this.RoutingPLCConnectionType = akConf.RoutingPLCConnectionType;

                        this.Timeout = akConf.Timeout;
                        this.TimeoutIPConnect = akConf.TimeoutIPConnect;
                    }
                }
                return;

                //RegistryKey myConnectionKey =
                //    Registry.CurrentUser.CreateSubKey(
                //        "Software\\JFKSolutions\\WPFToolboxForSiemensPLCs\\Connections\\" + ConnectionName);
                //if (myConnectionKey != null)
                //{
                //    this.EntryPoint = (String)myConnectionKey.GetValue("EntryPoint", "S7ONLINE");
                //    this.CpuRack = Convert.ToInt32(myConnectionKey.GetValue("CpuRack", "0"));
                //    this.CpuSlot = Convert.ToInt32(myConnectionKey.GetValue("CpuSlot", "2"));
                //    this.CpuMpi = Convert.ToInt32(myConnectionKey.GetValue("CpuMpi", "2"));
                //    this.CpuIP = (String)myConnectionKey.GetValue("CpuIP", "192.168.1.1");
                //    this.LokalMpi = Convert.ToInt32(myConnectionKey.GetValue("LokalMpi", "0"));
                //    this.ComPort = (String)myConnectionKey.GetValue("ComPort", "");
                //    this.ConnectionType = (LibNodaveConnectionTypes)Convert.ToInt32(myConnectionKey.GetValue("ConnectionType", "1"));
                //    this.BusSpeed = (LibNodaveConnectionBusSpeed)Convert.ToInt32(myConnectionKey.GetValue("BusSpeed", "2"));
                //    this.NetLinkReset = Convert.ToBoolean(myConnectionKey.GetValue("NetLinkReset", "false"));
                //    this.ComPortSpeed = (String)myConnectionKey.GetValue("ComPortSpeed", "38400");
                //    this.ComPortParity = (LibNodaveConnectionBusParity)Convert.ToInt32(myConnectionKey.GetValue("ComPortParity", "1"));
                //    this.Routing = Convert.ToBoolean(myConnectionKey.GetValue("Routing", "false"));
                //    this.RoutingDestinationRack =
                //        Convert.ToInt32(myConnectionKey.GetValue("RoutingDestinationRack", "0"));
                //    this.RoutingDestinationSlot =
                //        Convert.ToInt32(myConnectionKey.GetValue("RoutingDestinationSlot", "2"));
                //    this.RoutingSubnet1 = Convert.ToInt32(myConnectionKey.GetValue("RoutingSubnet1", "0"));
                //    this.RoutingSubnet2 = Convert.ToInt32(myConnectionKey.GetValue("RoutingSubnet2", "0"));
                //    this.RoutingDestination = Convert.ToString(myConnectionKey.GetValue("RoutingDestination", "2"));
                //    this.Port = Convert.ToInt32(myConnectionKey.GetValue("Port", "102"));
                //    this.WritePort = Convert.ToInt32(myConnectionKey.GetValue("WritePort", "30501"));

                //    this.PLCConnectionType = (LibNodaveConnectionResource)Convert.ToInt32(myConnectionKey.GetValue("PLCConnectionType", "1"));
                //    this.RoutingPLCConnectionType = (LibNodaveConnectionResource)Convert.ToInt32(myConnectionKey.GetValue("RoutingPLCConnectionType", "1"));

                //    this.Timeout = Convert.ToInt32(myConnectionKey.GetValue("Timeout", "5000000"));
                //    this.TimeoutIPConnect =  TimeSpan.FromMilliseconds( Convert.ToInt32(myConnectionKey.GetValue("TimeoutIPConnect", "5000")));
                //}
#endif
            }
            else
            {
                if (!_initDone)
                {
                    this.ConnectionType = LibNodaveConnectionTypes.ISO_over_TCP;
                    this.CpuMpi = 2;
                    this.EntryPoint = "S7ONLINE";
                    this.CpuIP = "192.168.1.1";
                    this.CpuRack = 0;
                    this.CpuSlot = 2;
                    this.Port = 102;
                    this.TimeoutIPConnect = TimeSpan.FromMilliseconds(5000);
                    this.Timeout = TimeSpan.FromMilliseconds(5000);
                    _initDone = true;
                }
            }
        }

        #region Load and Repair Config File

        /// <summary>
        /// Reads the configuration from an Stream
        /// </summary>
        /// <param name="strm"></param>
        /// <returns></returns>
        private static Dictionary<String, PLCConnectionConfiguration> LoadConfigFile(TextReader strm)
        {
            Dictionary<String, PLCConnectionConfiguration> Connections;
            DictionarySerializer<String, PLCConnectionConfiguration> ConnectionsDicSer = new DictionarySerializer<string, PLCConnectionConfiguration>();

            //the idea here is, to try to deserialize the data once, and if it fails,
            //Try to repair it. If it fails also the second time, then give up an throw
            //This is neede to maintain Backwards compatibility with config files that do not use
            //Enumerations.
            try
            {
                Connections = ConnectionsDicSer.Deserialize(strm);
            }
            catch (Exception ex)
            {
                Console.WriteLine("1 PLCConnectionConfiguration.cs threw exception");
                try
                {
                    strm.Close(); //Close old stream
                    strm = new StreamReader(ConfigurationPathAndFilename); //Reopen temporary stream
                    string repaired = repairConfig(strm.ReadToEnd());
                    strm.Close(); //Close temporary stream
                    strm = new StringReader(repaired);
                    Connections = ConnectionsDicSer.Deserialize(strm);
                }
                catch
                {
                    Console.WriteLine("2 PLCConnectionConfiguration.cs threw exception");
                    throw ex; //Throw orignial Exception
                }
            }
            return Connections;
        }

        /// <summary>
        /// Try to repair the configuration file if an error is encoutnerd
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private static string repairConfig(string original)
        {
            string config = original;
            config = fixEnum(config, "ConnectionType", typeof(LibNodaveConnectionTypes));
            config = fixEnum(config, "PLCConnectionType", typeof(LibNodaveConnectionResource));
            config = fixEnum(config, "RoutingPLCConnectionType", typeof(LibNodaveConnectionResource));
            config = fixEnum(config, "BusSpeed", typeof(LibNodaveConnectionBusSpeed));
            config = fixEnum(config, "ComPortParity", typeof(LibNodaveConnectionBusParity));

            return config;
        }

        /// <summary>
        /// Replace the numerical enumeration value with its textual Value
        /// </summary>
        /// <param name="original">The original Config file content to search in</param>
        /// <param name="PropertyName">The Name of the property to replace the value on</param>
        /// <param name="PropertyEnumType">The type of Enumeration of the property in question</param>
        /// <returns></returns>
        private static string fixEnum(string original, string PropertyName, Type PropertyEnumType)
        {
            string New = original;

            int idx = original.IndexOf("<" + PropertyName + ">", 0, StringComparison.Ordinal);

            while (idx >= 0)
            {
                idx = idx + PropertyName.Length + 2;

                //Find next "Closing" tag and extract the current numerical value
                int Eidx = New.IndexOf("</", idx, StringComparison.Ordinal);
                string Value = New.Substring(idx, Eidx - idx);

                //Convert to enumeration and replace in file
                string EnumValue = Enum.Parse(PropertyEnumType, Value).ToString();
                if (Value == EnumValue) EnumValue = Enum.GetNames(PropertyEnumType)[0]; //parsing failed, the Value was invalid

                New = New.Remove(idx, Eidx - idx);
                New = New.Insert(idx, EnumValue.ToString());

                idx = New.IndexOf("<" + PropertyName + ">", idx, StringComparison.Ordinal);
            }

            return New;
        }

        #endregion Load and Repair Config File

        public static void DeleteConfiguration(string ConnectionName)
        {
#if !IPHONE
            try
            {
                Dictionary<String, PLCConnectionConfiguration> Connections = null;

                if (File.Exists(ConfigurationPathAndFilename))
                {
                    using (TextReader strm = new StreamReader(ConfigurationPathAndFilename))
                    {
                        Connections = LoadConfigFile(strm);
                        strm.Close();
                    }
                }
                if (Connections == null)
                    Connections = new Dictionary<string, PLCConnectionConfiguration>();

                if (Connections.ContainsKey(ConnectionName))
                    Connections.Remove(ConnectionName);

                Directory.CreateDirectory(Path.GetDirectoryName(ConfigurationPathAndFilename));
                StreamWriter sstrm = new StreamWriter(ConfigurationPathAndFilename, false);
                DictionarySerializer<String, PLCConnectionConfiguration> ConnectionsDicSer = new DictionarySerializer<string, PLCConnectionConfiguration>();
                ConnectionsDicSer.Serialize(Connections, sstrm);
                //sstrm.Write(stxt);
                //sstrm.Flush();
                sstrm.Close();

                return;

                //Registry.CurrentUser.DeleteSubKeyTree(
                //    "Software\\JFKSolutions\\WPFToolboxForSiemensPLCs\\Connections\\" + ConnectionName);
            }
            catch (Exception)
            {
                Console.WriteLine("3 PLCConnectionConfiguration.cs threw exception");
            }
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
                    using (TextReader strm = new StreamReader(ConfigurationPathAndFilename))
                    {
                        Connections = LoadConfigFile(strm);
                        strm.Close();
                    }
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

                //RegistryKey myConnectionKey =
                //    Registry.CurrentUser.CreateSubKey(
                //        "Software\\JFKSolutions\\WPFToolboxForSiemensPLCs\\Connections\\" + ConnectionName);
                //if (myConnectionKey != null)
                //{
                //    myConnectionKey.SetValue("EntryPoint", this.EntryPoint);
                //    myConnectionKey.SetValue("CpuRack", this.CpuRack);
                //    myConnectionKey.SetValue("CpuSlot", this.CpuSlot);
                //    myConnectionKey.SetValue("CpuMpi", this.CpuMpi);
                //    myConnectionKey.SetValue("CpuIP", this.CpuIP);
                //    myConnectionKey.SetValue("LokalMpi", this.LokalMpi);
                //    myConnectionKey.SetValue("ComPort", this.ComPort);
                //    myConnectionKey.SetValue("ConnectionType", (int)this.ConnectionType);
                //    myConnectionKey.SetValue("BusSpeed", (int)this.BusSpeed);
                //    myConnectionKey.SetValue("NetLinkReset", this.NetLinkReset);
                //    myConnectionKey.SetValue("ComPortSpeed", this.ComPortSpeed);
                //    myConnectionKey.SetValue("ComPortParity", (int)this.ComPortParity);
                //    myConnectionKey.SetValue("Routing", this.Routing);
                //    myConnectionKey.SetValue("RoutingDestinationRack", this.RoutingDestinationRack);
                //    myConnectionKey.SetValue("RoutingDestinationSlot", this.RoutingDestinationSlot);
                //    myConnectionKey.SetValue("RoutingSubnet1", this.RoutingSubnet1);
                //    myConnectionKey.SetValue("RoutingSubnet2", this.RoutingSubnet2);
                //    myConnectionKey.SetValue("RoutingDestination", this.RoutingDestination);
                //    myConnectionKey.SetValue("Port", this.Port);
                //    myConnectionKey.SetValue("WritePort", this.WritePort);
                //    myConnectionKey.SetValue("PLCConnectionType", (int)this.PLCConnectionType);
                //    myConnectionKey.SetValue("RoutingPLCConnectionType", (int)this.RoutingPLCConnectionType);
                //    myConnectionKey.SetValue("Timeout", this.Timeout);
                //    myConnectionKey.SetValue("TimeoutIPConnect", this.TimeoutIPConnect);
                //}
#endif
            }
        }

        public override string ToString()
        {
            string retVal = "";
            switch (ConnectionType)
            {
                case LibNodaveConnectionTypes.MPI_über_Serial_Adapter:
                    retVal = "MPI über seriell" + " (Port: " + ComPort + ", MPI: " + CpuMpi.ToString() + ")";
                    break;

                case LibNodaveConnectionTypes.MPI_über_Serial_Adapter_Andrews_Version_without_STX:
                    retVal = "MPI über seriell (Andrews Version)" + " (Port: " + ComPort + ", MPI: " + CpuMpi.ToString() + ")";
                    break;

                case LibNodaveConnectionTypes.MPI_über_Serial_Adapter_Step_7_Version:
                    retVal = "MPI über seriell (Step7 Version)" + " (Port: " + ComPort + ", MPI: " + CpuMpi.ToString() + ")";
                    break;

                case LibNodaveConnectionTypes.MPI_über_Serial_Adapter_Adrews_Version_with_STX:
                    retVal = "MPI über seriell" + " (Port: " + ComPort + ", MPI: " + CpuMpi.ToString() + ")";
                    break;

                case LibNodaveConnectionTypes.PPI_über_Serial_Adapter:
                    retVal = "PPI über seriell";
                    break;

                case LibNodaveConnectionTypes.AS_511:
                    retVal = "AS 511";
                    break;

                case LibNodaveConnectionTypes.Use_Step7_DLL:
                case LibNodaveConnectionTypes.Use_Step7_DLL_Without_TCP:
                    retVal = "Step7 DLL" + " (" + EntryPoint + ")";
                    break;

                case LibNodaveConnectionTypes.ISO_over_TCP:
                    retVal = "ISO over TCP" + " (IP:" + CpuIP.ToString() + ",Rack:" + CpuRack.ToString() + ",Slot:" + CpuSlot.ToString() + ")";
                    break;

                case LibNodaveConnectionTypes.ISO_over_TCP_CP_243:
                    retVal = "ISO over TCP (CP243)" + " (IP:" + CpuIP.ToString() + ",Rack:" + CpuRack.ToString() + ",Slot:" + CpuSlot.ToString() + ")";
                    break;

                case LibNodaveConnectionTypes.Netlink_lite:
                    retVal = "Netlink lite" + " (IP:" + CpuIP.ToString() + ")";
                    break;

                case LibNodaveConnectionTypes.Netlink_lite_PPI:
                    retVal = "Netlink lite PPI" + " (IP:" + CpuIP.ToString() + ")";
                    break;

                case LibNodaveConnectionTypes.Netlink_Pro:
                    retVal = "Netlink PRO" + " (IP:" + CpuIP.ToString() + ")";
                    break;

                case LibNodaveConnectionTypes.Fetch_Write_Active:
                    retVal = "Fetch/Write (Active)" + " (IP:" + CpuIP.ToString() + ", Port:" + Port + ", WritePort:" + WritePort + ")";
                    break;

                case LibNodaveConnectionTypes.Fetch_Write_Passive:
                    retVal = "Fetch/Write (Passive)" + " (IP:" + CpuIP.ToString() + ", Port:" + Port + ", WritePort:" + WritePort + ")";
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
        [XmlEnum("RegistrySavedConfiguration")]
        RegistrySavedConfiguration = 1,

        [XmlEnum("ObjectSavedConfiguration")]
        ObjectSavedConfiguration = 2
    }

    public enum LibNodaveConnectionTypes
    {
        // ReSharper disable InconsistentNaming
        [XmlEnum("0")]
        None = 0,

        /// <summary>
        /// Connection via MPI adapter connected to an Serial Com Port
        /// </summary>
        [XmlEnum("1")]
        MPI_über_Serial_Adapter = 1,

        [XmlEnum("2")]
        MPI_über_Serial_Adapter_Andrews_Version_without_STX = 2,

        [XmlEnum("3")]
        MPI_über_Serial_Adapter_Step_7_Version = 3,

        [XmlEnum("4")]
        MPI_über_Serial_Adapter_Adrews_Version_with_STX = 4,

        /// <summary>
        /// Connection via PPI protocoll of an MPI/PB adapter. This is usually used for S7-200 family
        /// </summary>
        [XmlEnum("10")]
        PPI_über_Serial_Adapter = 10,

        /// <summary>
        /// Connection via the AS511 Protocoll used by S5 series PLCs
        /// </summary>
        [XmlEnum("20")]
        AS_511 = 20,

        /// <summary>
        /// Connection via the Simatic Net libraries. Simatic Net must be installed
        /// </summary>
        [XmlEnum("50")]
        Use_Step7_DLL = 50,

        [XmlEnum("51")]
        Use_Step7_DLL_Without_TCP = 51,

        [XmlEnum("52")]
        Use_Step7_DLL_Automatic_TCP_Detection = 52,

        /// <summary>
        /// Connections via the TCP/IP protocoll
        /// </summary>
        [XmlEnum("122")]
        ISO_over_TCP = 122,

        /// <summary>
        /// Connections via TCP/IP to an CP243 for S7-200 series PLCs
        /// </summary>
        [XmlEnum("123")]
        ISO_over_TCP_CP_243 = 123,

        [XmlEnum("223")]
        Netlink_lite = 223,

        [XmlEnum("224")]
        Netlink_lite_PPI = 224,

        [XmlEnum("230")]
        Netlink_Pro = 230,

        [XmlEnum("9122")]
        ISO_over_TCP_Managed = 9122,

        [XmlEnum("500")]
        Fetch_Write_Active = 500,

        [XmlEnum("501")]
        Fetch_Write_Passive = 501,

        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    /// Communication Speeds for Profibus and MPI bus connection Adapters.
    /// The possible speeds can not be chosen freely, but are rather defined by the ProfiBus Standard
    /// </summary>
    public enum LibNodaveConnectionBusSpeed
    {
        // ReSharper disable InconsistentNaming
        /// <summary>
        /// 9.6 kbps
        /// </summary>
        [XmlEnum("0")]
        Speed_9k = 0,

        /// <summary>
        /// 19.2 kbps
        /// </summary>
        [XmlEnum("1")]
        Speed_19k = 1,

        /// <summary>
        /// 187,5 kbps
        /// </summary>
        [XmlEnum("2")]
        Speed_187k = 2,

        /// <summary>
        /// 0,5 Mbps
        /// </summary>
        [XmlEnum("3")]
        Speed_500k = 3,

        /// <summary>
        /// 1.5 Mbps
        /// </summary>
        [XmlEnum("4")]
        Speed_1500k = 4,

        /// <summary>
        /// 45.45 kbps
        /// </summary>
        [XmlEnum("5")]
        Speed_45k = 5,

        /// <summary>
        /// 93.75 kbps
        /// </summary>
        [XmlEnum("6")]
        Speed_93k = 6

        // ReSharper restore InconsistentNaming
    }

    public enum LibNodaveConnectionBusParity
    {
        [XmlEnum("101")]
        even = 'e',

        [XmlEnum("111")]
        odd = 'o',

        [XmlEnum("110")]
        none = 'n'
    }

    /// <summary>
    /// Defines the type of connection resource to use for communiction with the PLC
    /// Depending on the connection partner and connection type, the range of values is automatically limited to valid values or the value of the connection resource is assigned permanently.
    /// </summary>
    public enum LibNodaveConnectionResource
    {
        /// <summary>
        ///Unknown connection resource
        /// </summary>
        [XmlEnum("0")]
        unknown = 0,

        /// <summary>
        /// Programming device connection
        /// Free connection (not configured)
        /// At least one resource per CPU is reserved for programming device connections. However, for certain S7-300 CPUs it is possible to reserve multiple resources in the CPU properties.
        /// S7 connections that are typically set up from a programming device or from a PC (with ES functionality). This type of connection is used to configure and program the addressed station/module as well as to test and commission it;
        /// afterwards, the connection is typically cleared again. This connection resource allows both read and write access (e.g., monitoring and loading).
        /// </summary>
        [XmlEnum("1")]
        PG = 1,

        /// <summary>
        /// OP connection
        /// Free connection (not configured)
        /// At least one resource per CPU is reserved for OP connections. However, for certain S7-300 CPUs it is possible to reserve multiple resources in the CPU properties.
        /// S7 connections that are typically set up from an OP or from a PC (with OS functionality). This type of connection is used to monitor the addressed station/module with regard to the process that is being controlled.
        /// </summary>
        [XmlEnum("2")]
        OP = 2,

        /// <summary>
        /// Other
        /// Free connection (configured, unspecified connection)
        /// This connection resource can operate multiple connections. Use: Connection configured at one end with unspecified connection partner! The connection partner does not have to be configured if the connection resource 0x03 is addressed.
        /// Use is not specified. For example, this resource is used automatically when an S7 connection configured at both ends is configured from an S7-400 to an S7-300.
        /// </summary>
        [XmlEnum("3")]
        Other = 3,

        /// <summary>
        /// CPU
        /// Connections that are typically set up from a CPU to another module (CPU, FM, etc.) within a subnet. The connection setup is initiated by the application program,
        /// in which a connection configuration does not exist. This type of connection allows process data to be exchanged between the modules. For certain S7-300 CPUs, it is possible to reserve resources for S7 basic communication.
        /// </summary>
        [XmlEnum("253")]
        CPU = 253
    }
}