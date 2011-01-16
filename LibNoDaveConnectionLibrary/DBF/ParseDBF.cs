using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.General;

namespace DotNetSiemensPLCToolBoxLibrary.DBF
{
    // Read an entire standard DBF file into a DataTable
    public class ParseDBF
    {
        #region DBF-Types
        // This is the file header for a DBF. We do this special layout with everything
        // packed so we can read straight from disk into the structure to populate it
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        private struct DBFHeader
        {
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

        // This is the field descriptor structure. There will be one of these for each column in the table.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        private struct FieldDescriptor
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
            public string fieldName;
            public byte fieldType;  //char
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
        }
        #endregion

        #region DBT-Types
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        private struct DBTHeader
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

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]        
        private struct MemoHeader
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] reserved;
            public Int16 startPosition;
            public Int32 fieldLength;
        }
        #endregion

        #region MDX-Types
        // This is the file header for a MDX. We do this special layout with everything
        // packed so we can read straight from disk into the structure to populate it
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        private struct MDXHeader
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

        // This is the tag table header for a MDX. We do this special layout with everything
        // packed so we can read straight from disk into the structure to populate it
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        private struct MDX4TagTableHeader
        {
            public Int32 tagHeaderPageNumber;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
            public string tagName;            
            public byte keyFormat; //0x00 = Calculated, 0x10 = Data Field
            public byte forwardTagThreadLeft;
            public byte forwardTagThreadRight;
            public byte backwardTagThread;            
            public byte reserved1;
            public byte keyType; //C = Character, N = Numerical, B = Byte
            [MarshalAs(UnmanagedType.ByValTStr /*Array*/, SizeConst = 10)]
            public string /*byte[]*/ reserved2;            
        }

        // This is the tag table header for a MDX. We do this special layout with everything
        // packed so we can read straight from disk into the structure to populate it
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        private struct MDX7TagTableHeader
        {
            public Int32 tagHeaderPageNumber;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
            public string tagName;
            public byte keyFormat; //0x00 = Calculated, 0x10 = Data Field
            public byte forwardTagThreadLeft;
            public byte forwardTagThreadRight;
            public byte backwardTagThread;
            public byte reserved1;
            public byte keyType;  //C = Character, N = Numerical, B = Byte
            [MarshalAs(UnmanagedType.ByValTStr /*Array*/, SizeConst = 10)]
            public string /*byte[]*/ reserved2;
        } 

        // This is the tag header for a MDX. We do this special layout with everything
        // packed so we can read straight from disk into the structure to populate it
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        private struct MDXTagHeader //This is the same, like in the normal NDX File??
        {
            public Int32 pointerToRootPage;
            public Int32 numPages;
            public byte keyFormat;  //00h: Right, Left, DTOC //08h: Descending order //10h: String //20h: Distinct //40h: Unique
            public byte keyType;
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
        }

        #endregion

        #region DBF-Read-Funtions
        // Read an entire standard DBF file into a DataTable
        public static DataTable ReadDBF(string dbfFile)
        {
            return ReadDBF(dbfFile, null, '/');
        }

        public static DataTable ReadDBF(string dbfFile, object zipfile, char DirSeperator)
        {
            long start = DateTime.Now.Ticks;
            DataTable dt = new DataTable();
            BinaryReader recReader;
            string number;
            string year;
            string month;
            string day;
            long lDate;
            long lTime;
            DataRow row;
            int fieldIndex;

            // If there isn't even a file, just return an empty DataTable
            if ((false == ZipHelper.FileExists(zipfile,dbfFile)))
            {
                return dt;
            }

            BinaryReader br = null;

            openMemoFile(dbfFile, zipfile, DirSeperator);

            readMDXFile(dbfFile, zipfile, DirSeperator);
            //Dictionary<int, byte[]> memoLookup = ReadDBT(dbfFile);

            try
            {
                // Read the header into a buffer
                //br = new BinaryReader(new FileStream(dbfFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

                Stream tmpStream = ZipHelper.GetReadStream(zipfile, dbfFile);
                br = new BinaryReader(tmpStream);
                byte[] completeBuffer = br.ReadBytes((int)ZipHelper.GetStreamLength(zipfile, dbfFile, tmpStream));
                br.Close();
                br = new BinaryReader(new MemoryStream(completeBuffer));

                byte[] buffer = br.ReadBytes(Marshal.SizeOf(typeof(DBFHeader)));
                

                // Marshall the header into a DBFHeader structure
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                DBFHeader header = (DBFHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DBFHeader));
                handle.Free();

                // Read in all the field descriptors. Per the spec, 13 (0D) marks the end of the field descriptors
                ArrayList fields = new ArrayList();
                
                while ((13 != br.PeekChar()))
                {
                    buffer = br.ReadBytes(Marshal.SizeOf(typeof(FieldDescriptor)));
                    handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    fields.Add((FieldDescriptor)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(FieldDescriptor)));
                    handle.Free();
                }

                // Read in the first row of records, we need this to help determine column types below
                (br.BaseStream).Seek(header.headerLen + 1, SeekOrigin.Begin);
                buffer = br.ReadBytes(header.recordLen);
                recReader = new BinaryReader(new MemoryStream(buffer));

                // Create the columns in our new DataTable
                DataColumn col = null;

                dt.Columns.Add(new DataColumn("DELETED_FLAG", typeof(bool)));

                foreach (FieldDescriptor field in fields)
                {
                    number = Encoding.ASCII.GetString(recReader.ReadBytes(field.fieldLen));
                    switch (field.fieldType)
                    {
                        case (byte)'N':
                            if (number.IndexOf(".") > -1)
                            {
                                col = new DataColumn(field.fieldName, typeof(decimal));
                            }
                            else
                            {
                                col = new DataColumn(field.fieldName, typeof(int));
                            }
                            break;
                        case (byte)'C':
                            col = new DataColumn(field.fieldName, typeof(string));
                            break;
                        case (byte)'T':
                            // You can uncomment this to see the time component in the grid
                            //col = new DataColumn(field.fieldName, typeof(string));
                            col = new DataColumn(field.fieldName, typeof(DateTime));
                            break;
                        case (byte)'D':
                            col = new DataColumn(field.fieldName, typeof(DateTime));
                            break;
                        case (byte)'L':
                            col = new DataColumn(field.fieldName, typeof(bool));
                            break;
                        case (byte)'F':
                            col = new DataColumn(field.fieldName, typeof(Double));
                            break;
                        case (byte)'M':
                            //Field Type Memo...
                            col = new DataColumn(field.fieldName, typeof(byte[]));
                            //col = new DataColumn(field.fieldName, typeof(string));
                            break;
                    }
                    dt.Columns.Add(col);
                }

                // Skip past the end of the header. 
                (br.BaseStream).Seek(header.headerLen, SeekOrigin.Begin);

                // Read in all the records
                for (int counter = 0; counter <= header.numRecords - 1; counter++)
                {
                    // First we'll read the entire record into a buffer and then read each field from the buffer
                    // This helps account for any extra space at the end of each record and probably performs better
                    buffer = br.ReadBytes(header.recordLen);
                    recReader = new BinaryReader(new MemoryStream(buffer));

                    // All dbf field records begin with a deleted flag field. Deleted - 0x2A (asterisk) else 0x20 (space)
                    //if (recReader.ReadChar() == '*')
                    //{
                    //	continue;
                    //}


                    // Loop through each field in a record
                    fieldIndex = 0;
                    row = dt.NewRow();

                    char delflg = recReader.ReadChar();
                    if (delflg == '*')
                        row[0] = true;
                    else
                        row[0] = false;


                    foreach (FieldDescriptor field in fields)
                    {
                        switch (field.fieldType)
                        {
                            case (byte)'N':  // Number
                                // If you port this to .NET 2.0, use the Decimal.TryParse method
                                number = Encoding.ASCII.GetString(recReader.ReadBytes(field.fieldLen));
                                if (IsNumber(number))
                                {
                                    if (number.IndexOf(".") > -1)
                                    {
                                        row[fieldIndex + 1] = decimal.Parse(number);
                                    }
                                    else
                                    {
                                        row[fieldIndex + 1] = int.Parse(number);
                                    }
                                }
                                else
                                {
                                    row[fieldIndex + 1] = 0;
                                }

                                break;

                            case (byte)'C': // String
                                {
                                    row[fieldIndex + 1] = Encoding.UTF7.GetString(recReader.ReadBytes(field.fieldLen)).TrimEnd(new char[] {' '});
                                    break;
                                }
                            case (byte)'M': // Memo
                                {
                                    int intRef;
                                    //if ((int)row[1] == 89 && (string)row[3] == "00014")
                                    //    intRef = 0;
                                    
                                    String strRef = Encoding.ASCII.GetString(recReader.ReadBytes(field.fieldLen)).Trim();
                                    if (Int32.TryParse(strRef, out intRef))
                                    {
                                        //if (memoLookup.ContainsKey(intRef))
                                        //    row[fieldIndex + 1] = memoLookup[intRef];
                                        //else
                                        //    row[fieldIndex + 1] = new byte[0];
                                        row[fieldIndex + 1] = ReadMemoBlock(intRef);
                                    }
                                    else
                                        row[fieldIndex + 1] = new byte[0];
                                    break;
                                }


                            case (byte)'D': // Date (YYYYMMDD)
                                year = Encoding.ASCII.GetString(recReader.ReadBytes(4));
                                month = Encoding.ASCII.GetString(recReader.ReadBytes(2));
                                day = Encoding.ASCII.GetString(recReader.ReadBytes(2));
                                row[fieldIndex + 1] = System.DBNull.Value;
                                try
                                {
                                    if (IsNumber(year) && IsNumber(month) && IsNumber(day))
                                    {
                                        if ((Int32.Parse(year) > 1900))
                                        {
                                            row[fieldIndex] = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
                                        }
                                    }
                                }
                                catch
                                { }

                                break;

                            case (byte)'T': // Timestamp, 8 bytes - two integers, first for date, second for time
                                // Date is the number of days since 01/01/4713 BC (Julian Days)
                                // Time is hours * 3600000L + minutes * 60000L + Seconds * 1000L (Milliseconds since midnight)
                                lDate = recReader.ReadInt32();
                                lTime = recReader.ReadInt32() * 10000L;
                                row[fieldIndex + 1] = JulianToDateTime(lDate).AddTicks(lTime);
                                break;

                            case (byte)'L': // Boolean (Y/N)
                                if ('Y' == recReader.ReadByte())
                                {
                                    row[fieldIndex + 1] = true;
                                }
                                else
                                {
                                    row[fieldIndex + 1] = false;
                                }

                                break;

                            case (byte)'F':
                                number = Encoding.ASCII.GetString(recReader.ReadBytes(field.fieldLen));
                                if (IsNumber(number))
                                {
                                    row[fieldIndex + 1] = double.Parse(number);
                                }
                                else
                                {
                                    row[fieldIndex + 1] = 0.0F;
                                }
                                break;
                        }
                        fieldIndex++;
                    }

                    recReader.Close();
                    dt.Rows.Add(row);
                }
            }

            catch
            {
                throw;
            }
            finally
            {
                if (null != br)
                {
                    br.Close();
                }

                if (dbtReader != null)
                {
                    dbtReader.Close();
                    dbtReader = null;
                }
            }

            long count = DateTime.Now.Ticks - start;

            return dt;
        }
        #endregion


        //Update to the DBF Methods:
        //Create a Class DBFFile
        //this class contains a Datatable
        //a option that changes to the Datatable a directly written to disk
        //a list of indexes
        //a list of mdx indexes (this list can only contain 48 indexs or so)
        //function addindex
        //this list can only contain

        #region MDX-Functions
        private static void updateMDXFile(string dbfFile)
        {

        }



        private static void readMDXFile(string dbfFile, object zipfile, char DirSeperator)
        {
            string mdxFile =
                Path.GetDirectoryName(dbfFile) + DirSeperator + Path.GetFileNameWithoutExtension(dbfFile) + ".mdx";

            if (ZipHelper.FileExists(zipfile, mdxFile))
            {
                BinaryReader mdxReader = null;
                try
                {
                    byte[] buffer;
                    GCHandle handle;
                    //mdxReader = new BinaryReader(new FileStream(mdxFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                    //mdxReader = new BinaryReader(ZipHelper.GetReadStream(zipfile, mdxFile));
                    
                    Stream tmpStream = ZipHelper.GetReadStream(zipfile, mdxFile);
                    mdxReader = new BinaryReader(tmpStream);
                    byte[] completeBuffer = mdxReader.ReadBytes((int)ZipHelper.GetStreamLength(zipfile, mdxFile, tmpStream));
                    mdxReader.Close();
                    mdxReader = new BinaryReader(new MemoryStream(completeBuffer));

                    // Marshall the header into a MDXHeader structure
                    buffer = mdxReader.ReadBytes(Marshal.SizeOf(typeof(MDXHeader)));
                    handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    MDXHeader header = (MDXHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(MDXHeader));
                    handle.Free();

                    //mdxReader.BaseStream.Position = 1024;

                    List<MDX4TagTableHeader> tagtableheaders = new List<MDX4TagTableHeader>(header.numberOfEntrysInTag);
                    for (int n = 0; n < header.numberOfTagsInUse ; n++)
                    {
                        // Marshall the header into a MDXHeader structure
                        buffer = mdxReader.ReadBytes(header.lengthOfTag);                        
                        handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                        MDX4TagTableHeader tagtableheader = (MDX4TagTableHeader) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof (MDX4TagTableHeader));                        
                        handle.Free();

                        tagtableheaders.Add(tagtableheader);
                    }

                    List<MDXTagHeader> tagheaders = new List<MDXTagHeader>(header.numberOfEntrysInTag);
                    for (int n = 0; n < header.numberOfTagsInUse ; n++)
                    {
                        // Marshall the header into a MDXHeader structure
                        mdxReader.BaseStream.Position = tagtableheaders[n].tagHeaderPageNumber*0x200;
                        buffer = mdxReader.ReadBytes(Marshal.SizeOf(typeof(MDXTagHeader)));
                        handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                        MDXTagHeader tagheader = (MDXTagHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(MDXTagHeader));
                        handle.Free();

                        tagheaders.Add(tagheader);
                    }



                    //And now, read the Values from the rows...

                    //First value is the number of entrys,
                    //indexkeylength, is the length of an entry, but + so many that it can be divided by 4!
                    
                }
                finally
                {
                    mdxReader.Close();
                }
            }
        }

        //This function should wite a MDX file with the specified indexes.!
       /* private static void writeMDXFile(string dbfFile, indexes)
        {

        }*/
        #endregion


        #region DBF-Write-Functions
        /// <summary>
        /// This Function Writes directly to a DBF File.
        /// It reads the Field list, and writes to the correct position.
        /// To access the deleted flag, use DELETED_FLAG as column Name
        /// </summary>
        /// <param name="dbfFile"></param>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool WriteValue(string dbfFile, string column, int row, object value, object zipfile)
        {
            if (zipfile != null)
                throw new Exception("Write to Zipped Files is not supported!");

            int BytesToRecordStart = 0;
            long start = DateTime.Now.Ticks;
            
            // If there isn't even a file, just return an empty DataTable
            if ((false == File.Exists(dbfFile)))
            {
                return false;
            }

            BinaryReader br = null;
            BinaryWriter bw = null;

            try
            {
                // Read the header into a buffer
                br = new BinaryReader(new FileStream(dbfFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                byte[] buffer = br.ReadBytes(Marshal.SizeOf(typeof (DBFHeader)));

                // Marshall the header into a DBFHeader structure
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                DBFHeader header = (DBFHeader) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof (DBFHeader));
                handle.Free();

                // Read in all the field descriptors. Per the spec, 13 (0D) marks the end of the field descriptors
                ArrayList fields = new ArrayList();
                while ((13 != br.PeekChar()))
                {
                    buffer = br.ReadBytes(Marshal.SizeOf(typeof (FieldDescriptor)));
                    handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    fields.Add((FieldDescriptor) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof (FieldDescriptor)));
                    handle.Free();
                }                


                char writeFieldType = ' ';
                int writeFieldLength = 0;
                foreach (FieldDescriptor field in fields)
                {
                    writeFieldType = (char)field.fieldType;
                    writeFieldLength = field.fieldLen;
                    if (field.fieldName == column)
                        break;
                    BytesToRecordStart += field.fieldLen;   
                }

                br.Close();
                
                bw = new BinaryWriter(File.OpenWrite(dbfFile));

                if (column != "DELETED_FLAG")
                    BytesToRecordStart++;
                else
                    BytesToRecordStart = 0;

                ((FileStream) bw.BaseStream).Seek(header.headerLen + row*header.recordLen + BytesToRecordStart, SeekOrigin.Begin);

                if (column == "DELETED_FLAG")
                    if ((bool)value == true)
                        bw.Write(Encoding.ASCII.GetBytes("*"));
                    else
                        bw.Write(Encoding.ASCII.GetBytes(" "));
                else
                {
                    switch (writeFieldType)
                    {
                        case 'N':
                            bw.Write(Encoding.ASCII.GetBytes(value.ToString().PadLeft(writeFieldLength, ' ')));
                            break;
                        case 'C':
                            bw.Write(Encoding.ASCII.GetBytes(value.ToString().PadRight(writeFieldLength, ' ')));
                            break;
                        default:
                            br.Close();
                            return false;
                            break;
                    }
                }
                bw.Close();                
            }
            finally
            {
                if (br != null)
                    br.Close();
                if (bw != null)
                    bw.Close();
            }

            return true;
        }

        public static bool Write(string dbfFile, DataTable dt, char DirSeperator)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region DBT (Memo) Functions
        private static int memoBlockLength = 512;
        private static BinaryReader dbtReader = null;
        private static void openMemoFile(string dbfFile, object zipfile, char DirSeperator)
        {
            string dbtFile = Path.GetDirectoryName(dbfFile) + DirSeperator + Path.GetFileNameWithoutExtension(dbfFile) + ".dbt";

            if (ZipHelper.FileExists(zipfile, dbtFile))
            {
                dbtReader = null;
                try
                {
                    //dbtReader = new BinaryReader(new FileStream(dbtFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                    //dbtReader=new BinaryReader(ZipHelper.GetReadStream(zipfile, dbtFile));

                    Stream tmpStream = ZipHelper.GetReadStream(zipfile, dbtFile);
                    dbtReader = new BinaryReader(tmpStream);
                    byte[] completeBuffer = dbtReader.ReadBytes((int)ZipHelper.GetStreamLength(zipfile, dbtFile, tmpStream));
                    dbtReader.Close();
                    dbtReader = new BinaryReader(new MemoryStream(completeBuffer));

                    // Read the header into a buffer
                    byte[] buffer = dbtReader.ReadBytes(Marshal.SizeOf(typeof(DBTHeader)));

                    // Marshall the header into a DBTHeader structure
                    GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    DBTHeader header = (DBTHeader) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof (DBTHeader));
                    handle.Free();

                    memoBlockLength = header.blockLength;
                }
                catch(Exception)
                {
                }
            }
        }
        private static byte[] ReadMemoBlock(int recordnumber)
        {
            if (recordnumber == 0 || dbtReader == null)
                return null;

            // Position reader at beginning of current block
            dbtReader.BaseStream.Position = memoBlockLength * recordnumber;

            // Read the memo field header into a buffer
            byte[] buffer = dbtReader.ReadBytes(Marshal.SizeOf(typeof(MemoHeader)));

            // Marshall the header into a MemoHeader structure
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            MemoHeader memHeader = (MemoHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(MemoHeader));
            handle.Free();

            int bytesToRead = memHeader.fieldLength - memHeader.startPosition;

            return dbtReader.ReadBytes(bytesToRead);
        }
        #endregion

        #region Helper Functions
        /// <summary>
        /// Simple function to test is a string can be parsed. There may be a better way, but this works
        /// If you port this to .NET 2.0, use the new TryParse methods instead of this        
        /// </summary>
        /// <param name="number">string to test for parsing</param>
        /// <returns>true if string can be parsed</returns>
        public static bool IsNumber(string numberString)
        {
            char[] numbers = numberString.ToCharArray();
            int number_count = 0;
            int point_count = 0;
            int space_count = 0;

            foreach (char number in numbers)
            {
                if ((number >= 48 && number <= 57))
                {
                    number_count += 1;
                }
                else if (number == 46)
                {
                    point_count += 1;
                }
                else if (number == 32)
                {
                    space_count += 1;
                }
                else
                {
                    return false;
                }
            }

            return (number_count > 0 && point_count < 2);
        }

        /// <summary>
        /// Convert a Julian Date to a .NET DateTime structure
        /// Implemented from pseudo code at http://en.wikipedia.org/wiki/Julian_day
        /// </summary>
        /// <param name="lJDN">Julian Date to convert (days since 01/01/4713 BC)</param>
        /// <returns>DateTime</returns>
        private static DateTime JulianToDateTime(long lJDN)
        {
            double p = Convert.ToDouble(lJDN);
            double s1 = p + 68569;
            double n = Math.Floor(4 * s1 / 146097);
            double s2 = s1 - Math.Floor((146097 * n + 3) / 4);
            double i = Math.Floor(4000 * (s2 + 1) / 1461001);
            double s3 = s2 - Math.Floor(1461 * i / 4) + 31;
            double q = Math.Floor(80 * s3 / 2447);
            double d = s3 - Math.Floor(2447 * q / 80);
            double s4 = Math.Floor(q / 11);
            double m = q + 2 - 12 * s4;
            double j = 100 * (n - 49) + i + s4;
            return new DateTime(Convert.ToInt32(j), Convert.ToInt32(m), Convert.ToInt32(d));
        }
        #endregion
    }
}