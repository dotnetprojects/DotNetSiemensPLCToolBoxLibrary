using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

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

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(new FileStream(projectfile, FileMode.Open, FileAccess.Read, System.IO.FileShare.ReadWrite));

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("x", "http://www.siemens.com/2007/07/Automation/CommonServices/DataInfoValueData");

            var nd = xmlDoc.SelectSingleNode("x:Data",nsmgr);
            this.ProjectName = nd.Attributes["Name"].Value;


            DataFile = Path.GetDirectoryName(projectfile) + "\\System\\PEData.plf";
            ProjectFolder = projectfile.Substring(0, projectfile.LastIndexOf(Path.DirectorySeparatorChar)) + Path.DirectorySeparatorChar;            
            LoadProject();

            currentDomain.AssemblyResolve -= currentDomain_AssemblyResolve;            
        }
        
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
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(data);
            var nd = xmlDoc.SelectSingleNode("root/rootObjects/entry[@name='Project']");
            var prjObjId = nd.Attributes["objectId"].Value; 

            var hw = xmlDoc.SelectSingleNode("root/importTypes/typeInfo[@name='Siemens.Simatic.HwConfiguration.Model.DeviceData']");
            var hwConfId = hw.Attributes["id"].Value;


            var coreAttributesId = xmlDoc.SelectSingleNode("root/asId2Name/typeInfo[@name='Siemens.Automation.ObjectFrame.ICoreAttributes']").Attributes["id"].Value;
            

            var s7 = xmlDoc.SelectSingleNode("root/importTypes/typeInfo[@name='Siemens.Simatic.HwConfiguration.Model.S7ControllerTargetData']");
            var s7ConfId = s7.Attributes["id"].Value;

            
            //var s7s = xmlDoc.SelectNodes("root/objects/StorageObject[parentlink[@link='" + prjObjId + "']][@id='" + s7ConfId + "']");
            var s7s = xmlDoc.SelectNodes("root/objects/StorageObject[@id='" + s7ConfId + "']");

            if (s7s != null)
                foreach (XmlNode mys7 in s7s)
                {
                    var akS7 = mys7.SelectSingleNode("attribSet[@id='" + coreAttributesId + "']/attrib[@name='Name']");
                    ProjectStructure.SubItems.Add(new TIACPUFolder(this){ Name = akS7.InnerText });
                }            
        }


    }
}
