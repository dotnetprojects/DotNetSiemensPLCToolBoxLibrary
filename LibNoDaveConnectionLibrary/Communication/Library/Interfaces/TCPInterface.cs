//using System;
//using System.Net;
//using DotNetSiemensPLCToolBoxLibrary.Communication.Library.Pdus;

//namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library.Interfaces
//{
    
//    public class TCPInterface// : ISONetworkInterface
//    {
//        //IPAddress addr;
//        public TcpNETdave _dc = null;
//        TimeSpan TimeOut;
//        PLCConnectionConfiguration conf = new PLCConnectionConfiguration();

//        public TCPInterface()
//        {
//            TimeOut = TimeSpan.FromMilliseconds(5000);
//        }
//        public void SetTimeOut(int timeout_ms)
//        {
//            TimeOut = TimeSpan.FromMilliseconds(timeout_ms);
//        }

//        public void disconnectAdapter()
//        {

//        }
//        public void ConnectPlc(ConnectionConfig config)
//        {
//        }
//        public void ConnectPlc(PLCConnectionConfiguration config)
//        {
//            conf = config;
//            //_dc = new TcpNETdave(conf.CpuIP, conf.Port, conf.CpuRack, conf.CpuSlot);
//            _dc = new TcpNETdave(conf);
//            _dc.connectPLC();
//        }
//        public bool Connected()
//        {
//            return _dc != null ? _dc.Connected() : false;
//        }
//        //protected override byte[] ConnectPlc(byte cpuMpi, byte cpuRack, byte cpuSlot, byte connType, bool routing, bool routingDestIsIp, IPAddress routingIp, byte routingMpi, byte routingRack, byte routingSlot, byte routingConnType)
//        //{
            
///*            byte[] stdConn = {0x11, //Length
//                              0xE0, // TDPU Type CR = Connection Request (see RFC1006/ISO8073)
//                              0x00, 0x00, // TPDU Destination Reference (unknown)
//                              0x00, 0x01, // TPDU Source-Reference (my own reference, should not be zero)
//                              0x00, // TPDU Class 0 and no Option 
//                              0xC1, // Parameter Source-TSAP
//                              2, // Length of this parameter 
//                              1, // Everytime 1
//                              0, // Everytime 0
//                              0xC2, // Parameter Destination-TSAP
//                              2, // Length of this parameter 
//                              1, // Function (1=PG,2=OP,3=Step7Basic)
//                              (byte) (cpuSlot + cpuRack*32), // Rack (Bit 7-5) and Slot (Bit 4-0)
//                              0xC0, // Parameter requested TPDU-Size
//                              1, // Length of this parameter 
//                              9, // requested TPDU-Size 8=256 Bytes, 9=512 Bytes , a=1024 Bytes
//                             };

            
//            byte[] routingConn ={			// for routing
//		6 + 30 + 30 + 3,	// Length over all without this byte (fixed
//		// Data 6 Bytes + size of Parameters (3 for C0h,30 for C1h+C2h)

//		0xE0,		// TDPU Type CR = Connection Request (see RFC1006/ISO8073)
//		0x00,0x00,	// TPDU Destination Reference (unknown)
//		0x00,0x01,	// TPDU Source-Reference (my own reference, should not be zero)
//		0x00,		// TPDU Class 0 and no Option 

//		0xC1,		// Parameter Source-TSAP
//		28,		// Length of this parameter 
//		1,		// one block of data (???)
//		0,		// Length for S7-Subnet-ID
//		0,		// Length of PLC-Number
//		2,		// Length of Function/Rack/Slot
//		0,0,0,0,0,0,0,0,	// empty Data 
//		0,0,0,0,0,0,0,0,
//		0,0,0,0,0,0,
//		1,		// Function (1=PG,2=OP,3=Step7Basic)
//		(byte)(cpuSlot + cpuRack * 32),		// Rack (Bit 7-5) and Slot (Bit 4-0)

//		0xC2,		// Parameter Destination-TSAP
//		28,		// Length of this parameter 
//		1,		// one block of data (???)
//		6,		// Length for S7-Subnet-ID
//		(byte)(routingDestIsIp==true ? 4 : 1) ,		// Length of PLC-Number - 04 if you use a IP as Destination!
//		2,		// Length of Function/Rack/Slot

//		(byte) (dc->routingSubnetFirst >> 8), (unsigned char) dc->routingSubnetFirst,	// first part of S7-Subnet-ID 
//		// (look into the S7Project/Network configuration)
//		0x00,0x00,		// fix always 0000 (reserved for later use ?)
//		(unsigned char) (dc->routingSubnetSecond >> 8), (unsigned char) dc->routingSubnetSecond,		// second part of S7-Subnet-ID 
//		// (see S7Project/Network configuration)

//		dc->_routingDestination1,			// PLC-Number (0-126) or IP Adress (then 4 Bytes are used)
//		dc->_routingDestination2,
//		dc->_routingDestination3,
//		dc->_routingDestination4,

//		0,0,0,0,0,	// empty 
//		0,0,0,0,0,0,0,

//		1,		// Function (1=PG,2=OP,3=Step7Basic)
//		(dc->routingSlot + dc->routingRack*32),		// Rack (Bit 7-5) and Slot (Bit 4-0)
//		// 0 for slot = let select the plc itself the correct slotnumber

//		0xC0,		// Parameter requested TPDU-Size
//		1,		// Length of this parameter 
//		9,		// requested TPDU-Size 8=256 Bytes, 9=512 Bytes , a=1024 Bytes
//        }*/
      
    
  	           
//          //  byte[] res;

//            //sendISOPacket(stdConn);
//          //  res = readISOPacket();

//          //  return null;
//        //}

//        private void sendISOPacket(byte[] message)
//        {
//            byte[] _message = new byte[message.Length + 4];
//            _message[0] = 0x03;
//            _message[1] = 0x0;
//            _message[2] = (byte) (message.Length/0x100);
//            _message[3] = (byte) (message.Length%0x100);

//            Array.Copy(message, 0, _message, 4, message.Length);

//            //sendPacket(_message);

            
//        }

//        private byte[] readISOPacket()
//        {

//            //byte[] res = readPacket();
//            //return readPacket();
//            return null;
//        }
//        public void ExchangePdu(Pdu pdu, Connection connection)
//        {
//        }
//    }
//}
