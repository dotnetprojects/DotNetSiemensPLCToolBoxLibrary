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
using System.Threading;
using System.Timers;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;
using Microsoft.Win32;

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

        private readonly PLCConnectionConfiguration _myConfig;

        private ConnectionTargetPLCType _connectionTargetPlcType;
        ConnectionTargetPLCType ConnectionTargetPLCType
        {
            get { return _connectionTargetPlcType; }            
        }

        public PLCConnection(String name)
        {
            if (name == "")
                throw new Exception("No Connection Name specified!");

            _myConfig = new PLCConnectionConfiguration(name);

            _connectionTargetPlcType = ConnectionTargetPLCType.S7;
        }

        /// <summary>
        /// Constructor wich uses a LibNoDavaeConnectionConfiguration from outside.
        /// </summary>
        /// <param name="akConfig"></param>
        public PLCConnection(PLCConnectionConfiguration akConfig)
        {
            _myConfig = akConfig;
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

        private bool _netlinkReseted = false;

        //LibNoDave used types
        private libnodave.daveOSserialType _fds;
        private libnodave.daveInterface _di = null; //dave Interface
        private libnodave.daveConnection _dc = null;

        private System.Timers.Timer socketTimer;
        private Thread socketThread;

        /// <summary>
        /// If you use this Connect without HWND, you can not use the S7Online Connection!
        /// </summary>
        public void Connect()
        {

            Connect(0);
        }


        void socketTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            socketTimer.Stop();
            socketThread.Abort();            
        }

        public void socket_Thread()
        {
            _fds.rfd = -999;
            _fds.rfd = libnodave.openSocket(_myConfig.Port, _myConfig.CpuIP);
        }

        /// <summary>
        /// Connect to the PLC
        /// </summary>
        /// <param name="hwnd"></param>
        public void Connect(int hwnd)
        {
            _NeedDispose = true;
            //Debugging for LibNoDave
            //libnodave.daveSetDebug(0x1ffff);

            _myConfig.ReloadConfiguration();

            if (hwnd == 0 && _myConfig.ConnectionType == 50)
                throw new Exception("Error: You can only use the S7Online Connection when you specify the HWND Parameter on the Connect Function");

            //This Jump mark is used when the Netlink Reset is activated!
        NLAgain:

            //LibNodave Verbindung aufbauen
            switch (_myConfig.ConnectionType)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 10:
                    _fds.rfd = libnodave.setPort(_myConfig.ComPort, _myConfig.ComPortSpeed, _myConfig.ComPortParity);
                    break;
                case 20:   //AS511            
                    _fds.rfd = libnodave.setPort(_myConfig.ComPort, "9600" /*_myConfig.ComPortSpeed*/, 'E' /*_myConfig.ComPortParity*/);
                    break;
                case 50:
                    _fds.rfd = libnodave.openS7online(_myConfig.EntryPoint, hwnd);
                    if (_fds.rfd == -1)
                    {
                        _NeedDispose = false;
                        throw new Exception("Error: " + libnodave.daveStrS7onlineError());
                    }
                    break;
                case 122:
                case 123:
                case 124:
                case 223:
                case 224:
                case 230:
                    socketTimer = new System.Timers.Timer(_myConfig.TimeoutIPConnect);
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
                    //_fds.rfd = libnodave.openSocket(_myConfig.Port, _myConfig.CpuIP););
                    break;
            }

            if (_fds.rfd == -999)
            {
                _NeedDispose = false;
                throw new Exception("Error: Timeout Connecting the IP");
            }

            if ((!(_myConfig.ConnectionType == 50) && _fds.rfd == 0) || _fds.rfd < 0)
            {
                _NeedDispose = false;
                throw new Exception(
                    "Error: Could not creating the Physical Interface (Maybe wrong IP, COM-Port not Ready,...)");
            }

            //daveOSserialType Struktur befüllen
            _fds.wfd = _fds.rfd;
            
            //Dave Interface Erzeugen
            _di = new libnodave.daveInterface(_fds, _myConfig.ConnectionName, _myConfig.LokalMpi, _myConfig.ConnectionType, _myConfig.BusSpeed);

            //Timeout setzen...
            _di.setTimeout(_myConfig.Timeout);
            //_di.setTimeout(500000);
            //Dave Interface initialisieren
            int ret = _di.initAdapter();
            if (ret != 0)
                throw new Exception("Error: " + libnodave.daveStrerror(ret));

            //Get S7OnlineType - To detect if is a IPConnection 
            bool IPConnection = false;
#if !IPHONE
            if (_myConfig.ConnectionType == 50)
            {
                RegistryKey myConnectionKey =
                    Registry.LocalMachine.CreateSubKey("SOFTWARE\\Siemens\\SINEC\\LogNames\\" + _myConfig.EntryPoint);
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
            if (_myConfig.Routing || IPConnection)
                _dc = new libnodave.daveConnection(_di, _myConfig.CpuMpi, _myConfig.CpuIP, IPConnection,
                                                   _myConfig.CpuRack, _myConfig.CpuSlot, _myConfig.Routing,
                                                   _myConfig.RoutingSubnet1, _myConfig.RoutingSubnet2,
                                                   _myConfig.RoutingDestinationRack, _myConfig.RoutingDestinationSlot,
                                                   _myConfig.RoutingDestination);
            else
                _dc = new libnodave.daveConnection(_di, _myConfig.CpuMpi, _myConfig.CpuRack, _myConfig.CpuSlot);

            if (_myConfig.NetLinkReset && !_netlinkReseted && (_myConfig.ConnectionType == 223 || _myConfig.ConnectionType == 224))
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

            Connected = true;
        }        

        public void PLCStop()
        {
            if (_dc != null)
                lock (_dc)
                    _dc.stop();
        }

        public void PLCStart()
        {
            if (_dc != null)
                lock (_dc)
                    _dc.start();
        }

        public DateTime PLCReadTime()
        {
            if (_dc != null)
                lock (_dc)
                    return _dc.daveReadPLCTime();
            return DateTime.MinValue;
        }

        public void PLCSetTime(DateTime tm)
        {
            if (_dc != null)
                lock (_dc)
                    _dc.daveSetPLCTime(tm);
        }

        public class DiagnosticData : IDisposable
        {
            internal S7FunctionBlock myBlock;
            internal S7FunctionBlockRow.SelectedStatusValues selRegister;
            internal Dictionary<int, List<S7FunctionBlockRow>> ByteAdressNumerPLCFunctionBlocks;
            internal short ReqestID;
            internal PLCConnection myConn;
            internal int readLineCounter;
            internal byte DiagDataTeletype;


            public void RequestDiagnosticData()
            {
                libnodave.PDU myPDU = new libnodave.PDU();

                byte[] para;
                byte[] data;

                myPDU = new libnodave.PDU();
                para = new byte[] { 0x00, 0x01, 0x12, 0x08, 0x12, 0x41, 0x0e, 0x00, 0x00, 0x00, 0x00, 0x00 };
                data = new byte[] { 0x00, 0x14, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, BitConverter.GetBytes(ReqestID)[0], BitConverter.GetBytes(ReqestID)[1] };
                myConn._dc.daveBuildAndSendPDU(myPDU, para, data);

                byte[] rdata, rparam;
                int res = myConn._dc.daveRecieveData(out rdata, out rparam);

                if (rparam[10] == 0xd0 && rparam[11] == 0xa5)
                    throw new Exception("Error, the Commands are not excetuted");
                else if (rparam[10] == 0xd0)
                    throw new Exception("Error, the Trigger is already in use. Err. Code: " + rparam[11].ToString("X"));
                else if (rparam[10] != 0x00)
                    throw new Exception("Error reading Diagnostic Data");
                else if (rdata.Length < 14) //Function Block is not called!
                    return;

                int answLen = rdata[6] * 0x100 + rdata[7];

                var prev = new S7FunctionBlockRow.BlockStatus();
                int linenr = 14;
                
                //In the 0x01 Telegramm, only Akku1 and 2 can be selected and STW is always Selected!
                if (DiagDataTeletype == 0x01 && ((selRegister & S7FunctionBlockRow.SelectedStatusValues.Akku1) > 0 || (selRegister & S7FunctionBlockRow.SelectedStatusValues.Akku2) > 0))
                    selRegister |= S7FunctionBlockRow.SelectedStatusValues.Akku1 | S7FunctionBlockRow.SelectedStatusValues.Akku2;
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

                        prev = S7FunctionBlockRow.BlockStatus.ReadBlockStatus(rdata, linenr + 2, akSelRegister, prev);
                        foreach (S7FunctionBlockRow tmp in akRow)
                            tmp.ActualBlockStatus = prev;
                        linenr += akAskSize + 2;
                    }                               
            }

            public void RemoveDiagnosticData()
            {
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
        public DiagnosticData startRequestDiagnosticData(S7FunctionBlock myBlock, int StartAWLByteAdress, S7FunctionBlockRow.SelectedStatusValues selRegister /*Count of the Rows wich should be read, Registers wich should be read! */)
        {
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
                    byte DiagDataTeletype = 0x13;
                    DiagDataTeletype = 0x01;

                    //len of the AnswBlock Block in the PDU
                    short answSize = (short)(S7FunctionBlockRow._GetCommandStatusAskSize(selRegister, DiagDataTeletype) + 2);


                    int askSize = 4; //This is minimum 4 (For the Start Registers)
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

                    S7FunctionBlockRow prevFunctionBlock = null;

                    foreach (var plcFunctionBlockRow in myBlock.AWLCode)
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
                                        }
                                        else
                                        {
                                            byte askRegisterByte = GetByteForSelectedRegisters(askRegister, DiagDataTeletype);
                                            LinesSelectedRegisters.AddRange(new byte[] { 0x80, askRegisterByte });

                                            for (int g = 0; g < ((plcFunctionBlockRow.ByteSize - 2) / 2); g++)
                                            {
                                                byte wr = 0x00;
                                                //if (g % 2 != 0) wr = 0x80;
                                                LinesSelectedRegisters.AddRange(new byte[] { wr, wr });
                                            }
                                        }

                                        //Jede Anfrage braucht 4 Byte
                                        askSize += 4;
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

                                            for (int g = 0; g < ((plcFunctionBlockRow.ByteSize - 2) / 2); g++)
                                            {
                                                byte wr = 0x00;
                                                //if (g % 2 != 0) wr = 0x80;
                                                LinesSelectedRegisters.AddRange(new byte[] { wr, wr });
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

                    retDiagnosticData.ReqestID = BitConverter.ToInt16(stid, 0);

                    retDiagnosticData.DiagDataTeletype = DiagDataTeletype;

                    return retDiagnosticData;
                }
            return null;
        }

        public DataTypes.PLCState PLCGetSafetyStep()
        {           
            if (_dc != null)
                lock (_dc)
                {
                    byte[] buffer = new byte[40];
                    int ret = _dc.readSZL(0x232, 4, buffer);

                    if (ret < 0)
                        throw new Exception("Error: " + libnodave._daveStrerror(ret));
                    if (buffer[1] != 0x04)
                        throw new WPFToolboxForSiemensPLCsException(WPFToolboxForSiemensPLCsExceptionType.ErrorReadingSZL);


                    //byte 2,3 betriebsartschalter
                    //    byte 4,5 schutzstufe
                }
            return 0;
        }

        public void PLCSendPassword(string pwd)
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
                throw new Exception("Wrong Password");
        }

        public DataTypes.PLCState PLCGetState()
        {
            //Dokumentation of SZL can be found here: http://www.google.de/url?sa=t&source=web&cd=3&ved=0CCQQFjAC&url=http%3A%2F%2Fdce.felk.cvut.cz%2Frs%2Fplcs7315%2Fmanualy%2Fsfc_e.pdf&ei=tY8QTJufEYSNOLD_oMoH&usg=AFQjCNEHofHOLDcvGp-4eQBwlboKPu3oxQ
            if (_dc != null)
                lock (_dc)
                {
                    byte[] buffer = new byte[64]; 
                    int ret = _dc.readSZL(0x174, 4, buffer); //SZL 0x174 is for PLC LEDs

                    if (ret < 0)
                        throw new Exception("Error: " + libnodave._daveStrerror(ret));
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
            if (_dc != null)
                lock (_dc)
                {
                    List<string> myRet = new List<string>();                    

                    byte[] blocks = new byte[2048 * 4];

                    //_myConfig.ConnectionTyp

                    if (myBlk == DataTypes.PLCBlockType.AllBlocks && ConnectionTargetPLCType==ConnectionTargetPLCType.S7)                        
                        {
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.DB));
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.FC));
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.FB));
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.SFC));
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.SFB));
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.SDB));                            
                        }
                    else if (myBlk == DataTypes.PLCBlockType.AllEditableBlocks && ConnectionTargetPLCType == ConnectionTargetPLCType.S7)                        
                        {
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.DB));
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.FC));
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.FB));
                            myRet.AddRange(PLCListBlocks(DataTypes.PLCBlockType.SDB));               
                        }
                    else
                    {
                        int ret = _dc.ListBlocksOfType(Helper.GetPLCBlockTypeForBlockList(myBlk), blocks);
                        if (ret < 0 && ret != -53774 && ret != -255)
                            throw new Exception("Error: " + libnodave._daveStrerror(ret));
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
            if (_dc != null)
                lock (_dc)
                {
                    //Todo: Better way to Split number and chars
                    byte[] buffer = new byte[65536];
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
                    else
                        return null;
                }
            return null;
        }

        public void PLCDeleteBlock(string BlockName)
        {
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

        public List<DataTypes.DiagnosticEntry> PLCGetDiagnosticBuffer()
        {
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
                        int nr = buffer[n * 20 + 8] * 256 + buffer[n * 20 + 9];
                        DataTypes.DiagnosticEntry myEntr = new DataTypes.DiagnosticEntry(nr);
                        myEntr.TimeStamp = libnodave.getDateTimefrom(buffer, n*20 + 20);
                        retVal.Add(myEntr);
                    }

                    return retVal;
                }
            return null;
        }

        public DataTypes.PLCMemoryInfo PLCGetMemoryInfo()
        {
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


        //Todo: Implement this an then Implement intelligent reading, not only Group Values, combine Addresses

        /// <summary>
        /// Group Values with Same address, so that they are only Read once from the PLC!
        /// </summary>
        /// <param name="valueList"></param>
        /// <returns></returns>
        private  Dictionary<PLCTag, List<PLCTag>> GroupReadValuesFromSameAddress(IEnumerable<PLCTag> valueList)
        {
            //When DB-Number, Byte-Number, Bit-Number and Readsize are the same, they read the identical Data!
 
            Dictionary<PLCTag, List<PLCTag>> doubleReadList = new Dictionary<PLCTag, List<PLCTag>>();
           
            return doubleReadList;
        }

        // Todo: optimize reading, so that when not more then 4 bytes between a tag, read a lager block!
        /// <summary>
        /// This Function Reads Values from the PLC it needs a Array of LibNodaveValues
        /// It tries to Optimize how the Values are Read from the PLC
        /// </summary>
        /// <param name="valueList"></param>        
        public void ReadValues(IEnumerable<PLCTag> valueList)
        {
            
            if (_dc != null)
                lock (_dc)
                {                    
                    List<bool> NotExistedValue = new List<bool>();

                    //Count how Many Bytes from the PLC should be read and create a Byte Array for the Values
                    int completeReadSize = 0;
                    foreach (var libNoDaveValue in valueList)
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

                    foreach (var libNoDaveValue in valueList)
                    {
                        libNoDaveValue.ItemDoesNotExist = false;

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
                        if ((readSizeWithHeader + gesReadSize > maxReadSize || anzReadVar == maxReadVar) && (!libNoDaveValue.DontSplitValue || readSize > maxReadSize))
                        {
                            //If there is space for a tag left.... Then look how much Bytes we can put into this PDU
                            if (anzReadVar < maxReadVar)
                            {
                                int restBytes = maxReadSize - gesReadSize - HeaderTagSize;
                                if (restBytes > 0)
                                {
                                    myPDU.addVarToReadRequest(Convert.ToInt32(libNoDaveValue.LibNoDaveDataSource), libNoDaveValue.DatablockNumber, akByteAddress, restBytes);
                                    //Only at the rest of the bytes to the next read request, and increase the start address!
                                    readSize = readSize - restBytes;
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
                                res = _dc.useResult(rs, akVar);
                                if (res == 10 || res == 5)
                                    NotExistedValue.Add(true);
                                else if (res != 0)
                                    throw new Exception("Error: " + libnodave.daveStrerror(res));
                                else
                                {
                                    NotExistedValue.Add(false);
                                    for (int n = 0; n < readenSizes[akVar]; n++)
                                    {
                                        completeData[positionInCompleteData++] = Convert.ToByte(_dc.getU8());
                                    }
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
                        if (res != 0 && res != 10)
                            throw new Exception("Error: " + libnodave.daveStrerror(res));

                        //Save the Read Data to a User Byte Array (Because we use this in the libnodavevalue class!)
                        for (akVar = 0; akVar < anzVar; akVar++)
                        {
                            res = _dc.useResult(rs, akVar);
                            if (res == 10 || res == 5)
                                NotExistedValue.Add(true);
                            else if (res != 0)
                                throw new Exception("Error: " + libnodave.daveStrerror(res));
                            else
                            {
                                NotExistedValue.Add(false);
                                for (int n = 0; n < readenSizes[akVar]; n++)
                                {
                                    completeData[positionInCompleteData++] = Convert.ToByte(_dc.getU8());
                                }
                            }
                        }
                    }

                    int buffPos = 0;
                    int nr = 0;
                    foreach (var value in valueList)
                    {
                        if (!NotExistedValue[nr])
                        {
                            value._readValueFromBuffer(completeData, buffPos);
                            buffPos += value._internalGetSize();
                        }
                        else
                            value.ItemDoesNotExist = true;                        
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
                        buffPos += value._internalGetSize();
                    }
                    else
                        throw new Exception("Error: " + libnodave.daveStrerror(res));
                }
        }

        /// <summary>
        /// This Function Reads a List of LibNoDaveValues from a Byte Array.
        /// This can be used if you want to send Variables via a TCP Byte Stream from a PLC, and this
        /// Function is also used for the optimized reading.
        /// </summary>
        /// <param name="values">List of the Values</param>
        /// <param name="bytearray">ByteArray</param>
        /// <returns></returns>
        public void ReadValuesFromByteArray(IEnumerable<PLCTag> values, byte[] bytearray)
        {
            if (_dc != null)
                lock (_dc)
                {
                    if (_dc == null)
                        throw new Exception("Error: Not Connected");

                    int pos = 0;

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
            if (_dc != null)
                lock (_dc)
                {
                    if (_dc == null)
                        throw new Exception("Error: Not Connected");

                    int readSize = value._internalGetSize();
                    byte[] myBuff = new byte[readSize];


                    value._putValueIntoBuffer(myBuff, 0);

                    if (value.LibNoDaveDataSource != TagDataSource.Datablock && value.LibNoDaveDataSource != TagDataSource.InstanceDatablock)
                        value.DatablockNumber = 0;

                    int res = _dc.writeManyBytes(Convert.ToInt32(value.LibNoDaveDataSource), value.DatablockNumber, value.ByteAddress, readSize, myBuff);

                    if (res != 0)
                        throw new Exception("Error: " + libnodave.daveStrerror(res));
                }
        }

        public void WriteValues(IEnumerable<PLCTag> valueList)
        {
            if (_dc != null)
                lock (_dc)
                {
                    //Get the Maximum Answer Len for One PDU
                    int maxWriteSize = _dc.getMaxPDULen() - 32; //32 = Header
                    int gesWriteSize = 0;


                    libnodave.resultSet rs;
                    int res;

                    libnodave.PDU myPDU;

                    myPDU = _dc.prepareWriteRequest();
                    foreach (var libNoDaveValue in valueList)
                    {
                        int akVarSize = libNoDaveValue._internalGetSize();
                        if (gesWriteSize + akVarSize + 12 <= maxWriteSize) //12 Header Größe Variable
                        {
                            byte[] wrt = new byte[akVarSize];
                            libNoDaveValue._putValueIntoBuffer(wrt, 0);
                            if (libNoDaveValue.LibNoDaveDataType == TagDataType.Bool)
                                myPDU.addBitVarToWriteRequest(Convert.ToInt32(libNoDaveValue.LibNoDaveDataSource), libNoDaveValue.DatablockNumber, libNoDaveValue.ByteAddress * 8 + libNoDaveValue.BitAddress, 1, wrt);
                            else
                                myPDU.addVarToWriteRequest(Convert.ToInt32(libNoDaveValue.LibNoDaveDataSource), libNoDaveValue.DatablockNumber, libNoDaveValue.ByteAddress, akVarSize, wrt);
                            gesWriteSize += 12 + akVarSize;
                        }
                        else
                        {
                            rs = new libnodave.resultSet();
                            res = _dc.execReadRequest(myPDU, rs);
                        }
                    }

                    if (gesWriteSize > 0)
                    {
                        rs = new libnodave.resultSet();
                        res = _dc.execReadRequest(myPDU, rs);
                    }
                }
        }

        /// <summary>
        /// This Function Reads a DB,FC,OB or FB from a PLC
        /// </summary>
        /// <param name="ModuleName"></param>
        /// <returns></returns>
        public string ReadModule(string ModuleName)
        {
            string type = ModuleName.ToLower().Substring(0, 2);

            switch (type)
            {

                case "fc":
                    //_dc.getProgramBlock(libnodave.daveBlockType_FC,)

                    break;
            }

            return "";
        }

        /// <summary>
        /// This Function write's a DB,FC or FB to a PLC
        /// </summary>
        /// <param name="ModuleName"></param>
        public void WriteModule(string ModuleName)
        {
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

                switch (_myConfig.ConnectionType)
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
