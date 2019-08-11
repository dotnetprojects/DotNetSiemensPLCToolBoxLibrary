//using System;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using System.Threading;
//using System.Windows.Forms;
//using DotNetSiemensPLCToolBoxLibrary.Communication.Library.Pdus;

//namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library.Interfaces
//{

//    public class S7OnlineInterface : Interface
//    {
//        private enum com_class
//        {
//            request = 0x00,
//            confirm = 0x01,
//            indication = 0x02
//        }; 

//        private enum service_code
//        {
//            sda = 0x00,       /* Send Data with Acknowledge                                           */
//            sdn = 0x01,       /* Send Data with no Acknowledge                                        */
//            sdn_broadcast = 0x7f,       /* only for FDL-indication !!! (signs received broadcast-telegram)      */
//            srd = 0x03,       /* Send and Request Data                                                */
//            csrd = 0x05,       /* Cyclic Send and Request Data                                         */
//            reply_update_single = 0x06,       /* Reply Update Single Mode                                             */
//            reply_update_multiple = 0x07,       /* Reply Update Multiple Mode                                           */
//            fdl_read_value = 0x0b,       /* read busparameter                                                    */
//            fdl_set_value = 0x0c,       /* set busparameter                                                     */
//            sap_activate = 0x0e,       /* activate local SAP                                                   */
//            rsap_activate = 0x11,       /* activate local Responder-SAP                                         */
//            sap_deactivate = 0x12,       /* deactivate local (R)SAP                                              */
//            fdl_reset = 0x13,       /* reset PHY and FDL; all FDL-information is lost, exc. last busparam.  */
//            mac_reset = 0x15,       /* reset for MAC; a part of last valid busparameter will be updated     */
//            fdl_event = 0x18,       /* only for indication, list of events                                  */
//            lsap_status = 0x19,       /* requests information of remote-SAP or local-SAP                      */
//            fdl_life_list_create_remote = 0x1a,       /* requests list of intact stations                                     */
//            fdl_life_list_create_local = 0x1b,       /* requests quick-list of intact stations (LAS and GAP will be actual)  */
//            fdl_ident = 0x1c,       /* requests data of software- and hardware-release                      */
//            fdl_read_statistic_ctr = 0x1d,       /* NOT SUPPORTED! reads counter values of statistic and resets counter                 */
//            fdl_read_las_statistic_ctr = 0x1e,       /* NOT SUPPORTED! reads LAS and las_cycle_ctr and resets las_cycle_ctr                 */
//            await_indication = 0x1f,       /* provides resources for indication (sap-dependent)                    */
//            withdraw_indication = 0x20,       /* returnes indication-resources                                        */
//            load_routing_table = 0x21,       /* only for network-connection !!!                                      */
//            deactivate_routing_table = 0x22,       /* only for network-connection !!!                                      */
//            get_direct_conn = 0x23,       /* gets adress of next station                                          */
//        }; 

//        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
//        private struct S7OexchangeBlock
//        {
//            //Header
//            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)] public ushort[] unknown;
//            public byte headerlength; //Length of the Request Block without Userdata_1 and 2 (80 Bytes!)
//            public ushort user; //Application Specific
//            public byte rb_type; //Request Block type (always 2)
//            public byte priority; //Priority of the Task, identical like serv_class in the application block
//            public byte reserved_1;
//            public ushort reserved_2;
//            public byte subsystem; //For FDL Communication this is 22h = 34
//            public byte opcode; //request, confirm, indication => same as opcode in application block
//            public ushort response; //return-parameter => same as l_status in application block
//            public ushort fill_length_1;
//            public byte reserved_3;
//            public ushort seg_length_1; //Lengthz of Userdata_1
//            public ushort offset_1;
//            public ushort reserved_4;
//            public ushort fill_length_2;
//            public byte reserved_5;
//            public ushort seg_length_2;
//            public ushort offset_2;
//            public ushort reserved_6;
//            //End of Header

//            //Application Block
//            public byte application_block_opcode; // class of communication   (00 = request, 01=confirm, 02=indication)                                             
//            public byte application_block_subsystem; // number of source-task (only necessary for MTK-user !!!!!)             
//            public ushort application_block_id; // identification of FDL-USER                                            
//            public ushort application_block_service; // identification of service (00 -> SDA, send data with acknowlege)                                         
//            public byte application_block_local_address_station; // only for network-connection !!!                                       
//            public byte application_block_local_address_segment; // only for network-connection !!!                                      
//            public byte application_block_ssap; // source-service-access-point                                          
//            public byte application_block_dsap; // destination-service-access-point                                      
//            public byte application_block_remote_address_station; // address of the remote-station                                        
//            public byte application_block_remote_address_segment; // only for network-connection !!!                                      
//            public ushort application_block_service_class; // priority of service                                  
//            public Int32 application_block_receive_l_sdu_buffer_ptr; // address and length of received netto-data, exception:                
//            public byte application_block_receive_l_sdu_length; // address and length of received netto-data, exception:                    
//            public byte application_block_reserved_1; // (reserved for FDL !!!!!!!!!!)                                        
//            public byte application_block_reserved; // (reserved for FDL !!!!!!!!!!) 
//            public Int32 application_block_send_l_sdu_buffer_ptr; // address and length of send-netto-data, exception:                    
//            public byte application_block_send_l_sdu_length; // address and length of send-netto-data, exception:                        
//            // 1. csrd  : length means number of POLL-elements       
//            // 2. await_indication    : concatenation of application-blocks and   
//            //    withdraw_indication : number of application-blocks               
//            public ushort application_block_l_status; // link-status of service or update_state for srd-indication           
//            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)] public ushort[] application_block_reserved_2; // for concatenated lists       (reserved for FDL !!!!!!!!!!)          
//            //End Application block

//            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)] public byte[] reserved;
//            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)] public byte[] reference;

//            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)] public byte[] user_data_1;
//            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)] public byte[] user_data_2;

//        }

//        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
//        private struct UserDataConnectionConfig
//        {
//            public byte routing_enabled;
//            public byte b01;
//            public byte b02;
//            public byte b03;
//            public byte b04;
//            public byte b05;
//            public byte b06;
//            public byte b07;
//            public byte b08;
//            public byte destination_1;
//            public byte destination_2;
//            public byte destination_3;
//            public byte destination_4;
//            public byte b13;
//            public byte b14;
//            public byte b15;
//            public byte b16;
//            public byte b17;
//            public byte connection_type;
//            public byte rack_slot;
//            public byte b20;
//            public byte size_to_end;
//            public byte size_of_subnet;
//            public byte subnet_1;
//            public byte subnet_2;
//            public byte b25;
//            public byte b26;
//            public byte subnet_3;
//            public byte subnet_4;
//            public byte size_of_routing_destination;
//            public byte routing_destination_1;
//            public byte routing_destination_2;
//            public byte routing_destination_3;
//            public byte routing_destination_4;

//            public UserDataConnectionConfig(bool useConstructor) : this()
//            {
//                b01 = 0x02;
//                b02 = 0x01;
//                b04 = 0x0C;
//                b05 = 0x01;
//                b15 = 0x01;
//                b17 = 0x02;
//            }
//        }

//        #region External DLL References

//        private const int WM_USER = 0x0400;
//        private const int WM_SINEC = WM_USER + 500;

//        [DllImport("S7onlinx.dll")]
//        private static extern int SetSinecHWnd(int handle, IntPtr hwnd);

//        [DllImport("S7onlinx.dll")]
//        private static extern int SetSinecHWndMsg(int handle, IntPtr hwnd, uint msg_id);


//        [DllImport("S7onlinx.dll")]
//        private static extern int SCP_open([MarshalAs(UnmanagedType.LPStr)] string name);

//        [DllImport("S7onlinx.dll")]
//        private static extern int SCP_close(int handle);

//        [DllImport("S7onlinx.dll")]
//        private static extern int SCP_send(int handle, ushort length, byte[] data);

//        [DllImport("S7onlinx.dll")]
//        private static extern int SCP_receive(int handle, ushort timeout, int[] recievendlength, ushort length, byte[] data);

//        [DllImport("S7onlinx.dll")]
//        private static extern int SCP_get_errno();

//        #endregion

//        #region Internal Form derivate for WinProc Handling

//        /// <summary>
//        /// Internal Form, used for Recieving WinProc Messages, because Asynchronous S7Onlinx is working with them.
//        /// </summary>
//        private class _intForm : Form
//        {
//            private int _connectionHandle = -1;
//            private int fdrlen = 0;
//            private S7OnlineInterface Interface;

//            public _intForm(int ConnectionHandle, int fdrlen, S7OnlineInterface Interface)
//            {
//                _connectionHandle = ConnectionHandle;
//                this.fdrlen = fdrlen;
                
//                this.Interface = Interface;
//            }
            
//            protected override void WndProc(ref Message m)
//            {
//                if (m.Msg == WM_SINEC) //WM_SINEC Message recieved, recieve Data
//                {                   
//                    int[] rec_len = new int[1];
//                    byte[] buffer = new byte[fdrlen];
//                    SCP_receive(_connectionHandle, 0, rec_len, (ushort) fdrlen, buffer);

//                    GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
//                    S7OexchangeBlock rec = (S7OexchangeBlock)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(S7OexchangeBlock));
//                    handle.Free();

//                    if (Interface.ConnectionsList[rec.user].ConnectionEstablished)
//                        Interface.ConnectionsList[rec.user].SetRecievedPdu(new Pdu(rec.user_data_1));
//                    else
//                        Interface.ConnectionsList[rec.user].RecievedData = rec;
//                }
//                else
//                    base.WndProc(ref m);
//            }
//        }

//        #endregion

//        #region S7Online Exception

//        public class S7OnlineException : Exception
//        {
//            private static string GetErrTxt(int number)
//            {
//                switch (number)
//                {
//                    case 202:
//                        return "S7Online: Ressourcenengpaß im Treiber oder in der Library";
//                    case 203:
//                        return "S7Online: Konfigurationsfehler";
//                    case 205:
//                        return "S7Online: Auftrag zur Zeit nicht erlaubt";
//                    case 206:
//                        return "S7Online: Parameterfehler";
//                    case 207:
//                        return "S7Online: Gerät bereits/noch nicht geöffnet.";
//                    case 208:
//                        return "S7Online: CP reagiert nicht";
//                    case 209:
//                        return "S7Online: Fehler in der Firmware";
//                    case 210:
//                        return "S7Online: Speicherengpaß im Treiber";
//                    case 215:
//                        return "S7Online: Keine Nachricht vorhanden";
//                    case 216:
//                        return "S7Online: Fehler bei Zugriff auf Anwendungspuffer";
//                    case 219:
//                        return "S7Online: Timeout abgelaufen";
//                    case 225:
//                        return "S7Online: Die maximale Anzahl an Anmeldungen ist überschritten";
//                    case 226:
//                        return "S7Online: Der Auftrag wurde abgebrochen";
//                    case 233:
//                        return "S7Online: Ein Hilfsprogramm konnte nicht gestartet werden";
//                    case 234:
//                        return "S7Online: Keine Autorisierung für diese Funktion vorhanden";
//                    case 304:
//                        return "S7Online: Initialisierung noch nicht abgeschlossen";
//                    case 305:
//                        return "S7Online: Funktion nicht implementiert";
//                    case 4865:
//                        return "S7Online: CP-Name nicht vorhanden";
//                    case 4866:
//                        return "S7Online: CP-Name nicht konfiguriert";
//                    case 4867:
//                        return "S7Online: Kanalname nicht vorhanden";
//                    case 4868:
//                        return "S7Online: Kanalname nicht konfiguriert";
//                    default:
//                        return "S7Online: fehler nicht definiert";
//                }
//            }

//            private int _exceptionId;

//            private int ExceptionID
//            {
//                get { return _exceptionId; }
//            }

//            public S7OnlineException(int number) : base(GetErrTxt(number))
//            {
//                _exceptionId = number;
//            }

//            public S7OnlineException(string Message) : base(Message)
//            {
//            }
//        }

//        #endregion


//        private int _connectionHandle; //Handle from SCP_Open (for the Interface)

//        private _intForm _myForm; //Internal used form for WinProc Messages

//        private string _entryPoint;

//        public string EntryPoint
//        {
//            get { return _entryPoint; }
//        }

//        internal byte[] lastMessage = null;

//        public S7OnlineInterface(string EntryPoint)
//        {
//            _entryPoint = EntryPoint;

//            _connectionHandle = SCP_open(_entryPoint);

//            if (_connectionHandle < 0)
//            {
//                this.Dispose();
//                throw new S7OnlineException(SCP_get_errno());
//            }

//            S7OexchangeBlock fdr = new S7OexchangeBlock();


//            formBackgroundThread = new Thread(new ThreadStart(createFormInOtherThread));
//            formBackgroundThread.IsBackground = true;
//            formBackgroundThread.Start();
            
//            //_myForm = new _intForm(_connectionHandle, Marshal.SizeOf(fdr), this);            
//            //SetSinecHWndMsg(_connectionHandle, _myForm.Handle, WM_SINEC);
//            while (!WndProcReady)
//            { }
//        }

//        private Thread formBackgroundThread;
//        private bool WndProcReady = false;
//        private void createFormInOtherThread()
//        {
//            try
//            {
//                S7OexchangeBlock fdr = new S7OexchangeBlock();
//                _myForm = new _intForm(_connectionHandle, Marshal.SizeOf(fdr), this);
//                SetSinecHWndMsg(_connectionHandle, _myForm.Handle, WM_SINEC);
//                WndProcReady = true;
//                Application.Run();
//            }
//            catch (ThreadAbortException)
//            { }
//        }
        
//        private bool Disposed = false;
//        public void Dispose()
//        {
//            if (!Disposed)
//            {
//                Disposed = true;

//                if (_myForm != null) _myForm.Invoke((MethodInvoker) (() => _myForm.Dispose()));                

//                if (formBackgroundThread != null && formBackgroundThread.IsAlive)
//                    formBackgroundThread.Abort();

//                if (_connectionHandle >= 0)
//                {
//                    int ret = SCP_close(_connectionHandle);
//                    if (ret < 0)
//                        throw new S7OnlineException(SCP_get_errno());
//                }
//            }
//        }

//        private TimeSpan _timeOut = new TimeSpan(0, 0, 0, 1, 500);
//        public TimeSpan TimeOut
//        {
//            get { return _timeOut; }
//            set { _timeOut = value; }
//        }


//        private ushort connNr = 1;
//        internal Dictionary<int, Connection> ConnectionsList = new Dictionary<int, Connection>();
//        public Connection ConnectPlc(ConnectionConfig config)
//        {

//            //eine eindeutige connection id erzeugen
//            //in der connection eine liste der pdus pflegen, auf welche noch gewartet wird.
//            //für die pdu welche asynchron kommt nummer 0 verwenden!
            
//            S7OexchangeBlock fdr = new S7OexchangeBlock();
//            S7OexchangeBlock rec = new S7OexchangeBlock();
//            int len = Marshal.SizeOf(fdr);
//            byte[] buffer = new byte[len];
//            IntPtr ptr;
//            GCHandle handle;

//            Connection retVal;


//            while (ConnectionsList.ContainsKey(connNr))
//            {
//                connNr++;
//                if (connNr >= ushort.MaxValue)
//                    connNr = 1;
//            }
//            retVal = new Connection(this, config, 0);
//            retVal.ConnectionNumber = connNr;            
//            ConnectionsList.Add(connNr, retVal);

//            //Todo:
//            //Im Feld fdr.user für jede Connection einen eigenen Wert verwenden, so das bei empfangen Daten auch
//            //Festgestellt werden kann für welche Connection die sind.
//            //Dann muss eine Liste geführt werden, in der die IDs und die zugehörige Connection gespeichert ist.
//            //Wenn dann auf der ID was empfangen wird, wird das an die Connection weitergeleitet!

//            if (!config.ConnectionToEthernet)
//            {
//                #region Telegramm 1
//                fdr.subsystem = 0x22;
//                fdr.response = 0xFF;
//                fdr.user = connNr;//0xFF;
//                fdr.seg_length_1 = 0x80;
//                fdr.priority = 1;
//                fdr.application_block_service = (ushort)service_code.fdl_life_list_create_remote;

//                SendData(fdr);
//                rec = RecieveData(connNr);
//                #endregion

//                #region Telegramm 2
//                fdr.seg_length_1 = 0xF2;
//                fdr.application_block_service = (ushort) service_code.fdl_read_value;

//                SendData(fdr);
//                rec = RecieveData(connNr);
//                #endregion
//            }

//            #region Telegramm 3 (Ethernet 1)

//            fdr = new S7OexchangeBlock();
//            fdr.user = connNr;
//            fdr.response = 255;
//            fdr.subsystem = 0x40;
//            SendData(fdr);
//            rec = RecieveData(connNr);

//            retVal.application_block_opcode = rec.application_block_opcode;
//            retVal.application_block_subsystem = rec.application_block_subsystem;            
//            #endregion

//            #region Telegramm 4 (Ethernet 2)

//            fdr = new S7OexchangeBlock();

//            fdr.user = connNr;// 111;
//            fdr.subsystem = 64;
//            fdr.opcode = 1;
//            fdr.response = 255;
//            fdr.fill_length_1 = 126;
//            fdr.seg_length_1 = 126;
//            fdr.application_block_opcode = retVal.application_block_opcode;
//            fdr.application_block_ssap = 2;
//            fdr.application_block_remote_address_station = 114;
//            fdr.application_block_subsystem = retVal.application_block_subsystem; //When this is One it is a MPI Connection, zero means TCP Connection!
//            UserDataConnectionConfig ud_cfg = new UserDataConnectionConfig(true);
//            ud_cfg.rack_slot = (byte) (config.Slot + config.Rack*32);
//            ud_cfg.connection_type = (byte) config.ConnectionType;
//            if (config.ConnectionToEthernet)
//            {
//                ud_cfg.destination_1 = config.IPAddress.GetAddressBytes()[0];
//                ud_cfg.destination_2 = config.IPAddress.GetAddressBytes()[1];
//                ud_cfg.destination_3 = config.IPAddress.GetAddressBytes()[2];
//                ud_cfg.destination_4 = config.IPAddress.GetAddressBytes()[3];
//            }
//            else
//                ud_cfg.destination_1 = (byte) config.MPIAddress;

//            if (config.Routing)
//            {
//                ud_cfg.routing_enabled = 0x01;
//                ud_cfg.rack_slot = (byte) (config.RoutingSlot + config.RoutingRack*32);
//                ud_cfg.size_of_subnet = 0x06;
//                ud_cfg.routing_destination_1 = (byte) ((config.RoutingSubnet1 >> 8) & 0xFF);
//                ud_cfg.routing_destination_2 = (byte) ((config.RoutingSubnet1) & 0xFF);
//                ud_cfg.routing_destination_3 = (byte) ((config.RoutingSubnet2 >> 8) & 0xFF);
//                ud_cfg.routing_destination_4 = (byte) ((config.RoutingSubnet2) & 0xFF);

//                if (!config.RoutingToEthernet)
//                {
//                    ud_cfg.size_of_routing_destination = 0x01;
//                    ud_cfg.routing_destination_1 = (byte) config.RoutingMPIAddres;
//                    ud_cfg.size_to_end = 0x09;
//                }
//                else
//                {
//                    ud_cfg.size_of_routing_destination = 0x04;
//                    ud_cfg.routing_destination_1 = config.RoutingIPAddress.GetAddressBytes()[0];
//                    ud_cfg.routing_destination_2 = config.RoutingIPAddress.GetAddressBytes()[1];
//                    ud_cfg.routing_destination_3 = config.RoutingIPAddress.GetAddressBytes()[2];
//                    ud_cfg.routing_destination_4 = config.RoutingIPAddress.GetAddressBytes()[3];
//                    ud_cfg.size_to_end = 0x0C;
//                }
//            }
//            ptr = Marshal.AllocHGlobal(Marshal.SizeOf(ud_cfg));
//            Marshal.StructureToPtr(ud_cfg, ptr, true);
//            fdr.user_data_1 = new byte[260];
//            Marshal.Copy(ptr, fdr.user_data_1, 0, Marshal.SizeOf(ud_cfg));
//            Marshal.FreeHGlobal(ptr);
//            SendData(fdr);
//            rec = RecieveData(connNr);            
//            if (rec.response != 0x01)
//                throw new S7OnlineException("S7Online: Error Connection to PLC");

//            #endregion

//            #region Telegramm 5 (Ethernet 3) (this Telegramm sends a PDU)
//            //5th Telegramm / TCP(3rd)
//            Pdu pdu = new Pdu(1);
//            pdu.Param.AddRange(new byte[] {0xF0, 0, 0, 1, 0, 1, 3, 0xc0});
//            SendPdu(pdu, retVal);
//            Pdu recPdu = RecievePdu(connNr);            
//            #endregion

//            #region Telegramm 6 (Ethernet 4) (get PDU size)
//            fdr = new S7OexchangeBlock();
//            fdr.user = connNr;//0;
//            fdr.subsystem = 64;
//            fdr.opcode = 7;
//            fdr.response = 16642;
//            fdr.seg_length_1 = 480;
//            fdr.application_block_opcode = retVal.application_block_opcode;
//            fdr.application_block_subsystem = retVal.application_block_subsystem;
//            SendData(fdr);
//            recPdu = RecievePdu(connNr);
//            retVal.PduSize = ByteFunctions.getU16from(recPdu.Param.ToArray(), 6);
//            #endregion

//            retVal.ConnectionEstablished = true;

//            return retVal;
//        }

//        public void DisconnectPlc(Connection conn)
//        {
//            conn.ConnectionEstablished = false;
//            S7OexchangeBlock fdr;

//            if (conn.ConnectionConfig.ConnectionToEthernet)
//            {
//                fdr = new S7OexchangeBlock();
//                fdr.user = (ushort) conn.ConnectionNumber;
//                fdr.subsystem = 64;
//                fdr.opcode = 8;
//                fdr.response = 255;
//                fdr.application_block_opcode = conn.application_block_opcode;
//                SendData(fdr);
//                RecieveData((ushort) conn.ConnectionNumber);
//            }

//            fdr = new S7OexchangeBlock();
//            fdr.user = (ushort)conn.ConnectionNumber;            
//            fdr.subsystem = 64;
//            fdr.opcode = 0xC;
//            fdr.response = 255;
//            fdr.application_block_service = 0x8000;
//            fdr.application_block_opcode = conn.application_block_opcode;
//            fdr.application_block_subsystem = conn.application_block_subsystem;
//            SendData(fdr);
//            RecieveData((ushort)conn.ConnectionNumber);

//            ConnectionsList.Remove(conn.ConnectionNumber);
//        }


//        public List<int> ListReachablePartners()
//        {
//            throw new NotImplementedException();
//        }         			

//        private void SendData(S7OexchangeBlock data)
//        {
//            if (!Disposed)
//            {
//                int len = Marshal.SizeOf(data);
//                byte[] buffer = new byte[len];

//                data.headerlength = 80; //Length of the Header (always 80)  (but the 4 first unkown bytes are not count)
//                data.rb_type = 2; //rb_type is always 2
//                data.offset_1 = 80; //Offset of the Begin of userdata (but the 4 first unkown bytes are not count)	

//                IntPtr ptr = Marshal.AllocHGlobal(len);
//                Marshal.StructureToPtr(data, ptr, true);
//                Marshal.Copy(ptr, buffer, 0, len);
//                Marshal.FreeHGlobal(ptr);

//                int ret = SCP_send(_connectionHandle, (ushort) (data.seg_length_1 + data.headerlength), buffer);
//                if (ret < 0)
//                    throw new S7OnlineException(SCP_get_errno());
//            }
//            else
//            {
//                throw new ObjectDisposedException(this.ToString());
//            }
//        }

//        public void SendPdu(Pdu pdu, Connection connection)
//        {
//            if (!Disposed)
//            {
//                byte[] pdubytes = pdu.ToBytes();

//                S7OexchangeBlock fdr = new S7OexchangeBlock();

//                fdr.user = (ushort) connection.ConnectionNumber;
//                fdr.subsystem = 64;
//                fdr.opcode = 6;  
//                fdr.response = 255;
//                fdr.fill_length_1 = (byte) pdubytes.Length;
//                fdr.seg_length_1 = (byte) pdubytes.Length;
//                fdr.application_block_opcode = connection.application_block_opcode;
//                //if (!connection.ConnectionConfig.ConnectionToEthernet)
//                fdr.application_block_subsystem = connection.application_block_subsystem;


//                fdr.headerlength = 80; //Length of the Header (always 80)  (but the 4 first unkown bytes are not count)
//                fdr.rb_type = 2; //rb_type is always 2
//                fdr.offset_1 = 80; //Offset of the Begin of userdata (but the 4 first unkown bytes are not count)	

//                fdr.user_data_1 = new byte[260];
//                Array.Copy(pdubytes, fdr.user_data_1, pdubytes.Length);                          

//                int len = Marshal.SizeOf(fdr);
//                byte[] buffer = new byte[len];
//                IntPtr ptr = Marshal.AllocHGlobal(len);
//                Marshal.StructureToPtr(fdr, ptr, true);
//                Marshal.Copy(ptr, buffer, 0, len);
//                Marshal.FreeHGlobal(ptr);

//                int ret = SCP_send(_connectionHandle, (ushort)(fdr.seg_length_1 + fdr.headerlength), buffer);
//                if (ret < 0)
//                    throw new S7OnlineException(SCP_get_errno());
//            }
//            else
//            {
//                throw new ObjectDisposedException(this.ToString());
//            }
//        }

//        public void ExchangePdu(Pdu pdu, Connection connection)
//        {
//            SendPdu(pdu, connection);
//            while (connection.RecievedPdus[pdu.header.number] == null)
//            { }
//            connection.RecievedPdus[pdu.header.number] = null;
//            GetResponsePdu(connection);
//        }

//        private void GetResponsePdu(Connection connection)
//        {
//            if (!Disposed)
//            {                
//                S7OexchangeBlock fdr = new S7OexchangeBlock();
                
//                fdr.user = (ushort)connection.ConnectionNumber;
//                fdr.subsystem = 64;
//                fdr.opcode = 7; 
                
//                fdr.seg_length_1 = 480;
//                fdr.response = 16642;
                
//                fdr.application_block_opcode = connection.application_block_opcode;               
//                fdr.application_block_subsystem = connection.application_block_subsystem;

//                fdr.headerlength = 80; //Length of the Header (always 80)  (but the 4 first unkown bytes are not count)
//                fdr.rb_type = 2;       //rb_type is always 2
//                fdr.offset_1 = 80;     //Offset of the Begin of userdata (but the 4 first unkown bytes are not count)	

//                fdr.user_data_1 = new byte[260];

//                int len = Marshal.SizeOf(fdr);
//                byte[] buffer = new byte[len];
//                IntPtr ptr = Marshal.AllocHGlobal(len);
//                Marshal.StructureToPtr(fdr, ptr, true);
//                Marshal.Copy(ptr, buffer, 0, len);
//                Marshal.FreeHGlobal(ptr);

//                int ret = SCP_send(_connectionHandle, (ushort)(fdr.seg_length_1 + fdr.headerlength), buffer);
//                if (ret < 0)
//                    throw new S7OnlineException(SCP_get_errno());
//            }
//            else
//            {
//                throw new ObjectDisposedException(this.ToString());
//            }
//        }

//        private S7OexchangeBlock RecieveData(ushort myConnNr)
//        {
//            while (ConnectionsList[myConnNr].RecievedData == null)
//            { }

//            S7OexchangeBlock rec = (S7OexchangeBlock)ConnectionsList[myConnNr].RecievedData;
//            ConnectionsList[myConnNr].RecievedData = null;

//            return rec;
//        }
        
//        private Pdu RecievePdu(ushort myConnNr)
//        {
//            S7OexchangeBlock rec = RecieveData(myConnNr);
//            return new Pdu(rec.user_data_1);
//        }        
//    }
//}
