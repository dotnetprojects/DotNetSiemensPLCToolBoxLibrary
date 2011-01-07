/*
 This implements a high level Wrapper between libnodave.dll and applications written
 in MS .Net languages.
 
 This ConnectionLibrary was written by Jochen Kuehner
 * http://jfk-solutuions.de/
 * 
 * Thanks go to:
 * Steffen Krayer -> For his work on MC7 decoding and the Source for his Decoder
 * Zottel         -> For LibNoDave

 LibNoDaveConnectionLibrary is free software; you can redistribute it and/or modify
 it under the terms of the GNU Library General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 LibNoDaveConnectionLibrary is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Library General Public License
 along with Libnodave; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/
using System;
using Microsoft.Win32;

namespace LibNoDaveConnectionLibrary
{
#if !IPHONE
    [System.ComponentModel.Editor(typeof(LibNoDaveConnectionUITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
#endif
    [Serializable]
    public class LibNoDaveConnectionConfiguration
    {        
        public String ConnectionName { get; set; }
        public string EntryPoint { get; set; }
        public int CpuRack { get; set; }
        public int CpuSlot { get; set; }
        public int CpuMpi { get; set; }
        public string CpuIP { get; set; }
        public int Port { get; set; }
        public int LokalMpi { get; set; }
        public string ComPort { get; set; }
        public int ConnectionType { get; set; }
        public int BusSpeed { get; set; }
        public bool NetLinkReset { get; set; }
        public string ComPortSpeed { get; set; }
        public int ComPortParity { get; set; }

        public bool Routing { get; set;}
        public string RoutingDestination { get; set; }
        public int RoutingDestinationRack { get; set; }
        public int RoutingDestinationSlot { get; set; }
        public int RoutingSubnet1 { get; set; }
        public int RoutingSubnet2 { get; set; }

        public int Timeout { get; set; }
        public int TimeoutIPConnect { get; set; }

		private bool _initDone { get; set;}
		
        public LibNodaveConnectionConfigurationType ConfigurationType { get; set; }

        public static String[] GetConfigurationNames()
        {
#if !IPHONE
            RegistryKey myConnectionKey = Registry.CurrentUser.CreateSubKey("Software\\JFKSolutions\\LibNoDaveConnectionLibrary\\Connections");
            return myConnectionKey.GetSubKeyNames();  
#else
			return null;
#endif
        }

        /// <summary>
        /// Empty Constructor for Serialization
        /// </summary>
        public LibNoDaveConnectionConfiguration()
        { }

        /// <summary>
        /// Normal Constructor of the Config Object
        /// </summary>
        /// <param name="ConnectionName"></param>
        public LibNoDaveConnectionConfiguration(String ConnectionName)
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
        public LibNoDaveConnectionConfiguration(String ConnectionName, LibNodaveConnectionConfigurationType configurationType)
        {
            if (ConnectionName == "")
                this.ConnectionName = "tmpConnection1";
            else
                this.ConnectionName = ConnectionName;

            this.ConfigurationType = configurationType;

            this.ReloadConfiguration();
        }


        public void ReloadConfiguration()
        {
            if (ConfigurationType == LibNodaveConnectionConfigurationType.RegistrySavedConfiguration)
            {
#if !IPHONE
                RegistryKey myConnectionKey =
                    Registry.CurrentUser.CreateSubKey(
                        "Software\\JFKSolutions\\LibNoDaveConnectionLibrary\\Connections\\" + ConnectionName);
                if (myConnectionKey != null)
                {
                    this.EntryPoint = (String) myConnectionKey.GetValue("EntryPoint", "S7ONLINE");
                    this.CpuRack = Convert.ToInt32(myConnectionKey.GetValue("CpuRack", "0"));
                    this.CpuSlot = Convert.ToInt32(myConnectionKey.GetValue("CpuSlot", "2"));
                    this.CpuMpi = Convert.ToInt32(myConnectionKey.GetValue("CpuMpi", "2"));
                    this.CpuIP = (String) myConnectionKey.GetValue("CpuIP", "192.168.1.1");
                    this.LokalMpi = Convert.ToInt32(myConnectionKey.GetValue("LokalMpi", "0"));
                    this.ComPort = (String) myConnectionKey.GetValue("ComPort", "");
                    this.ConnectionType = Convert.ToInt32(myConnectionKey.GetValue("ConnectionType", "1"));
                    this.BusSpeed = Convert.ToInt32(myConnectionKey.GetValue("BusSpeed", "2"));
                    this.NetLinkReset = Convert.ToBoolean(myConnectionKey.GetValue("NetLinkReset", "false"));
                    this.ComPortSpeed = (String) myConnectionKey.GetValue("ComPortSpeed", "38400");
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
            Registry.CurrentUser.DeleteSubKeyTree(
                        "Software\\JFKSolutions\\LibNoDaveConnectionLibrary\\Connections\\" + ConnectionName);
            #endif 
        }

        public void SaveConfiguration()
        {
            if (ConfigurationType == LibNodaveConnectionConfigurationType.RegistrySavedConfiguration)
            {
#if !IPHONE
                RegistryKey myConnectionKey =
                    Registry.CurrentUser.CreateSubKey(
                        "Software\\JFKSolutions\\LibNoDaveConnectionLibrary\\Connections\\" + ConnectionName);
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
                    retVal = "MPI über seriell" + " (MPI: " + CpuMpi.ToString() + ")";
                    break;
                case 2:
                    retVal = "MPI über seriell (Andrews Version)" + " (MPI: " + CpuMpi.ToString() + ")";
                    break;
                case 3:
                    retVal = "MPI über seriell (Step7 Version)" + " (MPI: " + CpuMpi.ToString() + ")";
                    break;
                case 4:
                    retVal = "MPI über seriell" + " (MPI: " + CpuMpi.ToString() + ")";
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
    }

    public enum LibNodaveConnectionConfigurationType
    {
        RegistrySavedConfiguration=1,
        ObjectSavedConfiguration=2
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
