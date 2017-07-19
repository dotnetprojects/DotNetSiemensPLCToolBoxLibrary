using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Net;
using DotNetSiemensPLCToolBoxLibrary.Communication.S7_xxx;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Discovery
{
    public class S7ReachablePLCScanner
    {

        /// <summary>
        /// An new accessible PLC was found. 
        /// </summary>
        /// <remarks>
        ///  WARNING this event is not beeing raised on the UI Thread. to use on UI it must be dispached first
        /// </remarks>
        public event EventHandler<PlcFoundEventArgs> NewPlcFound;

        /// <summary>
        /// The scanning process has concluded
        /// </summary>
        /// <remarks>
        ///  WARNING this event is not beeing raised on the UI Thread. to use on UI it must be dispached first
        /// </remarks>
        public event EventHandler<EventArgs> ScanComplete;

        /// <summary>
        /// The scanning process has made an progress
        /// </summary>
        /// <remarks>
        ///  WARNING this event is not beeing raised on the UI Thread. to use on UI it must be dispached first
        /// </remarks>
        public event EventHandler<IPScanner.ProgressChangedEventArgs> ProgressChanged;


        private IPScanner _Scanner = new IPScanner();
        /// <summary>
        /// The scanning process is currently running
        /// </summary>
        /// <returns></returns>
        public bool isRunning
        {
            get { return _Scanner.isRunning; }
        }


        private List<FoundPlc> _DiscoveredPLCs = new List<FoundPlc>();
        /// <summary>
        /// An list of all found plcs during the last scanning
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<FoundPlc> DiscoveredPLCs
        {
            get
            {
                lock (_DiscoveredPLCs)
                {
                    return new ReadOnlyCollection<FoundPlc>(_DiscoveredPLCs);
                }
            }
        }

        public S7ReachablePLCScanner()
        {
            _Scanner.NewAdressFound += OnScanner_NewAdressFound;
            _Scanner.ScanComplete += OnScanner_ScanComplete;
            _Scanner.ProgressChanged += OnScanner_ProgressChanged;
        }

        /// <summary>
        /// Get all IP Addresses of Ethernet (Copper) and Wifi, that are currently up and connected. 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IPAddress> GetLocalIPAdressesV4()
        {
            List<IPAddress> List = new List<IPAddress>();

            foreach (var Iface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (Iface.NetworkInterfaceType == NetworkInterfaceType.Ethernet | Iface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    if (Iface.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (var Adr in Iface.GetIPProperties().UnicastAddresses)
                        {
                            if (Adr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                List.Add(Adr.Address);
                            }
                        }
                    }
                }
            }
            return List;
        }

        /// <summary>
        /// Start an new scann of available and Accesible PLCs
        /// This method is non blocking, and returns immeadiatly.
        /// </summary>
        /// <param name="ConnectionType"></param>
        public void BeginScan(LibNodaveConnectionTypes ConnectionType)
        {
            if (_Scanner.isRunning)
                throw new InvalidOperationException("An Scan is still running");

            if (!(ConnectionType == LibNodaveConnectionTypes.ISO_over_TCP) & !(ConnectionType == LibNodaveConnectionTypes.ISO_over_TCP_CP_243))
            {
                throw new NotImplementedException("Only ISO_over_TCP connections are available at the moment");
            }

            List<System.Net.IPEndPoint> AdressList = new List<System.Net.IPEndPoint>();

            //Build Adress List to Check
            foreach (IPAddress IP in GetLocalIPAdressesV4())
            {
                string BaseIP = IP.ToString().Remove(IP.ToString().LastIndexOf("."));
                for (int i = 1; i <= 255; i++)
                {
                    AdressList.Add(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(BaseIP + "." + i), 102));
                }
            }

            lock (_DiscoveredPLCs)
            {
                _DiscoveredPLCs.Clear();
            }

            //Search Adresses
            _Scanner.BeginScan(AdressList);
        }

        /// <summary>
        /// Wait until the Scanning process has finished
        /// </summary>
        public void EndScan()
        {
            _Scanner.EndScan();
        }

        public void AbortScan()
        {
            _Scanner.AbortScan();
        }

        public ReadOnlyCollection<FoundPlc> Scan(LibNodaveConnectionTypes ConnectionType)
        {
            BeginScan(ConnectionType);
            EndScan();
            return DiscoveredPLCs;
        }

        /// <summary>
        /// Go through all reasonable Configurations for S7 Connections, and return one that works
        /// </summary>
        /// <param name="plcConfig"></param>

        private void SearchCPUs(PLCConnectionConfiguration plcConfig)
        {
            //go through Slot 0 to 5 and return all found CPU's, this is because S7-400 may have several CPU's in different Slots in one rack
            for (int i = 0; i <= 5; i++)
            {
                PLCConnectionConfiguration Conf = new PLCConnectionConfiguration();
                Conf.ConnectionType = plcConfig.ConnectionType;
                Conf.CpuIP = plcConfig.CpuIP;
                Conf.Port = plcConfig.Port;
                Conf.CpuRack = plcConfig.CpuRack;
                Conf.CpuSlot = i;

                PLCConnection Con = new PLCConnection(Conf);
                try
                {
                    Con.Connect();

                    FoundPlc FoundPlc = new FoundPlc();
                    FoundPlc.S7ConnectionSettings = Conf;

                    //Read name of PLC
                    var HWInfos = Con.PLCGetSZL(0x1c, 0);

                    foreach (xy1CDataset HWInfo in HWInfos.SZLDaten)
                    {
                        switch (HWInfo.Index)
                        {
                            case 1:
                                FoundPlc.S7ConnectionSettings.ConnectionName = HWInfo.Text;
                                FoundPlc.PlcName = HWInfo.Text;
                                break;
                            case 2:
                                FoundPlc.ModuleName = HWInfo.Text;
                                break;
                            case 3:
                                FoundPlc.PlantIdentification = HWInfo.Text;
                                break;
                            case 4:
                                FoundPlc.Copyright = HWInfo.Text;
                                break;
                            case 5:
                                FoundPlc.ModuleSerialNumber = HWInfo.Text;
                                break;
                            case 6:
                                break;
                            case 7:
                                FoundPlc.ModuleTypeName = HWInfo.Text;
                                break;
                            case 8:
                                break;
                            case 9:
                                FoundPlc.Manufacturer = HWInfo.Text;
                                break;
                            case 10:
                                break;
                            case 11:
                                FoundPlc.Location = HWInfo.Text;
                                break;
                        }
                    }

                    //Check if we actually found an CPU
                    //This is because some S7-400 CPUs also accept connections to Modules other than CPU, such as CP or similiar. But
                    //these are not wat we are looking for.
                    if (!FoundPlc.ModuleTypeName.ToLower().Contains("cpu") && !FoundPlc.ModuleName.ToLower().Contains("cpu"))
                    {
                        Debug.WriteLine("Found an available interface that accepted the connection but apparently was not an CPU. The adress was {0} and the Slot was {1}", plcConfig.CpuIP, plcConfig.CpuSlot);
                        continue;
                    }

                    lock (_DiscoveredPLCs)
                    {
                        _DiscoveredPLCs.Add(FoundPlc);
                    }
                    NewPlcFound?.Invoke(this, new PlcFoundEventArgs { FoundPlc = FoundPlc });
                    System.Threading.Thread.Sleep(100); //Some CPUs, especially IBH Softec Simulatin CPUs dont like it when the request are coming to fast. IBH chrashes


                }
                catch (Exception ex)
                {
                    continue;//Ignore erorr and try next Slot
                }
                finally
                {
                    Con.Disconnect();
                }
            }
        }

        private void OnScanner_NewAdressFound(object sender, IPScanner.AdressFoundEventArgs e)
        {
            try
            {
                PLCConnectionConfiguration S7Setting = new PLCConnectionConfiguration();
                S7Setting.ConnectionType = LibNodaveConnectionTypes.ISO_over_TCP;
                S7Setting.Port = 102;
                S7Setting.CpuIP = e.Adress.Address.ToString();

                SearchCPUs(S7Setting);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error ocurred while an new PLC was found: {0}", ex.Message);
            }
        }

        private void OnScanner_ScanComplete(object sender, EventArgs e)
        {
            ScanComplete?.Invoke(this, e);
        }

        private void OnScanner_ProgressChanged(object sender, IPScanner.ProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }

        public class FoundPlc
        {
            public PLCConnectionConfiguration S7ConnectionSettings { get; set; }
            public string PlcName { get; set; }
            public string ModuleName { get; set; }
            public string PlantIdentification { get; set; }
            public string Copyright { get; set; }
            public string ModuleSerialNumber { get; set; }
            public string ModuleTypeName { get; set; }
            public string Manufacturer { get; set; }
            public string Location { get; set; }
        }

        public class PlcFoundEventArgs : EventArgs
        {
            public FoundPlc FoundPlc { get; set; }
        }
    }
}