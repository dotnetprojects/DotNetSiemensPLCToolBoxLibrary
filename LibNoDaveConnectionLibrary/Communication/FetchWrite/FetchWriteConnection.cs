using DotNetSiemensPLCToolBoxLibrary.Communication.Networking;
using DotNetSiemensPLCToolBoxLibrary.DataTypes;
using System;
using System.Net;
using System.Runtime.InteropServices;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.FetchWrite
{
    public class FetchWriteConnection : IDisposable
    {
        private TCPFunctionsAsync _tcp;
        private TCPFunctionsAsync _tcpWrite;

        private PLCConnectionConfiguration configuration;

        private bool _connected = false;
        private bool _connectedWrite = false;

        public FetchWriteConnection(PLCConnectionConfiguration configuration)
        {
            this._tcp = new TCPFunctionsAsync(null, IPAddress.Parse(configuration.CpuIP), configuration.Port, true);
            _tcp.ConnectionEstablished += _tcp_ConnectionEstablished;
            _tcp.ConnectionClosed += _tcp_ConnectionClosed;
            _tcp.DoNotUseRecieveEvent = true;
            _tcp.UseKeepAlive = true;
            if (configuration.WritePort != 0)
            {
                if (configuration.WritePort == configuration.Port)
                    _tcpWrite = _tcp;
                else
                {
                    this._tcpWrite = new TCPFunctionsAsync(null, IPAddress.Parse(configuration.CpuIP), configuration.WritePort, true);
                    _tcpWrite.DoNotUseRecieveEvent = true;
                    _tcpWrite.UseKeepAlive = true;
                }
                _tcpWrite.ConnectionEstablished += _tcpWrite_ConnectionEstablished;
                _tcpWrite.ConnectionClosed += _tcpWrite_ConnectionClosed;
            }
        }

        private void _tcpWrite_ConnectionClosed(System.Net.Sockets.TcpClient obj)
        {
            _connectedWrite = false;
        }

        private void _tcpWrite_ConnectionEstablished(System.Net.Sockets.TcpClient obj)
        {
            _connectedWrite = true;
        }

        private void _tcp_ConnectionClosed(System.Net.Sockets.TcpClient obj)
        {
            _connected = false;
        }

        private void _tcp_ConnectionEstablished(System.Net.Sockets.TcpClient obj)
        {
            _connected = true;
        }

        public void Connect()
        {
            _tcp.AutoReConnect = true;
            _tcp.Start();
            if (_tcp != _tcpWrite && _tcpWrite != null)
            {
                _tcpWrite.AutoReConnect = true;
                _tcpWrite.Start();
            }
        }

        public void Dispose()
        {
            _tcp.AutoReConnect = false;
            _tcp.Stop();
            if (_tcp != _tcpWrite && _tcpWrite != null)
            {
                _tcpWrite.AutoReConnect = false;
                _tcpWrite.Stop();
            }
        }

        public void ReadValue(PLCTag tag)
        {
            if (_connected)
            {
                var rqHeader = new RequestHeader();
                rqHeader.system_id = new[] { 'S', '5' };
                rqHeader.header_length = (byte)Marshal.SizeOf(typeof(RequestHeader));
                rqHeader.opcode_id = 1;
                rqHeader.opcode_length = 3;
                rqHeader.opcode = (byte)OperationCode.Fetch;
                rqHeader.org = 3;
                rqHeader.org_length = 8;
                switch (tag.TagDataSource)
                {
                    case MemoryArea.Datablock:
                    case MemoryArea.InstanceDatablock:
                        rqHeader.org_id = (byte)OrgTypes.DB;
                        break;

                    case MemoryArea.Inputs:
                        rqHeader.org_id = (byte)OrgTypes.Inputs;
                        break;

                    case MemoryArea.Outputs:
                        rqHeader.org_id = (byte)OrgTypes.Outputs;
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
                if (sz % 2 > 0)
                    sz++;

                rqHeader.length1 = (byte)(((sz / 2) & 0xff00) >> 8);
                rqHeader.length2 = (byte)(((sz / 2) & 0x00ff));
                rqHeader.req_empty = 0xff;
                rqHeader.req_empty_length = 2;

                var bytes = getBytes(rqHeader);

                _tcp.SendData(bytes);

                var header = new byte[Marshal.SizeOf(typeof(ResponseHeader))];
                var readBytes = 0;
                while (readBytes < header.Length)
                {
                    readBytes += _tcp.NetworkStream.Read(header, readBytes, header.Length - readBytes);
                }
                var response = fromBytes<ResponseHeader>(header);

                var data = new byte[sz];
                readBytes = 0;
                while (readBytes < sz)
                {
                    readBytes += _tcp.NetworkStream.Read(data, readBytes, sz - readBytes);
                }

                tag.ParseValueFromByteArray(data, tag.ByteAddress % 2 > 0 ? 1 : 0);
            }
        }

        public void WriteValue(PLCTag tag)
        {
            if (_connectedWrite)
            {
                var rqHeader = new RequestHeader();
                rqHeader.system_id = new[] { 'S', '5' };
                rqHeader.header_length = (byte)Marshal.SizeOf(typeof(RequestHeader));
                rqHeader.opcode_id = 1;
                rqHeader.opcode_length = 3;
                rqHeader.opcode = (byte)OperationCode.Write;
                rqHeader.org = 3;
                rqHeader.org_length = 8;
                switch (tag.TagDataSource)
                {
                    case MemoryArea.Datablock:
                    case MemoryArea.InstanceDatablock:
                        rqHeader.org_id = (byte)OrgTypes.DB;
                        break;

                    case MemoryArea.Inputs:
                        rqHeader.org_id = (byte)OrgTypes.Inputs;
                        break;

                    case MemoryArea.Outputs:
                        rqHeader.org_id = (byte)OrgTypes.Outputs;
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
                if (sz % 2 > 0)
                    sz++;

                rqHeader.length1 = (byte)(((sz / 2) & 0xff00) >> 8);
                rqHeader.length2 = (byte)(((sz / 2) & 0x00ff));
                rqHeader.req_empty = 0xff;
                rqHeader.req_empty_length = 2;

                var bytes = getBytes(rqHeader);

                var writeByte = new byte[bytes.Length + sz];
                Array.Copy(bytes, 0, writeByte, 0, bytes.Length);

                var putPos = bytes.Length;
                if (tag.ReadByteSize % 2 > 0)
                    putPos++;

                if (tag.TagDataType == TagDataType.Bool)
                {
                    if (object.Equals(tag.Controlvalue, true))
                        writeByte[putPos] = (byte)(Math.Pow(2, (tag.BitAddress)));
                    else
                        writeByte[putPos] = 0;
                }
                else
                {
                    tag._putControlValueIntoBuffer(writeByte, putPos);
                }

                _tcpWrite.SendData(writeByte);
                var data = new byte[Marshal.SizeOf(typeof(ResponseHeader))];
                var readBytes = 0;
                while (readBytes < data.Length)
                {
                    readBytes += _tcpWrite.NetworkStream.Read(data, readBytes, data.Length - readBytes);
                }

                var response = fromBytes<ResponseHeader>(data);
            }
        }

        private byte[] getBytes<T>(T str) where T : struct
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        private T fromBytes<T>(byte[] arr) where T : struct
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