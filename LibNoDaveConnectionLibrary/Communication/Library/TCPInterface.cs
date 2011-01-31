using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library
{
    public class TCPInterface : NetworkInterface
    {

        protected override byte[] ConnectPlc(byte cpuMpi, byte cpuRack, byte cpuSlot, byte connType, bool routing, bool routingDestIsIp, IPAddress routingIp, byte routingMpi, byte routingRack, byte routingSlot, byte routingConnType)
        {
            int rack = 0, slot = 0;
            byte[] stdConn = {0x11, //Length
                              0xE0, // TDPU Type CR = Connection Request (see RFC1006/ISO8073)
                              0x00, 0x00, // TPDU Destination Reference (unknown)
                              0x00, 0x01, // TPDU Source-Reference (my own reference, should not be zero)
                              0x00, // TPDU Class 0 and no Option 
                              0xC1, // Parameter Source-TSAP
                              2, // Length of this parameter 
                              1, // Everytime 1
                              0, // Everytime 0
                              0xC2, // Parameter Destination-TSAP
                              2, // Length of this parameter 
                              1, // Function (1=PG,2=OP,3=Step7Basic)
                              (byte) (slot + rack*32), // Rack (Bit 7-5) and Slot (Bit 4-0)
                              0xC0, // Parameter requested TPDU-Size
                              1, // Length of this parameter 
                              9, // requested TPDU-Size 8=256 Bytes, 9=512 Bytes , a=1024 Bytes
                             };

            sendISOPacket(stdConn);

            return null;
        }


        private void sendISOPacket(byte[] message)
        {
            byte[] _message = new byte[message.Length + 4];
            _message[0] = 0x03;
            _message[1] = 0x0;
            _message[2] = (byte) (message.Length/0x100);
            _message[3] = (byte) (message.Length%0x100);

            Array.Copy(message, 0, _message, 4, message.Length);

            sendPacket(_message);
        }

        private byte[] readISOPacket()
        {
            return readPacket();
        }        
    }
}
