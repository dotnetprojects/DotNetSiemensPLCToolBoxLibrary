using System;
using System.Runtime.InteropServices;

namespace DotNetSiemensPLCToolBoxLibrary.DBF.Structures.DBT
{
    /// <summary>
    /// The header of a DBT file
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    internal struct DBTHeader
    {
        public Int32 nextBlockID;
        [MarshalAs(UnmanagedType.ByValTStr /*Array*/, SizeConst = 4)]
        public string /* byte[] */  reserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string fileName;
        public byte version; // 0x03 = Version III, 0x00 = Version IV
        [MarshalAs(UnmanagedType.ByValTStr /*Array*/, SizeConst = 3)]
        public string /*byte[]*/ reserved3;
        public Int16 blockLength;
        [MarshalAs(UnmanagedType.ByValTStr /*Array*/, SizeConst = 490)]
        public string /*byte[]*/ reserved4;
    }
}
