/*
 This implements a high level Wrapper between libnodave.dll and applications written
 in MS .Net languages.
 
 This ConnectionLibrary was written by Jochen Kuehner
 * http://jfk-solutuions.de/
 * 
 * Thanks go to:
 * Steffen Krayer -> For his work on MC7 decoding and the Source for his Decoder
 * Zottel         -> For LibNoDave

 The NCK part was written by J.Eger
 * 
 * Thanks go to:
 * Jochen Kuehner -> For his nice ConnectionLibrary
 * Thomas_v2.1    -> For the support of the telegram analyze

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
//#define daveDebug

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Timers;
using System.Linq;
using DotNetSiemensPLCToolBoxLibrary.Communication.FetchWrite;
using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using DotNetSiemensPLCToolBoxLibrary.Communication.Library;
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
 * Todo: Compress Memory (needs more testing)
 */
namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
    public class PLCConnection : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<PLCTag> _writeQueue = new List<PLCTag>();

        private object lockObj = new object();

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

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

        public Action<string> Logger { get; set; }

        public bool AutoDisconnect { get; set; }

        public PLCConnection(String name)
        {
            if (name == "")
                throw new Exception("No Connection Name specified!");

            _configuration = new PLCConnectionConfiguration(name);

            _connectionTargetPlcType = ConnectionTargetPLCType.S7;
        }

        /// <summary>
        /// Constructor for untiitests
        /// </summary>
        internal PLCConnection(PLCConnectionConfiguration config, IDaveConnection unittestConnection)
        {
            _connected = true;
            _configuration = config;
            _dc = unittestConnection;
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
        public IDaveConnection _dc = null;
        private Func<int, string> _errorCodeConverter;

        private System.Timers.Timer socketTimer;
        private Thread socketThread;

        private FetchWriteConnection _fetchWriteConnection;

        void socketTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (socketThread != null)
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
            catch (Exception ex)
            { }

            //TcpClient sock = new TcpClient(ip, _configuration.Port);
            //_fds.rfd = sock.Client.Handle;

            if (ip != null)
                _fds.rfd = libnodave.openSocket(_configuration.Port, ip);
            else
                _fds.rfd = libnodave.openSocket(_configuration.Port, _configuration.CpuIP);
        }

        #region General
        /// <summary>
        /// Connect to the PLC with the selected Configuration
        /// /// </summary>
        public void Connect()
        {
            lock (lockObj)
            {
                _NeedDispose = true;

                //Debugging for LibNoDave
                libnodave.daveSetDebug(0x0); //turn off libnodave log messages to console
                //libnodave.daveSetDebug(0x1ffff);

                //_configuration.ReloadConfiguration();

                //if (hwnd == 0 && _configuration.ConnectionType == 50)
                //    throw new Exception("Error: You can only use the S7Online Connection when you specify the HWND Parameter on the Connect Function");

                //This Jump mark is used when the Netlink Reset is activated!
                NLAgain:

                #region Setup Port/Adapter
                //LibNodave Verbindung aufbauen
                switch (_configuration.ConnectionType)
                {
                    case LibNodaveConnectionTypes.MPI_über_Serial_Adapter:
                    case LibNodaveConnectionTypes.MPI_über_Serial_Adapter_Andrews_Version_without_STX:
                    case LibNodaveConnectionTypes.MPI_über_Serial_Adapter_Step_7_Version:
                    case LibNodaveConnectionTypes.MPI_über_Serial_Adapter_Adrews_Version_with_STX:
                    case LibNodaveConnectionTypes.PPI_über_Serial_Adapter:
                        _errorCodeConverter = libnodave.daveStrerror;
                        _fds.rfd = libnodave.setPort(_configuration.ComPort, _configuration.ComPortSpeed, (int)_configuration.ComPortParity);
                        break;

                    case LibNodaveConnectionTypes.AS_511:
                        _errorCodeConverter = libnodave.daveStrerror;
                        _fds.rfd = libnodave.setPort(_configuration.ComPort, _configuration.ComPortSpeed, (int)_configuration.ComPortParity);
                        break;

#if !IPHONE
                    case LibNodaveConnectionTypes.Use_Step7_DLL:
                        _errorCodeConverter = libnodave.daveStrerror;
                        _fds.rfd = libnodave.openS7online(_configuration.EntryPoint, 0);
                        if (_fds.rfd.ToInt32() == -1)
                        {
                            _NeedDispose = false;
                            throw new Exception("Error: " + libnodave.daveStrS7onlineError());
                        }
                        break;
#endif

                    case LibNodaveConnectionTypes.ISO_over_TCP:
                    case LibNodaveConnectionTypes.ISO_over_TCP_CP_243:
                    case LibNodaveConnectionTypes.Netlink_lite:
                    case LibNodaveConnectionTypes.Netlink_lite_PPI:
                    case LibNodaveConnectionTypes.Netlink_Pro:
                        _errorCodeConverter = libnodave.daveStrerror;
                        socketTimer = new System.Timers.Timer(_configuration.TimeoutIPConnect.TotalMilliseconds);
                        socketTimer.AutoReset = true;
                        socketTimer.Elapsed += socketTimer_Elapsed;
                        socketTimer.Start();
                        socketThread = new Thread(this.socket_Thread);
                        socketThread.Start();
                        try
                        {
                            while (socketThread.ThreadState != ThreadState.AbortRequested &&
                                   socketThread.ThreadState != ThreadState.Aborted &&
                                   socketThread.ThreadState != ThreadState.Stopped)
                            {
                                Thread.Sleep(50);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        if (socketTimer != null)
                            socketTimer.Stop();
                        if (socketThread != null)
                            socketThread.Abort();
                        socketTimer = null;
                        socketThread = null;
                        break;

                    case LibNodaveConnectionTypes.Fetch_Write_Active:
                        _fetchWriteConnection = new FetchWriteConnection(this.Configuration);
                        _fetchWriteConnection.Connect();
                        Connected = true;
                        return;

                    //case 9050:
                    //    _errorCodeConverter = Connection.daveStrerror;
                    //    _dc = new S7onlineNETdave(_configuration);
                    //    break;
                    case LibNodaveConnectionTypes.ISO_over_TCP_Managed:
                        _errorCodeConverter = Connection.daveStrerror;
                        _dc = new TcpNETdave(_configuration);
                        break;
                }

                //if it is an Non manged version, using libnodave. Enums > 9000 are Managed implemntations
                if ((int)_configuration.ConnectionType < 9000)
                {

                    //if the socket handle still has its default value after connection
                    //this means it was an IP connection type, and it did not succed
                    if (_fds.rfd.ToInt32() == -999)
                    {
                        _NeedDispose = false;
                        throw new Exception("Error: Timeout Connecting the IP (" + _configuration.CpuIP + ":" +
                                            _configuration.Port + ")");
                    }

                    //if the read handle is still null or even has an error code, except for Simatic NEt connectoins
                    if ((_configuration.ConnectionType != LibNodaveConnectionTypes.Use_Step7_DLL && _fds.rfd.ToInt32() == 0) || _fds.rfd.ToInt32() < 0)
                    {
                        _NeedDispose = false;
                        throw new Exception(
                            "Error: Could not creating the Physical Interface (Maybe wrong IP, COM-Port not Ready,...)");
                    }

                    //The connection was successfull
                    _fds.wfd = _fds.rfd;
                }

                if (_configuration.ConnectionName == null)
                    _configuration.ConnectionName = Guid.NewGuid().ToString();

                #endregion

                #region Create the Interface
                //Create the Interface
                if ((int)_configuration.ConnectionType < 9000) //Enums > 9000 are Managed implemntations
                {
                    //Dave Interface Erzeugen
                    _di = new libnodave.daveInterface(_fds, _configuration.ConnectionName, _configuration.LokalMpi, (int)_configuration.ConnectionType, (int)_configuration.BusSpeed);

                    //Timeout setzen...
                    _di.setTimeout((int)_configuration.TimeoutMicroseconds); //WARNING! setTimeout needs value in Microseconds

                    //Dave Interface initialisieren
                    var initret = _di.initAdapter();
                    if (initret != 0)
                        throw new PLCException("Error: (Interface) (Code: " + initret.ToString() + ") " + _errorCodeConverter(initret), initret);
                }

                //Get S7OnlineType - To detect if is a IPConnection 
                bool IPConnection = false;
#if !IPHONE
                if (_configuration.ConnectionType == LibNodaveConnectionTypes.Use_Step7_DLL)
                {
                    RegistryKey myConnectionKey =
                        Registry.LocalMachine.CreateSubKey("SOFTWARE\\Siemens\\SINEC\\LogNames\\" +
                                                           _configuration.EntryPoint);
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
                #endregion

                #region Create Connection
                if (_configuration.ConnectionType == LibNodaveConnectionTypes.AS_511)
                {
                    _dc = new libnodave.daveConnection(_di, _configuration.CpuMpi, 0, 0);
                }
                else if ((int)_configuration.ConnectionType < 9000) //Enums > 9000 are Managed implemntations
                {
                    _dc = new libnodave.daveConnection(_di, _configuration.CpuMpi, _configuration.CpuIP, IPConnection,
                        _configuration.CpuRack, _configuration.CpuSlot, _configuration.Routing,
                        _configuration.RoutingSubnet1, _configuration.RoutingSubnet2,
                        _configuration.RoutingDestinationRack, _configuration.RoutingDestinationSlot,
                        _configuration.RoutingDestination, (int)_configuration.PLCConnectionType,
                        (int)_configuration.RoutingPLCConnectionType);
                }
                else
                {

                }
                #endregion

                #region Connect PLC
                if (_configuration.NetLinkReset && !_netlinkReseted && (_configuration.ConnectionType == LibNodaveConnectionTypes.Netlink_lite || _configuration.ConnectionType == LibNodaveConnectionTypes.Netlink_lite_PPI))
                {
                    _dc.resetIBH();
                    _netlinkReseted = true;
                    System.Threading.Thread.Sleep(1000);
                    goto NLAgain;
                }

                var ret = _dc.connectPLC();
                if (ret == -1)
                {
                    _dc = null;
                    throw new Exception("Error: CPU not available! Maybe wrong IP or MPI Address or Rack/Slot or ...");
                }
                if (ret != 0)
                    throw new PLCException("Error: (Connection) (Code: " + ret.ToString() + ") " + _errorCodeConverter(ret), ret);

                Connected = true;
                #endregion
            }
        }

        /// <summary>
        /// Internal Helper function that checks if the PLC is connectec, reconnects automatially or throws an Exception
        /// Call this function befor accessing the PLC Communication
        /// </summary>
        void CheckConnection()
        {
            if (Connected)
                return;

            if (AutoConnect)
                Connect();
            else
                throw new Exception("No connection to the PLC established yet");
        }

        /// <summary>
        /// This Disconnects from the PLC and the Adapter
        /// </summary>
        public void Disconnect()
        {
            Dispose();
        }

        /// <summary>
        /// Stop execution of the User Program in the Controller
        /// WARNING! use with caution!
        /// </summary>
        public void PLCStop()
        {
            lock (lockObj)
            {
                CheckConnection();

                if (_dc != null)
                    _dc.stop();
            }
        }

        /// <summary>
        /// Start execution of the User Program in the Controller
        /// WARNING! use with caution!
        /// </summary>
        public void PLCStart()
        {
            lock (lockObj)
            {
                CheckConnection();

                if (_dc != null)
                    _dc.start();
            }
        }

        /// <summary>
        /// Read the current time of the controllers system time
        /// </summary>
        /// <returns></returns>
        public DateTime PLCReadTime()
        {
            lock (lockObj)
            {
                CheckConnection();

                DateTime retVal = DateTime.MinValue;
                if (_dc != null)
                {
                    var res = _dc.daveReadPLCTime(out retVal);

                    if (res != 0)
                    {
                        throw new PLCException(res);
                    }
                }

                return retVal;
            }
        }

        /// <summary>
        /// Set the controllers current system time
        /// </summary>
        /// <param name="tm"></param>
        public void PLCSetTime(DateTime tm)
        {
            lock (lockObj)
            {
                CheckConnection();

                if (_dc != null)
                    _dc.daveSetPLCTime(tm);
            }
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
                lock (myConn.lockObj)
                {
                    if (myConn._dc != null)
                    {
                        byte[] para;
                        byte[] data;

                        var myPDU = myConn._dc.createPDU();
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

                        if (rparam[10] == 0xd0 && rparam[11] == 0xa5)
                            throw new Exception("Error, the Commands are not excetuted");
                        else if (rparam[10] == 0xd0)
                            throw new Exception("Error, the Trigger is already in use. Err. Code: " +
                                rparam[11].ToString("X"));
                        else if (rparam[10] != 0x00)
                            throw new Exception("Error reading Diagnostic Data");
                        else if (rdata.Length < 14) //Function Block is not called!
                            return;

                        int answLen = rdata[6] * 0x100 + rdata[7];

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
                            int ByteRow = rdata[linenr] * 0x100 + rdata[linenr + 1];
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
                lock (myConn.lockObj)
                {
                    byte[] para;
                    byte[] data;

                    var myPDU = myConn._dc.createPDU();

                    para = new byte[] {0x00, 0x01, 0x12, 0x08, 0x12, 0x41, 0x0f, 0x00, 0x00, 0x00, 0x00, 0x00};
                    data = new byte[]
                    {
                        0x00, 0x14, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,
                        0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, BitConverter.GetBytes(ReqestID)[0],
                        BitConverter.GetBytes(ReqestID)[1]
                    };
                    myConn._dc.daveBuildAndSendPDU(myPDU, para, data);
                }
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
                return (byte)mySel;
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
        public DiagnosticData PLCstartRequestDiagnosticData(S7FunctionBlock myBlock, int StartAWLByteAdress,
            S7FunctionBlockRow.SelectedStatusValues selRegister
            /*Count of the Rows wich should be read, Registers wich should be read! */)
        {
            lock (lockObj)
            {
                CheckConnection();

                if (_dc != null)
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
                    if ((((xy31_2Dataset)szlDatasets[0]).funkt_2 & 0x08) > 0)
                        //Byte 3 and 4 say as a Bit array wich Status Tele is supported!                     
                        DiagDataTeletype = 0x13;


                    //DiagDataTeletype = 0x01;

                    //len of the AnswBlock Block in the PDU
                    short answSize =
                        (short)(S7FunctionBlockRow._GetCommandStatusAskSize(selRegister, DiagDataTeletype) + 2);


                    //Todo: Implement Callingpath
                    int askHeaderSize = 28;
                    //This is 28 when no Callingpath is defined! (Callingpatth is not yet implemented)

                    //These are the Bytes in wich the Selected Registers for each Row are stored!
                    //When in a Row there is no Change for the selected Registers, these Row is added to the previous, so no extra Status for this row is necessary
                    List<byte> LinesSelectedRegisters = new List<byte>();
                    Dictionary<int, List<S7FunctionBlockRow>> ByteAdressNumerPLCFunctionBlocks =
                        new Dictionary<int, List<S7FunctionBlockRow>>();

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
                            S7FunctionBlockRow.SelectedStatusValues askRegister =
                                plcFunctionBlockRow._GetCommandStatusAskValues(selRegister, DiagDataTeletype);

                            int akAskSize = S7FunctionBlockRow._GetCommandStatusAskSize(askRegister, DiagDataTeletype);

                            if ((answSize + akAskSize) < 182)
                            //Max Size of the Answer Len! (In Step 7 this is 202 i think, have to try it...)
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

                                    akAskSize = S7FunctionBlockRow._GetCommandStatusAskSize(askRegister,
                                        DiagDataTeletype);

                                    /*
                                    if (akAskSize > 0 && plcFunctionBlockRow.Command == Call und parameter ist fc dann)
                                    {
                                     * The Status of a UC or CC should not be asked in the byte address, no it sould be asked at the Address of the SPA
                                     * a UC with FC <=255 is 2 Byte (+4 byte of the SPA), greater it's 4 byte (+4 byte of the SPA)
                                     * a UC/CC for a FB contains no SPA so this state should be asked directly!

                                    }
                                    
                                    else */
                                    if (akAskSize > 0)
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
                                            byte askRegisterByte = GetByteForSelectedRegisters(askRegister,
                                                DiagDataTeletype);
                                            LinesSelectedRegisters.AddRange(new byte[]
                                            {bt[1], bt[0], 0x00, askRegisterByte});
                                            //askSize += 4;
                                        }
                                        else
                                        {
                                            byte askRegisterByte = GetByteForSelectedRegisters(askRegister,
                                                DiagDataTeletype);
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

                    var myPDU = _dc.createPDU();

                    //PDU Header
                    byte[] para;
                    //PDU Data
                    byte[] data;

                    para = new byte[]
                    {0x00, 0x01, 0x12, 0x08, 0x12, 0x41, DiagDataTeletype, 0x00, 0x00, 0x00, 0x00, 0x00};
                    byte[] tmp1;
                    if (DiagDataTeletype == 0x13)
                        tmp1 = new byte[]
                        {
                            BitConverter.GetBytes(askHeaderSize)[1], BitConverter.GetBytes(askHeaderSize)[0],
                            BitConverter.GetBytes(askSize)[1], BitConverter.GetBytes(askSize)[0], 0x00, 0x00, 0x00, 0x01
                            , 0x00, 0x00, BitConverter.GetBytes(answSize)[1], BitConverter.GetBytes(answSize)[0], 0x00,
                            0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x06, 0x00, 0x00,
                            Convert.ToByte(myBlock.BlockType), BitConverter.GetBytes(myBlock.BlockNumber)[1],
                            BitConverter.GetBytes(myBlock.BlockNumber)[0], BitConverter.GetBytes(StartAWLByteAdress)[1],
                            BitConverter.GetBytes(StartAWLByteAdress)[0],
                            BitConverter.GetBytes(rowByteAdress - StartAWLByteAdress)[1],
                            BitConverter.GetBytes(rowByteAdress - StartAWLByteAdress)[0], 0x80, Convert.ToByte(cnt),
                            0x00, GetByteForSelectedRegisters(selRegister, DiagDataTeletype)
                        };
                    else
                        tmp1 = new byte[]
                        {
                            BitConverter.GetBytes(askHeaderSize)[1], BitConverter.GetBytes(askHeaderSize)[0],
                            BitConverter.GetBytes(askSize)[1], BitConverter.GetBytes(askSize)[0], 0x00, 0x00, 0x00, 0x01
                            , 0x00, 0x00, BitConverter.GetBytes(answSize)[1], BitConverter.GetBytes(answSize)[0], 0x00,
                            0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x06, 0x00, 0x00,
                            Convert.ToByte(myBlock.BlockType), BitConverter.GetBytes(myBlock.BlockNumber)[1],
                            BitConverter.GetBytes(myBlock.BlockNumber)[0], BitConverter.GetBytes(StartAWLByteAdress)[1],
                            BitConverter.GetBytes(StartAWLByteAdress)[0],
                            BitConverter.GetBytes(rowByteAdress - StartAWLByteAdress)[1],
                            BitConverter.GetBytes(rowByteAdress - StartAWLByteAdress)[0], 0x80,
                            GetByteForSelectedRegisters(selRegister, DiagDataTeletype)
                        };
                    data = Helper.CombineByteArray(tmp1, LinesSelectedRegisters.ToArray());
                    _dc.daveBuildAndSendPDU(myPDU, para, data);

                    byte[] rdata, rparam;
                    int res = _dc.daveGetPDUData(myPDU, out rdata, out rparam);

                    byte[] stid = new byte[] { rparam[6], rparam[7] };

                    if (rparam[10] != 0x00 && rparam[11] != 0x00) // 0xd05f
                        throw new Exception("Error Requesting Block Status, Error Code: 0x" +
                            rparam[10].ToString("X").PadLeft(2, '0') + rparam[11].ToString("X").PadLeft(2, '0'));

                    retDiagnosticData.ReqestID = BitConverter.ToInt16(stid, 0);

                    retDiagnosticData.DiagDataTeletype = DiagDataTeletype;

                    myBlock.DiagnosticData = retDiagnosticData;
                    return retDiagnosticData;
                }
                return null;
            }
        }

        public DataTypes.PLCState PLCGetSafetyStep()
        {
            lock (lockObj)
            {
                CheckConnection();

                if (_dc != null)
                {
                    byte[] buffer = new byte[40];
                    int ret = _dc.readSZL(0x232, 4, buffer);

                    if (ret < 0)
                        throw new PLCException(ret);
                    if (buffer[1] != 0x04)
                        throw new WPFToolboxForSiemensPLCsException(
                            WPFToolboxForSiemensPLCsExceptionType.ErrorReadingSZL);


                    //byte 2,3 betriebsartschalter
                    //    byte 4,5 schutzstufe
                }
                return 0;
            }
        }

        /// <summary>
        /// Send the password to the PLC for authentication. this is only necessary if the controllers password protection is active
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public bool PLCSendPassword(string pwd)
        {
            lock (lockObj)
            {
                CheckConnection();

                if (_dc != null)
                {
                    var myPDU = _dc.createPDU();

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
        }

        /// <summary>
        /// Get the current Execution status from the controller
        /// </summary>
        /// <returns></returns>
        public DataTypes.PLCState PLCGetState()
        {
            lock (lockObj)
            {
                CheckConnection();

                //Dokumentation of SZL can be found here: http://www.google.de/url?sa=t&source=web&cd=3&ved=0CCQQFjAC&url=http%3A%2F%2Fdce.felk.cvut.cz%2Frs%2Fplcs7315%2Fmanualy%2Fsfc_e.pdf&ei=tY8QTJufEYSNOLD_oMoH&usg=AFQjCNEHofHOLDcvGp-4eQBwlboKPu3oxQ
                if (_dc != null)
                {
                    byte[] buffer = new byte[64];
                    int ret = _dc.readSZL(0x174, 4, buffer); //SZL 0x174 is for PLC LEDs

                    if (AutoDisconnect && (ret == -1025 || ret == -128))
                    {
                        if (Logger != null)
                            Logger("(1) Auto Disconnect cause of :" + libnodave.daveStrerror(ret));
                        this.Disconnect();
                        return DataTypes.PLCState.Unkown; ;
                    }

                    if (ret == 54273)
                        return DataTypes.PLCState.NotSupported;
                    if (ret < 0)
                        throw new PLCException(ret);
                    if (buffer[10] == 1 && buffer[11] == 1)
                        return DataTypes.PLCState.Starting;
                    else if (buffer[10] == 1)
                        return DataTypes.PLCState.Running;
                    else
                        return DataTypes.PLCState.Stopped;
                }
                return DataTypes.PLCState.Unkown;
            }
        }
        #endregion

        #region PLC Blocks and Inventory
        /// <summary>
        /// Load an full or partial list of all currently loaded code and data blocks in the controller
        /// </summary>
        /// <param name="myBlk">the block type that will be listed</param>
        /// <returns>Returns an list of string short names of the existing blocks such as "DB1" "FC987"...</returns>
        public List<string> PLCListBlocks(DataTypes.PLCBlockType myBlk)
        {
            lock (lockObj)
            {
                CheckConnection();

                if (_dc != null)
                {
                    List<string> myRet = new List<string>();

                    byte[] blocks = new byte[2048 * 16];

                    //_configuration.ConnectionTyp

                    if (myBlk == DataTypes.PLCBlockType.AllBlocks &&
                        ConnectionTargetPLCType == ConnectionTargetPLCType.S7)
                    {
                        myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.OB));
                        myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.FC));
                        myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.FB));
                        myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.DB));
                        myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.SFC));
                        myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.SFB));
                        myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.SDB));
                    }
                    else if (myBlk == DataTypes.PLCBlockType.AllEditableBlocks &&
                        ConnectionTargetPLCType == ConnectionTargetPLCType.S7)
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
                        if (ret < 0 && ret != -53763 && ret != -53774 && ret != -255)
                            throw new PLCException(ret);
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
        }

        /// <summary>
        /// Load an full or partial list of all currently loaded code and data blocks in the controller
        /// </summary>
        /// <param name="myBlk">the block type that will be listed</param>
        /// <returns>Returns an list of Block Names containing the block types and numbers</returns>
        public List<PLCBlockName> PLCListBlocks2(DataTypes.PLCBlockType myBlk)
        {
            lock (lockObj)
            {
                CheckConnection();

                if (_dc != null)
                {
                    List<PLCBlockName> myRet = new List<PLCBlockName>();

                    byte[] blocks = new byte[2048 * 16];

                    if (myBlk == DataTypes.PLCBlockType.AllBlocks &&
                        ConnectionTargetPLCType == ConnectionTargetPLCType.S7)
                    {
                        myRet.AddRange(PLCListBlocks2(DataTypes.PLCBlockType.OB));
                        myRet.AddRange(PLCListBlocks2(DataTypes.PLCBlockType.FC));
                        myRet.AddRange(PLCListBlocks2(DataTypes.PLCBlockType.FB));
                        myRet.AddRange(PLCListBlocks2(DataTypes.PLCBlockType.DB));
                        myRet.AddRange(PLCListBlocks2(DataTypes.PLCBlockType.SFC));
                        myRet.AddRange(PLCListBlocks2(DataTypes.PLCBlockType.SFB));
                        myRet.AddRange(PLCListBlocks2(DataTypes.PLCBlockType.SDB));
                    }
                    else if (myBlk == DataTypes.PLCBlockType.AllEditableBlocks &&
                        ConnectionTargetPLCType == ConnectionTargetPLCType.S7)
                    {
                        myRet.AddRange(PLCListBlocks2(DataTypes.PLCBlockType.OB));
                        myRet.AddRange(PLCListBlocks2(DataTypes.PLCBlockType.FC));
                        myRet.AddRange(PLCListBlocks2(DataTypes.PLCBlockType.FB));
                        myRet.AddRange(PLCListBlocks2(DataTypes.PLCBlockType.DB));
                        myRet.AddRange(PLCListBlocks2(DataTypes.PLCBlockType.SDB));
                    }
                    else
                    {
                        int ret = _dc.ListBlocksOfType(Helper.GetPLCBlockTypeForBlockList(myBlk), blocks);
                        if (ret < 0 && ret != -53763 && ret != -53774 && ret != -255)
                            throw new PLCException(ret);
                        if (ret > 0)
                            for (int n = 0; n < ret * 4; n += 4)
                            {
                                int nr = blocks[n] + blocks[n + 1] * 256;
                                myRet.Add(new PLCBlockName(myBlk, nr));
                            }
                    }
                    return myRet;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the amount of blocks per block type in the PLC
        /// </summary>
        /// <returns>An Dictionary containing the block count for each Block type in the PLC</returns>
        public Dictionary<PLCBlockType, int> PLCGetBlockCount()
        {
            lock (lockObj)
            {
                CheckConnection();

                if (_dc != null)
                {
                    int res = 0;
                    libnodave.PDU PDU = new libnodave.PDU();
                    byte[] Para = { 0, 1, 18, 4, 17, 67, 1, 0 };
                    byte[] Data = null; //no data part for this message

                    //Send Request
                    res = _dc.daveBuildAndSendPDU(PDU, Para, Data);
                    if (res != 0)
                        throw new PLCException(res);

                    //Wait for Respond
                    byte[] RecData = null;
                    byte[] RecPara = null;

                    res = _dc.daveRecieveData(out RecData, out RecPara);
                    if (!(res == 0))
                        throw new PLCException(res);

                    //Parse Response data
                    Dictionary<PLCBlockType, int> dict = new Dictionary<PLCBlockType, int>();

                    //Each Structure has 4 Byte
                    for (int i = 4; i <= RecData.Length - 1; i += 4)
                    {
                        int count = RecData[i + 3] + RecData[i + 2] * 256;

                        //the Block type is sent as ASCII
                        char blk = (char)RecData[i + 1];
                        PLCBlockType blkt = (PLCBlockType)int.Parse(blk.ToString(), System.Globalization.NumberStyles.HexNumber);

                        dict.Add(blkt, count);
                    }
                    return dict;
                }
                return null;
            }
        }

        /// <summary>
        /// Load the basic header information from the PLC. This is more efficient than loading whole MC7 code from the plc
        /// </summary>
        public S7Block PLCGetBlockHeader(string blockName)
        {
            PLCBlockName bn = new PLCBlockName(blockName);
            return PLCGetBlockHeader(bn.BlockType, bn.BlockNumber);
        }

        /// <summary>
        /// Load the basic header information from the PLC. This is more efficient than loading whole MC7 code from the plc
        /// </summary>
        /// <param name="type"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public S7Block PLCGetBlockHeader(PLCBlockType type, int number)
        {
            lock (lockObj)
            {
                //Auto connect or Error if not connected yet
                CheckConnection();

                //Load Block List from PLC
                //Para Format:
                //Byte 0 : 0 Header 
                //Byte 1 : 1 Header
                //Byte 2 : 18 Header
                //Byte 3 : length of Parameter in Bytes starting at byte 4
                //Byte 4 : 17 Unknown
                //Byte 5 : Function code?
                //Byte 6 :Sub Function Code?

                //Data Format:
                //   Byte 0-1    : Block type as Hexadecimal in ASCII (DB = 10d = 0xA = 'A')
                //   Byte 2-6    : Block Number formated as "0" padded ASCII Number. So the maximum number is 99999
                //   Byte 7      : 'A' Identifier for end of request
                int res = 0;
                libnodave.PDU PDU = new libnodave.PDU();
                byte[] Para = { 0, 1, 18, 4, 17, 67, 3, 0 };
                byte[] Data = { 48, 48, 48, 48, 48, 49, 48, 65 }; //Fill request with out default Block type and Block number = 1

                //Block Type sent as Hexadecimal via ASCII
                Data[1] = (byte)((int)type).ToString("X")[0];

                //Block number
                string NumberStr = number.ToString().PadLeft(5, '0');
                byte[] NumberBytes = System.Text.Encoding.ASCII.GetBytes(NumberStr);
                NumberBytes.CopyTo(Data, 2);

                //End of Request
                Data[7] = (byte)'A';

                //Send Request
                res = _dc.daveBuildAndSendPDU(PDU, Para, Data);
                if (res != 0)
                    throw new PLCException(res);

                byte[] RecData = null;  //the Received Data
                byte[] RecPara = null;  //The Receive Parameter

                //Get response from controller
                res = _dc.daveGetPDUData(PDU, out RecData, out RecPara);
                if (!(res == 0))
                    throw new PLCException(res);

                //Trim first 10 bytes from header
                byte[] MC7Code = new byte[RecData.Length - 10];
                Array.Copy(RecData, 10, MC7Code, 0, MC7Code.Length);

                //Parse Header
                return MC7Converter.ParseBlockHeaderAndFooterFromMC7(MC7Code, 0);
            }
        }

        /// <summary>
        /// Returns the data block size of th Requested data block
        /// </summary>
        /// <param name="BlockName">The short name representation of the data block. such as "DB1" or "DB992"</param>
        /// <returns></returns>
        public int PLCGetDataBlockSize(string BlockName)
        {
            var blk = PLCGetBlockHeader(BlockName);
            if (blk == null)
                return 0;
            return blk.CodeSize;
        }

        /// <summary>
        /// Get the MC7 Code from the controller of the requested block. The MC7 code represents the whole code and all header information of the block
        /// </summary>
        /// <param name="BlockName">The short name representation of the data block. such as "DB1" or "DB992"</param>
        /// <returns></returns>
        public byte[] PLCGetBlockInMC7(string BlockName)
        {
            lock (lockObj)
            {
                CheckConnection();

                if (_dc != null)
                {
                    //Todo: Better way to Split number and chars
                    byte[] buffer = new byte[65536 * 3];
                    string tmp = BlockName.ToUpper().Trim().Replace(" ", "");
                    string block = "";
                    int nr = 0;
                    if (tmp.StartsWith("SDB"))
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
                    else if (ret == 53825) //Error: D241: Operation not permitted in current protection level.
                        throw new PLCException("PLC is Password Protected, unlock before downloading Blocks!", ret);
                    else
                        throw new PLCException(ret);
                }
                return null;
            }
        }

        /// <summary>
        /// Downloads an MC7 formatted block to the PLC. WARNING use with caution!
        /// </summary>
        /// <param name="BlockName">The short name representation of the data block. such as "DB1" or "DB992"</param>
        /// <param name="buffer">An buffer containing the MC7 code of the block to download</param>
        public void PLCPutBlockFromMC7toPLC(string BlockName, byte[] buffer)
        {
            lock (lockObj)
            {
                CheckConnection();

                if (_dc != null)
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


                    //Transfer crc:
                    //Benötigt für Safety übertragung!
                    //Es ist eine CRC16 Prüfsumme mit dem Generator Polynom 0x9003 , Init = 0x0000 , RefIn = False, RefOut = False, XorOut = 0x0000.
                    //Die Prüfsumme wird aus folgenden Bytes gebildet:
                    //Byte 5, Bausteinkennung(0x0A for DB, 0x0C für FC, .... )
                    //    Byte 34, 35 Länge des Arbeitsspeicher in Bytes(ohne die 36 Bytes Header Länge) Länge MC7 Code
                    // Byte 36 bis Byte (36 + Länge des Arbeitsspeicher - 1 ) 
                    //var crcbyte = new[] {0x9003, (short) blk,};
                    var sizeHighByte = (buffer.Length - 36) / 256;
                    var sizeLowByte = ((buffer.Length - 36) - 256 * sizeHighByte);
                    var crcHeader = new byte[] { (byte)blk, (byte)sizeHighByte, (byte)sizeLowByte };
                    var crcBytes = new byte[buffer.Length - 36 + 3];
                    Array.Copy(crcHeader, 0, crcBytes, 0, 3);
                    Array.Copy(buffer, 36, crcBytes, 3, buffer.Length - 36);
                    var crc = CrcHelper.GetCrc16(crcBytes);
                    //Add CRC to transmit data

                    if (blk == DataTypes.PLCBlockType.AllBlocks || nr < 0)
                        throw new Exception("Unsupported Block Type!");

                    int readsize = buffer.Length;
                    int ret = _dc.putProgramBlock(Helper.GetPLCBlockTypeForBlockList(blk), nr, buffer, readsize);

                    if (ret == 0 && readsize > 0)
                    {
                        return;
                    }
                    else if (ret == 53825)
                        throw new PLCException("PLC is Password Protected, unlock before downloading Blocks!", ret);
                    else
                        throw new PLCException(ret);
                }
                return;
            }
        }

        /// <summary>
        /// Deletes an code or data block from the connected controller. WARNING use with cation!
        /// </summary>
        /// <param name="BlockName">The short name representation of the data block. such as "DB1" or "DB992"</param>
        public void PLCDeleteBlock(string BlockName)
        {
            lock (lockObj)
            {
                CheckConnection();

                if (_dc != null)
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
        }
        #endregion

        #region PLC Memory and Diagnostics
        /// <summary>
        /// Upload an System State list (SZL) from the controller that hold configuration, state and capability information
        /// For information about SZLNummbers and indexes please consult the information from SIEMENS regarding SFC51 "RDSYSST"
        /// </summary>
        /// <param name="SZLNummer">The System State list Number</param>
        /// <param name="Index">The System State sub list number</param>
        /// <returns></returns>
        public SZLData PLCGetSZL(Int16 SZLNummer, Int16 Index)
        {
            lock (lockObj)
            {
                CheckConnection();

                if (_dc != null)
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
                        throw new PLCException(ret);

                    retVal.SzlId = (short)(buffer[1] + buffer[0] * 256);
                    retVal.Index = (short)(buffer[3] + buffer[2] * 256);
                    retVal.Size = (short)(buffer[5] + buffer[4] * 256);
                    retVal.Count = (short)(buffer[7] + buffer[6] * 256);

                    List<SZLDataset> datsets = new List<SZLDataset>();

                    for (int n = 0; n < retVal.Count; n++)
                    {
                        byte[] objBuffer = new byte[retVal.Size];
                        Array.Copy(buffer, (n * retVal.Size) + 8, objBuffer, 0, retVal.Size);
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
                                    case 4:
                                        datsets.Add(EndianessMarshaler.BytesToStruct<xy32_4Dataset>(objBuffer));
                                        break;
                                    case 8:
                                        datsets.Add(EndianessMarshaler.BytesToStruct<xy32_8Dataset>(objBuffer));
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
        }

        /// <summary>
        /// Read the Controllers Diagnostic buffer containing controller events
        /// </summary>
        /// <returns></returns>
        public List<DataTypes.DiagnosticEntry> PLCGetDiagnosticBuffer()
        {
            lock (lockObj)
            {
                CheckConnection();

                if (_dc != null)
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
                        throw new PLCException(ret);

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
        }

        public DataTypes.PLCMemoryInfo PLCGetMemoryInfo()
        {
            lock (lockObj)
            {
                CheckConnection();

                if (_dc != null)
                {
                    /*
                    List<LibNoDaveDataTypes.DiagnosticEntry> retVal = new List<LibNoDaveDataTypes.DiagnosticEntry>();
                    byte[] buffer = new byte[65536];
                    int ret = _dc.readSZL(0xA0, 0, buffer);

                    if (ret < 0)
                        throw new Exception("Error: " + _errorCodeConverter(ret));

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
        }

        /// <summary>
        /// Compress the Memory of the PLC. This performs an "de-fragmentation" of the PLCs memory 
        /// in order to produce bigger "chunks" of free memory.
        /// </summary>
        /// <remarks>This function takes a very, very long time to process</remarks>
        public void PLCCompressMemory()
        {
            lock (lockObj)
            {
                CheckConnection();

                //This is a very slow function, so increase the timeout
                int Timeout = _di.getTimeout();
                _di.setTimeout(Timeout * 10);

                int res = 0;
                libnodave.PDU PDU = new libnodave.PDU();
                byte[] Para = { 0x28, 0, 0, 0, 0, 0, 0, 0xFD, 0, 0x00, 5, (byte)'_', (byte)'G', (byte)'A', (byte)'R', (byte)'B' };
                byte[] Data = null; //no data part for this message

                //Send Request
                res = _dc.daveBuildAndSendPDU(PDU, Para, Data);
                if (res != 0)
                    throw new PLCException(res);

                //Wait for Respond
                byte[] RecData = null;
                byte[] RecPara = null;

                res = _dc.daveRecieveData(out RecData, out RecPara);
                _di.setTimeout(Timeout * 10); //Restore original Timeout

                if (!(res == 0))
                    throw new PLCException(res);
            }
        }

        /// <summary>
        /// Copy the Content of the volatile RAM memory to the Non-volatile ROM memory
        /// </summary>
        public void PLCCopyRamToRom()
        {
            lock (lockObj)
            {
                CheckConnection();

                //This is a very slow function, so increase the timeout
                int Timeout = _di.getTimeout();
                _di.setTimeout(Timeout * 10);

                int res = 0;
                libnodave.PDU PDU = new libnodave.PDU();
                byte[] Para = { 0x28, 0, 0, 0, 0, 0, 0, 0xfd, 0, 2, (byte)'E', (byte)'P', 5, (byte)'_', (byte)'M', (byte)'O', (byte)'D', (byte)'U' };
                byte[] Data = null; //no data part for this message

                //Send Request
                res = _dc.daveBuildAndSendPDU(PDU, Para, Data);
                if (res != 0)
                    throw new PLCException(res);

                //Wait for Respond
                byte[] RecData = null;
                byte[] RecPara = null;

                res = _dc.daveRecieveData(out RecData, out RecPara);
                _di.setTimeout(Timeout * 10); //Restore original Timeout

                if (!(res == 0))
                    throw new PLCException(res);
            }
        }
        #endregion

        #region VarTab
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
                lock (myConn.lockObj)
                {
                    if (myConn._dc != null)
                    {
                        var myPDU = myConn._dc.createPDU();

                        byte[] para;
                        byte[] data;

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
                                len = rdata[pos + 2] * 0x100 + rdata[pos + 3];
                                if (len < PLCTags[i].ReadByteSize)
                                    throw new Exception("The Tag for the VarTabRead Function was to huge, " +
                                        PLCTags[i].ReadByteSize + " Bytes should be read, but only " + len +
                                        " Bytes were read!");
                                if (len % 2 != 0) len++;
                                PLCTags[i]._readValueFromBuffer(rdata, pos + 4);
                            }
                            else
                                PLCTags[i].ItemDoesNotExist = true;
                            pos += 4 + len;
                        }
                    }
                }
            }

            private bool Closed;
            public void Close()
            {
                lock (myConn.lockObj)
                {
                    byte[] para;
                    byte[] data;

                    para = new byte[] {0x00, 0x01, 0x12, 0x08, 0x12, 0x41, 0x0f, 0x00, 0x00, 0x00, 0x00, 0x00};
                    data = new byte[]
                    {
                        0x00, 0x14, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,
                        0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, BitConverter.GetBytes(ReqestID)[0],
                        BitConverter.GetBytes(ReqestID)[1]
                    };

                    var myPDU = myConn._dc.createPDU();
                    myConn._dc.daveBuildAndSendPDU(myPDU, para, data);
                }
            }

            public void Dispose()
            {
                if (!Closed)
                    Close();
            }
        }

        public VarTabReadData ReadValuesWithVarTabFunctions(IEnumerable<PLCTag> valueList, PLCTriggerVarTab ReadTrigger)
        {
            lock (lockObj)
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

                    switch (plcTag.TagDataSource)
                    {
                        case MemoryArea.Flags:
                            dtaTyp = 0;
                            break;
                        case MemoryArea.Inputs:
                            dtaTyp = 1;
                            break;
                        case MemoryArea.Outputs:
                            dtaTyp = 2;
                            break;
                        case MemoryArea.Datablock:
                            dtaTyp = 0x07;
                            dbNo = plcTag.DataBlockNumber;
                            break;
                        case MemoryArea.Timer:
                            dtaTyp = 5;
                            break;
                        case MemoryArea.Counter:
                            dtaTyp = 6;
                            break;
                        case MemoryArea.LocalData:
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
                            if (plcTag.TagDataSource == MemoryArea.Timer || plcTag.TagDataSource == MemoryArea.Counter)
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

                    akAsk[0] = (byte) (dtaTyp * 0x10 + dtaSize);
                    akAsk[1] = (byte) dtaArrSize;
                    akAsk[2] = (byte) (dbNo / 0x100);
                    akAsk[3] = (byte) (dbNo % 0x100);
                    akAsk[4] = (byte) (plcTag.ByteAddress / 0x100);
                    akAsk[5] = (byte) (plcTag.ByteAddress % 0x100);

                    askBytes.AddRange(akAsk);

                    anzZeilen++;
                }

                int len1 = anzZeilen * 6 + 2;

                var myPDU = _dc.createPDU();

                byte[] para;
                byte[] data;

                para = new byte[] {0x00, 0x01, 0x12, 0x08, 0x12, 0x41, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00};
                data = new byte[]
                {
                    0x00, 0x14, BitConverter.GetBytes(len1)[1], BitConverter.GetBytes(len1)[0], 0x00, 0x00, 0x00, 0x01,
                    0x00, 0x00, 0x00, 0x0e, 0x00, 0x01,
                    0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, (byte) ReadTrigger, 0x00,
                    BitConverter.GetBytes(anzZeilen)[1], BitConverter.GetBytes(anzZeilen)[0],
                    //0x01, 0x08, 0x00, 0x00, 0x00, 0x00 //Tag

                };

                data = Helper.CombineByteArray(data, askBytes.ToArray());

                _dc.daveBuildAndSendPDU(myPDU, para, data);

                byte[] rdata, rparam;
                int res = _dc.daveRecieveData(out rdata, out rparam);

                byte[] stid = new byte[] {rparam[6], rparam[7]};

                if (rparam[10] != 0x00 && rparam[11] != 0x00) // 0xd05f
                    throw new Exception("Error Reading Tags with Var Tab Functions, Error Code: 0x" +
                                        rparam[10].ToString("X").PadLeft(2, '0') +
                                        rparam[11].ToString("X").PadLeft(2, '0'));

                VarTabReadData retVal = new VarTabReadData(BitConverter.ToInt16(stid, 0),
                    General.IEnumerableExtensions.ToArray<PLCTag>(valueList), this);

                return retVal;
            }
        }

        public void WriteValuesWithVarTabFunctions(IEnumerable<PLCTag> valueList, PLCTriggerVarTab WriteTrigger)
        {
            lock (lockObj)
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

                    switch (plcTag.TagDataSource)
                    {
                        case MemoryArea.Flags:
                            dtaTyp = 0;
                            break;
                        case MemoryArea.Inputs:
                            dtaTyp = 1;
                            break;
                        case MemoryArea.Outputs:
                            dtaTyp = 2;
                            break;
                        case MemoryArea.Datablock:
                            dtaTyp = 0x07;
                            dbNo = plcTag.DataBlockNumber;
                            break;
                        case MemoryArea.Timer:
                            dtaTyp = 5;
                            break;
                        case MemoryArea.Counter:
                            dtaTyp = 6;
                            break;
                        case MemoryArea.LocalData:
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
                            if (plcTag.TagDataSource == MemoryArea.Timer || plcTag.TagDataSource == MemoryArea.Counter)
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

                    akAsk[0] = (byte) (dtaTyp * 0x10 + dtaSize);
                    akAsk[1] = (byte) dtaArrSize;
                    akAsk[2] = (byte) (dbNo / 0x100);
                    akAsk[3] = (byte) (dbNo % 0x100);
                    akAsk[4] = (byte) (plcTag.ByteAddress / 0x100);
                    akAsk[5] = (byte) (plcTag.ByteAddress % 0x100);

                    controlBytes.AddRange(akAsk);

                    //Add Control Values ....
                    controlValues.AddRange(new byte[]
                        {0x00, 0x09, (byte) (plcTag.ReadByteSize / 0x100), (byte) (plcTag.ReadByteSize % 0x100)});
                    byte[] ctrl = new byte[plcTag.ReadByteSize];
                    plcTag._putControlValueIntoBuffer(ctrl, 0);
                    controlValues.AddRange(ctrl);

                    anzZeilen++;
                }

                int len1 = anzZeilen * 6 + 2 + controlValues.Count;

                byte[] para;
                byte[] data;

                var myPDU = _dc.createPDU();

                para = new byte[] {0x00, 0x01, 0x12, 0x08, 0x12, 0x41, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00};
                data = new byte[]
                {
                    //1 means use Trigger? maybe
                    0x00, 0x14, BitConverter.GetBytes(len1)[1], BitConverter.GetBytes(len1)[0], 0x00, 0x00,
                    0x00, /* 0x00 */ 0x01, 0x00, 0x01, 0x00, 0x04, 0x00, 0x01,
                    0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, /*0x00*/ (byte) WriteTrigger, 0x00,
                    BitConverter.GetBytes(anzZeilen)[1], BitConverter.GetBytes(anzZeilen)[0],
                    //0x01, 0x08, 0x00, 0x00, 0x00, 0x00 //Tag

                };

                data = Helper.CombineByteArray(data, controlBytes.ToArray());
                data = Helper.CombineByteArray(data, controlValues.ToArray());

                _dc.daveBuildAndSendPDU(myPDU, para, data);

                byte[] rdata, rparam;
                int res = _dc.daveRecieveData(out rdata, out rparam);

                byte[] stid = new byte[] {rparam[6], rparam[7]};

                if (rparam[10] != 0x00 && rparam[11] != 0x00) // 0xd05f
                    throw new Exception("Error Wrting Tags with VarTab Functions, Error Code: 0x" +
                                        rparam[10].ToString("X").PadLeft(2, '0') +
                                        rparam[11].ToString("X").PadLeft(2, '0'));
                /*
                VarTabWriteData retVal = new VarTabWriteData(BitConverter.ToInt16(stid, 0), General.IEnumerableExtensions.ToArray<PLCTag>(valueList), this);
    
                return retVal;
                */
            }
        }
        #endregion

        #region Read and Write Memory
        //Helper for Readvalues
        //Sort the PLC TAGs
        private class SorterForPLCTags : IComparer<PLCTag>
        {
            public int Compare(PLCTag pt1, PLCTag pt2)
            {
                //Beides DBs, vergleiche DBs
                if (pt1.TagDataSource == MemoryArea.Datablock && pt2.TagDataSource == MemoryArea.Datablock)
                {
                    if (pt1.DataBlockNumber == pt2.DataBlockNumber)
                        if (pt1.ByteAddress > pt2.ByteAddress)
                            return 1;
                        else if (pt1.ByteAddress < pt2.ByteAddress)
                            return -1;
                        else
                            return 0;
                    else
                        if (pt1.DataBlockNumber > pt2.DataBlockNumber)
                        return 1;
                    else
                        return -1;

                    return 0;
                }
                //nicht beides DBs, vergleiche nur DataSource und danach Byte Adresse
                else
                {
                    if (pt1.TagDataSource == pt2.TagDataSource)
                        if (pt1.ByteAddress > pt2.ByteAddress)
                            return 1;
                        else if (pt1.ByteAddress < pt2.ByteAddress)
                            return -1;
                        else
                            return 0;
                    else
                        if (pt1.TagDataSource > pt2.TagDataSource)
                        return 1;
                    else
                        return -1;

                }
            }
        }

        private class pduRead
        {
            public IPDU pdu;
            //public int gesAskSize = 0;
            public int gesReadSize = 0;
            //Size f the current ask request, that means every Tag adds 12 Bytes (symbolic tia Tags add more)
            public bool lastRequestWasAUnevenRequest = false;
            public List<bool> usedShortRequest = new List<bool>(50);
            public List<int> readenSizes = new List<int>(50);
            //normaly on a 400 CPU, max 38 Tags fit into a PDU, so this List as a Start would be enough
            //With Short Request this could be a little more so we use 50
            public int anzVar = 0;
            //public int anzReadVar = 0;
            public pduRead(IPDU p)
            {
                pdu = p;
            }
        }


        public int? ForcedPduSize { get; set; }

        private int GetPduSize()
        {
            if (ForcedPduSize.HasValue)
                return ForcedPduSize.Value;

            return _dc.getMaxPDULen();
        }

        /// <summary>
        /// A new impl. of read Values...
        /// Need to test it befor, maybe I switch to this....
        /// </summary>
        /// <param name="valueList"></param>
        /// <param name="useReadOptimization"></param>
        private void _TestNewReadValues(IEnumerable<PLCTag> valueList, bool useReadOptimization)
        {
            if (Configuration.ConnectionType == LibNodaveConnectionTypes.AS_511) //AS511
            {
                foreach (var plcTag in valueList)
                {
                    this.ReadValue(plcTag);
                }
                return;
            }

            lock (lockObj)
            {
                //Got through the list of values
                //Order them at first with the DB, then the byte address
                //If the Byte count of a tag is uneven, add 1
                //Then Look if Some Values lay in othe values or if the byte adress difference is <= 4
                //if it is so, create a replacement value wich reads the bytes and stores at wich tags are in this value and at wich adress
                //read the tags!
                //Look, that the byte count gets not bigger than a pdu!            

                CheckConnection();

                if (_dc != null)
                {

                    IEnumerable<PLCTag> readTagList = valueList;

                    #region Optimize Reading List....

                    if (useReadOptimization)
                    {
                        List<PLCTag> orderedList = new List<PLCTag>();
                        orderedList.AddRange(valueList);
                        orderedList.Sort(new SorterForPLCTags());

                        List<PLCTag> intReadTagList = new List<PLCTag>();

                        //Go through the List of PLC Tags and Combine the ones, where the Byte Addres does not differ more than 4 Bytes...
                        MemoryArea oldDataSource = 0;
                        int oldDB = 0, oldByteAddress = 0, oldLen = 0;
                        int cntCombinedTags = 0;
                        PLCTag lastTag = null;
                        PLCTagReadHelper rdHlp = new PLCTagReadHelper() { TagDataType = TagDataType.ByteArray };
                        foreach (PLCTag plcTag in orderedList)
                        {
                            if (cntCombinedTags == 0)
                            {
                                oldDataSource = plcTag.TagDataSource;
                                oldDB = plcTag.DataBlockNumber;
                                oldByteAddress = plcTag.ByteAddress;
                                oldLen = plcTag._internalGetSize();
                                lastTag = plcTag;
                                cntCombinedTags++;
                            }
                            else
                            {
                                if (oldDataSource == plcTag.TagDataSource &&
                                    (oldDataSource != MemoryArea.Datablock || oldDB == plcTag.DataBlockNumber) &&
                                    plcTag.ByteAddress <= oldByteAddress + oldLen + 4)
                                {
                                    //todo: test if this is correct
                                    if (cntCombinedTags == 1) rdHlp.PLCTags.Add(lastTag, 0);

                                    cntCombinedTags++;
                                    int newlen = plcTag._internalGetSize() + (plcTag.ByteAddress - oldByteAddress);
                                    oldLen = oldLen < newlen ? newlen : oldLen;
                                    if (oldLen % 2 != 0) oldLen++;
                                    rdHlp.PLCTags.Add(plcTag, plcTag.ByteAddress - oldByteAddress);
                                    rdHlp.ByteAddress = oldByteAddress;
                                    rdHlp.ArraySize = oldLen;
                                    rdHlp.TagDataSource = oldDataSource;
                                    rdHlp.DataBlockNumber = oldDB;
                                }
                                else
                                {
                                    if (cntCombinedTags > 1)
                                    {
                                        intReadTagList.Add(rdHlp);
                                        rdHlp = new PLCTagReadHelper() { TagDataType = TagDataType.ByteArray };
                                        cntCombinedTags = 0;
                                    }
                                    else
                                    {
                                        intReadTagList.Add(lastTag);
                                        cntCombinedTags = 0;
                                    }

                                    oldDataSource = plcTag.TagDataSource;
                                    oldDB = plcTag.DataBlockNumber;
                                    oldByteAddress = plcTag.ByteAddress;
                                    oldLen = plcTag._internalGetSize();
                                    if (oldLen % 2 != 0) oldLen++;
                                    lastTag = plcTag;
                                    cntCombinedTags++;
                                }
                            }

                        }
                        if (cntCombinedTags > 1) intReadTagList.Add(rdHlp);
                        else if (cntCombinedTags == 1) intReadTagList.Add(lastTag);

                        readTagList = intReadTagList;
                    }

                    #endregion



                    //Count how Many Bytes from the PLC should be read and create a Byte Array for the Values
                    int completeReadSize = 0;
                    foreach (var libNoDaveValue in readTagList)
                    {
                        completeReadSize += libNoDaveValue._internalGetSize();
                    }
                    byte[] completeData = new byte[completeReadSize];


                    //Get the Maximum Answer Len for One PDU
                    int maxReadSize = GetPduSize() - 32; //32 = Header

                    //int maxReadVar = maxReadSize / 12; //12 Header Größe Variablenanfrage


                    int positionInCompleteData = 0;
                    int akVar = 0;

                    int akByteAddress = 0;

                    //libnodave.PDU myPDU = _dc.prepareReadRequest();
                    List<pduRead> listPDU = new List<pduRead>();
                    pduRead curReadPDU = new pduRead(_dc.prepareReadRequest());
                    listPDU.Add(curReadPDU);
                    int HeaderTagSize = 4; //Todo: If I use the Short Request, the Header in the answer is 5 Bytes, not 4! Look how to do this...

                    foreach (var libNoDaveValue in readTagList)
                    {
                        bool shortDbRequest = false;
                        int askSize = 12;
                        HeaderTagSize = 4;
                        if (libNoDaveValue.TagDataSource == MemoryArea.Datablock && this._configuration.UseShortDataBlockRequest)
                        {
                            shortDbRequest = true;
                            askSize = 7;
                            HeaderTagSize = 5;
                        }

                        bool symbolicTag = false;

                        if (!string.IsNullOrEmpty(libNoDaveValue.SymbolicAccessKey))
                        {
                            askSize = HeaderTagSize + libNoDaveValue.SymbolicAccessKey.Length;
                            symbolicTag = true;
                        }
                        //Save the Byte Address in anthoer Variable, because if we split the Read Request, we need not the real Start Address
                        akByteAddress = libNoDaveValue.ByteAddress;

                        if (libNoDaveValue.TagDataSource != MemoryArea.Datablock &&
                            libNoDaveValue.TagDataSource != MemoryArea.InstanceDatablock)
                            libNoDaveValue.DataBlockNumber = 0;

                        int readSize = libNoDaveValue._internalGetSize();

                        //tryAgain:
                        while (readSize > 0)
                        {

                            int readSizeWithHeader = readSize + HeaderTagSize; //HeaderTagSize Bytes Header for each Tag
                            readSizeWithHeader += readSizeWithHeader % 2;//Ungerade Anzahl Bytes, noch eines dazu...

                            var currentAskSize = askSize;       //
                            if (curReadPDU.lastRequestWasAUnevenRequest)
                            {
                                currentAskSize++;
                            }

                            int restBytes = Math.Min(maxReadSize - curReadPDU.gesReadSize, readSizeWithHeader) - HeaderTagSize;//len read: or real full len, or remaining free

                            if (restBytes < HeaderTagSize || symbolicTag || (curReadPDU.gesReadSize > 0 && libNoDaveValue.DontSplitValue && curReadPDU.gesReadSize + readSizeWithHeader > maxReadSize))
                            {//or remaining free < HeaderTagSize, or Simbol, or Value don't split and full value can don't read without split and PDU nit empty (if PDU empty Value DontSplitValue is spliting)
                                listPDU.Add(curReadPDU = new pduRead(_dc.prepareReadRequest()));//current PDU is END, create new PDU
                                continue;//to while (readSize > 0)
                            }

                            if (curReadPDU.lastRequestWasAUnevenRequest)
                            {
                                //curReadPDU.gesAskSize++;
                                curReadPDU.pdu.daveAddFillByteToReadRequest();
                                curReadPDU.lastRequestWasAUnevenRequest = false;
                            }

                            if (symbolicTag)
                            {
                                curReadPDU.usedShortRequest.Add(false);
                                curReadPDU.pdu.addSymbolVarToReadRequest(libNoDaveValue.SymbolicAccessKey);
                            }
                            //Only at the rest of the bytes to the next read request, and increase the start address!  
                            else if (shortDbRequest)
                            {
                                curReadPDU.usedShortRequest.Add(true);
                                curReadPDU.lastRequestWasAUnevenRequest = true;
                                curReadPDU.pdu.addDbRead400ToReadRequest(libNoDaveValue.DataBlockNumber, akByteAddress, restBytes);
                            }
                            else
                            {
                                curReadPDU.usedShortRequest.Add(false);
                                curReadPDU.pdu.addVarToReadRequest(Convert.ToInt32(libNoDaveValue.TagDataSource), libNoDaveValue.DataBlockNumber, akByteAddress, restBytes);
                            }

                            readSize -= restBytes;

                            curReadPDU.gesReadSize += restBytes + HeaderTagSize;//readSizeWithHeader
                            //if (symbolicTag)
                            //    curReadPDU.gesAskSize += askSize;

                            akByteAddress += restBytes;

                            curReadPDU.readenSizes.Add(restBytes);
                            curReadPDU.anzVar++;
                            //listPDU.Add(curReadPDU = new pduRead(_dc.prepareReadRequest()));//current PDU is FULL, create new PDU
                            //useresult muss noch programmiert werden.
                        }
                    }
                    //if ( curReadPDU.gesReadSize > 0)
                    //    listPDU.Add(curReadPDU);

                    List<bool> NotExistedValue = new List<bool>();

                    //Only for debugging...

                    foreach (var cPDU in listPDU)
                    {
                        if (cPDU.gesReadSize > 0)
                        {
                            var rs = _dc.getResultSet();
                            int res;
                            lock (lockObj)
                            {
                                res = _dc.execReadRequest(cPDU.pdu, rs);
                            }

                            if (AutoDisconnect && (res == -1025 || res == -128))
                            {
                                if (Logger != null)
                                    Logger("(1) Auto Disconnect cause of :" + libnodave.daveStrerror(res));
                                this.Disconnect();
                                return;
                            }
                            else if (res != 0 && res != 10)
                                throw new PLCException(res);

                            //positionInCompleteData = 0;
                            //Save the Read Data to a User Byte Array (Because we use this in the libnodavevalue class!)

                            for (akVar = 0; akVar < cPDU.anzVar; akVar++)
                            {
                                byte[] myBuff = new byte[ /* gesReadSize */cPDU.readenSizes[akVar] + 1];

                                lock (lockObj)
                                {
                                    res = _dc.useResultBuffer(rs, akVar, myBuff);
                                }

                                if (res == 10 || res == 5)
                                {
                                    NotExistedValue.Add(true);
                                }
                                else if (res != 0)
                                {
                                    var details = Environment.NewLine + Environment.NewLine + "AnzVar " +
                                                  cPDU.anzVar.ToString() + "; akVar " + akVar.ToString() +
                                                  Environment.NewLine;

                                    details += "readsizes " + string.Join(";", cPDU.readenSizes) + Environment.NewLine;
                                    details += "usedShortRequest " + string.Join(";", cPDU.usedShortRequest) + Environment.NewLine;
                                    throw new PLCException("Error (1): " + _errorCodeConverter(res) + details, res);
                                }
                                else
                                {
                                    int myBuffStart = 0;
                                    if (cPDU.usedShortRequest[akVar])
                                        myBuffStart = 1;

                                    if (cPDU.usedShortRequest[akVar] && (myBuff[0] == 10 || myBuff[0] == 5))
                                    {
                                        NotExistedValue.Add(true);
                                    }
                                    else
                                    {
                                        NotExistedValue.Add(false);
                                        Array.Copy(myBuff, myBuffStart, completeData, positionInCompleteData, cPDU.readenSizes[akVar]);
                                        positionInCompleteData += cPDU.readenSizes[akVar];
                                    }
                                    //for (int n = 0; n < readenSizes[akVar]; n++)
                                    //{
                                    //    completeData[positionInCompleteData++] = myBuff[n]; // Convert.ToByte(_dc.getU8());
                                    //}
                                }
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
                            value._setValueProp = null;
                        }
                        nr++;
                    }
                }
            }
        }

        /// <summary>
        /// This Function Reads Values from the PLC it needs a Array of LibNodaveValues
        /// It tries to Optimize how the Values are Read from the PLC
        /// </summary>
        /// <param name="valueList">The List of values to be read form the controller</param>   
        public void ReadValues(IEnumerable<PLCTag> valueList)
        {
            ReadValues(valueList, true);
        }

        Dictionary<int, int> _dbSizes = null;

        /// <summary>
        /// This function read Values from the PLC but also tries to verify the data-block sizes against 
        /// the current sizes in the controller. If an requested TAG exceeds the data-blocks current size
        /// the item will be set to "ItemDoesNotExist". This will only affect Tags reading from data-blocks
        /// </summary>
        /// <param name="valueList">The list of tags to read from the controller</param>
        /// <param name="cacheDbSizes">Read the data-blocks length and cache them for future requests.</param>
        /// <remarks>This function potentially improves read performance when there are many tags with different data block length.
        /// if the tag exceeds the data-blocks length, it will fail before sending it to the controller</remarks>
        public void ReadValuesWithCheck(IEnumerable<PLCTag> valueList, bool cacheDbSizes = false)
        {
            if (Configuration.ConnectionType == LibNodaveConnectionTypes.Fetch_Write_Passive || Configuration.ConnectionType == LibNodaveConnectionTypes.Fetch_Write_Active)
            {
                ReadValuesFetchWrite(valueList, false);
                return;
            }

            var tags = valueList.ToList();

            if (!cacheDbSizes || _dbSizes == null)
            {
                var dbList = tags.Where(x => x.TagDataSource == MemoryArea.Datablock || x.TagDataSource == MemoryArea.InstanceDatablock).Select(x => x.DataBlockNumber).Distinct();

                _dbSizes = new Dictionary<int, int>();
                foreach (var db in dbList)
                {
                    var size = this.PLCGetDataBlockSize("DB" + db);
                    _dbSizes.Add(db, size);
                }
            }

            var readList = new List<PLCTag>(tags.Count());

            foreach (var tag in tags)
            {
                if ((tag.TagDataSource == MemoryArea.Datablock || tag.TagDataSource == MemoryArea.InstanceDatablock) && _dbSizes[tag.DataBlockNumber] < tag.ByteAddress + tag.ReadByteSize)
                {
                    tag.ItemDoesNotExist = true;
                }
                else
                {
                    readList.Add(tag);
                }
            }

            ReadValues(readList, false);

            if (!cacheDbSizes)
                _dbSizes = null;
        }

        /// <summary>
        /// This Function Reads Values from the PLC it needs a Array of LibNodaveValues
        /// It tries to Optimize how the Values are Read from the PLC
        /// </summary>
        /// <param name="valueList">The List of values to be read form the controller</param>   
        public void ReadValues(IEnumerable<PLCTag> valueList, bool useReadOptimization)
        {
            if (Configuration.ConnectionType == LibNodaveConnectionTypes.AS_511) //AS511
            {
                foreach (var plcTag in valueList)
                {
                    this.ReadValue(plcTag);
                }
                return;
            }

            if (Configuration.ConnectionType == LibNodaveConnectionTypes.Fetch_Write_Active || Configuration.ConnectionType == LibNodaveConnectionTypes.Fetch_Write_Passive)
            {
                ReadValuesFetchWrite(valueList, useReadOptimization);
                return;
            }

            var lockObtained = false;
            //lock (lockObj)
            try
            {
                //Got through the list of values
                //Order them at first with the DB, then the byte address
                //If the Byte count of a tag is uneven, add 1
                //Then Look if Some Values lay in othe values or if the byte adress difference is <= 4
                //if it is so, create a replacement value wich reads the bytes and stores at wich tags are in this value and at wich adress
                //read the tags!
                //Look, that the byte count gets not bigger than a pdu!            

                CheckConnection();

                if (_dc != null)
                {

                    IEnumerable<PLCTag> readTagList = valueList;

                    #region Optimize Reading List....

                    if (useReadOptimization && !(valueList.First() is PLCNckTag))
                    {
                        List<PLCTag> orderedList = new List<PLCTag>();
                        orderedList.AddRange(valueList);
                        orderedList.Sort(new SorterForPLCTags());

                        List<PLCTag> intReadTagList = new List<PLCTag>();

                        //Go through the List of PLC Tags and Combine the ones, where the Byte Addres does not differ more than 4 Bytes...
                        MemoryArea oldDataSource = 0;
                        int oldDB = 0, oldByteAddress = 0, oldLen = 0;
                        int cntCombinedTags = 0;
                        PLCTag lastTag = null;
                        PLCTagReadHelper rdHlp = new PLCTagReadHelper() { TagDataType = TagDataType.ByteArray };
                        foreach (PLCTag plcTag in orderedList)
                        {
                            if (cntCombinedTags == 0)
                            {
                                oldDataSource = plcTag.TagDataSource;
                                oldDB = plcTag.DataBlockNumber;
                                oldByteAddress = plcTag.ByteAddress;
                                oldLen = plcTag._internalGetSize();
                                lastTag = plcTag;
                                cntCombinedTags++;
                            }
                            else
                            {
                                if (oldDataSource == plcTag.TagDataSource &&
                                    (oldDataSource != MemoryArea.Datablock || oldDB == plcTag.DataBlockNumber) &&
                                    plcTag.ByteAddress <= oldByteAddress + (oldLen % 2 != 0 ? oldLen +1 : oldLen) + 4)
                                {
                                    if (cntCombinedTags == 1)
                                        rdHlp.PLCTags.Add(lastTag, 0);

                                    cntCombinedTags++;
                                    int newlen = plcTag._internalGetSize() + (plcTag.ByteAddress - oldByteAddress);
                                    oldLen = oldLen < newlen ? newlen : oldLen;
                                    rdHlp.PLCTags.Add(plcTag, plcTag.ByteAddress - oldByteAddress);
                                    rdHlp.ByteAddress = oldByteAddress;
                                    rdHlp.ArraySize = oldLen;
                                    rdHlp.TagDataSource = oldDataSource;
                                    rdHlp.DataBlockNumber = oldDB;
                                }
                                else
                                {
                                    if (cntCombinedTags > 1)
                                    {
                                        intReadTagList.Add(rdHlp);
                                        rdHlp = new PLCTagReadHelper() { TagDataType = TagDataType.ByteArray };
                                        cntCombinedTags = 0;
                                    }
                                    else
                                    {
                                        intReadTagList.Add(lastTag);
                                        cntCombinedTags = 0;
                                    }

                                    oldDataSource = plcTag.TagDataSource;
                                    oldDB = plcTag.DataBlockNumber;
                                    oldByteAddress = plcTag.ByteAddress;
                                    oldLen = plcTag._internalGetSize();
                                    lastTag = plcTag;
                                    cntCombinedTags++;
                                }
                            }

                        }
                        if (cntCombinedTags > 1) intReadTagList.Add(rdHlp);
                        else if (cntCombinedTags == 1) intReadTagList.Add(lastTag);

                        readTagList = intReadTagList;
                    }

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
                    int maxReadSize = GetPduSize() - 32; //32 = Header

                    // Die NC akzeptiert im Drive Bereich nicht die ausgehandelte PDU Größe
                    // maximal 240 - 32 (Header) => 208
                    if (maxReadSize > 208 && readTagList.First() is PLCNckTag && ((PLCNckTag)readTagList.First()).NckArea == NCK_Area.AreaFeedDrive)
                        maxReadSize = 208;

                    //int maxReadVar = maxReadSize / 12; //12 Header Größe Variablenanfrage

                    List<int> readenSizes = new List<int>(50);
                    List<bool> usedShortRequest = new List<bool>(50);
                    List<bool> tagWasSplitted = new List<bool>(50);
                    //normaly on a 400 CPU, max 38 Tags fit into a PDU, so this List as a Start would be enough
                    //With Short Request this could be a little more so we use 50

                    int gesReadSize = 0;
                    int positionInCompleteData = 0;
                    int akVar = 0;
                    int anzVar = 0;
                    int anzReadVar = 0;

                    int akByteAddress = 0;

                    int gesAskSize = 0;
                    //Size f the current ask request, that means every Tag adds 12 Bytes (symbolic tia Tags add more)

                    bool lastRequestWasAUnevenRequest = false;

                    Monitor.Enter(lockObj, ref lockObtained);
                    var myPDU = _dc.prepareReadRequest();

                    var currentRead = new List<string>();

                    foreach (var libNoDaveValue in readTagList)
                    {
                        bool shortDbRequest = false;
                        int askSize = 12;
                        if (libNoDaveValue.TagDataSource == MemoryArea.Datablock && this._configuration.UseShortDataBlockRequest)
                        {
                            shortDbRequest = true;
                            askSize = 7;
                        }

                        bool symbolicTag = false;

                        var nckT = libNoDaveValue as PLCNckTag;

                        if (!string.IsNullOrEmpty(libNoDaveValue.SymbolicAccessKey))
                        {
                            askSize = 4 + libNoDaveValue.SymbolicAccessKey.Length;
                            symbolicTag = true;
                        }
                        //Save the Byte Address in anthoer Variable, because if we split the Read Request, we need not the real Start Address
                        akByteAddress = libNoDaveValue.ByteAddress;

                        if (libNoDaveValue.TagDataSource != MemoryArea.Datablock &&
                            libNoDaveValue.TagDataSource != MemoryArea.InstanceDatablock)
                            libNoDaveValue.DataBlockNumber = 0;

                        int readSize = libNoDaveValue._internalGetSize();

                        const int HeaderTagSize = 4; //Todo: If I use the Short Request, the Header in the answer is 5 Bytes, not 4! Look how to do this...

                        tryAgain:
                        int readSizeWithHeader = readSize + HeaderTagSize; //HeaderTagSize Bytes Header for each Tag
                        if (readSizeWithHeader % 2 != 0) //Ungerade Anzahl Bytes, noch eines dazu...
                            readSizeWithHeader++;

                        var currentAskSize = askSize;       //
                        if (lastRequestWasAUnevenRequest)
                            currentAskSize++;
                        //When there are too much bytes in the answer pdu, or you read more then the possible tags...
                        //But don't split if the bit is set (but ignore it if the tag is bigger then the pdu size!)
                        if ((readSizeWithHeader + gesReadSize > maxReadSize || gesAskSize + currentAskSize > maxReadSize))
                        {
                            //If there is space for a tag left.... Then look how much Bytes we can put into this PDU
                            if (nckT == null && !symbolicTag && gesAskSize + currentAskSize <= maxReadSize && (!libNoDaveValue.DontSplitValue || readSize > maxReadSize))
                            {
                                #region Without NCK
                                int restBytes = maxReadSize - gesReadSize - HeaderTagSize;
                                //Howmany Bytes can be added to this call
                                if (restBytes > 0)
                                {
                                    if (lastRequestWasAUnevenRequest)
                                    {
                                        myPDU.daveAddFillByteToReadRequest();
                                        lastRequestWasAUnevenRequest = false;
                                    }

                                    //Only at the rest of the bytes to the next read request, and increase the start address!  
                                    if (shortDbRequest)
                                    {
                                        usedShortRequest.Add(true);
                                        lastRequestWasAUnevenRequest = true;
                                        currentRead.Add(string.Format("shortDbRequest, db:{0}, byte:{1}, size{2}", libNoDaveValue.DataBlockNumber, akByteAddress, readSize));
                                        myPDU.addDbRead400ToReadRequest(libNoDaveValue.DataBlockNumber, akByteAddress, restBytes);
                                    }
                                    else
                                    {
                                        usedShortRequest.Add(false);
                                        currentRead.Add(string.Format("addVarToReadRequest, source:{0}, db:{1}, byte:{2}, size{3}", libNoDaveValue.TagDataSource, libNoDaveValue.DataBlockNumber, akByteAddress, readSize));
                                        myPDU.addVarToReadRequest(Convert.ToInt32(libNoDaveValue.TagDataSource), libNoDaveValue.DataBlockNumber, akByteAddress, restBytes);
                                    }

                                    readSize = readSize - restBytes;

                                    //Tag was splitted to more than one read request
                                    if (readSize > 0)
                                        tagWasSplitted.Add(true);
                                    else
                                        tagWasSplitted.Add(false);

                                    gesReadSize += restBytes;

                                    akByteAddress += restBytes;

                                    readenSizes.Add(restBytes);
                                    anzVar++;

                                    //useresult muss noch programmiert werden.
                                }
                                #endregion
                            }
                            var rs = _dc.getResultSet();
                            int res;
                            //lock (lockObj)
                            res = _dc.execReadRequest(myPDU, rs);
                            Monitor.Exit(lockObj);
                            lockObtained = false;

                            if (AutoDisconnect && (res == -1025 || res == -128))
                            {
                                if (Logger != null)
                                    Logger("(2) Auto Disconnect cause of :" + libnodave.daveStrerror(res));
                                this.Disconnect();
                                return;
                            }
                            else if (res != 0)
                                throw new PLCException(res);

                            //Save the Read Data to a User Byte Array (Because we use this in the libnodavevalue class!)                    
                            for (akVar = 0; akVar < anzVar; akVar++)
                            {
                                byte[] myBuff = new byte[gesReadSize];

                                res = _dc.useResultBuffer(rs, akVar, myBuff);
                                if (res == 10 || res == 5)
                                {
                                    if (!tagWasSplitted[akVar])
                                        NotExistedValue.Add(true);
                                }
                                else if (res != 0)
                                {
                                    var details = Environment.NewLine + Environment.NewLine + "AnzVar " +
                                                   anzVar.ToString() + "; akVar " + akVar.ToString() +
                                                   Environment.NewLine;

                                    details += "readsizes " + string.Join(";", readenSizes) + Environment.NewLine;
                                    details += "usedShortRequest " + string.Join(";", usedShortRequest) + Environment.NewLine;
                                    details += Environment.NewLine + Environment.NewLine +
                                               string.Join(Environment.NewLine, currentRead) + Environment.NewLine +
                                               Environment.NewLine;

                                    throw new PLCException("Error (2): " + _errorCodeConverter(res) + details, res);
                                }
                                else
                                {
                                    int myBuffStart = 0;
                                    if (usedShortRequest[akVar])
                                        myBuffStart = 1;
                                    if (usedShortRequest[akVar] && (myBuff[0] == 10 || myBuff[0] == 5))
                                    {
                                        if (!tagWasSplitted[akVar])
                                            NotExistedValue.Add(true);
                                    }
                                    else
                                    {
                                        if (!tagWasSplitted[akVar])
                                            NotExistedValue.Add(false);
                                        Array.Copy(myBuff, myBuffStart, completeData, positionInCompleteData, readenSizes[akVar]);
                                        positionInCompleteData += readenSizes[akVar];
                                    }
                                    //for (int n = 0; n < readenSizes[akVar]; n++)
                                    //{
                                    //    completeData[positionInCompleteData++] = myBuff[n]; //Convert.ToByte(_dc.getU8());
                                    //}
                                }
                            }

                            //rs = null;
                            //myPDU = null;
                            currentRead = new List<string>();
                            anzVar = 0;
                            gesAskSize = 0;
                            Monitor.Enter(lockObj, ref lockObtained);
                            myPDU = _dc.prepareReadRequest();
                            gesReadSize = 0;
                            anzReadVar = 0;
                            readenSizes.Clear();
                            tagWasSplitted.Clear();
                            usedShortRequest.Clear();
                            //I need to do the whole splitting in anothe way, so that the goto disapears! But for the moment ir works!
                            goto tryAgain;
                            //It tries again the Size test, this is necessary, when the Tag is bigger then one PDU
                        }

                        gesReadSize = gesReadSize + readSizeWithHeader;
                        gesAskSize += askSize;
                        readenSizes.Add(readSize);
                        anzVar++;
                        anzReadVar++;

                        if (lastRequestWasAUnevenRequest)
                        {
                            gesAskSize++;
                            myPDU.daveAddFillByteToReadRequest();
                            lastRequestWasAUnevenRequest = false;
                        }

                        if (nckT != null)
                        {
                            usedShortRequest.Add(false);
                            tagWasSplitted.Add(false);
                            myPDU.addNCKToReadRequest((int)nckT.NckArea, nckT.NckUnit, nckT.NckColumn, nckT.NckLine, nckT.NckModule, nckT.NckLinecount);
                        }
                        else if (symbolicTag)
                        {
                            usedShortRequest.Add(false);
                            tagWasSplitted.Add(false);
                            myPDU.addSymbolVarToReadRequest(libNoDaveValue.SymbolicAccessKey);
                        }
                        else if (shortDbRequest)
                        {
                            usedShortRequest.Add(true);
                            tagWasSplitted.Add(false);
                            lastRequestWasAUnevenRequest = true;
                            currentRead.Add(string.Format("shortDbRequest, db:{0}, byte:{1}, size{2}", libNoDaveValue.DataBlockNumber, akByteAddress, readSize));
                            myPDU.addDbRead400ToReadRequest(libNoDaveValue.DataBlockNumber, akByteAddress, readSize);
                        }
                        else
                        {
                            usedShortRequest.Add(false);
                            tagWasSplitted.Add(false);
                            currentRead.Add(string.Format("addVarToReadRequest, source:{0}, db:{1}, byte:{2}, size{3}", libNoDaveValue.TagDataSource, libNoDaveValue.DataBlockNumber, akByteAddress, readSize));
                            myPDU.addVarToReadRequest(Convert.ToInt32(libNoDaveValue.TagDataSource), libNoDaveValue.DataBlockNumber, akByteAddress, readSize);
                        }
                    }

                    if (gesReadSize > 0)
                    {
                        var rs = _dc.getResultSet();
                        int res;
                        res = _dc.execReadRequest(myPDU, rs);
                        Monitor.Exit(lockObj);
                        lockObtained = false;
                        if (AutoDisconnect && (res == -1025 || res == -128))
                        {
                            if (Logger != null)
                                Logger("(3) Auto Disconnect cause of :" + libnodave.daveStrerror(res));
                            this.Disconnect();
                            return;
                        }
                        else if (res != 0 && res != 10)
                            throw new PLCException(res);

                        //positionInCompleteData = 0;
                        //Save the Read Data to a User Byte Array (Because we use this in the libnodavevalue class!)

                        for (akVar = 0; akVar < anzVar; akVar++)
                        {
                            byte[] myBuff = new byte[ /* gesReadSize */readenSizes[akVar] + 1];

                            lock (lockObj)
                            {
                                res = _dc.useResultBuffer(rs, akVar, myBuff);
                            }

                            if (res == 10 || res == 5)
                            {
                                NotExistedValue.Add(true);
                            }
                            else if (res != 0)
                            {
                                var details = Environment.NewLine + Environment.NewLine + "AnzVar " +
                                                   anzVar.ToString() + "; akVar " + akVar.ToString() +
                                                   Environment.NewLine;

                                details += "variables " + string.Join(";", readTagList.Select(x => x.ValueName ?? "")) + Environment.NewLine;
                                details += "readsizes " + string.Join(";", readenSizes) + Environment.NewLine;
                                details += "usedShortRequest " + string.Join(";", usedShortRequest) + Environment.NewLine;
                                throw new PLCException("Error (3): " + _errorCodeConverter(res) + details, res);
                            }
                            else
                            {
                                int myBuffStart = 0;
                                if (usedShortRequest[akVar])
                                    myBuffStart = 1;

                                if (usedShortRequest[akVar] && (myBuff[0] == 10 || myBuff[0] == 5))
                                {
                                    NotExistedValue.Add(true);
                                }
                                else
                                {
                                    NotExistedValue.Add(false);
                                    Array.Copy(myBuff, myBuffStart, completeData, positionInCompleteData, readenSizes[akVar]);
                                    positionInCompleteData += readenSizes[akVar];
                                }
                                //for (int n = 0; n < readenSizes[akVar]; n++)
                                //{
                                //    completeData[positionInCompleteData++] = myBuff[n]; // Convert.ToByte(_dc.getU8());
                                //}
                            }
                        }
                    }

                    if (lockObtained)
                        Monitor.Exit(lockObj);
                    lockObtained = false;

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
                            value._setValueProp = null;
                        }
                        nr++;
                    }
                }
            }
            finally {
                if (lockObtained)
                {
                    Monitor.Exit(lockObj);
                }
            }
        }

        internal void WriteValuesFetchWrite(IEnumerable<PLCTag> valueList)
        {
            foreach (var libNoDaveValue in valueList)
            {
                _fetchWriteConnection.WriteValue(libNoDaveValue);
            }
        }

        /// <summary>
        /// Read PLC Tags using the Fetch/Write mechanism
        /// </summary>
        /// <param name="valueList"></param>
        /// <param name="useReadOptimization"></param>
        internal void ReadValuesFetchWrite(IEnumerable<PLCTag> valueList, bool useReadOptimization)
        {
            lock (lockObj)
            {
                if (_fetchWriteConnection != null)
                {
                    IEnumerable<PLCTag> readTagList = valueList;

                    #region Optimize Reading List....

                    if (useReadOptimization)
                    {
                        List<PLCTag> orderedList = new List<PLCTag>();
                        orderedList.AddRange(valueList);
                        orderedList.Sort(new SorterForPLCTags());

                        List<PLCTag> intReadTagList = new List<PLCTag>();

                        //Go through the List of PLC Tags and Combine the ones, where the Byte Addres does not differ more than 4 Bytes...
                        MemoryArea oldDataSource = 0;
                        int oldDB = 0, oldByteAddress = 0, oldLen = 0;
                        int cntCombinedTags = 0;
                        PLCTag lastTag = null;
                        PLCTagReadHelper rdHlp = new PLCTagReadHelper() { TagDataType = TagDataType.ByteArray };
                        foreach (PLCTag plcTag in orderedList)
                        {
                            if (cntCombinedTags == 0)
                            {
                                oldDataSource = plcTag.TagDataSource;
                                oldDB = plcTag.DataBlockNumber;
                                oldByteAddress = plcTag.ByteAddress;
                                oldLen = plcTag._internalGetSize();
                                lastTag = plcTag;
                                cntCombinedTags++;
                            }
                            else
                            {
                                if (oldDataSource == plcTag.TagDataSource &&
                                    (oldDataSource != MemoryArea.Datablock || oldDB == plcTag.DataBlockNumber) &&
                                    plcTag.ByteAddress <= oldByteAddress + oldLen + 14)
                                {
                                    //todo: test if this is correct
                                    if (cntCombinedTags == 1)
                                        rdHlp.PLCTags.Add(lastTag, 0);

                                    cntCombinedTags++;
                                    int newlen = plcTag._internalGetSize() + (plcTag.ByteAddress - oldByteAddress);
                                    oldLen = oldLen < newlen ? newlen : oldLen;
                                    if (oldLen % 2 != 0)
                                        oldLen++;
                                    rdHlp.PLCTags.Add(plcTag, plcTag.ByteAddress - oldByteAddress);
                                    rdHlp.ByteAddress = oldByteAddress;
                                    rdHlp.ArraySize = oldLen;
                                    rdHlp.TagDataSource = oldDataSource;
                                    rdHlp.DataBlockNumber = oldDB;
                                }
                                else
                                {
                                    if (cntCombinedTags > 1)
                                    {
                                        intReadTagList.Add(rdHlp);
                                        rdHlp = new PLCTagReadHelper() { TagDataType = TagDataType.ByteArray };
                                        cntCombinedTags = 0;
                                    }
                                    else
                                    {
                                        intReadTagList.Add(lastTag);
                                        cntCombinedTags = 0;
                                    }

                                    oldDataSource = plcTag.TagDataSource;
                                    oldDB = plcTag.DataBlockNumber;
                                    oldByteAddress = plcTag.ByteAddress;
                                    oldLen = plcTag._internalGetSize();
                                    if (oldLen % 2 != 0)
                                        oldLen++;
                                    lastTag = plcTag;
                                    cntCombinedTags++;
                                }
                            }

                        }
                        if (cntCombinedTags > 1)
                            intReadTagList.Add(rdHlp);
                        else if (cntCombinedTags == 1)
                            intReadTagList.Add(lastTag);

                        readTagList = intReadTagList;
                    }

                    #endregion

                    foreach (var libNoDaveValue in readTagList)
                    {
                        _fetchWriteConnection.ReadValue(libNoDaveValue);
                    }
                }
            }
        }

        /// <summary>
        /// Read one single value from the PLC
        /// </summary>
        /// <param name="address">An Simatic Address Identifier. see <seealso cref="PLCTag"/> for syntax</param>
        /// <param name="type">The PLC data type to load and convert</param>
        /// <returns></returns>
        public object ReadValue(string address, TagDataType type)
        {
            var tag = new PLCTag(address, type);
            this.ReadValue(tag);
            return tag.Value;
        }

        /// <summary>
        /// Read one single value from the PLC
        /// </summary>
        /// <param name="address">An Simatic Address Identifier. see <seealso cref="PLCTag"/> for syntax</param>
        /// <param name="type">The PLC data type to load and convert</param>
        /// <returns></returns>
        public T ReadValue<T>(string address, TagDataType type)
        {
            var wrt = ReadValue(address, type);
            return (T)wrt;
        }

        /// <summary>
        /// Read one single value from the PLC
        /// </summary>
        /// <param name="address">An Simatic Address Identifier. see <seealso cref="PLCTag"/> for syntax</param>
        /// <returns></returns>
        public object ReadValue(string address)
        {
            var tag = new PLCTag(address);
            this.ReadValue(tag);
            return tag.Value;
        }

        /// <summary>
        /// Read one single value from the PLC
        /// </summary>
        /// <param name="address">An Simatic Address Identifier. see <seealso cref="PLCTag"/> for syntax</param>
        /// <returns></returns>
        public T ReadValue<T>(string address)
        {
            var wrt = ReadValue(address);
            return (T)wrt;
        }

        /// <summary>
        /// Read one single value from the NCK
        /// </summary>
        /// <param name="address">An Sinumerik Address Identifier. see <seealso cref="PLCNckTag"/> for syntax</param>
        /// <returns></returns>
        public object ReadValue(NC_Var address)
        {
            var tag = address.GetNckTag(0, 0);
            this.ReadValue(tag);
            return tag.Value;
        }

        /// <summary>
        /// Read one single value from the NCK
        /// </summary>
        /// <param name="address">An Sinumerik Address Identifier. see <seealso cref="PLCNckTag"/> for syntax</param>
        /// <returns></returns>
        public T ReadValue<T>(NC_Var address)
        {
            var wrt = ReadValue(address);
            return (T)wrt;
        }

        /// <summary>
        /// This Function Reads One LibNoDave Value from the PLC
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>        
        public void ReadValue(PLCTag value)
        {
            if (!string.IsNullOrEmpty(value.SymbolicAccessKey) && Configuration.ConnectionType != LibNodaveConnectionTypes.AS_511)
            {
                ReadValues(new[] { value });
                return;
            }

            if (value is PLCNckTag)
            {
                ReadValues(new[] { value });
                return;
            }

            if (Configuration.ConnectionType == LibNodaveConnectionTypes.Fetch_Write_Active || Configuration.ConnectionType == LibNodaveConnectionTypes.Fetch_Write_Passive)
            {
                ReadValuesFetchWrite(new[] { value }, false);
                return;
            }


            lock (lockObj)
            {
                CheckConnection();

                if (_dc != null)
                {

                    if (_dc == null)
                        throw new Exception("Error: Not Connected");

                    int readSize = value._internalGetSize();
                    byte[] myBuff = new byte[readSize];

                    if (value.TagDataSource != MemoryArea.Datablock &&
                        value.TagDataSource != MemoryArea.InstanceDatablock)
                        value.DataBlockNumber = 0;


                    int res = _dc.readManyBytes(Convert.ToInt32(value.TagDataSource), value.DataBlockNumber,
                        value.ByteAddress, readSize, ref myBuff);

                    int buffPos = 0;
                    if (res == 0)
                    {
                        value._readValueFromBuffer(myBuff, buffPos);
                        value.ItemDoesNotExist = false;
                    }
                    else if (AutoDisconnect && (res == -1025 || res == -128))
                    {
                        if (Logger != null)
                            Logger("(4) Auto Disconnect cause of :" + libnodave.daveStrerror(res));
                        this.Disconnect();
                        return;
                    }
                    else if (res == 5 || res == 10)
                        value.ItemDoesNotExist = true;
                    else
                        throw new PLCException(res);
                }
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
            int pos = startpos;

            foreach (var libNoDaveValue in values)
            {
                libNoDaveValue._readValueFromBuffer(bytearray, pos);
                pos += libNoDaveValue._internalGetSize();
            }
        }

        /// <summary>
        /// Writes a single Value to the PLC
        /// </summary>
        /// <param name="value"></param>
        public void WriteValue(PLCTag value)
        {
            if (value is PLCNckTag)
            {
                WriteValues(new[] { value });
                return;
            }

            lock (lockObj)
            {
                CheckConnection();

                if (Configuration.ConnectionType == LibNodaveConnectionTypes.Fetch_Write_Active || Configuration.ConnectionType == LibNodaveConnectionTypes.Fetch_Write_Passive)
                {
                    WriteValuesFetchWrite(new[] { value });
                    return;
                }

                if (_dc != null)
                {
                    if (_dc == null)
                        throw new Exception("Error: Not Connected");

                    value.RaiseValueChangedEvenWhenNoChangeHappened = true;

                    int readSize = value._internalGetSize();
                    byte[] myBuff = new byte[readSize];


                    value._putControlValueIntoBuffer(myBuff, 0);

                    if (value.TagDataSource != MemoryArea.Datablock &&
                        value.TagDataSource != MemoryArea.InstanceDatablock)
                        value.DataBlockNumber = 0;

                    int res;
                    if (value.TagDataType == TagDataType.Bool)
                    {
                        lock (lockObj)
                        {
                            res = _dc.writeBits(Convert.ToInt32(value.TagDataSource), value.DataBlockNumber,
                                value.ByteAddress * 8 + value.BitAddress, readSize, myBuff);
                        }
                    }
                    else
                    {
                        lock (lockObj)
                        {
                            res = _dc.writeManyBytes(Convert.ToInt32(value.TagDataSource), value.DataBlockNumber,
                                value.ByteAddress, readSize, myBuff);
                        }
                    }

                    if (AutoDisconnect && (res == -1025 || res == -128))
                    {
                        if (Logger != null)
                            Logger("(5) Auto Disconnect cause of :" + libnodave.daveStrerror(res));
                        this.Disconnect();
                        return;
                    }
                    else if (res != 0)
                        throw new PLCException(res);
                }
            }
        }

        /// <summary>
        /// Remove all PLC tags currently in the write Queue. This aborts the pending write requests.
        /// </summary>
        public void WriteQueueClear()
        {
            _writeQueue.Clear();
        }

        /// <summary>
        /// Add an new PLC tag to the Queue to be written to the PLC
        /// </summary>
        /// <param name="tag"></param>
        public void WriteQueueAdd(PLCTag tag)
        {
            _writeQueue.Add(tag);
        }

        /// <summary>
        /// Write all pending PLC tags in the Queue to the PLC
        /// </summary>
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

        /// <summary>
        /// Write an list of values to the PLC
        /// </summary>
        /// <param name="valueList">The list of Values to write to the controller</param>
        public void WriteValues(IEnumerable<PLCTag> valueList)
        {
            WriteValues(valueList, false);
        }

        /// <summary>
        /// Write an list of values to the PLC
        /// </summary>
        /// <param name="valueList">The list of Values to write to the controller</param>
        /// <param name="useWriteOptimation">If set to true, write optimation is enabled, but then, the order of your written values can varry, also a 4 byte value can be splittet written to the plc!</param>
        public void WriteValues(IEnumerable<PLCTag> valueList, bool useWriteOptimation)
        {
            lock (lockObj)
            {
                CheckConnection();

                if (Configuration.ConnectionType == LibNodaveConnectionTypes.Fetch_Write_Active || Configuration.ConnectionType == LibNodaveConnectionTypes.Fetch_Write_Passive)
                {
                    WriteValuesFetchWrite(valueList);
                    return;
                }


                if (_dc != null)
                {

                    /*foreach (PLCTag plcTag in valueList)
                    {
                        plcTag.RaiseValueChangedEvenWhenNoChangeHappened = true;
                    }*/

                    //PLCNckTag kann im Moment noch nicht optimiert werden
                    if (useWriteOptimation && !(valueList.Cast<PLCTag>().ToList()[0] is PLCNckTag))
                    {
                        #region Optimize Writing List....

                        List<PLCTag> orderedList = new List<PLCTag>();
                        orderedList.AddRange(valueList);
                        orderedList.Sort(new SorterForPLCTags());

                        List<PLCTag> writeTagList = new List<PLCTag>();

                        //Go through the List of PLC Tags and Combine the ones, where the Byte Address does not differ...
                        MemoryArea oldDataSource = 0;
                        int oldDB = 0, oldByteAddress = 0, oldLen = 0;
                        int cntCombinedTags = 0;
                        PLCTag lastTag = null;
                        PLCTagReadHelper rdHlp = new PLCTagReadHelper() { TagDataType = TagDataType.ByteArray };
                        foreach (PLCTag plcTag in orderedList)
                        {
                            if (cntCombinedTags == 0) // && plcTag.TagDataType!=TagDataType.Bool)
                            {
                                oldDataSource = plcTag.TagDataSource;
                                oldDB = plcTag.DataBlockNumber;
                                oldByteAddress = plcTag.ByteAddress;
                                oldLen = plcTag._internalGetSize();
                                lastTag = plcTag;
                                cntCombinedTags++;
                            }
                            else
                            {
                                if (plcTag.TagDataType != TagDataType.Bool && oldDataSource == plcTag.TagDataSource &&
                                    (oldDataSource != MemoryArea.Datablock || oldDB == plcTag.DataBlockNumber) &&
                                    plcTag.ByteAddress <= oldByteAddress + oldLen)
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
                                    rdHlp.TagDataSource = oldDataSource;
                                    rdHlp.DataBlockNumber = oldDB;
                                    lastTag = plcTag;
                                }
                                else
                                {
                                    if (cntCombinedTags > 1)
                                    {
                                        writeTagList.Add(rdHlp);
                                        rdHlp = new PLCTagReadHelper() { TagDataType = TagDataType.ByteArray };
                                        cntCombinedTags = 0;
                                    }
                                    else
                                    {
                                        writeTagList.Add(lastTag);
                                        cntCombinedTags = 0;
                                    }

                                    if (plcTag.TagDataType == TagDataType.Bool)
                                    {
                                        lastTag = plcTag;
                                        writeTagList.Add(lastTag);
                                    }
                                    else
                                    {
                                        oldDataSource = plcTag.TagDataSource;
                                        oldDB = plcTag.DataBlockNumber;
                                        oldByteAddress = plcTag.ByteAddress;
                                        oldLen = plcTag._internalGetSize();
                                        if (oldLen % 2 != 0) oldLen++;
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
                    int maxWriteSize = GetPduSize() - 32; //32 = Header
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

                        if (!(currVal is PLCNckTag) && gesWriteSize < maxWriteSize && //Maximale Byte Anzahl noch nicht erreicht
                            /*anzWriteVar < maxWriteVar &&*/
                            ( //maximale Variablenanzahl noch nicht erreicht                        
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

                                    if (currVal.TagDataType == TagDataType.Bool)
                                        myPDU.addBitVarToWriteRequest(Convert.ToInt32(currVal.TagDataSource),
                                            currVal.DataBlockNumber,
                                            (currVal.ByteAddress + splitPos) * 8 + currVal.BitAddress, 1, wrt);
                                    else
                                        myPDU.addVarToWriteRequest(Convert.ToInt32(currVal.TagDataSource),
                                            currVal.DataBlockNumber, currVal.ByteAddress + splitPos, currValSize, wrt);
                                    gesWriteSize += tagHeaderSize + wrt.Length;
                                }
                                //Wert war gesplittet
                                else
                                {
                                    byte[] tmp = new byte[currValSize];
                                    currVal._putControlValueIntoBuffer(tmp, 0);

                                    byte[] wrt = new byte[currValSize - splitPos];
                                    Array.Copy(tmp, splitPos, wrt, 0, wrt.Length);
                                    if (currVal.TagDataType == TagDataType.Bool)
                                        myPDU.addBitVarToWriteRequest(Convert.ToInt32(currVal.TagDataSource),
                                            currVal.DataBlockNumber,
                                            (currVal.ByteAddress + splitPos) * 8 + currVal.BitAddress, 1, wrt);
                                    else
                                        myPDU.addVarToWriteRequest(Convert.ToInt32(currVal.TagDataSource),
                                            currVal.DataBlockNumber, currVal.ByteAddress + splitPos, wrt.Length, wrt);
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
                                if (currVal.TagDataType == TagDataType.Bool)
                                    myPDU.addBitVarToWriteRequest(Convert.ToInt32(currVal.TagDataSource),
                                        currVal.DataBlockNumber, (currVal.ByteAddress + splitPos) * 8 + currVal.BitAddress,
                                        1, wrt);
                                else
                                    myPDU.addVarToWriteRequest(Convert.ToInt32(currVal.TagDataSource),
                                        currVal.DataBlockNumber, currVal.ByteAddress + splitPos, wrt.Length, wrt);
                                gesWriteSize += tagHeaderSize + wrt.Length;

                                splitPos = splitPos + maxCurrAddSize;
                                anzWriteVar++;
                            }

                        }
                        else
                        {
                            //Send Request
                            rs = new libnodave.resultSet();

                            if (currVal is PLCNckTag)
                            {
                                byte[] wrt = new byte[currValSize];
                                currVal._putControlValueIntoBuffer(wrt, 0);
                                var nckT = currVal as PLCNckTag;
                                #region Reverse
                                if (nckT != null && nckT.TagDataType != TagDataType.String && nckT.TagDataType != TagDataType.CharArray && nckT.NckArea != NCK_Area.AreaFeedDrive && nckT.NckArea != NCK_Area.AreaMainDrive)
                                    System.Array.Reverse(wrt, 0, wrt.Length);
                                #endregion

                                #region Transport sizes
                                //**************************************************************************
                                // Transport sizes in data
                                //
                                //S7COMM_DATA_TRANSPORT_SIZE_NULL     0
                                //S7COMM_DATA_TRANSPORT_SIZE_BBIT     3           /* bit access, len is in bits */
                                //S7COMM_DATA_TRANSPORT_SIZE_BBYTE    4           /* byte/word/dword access, len is in bits */
                                //S7COMM_DATA_TRANSPORT_SIZE_BINT     5           /* integer access, len is in bits */
                                //S7COMM_DATA_TRANSPORT_SIZE_BDINT    6           /* integer access, len is in bytes */
                                //S7COMM_DATA_TRANSPORT_SIZE_BREAL    7           /* real access, len is in bytes */
                                //S7COMM_DATA_TRANSPORT_SIZE_BSTR     9           /* octet string, len is in bytes */
                                //**************************************************************************

                                //int transsize = 4;
                                //if (nckT.TagDataType == TagDataType.LReal)
                                //    transsize = 9;

                                #endregion
                                myPDU.addNCKToWriteRequest((int)nckT.NckArea, nckT.NckUnit, nckT.NckColumn, nckT.NckLine, nckT.NckModule, nckT.NckLinecount, wrt.Length, wrt);
                                valueListT.Remove(currVal); //Wert erledigt... löschen....
                            }

                            lock (lockObj)
                            {
                                res = _dc.execWriteRequest(myPDU, rs);
                            }

                            if (AutoDisconnect && (res == -1025 || res == -128))
                            {
                                if (Logger != null)
                                    Logger("(6) Auto Disconnect cause of :" + libnodave.daveStrerror(res));
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
                        lock (lockObj)
                        {
                            res = _dc.execWriteRequest(myPDU, rs);
                        }

                        if (AutoDisconnect && (res == -1025 || res == -128))
                        {
                            if (Logger != null)
                                Logger("(7) Auto Disconnect cause of :" + libnodave.daveStrerror(res));
                            this.Disconnect();
                            return;
                        }
                    }
                }
            }
        }
        #endregion

        #region NC PI-Service

        public void PI_Service(string piservice, string[] param)
        {
            libnodave.resultSet rs = new libnodave.resultSet();
            int res = _dc.PI_StartNC(piservice, param, param.Length);

            if (res == -1025)
            {
                this.Disconnect();
                throw new System.Runtime.InteropServices.ExternalException("PI_Service: " + res);
            }
            if (res != 0)
                throw new Exception("PI_Service: " + res);
        }
        #endregion

        #region NC file transfer
        /// <summary>
        /// Load complete file from NC
        /// </summary>
        /// <param name="fullFileName">full filename inc. path</param>
        /// <param name="F_XFER">Start PI-Service F_XFER before upload</param>
        /// <returns></returns>
        public byte[] BinaryUploadFromNC(string fullFileName, bool F_XFER = false)
        {
            libnodave.resultSet rs = new libnodave.resultSet();
            byte[] id = new byte[4];
            List<byte> lRet = new List<byte>();
            string file = fullFileName.Remove(0, fullFileName.LastIndexOf('/') > 0 ? fullFileName.LastIndexOf('/') + 1 : 0);

            if (F_XFER)
                PI_Service("_N_F_XFER", new string[] { "P01", fullFileName });

            int res = _dc.initUploadNC(file, ref id);
            if (res != 0)
                throw new Exception("UploadFromNC: " + res);

            int more = 0;
            int len = 0;
            byte[] buffer = new byte[1024];

            do
            {
                res = _dc.doUploadNC(out more, buffer, out len, id);
                if (res != 0)
                    break;

                for (int i = 0; i < len; i++)
                {
                    lRet.Add(buffer[i]);
                }
            } while (more != 0);

            res = _dc.endUploadNC(id);
            if (res != 0)
                throw new Exception("BinaryUploadFromNC: " + res);

            return lRet.ToArray();
        }

        /// <summary>
        /// Load complete file from NC
        /// </summary>
        /// <param name="fullFileName">full filename inc. path</param>
        /// <param name="F_XFER">Start PI-Service F_XFER before upload</param>
        /// <returns></returns>
        public string UploadFromNC(string fullFileName, bool F_XFER = true)
        {
            libnodave.resultSet rs = new libnodave.resultSet();
            byte[] id = new byte[4];
            string ret = string.Empty;
            string file = fullFileName.Remove(0, fullFileName.LastIndexOf('/') > 0 ? fullFileName.LastIndexOf('/') + 1 : 0);

            if (F_XFER)
                PI_Service("_N_F_XFER", new string[] { "P01", fullFileName });

            int res = _dc.initUploadNC(file, ref id);
            if (res != 0)
                throw new Exception("UploadFromNC: " + res);

            int more = 0;
            int len = 0;
            byte[] buffer = new byte[1024];

            do
            {
                res = _dc.doUploadNC(out more, buffer, out len, id);
                if (res != 0)
                    break;
                ret += System.Text.Encoding.Default.GetString(buffer, 0, len);
            } while (more != 0);

            res = _dc.endUploadNC(id);
            if (res != 0)
                throw new Exception("UploadFromNC: " + res);

            return ret;
        }

        /// <summary>
        /// Load complete file from NC
        /// </summary>
        /// <param name="fullFileName">full filename inc. path</param>
        /// <param name="size">size of the file (buffer)</param>
        /// <param name="F_XFER">Start PI-Service F_XFER before upload</param>
        /// <returns></returns>
        public string UploadFromNC(string fullFileName, int size, bool F_XFER = true)
        {
            libnodave.resultSet rs = new libnodave.resultSet();
            string ret = string.Empty;
            string filename = fullFileName.Remove(0, fullFileName.LastIndexOf('/') > 0 ? fullFileName.LastIndexOf('/') + 1 : 0);

            if (F_XFER)
                PI_Service("_N_F_XFER", new string[] { "P01", fullFileName });

            int length = 0;
            byte[] buffer = new byte[size];
            int res = _dc.daveGetNCProgram(filename, buffer, ref length);

            if (res != 0)
                throw new Exception("UploadFromNC: " + res);
            else
                ret = System.Text.Encoding.Default.GetString(buffer, 0, length);

            return ret;
        }

        /// <summary>
        /// Load complete file from NC
        /// </summary>
        /// <param name="fullFileName">full filename inc. path</param>
        /// <param name="size">size of the file (buffer)</param>
        /// <param name="F_XFER">Start PI-Service F_XFER before upload</param>
        /// <returns></returns>
        public byte[] BinaryUploadNcFile(string fullFileName, int size = 0, bool F_XFER = false)
        {
            libnodave.resultSet rs = new libnodave.resultSet();
            string filename = fullFileName.Remove(0, fullFileName.LastIndexOf('/') > 0 ? fullFileName.LastIndexOf('/') + 1 : 0);

            if (size == 0)
                size = UploadNcFileSize(fullFileName, F_XFER);
            if (F_XFER)
                PI_Service("_N_F_XFER", new string[] { "P01", fullFileName });

            int length = 0;
            byte[] buffer = new byte[size];
            int res = _dc.daveGetNcFile(filename, buffer, ref length);

            if (res != 0)
                throw new Exception("BinaryUploadNcFile: " + res);

            byte[] ret = new byte[length];
            Array.Copy(buffer, ret, length);

            return ret;
        }

        /// <summary>
        /// Load complete file from NC
        /// </summary>
        /// <param name="fullFileName">full filename inc. path</param>
        /// <param name="size">size of the file (buffer)</param>
        /// <param name="F_XFER">Start PI-Service F_XFER before upload</param>
        /// <returns></returns>
        public string UploadNcFile(string fullFileName, int size = 0, bool F_XFER = true)
        {
            libnodave.resultSet rs = new libnodave.resultSet();
            string ret = string.Empty;
            string filename = fullFileName.Remove(0, fullFileName.LastIndexOf('/') > 0 ? fullFileName.LastIndexOf('/') + 1 : 0);

            if (size == 0)
                size = UploadNcFileSize(fullFileName, F_XFER);
            if (F_XFER)
                PI_Service("_N_F_XFER", new string[] { "P01", fullFileName });

#if daveDebug
            libnodave.daveSetDebug(0x1ffff);
#endif

            int length = 0;
            byte[] buffer = new byte[size];
            int res = _dc.daveGetNcFile(filename, buffer, ref length);

#if daveDebug
            var a = libnodave.daveGetDebug();

            if (res != 0)
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i] == 0)
                    {
                        length = i;
                        break;
                    }
                }

            libnodave.daveSetDebug(0);
#endif

            if (res != 0)
                throw new Exception("UploadNcFile: " + res);
            else if (length > buffer.Length)
                throw new ArgumentOutOfRangeException("size", size, "File size: " + length);
            else
                ret = System.Text.Encoding.Default.GetString(buffer, 0, length);

            return ret;
        }

        /// <summary>
        /// Load file size from NC
        /// </summary>
        /// <param name="fullFileName">full filename inc. path</param>
        /// <param name="F_XFER">Start PI-Service F_XFER before upload</param>
        /// <returns></returns>
        public int UploadNcFileSize(string fullFileName, bool F_XFER = true)
        {
            libnodave.resultSet rs = new libnodave.resultSet();
            string filename = fullFileName.Remove(0, fullFileName.LastIndexOf('/') > 0 ? fullFileName.LastIndexOf('/') + 1 : 0);

            if (filename.ToUpper().EndsWith("WPD") || filename.ToUpper().EndsWith("DIR"))
                return Int16.MaxValue;

            if (F_XFER)
                PI_Service("_N_F_XFER", new string[] { "P01", fullFileName });

#if daveDebug
            libnodave.daveSetDebug(0x1ffff);
#endif

            int length = 0;
            int res = _dc.daveGetNcFileSize(filename, ref length);

#if daveDebug
            libnodave.daveSetDebug(0);
#endif

            if (res != 0)
                throw new Exception("UploadNcFileSize: " + res);
            return length;
        }

        /// <summary>
        /// Transfer file to NC
        /// </summary>
        /// <param name="fullFileName">full filename inc. path</param>
        /// <param name="ts">DateTime Format: yyMMddHHmmss</param>
        /// <param name="data">Data of the file</param>
        public void DownloadToNC(string fullFileName, string ts, string data)
        {
            libnodave.resultSet rs = new libnodave.resultSet();

            string filename = fullFileName.Remove(0, fullFileName.LastIndexOf('/') > 0 ? fullFileName.LastIndexOf('/') + 1 : 0);
            string path = fullFileName.Substring(0, fullFileName.LastIndexOf('/') > 0 ? fullFileName.LastIndexOf('/') : fullFileName.Length);

            byte[] buffer = System.Text.Encoding.Default.GetBytes(data);
            int res = _dc.davePutNCProgram(filename, path, ts, buffer, buffer.Length);
            if (res != 0)
                throw new Exception("DownloadToNC: " + res);
        }
        #endregion

        #region SPS Alarm Query
        public int[] GetAlarmS_IDs()
        {
            int size = 32767;

            libnodave.resultSet rs = new libnodave.resultSet();
            List<int> lRet = new List<int>();

            try
            {
                int alarmCount = 0;
                byte[] buffer = new byte[size];
                int res = _dc.alarmQueryAlarm_S(buffer, size, ref alarmCount);

                if (res != 0)
                    throw new Exception("GetSPS_AlarmQuery: " + res);
                else
                {
                    byte[] dummy = new byte[System.Runtime.InteropServices.Marshal.SizeOf(typeof(alarmMessageHeader))];
                    int index = 0;
                    for (int i = 0; i < alarmCount; i++)
                    {
                        Array.Copy(buffer, index, dummy, 0, dummy.Length);
                        var alarmHeader = EndianessMarshaler.BytesToStruct<alarmMessageHeader>(dummy);
                        lRet.Add(alarmHeader.EventID);
                        index += alarmHeader.Lenght + 2;
                    }
                }
                //ret = System.Text.Encoding.Default.GetString(buffer, 0, alarmCount);
            }
            catch (Exception)
            {
            }

            return lRet.ToArray();
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi, Pack = 1)]
        public class alarmMessageHeader
        {
            //[Endian(Endianness.BigEndian)]
            private byte _Lenght;
            public Byte Lenght
            {
                get { return _Lenght; }
                set { _Lenght = value; }
            }

            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2)]
            private byte[] _x;
            public Byte[] _
            {
                get { return _x; }
                set { _x = value; }
            }

            private byte _Alarmtype;
            public Byte Alarmtype
            {
                get { return _Alarmtype; }
                set { _Alarmtype = value; }
            }

            [Endian(Endianness.BigEndian)]
            private int _EventID;
            public Int32 EventID
            {
                get { return _EventID; }
                set { _EventID = value; }
            }

#if TimestampMessageComing
            private byte __x;
            public Byte __
            {
                get { return __x; }
                set { __x = value; }
            }

            private byte _EventState;
            public Byte EventState
            {
                get { return _EventState; }
                set { _EventState = value; }
            }

            private byte _AckState_going;
            public Byte AckState_going
            {
                get { return _AckState_going; }
                set { _AckState_going = value; }
            }

            private byte _AckState_coming;
            public Byte AckState_coming
            {
                get { return _AckState_coming; }
                set { _AckState_coming = value; }
            }

            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 8)]
            private byte[] _TimestampMessageComing;
            public DateTime TimestampMessageComing
            {
                get { return GetDateTimeFromByteArray(_TimestampMessageComing); }
                set { _TimestampMessageComing = GetByteArrayFromBuffer(value); }
            }
#endif
        }

        private static DateTime GetDateTimeFromByteArray(byte[] buffer)
        {
            try
            {
                string[] sAr = BitConverter.ToString(buffer).Trim().Split('-');
                return new DateTime(int.Parse(sAr[0]) >= 90 ? 1900 + int.Parse(sAr[0]) : 2000 + int.Parse(sAr[0]), int.Parse(sAr[1]), int.Parse(sAr[2]), int.Parse(sAr[3]), int.Parse(sAr[4]), int.Parse(sAr[5]), int.Parse(sAr[6] + sAr[7].Substring(0, 1)));
            }
            catch (Exception)
            {
                return new DateTime(); // DateTime(1990, 1, 1);
            }
        }

        private static byte[] GetByteArrayFromBuffer(DateTime dt)
        {
            byte[] ret = new byte[8];
            try
            {
                if (Equals(dt, new DateTime()))
                    return new byte[8];

                ret[0] = Convert.ToByte((dt.Year > 2000 ? dt.Year - 2000 : dt.Year - 1900).ToString(), 16);
                ret[1] = Convert.ToByte(dt.Month.ToString(), 16);// (byte)dt.Month;
                ret[2] = Convert.ToByte(dt.Day.ToString(), 16);
                ret[3] = Convert.ToByte(dt.Hour.ToString(), 16);
                ret[4] = Convert.ToByte(dt.Minute.ToString(), 16);
                ret[5] = Convert.ToByte(dt.Second.ToString(), 16);
                ret[6] = Convert.ToByte(dt.Millisecond.ToString("000").Substring(0, 2), 16);
                ret[7] = Convert.ToByte(dt.Millisecond.ToString("000").Substring(2) + ((byte)dt.DayOfWeek + 1), 16);
            }
            catch (Exception)
            { }
            return ret;
        }

#if aaa
        public class objMSG
        {
            objMSG() { }

            objMSG(byte[] Buffer)
            {
                this.lenght = Buffer[0];
                byte Alarmtype = Buffer[3];

                dummy = new byte[4];
                Array.Copy(Buffer, index + 4, dummy, 0, dummy.Length);
                Array.Reverse(dummy);
                var EventID = BitConverter.ToUInt32(dummy, 0);

                byte EventState = Buffer[index + 9];
                byte ActStateGoing = Buffer[index + 10];
                byte ActStateComing = Buffer[index + 11];
                byte[] _TimeStampMessageComming = new byte[8];
                Array.Copy(Buffer, index + 12, _TimeStampMessageComming, 0, _TimeStampMessageComming.Length);
                DateTime TimeStampMessageComming = getDateTimeFromDateAndTimeString(BitConverter.ToString(_TimeStampMessageComming));
            }

            [Endian(Endianness.BigEndian)]
            private byte lenght;
            public Byte Lenght
            {
                get { return lenght; }
                set { lenght = value; }
            }

            private byte alarmtype;
            public Byte Alarmtype
            {
                get { return alarmtype; }
                set { alarmtype = value; }
            }

        }
#endif
        #endregion

        #region Debug
        public void SetDaveDebug(int newDebugLevel = 0)
        {
            libnodave.daveSetDebug(newDebugLevel);
        }
        #endregion

        public void Dispose()
        {
            Connected = false;
            if (_NeedDispose)
            {
                _NeedDispose = false;

                if (_fetchWriteConnection != null)
                    _fetchWriteConnection.Dispose();

                if (_dc != null)
                    _dc.disconnectPLC();
                _dc = null;

                if (_di != null)
                    _di.disconnectAdapter();
                _di = null;

                if (_configuration != null)
                    switch (_configuration.ConnectionType)
                    {
                        case LibNodaveConnectionTypes.MPI_über_Serial_Adapter:
                        case LibNodaveConnectionTypes.MPI_über_Serial_Adapter_Andrews_Version_without_STX:
                        case LibNodaveConnectionTypes.MPI_über_Serial_Adapter_Step_7_Version:
                        case LibNodaveConnectionTypes.MPI_über_Serial_Adapter_Adrews_Version_with_STX:
                        case LibNodaveConnectionTypes.PPI_über_Serial_Adapter:
                        case LibNodaveConnectionTypes.AS_511:
                            libnodave.closePort(_fds.rfd);
                            break;
                        case LibNodaveConnectionTypes.Use_Step7_DLL:
                            libnodave.closeS7online(_fds.rfd);
                            break;
                        case LibNodaveConnectionTypes.ISO_over_TCP:
                        case LibNodaveConnectionTypes.ISO_over_TCP_CP_243:
                        case LibNodaveConnectionTypes.Netlink_lite:
                        case LibNodaveConnectionTypes.Netlink_lite_PPI:
                        case LibNodaveConnectionTypes.Netlink_Pro:
                            libnodave.closeSocket(_fds.rfd);
                            break;
                    }
            }

            _dbSizes = null;
        }
    }
}
