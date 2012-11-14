using System;
using System.Runtime.InteropServices;

namespace DotNetSiemensPLCToolBoxLibrary.DBF.Structures.MDX
{
    /// <summary>
    /// A MDX file header
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    internal struct MDXHeader
    {
        public byte version;
        public byte creationYear;
        public byte creationMonth;
        public byte creationDay;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string fileName;
        public Int16 blockSize;
        public Int16 blockSizeAdderN;
        public byte productionIndexflag;
        public byte numberOfEntrysInTag;
        public byte lengthOfTag;
        public byte reserved1;
        public Int16 numberOfTagsInUse;
        public Int16 reserved2;
        public Int32 numberOfPagesInTagfile;
        public Int32 pointerToFirstfreePage;
        public Int32 numberOfBlockAviable;
        public byte updateYear;
        public byte updateMonth;
        public byte updateDay;
        public byte reserved3;
        [MarshalAs(UnmanagedType.ByValTStr /*Array*/, SizeConst = 496)]
        public string /*byte[]*/ garbage;
    }
}
