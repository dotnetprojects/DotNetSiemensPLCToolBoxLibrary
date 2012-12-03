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

            var xmlRead = XmlReader.Create(new FileStream(projectfile, FileMode.Open, FileAccess.Read, System.IO.FileShare.ReadWrite));
            
            DataFile = Path.GetDirectoryName(projectfile) + "\\System\\PEData.plf";
            ProjectFolder = projectfile.Substring(0, projectfile.LastIndexOf(Path.DirectorySeparatorChar)) + Path.DirectorySeparatorChar;            
            LoadProject();

            currentDomain.AssemblyResolve -= currentDomain_AssemblyResolve;

            //Needed Assemblys....
            //assemblyref://Siemens.Automation.ObjectFrame.FileStorage&assemblyref://Siemens.Automation.ObjectFrame.FileStorage.Base&assemblyref://Siemens.Automation.ObjectFrame.Kernel
        }
        
        Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string tiaPath = "C:\\Program Files (x86)\\Siemens\\Automation\\Portal V11\\Bin";
            var dll = args.Name.Split(',')[0];
            var load = Path.Combine(tiaPath, dll + ".dll");
            return Assembly.LoadFrom(load);
        }

        internal override void LoadProject()
        {
           /* _projectLoaded = true;
            ProjectStructure = new TIAProjectFolder(this);


            Stream stream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(stream);
            XmlWriter xmlWriter = XmlWriter.Create(streamWriter, new XmlWriterSettings { Indent = true, CheckCharacters = false });

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("root");
            
            var exp = Export.CreateInstance(DataFile, true);

            try { MemoryManager.Initialize(104857600); }
            catch (Exception)
            { }
            
            typeof(Export).InvokeMember("WriteCultures", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, (Export)exp, new object[] { xmlWriter });
            exp.GetType().InvokeMember("StartExport", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, exp, new object[] { xmlWriter });
            typeof(Export).InvokeMember("WriteRootObjectList", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, (Export)exp, new object[] { xmlWriter });
            typeof(Export).InvokeMember("SerializeObjects", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, exp, new object[] { xmlWriter });
            
            xmlWriter.Flush();
            xmlWriter.Close();

            stream.Position = 0;
            var rd = new StreamReader(stream);
            var prj = rd.ReadToEnd();

            ParseProjectString(prj);     */       
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
