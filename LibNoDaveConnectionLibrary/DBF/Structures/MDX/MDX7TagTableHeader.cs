using System;
using System.IO;
using System.Runtime.InteropServices;
using DotNetSiemensPLCToolBoxLibrary.DBF.Enums;

namespace DotNetSiemensPLCToolBoxLibrary.DBF.Structures.MDX
{
    /// <summary>
    /// A MDX7 Tag Table Header
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    internal struct MDX7TagTableHeader
    {
        public Int32 tagHeaderPageNumber;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string tagName;
        public TagKeyFormat keyFormat; //0x00 = Calculated, 0x10 = Data Field
        public byte forwardTagThreadLeft;
        public byte forwardTagThreadRight;
        public byte backwardTagThread;
        public byte reserved1;
        public dBaseType keyType;  //C = Character, N = Numerical, B = Byte
        [MarshalAs(UnmanagedType.ByValTStr /*Array*/, SizeConst = 10)]
        public string /*byte[]*/ reserved2;

        /// <summary>
        /// Extracts a MDX7TagTableHeader from a Stream 
        /// </summary>
        /// <param name="Reader">The BinaryReader that contains the MDX7TTH structure</param>
        /// <param name="LengthOfTags">The total length of a single MDX7TTH structrue + overhead</param>
        /// <param name="StreamPosition">The start position in the stream</param>
        /// <returns>An MDX7TagTableHeader structure or null (if the operation failed)</returns>
        public static MDX7TagTableHeader Read(BinaryReader Reader, byte LengthOfTags, int StreamPosition)
        {
            GCHandle handle;
            byte[] buffer;
            Reader.BaseStream.Position = StreamPosition;
            buffer = Reader.ReadBytes(LengthOfTags);
            handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            MDX7TagTableHeader TempObject = (MDX7TagTableHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(MDX7TagTableHeader));
            handle.Free();
            return TempObject;
        }
    }
}
