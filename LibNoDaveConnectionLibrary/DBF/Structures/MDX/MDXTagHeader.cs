using System;
using System.IO;
using System.Runtime.InteropServices;
using DotNetSiemensPLCToolBoxLibrary.DBF.Enums;

namespace DotNetSiemensPLCToolBoxLibrary.DBF.Structures.MDX
{
    /// <summary>
    /// A MDX tag header
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    internal struct MDXTagHeader //This is the same, like in the normal NDX File??
    {
        public Int32 pointerToRootPage;
        public Int32 numPages;
        public TagHeaderKeyFormat keyFormat;
        public dBaseType keyType;
        public Int16 reserved1;
        public Int16 indexKeyLength;
        public Int16 maxNumberOfKeysPage;
        public Int16 secondaryKeyType; //00h: DB4: C/N; DB3: C
        //01h: DB4: D  ; DB3: N/D
        public Int16 indeyKeyItemLength;
        public Int16 version;
        public byte reserved3;
        public byte uniqueFlag;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)] public string KeyString1; // 24..

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 233)] // 512 - 255 - 24
        public string KeyString2;

        /// <summary>
        /// Extracts a MDXTagHeader from a Stream 
        /// </summary>
        /// <param name="Reader">The BinaryReader that contains the MDXTH structure</param>
        /// <param name="LengthOfTags">The total length of a single MDXTH structrue + overhead</param>
        /// <param name="StreamPosition">The start position in the stream</param>
        /// <returns>An MDXTagHeader structure or null (if the operation failed)</returns>
        public static MDXTagHeader Read(BinaryReader Reader, int StreamPosition)
        {
            GCHandle handle;
            byte[] buffer;

            Reader.BaseStream.Position = StreamPosition;
            buffer = Reader.ReadBytes(Marshal.SizeOf(typeof (MDXTagHeader)));
            handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            MDXTagHeader TempObject = (MDXTagHeader) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof (MDXTagHeader));
            handle.Free();
            return TempObject;
        }

        /// <summary>
        /// Write this MDXTagHeader to the current position of a Stream
        /// </summary>
        /// <param name="Writer">A BinaryWriter to write the MDXTH to</param>
        public void Write(BinaryWriter Writer)
        {
            GCHandle handle;
            byte[] buffer = new byte[Marshal.SizeOf(typeof (MDXTagHeader))];
            handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(this, handle.AddrOfPinnedObject(), true);
            Writer.Write(buffer);
            handle.Free();
        }
    }
}