using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;

/*using Siemens.Automation.ObjectFrame.FileStorage.Base.IO;
using Siemens.Automation.ObjectFrame.FileStorage.Client;
using Siemens.Automation.ObjectFrame.FileStorage.Conversion;
using Siemens.Automation.ObjectFrame.Kernel;*/

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles
{
    public class Step7ProjectV11 : Project
    {
        private string DataFile = null;

        private XmlDocument tiaProject;

        public Step7ProjectV11(string projectfile)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += currentDomain_AssemblyResolve;

            ProjectFile = projectfile;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(new FileStream(projectfile, FileMode.Open, FileAccess.Read, System.IO.FileShare.ReadWrite));

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("x", "http://www.siemens.com/2007/07/Automation/CommonServices/DataInfoValueData");

                var nd = xmlDoc.SelectSingleNode("x:Data", nsmgr);
                this.ProjectName = nd.Attributes["Name"].Value;
            }
            catch (Exception) 
            { }

            DataFile = Path.GetDirectoryName(projectfile) + "\\System\\PEData.plf";
            ProjectFolder = projectfile.Substring(0, projectfile.LastIndexOf(Path.DirectorySeparatorChar)) + Path.DirectorySeparatorChar;            
            LoadProject();

            currentDomain.AssemblyResolve -= currentDomain_AssemblyResolve;            
        }

        private XmlDocument xmlDoc;
        
        Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var prg = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            string tiaPath = prg + "\\Siemens\\Automation\\Portal V11\\Bin";
            var dll = args.Name.Split(',')[0];
            var load = Path.Combine(tiaPath, dll + ".dll");
            return Assembly.LoadFrom(load);
        }

        private object tiaExport;
        private Type tiaExportType;

        internal override void LoadProject()
        {
            _projectLoaded = true;
            ProjectStructure = new TIAProjectFolder(this);


            Stream stream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(stream);
            XmlWriter xmlWriter = XmlWriter.Create(streamWriter, new XmlWriterSettings { Indent = true, CheckCharacters = false });

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("root");

            if (tiaExport == null)
            {
                tiaExportType = Type.GetType("Siemens.Automation.ObjectFrame.FileStorage.Conversion.Export, Siemens.Automation.ObjectFrame.FileStorage");
                tiaExport = tiaExportType.InvokeMember("CreateInstance", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, new object[] { DataFile, true });
                //var exp = Export.CreateInstance(DataFile, true);

                var memMgrType = Type.GetType("Siemens.Automation.ObjectFrame.Kernel.MemoryManager, Siemens.Automation.ObjectFrame.Kernel");
                try
                {
                    memMgrType.InvokeMember("Initialize", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, new object[] { 104857600 });
                    //try { MemoryManager.Initialize(104857600); }
                }
                catch (Exception)
                {

                }
            }

            tiaExportType.InvokeMember("WriteCultures", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, tiaExport, new object[] { xmlWriter });
            tiaExportType.InvokeMember("StartExport", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, tiaExport, new object[] { xmlWriter });
            tiaExportType.InvokeMember("WriteRootObjectList", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, tiaExport, new object[] { xmlWriter });
            tiaExportType.InvokeMember("SerializeObjects", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, tiaExport, new object[] { xmlWriter });
            
            xmlWriter.Flush();
            xmlWriter.Close();

            stream.Position = 0;
            var rd = new StreamReader(stream);
            var prj = rd.ReadToEnd();

            ParseProjectString(prj);            
        }

        private void ParseProjectString(string data)
        {
            xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(data);
            var nd = xmlDoc.SelectSingleNode("root/rootObjects/entry[@name='Project']");
            var prjObjId = nd.Attributes["objectId"].Value; 

            var hw = xmlDoc.SelectSingleNode("root/importTypes/typeInfo[@name='Siemens.Simatic.HwConfiguration.Model.DeviceData']");
            var hwConfId = hw.Attributes["id"].Value;

            var s7ControllerTargetDataTypeId = xmlDoc.SelectSingleNode("root/importTypes/typeInfo[@name='Siemens.Simatic.HwConfiguration.Model.S7ControllerTargetData']").Attributes["id"].Value;
            var symbolTableTargetDataTypeId = xmlDoc.SelectSingleNode("root/importTypes/typeInfo[@name='Siemens.Automation.DomainModel.EAMTZTagTableData']").Attributes["id"].Value;
            var symbolTableEntryDataTypeId = xmlDoc.SelectSingleNode("root/importTypes/typeInfo[@name='Siemens.Automation.DomainModel.EAMTZTagData']").Attributes["id"].Value;
            
            var coreAttributesId = xmlDoc.SelectSingleNode("root/asId2Name/typeInfo[@name='Siemens.Automation.ObjectFrame.ICoreAttributes']").Attributes["id"].Value;
            var symAddressAttributesId = xmlDoc.SelectSingleNode("root/asId2Name/typeInfo[@name='Siemens.Automation.DomainModel.ITagAddress']").Attributes["id"].Value;
            var structItemAttributesId = xmlDoc.SelectSingleNode("root/asId2Name/typeInfo[@name='Siemens.Automation.DomainServices.CommonTypeSystem.IStructureItem']").Attributes["id"].Value;
            

            var coreObjectTargetId = xmlDoc.SelectSingleNode("root/relationId2Name/typeInfo[@name='Siemens.Automation.ObjectFrame.CoreObject.Target']").Attributes["id"].Value;
            var tagTableContentDataTagTableId = xmlDoc.SelectSingleNode("root/relationId2Name/typeInfo[@name='Siemens.Automation.DomainModel.TagTableContentData.TagTable']").Attributes["id"].Value;
            
            

            var folderDataType = xmlDoc.SelectSingleNode("root/importTypes/typeInfo[@name='Siemens.Automation.DomainModel.FolderData']");
            var folderDataTypeId = folderDataType.Attributes["id"].Value;
            
            //var s7s = xmlDoc.SelectNodes("root/objects/StorageObject[parentlink[@link='" + prjObjId + "']][@id='" + s7ConfId + "']");


            List<TIACPUFolder> cpus = new List<TIACPUFolder>();

            var plcs = xmlDoc.SelectNodes("root/objects/StorageObject[@id='" + s7ControllerTargetDataTypeId + "']");
            if (plcs != null)
                foreach (XmlNode myPlc in plcs)
                {
                    var akPlc = myPlc.SelectSingleNode("attribSet[@id='" + coreAttributesId + "']/attrib[@name='Name']");
                    var cpuFld = new TIACPUFolder(this) { Name = akPlc.InnerText, ID = myPlc.Attributes["id"].Value, InstID = myPlc.Attributes["instId"].Value };
                    cpus.Add(cpuFld);
                    ProjectStructure.SubItems.Add(cpuFld);
                }

            List<TIASymTabFolder> symTabs = new List<TIASymTabFolder>();

            var symboltables = xmlDoc.SelectNodes("root/objects/StorageObject[@id='" + symbolTableTargetDataTypeId + "']");
            if (symboltables != null)
                foreach (XmlNode mySymTable in symboltables)
                {
                    var akSymTableInfo = mySymTable.SelectSingleNode("attribSet[@id='" + coreAttributesId + "']/attrib[@name='Name']");
                    var symTabFld = new TIASymTabFolder(this) { Name = akSymTableInfo.InnerText, ID = mySymTable.Attributes["id"].Value, InstID = mySymTable.Attributes["instId"].Value };

                    symTabs.Add(symTabFld);
                    
                    var coreObjectTargetInfo = mySymTable.SelectSingleNode("relation[@id='" + coreObjectTargetId + "']/link").InnerText.Split('-');
                    if (coreObjectTargetInfo[0] == s7ControllerTargetDataTypeId)
                    {
                        var akCpu = cpus.FirstOrDefault(itm => itm.InstID == coreObjectTargetInfo[1]);
                        if (akCpu != null) 
                            akCpu.SubItems.Add(symTabFld);
                    }                    
                }

            

            var symboltableEntrys = xmlDoc.SelectNodes("root/objects/StorageObject[@id='" + symbolTableEntryDataTypeId + "']");
            if (symboltableEntrys != null)
                foreach (XmlNode mySymTableEntry in symboltableEntrys)
                {
                    var akSymName = mySymTableEntry.SelectSingleNode("attribSet[@id='" + coreAttributesId + "']/attrib[@name='Name']").InnerText;
                    var akSymAddress = mySymTableEntry.SelectSingleNode("attribSet[@id='" + symAddressAttributesId + "']/attrib[@name='LogicalAddress']").InnerText;
                    var akSymType = mySymTableEntry.SelectSingleNode("attribSet[@id='" + structItemAttributesId + "']/attrib[@name='DisplayTypeName']").InnerText;

                    var entry = new SymbolTableEntry() { Symbol = akSymName, OperandIEC = akSymAddress, DataType = akSymType };
                    var tagTableContentDataTagTableInfo = mySymTableEntry.SelectSingleNode("parentlink[@relId='" + tagTableContentDataTagTableId + "']").Attributes["link"].Value.Split('-');
                    if (tagTableContentDataTagTableInfo[0] == symbolTableTargetDataTypeId)
                    {
                        var akSymTab = symTabs.FirstOrDefault(itm => itm.InstID == tagTableContentDataTagTableInfo[1]);
                        if (akSymTab != null)
                            akSymTab.SymbolTableEntrys.Add(entry);
                    }
                }

        }


    }
}
