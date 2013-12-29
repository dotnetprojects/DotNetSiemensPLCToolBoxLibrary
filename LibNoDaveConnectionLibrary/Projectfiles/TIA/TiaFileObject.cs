using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Structs;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA
{
    public class TiaFileObject
    {
        public TiaObjectHeader Header { get; set; }
        public byte[] Data { get; set; }
        public TiaFileObject(TiaObjectHeader header, byte[] data)
        {
            this.Header = header;
            this.Data = data;
        }
    }
}
