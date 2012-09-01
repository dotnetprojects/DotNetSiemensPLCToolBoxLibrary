using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

// We do this special layout with everything
// packed so we can read straight from disk into the structure to populate it

namespace DotNetSiemensPLCToolBoxLibrary.DBF {
    #region DBF-Types
    
    /// <summary>
    /// The header of a DBF file
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    internal struct DBFHeader {
        public byte version;
        public byte updateYear;
        public byte updateMonth;
        public byte updateDay;
        public Int32 numRecords;
        public Int16 headerLen;
        public Int16 recordLen;
        public Int16 reserved1;
        public byte incompleteTrans;
        public byte encryptionFlag;
        public Int32 reserved2;
        public Int64 reserved3;
        public byte MDX;
        public byte language;
        public Int16 reserved4;
    }

    /// <summary>
    /// The DBF field descriptor structure. There will be one of these for each column in the table.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    internal struct FieldDescriptor {
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

        public override string ToString() {
            return "Field-Name: " + fieldName;
        }
    }
    #endregion

    #region DBT-Types

    /// <summary>
    /// The header of a DBT file
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    internal struct DBTHeader {
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

    /// <summary>
    /// The header of a memo
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    internal struct MemoHeader {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] reserved;
        public Int16 startPosition;
        public Int32 fieldLength;
    }
    #endregion

    #region MDX-Types

    /// <summary>
    /// A MDX file header
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    internal struct MDXHeader {
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

    /// <summary>
    /// A MDX4 Tag Table Header
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    internal struct MDX4TagTableHeader {
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
        public static MDX4TagTableHeader Read(BinaryReader Reader, byte LengthOfTags, int StreamPosition) {
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

    /// <summary>
    /// A MDX7 Tag Table Header
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    internal struct MDX7TagTableHeader {
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
        public static MDX7TagTableHeader Read(BinaryReader Reader, byte LengthOfTags, int StreamPosition) {
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
        public Int16 secondaryKeyType;  //00h: DB4: C/N; DB3: C
        //01h: DB4: D  ; DB3: N/D
        public Int16 indeyKeyItemLength;
        public Int16 version;
        public byte reserved3;
        public byte uniqueFlag;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string KeyString1; // 24..
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 233)]// 512 - 255 - 24
        public string KeyString2;

        /// <summary>
        /// Extracts a MDXTagHeader from a Stream 
        /// </summary>
        /// <param name="Reader">The BinaryReader that contains the MDXTH structure</param>
        /// <param name="LengthOfTags">The total length of a single MDXTH structrue + overhead</param>
        /// <param name="StreamPosition">The start position in the stream</param>
        /// <returns>An MDXTagHeader structure or null (if the operation failed)</returns>
        public static MDXTagHeader Read(BinaryReader Reader, int StreamPosition) {
            GCHandle handle;
            byte[] buffer;

            Reader.BaseStream.Position = StreamPosition;
            buffer = Reader.ReadBytes(Marshal.SizeOf(typeof(MDXTagHeader)));
            handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            MDXTagHeader TempObject = (MDXTagHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(MDXTagHeader));
            handle.Free();
            return TempObject;
        }

        /// <summary>
        /// Write this MDXTagHeader to the current position of a Stream
        /// </summary>
        /// <param name="Writer">A BinaryWriter to write the MDXTH to</param>
        public void Write(BinaryWriter Writer) {
            GCHandle handle;
            byte[] buffer = new byte[Marshal.SizeOf(typeof(MDXTagHeader))];
            handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(this, handle.AddrOfPinnedObject(), true);
            Writer.Write(buffer);
            handle.Free();
        }
    }
    #endregion
}
