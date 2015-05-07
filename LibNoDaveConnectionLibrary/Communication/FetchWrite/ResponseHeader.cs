using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.FetchWrite
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ResponseHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public char[] system_id;

        public byte header_length;

        public byte opcode_id;

        public byte opcode_length;

        public byte opcode;

        public byte ack;

        public byte ack_length;

        public byte error;

        public byte ack_empty;

        public byte ack_empty_length;

        public byte free1;
        public byte free2;
        public byte free3;
        public byte free4;
        public byte free5;
    }
}
