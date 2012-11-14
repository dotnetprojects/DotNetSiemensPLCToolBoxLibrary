/*
 * Code from http://www.codeproject.com/KB/bugs/LoadDBF.aspx
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.DBF.Enums;
using DotNetSiemensPLCToolBoxLibrary.DBF.Index.MDX;
using DotNetSiemensPLCToolBoxLibrary.DBF.Structures;
using DotNetSiemensPLCToolBoxLibrary.DBF.Structures.DBT;
using DotNetSiemensPLCToolBoxLibrary.General;

namespace DotNetSiemensPLCToolBoxLibrary.DBF
{
    // Read an entire standard DBF file into a DataTable
    public class ParseDBF{

        #region DBF-Read-Funtions
        // Read an entire standard DBF file into a DataTable
        public static DataTable ReadDBF(string dbfFile)
        {
            return ReadDBF(dbfFile, null, '/');
        }

        public static DataTable ReadDBF(string dbfFile, ZipHelper _ziphelper, char DirSeperator)
        {
            long start = DateTime.Now.Ticks;
            DataTable dt = new DataTable();
            BinaryReader recReader;
            DataRow row;
            int fieldIndex;

            // If there isn't even a file, just return an empty DataTable
            if ((false == _ziphelper.FileExists(dbfFile)))
            {
                return dt;
            }

            BinaryReader br = null;

            openMemoFile(dbfFile, _ziphelper, DirSeperator);

            readMDXFile(dbfFile, _ziphelper, DirSeperator);
            //Dictionary<int, byte[]> memoLookup = ReadDBT(dbfFile);

            try
            {
                // Read the header into a buffer
                //br = new BinaryReader(new FileStream(dbfFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

                Stream tmpStream = _ziphelper.GetReadStream(dbfFile);
                br = new BinaryReader(tmpStream);
                byte[] completeBuffer = br.ReadBytes((int)_ziphelper.GetStreamLength(dbfFile, tmpStream));
                tmpStream.Close();
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
                    byte[] NumberByteArray = recReader.ReadBytes(field.fieldLen);
                    switch (field.fieldType)
                    {
                        case dBaseType.N:
                            if (dBaseConverter.N_IsDecimal(NumberByteArray)){
                                col = new DataColumn(field.fieldName, typeof(decimal));
                            }else{
                                col = new DataColumn(field.fieldName, typeof(int));
                            }
                            break;
                        case dBaseType.C:
                            col = new DataColumn(field.fieldName, typeof(string));
                            break;
                        case dBaseType.T:
                            col = new DataColumn(field.fieldName, typeof(DateTime));
                            break;
                        case dBaseType.D:
                            col = new DataColumn(field.fieldName, typeof(DateTime));
                            break;
                        case dBaseType.L:
                            col = new DataColumn(field.fieldName, typeof(bool));
                            break;
                        case dBaseType.F:
                            col = new DataColumn(field.fieldName, typeof(Double));
                            break;
                        case dBaseType.M:
                            //Field Type Memo...
                            col = new DataColumn(field.fieldName, typeof(byte[]));
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
                            case dBaseType.N:  // Number
                                byte[] NumberBytes = recReader.ReadBytes(field.fieldLen);
                                if (dBaseConverter.N_IsDecimal(NumberBytes)) {
                                    row[fieldIndex + 1] = dBaseConverter.N_ToDecimal(NumberBytes);
                                } else {
                                    row[fieldIndex + 1] = dBaseConverter.N_ToInt(NumberBytes);
                                }
                                break;

                            case dBaseType.C: // String
							    row[fieldIndex + 1] = dBaseConverter.C_ToString( recReader.ReadBytes(field.fieldLen));
								break;

                            case dBaseType.M: // Memo
                                row[fieldIndex + 1] = ReadMemoBlock(dBaseConverter.N_ToInt(recReader.ReadBytes(field.fieldLen)));
                                break;

                            case dBaseType.D: // Date (YYYYMMDD)
                                DateTime DTFromFile = dBaseConverter.D_ToDateTime(recReader.ReadBytes(8));
                                if (DTFromFile == DateTime.MinValue) {
                                    row[fieldIndex + 1] = System.DBNull.Value;
                                } else {
                                    row[fieldIndex] = DTFromFile;
                                }
                                break;

                            case dBaseType.T:
                                row[fieldIndex + 1] = dBaseConverter.T_ToDateTime(recReader.ReadBytes(8));
                                break;

                            case dBaseType.L: // Boolean (Y/N)
                                row[fieldIndex + 1] = dBaseConverter.L_ToBool(recReader.ReadByte());
                                break;

                            case dBaseType.F:
                                row[fieldIndex + 1] = dBaseConverter.F_ToDouble(recReader.ReadBytes(field.fieldLen));
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

        private static void updateMDXFile(string dbfFile){

        }

        private static void readMDXFile(string dbfFile, ZipHelper _ziphelper, char DirSeperator)
        {
            string mdxFile = Path.GetDirectoryName(dbfFile) + DirSeperator + Path.GetFileNameWithoutExtension(dbfFile) + ".mdx";


            //TEST MDXFile
            MDXFile mdxFileObject = new MDXFile(dbfFile, _ziphelper, DirSeperator, true);

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
        public static bool WriteValue(string dbfFile, string column, int row, object value, ZipHelper _ziphelper, char DirSeperator)
        {
            //if (zipfile != null)
            //    throw new Exception("Write to Zipped Files is not supported!");

            int BytesToRecordStart = 0;
            long start = DateTime.Now.Ticks;
            
            // If there isn't even a file, just return an empty DataTable
            if ((false == _ziphelper.FileExists(dbfFile)))
            {
                return false;
            }

            BinaryReader br = null;
            BinaryWriter bw = null;

            try
            {
                // Read the header into a buffer
                Stream tmpStream = _ziphelper.GetReadStream(dbfFile);
                br = new BinaryReader(tmpStream);
                byte[] completeBuffer = br.ReadBytes((int)_ziphelper.GetStreamLength(dbfFile, tmpStream));
                tmpStream.Close();
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

                Stream strm = _ziphelper.GetWriteStream(dbfFile);
                bw = new BinaryWriter(strm);

                if (column != "DELETED_FLAG")
                    BytesToRecordStart++;
                else
                    BytesToRecordStart = 0;

                (/*(FileStream)*/ bw.BaseStream).Seek(header.headerLen + row*header.recordLen + BytesToRecordStart, SeekOrigin.Begin);

                if (column == "DELETED_FLAG")
                    if ((bool)value == true)
                        bw.Write(Encoding.ASCII.GetBytes("*"));
                    else
                        bw.Write(Encoding.ASCII.GetBytes(" "));
                else
                {

                    /*
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
                    }*/


                    switch (writeFieldType)
                    {
                        case 'N':
                            bw.Write(Encoding.ASCII.GetBytes(value.ToString().PadLeft(writeFieldLength, ' ')));
                            break;
                        case 'C':
                            bw.Write(Encoding.ASCII.GetBytes(value.ToString().PadRight(writeFieldLength, ' ')));
                            break;
                        default:
                            //br.Close();
                            return false;
                            break;
                    }
                }

                _ziphelper.WriteBackStream(dbfFile, strm);

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
        private static void openMemoFile(string dbfFile, ZipHelper _ziphelper, char DirSeperator)
        {
            string dbtFile = Path.GetDirectoryName(dbfFile) + DirSeperator + Path.GetFileNameWithoutExtension(dbfFile) + ".dbt";

            if (_ziphelper.FileExists(dbtFile))
            {
                dbtReader = null;
                try
                {
                    //dbtReader = new BinaryReader(new FileStream(dbtFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                    //dbtReader=new BinaryReader(ZipHelper.GetReadStream(zipfile, dbtFile));

                    Stream tmpStream = _ziphelper.GetReadStream(dbtFile);
                    dbtReader = new BinaryReader(tmpStream);
                    byte[] completeBuffer = dbtReader.ReadBytes((int)_ziphelper.GetStreamLength(dbtFile, tmpStream));
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
        private static byte[] ReadMemoBlock(int recordnumber){
            if (recordnumber == 0 || dbtReader == null)
                return new byte[0];

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
    }

    
}