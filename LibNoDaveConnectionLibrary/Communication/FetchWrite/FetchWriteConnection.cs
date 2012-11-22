using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using DotNetSiemensPLCToolBoxLibrary.Communication.Networking;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.FetchWrite
{
    public class FetchWriteConnection
    {
        private TCPFunctionsAsync tcp;

        private byte PLCConnectionID;

        
        public FetchWriteConnection(TCPFunctionsAsync tcp, byte PLCConnectionID, bool UseIsoProtokoll)
        {
            this.tcp = tcp;

            this.PLCConnectionID = PLCConnectionID;

            tcp.DataRecieved += tcp_DataRecieved;
        }

        void tcp_DataRecieved(byte[] data, System.Net.Sockets.TcpClient client)
        {
            if (data[0] == (byte)'S' && data[1] == (byte)'5' && data[2] == (byte)16)
            {
                
            }
        }

        public void ReadValue(PLCTag tag)
        {
            var telHead = new byte[] { (byte)'S', (byte)'5', 16, 1, 3, 5, 3, 8, PLCConnectionID, (byte)tag.DatablockNumber, 0, 0, 0, 0, 0xff, 2 };
            LibNoDave.libnodave.putBCD16at(telHead, 10, tag.ByteAddress);
            LibNoDave.libnodave.putBCD16at(telHead, 10, tag.ReadByteSize);

            tcp.SendData(telHead);
        }

        public void WriteValue(PLCTag tag)
        {

            var telHead = new byte[] { (byte)'S', (byte)'5', 16, 1, 3, 3, 3, 8, PLCConnectionID, (byte)tag.DatablockNumber, 0, 0, 0, 0, 0xff, 2 };
            LibNoDave.libnodave.putBCD16at(telHead, 10, tag.ByteAddress);
            LibNoDave.libnodave.putBCD16at(telHead, 10, tag.ReadByteSize);

            var tel = new byte[telHead.Length + tag.ReadByteSize];

            Array.Copy(telHead, 0, tel, 0, telHead.Length);
        }
    }
}
