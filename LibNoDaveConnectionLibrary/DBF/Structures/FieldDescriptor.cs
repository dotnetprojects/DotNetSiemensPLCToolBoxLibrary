using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.DBF.Enums;

namespace DotNetSiemensPLCToolBoxLibrary.DBF.Structures
{
    /// <summary>
    /// The DBF field descriptor structure. There will be one of these for each column in the table.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    internal struct FieldDescriptor
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string fieldName;
        public dBaseType fieldType;//public byte fieldType;  //char
        public Int32 address;
        public byte fieldLen;
        public byte count;
        public Int16 reserved1;
        public byte workArea;
        public Int16 reserved2;
        public byte flag;
        [MarshalAs(UnmanagedType.ByValTStr /* Array */, SizeConst = 7)] //Changed Type to Sting, Monotouch Compiler has Problems with Bytearrays when using PtrtoStructure
        public string /* byte[] */ reserved3;
        public byte indexFlag;

        public override string ToString()
        {
            return "Field-Name: " + fieldName;
        }
    }
}
