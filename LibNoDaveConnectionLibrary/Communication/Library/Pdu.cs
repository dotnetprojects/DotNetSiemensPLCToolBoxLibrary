using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library
{
    public class Pdu
    {
        public byte[] Header { get; set; }
        public byte[] Param { get; set; }
        public byte[] Data { get; set; }
    }
}
