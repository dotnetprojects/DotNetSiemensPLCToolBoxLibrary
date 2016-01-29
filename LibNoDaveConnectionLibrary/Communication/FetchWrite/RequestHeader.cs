using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.FetchWrite
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct RequestHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)] 
        public char[] system_id;

        public byte header_length;

        public byte opcode_id;

        public byte opcode_length;

        public byte opcode;

        public byte org;

        public byte org_length;

        public byte org_id;

        public byte dbnr;

        public byte start_address1;
        public byte start_address2;

        public byte length1;
        public byte length2;

        public byte req_empty;

        public byte req_empty_length;
    }
}
