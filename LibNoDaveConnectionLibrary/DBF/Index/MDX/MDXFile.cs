using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.DBF.Structures.MDX;
using DotNetSiemensPLCToolBoxLibrary.General;

namespace DotNetSiemensPLCToolBoxLibrary.DBF.Index.MDX {

    /// <summary>
    /// Represents a complete MDX file, containing all sub indexes, headers and read / write methods
    /// </summary>
    public class MDXFile : IDisposable
    {
        private string strName;
        private bool boolReadOnly = true;
        private MDXHeader objHeader;
        private List<MDX> objMDXEntrys;
        //File Access Objects
        private Stream objFileStream = null;
        //Const
        private const short MAX_TAGS = 47;

        public MDXFile(string dbfFile, ZipHelper _ziphelper, char DirSeperator, bool ReadOnly)
        {
            this.boolReadOnly = ReadOnly;
            this.objMDXEntrys = new List<MDX>(MAX_TAGS);
            short i;

            this.strName = Path.GetDirectoryName(dbfFile) + DirSeperator + Path.GetFileNameWithoutExtension(dbfFile) + ".mdx";

            if (_ziphelper.FileExists(this.strName))
            {

                BinaryReader mdxReader = null;
                try
                {
                    byte[] buffer;
                    GCHandle handle;

                    this.objFileStream = _ziphelper.GetReadStream(this.strName);

                    //Create a Binary Reader for the MDX file
                    mdxReader = new BinaryReader(objFileStream);
                    byte[] completeBuffer = mdxReader.ReadBytes((int) _ziphelper.GetStreamLength(this.strName, objFileStream));
                    mdxReader.Close();
                    mdxReader = new BinaryReader(new MemoryStream(completeBuffer), ASCIIEncoding.ASCII);


                    // Marshall the header into a MDXHeader structure
                    buffer = mdxReader.ReadBytes(Marshal.SizeOf(typeof (MDXHeader)));
                    handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    this.objHeader = (MDXHeader) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof (MDXHeader));
                    handle.Free();


                    //Read the Key Nodes
                    for (i = 0; i < objHeader.numberOfTagsInUse; i++)
                    {
                        int StreamStartPosition = Marshal.SizeOf(typeof (MDXHeader));
                        StreamStartPosition += (this.objHeader.lengthOfTag*i);
                        if (i < objHeader.numberOfTagsInUse)
                        {
                            MDX newMDX = new MDX(i);
                            this.objMDXEntrys.Add(newMDX);
                            newMDX.Read(mdxReader, this.objHeader.lengthOfTag, StreamStartPosition);
                        }
                        else
                        {
                            objMDXEntrys.Add(null);
                        }
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                finally
                {
                    mdxReader.Close();
                }
            }
        }

        #region Dispose

        /// <summary>
        /// Destructor
        /// </summary>
        ~MDXFile()
        {
            this.Dispose();
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        public void Dispose()
        {
            try
            {
                this.objFileStream.Close();
            }
            catch (Exception e)
            {

            }
        }

        #endregion
    }
}
