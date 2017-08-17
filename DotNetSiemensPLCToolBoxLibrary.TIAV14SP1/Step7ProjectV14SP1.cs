using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using DotNetSiemensPLCToolBoxLibrary.General;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Enums;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Structs;
using Microsoft.Win32;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles
{
    public partial class Step7ProjectV14SP1 : Project, IDisposable
    {
        public enum TiaVersionTypes
        {
            V11 = 11,
            V12 = 12,
            V13 = 13,
            V14 = 14,
        }

        public static TiaVersionTypes TiaVersion { get; private set; }

        private string DataFile = null;

        private XmlDocument tiaProject;

        internal ZipHelper _ziphelper = new ZipHelper(null);

        public CultureInfo Culture { get; set; }

        public Step7ProjectV14SP1(string projectfile, CultureInfo culture = null)
        {
            if (culture == null)
                Culture = CultureInfo.CurrentCulture;
            else
                Culture = culture;

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += currentDomain_AssemblyResolve;

            ProjectFile = projectfile;

            if (ProjectFile.ToLower().EndsWith("zip") || ProjectFile.ToLower().EndsWith("zap14"))
            {
                if (string.IsNullOrEmpty(ProjectFile))
                    ProjectFile = ZipHelper.GetFirstZipEntryWithEnding(ProjectFile, ".ap14");
                if (string.IsNullOrEmpty(ProjectFile))
                    ProjectFile = ZipHelper.GetFirstZipEntryWithEnding(ProjectFile, ".al14");

                if (string.IsNullOrEmpty(projectfile))
                    throw new Exception("Zip-File contains no valid TIA Project !");
                this._ziphelper = new ZipHelper(projectfile);
            }

           
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(_ziphelper.GetReadStream(projectfile));

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("x", "http://www.siemens.com/2007/07/Automation/CommonServices/DataInfoValueData");

                var nd = xmlDoc.SelectSingleNode("x:Data", nsmgr);
                this.ProjectName = nd.Attributes["Name"].Value;
            }
            catch (Exception) 
            { }

            DataFile = Path.GetDirectoryName(projectfile) + "\\System\\PEData.plf";
            ProjectFolder = projectfile.Substring(0, projectfile.LastIndexOf(Path.DirectorySeparatorChar)) + Path.DirectorySeparatorChar;

            //BinaryParseTIAFile();
            //LoadProject();
            LoadViaOpennessDlls();

            currentDomain.AssemblyResolve -= currentDomain_AssemblyResolve;            
        }        

        internal XmlDocument xmlDoc;
        
        Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            int index = args.Name.IndexOf(',');
            if (index == -1)
            {
                return null;
            }
            var name = args.Name.Substring(0, index) + ".dll";

            var filePathReg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Siemens\\Automation\\_InstalledSW\\TIAP14\\Global") ??
                              Registry.LocalMachine.OpenSubKey("SOFTWARE\\Siemens\\Automation\\_InstalledSW\\TIAP14\\Global");

            if (filePathReg != null)
            {
                string filePath = filePathReg.GetValue("Path").ToString() + "PublicAPI\\V14";
                if (Directory.Exists(filePath) == false)
                    filePath = filePathReg.GetValue("Path").ToString() + "PublicAPI\\V14 SP1";
                var path = Path.Combine(filePath, name);
                var fullPath = Path.GetFullPath(path);
                if (File.Exists(fullPath))
                {
                    return Assembly.LoadFrom(fullPath);
                }
            }

            return null;
        }

        private object tiaExport;
        private Type tiaExportType;
        

        internal Dictionary<TiaObjectId, TiaFileObject> TiaObjects = new Dictionary<TiaObjectId, TiaFileObject>();

        internal void BinaryParseTIAFile()
        {
            using (var sourceStream = new FileStream(DataFile, FileMode.Open, FileAccess.Read, System.IO.FileShare.ReadWrite))
            {
                var buffer = new byte[Marshal.SizeOf(typeof(TiaFileHeader))];
                sourceStream.Read(buffer, 0, buffer.Length);

                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                TiaFileHeader header = (TiaFileHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(TiaFileHeader));
                handle.Free();
                TiaMarker? lastMarker = null;

                while (sourceStream.Position < sourceStream.Length)
                {
                    if (TiaHelper.IsMarker(sourceStream))
                    {
                        var buffer2 = new byte[Marshal.SizeOf(typeof(TiaMarker))];
                        sourceStream.Read(buffer2, 0, buffer2.Length);
                        GCHandle handle2 = GCHandle.Alloc(buffer2, GCHandleType.Pinned);
                        TiaMarker marker = (TiaMarker)Marshal.PtrToStructure(handle2.AddrOfPinnedObject(), typeof(TiaMarker));
                        handle2.Free();

                        lastMarker = marker;
                    }
                    else
                    {
                        var buffer3 = new byte[Marshal.SizeOf(typeof(TiaObjectHeader))];
                        sourceStream.Read(buffer3, 0, buffer3.Length);
                        GCHandle handle3 = GCHandle.Alloc(buffer3, GCHandleType.Pinned);
                        TiaObjectHeader hd = (TiaObjectHeader)Marshal.PtrToStructure(handle3.AddrOfPinnedObject(), typeof(TiaObjectHeader));
                        handle3.Free();

                        var bytes = new byte[hd.Size - buffer3.Length];
                        sourceStream.Read(bytes, 0, bytes.Length);
                        var id = hd.GetTiaObjectId();
                        if (!TiaObjects.ContainsKey(id))
                        {
                            TiaObjects.Add(id, new TiaFileObject(hd, bytes));

                            var size = Marshal.SizeOf(typeof (TiaObjectHeader))+4+BitConverter.ToInt32(bytes, 0)+1;
                            if (hd.Size != size || bytes[bytes.Length - 1] != 0xff)
                            {
                                //Fehler ??? 
                            }


                            //var strm = new MemoryStream(bytes);
                            //var dec = TiaCompression.DecompressStream(strm);
                            //var rd = new StreamReader(dec);
                            //var wr = rd.ReadToEnd();
                        }
                        else
                        {
                            //Todo: look why this happens, and how TIA Handles this!!
                            //Console.WriteLine("double Id:" + id.ToString());
                        }
                    }
                }

                var rootId = new TiaObjectId(TiaFixedRootObjectInstanceIds.RootObjectCollectionId);
                var rootObjects = new TiaRootObjectList(TiaObjects[rootId]);
                var projectid = rootObjects.TiaRootObjectEntrys.FirstOrDefault(x => x.ObjectId.TypeId == (int)TiaTypeIds.Siemens_Automation_DomainModel_ProjectData).ObjectId;
                var projectobj = TiaObjects[projectid];
            }
        }

        
       
        
        internal override void LoadProject()
        {
            _projectLoaded = true;
        }
    }
}
