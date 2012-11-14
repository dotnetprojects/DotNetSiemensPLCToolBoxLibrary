using System;
using System.Runtime.InteropServices;

namespace DotNetSiemensPLCToolBoxLibrary.DBF.Structures.DBT
{
    /// <summary>
    /// The header of a memo
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    internal struct MemoHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;
        public Int16 startPosition;
        public Int32 fieldLength;
    }
}
