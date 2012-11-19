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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Timers;
using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
//using DotNetSiemensPLCToolBoxLibrary.Communication.Library;
//using DotNetSiemensPLCToolBoxLibrary.Communication.Library.Interfaces;
using DotNetSiemensPLCToolBoxLibrary.Communication.S7_xxx;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.General;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;
using Microsoft.Win32;
using ThreadState = System.Threading.ThreadState;

/*
 * Todo: List Online Partners
 * Todo: (Maybe) Read the Routing SDB (write it also)
 * Todo: Memory of the CPU
 * Todo: Compress Memory
 */
namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
    public class PLCConnection : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<PLCTag> _writeQueue = new List<PLCTag>(); 

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        //Locking Object for Multithreaded Calls of LibNoDave...
        private static object _MultiThreadLocking = new object();

        private bool _NeedDispose = false;

        private bool _autoConnect = true;
        public bool AutoConnect
        {
            get { return _autoConnect; }
            set { _autoConnect = value; }
        }

        private PLCConnectionConfiguration _configuration;
        public PLCConnectionConfiguration Configuration
        {
            get { return _configuration; }            
        }

        private ConnectionTargetPLCType _connectionTargetPlcType;
        ConnectionTargetPLCType ConnectionTargetPLCType
        {
            get { return _connectionTargetPlcType; }            
        }

        public PLCConnection(String name)
        {
            if (name == "")
                throw new Exception("No Connection Name specified!");

            _configuration = new PLCConnectionConfiguration(name);

            _connectionTargetPlcType = ConnectionTargetPLCType.S7;
        }

        /// <summary>
        /// Constructor wich uses a LibNoDavaeConnectionConfiguration from outside.
        /// </summary>
        /// <param name="akConfig"></param>
        public PLCConnection(PLCConnectionConfiguration akConfig)
        {
            _configuration = akConfig;
        }


        private bool _connected;

        /// <summary>
        /// Is the Connection established?
        /// </summary>
        public bool Connected
        {
            get { return _connected; }
            set
            {
                _connected = value;
                NotifyPropertyChanged("Connected");
            }
        }

        public string Name
        {
            get { return Configuration.ConnectionName; }
        }

        private bool _netlinkReseted = false;

        //LibNoDave used types
        private libnodave.daveOSserialType _fds;
        private libnodave.daveInterface _di = null; //dave Interface
        public /*private*/ libnodave.daveConnection _dc = null;

        private System.Timers.Timer socketTimer;
        private Thread socketThread;
        
        void socketTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            socketTimer.Stop();
            socketThread.Abort();            
        }

        public void socket_Thread()
        {
            _fds.rfd = new IntPtr(-999);
            string ip = null;
            try
            {
                IPAddress[] addresslist = Dns.GetHostAddresses(_configuration.CpuIP);                
                foreach (var ipAddress in addresslist)
                {
                    if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        ip = ipAddress.ToString();
                        break;
                    }
                }
            }
            catch (Exception)
            { }

            if (ip != null)
                _fds.rfd = libnodave.openSocket(_configuration.Port, ip);
            else
                _fds.rfd = libnodave.openSocket(_configuration.Port, _configuration.CpuIP);
        }

        /// <summary>
        /// Connect to the PLC
        /// </summary>
        public void Connect()
        {
            _NeedDispose = true;
            //Debugging for LibNoDave
            libnodave.daveSetDebug(0x0);
            //libnodave.daveSetDebug(0x1ffff);

            //_configuration.ReloadConfiguration();

            //if (hwnd == 0 && _configuration.ConnectionType == 50)
            //    throw new Exception("Error: You can only use the S7Online Connection when you specify the HWND Parameter on the Connect Function");

            //This Jump mark is used when the Netlink Reset is activated!
        NLAgain:
            
            //LibNodave Verbindung aufbauen
            switch (_configuration.ConnectionType)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 10:
                    _fds.rfd = libnodave.setPort(_configuration.ComPort, _configuration.ComPortSpeed, _configuration.ComPortParity);
                    break;
                case 20:   //AS511            
                    _fds.rfd = libnodave.setPort(_configuration.ComPort, _configuration.ComPortSpeed, _configuration.ComPortParity);
                    break;
#if !IPHONE
                case 50:
                    _fds.rfd = libnodave.openS7online(_configuration.EntryPoint, 0);
                    if (_fds.rfd.ToInt32() == -1)
                    {
                        _NeedDispose = false;
                        throw new Exception("Error: " + libnodave.daveStrS7onlineError());
                    }
                    break;
#endif
                case 122:
                case 123:
                case 124:
                case 223:
                case 224:
                case 230:
                    socketTimer = new System.Timers.Timer(_configuration.TimeoutIPConnect);
                    socketTimer.Elapsed += new ElapsedEventHandler(socketTimer_Elapsed);
                    socketTimer.Start();
                    socketThread = new Thread(new ThreadStart(this.socket_Thread));
                    socketThread.Start();
                    while (socketThread.ThreadState==ThreadState.Running)
                    { }
                    socketTimer.Stop();
                    socketThread.Abort();
                    socketTimer = null;
                    socketThread = null;
                    //_fds.rfd = libnodave.openSocket(_configuration.Port, _configuration.CpuIP););
                    break;
            }

            if (_fds.rfd.ToInt32() == -999)
            {
                _NeedDispose = false;
                throw new Exception("Error: Timeout Connecting the IP");
            }

            if ((!(_configuration.ConnectionType == 50) && _fds.rfd.ToInt32() == 0) || _fds.rfd.ToInt32() < 0)
            {
                _NeedDispose = false;
                throw new Exception(
                    "Error: Could not creating the Physical Interface (Maybe wrong IP, COM-Port not Ready,...)");
            }

            //daveOSserialType Struktur befüllen
            _fds.wfd = _fds.rfd;

            //System.Windows.Forms.MessageBox.Show("Socket:" + _fds.rfd.ToString());

            if (_configuration.ConnectionName == null) _configuration.ConnectionName = Guid.NewGuid().ToString();

            //Dave Interface Erzeugen
            _di = new libnodave.daveInterface(_fds, _configuration.ConnectionName, _configuration.LokalMpi, _configuration.ConnectionType, _configuration.BusSpeed);

            //System.Windows.Forms.MessageBox.Show("DI:" + _di.ToString());

            //Timeout setzen...
            _di.setTimeout(_configuration.Timeout);

            //System.Windows.Forms.MessageBox.Show("Timeout gesetzt" + _di.ToString());

            //_di.setTimeout(500000);
            //Dave Interface initialisieren
            int ret = _di.initAdapter();
            if (ret != 0)
                throw new Exception("Error: " + libnodave.daveStrerror(ret));
            
            //System.Windows.Forms.MessageBox.Show("Adapter initialisiert" + _di.ToString());

            //Get S7OnlineType - To detect if is a IPConnection 
            bool IPConnection = false;
#if !IPHONE
            if (_configuration.ConnectionType == 50)
            {
                RegistryKey myConnectionKey =
                    Registry.LocalMachine.CreateSubKey("SOFTWARE\\Siemens\\SINEC\\LogNames\\" + _configuration.EntryPoint);
                string tmpDevice = (string)myConnectionKey.GetValue("LogDevice");
                string retVal = "";
                if (tmpDevice != "")
                {   
                    myConnectionKey =
                        Registry.LocalMachine.CreateSubKey("SOFTWARE\\Siemens\\SINEC\\LogDevices\\" + tmpDevice);
                    retVal = (string)myConnectionKey.GetValue("L4_PROTOCOL");
                }
                if (retVal == "TCPIP" || retVal == "ISO")
                    IPConnection = true;
            }
            //Get S7OnlineType - To detect if is a IPConnection
#endif


            //Connection aufbauen (Routing oder nicht...) (Bei IPConnection auch...)
            //if (_configuration.Routing || IPConnection)
            //Immer die extended Connection benutzen!
                _dc = new libnodave.daveConnection(_di, _configuration.CpuMpi, _configuration.CpuIP, IPConnection,
                                                   _configuration.CpuRack, _configuration.CpuSlot, _configuration.Routing,
                                                   _configuration.RoutingSubnet1, _configuration.RoutingSubnet2,
                                                   _configuration.RoutingDestinationRack, _configuration.RoutingDestinationSlot,
                                                   _configuration.RoutingDestination, _configuration.PLCConnectionType, _configuration.RoutingPLCConnectionType);
            
            //else
            //    _dc = new libnodave.daveConnection(_di, _configuration.CpuMpi, _configuration.CpuRack, _configuration.CpuSlot);

            if (_configuration.NetLinkReset && !_netlinkReseted && (_configuration.ConnectionType == 223 || _configuration.ConnectionType == 224))
            {
                _dc.resetIBH();
                _netlinkReseted = true;
                System.Threading.Thread.Sleep(1000);
                goto NLAgain;
            }

            ret = _dc.connectPLC();

            if (ret == -1)
            {
                _dc = null;
                throw new Exception("Error: CPU not available! Maybe wrong IP or MPI Address or Rack/Slot or ...");
            }
            if (ret != 0)
                throw new Exception("Error: " + libnodave.daveStrerror(ret));

            //System.Windows.Forms.MessageBox.Show("Connected" + ret.ToString());

            Connected = true;
        }

		/*
        private Interface myIf;
        private Connection myConn;
        public void ConnectTestwithNewInterface()
        {
            _NeedDispose = true;

            ConnectionConfig connConf = new ConnectionConfig();

            bool IPConnection = false;

            switch (_configuration.ConnectionType)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 10:
                    //_fds.rfd = libnodave.setPort(_configuration.ComPort, _configuration.ComPortSpeed, _configuration.ComPortParity);
                    break;
                case 20: //AS511            
                    //_fds.rfd = libnodave.setPort(_configuration.ComPort, _configuration.ComPortSpeed, _configuration.ComPortParity);
                    break;
                case 50:
                    RegistryKey myConnectionKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Siemens\\SINEC\\LogNames\\" + _configuration.EntryPoint);
                    string tmpDevice = (string) myConnectionKey.GetValue("LogDevice");
                    string retVal = "";
                    if (tmpDevice != "")
                    {
                        myConnectionKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Siemens\\SINEC\\LogDevices\\" + tmpDevice);
                        retVal = (string) myConnectionKey.GetValue("L4_PROTOCOL");
                    }
                    if (retVal == "TCPIP" || retVal == "ISO")
                        IPConnection = true;
                    myIf = new S7OnlineInterface(_configuration.EntryPoint);
                    break;
                case 122:
                case 123:
                case 124:
                case 223:
                case 224:
                    break;
                case 230:
                    IPConnection = true;
                    //_fds.rfd = libnodave.openSocket(_configuration.Port, _configuration.CpuIP););
                    break;
            }

            connConf.ConnectionToEthernet = IPConnection;
            if (IPConnection)
                connConf.IPAddress = IPAddress.Parse(_configuration.CpuIP);
            else
                connConf.MPIAddress = _configuration.CpuMpi;
            connConf.ConnectionType = _configuration.PLCConnectionType;
            connConf.Rack = _configuration.CpuRack;
            connConf.Slot = _configuration.CpuSlot;
            connConf.Routing = _configuration.Routing;
            connConf.RoutingConnectionType = _configuration.RoutingPLCConnectionType;
            connConf.RoutingToEthernet = _configuration.RoutingDestination.Length > 4;
            if (connConf.RoutingToEthernet)
                connConf.RoutingIPAddress = IPAddress.Parse(_configuration.RoutingDestination);
            else
                connConf.RoutingMPIAddres = int.Parse(_configuration.RoutingDestination);
            connConf.RoutingRack = _configuration.RoutingDestinationRack;
            connConf.RoutingSlot = _configuration.RoutingDestinationSlot;
            connConf.RoutingSubnet1 = _configuration.RoutingSubnet1;
            connConf.RoutingSubnet2 = _configuration.RoutingSubnet2;


            myConn = myIf.ConnectPlc(connConf);
        }
        */

        public void PLCStop()
        {
            if (AutoConnect && !Connected)
                Connect();

            if (_dc != null)
                lock (_dc)
                    _dc.stop();
        }

        public void PLCStart()
        {
            if (AutoConnect && !Connected)
                Connect();

            if (_dc != null)
                lock (_dc)
                    _dc.start();
        }

        public DateTime PLCReadTime()
        {
            if (AutoConnect && !Connected)
                Connect();

            if (_dc != null)
                lock (_dc)
                    return _dc.daveReadPLCTime();
            return DateTime.MinValue;
        }

        public void PLCSetTime(DateTime tm)
        {
            if (AutoConnect && !Connected)
                Connect();

            if (_dc != null)
                lock (_dc)
                    _dc.daveSetPLCTime(tm);
        }

        public class DiagnosticData : IDisposable
        {
            private bool IsAkku1Enabled { get { return (selRegister & S7FunctionBlockRow.SelectedStatusValues.Akku1) > 0; } }
            private bool IsAkku2Enabled { get { return (selRegister & S7FunctionBlockRow.SelectedStatusValues.Akku2) > 0; } }
            private bool IsAR1Enabled { get { return (selRegister & S7FunctionBlockRow.SelectedStatusValues.AR1) > 0; } }
            private bool IsAR2Enabled { get { return (selRegister & S7FunctionBlockRow.SelectedStatusValues.AR2) > 0; } }
            private bool IsDBEnabled { get { return (selRegister & S7FunctionBlockRow.SelectedStatusValues.DB) > 0; } }
            private bool IsDIEnabled { get { return (selRegister & S7FunctionBlockRow.SelectedStatusValues.DB) > 0; } }
            private bool IsSTWEnabled { get { return (selRegister & S7FunctionBlockRow.SelectedStatusValues.STW) > 0; } }

            internal S7FunctionBlock myBlock;
            internal S7FunctionBlockRow.SelectedStatusValues selRegister;
            internal Dictionary<int, List<S7FunctionBlockRow>> ByteAdressNumerPLCFunctionBlocks;

            //todo call parameters in diag
            //internal Dictionary<int, List<S7FunctionBlockParameter>> NumberOfCallParameter;

            internal short ReqestID;
            internal PLCConnection myConn;
            internal int readLineCounter;
            internal byte DiagDataTeletype;

            internal DiagnosticData()
            { }

            public void RequestDiagnosticData()
            {
                 if (myConn._dc != null)
                     lock (myConn._dc)
                     {
                         libnodave.PDU myPDU = new libnodave.PDU();

                         byte[] para;
                         byte[] data;

                         myPDU = new libnodave.PDU();
                         para = new byte[] {0x00, 0x01, 0x12, 0x08, 0x12, 0x41, 0x0e, 0x00, 0x00, 0x00, 0x00, 0x00};
                         data = new byte[]
                                    {
                                        0x00, 0x14, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00,
                                        0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00,
                                        BitConverter.GetBytes(ReqestID)[0], BitConverter.GetBytes(ReqestID)[1]
                                    };
                         myConn._dc.daveBuildAndSendPDU(myPDU, para, data);

                         byte[] rdata, rparam;
                         int res = myConn._dc.daveRecieveData(out rdata, out rparam);

                         if (rparam[10] == 0xd0 && rparam[11] == 0xa5)
                             throw new Exception("Error, the Commands are not excetuted");
                         else if (rparam[10] == 0xd0)
                             throw new Exception("Error, the Trigger is already in use. Err. Code: " +
                                                 rparam[11].ToString("X"));
                         else if (rparam[10] != 0x00)
                             throw new Exception("Error reading Diagnostic Data");
                         else if (rdata.Length < 14) //Function Block is not called!
                             return;

                         int answLen = rdata[6]*0x100 + rdata[7];

                         var prev = new S7FunctionBlockRow.BlockStatus();
                         int linenr = 14;

                         //In the 0x01 Telegramm, only Akku1 and 2 can be selected and STW is always Selected!
                         if (DiagDataTeletype == 0x01 &&
                             ((selRegister & S7FunctionBlockRow.SelectedStatusValues.Akku1) > 0 ||
                              (selRegister & S7FunctionBlockRow.SelectedStatusValues.Akku2) > 0))
                             selRegister |= S7FunctionBlockRow.SelectedStatusValues.Akku1 |
                                            S7FunctionBlockRow.SelectedStatusValues.Akku2;
                         if (DiagDataTeletype == 0x01)
                             selRegister |= S7FunctionBlockRow.SelectedStatusValues.STW;

                         prev = S7FunctionBlockRow.BlockStatus.ReadBlockStatus(rdata, linenr, selRegister, prev);

                         List<S7FunctionBlockRow> akRow;

                         //In 
                         if (ByteAdressNumerPLCFunctionBlocks.ContainsKey(0))
                         {
                             akRow = ByteAdressNumerPLCFunctionBlocks[0];
                             foreach (S7FunctionBlockRow tmp in akRow)
                                 tmp.ActualBlockStatus = prev;
                         }

                         linenr += S7FunctionBlockRow._GetCommandStatusAskSize(selRegister, DiagDataTeletype);
                         for (int n = 1; n <= readLineCounter; n++)
                         {

                             if (linenr >= rdata.Length)
                                 return;
                             int ByteRow = rdata[linenr]*0x100 + rdata[linenr + 1];
                             akRow = ByteAdressNumerPLCFunctionBlocks[ByteRow];

                             /*
                             PLCFunctionBlockRow.SelectedStatusValues akSelRegister = akRow[0]._GetCommandStatusAskValues(selRegister, DiagDataTeletype);

                             //If the akSelRegister for the Command is 0, set STW as Minimum
                             //This is neccessary, because we ask for a STW on a Line after a Jump, even if nothing should be requested!                        
                             if (akSelRegister == 0)
                                 akSelRegister = PLCFunctionBlockRow.SelectedStatusValues.STW;
                             */

                             S7FunctionBlockRow.SelectedStatusValues akSelRegister = akRow[0].askedStatusValues;

                             int akAskSize = S7FunctionBlockRow._GetCommandStatusAskSize(akSelRegister, DiagDataTeletype);

                             if (linenr + akAskSize - 14 > answLen)
                                 return;

                             prev = S7FunctionBlockRow.BlockStatus.ReadBlockStatus(rdata, linenr + 2, akSelRegister,
                                                                                   prev);
                             foreach (S7FunctionBlockRow tmp in akRow)
                                 tmp.ActualBlockStatus = prev;
                             linenr += akAskSize + 2;
                         }
                     }
            }

            public void RemoveDiagnosticData()
            {
                this.myBlock.DiagnosticData = null;

                foreach (var tmp in ByteAdressNumerPLCFunctionBlocks)
                    foreach (var tmp2 in tmp.Value)
                        tmp2.ActualBlockStatus = null;
            }

            private bool Closed;
            public void Close()
            {
                libnodave.PDU myPDU = new libnodave.PDU();

                byte[] para;
                byte[] data;

                myPDU = new libnodave.PDU();

                para = new byte[] { 0x00, 0x01, 0x12, 0x08, 0x12, 0x41, 0x0f, 0x00, 0x00, 0x00, 0x00, 0x00 };
                data = new byte[] { 0x00, 0x14, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, BitConverter.GetBytes(ReqestID)[0], BitConverter.GetBytes(ReqestID)[1] };
                myConn._dc.daveBuildAndSendPDU(myPDU, para, data);
            }

            public void Dispose()
            {
                if (!Closed)
                    Close();
            }
        }


        private byte GetByteForSelectedRegisters(S7FunctionBlockRow.SelectedStatusValues mySel, byte telegrammType)            
        {
            byte retval = 0x00;

            if (telegrammType == 0x13)
                return (byte) mySel;
            else
            {
                retval |= (mySel & S7FunctionBlockRow.SelectedStatusValues.STW) > 0 ? (byte)0x00 : (byte)0x00;
                retval |= (mySel & S7FunctionBlockRow.SelectedStatusValues.Akku1) > 0 ? (byte)0x01 : (byte)0x00;
                retval |= (mySel & S7FunctionBlockRow.SelectedStatusValues.Akku2) > 0 ? (byte)0x01 : (byte)0x00;
                retval |= (mySel & S7FunctionBlockRow.SelectedStatusValues.AR1) > 0 ? (byte)0x02 : (byte)0x00;
                retval |= (mySel & S7FunctionBlockRow.SelectedStatusValues.AR2) > 0 ? (byte)0x04 : (byte)0x00;
                retval |= (mySel & S7FunctionBlockRow.SelectedStatusValues.DB) > 0 ? (byte)0x08 : (byte)0x00;
                return retval;
            }
        }


        /// <summary>
        /// This Starts a Request for the Status of a PLC FunctionBlock
        /// </summary>
        /// <param name="myBlock"></param>
        /// <param name="StartAWLByteAdress"></param>
        /// <param name="selRegister"></param>
        /// <returns></returns>
        public DiagnosticData PLCstartRequestDiagnosticData(S7FunctionBlock myBlock, int StartAWLByteAdress, S7FunctionBlockRow.SelectedStatusValues selRegister /*Count of the Rows wich should be read, Registers wich should be read! */)
        {
            if (AutoConnect && !Connected)
                Connect();

            if (_dc != null)
                lock (_dc)
                {
                    if (selRegister == 0)
                        return null;

                    DiagnosticData retDiagnosticData = new DiagnosticData();
                    retDiagnosticData.myBlock = myBlock;
                    retDiagnosticData.selRegister = selRegister;
                    retDiagnosticData.myConn = this;

                    //This is the real byte address inside the Function
                    int rowByteAdress = 0;

                    //There are 2 Telegram types for Requesting Diag Data (0x01 and 0x13)
                    byte DiagDataTeletype = 0x01;

                    //Look in SZL wich Status Telegramm is supported!
                    SZLData szlData = PLCGetSZL(0x0131, 2);
                    SZLDataset[] szlDatasets = szlData.SZLDaten;
                    //if ((((DefaultSZLDataset)szlDatasets[0]).Bytes[4] & 0x08) > 0) //Byte 3 and 4 say as a Bit array wich Status Tele is supported!
                    if ((((xy31_2Dataset)szlDatasets[0]).funkt_2 & 0x08) > 0) //Byte 3 and 4 say as a Bit array wich Status Tele is supported!                     
                        DiagDataTeletype = 0x13;


                    //DiagDataTeletype = 0x01;

                    //len of the AnswBlock Block in the PDU
                    short answSize = (short)(S7FunctionBlockRow._GetCommandStatusAskSize(selRegister, DiagDataTeletype) + 2);


                    //Todo: Implement Callingpath
                    int askHeaderSize = 28; //This is 28 when no Callingpath is defined! (Callingpatth is not yet implemented)

                    //These are the Bytes in wich the Selected Registers for each Row are stored!
                    //When in a Row there is no Change for the selected Registers, these Row is added to the previous, so no extra Status for this row is necessary
                    List<byte> LinesSelectedRegisters = new List<byte>();
                    Dictionary<int, List<S7FunctionBlockRow>> ByteAdressNumerPLCFunctionBlocks = new Dictionary<int, List<S7FunctionBlockRow>>();

                    //Number of Lines wich are Read from the PLC
                    int cnt = 0;                    

                    //Adress of the lastByte wich was added to a Row
                    int lastByteAddress = 0;

                    //int askSize = 4; //This is minimum 4 (For the Start Registers)

                    S7FunctionBlockRow prevFunctionBlock = null;

                    foreach (S7FunctionBlockRow plcFunctionBlockRow in myBlock.AWLCode)
                    {
                        int commandSize = plcFunctionBlockRow.ByteSize;

                        if (commandSize > 0)
                        {
                            S7FunctionBlockRow.SelectedStatusValues askRegister = plcFunctionBlockRow._GetCommandStatusAskValues(selRegister, DiagDataTeletype);
                    
                            int akAskSize = S7FunctionBlockRow._GetCommandStatusAskSize(askRegister, DiagDataTeletype);

                            if ((answSize + akAskSize) < 182) //Max Size of the Answer Len! (In Step 7 this is 202 i think, have to try it...)
                            {                              
                                if (StartAWLByteAdress <= rowByteAdress)
                                {
                                    //Fill every asked Row with an empty status!
                                    plcFunctionBlockRow.ActualBlockStatus = new S7FunctionBlockRow.BlockStatus();
                                    
                                    if (Helper.IsJump(prevFunctionBlock, 0) && askRegister == 0)
                                        askRegister = S7FunctionBlockRow.SelectedStatusValues.STW;

                                    //When is jump target is true, we need to ask for every register again, don't know from where we jump!
                                    if (Helper.IsJumpTarget(plcFunctionBlockRow, myBlock))
                                        askRegister = selRegister;

                                    plcFunctionBlockRow.askedStatusValues = askRegister;

                                    akAskSize = S7FunctionBlockRow._GetCommandStatusAskSize(askRegister, DiagDataTeletype);

                                    /*
                                    if (akAskSize > 0 && plcFunctionBlockRow.Command == Call und parameter ist fc dann)
                                    {
                                     * The Status of a UC or CC should not be asked in the byte address, no it sould be asked at the Address of the SPA
                                     * a UC with FC <=255 is 2 Byte (+4 byte of the SPA), greater it's 4 byte (+4 byte of the SPA)
                                     * a UC/CC for a FB contains no SPA so this state should be asked directly!

                                    }
                                    
                                    else */ if (akAskSize > 0)
                                    {
                                        //if (akAskSize == 0)
                                        //    askRegister = PLCFunctionBlockRow.SelectedStatusValues.STW;

                                        lastByteAddress = rowByteAdress;

                                        var newLst = new List<S7FunctionBlockRow> { plcFunctionBlockRow };
                                        //This is a List of the Byte Address and the Coresponing Command List.
                                        //The Corresponding Commands are a List, because not every Command changes every Value,
                                        //So when not asked for all Registers, it can be that 2 commands have the same Status values and it's only asked for that one times!
                                        ByteAdressNumerPLCFunctionBlocks.Add(rowByteAdress, newLst);

                                        if (DiagDataTeletype == 0x13)
                                        {
                                            byte[] bt = BitConverter.GetBytes(rowByteAdress);
                                            //Ausgewählte Register für die zeile in die Abfrage eintragen
                                            byte askRegisterByte = GetByteForSelectedRegisters(askRegister, DiagDataTeletype);
                                            LinesSelectedRegisters.AddRange(new byte[] { bt[1], bt[0], 0x00, askRegisterByte });
                                            //askSize += 4;
                                        }
                                        else
                                        {
                                            byte askRegisterByte = GetByteForSelectedRegisters(askRegister, DiagDataTeletype);
                                            LinesSelectedRegisters.AddRange(new byte[] { 0x80, askRegisterByte });
                                            //askSize += 2;

                                            for (int g = 0; g < ((plcFunctionBlockRow.ByteSize - 2) / 2); g++)
                                            {
                                                byte wr = 0x00;
                                                //if (g % 2 != 0) wr = 0x80;
                                                LinesSelectedRegisters.AddRange(new byte[] { wr, wr });
                                                //askSize += 2;
                                            }
                                        }

                                        //Jede Anfrage braucht 4 Byte
                                        //askSize += 4;


                                        //Antwortgröße
                                        answSize += (short)(akAskSize + 2); //+2 for the Line Address
                                        //Anzahl der angefr. Zeilen!
                                        cnt++;
                                    }
                                    else
                                    {
                                        if (DiagDataTeletype == 0x01)
                                        {
                                            //Add for every Command, that asks for no Register a 0x80 0x80 to the ask Command!
                                            LinesSelectedRegisters.AddRange(new byte[] { 0x80, 0x80 });
                                            //askSize += 2;

                                            for (int g = 0; g < ((plcFunctionBlockRow.ByteSize - 2) / 2); g++)
                                            {
                                                byte wr = 0x00;
                                                //if (g % 2 != 0) wr = 0x80;
                                                LinesSelectedRegisters.AddRange(new byte[] { wr, wr });
                                                //askSize += 2;
                                            }
                                        }

                                        if (ByteAdressNumerPLCFunctionBlocks.Count == 0)
                                        {
                                            var newLst = new List<S7FunctionBlockRow> { plcFunctionBlockRow };
                                            ByteAdressNumerPLCFunctionBlocks.Add(rowByteAdress, newLst);
                                        }
                                        else
                                            ByteAdressNumerPLCFunctionBlocks[lastByteAddress].Add(plcFunctionBlockRow);
                                    }
                                }
                                rowByteAdress += commandSize;
                            }
                            else
                                break;
                        }

                        prevFunctionBlock = plcFunctionBlockRow;
                    }

                    int askSize = LinesSelectedRegisters.Count + 2;
                    if (DiagDataTeletype == 0x13)
                        askSize += 2;

                    if (answSize <= 0)
                        return null;

                    retDiagnosticData.ByteAdressNumerPLCFunctionBlocks = ByteAdressNumerPLCFunctionBlocks;
                    retDiagnosticData.readLineCounter = cnt;

                    libnodave.PDU myPDU = new libnodave.PDU();

                    //PDU Header
                    byte[] para;
                    //PDU Data
                    byte[] data;
                                                                            
                    para = new byte[] { 0x00, 0x01, 0x12, 0x08, 0x12, 0x41, DiagDataTeletype, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    byte[] tmp1;
                    if (DiagDataTeletype == 0x13)
                        tmp1 = new byte[] { BitConverter.GetBytes(askHeaderSize)[1], BitConverter.GetBytes(askHeaderSize)[0], BitConverter.GetBytes(askSize)[1], BitConverter.GetBytes(askSize)[0], 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, BitConverter.GetBytes(answSize)[1], BitConverter.GetBytes(answSize)[0], 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x06, 0x00, 0x00, Convert.ToByte(myBlock.BlockType), BitConverter.GetBytes(myBlock.BlockNumber)[1], BitConverter.GetBytes(myBlock.BlockNumber)[0], BitConverter.GetBytes(StartAWLByteAdress)[1], BitConverter.GetBytes(StartAWLByteAdress)[0], BitConverter.GetBytes(rowByteAdress - StartAWLByteAdress)[1], BitConverter.GetBytes(rowByteAdress - StartAWLByteAdress)[0], 0x80, Convert.ToByte(cnt), 0x00, GetByteForSelectedRegisters(selRegister, DiagDataTeletype) };
                    else
                        tmp1 = new byte[] {BitConverter.GetBytes(askHeaderSize)[1], BitConverter.GetBytes(askHeaderSize)[0], BitConverter.GetBytes(askSize)[1], BitConverter.GetBytes(askSize)[0], 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, BitConverter.GetBytes(answSize)[1], BitConverter.GetBytes(answSize)[0], 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x06, 0x00, 0x00, Convert.ToByte(myBlock.BlockType), BitConverter.GetBytes(myBlock.BlockNumber)[1], BitConverter.GetBytes(myBlock.BlockNumber)[0], BitConverter.GetBytes(StartAWLByteAdress)[1], BitConverter.GetBytes(StartAWLByteAdress)[0], BitConverter.GetBytes(rowByteAdress - StartAWLByteAdress)[1], BitConverter.GetBytes(rowByteAdress - StartAWLByteAdress)[0], 0x80, GetByteForSelectedRegisters(selRegister, DiagDataTeletype)};
                    data = Helper.CombineByteArray(tmp1, LinesSelectedRegisters.ToArray());
                    _dc.daveBuildAndSendPDU(myPDU, para, data);

                    byte[] rdata, rparam;
                    int res = _dc.daveGetPDUData(myPDU, out rdata, out rparam);

                    byte[] stid = new byte[] { rparam[6], rparam[7] };

                    if (rparam[10] != 0x00 && rparam[11] != 0x00) // 0xd05f
                        throw new Exception("Error Requesting Block Status, Error Code: 0x" + rparam[10].ToString("X").PadLeft(2, '0') + rparam[11].ToString("X").PadLeft(2, '0'));

                    retDiagnosticData.ReqestID = BitConverter.ToInt16(stid, 0);

                    retDiagnosticData.DiagDataTeletype = DiagDataTeletype;

                    myBlock.DiagnosticData = retDiagnosticData;
                    return retDiagnosticData;
                }
            return null;
        }

        public DataTypes.PLCState PLCGetSafetyStep()
        {
            if (AutoConnect && !Connected)
                Connect();

            if (_dc != null)
                lock (_dc)
                {
                    byte[] buffer = new byte[40];
                    int ret = _dc.readSZL(0x232, 4, buffer);

                    if (ret < 0)
                        throw new Exception("Error: " + libnodave.daveStrerror(ret));
                    if (buffer[1] != 0x04)
                        throw new WPFToolboxForSiemensPLCsException(WPFToolboxForSiemensPLCsExceptionType.ErrorReadingSZL);


                    //byte 2,3 betriebsartschalter
                    //    byte 4,5 schutzstufe
                }
            return 0;
        }

        public bool PLCSendPassword(string pwd)
        {
            if (AutoConnect && !Connected)
                Connect();

            if (_dc != null)
                lock (_dc)
                {
                    libnodave.PDU myPDU = new libnodave.PDU();

                    //PDU Header
                    byte[] para;
                    //PDU Data
                    byte[] data;

                    para = new byte[] { 0x00, 0x01, 0x12, 0x04, 0x11, 0x45, 0x01, 0x00 };
                    data = Helper.EncodePassword(pwd);
                    _dc.daveBuildAndSendPDU(myPDU, para, data);

                    byte[] rdata, rparam;
                    int res = _dc.daveGetPDUData(myPDU, out rdata, out rparam);

                    if (rparam[10] == 0xd6 && rparam[11] == 0x02)
                        return false;
                    //throw new Exception("Wrong Password");
                    return true;
                }
            return false;
        }

        public DataTypes.PLCState PLCGetState()
        {
            if (AutoConnect && !Connected)
                Connect();
            //Dokumentation of SZL can be found here: http://www.google.de/url?sa=t&source=web&cd=3&ved=0CCQQFjAC&url=http%3A%2F%2Fdce.felk.cvut.cz%2Frs%2Fplcs7315%2Fmanualy%2Fsfc_e.pdf&ei=tY8QTJufEYSNOLD_oMoH&usg=AFQjCNEHofHOLDcvGp-4eQBwlboKPu3oxQ
            if (_dc != null)
                lock (_dc)
                {
                    byte[] buffer = new byte[64]; 
                    int ret = _dc.readSZL(0x174, 4, buffer); //SZL 0x174 is for PLC LEDs

                    if (ret < 0)
                        throw new Exception("Error: " + libnodave.daveStrerror(ret));
                    if (buffer[10] == 1 && buffer[11] == 1)
                        return DataTypes.PLCState.Starting;
                    else if (buffer[10] == 1)
                        return DataTypes.PLCState.Running;
                    else
                        return DataTypes.PLCState.Stopped;
                }
            return DataTypes.PLCState.Stopped;
        }

        public List<string> PLCListBlocks(DataTypes.PLCBlockType myBlk)
        {
            if (AutoConnect && !Connected)
                Connect();

            if (_dc != null)
                lock (_dc)
                {
                    List<string> myRet = new List<string>();                    

                    byte[] blocks = new byte[2048 * 4];

                    //_configuration.ConnectionTyp

                    if (myBlk == DataTypes.PLCBlockType.AllBlocks && ConnectionTargetPLCType==ConnectionTargetPLCType.S7)                        
                        {
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.OB));
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.FC));
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.FB));
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.DB));
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.SFC));
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.SFB));
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.SDB));                            
                        }
                    else if (myBlk == DataTypes.PLCBlockType.AllEditableBlocks && ConnectionTargetPLCType == ConnectionTargetPLCType.S7)                        
                        {
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.OB));                            
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.FC));
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.FB));
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.DB));
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.SDB));               
                        }
                    else
                    {
                        int ret = _dc.ListBlocksOfType(Helper.GetPLCBlockTypeForBlockList(myBlk), blocks);
                        if (ret < 0 && ret != -53763  && ret != -53774 && ret != -255)
                            throw new Exception("Error: " + libnodave.daveStrerror(ret));
                        if (ret > 0)
                            for (int n = 0; n < ret * 4; n += 4)
                            {
                                int nr = blocks[n] + blocks[n + 1] * 256;
                                myRet.Add(myBlk.ToString() + nr.ToString());
                            }
                    }
                    return myRet;
                }
            return null;
        }

        public byte[] PLCGetBlockInMC7(string BlockName)
        {
            if (AutoConnect && !Connected)
                Connect();

            if (_dc != null)
                lock (_dc)
                {
                    //Todo: Better way to Split number and chars
                    byte[] buffer = new byte[65536 * 3];
                    string tmp = BlockName.ToUpper().Trim().Replace(" ", "");
                    string block = "";
                    int nr = 0;
                    if (tmp.Length > 3 && tmp.Substring(0, 3) == "SDB")
                    {
                        block = tmp.Substring(0, 3);
                        nr = Int32.Parse(tmp.Substring(3));                    
                    }
                    else
                    {
                        block = tmp.Substring(0, 2);
                        nr = Int32.Parse(tmp.Substring(2));                        
                    }
                    DataTypes.PLCBlockType blk = DataTypes.PLCBlockType.AllBlocks;

                    switch (block)
                    {
                        case "FC":
                            blk = DataTypes.PLCBlockType.FC;
                            break;
                        case "FB":
                            blk = DataTypes.PLCBlockType.FB;
                            break;
                        case "DB":
                            blk = DataTypes.PLCBlockType.DB;
                            break;
                        case "OB":
                            blk = DataTypes.PLCBlockType.OB;
                            break;
                        case "SDB":
                            blk = DataTypes.PLCBlockType.SDB;
                            break;
                    }
                    if (blk == DataTypes.PLCBlockType.AllBlocks || nr < 0)
                        throw new Exception("Unsupported Block Type!");

                    int readsize = buffer.Length;
                    int ret = _dc.getProgramBlock(Helper.GetPLCBlockTypeForBlockList(blk), nr, buffer, ref readsize);

                    if (ret == 0 && readsize > 0)
                    {
                        byte[] retVal = new byte[readsize];
                        Array.Copy(buffer, retVal, readsize);
                        return retVal;
                    }
                    else if (ret == 53825)
                        throw new Exception("PLC is Password Protected, unlock before downloading Blocks!");
                    else
                        return null;
                }
            return null;
        }

        public void PLCPutBlockFromMC7toPLC(string BlockName, byte[] buffer)
        {
            if (AutoConnect && !Connected)
                Connect();

            if (_dc != null)
                lock (_dc)
                {
                    string tmp = BlockName.ToUpper().Trim().Replace(" ", "");
                    string block = "";
                    int nr = 0;
                    if (tmp.Length > 3 && tmp.Substring(0, 3) == "SDB")
                    {
                        block = tmp.Substring(0, 3);
                        nr = Int32.Parse(tmp.Substring(3));
                    }
                    else
                    {
                        block = tmp.Substring(0, 2);
                        nr = Int32.Parse(tmp.Substring(2));
                    }
                    DataTypes.PLCBlockType blk = DataTypes.PLCBlockType.AllBlocks;

                    switch (block)
                    {
                        case "FC":
                            blk = DataTypes.PLCBlockType.FC;
                            break;
                        case "FB":
                            blk = DataTypes.PLCBlockType.FB;
                            break;
                        case "DB":
                            blk = DataTypes.PLCBlockType.DB;
                            break;
                        case "OB":
                            blk = DataTypes.PLCBlockType.OB;
                            break;
                        case "SDB":
                            blk = DataTypes.PLCBlockType.SDB;
                            break;
                    }
                    if (blk == DataTypes.PLCBlockType.AllBlocks || nr < 0)
                        throw new Exception("Unsupported Block Type!");

                    int readsize = buffer.Length;
                    int ret = _dc.putProgramBlock(Helper.GetPLCBlockTypeForBlockList(blk), nr, buffer, ref readsize);

                    if (ret == 0 && readsize > 0)
                    {
                        return;
                    }
                    else if (ret == 53825)
                        throw new Exception("PLC is Password Protected, unlock before downloading Blocks!");
                    else
                        throw new Exception("Error from PLC! Code: " + ret);
                }
            return;
        }
        public void PLCDeleteBlock(string BlockName)
        {
            if (AutoConnect && !Connected)
                Connect();

            if (_dc != null)
                lock (_dc)
                {
                    byte[] buffer = new byte[65536];
                    string tmp = BlockName.ToUpper().Trim().Replace(" ", "");
                    string block = tmp.Substring(0, 2);
                    int nr = Int32.Parse(tmp.Substring(2));
                    DataTypes.PLCBlockType blk = DataTypes.PLCBlockType.AllBlocks;

                    switch (block)
                    {
                        case "FC":
                            blk = DataTypes.PLCBlockType.FC;
                            break;
                        case "FB":
                            blk = DataTypes.PLCBlockType.FB;
                            break;
                        case "DB":
                            blk = DataTypes.PLCBlockType.DB;
                            break;
                        case "OB":
                            blk = DataTypes.PLCBlockType.OB;
                            break;
                    }
                    if (blk == DataTypes.PLCBlockType.AllBlocks || nr <= 0)
                        throw new Exception("Unsupported Block Type!");
                    _dc.deleteProgramBlock(Helper.GetPLCBlockTypeForBlockList(blk), nr);
                }
        }

        public SZLData PLCGetSZL(Int16 SZLNummer, Int16 Index)
        {
            if (AutoConnect && !Connected)
                Connect();

            if (_dc != null)
                lock (_dc)
                {
                    SZLData retVal = new SZLData();

                    byte[] buffer = new byte[65536];
                    int ret = _dc.readSZL(SZLNummer, Index, buffer);

                    //SZL:
                    //SZL-ID(WORD) 0,1
                    //INDEX(WORD) 2,3
                    //SIZE(WORD) 4,5
                    //COUNT(WORD) 6,7

                    if (ret < 0)
                        throw new Exception("Error: " + libnodave.daveStrerror(ret));

                    retVal.SzlId = (short) (buffer[1] + buffer[0]*256);
                    retVal.Index = (short) (buffer[3] + buffer[2]*256);
                    retVal.Size = (short) (buffer[5] + buffer[4]*256);
                    retVal.Count = (short) (buffer[7] + buffer[6]*256);

                    List<SZLDataset> datsets = new List<SZLDataset>();
                  
                    for (int n = 0; n < retVal.Count; n++)
                    {
                        byte[] objBuffer = new byte[retVal.Size];
                        Array.Copy(buffer, (n*retVal.Size) + 8, objBuffer, 0, retVal.Size);
                        //GCHandle handle = GCHandle.Alloc(objBuffer, GCHandleType.Pinned);

                        switch (retVal.SzlId & 0x00ff)
                        {
                            case 0x0000:
                                datsets.Add(EndianessMarshaler.BytesToStruct<xy00Dataset>(objBuffer));
                                break;
                            case 0x0011:
                                datsets.Add(EndianessMarshaler.BytesToStruct<xy11Dataset>(objBuffer));
                                break;
                            case 0x0012:
                                datsets.Add(EndianessMarshaler.BytesToStruct<xy12Dataset>(objBuffer));
                                break;
                            case 0x0013:
                                datsets.Add(EndianessMarshaler.BytesToStruct<xy13Dataset>(objBuffer));
                                break;
                            case 0x0014:
                                datsets.Add(EndianessMarshaler.BytesToStruct<xy14Dataset>(objBuffer));
                                break;
                            case 0x0015:
                                datsets.Add(EndianessMarshaler.BytesToStruct<xy15Dataset>(objBuffer));
                                break;
                            case 0x0016:
                                datsets.Add(EndianessMarshaler.BytesToStruct<xy16Dataset>(objBuffer));
                                break;
                            case 0x0017:
                                datsets.Add(EndianessMarshaler.BytesToStruct<xy17Dataset>(objBuffer));
                                break;
                            case 0x0018:
                                datsets.Add(EndianessMarshaler.BytesToStruct<xy18Dataset>(objBuffer));
                                break;
                            case 0x0019:
                                datsets.Add(EndianessMarshaler.BytesToStruct<xy19Dataset>(objBuffer));
                                break;
                            case 0x0021:
                                datsets.Add(EndianessMarshaler.BytesToStruct<xy21Dataset>(objBuffer));
                                break;
                            case 0x001C:
                                datsets.Add(EndianessMarshaler.BytesToStruct<xy1CDataset>(objBuffer));
                                break;
                            case 0x0022:
                                datsets.Add(EndianessMarshaler.BytesToStruct<xy22Dataset>(objBuffer));
                                break;
                            case 0x0023:
                                datsets.Add(EndianessMarshaler.BytesToStruct<xy23Dataset>(objBuffer));
                                break;
                            case 0x0024:
                                datsets.Add(EndianessMarshaler.BytesToStruct<xy24Dataset>(objBuffer));
                                break;
                            case 0x0025:
                                datsets.Add(EndianessMarshaler.BytesToStruct<xy25Dataset>(objBuffer));
                                break;
                            case 0x0031:
                                switch (retVal.Index)
                                {
                                    case 1:
                                        datsets.Add(EndianessMarshaler.BytesToStruct<xy31_1Dataset>(objBuffer));
                                        break;
                                    case 2:
                                        datsets.Add(EndianessMarshaler.BytesToStruct<xy31_2Dataset>(objBuffer));
                                        break;
                                    case 3:
                                        datsets.Add(EndianessMarshaler.BytesToStruct<xy31_3Dataset>(objBuffer));
                                        break;
                                    case 4:
                                        datsets.Add(EndianessMarshaler.BytesToStruct<xy31_4Dataset>(objBuffer));
                                        break;
                                    case 5:
                                        datsets.Add(EndianessMarshaler.BytesToStruct<xy31_5Dataset>(objBuffer));
                                        break;
                                    case 6:
                                        datsets.Add(EndianessMarshaler.BytesToStruct<xy31_6Dataset>(objBuffer));
                                        break;                                   
                                    default:
                                        {
                                            DefaultSZLDataset tmp = new DefaultSZLDataset();
                                            tmp.Bytes = objBuffer;
                                            datsets.Add(tmp);
                                        }
                                        break;
                                }
                                break;
                            case 0x0032:
                                switch (retVal.Index)
                                {
                                    case 1:
                                        datsets.Add(EndianessMarshaler.BytesToStruct<xy32_1Dataset>(objBuffer));
                                        break;                                    
                                    default:
                                        {
                                            DefaultSZLDataset tmp = new DefaultSZLDataset();
                                            tmp.Bytes = objBuffer;
                                            datsets.Add(tmp);
                                        }
                                        break;
                                }
                                break;
                            case 0x0071:
                                datsets.Add(EndianessMarshaler.BytesToStruct<xy71Dataset>(objBuffer));
                                break;
                            case 0x0074:
                                datsets.Add(EndianessMarshaler.BytesToStruct<xy74Dataset>(objBuffer));
                                break;
                            default:
                                {
                                    DefaultSZLDataset tmp = new DefaultSZLDataset();
                                    tmp.Bytes = objBuffer;
                                    datsets.Add(tmp);                                   
                                }
                                break;
                        }

                        //handle.Free();
                    }

                    retVal.SZLDaten = datsets.ToArray();

                    return retVal;
                }
            return null;
        }




        public List<DataTypes.DiagnosticEntry> PLCGetDiagnosticBuffer()
        {
            if (AutoConnect && !Connected)
                Connect();

            if (_dc != null)
                lock (_dc)
                {
                    List<DataTypes.DiagnosticEntry> retVal = new List<DataTypes.DiagnosticEntry>();
                    byte[] buffer = new byte[65536];
                    int ret = _dc.readSZL(0xA0, 0, buffer);

                    //SZL:
                    //SZL-ID(WORD) 0,1
                    //INDEX(WORD) 2,3
                    //SIZE(WORD) 4,5
                    //COUNT(WORD) 6,7

                    if (ret < 0)
                        throw new Exception("Error: " + libnodave.daveStrerror(ret));

                    int cnt = buffer[7] + buffer[6] * 256;

                    if (cnt > 10000) cnt = 100;

                    for (int n = 0; n < cnt; n++)
                    {
                        byte[] diagData = new byte[20];
                        Array.Copy(buffer, n * 20 + 8, diagData, 0, 20);

                        DataTypes.DiagnosticEntry myEntr = new DataTypes.DiagnosticEntry(diagData);
                        retVal.Add(myEntr);
                    }

                    return retVal;
                }
            return null;
        }

        public DataTypes.PLCMemoryInfo PLCGetMemoryInfo()
        {
            if (AutoConnect && !Connected)
                Connect();

            if (_dc != null)
                lock (_dc)
                {
                    /*
                    List<LibNoDaveDataTypes.DiagnosticEntry> retVal = new List<LibNoDaveDataTypes.DiagnosticEntry>();
                    byte[] buffer = new byte[65536];
                    int ret = _dc.readSZL(0xA0, 0, buffer);

                    if (ret < 0)
                        throw new Exception("Error: " + libnodave.daveStrerror(ret));

                    int cnt = buffer[7] + buffer[6] * 256;

                    if (cnt > 10000) cnt = 100;

                    for (int n = 0; n < cnt; n++)
                    {
                        int nr = buffer[n * 20 + 8] * 256 + buffer[n * 20 + 9];
                        LibNoDaveDataTypes.DiagnosticEntry myEntr = new LibNoDaveDataTypes.DiagnosticEntry(nr);
                        myEntr.TimeStamp = libnodave.getDateTimefrom(buffer, n * 20 + 20);
                        retVal.Add(myEntr);
                    }

                    return retVal;
                    */
                }
            return null;
        }
        
        public class VarTabReadData : IDisposable
        {
            internal short ReqestID;
            internal PLCConnection myConn;
            
            private PLCTag[] _plcTags;
            public PLCTag[] PLCTags
            {
                get { return _plcTags; }                
            }

            internal VarTabReadData(short reqId, PLCTag[] plcTags, PLCConnection conn)
            {
                ReqestID = reqId;
                _plcTags = plcTags;
                myConn = conn;
            }

            public void RequestData()
            {
                if (myConn._dc != null)
                    lock (myConn._dc)
                    {
                        libnodave.PDU myPDU = new libnodave.PDU();

                        byte[] para;
                        byte[] data;

                        myPDU = new libnodave.PDU();
                        para = new byte[] { 0x00, 0x01, 0x12, 0x08, 0x12, 0x41, 0x0e, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        data = new byte[]
                                    {
                                        0x00, 0x14, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00,
                                        0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00,
                                        BitConverter.GetBytes(ReqestID)[0], BitConverter.GetBytes(ReqestID)[1]
                                    };
                        myConn._dc.daveBuildAndSendPDU(myPDU, para, data);

                        byte[] rdata, rparam;
                        int res = myConn._dc.daveRecieveData(out rdata, out rparam);

                        //Todo: Look what error codes exist when using vartab, the following ones are from Diag Data!
                        if (rparam[10] == 0xd0 && rparam[11] == 0xa5)
                            throw new Exception("The Trigger Situation has not yet happened!");
                        else if (rparam[10] == 0xd0)
                            throw new Exception("Error, the Trigger is already in use. Err. Code: " +
                                                rparam[11].ToString("X"));
                        else if (rparam[10] != 0x00)
                            throw new Exception("Error reading Diagnostic Data");
                        else if (rdata.Length < 14) //Function Block is not called!
                            return;

                        int answLen = rdata[6] * 0x100 + rdata[7];

                        int pos = 14;
                        for (int i = 0; i < PLCTags.Length; i++)
                        {
                            int len = 0;
                            if (rdata[pos + 0] == 0xff)
                            {
                                //rdata[pos + 1] == 4 means len is in BITS, maybe we need this???
                                len = rdata[pos + 2]*0x100 + rdata[pos + 3];
                                if (len < PLCTags[i].ReadByteSize)
                                    throw new Exception("The Tag for the VarTabRead Function was to huge, " + PLCTags[i].ReadByteSize + " Bytes should be read, but only " + len + " Bytes were read!");
                                if (len % 2 != 0) len++;
                                PLCTags[i]._readValueFromBuffer(rdata, pos + 4);
                            }
                            else                            
                                PLCTags[i].ItemDoesNotExist = true;                           
                            pos += 4 + len;
                        }                       
                    }
            }

            private bool Closed;
            public void Close()
            {
                libnodave.PDU myPDU = new libnodave.PDU();

                byte[] para;
                byte[] data;

                para = new byte[] { 0x00, 0x01, 0x12, 0x08, 0x12, 0x41, 0x0f, 0x00, 0x00, 0x00, 0x00, 0x00 };
                data = new byte[] { 0x00, 0x14, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, BitConverter.GetBytes(ReqestID)[0], BitConverter.GetBytes(ReqestID)[1] };

                myPDU = new libnodave.PDU();
                myConn._dc.daveBuildAndSendPDU(myPDU, para, data);
            }

            public void Dispose()
            {
                if (!Closed)
                    Close();
            }
        }


        public VarTabReadData ReadValuesWithVarTabFunctions(IEnumerable<PLCTag> valueList, PLCTriggerVarTab ReadTrigger)
        {
            int anzZeilen = 0;

            List<byte> askBytes = new List<byte>();

            foreach (PLCTag plcTag in valueList)
            {
                int dtaTyp = 0;
                int dtaSize = 0;
                int dtaArrSize = 1;
                int dbNo = 0;
                
                byte[] akAsk = new byte[6];

                switch (plcTag.LibNoDaveDataSource)
                {
                    case TagDataSource.Flags:
                        dtaTyp = 0;
                        break;
                    case TagDataSource.Inputs:
                        dtaTyp = 1;
                        break;
                    case TagDataSource.Outputs:
                        dtaTyp = 2;
                        break;
                    case TagDataSource.Datablock:
                        dtaTyp = 0x07;
                        dbNo = plcTag.DatablockNumber;
                        break;
                    case TagDataSource.Timer:
                        dtaTyp = 5;
                        break;
                    case TagDataSource.Counter:
                        dtaTyp = 6;
                        break;
                    case TagDataSource.LocalData:
                        dtaTyp = 0x0c;
                        break;
                }

                switch (plcTag.ReadByteSize)
                {
                    case 1:
                        dtaSize = 1;
                        break;
                    case 2:
                        dtaSize = 2;
                        break;
                    case 4:
                        dtaSize = 3;
                        break;
                    default:
                        if (plcTag.LibNoDaveDataSource == TagDataSource.Timer || plcTag.LibNoDaveDataSource == TagDataSource.Counter)
                        {
                            dtaSize = 4;
                            dbNo = 1;
                        }
                        else
                        {
                            dtaArrSize = plcTag.ReadByteSize;
                            dtaSize = 1;
                        }
                        break;
                }

                akAsk[0] = (byte) (dtaTyp*0x10 + dtaSize);
                akAsk[1] = (byte) dtaArrSize;
                akAsk[2] = (byte) (dbNo/0x100);
                akAsk[3] = (byte) (dbNo%0x100);
                akAsk[4] = (byte) (plcTag.ByteAddress/0x100);
                akAsk[5] = (byte) (plcTag.ByteAddress%0x100);

                askBytes.AddRange(akAsk);

                anzZeilen++;
            }

            int len1 = anzZeilen*6 + 2;
            
            libnodave.PDU myPDU = new libnodave.PDU();

            byte[] para;
            byte[] data;

            //myPDU = new libnodave.PDU();
            
            para = new byte[] { 0x00, 0x01, 0x12, 0x08, 0x12, 0x41, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00 };
            data = new byte[]
                                    {
                                        0x00, 0x14,  BitConverter.GetBytes(len1)[1],  BitConverter.GetBytes(len1)[0], 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x0e, 0x00, 0x01, 
                                        0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, (byte)ReadTrigger, 0x00, BitConverter.GetBytes(anzZeilen)[1], BitConverter.GetBytes(anzZeilen)[0], 
                                        //0x01, 0x08, 0x00, 0x00, 0x00, 0x00 //Tag
                                        
                                    };

            data = Helper.CombineByteArray(data, askBytes.ToArray());

            _dc.daveBuildAndSendPDU(myPDU, para, data);

            byte[] rdata, rparam;
            int res = _dc.daveRecieveData(out rdata, out rparam);

            byte[] stid = new byte[] { rparam[6], rparam[7] };

            if (rparam[10] != 0x00 && rparam[11] != 0x00) // 0xd05f
                throw new Exception("Error Reading Tags with Var Tab Functions, Error Code: 0x" + rparam[10].ToString("X").PadLeft(2, '0') + rparam[11].ToString("X").PadLeft(2, '0'));

            VarTabReadData retVal = new VarTabReadData(BitConverter.ToInt16(stid, 0), General.IEnumerableExtensions.ToArray<PLCTag>(valueList), this);

            return retVal;
        }

        public void WriteValuesWithVarTabFunctions(IEnumerable<PLCTag> valueList, PLCTriggerVarTab WriteTrigger)
        {
            int anzZeilen = 0;

            List<byte> controlBytes = new List<byte>();
            List<byte> controlValues = new List<byte>();

            foreach (PLCTag plcTag in valueList)
            {
                int dtaTyp = 0;
                int dtaSize = 0;
                int dtaArrSize = 1;
                int dbNo = 0;

                byte[] akAsk = new byte[6];

                switch (plcTag.LibNoDaveDataSource)
                {
                    case TagDataSource.Flags:
                        dtaTyp = 0;
                        break;
                    case TagDataSource.Inputs:
                        dtaTyp = 1;
                        break;
                    case TagDataSource.Outputs:
                        dtaTyp = 2;
                        break;
                    case TagDataSource.Datablock:
                        dtaTyp = 0x07;
                        dbNo = plcTag.DatablockNumber;
                        break;
                    case TagDataSource.Timer:
                        dtaTyp = 5;
                        break;
                    case TagDataSource.Counter:
                        dtaTyp = 6;
                        break;
                    case TagDataSource.LocalData:
                        dtaTyp = 0x0c;
                        break;
                }

                switch (plcTag.ReadByteSize)
                {
                    case 1:
                        dtaSize = 1;
                        break;
                    case 2:
                        dtaSize = 2;
                        break;
                    case 4:
                        dtaSize = 3;
                        break;
                    default:
                        if (plcTag.LibNoDaveDataSource == TagDataSource.Timer || plcTag.LibNoDaveDataSource == TagDataSource.Counter)
                        {
                            dtaSize = 4;
                            dbNo = 1;
                        }
                        else
                        {
                            dtaArrSize = plcTag.ReadByteSize;
                            dtaSize = 1;
                        }
                        break;
                }

                akAsk[0] = (byte)(dtaTyp * 0x10 + dtaSize);
                akAsk[1] = (byte)dtaArrSize;
                akAsk[2] = (byte)(dbNo / 0x100);
                akAsk[3] = (byte)(dbNo % 0x100);
                akAsk[4] = (byte)(plcTag.ByteAddress / 0x100);
                akAsk[5] = (byte)(plcTag.ByteAddress % 0x100);

                controlBytes.AddRange(akAsk);

                //Add Control Values ....
                controlValues.AddRange(new byte[] { 0x00, 0x09, (byte)(plcTag.ReadByteSize / 0x100), (byte)(plcTag.ReadByteSize % 0x100) });
                byte[] ctrl = new byte[plcTag.ReadByteSize];
                plcTag._putControlValueIntoBuffer(ctrl, 0);
                controlValues.AddRange(ctrl);

                anzZeilen++;
            }

            int len1 = anzZeilen*6 + 2 + controlValues.Count;

            libnodave.PDU myPDU = new libnodave.PDU();

            byte[] para;
            byte[] data;

            myPDU = new libnodave.PDU();

            para = new byte[] { 0x00, 0x01, 0x12, 0x08, 0x12, 0x41, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00 };
            data = new byte[]
                                    {                                                                                                   //1 means use Trigger? maybe
                                        0x00, 0x14,  BitConverter.GetBytes(len1)[1],  BitConverter.GetBytes(len1)[0], 0x00, 0x00, 0x00, /* 0x00 */ 0x01, 0x00, 0x01, 0x00, 0x04, 0x00, 0x01, 
                                        0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, /*0x00*/ (byte)WriteTrigger, 0x00, BitConverter.GetBytes(anzZeilen)[1], BitConverter.GetBytes(anzZeilen)[0], 
                                        //0x01, 0x08, 0x00, 0x00, 0x00, 0x00 //Tag
                                        
                                    };

            data = Helper.CombineByteArray(data, controlBytes.ToArray());
            data = Helper.CombineByteArray(data, controlValues.ToArray());

            _dc.daveBuildAndSendPDU(myPDU, para, data);

            byte[] rdata, rparam;
            int res = _dc.daveRecieveData(out rdata, out rparam);

            byte[] stid = new byte[] { rparam[6], rparam[7] };

            if (rparam[10] != 0x00 && rparam[11] != 0x00) // 0xd05f
                throw new Exception("Error Wrting Tags with VarTab Functions, Error Code: 0x" + rparam[10].ToString("X").PadLeft(2,'0') + rparam[11].ToString("X").PadLeft(2,'0'));
            /*
            VarTabWriteData retVal = new VarTabWriteData(BitConverter.ToInt16(stid, 0), General.IEnumerableExtensions.ToArray<PLCTag>(valueList), this);

            return retVal;
            */
        }

        //Helper for Readvalues
        //Sort the PLC TAGs
        private class SorterForPLCTags : IComparer<PLCTag>
        {
            public int Compare(PLCTag pt1, PLCTag pt2)
            {
               //Beides DBs, vergleiche DBs
                if (pt1.LibNoDaveDataSource==TagDataSource.Datablock && pt2.LibNoDaveDataSource==TagDataSource.Datablock)
                {
                    if (pt1.DatablockNumber == pt2.DatablockNumber)
                        if (pt1.ByteAddress > pt2.ByteAddress)
                            return 1;
                        else if (pt1.ByteAddress < pt2.ByteAddress)
                            return -1;
                        else
                            return 0;
                    else
                        if (pt1.DatablockNumber > pt2.DatablockNumber)
                            return 1;
                        else
                            return -1;

                    return 0;
                }
                //nicht beides DBs, vergleiche nur DataSource und danach Byte Adresse
                else
                {
                    if (pt1.LibNoDaveDataSource == pt2.LibNoDaveDataSource)
                        if (pt1.ByteAddress > pt2.ByteAddress)
                            return 1;
                        else if (pt1.ByteAddress < pt2.ByteAddress)
                            return -1;
                        else
                            return 0;
                    else
                        if (pt1.LibNoDaveDataSource > pt2.LibNoDaveDataSource)
                            return 1;
                        else
                            return -1;

                }
            }           
        }

        /// <summary>
        /// This Function Reads Values from the PLC it needs a Array of LibNodaveValues
        /// It tries to Optimize how the Values are Read from the PLC
        /// </summary>
        /// <param name="valueList"></param>        
        public void ReadValues(IEnumerable<PLCTag> valueList)
        {

            //Got through the list of values
            //Order them at first with the DB, then the byte address
            //If the Byte count of a tag is uneven, add 1
            //Then Look if Some Values lay in othe values or if the byte adress difference is <= 4
            //if it is so, create a replacement value wich reads the bytes and stores at wich tags are in this value and at wich adress
            //read the tags!
            //Look, that the byte count gets not bigger than a pdu!


            if (AutoConnect && !Connected)
                Connect();

            if (_dc != null)
                lock (_dc)
                {

                    //IEnumerable<PLCTag> readTagList = valueList;


                    #region Optimize Reading List....                    
                    List<PLCTag> orderedList = new List<PLCTag>();
                    orderedList.AddRange(valueList);
                    orderedList.Sort(new SorterForPLCTags());

                    List<PLCTag> readTagList=new List<PLCTag>(); 

                    //Go through the List of PLC Tags and Combine the ones, where the Byte Addres does not differ more than 4 Bytes...
                    TagDataSource oldDataSource = 0;
                    int oldDB=0, oldByteAddress=0, oldLen=0;
                    int cntCombinedTags = 0;
                    PLCTag lastTag = null;
                    PLCTagReadHelper rdHlp = new PLCTagReadHelper() { LibNoDaveDataType = TagDataType.ByteArray };
                    foreach (PLCTag plcTag in orderedList)
                    {
                        if (cntCombinedTags == 0)
                        {
                            oldDataSource = plcTag.LibNoDaveDataSource;
                            oldDB = plcTag.DatablockNumber;
                            oldByteAddress = plcTag.ByteAddress;
                            oldLen = plcTag._internalGetSize();
                            lastTag = plcTag;
                            cntCombinedTags++;
                        }
                        else
                        {
                            if (oldDataSource == plcTag.LibNoDaveDataSource && (oldDataSource != TagDataSource.Datablock || oldDB == plcTag.DatablockNumber) && plcTag.ByteAddress <= oldByteAddress + oldLen + 4)
                            {
                                //todo: test if this is correct
                                if (cntCombinedTags == 1)
                                    rdHlp.PLCTags.Add(lastTag, 0);
                                
                                cntCombinedTags++;
                                int newlen = plcTag._internalGetSize() + (plcTag.ByteAddress - oldByteAddress);
                                oldLen = oldLen < newlen ? newlen : oldLen;
                                if (oldLen % 2 != 0) oldLen++;
                                rdHlp.PLCTags.Add(plcTag, plcTag.ByteAddress - oldByteAddress);
                                rdHlp.ByteAddress = oldByteAddress;
                                rdHlp.ArraySize = oldLen;
                                rdHlp.LibNoDaveDataSource = oldDataSource;
                                rdHlp.DatablockNumber = oldDB;
                            }
                            else
                            {
                                if (cntCombinedTags > 1)
                                {
                                    readTagList.Add(rdHlp);
                                    rdHlp = new PLCTagReadHelper() {LibNoDaveDataType = TagDataType.ByteArray};
                                    cntCombinedTags = 0;
                                }
                                else
                                {
                                    readTagList.Add(lastTag);                                    
                                    cntCombinedTags = 0;
                                }

                                oldDataSource = plcTag.LibNoDaveDataSource;
                                oldDB = plcTag.DatablockNumber;
                                oldByteAddress = plcTag.ByteAddress;
                                oldLen = plcTag._internalGetSize();
                                if (oldLen % 2 != 0) oldLen++;
                                lastTag = plcTag;
                                cntCombinedTags++;
                            }
                        }
                        
                    }
                    if (cntCombinedTags > 1)
                        readTagList.Add(rdHlp);
                    else if (cntCombinedTags == 1)
                        readTagList.Add(lastTag);                    
                    #endregion


                    List<bool> NotExistedValue = new List<bool>();

                    //Count how Many Bytes from the PLC should be read and create a Byte Array for the Values
                    int completeReadSize = 0;
                    foreach (var libNoDaveValue in readTagList)
                    {
                        completeReadSize += libNoDaveValue._internalGetSize();
                    }
                    byte[] completeData = new byte[completeReadSize];


                    //Get the Maximum Answer Len for One PDU
                    int maxReadSize = _dc.getMaxPDULen() - 32; //32 = Header

                    int maxReadVar = maxReadSize/12; //12 Header Größe Variablenanfrage

                    int[] readenSizes = new int[maxReadVar];

                    int gesReadSize = 0;
                    int positionInCompleteData = 0;
                    int akVar = 0;
                    int anzVar = 0;
                    int anzReadVar = 0;

                    int akByteAddress = 0;

                    libnodave.PDU myPDU = _dc.prepareReadRequest();

                    foreach (var libNoDaveValue in readTagList)
                    {                       
                        //Save the Byte Address in anthoer Variable, because if we split the Read Request, we need not the real Start Address
                        akByteAddress = libNoDaveValue.ByteAddress;

                        if (libNoDaveValue.LibNoDaveDataSource != TagDataSource.Datablock && libNoDaveValue.LibNoDaveDataSource != TagDataSource.InstanceDatablock)
                            libNoDaveValue.DatablockNumber = 0;

                        int readSize = libNoDaveValue._internalGetSize();

                        const int HeaderTagSize = 4;

                    tryAgain:
                        int readSizeWithHeader = readSize + HeaderTagSize; //HeaderTagSize Bytes Header for each Tag
                        if (readSizeWithHeader%2 != 0) //Ungerade Anzahl Bytes, noch eines dazu...
                            readSizeWithHeader++;

                        //When there are too much bytes in the answer pdu, or you read more then the possible tags...
                        //But don't split if the bit is set (but ignore it if the tag is bigger then the pdu size!)
                        if ((readSizeWithHeader + gesReadSize > maxReadSize || anzReadVar == maxReadVar))
                        {
                            //If there is space for a tag left.... Then look how much Bytes we can put into this PDU
                            if (anzReadVar < maxReadVar && (!libNoDaveValue.DontSplitValue || readSize > maxReadSize))
                            {
                                int restBytes = maxReadSize - gesReadSize - HeaderTagSize; //Howmany Bytes can be added to this call
                                if (restBytes > 0)
                                {
                                    //Only at the rest of the bytes to the next read request, and increase the start address!                                    
                                    myPDU.addVarToReadRequest(Convert.ToInt32(libNoDaveValue.LibNoDaveDataSource), libNoDaveValue.DatablockNumber, akByteAddress, restBytes);
                                    readSize = readSize - restBytes;
                                    gesReadSize += restBytes;

                                    akByteAddress += restBytes;

                                    readenSizes[anzVar] = restBytes;
                                    anzVar++;

                                    //useresult muss noch programmiert werden.
                                }
                            }
                            var rs = new libnodave.resultSet();
                            int res = _dc.execReadRequest(myPDU, rs);
                            if (res != 0)
                                throw new Exception("Error: " + libnodave.daveStrerror(res));

                            //Save the Read Data to a User Byte Array (Because we use this in the libnodavevalue class!)                    
                            for (akVar = 0; akVar < anzVar; akVar++)
                            {
                                byte[] myBuff = new byte[gesReadSize];

                                res = _dc.useResult(rs, akVar, myBuff);
                                if (res == 10 || res == 5)
                                    NotExistedValue.Add(true);
                                else if (res != 0)
                                    throw new Exception("Error: " + libnodave.daveStrerror(res));
                                else
                                {
                                    NotExistedValue.Add(false);

                                    Array.Copy(myBuff, 0, completeData, positionInCompleteData, readenSizes[akVar]);
                                    positionInCompleteData += readenSizes[akVar];
                                    //for (int n = 0; n < readenSizes[akVar]; n++)
                                    //{
                                    //    completeData[positionInCompleteData++] = myBuff[n]; //Convert.ToByte(_dc.getU8());
                                    //}
                                }
                            }
                            //rs = null;
                            //myPDU = null;
                            anzVar = 0;
                            myPDU = _dc.prepareReadRequest();
                            gesReadSize = 0;
                            anzReadVar = 0;

                            //I need to do the whole splitting in anothe way, so that the goto disapears! But for the moment ir works!
                            goto tryAgain; //It tries again the Size test, this is necessary, when the Tag is bigger then one PDU
                        }

                        gesReadSize = gesReadSize + readSizeWithHeader;

                        readenSizes[anzVar] = readSize;
                        anzVar++;
                        anzReadVar++;                        
                        myPDU.addVarToReadRequest(Convert.ToInt32(libNoDaveValue.LibNoDaveDataSource), libNoDaveValue.DatablockNumber, akByteAddress, readSize);
                    }

                    if (gesReadSize > 0)
                    {
                        var rs = new libnodave.resultSet();
                        int res = _dc.execReadRequest(myPDU, rs);

                        if (res == -1025)
                        {
                            this.Disconnect();
                            return;
                        }
                        else if (res != 0 && res != 10)
                            throw new Exception("Error: " + libnodave.daveStrerror(res));

                        //Save the Read Data to a User Byte Array (Because we use this in the libnodavevalue class!)
                        for (akVar = 0; akVar < anzVar; akVar++)
                        {
                            byte[] myBuff = new byte[ /* gesReadSize */ readenSizes[akVar]+1];

                            res = _dc.useResult(rs, akVar, myBuff);
                            if (res == 10 || res == 5)
                                NotExistedValue.Add(true);
                            else if (res != 0)
                                throw new Exception("Error: " + libnodave.daveStrerror(res));
                            else
                            {
                                NotExistedValue.Add(false);

                                Array.Copy(myBuff, 0, completeData, positionInCompleteData, readenSizes[akVar]);
                                positionInCompleteData += readenSizes[akVar];

                                //for (int n = 0; n < readenSizes[akVar]; n++)
                                //{
                                //    completeData[positionInCompleteData++] = myBuff[n]; // Convert.ToByte(_dc.getU8());
                                //}
                            }
                        }
                    }

                    int buffPos = 0;
                    int nr = 0;
                    foreach (var value in readTagList)
                    {
                        if (!NotExistedValue[nr])
                        {
                            value.ItemDoesNotExist = false;
                            value._readValueFromBuffer(completeData, buffPos);
                            buffPos += value._internalGetSize();
                        }
                        else
                        {
                            value.ItemDoesNotExist = true;
                        }
                        nr++;
                    }
                }
        }

        /// <summary>
        /// This Function Reads One LibNoDave Value from the PLC
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>        
        public void ReadValue(PLCTag value)
        {
            if (AutoConnect && !Connected)
                Connect();
            if (_dc != null)
                lock (_dc)
                {

                    if (_dc == null)
                        throw new Exception("Error: Not Connected");

                    int readSize = value._internalGetSize();
                    byte[] myBuff = new byte[readSize];

                    if (value.LibNoDaveDataSource != TagDataSource.Datablock && value.LibNoDaveDataSource != TagDataSource.InstanceDatablock)
                        value.DatablockNumber = 0;
                    
                
                    int res = _dc.readManyBytes(Convert.ToInt32(value.LibNoDaveDataSource), value.DatablockNumber, value.ByteAddress, readSize, ref myBuff);
                                
                    int buffPos = 0;
                    if (res == 0)
                    {
                        value._readValueFromBuffer(myBuff, buffPos);
                        value.ItemDoesNotExist = false;
                    }
                    else if (res == -1025)
                    {
                        this.Disconnect();
                        return;
                    }
                    else if (res == 5 || res == 10)
                        value.ItemDoesNotExist = true;
                    else
                        throw new Exception("Error: " + libnodave.daveStrerror(res));
                }
        }


        //todo implement this!
        /// <summary>
        /// This Function Reads a List of LibNoDaveValues from a Byte Array.
        /// This can be used if you want to send Variables via a TCP Byte Stream from a PLC, and this
        /// Function is also used for the optimized reading.
        /// </summary>
        /// <param name="values">List of the Values</param>
        /// <param name="bytearray">ByteArray</param>
        /// <returns></returns>
        public void ReadValuesFromByteArray(IEnumerable<PLCTag> values, byte[] bytearray, int startpos)
        {
          
            if (_dc != null)
                lock (_dc)
                {
                    if (_dc == null)
                        throw new Exception("Error: Not Connected");

                    int pos = startpos;

                    foreach (var libNoDaveValue in values)
                    {
                        libNoDaveValue._readValueFromBuffer(bytearray, pos);
                        pos += libNoDaveValue._internalGetSize();
                    }
                }
        }

        /// <summary>
        /// Writes a single Value to the PLC
        /// </summary>
        /// <param name="value"></param>
        public void WriteValue(PLCTag value)
        {
            if (AutoConnect && !Connected)
                Connect();

            if (_dc != null)
                lock (_dc)
                {
                    if (_dc == null)
                        throw new Exception("Error: Not Connected");

                    int readSize = value._internalGetSize();
                    byte[] myBuff = new byte[readSize];


                    value._putControlValueIntoBuffer(myBuff, 0);

                    if (value.LibNoDaveDataSource != TagDataSource.Datablock && value.LibNoDaveDataSource != TagDataSource.InstanceDatablock)
                        value.DatablockNumber = 0;

                    int res;
                    if (value.LibNoDaveDataType == TagDataType.Bool)
                        res = _dc.writeBits(Convert.ToInt32(value.LibNoDaveDataSource), value.DatablockNumber, value.ByteAddress * 8 + value.BitAddress, readSize, myBuff);
                    else
                        res = _dc.writeManyBytes(Convert.ToInt32(value.LibNoDaveDataSource), value.DatablockNumber, value.ByteAddress, readSize, myBuff);
                    
                    if (res == -1025)
                    {
                        this.Disconnect();
                        return;
                    }
                    else if (res != 0)
                        throw new Exception("Error: " + libnodave.daveStrerror(res));
                }
        }

        public void WriteQueueClear()
        {
            _writeQueue.Clear();
        }

        public void WriteQueueAdd(PLCTag tag)
        {
            _writeQueue.Add(tag);
        }

         
        public void WriteQueueWriteToPLC()
        {
            if (_writeQueue.Count > 0)
                WriteValues(_writeQueue, true);
            _writeQueue.Clear();
        }

        public void WriteQueueWriteToPLCWithVarTabFunctions(PLCTriggerVarTab WriteTrigger)
        {
            if (_writeQueue.Count > 0)
                WriteValuesWithVarTabFunctions(_writeQueue, WriteTrigger);
            _writeQueue.Clear();
        }

        public void WriteValues(IEnumerable<PLCTag> valueList)
        {
            WriteValues(valueList, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueList"></param>
        /// <param name="useWriteOptimation">If set to true, write optimation is enabled, but then, the order of your written values can varry, also a 4 byte value can be splittet written to the plc!</param>
        public void WriteValues(IEnumerable<PLCTag> valueList, bool useWriteOptimation)
        {
            if (AutoConnect && !Connected)
                Connect();

            if (_dc != null)
                lock (_dc)
                {

                    if (useWriteOptimation)
                    {
                        #region Optimize Writing List....

                        List<PLCTag> orderedList = new List<PLCTag>();
                        orderedList.AddRange(valueList);
                        orderedList.Sort(new SorterForPLCTags());

                        List<PLCTag> writeTagList = new List<PLCTag>();

                        //Go through the List of PLC Tags and Combine the ones, where the Byte Address does not differ...
                        TagDataSource oldDataSource = 0;
                        int oldDB = 0, oldByteAddress = 0, oldLen = 0;
                        int cntCombinedTags = 0;
                        PLCTag lastTag = null;
                        PLCTagReadHelper rdHlp = new PLCTagReadHelper() {LibNoDaveDataType = TagDataType.ByteArray};
                        foreach (PLCTag plcTag in orderedList)
                        {
                            if (cntCombinedTags == 0) // && plcTag.LibNoDaveDataType!=TagDataType.Bool)
                            {
                                oldDataSource = plcTag.LibNoDaveDataSource;
                                oldDB = plcTag.DatablockNumber;
                                oldByteAddress = plcTag.ByteAddress;
                                oldLen = plcTag._internalGetSize();
                                lastTag = plcTag;
                                cntCombinedTags++;
                            }
                            else
                            {
                                if (plcTag.LibNoDaveDataType != TagDataType.Bool && oldDataSource == plcTag.LibNoDaveDataSource && (oldDataSource != TagDataSource.Datablock || oldDB == plcTag.DatablockNumber) && plcTag.ByteAddress <= oldByteAddress + oldLen)
                                {
                                    //todo: test if this is correct
                                    if (cntCombinedTags == 1)
                                        rdHlp.PLCTags.Add(lastTag, 0);

                                    cntCombinedTags++;
                                    int newlen = plcTag._internalGetSize() + (plcTag.ByteAddress - oldByteAddress);
                                    oldLen = oldLen < newlen ? newlen : oldLen;
                                    if (oldLen%2 != 0) oldLen++;
                                    rdHlp.PLCTags.Add(plcTag, plcTag.ByteAddress - oldByteAddress);
                                    rdHlp.ByteAddress = oldByteAddress;
                                    rdHlp.ArraySize = oldLen;
                                    rdHlp.LibNoDaveDataSource = oldDataSource;
                                    rdHlp.DatablockNumber = oldDB;
                                    lastTag = plcTag;
                                }
                                else
                                {
                                    if (cntCombinedTags > 1)
                                    {
                                        writeTagList.Add(rdHlp);
                                        rdHlp = new PLCTagReadHelper() {LibNoDaveDataType = TagDataType.ByteArray};
                                        cntCombinedTags = 0;
                                    }
                                    else
                                    {
                                        writeTagList.Add(lastTag);
                                        cntCombinedTags = 0;
                                    }

                                    if (plcTag.LibNoDaveDataType == TagDataType.Bool)
                                    {
                                        lastTag = plcTag;
                                        writeTagList.Add(lastTag);
                                    }
                                    else
                                    {
                                        oldDataSource = plcTag.LibNoDaveDataSource;
                                        oldDB = plcTag.DatablockNumber;
                                        oldByteAddress = plcTag.ByteAddress;
                                        oldLen = plcTag._internalGetSize();
                                        if (oldLen%2 != 0) oldLen++;
                                        lastTag = plcTag;
                                        cntCombinedTags++;
                                    }


                                }
                            }

                        }
                        if (cntCombinedTags > 1)
                            writeTagList.Add(rdHlp);
                        else if (cntCombinedTags == 1)
                            writeTagList.Add(lastTag);

                        #endregion

                        //Enable write optimation...                
                        valueList = writeTagList;
                    }

                    //Get the Maximum Answer Len for One PDU
                    int maxWriteSize = _dc.getMaxPDULen() - 32; //32 = Header
                    int gesWriteSize = 0;

                    //int maxWriteVar = 12; //Is this limit reality? Maybe this is somewhere in the system Data...
                    int tagHeaderSize = 12 + 4; //12 bytes for the adress part, and 4 bytes header in the data part
                    int anzWriteVar = 0;

                    int splitPos = 0;

                    libnodave.resultSet rs;
                    int res;

                    var myPDU = _dc.prepareWriteRequest();

                    var valueListT = new List<PLCTag>(valueList);

                    while (valueListT.Count > 0)
                    {
                        var currVal = valueListT[0];
                        var currValSize = currVal._internalGetSize();

                        if (gesWriteSize < maxWriteSize && //Maximale Byte Anzahl noch nicht erreicht
                            /*anzWriteVar < maxWriteVar &&*/ ( //maximale Variablenanzahl noch nicht erreicht                        
                                                         splitPos != 0 || //Value ist schon gesplitted
                                                         !currVal.DontSplitValue || //Value Kann gesplitted Werden
                                                         currValSize + tagHeaderSize > maxWriteSize || //Value ist größer als ein request
                                                         gesWriteSize + currValSize + tagHeaderSize < maxWriteSize)) //Value passt noch rein
                        {
                            //Add Var to Request...


                            //Wieviel Bytes hinzufügen? Den ganzen Tag oder einen Teil
                            var maxCurrAddSize = maxWriteSize - tagHeaderSize - gesWriteSize;

                            //Kompletter Value passt noch rein...
                            if (currValSize - splitPos <= maxCurrAddSize)
                            {
                                //Wert ist nicht gesplittet
                                if (splitPos == 0)
                                {
                                    byte[] wrt = new byte[currValSize];
                                    currVal._putControlValueIntoBuffer(wrt, 0);

                                    if (currVal.LibNoDaveDataType == TagDataType.Bool)
                                        myPDU.addBitVarToWriteRequest(Convert.ToInt32(currVal.LibNoDaveDataSource), currVal.DatablockNumber, (currVal.ByteAddress + splitPos)*8 + currVal.BitAddress, 1, wrt);
                                    else
                                        myPDU.addVarToWriteRequest(Convert.ToInt32(currVal.LibNoDaveDataSource), currVal.DatablockNumber, currVal.ByteAddress + splitPos, currValSize, wrt);
                                    gesWriteSize += tagHeaderSize + wrt.Length;
                                }
                                    //Wert war gesplittet
                                else
                                {
                                    byte[] tmp = new byte[currValSize];
                                    currVal._putControlValueIntoBuffer(tmp, 0);

                                    byte[] wrt = new byte[currValSize - splitPos];
                                    Array.Copy(tmp, splitPos, wrt, 0, wrt.Length);
                                    if (currVal.LibNoDaveDataType == TagDataType.Bool)
                                        myPDU.addBitVarToWriteRequest(Convert.ToInt32(currVal.LibNoDaveDataSource), currVal.DatablockNumber, (currVal.ByteAddress + splitPos)*8 + currVal.BitAddress, 1, wrt);
                                    else
                                        myPDU.addVarToWriteRequest(Convert.ToInt32(currVal.LibNoDaveDataSource), currVal.DatablockNumber, currVal.ByteAddress + splitPos, wrt.Length, wrt);
                                    gesWriteSize += tagHeaderSize + wrt.Length;

                                }

                                splitPos = 0;
                                anzWriteVar++;
                                valueListT.Remove(currVal); //Wert erledigt... löschen....
                            }
                                //Wert muss gesplittet werden...
                            else
                            {
                                byte[] tmp = new byte[currValSize];
                                currVal._putControlValueIntoBuffer(tmp, 0);

                                byte[] wrt = new byte[maxCurrAddSize];
                                Array.Copy(tmp, splitPos, wrt, 0, maxCurrAddSize);
                                if (currVal.LibNoDaveDataType == TagDataType.Bool)
                                    myPDU.addBitVarToWriteRequest(Convert.ToInt32(currVal.LibNoDaveDataSource), currVal.DatablockNumber, (currVal.ByteAddress + splitPos)*8 + currVal.BitAddress, 1, wrt);
                                else
                                    myPDU.addVarToWriteRequest(Convert.ToInt32(currVal.LibNoDaveDataSource), currVal.DatablockNumber, currVal.ByteAddress + splitPos, wrt.Length, wrt);
                                gesWriteSize += tagHeaderSize + wrt.Length;

                                splitPos = splitPos + maxCurrAddSize;
                                anzWriteVar++;
                            }

                        }
                        else
                        {
                            //Send Request
                            rs = new libnodave.resultSet();
                            res = _dc.execWriteRequest(myPDU, rs);
                            if (res == -1025)
                            {
                                this.Disconnect();
                                return;
                            }
                            anzWriteVar = 0;
                            gesWriteSize = 0;
                            myPDU = _dc.prepareWriteRequest();
                        }
                    }

                    if (gesWriteSize > 0)
                    {
                        rs = new libnodave.resultSet();
                        res = _dc.execWriteRequest(myPDU, rs);
                        if (res == -1025)
                        {
                            this.Disconnect();
                            return;
                        }
                    }
                }
        }

        /// <summary>
        /// This Disconnects from the PLC and the Adapter
        /// </summary>
        public void Disconnect()
        {
            Dispose();
        }

        public void Dispose()
        {
            Connected = false;
            if (_NeedDispose)
            {
                _NeedDispose = false;
                if (_dc != null)
                    _dc.disconnectPLC();
                _dc = null;

                if (_di != null)
                    _di.disconnectAdapter();
                _di = null;

                switch (_configuration.ConnectionType)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 10:
                        libnodave.closePort(_fds.rfd);
                        break;
                    case 50:
                        libnodave.closeS7online(_fds.rfd);
                        break;
                    case 122:
                    case 123:
                    case 124:
                    case 223:
                    case 224:
                    case 230:
                    case 231:
                        libnodave.closeSocket(_fds.rfd);
                        break;
                }
            }
        }
    }
}
