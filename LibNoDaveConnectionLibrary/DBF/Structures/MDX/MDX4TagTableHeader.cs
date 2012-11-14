using System;
using System.IO;
using System.Runtime.InteropServices;
using DotNetSiemensPLCToolBoxLibrary.DBF.Enums;

namespace DotNetSiemensPLCToolBoxLibrary.DBF.Structures.MDX
{
    /// <summary>
    /// A MDX4 Tag Table Header
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    internal struct MDX4TagTableHeader
    {
        public Int32 tagHeaderPageNumber;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string tagName;
        public TagKeyFormat keyFormat; //0x00 = Calculated, 0x10 = Data Field
        public byte forwardTagThreadLeft;
        public byte forwardTagThreadRight;
        public byte backwardTagThread;
        public byte reserved1;
        public dBaseType keyType; //C = Character, N = Numerical, B = Byte
        [MarshalAs(UnmanagedType.ByValTStr /*Array*/, SizeConst = 10)]
        public string /*byte[]*/ reserved2;

        /// <summary>
        /// Extracts a MDX4TagTableHeader from a Stream 
        /// </summary>
        /// <param name="Reader">The BinaryReader that contains the MDX4TTH structure</param>
        /// <param name="LengthOfTags">The total length of a single MDX4TTH structrue + overhead</param>
        /// <param name="StreamPosition">The start position in the stream</param>
        /// <returns>An MDX4TagTableHeader structure or null (if the operation failed)</returns>
        public static MDX4TagTableHeader Read(BinaryReader Reader, byte LengthOfTags, int StreamPosition)
        {
            GCHandle handle;
            byte[] buffer;
            Reader.BaseStream.Position = StreamPosition;
            buffer = Reader.ReadBytes(LengthOfTags);
            handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            MDX4TagTableHeader TempObject = (MDX4TagTableHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(MDX4TagTableHeader));
            handle.Free();
            return TempObject;
        }
    }
}
