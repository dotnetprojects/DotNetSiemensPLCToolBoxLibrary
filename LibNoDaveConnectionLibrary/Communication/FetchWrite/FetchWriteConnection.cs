using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using DotNetSiemensPLCToolBoxLibrary.Communication.Networking;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.FetchWrite
{
    public class FetchWriteConnection:IDisposable
    {
        private TCPFunctionsAsync _tcp;

        private PLCConnectionConfiguration configuration;


        public FetchWriteConnection(PLCConnectionConfiguration configuration)
        {
            this._tcp = new TCPFunctionsAsync(null, IPAddress.Parse(configuration.CpuIP), configuration.Port, true);
            _tcp.DoNotUseRecieveEvent = true;
        }

        public void Connect()
        {
            _tcp.AutoReConnect = true;
            _tcp.Start();
        }

        public void Dispose()
        {
            _tcp.AutoReConnect = false;
            _tcp.Stop();
        }
 
        public void ReadValue(PLCTag tag)
        {
            var rqHeader = new RequestHeader();
            rqHeader.system_id = new[] {'S', '5'};
            rqHeader.header_length = (byte) Marshal.SizeOf(typeof (RequestHeader));
            rqHeader.opcode_id = 1;
            rqHeader.opcode_length = 3;
            rqHeader.opcode = (byte) OperationCode.Fetch;
            rqHeader.org = 3;
            rqHeader.org_length = 8;
            switch (tag.TagDataSource)
            {
                case MemoryArea.Datablock:
                case MemoryArea.InstanceDatablock:
                    rqHeader.org_id = (byte) OrgTypes.DB;
                    break;
                case MemoryArea.Inputs:
                    rqHeader.org_id = (byte) OrgTypes.Inputs;
                    break;
                case MemoryArea.Outputs:
                    rqHeader.org_id = (byte) OrgTypes.Outputs;
                    break;
                case MemoryArea.S5_DX:
                    rqHeader.org_id = (byte)OrgTypes.DBx;
                    break;
                case MemoryArea.Flags:
                    rqHeader.org_id = (byte)OrgTypes.Flags;
                    break;
                case MemoryArea.Counter:
                    rqHeader.org_id = (byte)OrgTypes.Counters;
                    break;
                case MemoryArea.Timer:
                    rqHeader.org_id = (byte)OrgTypes.Timer;
                    break;
            }
            rqHeader.dbnr = (byte)tag.DataBlockNumber;
            rqHeader.start_address1 = (byte)(((tag.ByteAddress / 2) & 0xff00) >> 8);
            rqHeader.start_address2 = (byte)(((tag.ByteAddress / 2) & 0x00ff));

            var sz = tag.ReadByteSize;
            if (sz%2 > 0)
                sz++;

            rqHeader.length1 = (byte)(((sz / 2) & 0xff00) >> 8);
            rqHeader.length2 = (byte)(((sz / 2) & 0x00ff));
            rqHeader.req_empty = 0xff;
            rqHeader.req_empty_length = 2;

            var bytes = getBytes(rqHeader);

            _tcp.SendData(bytes);
            
            var header = new byte[Marshal.SizeOf(typeof (ResponseHeader))];
            var readBytes = 0;
            while (readBytes < header.Length)
            {
                readBytes = _tcp.NetworkStream.Read(header, readBytes, header.Length - readBytes);
            }
            var response = fromBytes<ResponseHeader>(header);

            var data = new byte[sz];
            readBytes = 0;
            while (readBytes < sz)
            {
                readBytes = _tcp.NetworkStream.Read(data, readBytes, sz - readBytes);
            }

            tag.ParseValueFromByteArray(data, tag.ByteAddress % 2 > 0 ? 1 : 0);
        }

        public void WriteValue(PLCTag tag)
        {

            //var telHead = new byte[] { (byte)'S', (byte)'5', 16, 1, 3, 3, 3, 8, PLCConnectionID, (byte)tag.DataBlockNumber, 0, 0, 0, 0, 0xff, 2 };
            //LibNoDave.libnodave.putBCD16at(telHead, 10, tag.ByteAddress);
            //LibNoDave.libnodave.putBCD16at(telHead, 10, tag.ReadByteSize);

            //var tel = new byte[telHead.Length + tag.ReadByteSize];

            //Array.Copy(telHead, 0, tel, 0, telHead.Length);
        }

        byte[] getBytes<T>(T str) where T:struct 
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        T fromBytes<T>(byte[] arr) where T:struct 
        {
            T str = new T();

            int size = Marshal.SizeOf(str);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            str = (T)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);

            return str;
        }
    }
}
