using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Enums;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct TiaObjectHeaderLegacy
    {
        public int Size;
        public int TypeId;
        public long InstanceId;
        public int ClusterId;
        public int BackRefOffset;
        public sbyte OffsetCount;
        public TiaObjectStatesLegacy StateFlags;
        public sbyte MetaInfo;
    }
}
