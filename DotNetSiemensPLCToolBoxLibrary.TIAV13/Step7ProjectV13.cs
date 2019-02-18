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
    public partial class Step7ProjectV13 : Project, IDisposable
    {
        private string DataFile = null;

        private XmlDocument tiaProject;

        internal ZipHelper _ziphelper = new ZipHelper(null);

        public CultureInfo Culture { get; set; }

        public Step7ProjectV13(string projectfile, CultureInfo culture = null)
        {
            if (culture == null)
                Culture = CultureInfo.CurrentCulture;
            else
                Culture = culture;

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += currentDomain_AssemblyResolve;

            ProjectFile = projectfile;

            if (ProjectFile.ToLower().EndsWith("zip") || ProjectFile.ToLower().EndsWith("zap13") || ProjectFile.ToLower().EndsWith("zap14"))
            {
                ProjectFile = ZipHelper.GetFirstZipEntryWithEnding(ProjectFile, ".ap11");
                if (string.IsNullOrEmpty(ProjectFile))               
                    ProjectFile = ZipHelper.GetFirstZipEntryWithEnding(ProjectFile, ".ap12");
                if (string.IsNullOrEmpty(ProjectFile))
                    ProjectFile = ZipHelper.GetFirstZipEntryWithEnding(ProjectFile, ".ap13");
                if (string.IsNullOrEmpty(ProjectFile))
                    ProjectFile = ZipHelper.GetFirstZipEntryWithEnding(ProjectFile, ".ap14");
                if (string.IsNullOrEmpty(ProjectFile))
                    ProjectFile = ZipHelper.GetFirstZipEntryWithEnding(ProjectFile, ".al11");
                if (string.IsNullOrEmpty(ProjectFile))
                    ProjectFile = ZipHelper.GetFirstZipEntryWithEnding(ProjectFile, ".al12");
                if (string.IsNullOrEmpty(ProjectFile))
                    ProjectFile = ZipHelper.GetFirstZipEntryWithEnding(ProjectFile, ".al13");
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

            var filePathReg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Siemens\\Automation\\_InstalledSW\\TIAP13\\TIA_Opns") ??
                              Registry.LocalMachine.OpenSubKey("SOFTWARE\\Siemens\\Automation\\_InstalledSW\\TIAP13\\TIA_Opns");
            if (filePathReg != null)
            {
                string filePath = Path.Combine(filePathReg.GetValue("Path").ToString(), "PublicAPI\\V13");
                if (Directory.Exists(filePath) == false)
                    filePath = Path.Combine(filePathReg.GetValue("Path").ToString(), "PublicAPI\\V13 SP1");
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

        public override ProjectType ProjectType
        {
            get { return ProjectType.Tia13; }
        }

        protected override void LoadProject()
        {
            _projectLoaded = true;
            return;

            ////Stream stream = new FileStream("c:\\tia.xml", FileMode.OpenOrCreate); // new ChunkedMemoryStream();
            ////StreamWriter streamWriter = new StreamWriter(stream);
            
            ////XmlWriter xmlWriter = XmlWriter.Create(streamWriter, new XmlWriterSettings { Indent = true, CheckCharacters = false });

            //var tiaObjectStructure = new TiaObjectStructure();
            //var xmlWriter = new TiaXmlWriter(tiaObjectStructure);

            ////xmlWriter.WriteStartDocument();
            ////xmlWriter.WriteStartElement("root");

            //if (tiaExport == null)
            //{
            //    tiaExportType = Type.GetType("Siemens.Automation.ObjectFrame.FileStorage.Conversion.Export, Siemens.Automation.ObjectFrame.FileStorage");
            //    if (TiaVersion < TiaVersionTypes.V13)
            //    {
            //        tiaExport = tiaExportType.InvokeMember("CreateInstance", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, new object[] { DataFile, true });
            //    }
            //    else
            //    {
            //        //V13
            //        var helper = Type.GetType("Siemens.Automation.ObjectFrame.FileStorage.Base.Client.DataStoreClientHelper, Siemens.Automation.ObjectFrame.FileStorage.Base");
            //        var metaManagerMth = helper.GetMethods(BindingFlags.NonPublic | BindingFlags.Static).FirstOrDefault(x => x.Name == "GetMetaManager");
            //        var metaManager = metaManagerMth.Invoke(null, null);
            //        var crInst = tiaExportType.GetMethods().FirstOrDefault(x => x.Name == "CreateInstance");
            //        tiaExport = crInst.Invoke(null, new object[] { DataFile, null, true, metaManager });                
            //    }

            //    var memMgrType = Type.GetType("Siemens.Automation.ObjectFrame.Kernel.MemoryManager, Siemens.Automation.ObjectFrame.Kernel");
            //    try
            //    {
            //        memMgrType.InvokeMember("Initialize", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, new object[] { 104857600 });                    
            //    }
            //    catch (Exception)
            //    {

            //    }
            //}

            ////tiaExportType.InvokeMember("WriteCultures", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, tiaExport, new object[] { xmlWriter });
            ////tiaExportType.InvokeMember("StartExport", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, tiaExport, new object[] { xmlWriter });
            ////tiaExportType.InvokeMember("WriteRootObjectList", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, tiaExport, new object[] { xmlWriter });

            //if (TiaVersion >= TiaVersionTypes.V13)
            //{
            //    var bgWorker = tiaExportType.GetField("m_BgWorker", BindingFlags.Instance | BindingFlags.NonPublic);
            //    bgWorker.SetValue(tiaExport, new BackgroundWorker() {WorkerReportsProgress = true, WorkerSupportsCancellation = true});
            //}

            //var serializeObjects = tiaExportType.GetMethod("SerializeObjects",
            //    BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null,
            //    new Type[] {typeof (XmlWriter)}, null);

            //serializeObjects.Invoke(tiaExport, new object[] {xmlWriter});
            
            //xmlWriter.Flush();
            //xmlWriter.Close();

            ////streamWriter.Close();
            ////stream.Close();

            ////stream.Position = 0;
            ////ParseProject(stream);            
        }

        //internal Dictionary<string, string> importTypeInfos;
        //internal Dictionary<string, string> asId2Names;
        //internal Dictionary<string, string> relationId2Names;
        //internal string CoreAttributesId;

        //private TIAProjectFolder getProjectFolder(XmlNode Node)
        //{
        //    TIAProjectFolder fld = null;
        //    string id = Node.Attributes["id"].Value;
        //    string instid = Node.Attributes["instId"].Value;

        //    string tiaType = importTypeInfos[id];



        //    switch (tiaType)
        //    {
        //        case "Siemens.Automation.DomainModel.ProjectData":
        //            fld = new TIAProjectFolder(this, Node);                    
        //            break;
        //        case "Siemens.Automation.DomainModel.FolderData":
        //            {
        //                var subType = Node.SelectSingleNode("attribSet[@id='" + CoreAttributesId + "']/attrib[@name='Subtype']").InnerText;
        //                if (subType == "ProgramBlocksFolder" || subType == "ProgramBlocksFolder.Subfolder")
        //                {
        //                    fld = new TIABlocksFolder(this, Node);
        //                }
        //                else
        //                {
        //                    fld = new TIAProjectFolder(this, Node);
        //                }
        //                break;
        //            }
        //        case "Siemens.Simatic.HwConfiguration.Model.DeviceData":
        //            fld = new TIAProjectFolder(this, Node);
        //            break;
        //        case "Siemens.Simatic.HwConfiguration.Model.S7ControllerTargetData":
        //            fld = new TIACPUFolder(this, Node);
        //            break;
        //        case "Siemens.Automation.DomainModel.EAMTZTagTableData":
        //            fld = new TIASymTabFolder(this, Node);
        //            break;
        //        //case "Siemens.Simatic.PlcLanguages.Model.DataBlockData":
        //        //    fld = new TIAProjectFolder(this, Node);
        //        //    break;
        //        default:                    
        //            break;
        //    }

        //    if (fld != null)
        //    {
        //        _allFolders.Add(fld);

        //        var subFolderNodes = xmlDoc.SelectNodes("root/objects/StorageObject[parentlink[@link='" + id + "-" + instid + "']]");

        //        fld.SubNodes = subFolderNodes;

        //        foreach (XmlNode subFolderNode in subFolderNodes)
        //        {
        //            var subFld = this.getProjectFolder(subFolderNode);
        //            if (subFld != null) 
        //                fld.SubItems.Add(subFld);
        //        }
        //    }

        //    return fld;
        //}

        //private void ParseProject(Stream data)
        //{
        //    xmlDoc = new XmlDocument();

        //    xmlDoc.Load(data);

        //    //xmlDoc.Save("C:\\Temp\\tia-export.xml");

        //    importTypeInfos = new Dictionary<string, string>();
        //    foreach (XmlNode typeInfo in xmlDoc.SelectNodes("root/importTypes/typeInfo"))
        //    {
        //        importTypeInfos.Add(typeInfo.Attributes["id"].Value, typeInfo.Attributes["name"].Value);
        //    }

        //    asId2Names = new Dictionary<string, string>();
        //    foreach (XmlNode typeInfo in xmlDoc.SelectNodes("root/asId2Name/typeInfo"))
        //    {
        //        asId2Names.Add(typeInfo.Attributes["id"].Value, typeInfo.Attributes["name"].Value);
        //    }

        //    relationId2Names = new Dictionary<string, string>();
        //    foreach (XmlNode typeInfo in xmlDoc.SelectNodes("root/relationId2Name/typeInfo"))
        //    {
        //        relationId2Names.Add(typeInfo.Attributes["id"].Value, typeInfo.Attributes["name"].Value);
        //    }

        //    CoreAttributesId = asId2Names.FirstOrDefault(itm=>itm.Value=="Siemens.Automation.ObjectFrame.ICoreAttributes").Key;

        //    var nd = xmlDoc.SelectSingleNode("root/rootObjects/entry[@name='Project']");
        //    var prjObjId = nd.Attributes["objectId"].Value;
        //    var projectNode = xmlDoc.SelectSingleNode("root/objects/StorageObject[@instId='" + prjObjId.Split('-')[1] + "']");
        //    ProjectStructure = this.getProjectFolder(projectNode);
        //}
    }
}
